using RbsMain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RbsMain
{
    #region デリゲート定義
    //別スレッドからTrace.WriteLineにアクセスするデリゲート
    public delegate void dlgAccTrace(string text);
    #endregion

    public partial class RbsMain : Form
    {
        private int timerRBSD010cnt = 0;
        private int timerRBSD020cnt = 0;

        #region オブジェクト定義
        private CmnObject CmnObj;
        private 端数在庫実績 端数在庫実績ThreadObj;
        private 投入実績 投入実績ThreadObj;
        private 投入NG実績 投入NG実績ThreadObj;
        private 正梱投入指示 正梱投入指示ThreadObj;
        private 端数投入指示 端数投入指示ThreadObj;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RbsMain()
        {
            InitializeComponent();

            #region バージョン取得
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            System.Version ver = asm.GetName().Version;
            this.Text = "RBSインターフェース（ver " + ver.ToString() + "）";
            #endregion

            #region オブジェクト生成
            CmnObj = new CmnObject();
            CmnObj.InitAppConfig();
            #endregion

            #region ログ定義
            var dailyLogging = new DailyLoggingTraceListener()
            {
                OutputDirectory = Application.StartupPath + @"\Log\",   // 既定値 Application.StartupPath
                FileNameFormat = "{0:yyyyMMdd}_{1}.log",                // 既定値 {0:yyyyMMdd}_{1}.txt
                DatetimeFormat = "{0:MM/dd HH:mm:ss}",                  // 既定値 {0:MM/dd HH:mm:ss}
                DBcmndString =
                        "Data Source=" + CmnObj.DB_DataSource +
                        ";Initial Catalog=" + CmnObj.DB_InitialCatalog +
                        ";User Id=" + CmnObj.DB_UserId +
                        ";Password=" + CmnObj.DB_Password,
            };
            Trace.Listeners.Add(dailyLogging);
            Trace.Listeners.Add(new TextBoxTraceListener(textBox1));
            DateTime now = DateTime.Now;
            CmnObj.LastLogDate = now.Date;
            #endregion

            #region データベース接続確認
            for (int i = 0; ; i++)
            {
                Thread.Sleep(1000);
                Application.DoEvents();

                if (CmnObj.DBopenCheck() != "")
                {
                    if (i > 60)
                    {
                        Trace.WriteLine("起動時データベース接続異常");
                        i = 0;
                    }
                }
                else
                {
                    break;
                }
            }
            #endregion

            #region データテーブル生成
            if (CmnObj.SelectTableDef() != "")
                Trace.WriteLine("データベースアクセス異常！");
            CmnObj.CreateDataTable(CmnObj);
            #endregion

            #region スレッド生成

            #region 端数在庫実績スレッド生成
            端数在庫実績ThreadObj = new 端数在庫実績(this, CmnObj);
            Thread 端数在庫実績Thread = new Thread(new ThreadStart(端数在庫実績ThreadObj.端数在庫実績Func));
            端数在庫実績Thread.IsBackground = true;
            端数在庫実績Thread.Start();
            #endregion

            #region 投入実績スレッド生成
            投入実績ThreadObj = new 投入実績(this, CmnObj);
            Thread 投入実績Thread = new Thread(new ThreadStart(投入実績ThreadObj.投入実績Func));
            投入実績Thread.IsBackground = true;
            投入実績Thread.Start();
            #endregion

            #region 投入NG実績スレッド生成
            投入NG実績ThreadObj = new 投入NG実績(this, CmnObj);
            Thread 投入NG実績Thread = new Thread(new ThreadStart(投入NG実績ThreadObj.投入NG実績Func));
            投入NG実績Thread.IsBackground = true;
            投入NG実績Thread.Start();
            #endregion

            #region 正梱投入指示スレッド生成
            正梱投入指示ThreadObj = new 正梱投入指示(this, CmnObj);
            Thread 正梱投入指示Thread = new Thread(new ThreadStart(正梱投入指示ThreadObj.正梱投入指示Func));
            正梱投入指示Thread.IsBackground = true;
            正梱投入指示Thread.Start();
            #endregion

            #region 端数投入指示スレッド生成
            端数投入指示ThreadObj = new 端数投入指示(this, CmnObj);
            Thread 端数投入指示Thread = new Thread(new ThreadStart(端数投入指示ThreadObj.端数投入指示Func));
            端数投入指示Thread.IsBackground = true;
            端数投入指示Thread.Start();
            #endregion

            #endregion

            #region 監視タイマー起動
            timerログ監視.Interval = 60000;
            timerログ監視.Enabled = true;
            timer集配信監視.Interval = CmnObj.HULFT集配信監視間隔;
            timer集配信監視.Enabled = true;
            timerRBSD010.Interval = CmnObj.正梱投入指示監視間隔;
            timerRBSD010.Enabled = true;
            timerRBSD020.Interval = CmnObj.端数投入指示監視間隔;
            timerRBSD020.Enabled = true;
            #endregion

            Trace.WriteLine("RBSインターフェース（ver " + ver.ToString() + "）を起動しました");
        }

        /// <summary>
        /// 別スレッドからTrace.WriteLineにアクセスするデリゲート
        /// </summary>
        public void AccTrace(string text)
        {
            Trace.WriteLine(text);
        }

        /// <summary>
        /// xを押された時のフォームの終了
        /// </summary>
        private void RbsClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("RBSインターフェースを終了してよろしいですか？",
                "終了確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification)
                == DialogResult.No)
            {
                //イベントをキャンセルするかどうかの確認のため DialogResult.No にて
                //クローズイベントをキャンセルしない、つまり、クローズする
                e.Cancel = true;
            }
            else
            {
                #region 監視タイマー停止
                timerログ監視.Enabled = false;
                timer集配信監視.Enabled = false;
                timerRBSD010.Enabled = false;
                timerRBSD020.Enabled = false;
                #endregion

                #region スレッドへ終了通知

                #region 端数在庫実績スレッドへ終了通知
                try
                {
                    lock (CmnObj.端数在庫実績Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.端数在庫実績Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.端数在庫実績Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("端数在庫実績キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #region 投入実績スレッドへ終了通知
                try
                {
                    lock (CmnObj.投入実績Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.投入実績Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.投入実績Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("投入実績キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #region 投入NG実績スレッドへ終了通知
                try
                {
                    lock (CmnObj.投入NG実績Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.投入NG実績Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.投入NG実績Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("投入NG実績キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #region 正梱投入指示スレッドへ終了通知
                try
                {
                    lock (CmnObj.正梱投入指示Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.正梱投入指示Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.正梱投入指示Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("正梱投入指示キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #region 端数投入指示スレッドへ終了通知
                try
                {
                    lock (CmnObj.端数投入指示Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.端数投入指示Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.端数投入指示Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("端数投入指示キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #endregion

                #region スレッド終了確認
                for (; ; )
                {
                    Thread.Sleep(100);
                    Application.DoEvents();

                    if (CmnObj.端数在庫実績スレッド終了フラグ == true &&
                        CmnObj.投入実績スレッド終了フラグ == true &&
                        CmnObj.投入NG実績スレッド終了フラグ == true &&
                        CmnObj.正梱投入指示スレッド終了フラグ == true &&
                        CmnObj.端数投入指示スレッド終了フラグ == true)
                    {
                        break;
                    }
                }
                #endregion

                Trace.WriteLine("RBSインターフェースを終了しました");

                this.FormClosing -= new FormClosingEventHandler(this.RbsClosing);
                Close();
            }
        }

        /// <summary>
        /// ログ監視
        /// </summary>
        private void timerログ監視_Tick(object sender, EventArgs e)
        {
            timerログ監視.Enabled = false;

            #region ログ表示をクリア
            DateTime now = DateTime.Now;
            if (CmnObj.LastLogDate != now.Date)
            {
                textBox1.Clear();

                #region 過去ログファイル削除
                DirectoryInfo dilog;
                FileInfo[] fileslog;
                try
                {
                    DateTime pastlog = DateTime.Now.AddDays(-30);
                    dilog = new DirectoryInfo(Application.StartupPath + @"\Log");
                    fileslog = dilog.GetFiles("*.log");
                    foreach (System.IO.FileInfo f in fileslog)
                    {
                        if (int.Parse(pastlog.ToString("yyyyMMdd")) > int.Parse(f.Name.Substring(0, 8)))
                        {
                            File.Delete(f.FullName);
                        }
                    }
                }
                catch
                {
                }
                #endregion
            }
            CmnObj.LastLogDate = now.Date;
            #endregion

            timerログ監視.Enabled = true;
        }

        /// <summary>
        /// 集配信監視
        /// </summary>
        private void timer集配信監視_Tick(object sender, EventArgs e)
        {
            timer集配信監視.Enabled = false;

            #region HULFT集配信実行後ジョブからの通知
            DirectoryInfo di;
            FileInfo[] files;
            string[] ReqData;
            string fileID;
            string timestamp;
            string status;
            string message;

            di = new DirectoryInfo(CmnObj.RequestPass);
            files = di.GetFiles(CmnObject.HULFT要求拡張子);
            foreach (System.IO.FileInfo f in files)
            {
                // FILEID_タイムスタンプ_HULFT完了コード
                // MTLD010_yyyyMMddHHmmss_0.REQ
                string fname = Path.GetFileNameWithoutExtension(f.Name);
                ReqData = fname.Split('_');
                fileID = ReqData[0];
                timestamp = ReqData[1];
                status = ReqData[2];

                #region HULFT要求ファイル削除
                for (; ; )
                {
                    try
                    {
                        File.Delete(f.FullName);
                        break;
                    }
                    catch
                    {
                        Thread.Sleep(100);
                        Application.DoEvents();
                    }
                }
                #endregion

                if (status == CmnObject.HULFT完了コード正常)
                {
                    if (fileID == CmnObj.HulftID端数在庫実績)
                    {
                        #region 端数在庫実績スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.端数在庫実績Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_展開要求 + CmnObject.カンマ + timestamp;
                                CmnObj.端数在庫実績Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.端数在庫実績Event.Set();

                            Trace.WriteLine("-----------------------------------------------------------------");
                            message = timestamp + "_" + CmnObj.HulftID端数在庫実績 + ".csv";
                            Trace.WriteLine("端数在庫実績：1.集信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("端数在庫実績キューへのセット例外：" + ex.Message.ToString());
                        }
                        #endregion
                    }
                    else if (fileID == CmnObj.HulftID投入実績)
                    {
                        #region 投入実績スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.投入実績Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_展開要求 + CmnObject.カンマ + timestamp;
                                CmnObj.投入実績Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.投入実績Event.Set();

                            Trace.WriteLine("-----------------------------------------------------------------");
                            message = timestamp + "_" + CmnObj.HulftID投入実績 + ".csv";
                            Trace.WriteLine("投入実績：1.集信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("投入実績キューへのセット例外：" + ex.Message.ToString());
                        }
                        #endregion
                    }
                    else if (fileID == CmnObj.HulftID投入NG実績)
                    {
                        #region 投入NG実績スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.投入NG実績Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_展開要求 + CmnObject.カンマ + timestamp;
                                CmnObj.投入NG実績Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.投入NG実績Event.Set();

                            Trace.WriteLine("-----------------------------------------------------------------");
                            message = timestamp + "_" + CmnObj.HulftID投入NG実績 + ".csv";
                            Trace.WriteLine("投入NG実績：1.集信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("投入NG実績キューへのセット例外：" + ex.Message.ToString());
                        }
                        #endregion
                    }
                    else if (fileID == CmnObj.HulftID正梱投入指示)
                    {
                        #region 正梱投入指示スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.正梱投入指示Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_抽出ファイルバックアップ要求 + CmnObject.カンマ + timestamp;
                                CmnObj.正梱投入指示Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.正梱投入指示Event.Set();
                            message = timestamp + "_" + CmnObj.HulftID正梱投入指示 + ".csv";
                            Trace.WriteLine("正梱投入指示：7.配信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("正梱投入指示キューへのセット例外：" + ex.Message.ToString());
                        }
                        #endregion
                    }
                    else if (fileID == CmnObj.HulftID端数投入指示)
                    {
                        #region 端数投入指示スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.端数投入指示Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_抽出ファイルバックアップ要求 + CmnObject.カンマ + timestamp;
                                CmnObj.端数投入指示Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.端数投入指示Event.Set();
                            message = timestamp + "_" + CmnObj.HulftID端数投入指示 + ".csv";
                            Trace.WriteLine("端数投入指示：7.配信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("端数投入指示キューへのセット例外：" + ex.Message.ToString());
                        }
                        #endregion
                    }
                    else
                    {
                        message = "HULFT集配信異常（ID不正=" + fileID + "）";
                        Trace.WriteLine(message);
                    }
                }
                else
                {
                    #region HULFT集配信異常
                    if (fileID == CmnObj.HulftID端数在庫実績)
                    {
                        message = "HULFT集信異常（端数在庫実績）：" + status;
                    }
                    else if (fileID == CmnObj.HulftID投入実績)
                    {
                        message = "HULFT集信異常（投入実績）：" + status;
                    }
                    else if (fileID == CmnObj.HulftID投入NG実績)
                    {
                        message = "HULFT集信異常（投入NG実績）：" + status;
                    }
                    else if (fileID == CmnObj.HulftID正梱投入指示)
                    {
                        message = "HULFT配信異常（正梱投入指示）：" + status;
                    }
                    else if (fileID == CmnObj.HulftID端数投入指示)
                    {
                        message = "HULFT配信異常（端数投入指示）：" + status;
                    }
                    else
                    {
                        message = "HULFT集配信異常（ID不正=" + fileID + "）：" + status;
                    }
                    Trace.WriteLine(message);
                    #endregion
                }
            }
            #endregion

            timer集配信監視.Enabled = true;
        }

        /// <summary>
        /// 正梱投入指示監視
        /// </summary>
        private void timerRBSD010_Tick(object sender, EventArgs e)
        {
            if (CmnObj.正梱投入指示処理中フラグ == true)
            {
                return;
            }
            string 配信ファイル = CmnObj.配信フォルダ + CmnObj.HulftID正梱投入指示 + ".csv";
            if (File.Exists(配信ファイル))
            {
                timerRBSD010cnt++;
                if (timerRBSD010cnt > 60)
                {
                    Trace.WriteLine("正梱投入指示：配信ファイル存在中につき配信処理中止");
                    timerRBSD010cnt = 0;
                }
                return;
            }

            timerRBSD010.Enabled = false;

            #region 正梱投入指示スレッドへ処理通知
            try
            {
                lock (CmnObj.正梱投入指示Queue)    // 排他ロックを取得
                {
                    string QueueData = CmnObject.QUE_抽出要求;
                    CmnObj.正梱投入指示Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                }
                CmnObj.正梱投入指示Event.Set();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("正梱投入指示キューへのセット例外：" + ex.Message.ToString());
                return;
            }
            #endregion

            timerRBSD010.Enabled = true;
        }

        /// <summary>
        /// 端数投入指示監視
        /// </summary>
        private void timerRBSD020_Tick(object sender, EventArgs e)
        {
            if (CmnObj.端数投入指示処理中フラグ == true)
            {
                return;
            }
            string 配信ファイル = CmnObj.配信フォルダ + CmnObj.HulftID端数投入指示 + ".csv";
            if (File.Exists(配信ファイル))
            {
                timerRBSD020cnt++;
                if (timerRBSD020cnt > 60)
                {
                    Trace.WriteLine("端数投入指示：配信ファイル存在中につき配信処理中止");
                    timerRBSD020cnt = 0;
                }
                return;
            }

            timerRBSD020.Enabled = false;

            #region 端数投入指示スレッドへ処理通知
            try
            {
                lock (CmnObj.端数投入指示Queue)    // 排他ロックを取得
                {
                    string QueueData = CmnObject.QUE_抽出要求;
                    CmnObj.端数投入指示Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                }
                CmnObj.端数投入指示Event.Set();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("端数投入指示キューへのセット例外：" + ex.Message.ToString());
                return;
            }
            #endregion

            timerRBSD020.Enabled = true;
        }
    }
}

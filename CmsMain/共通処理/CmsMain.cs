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

namespace CmsMain
{
    #region デリゲート定義
    //別スレッドからTrace.WriteLineにアクセスするデリゲート
    public delegate void dlgAccTrace(string text);
    #endregion

    public partial class CmsMain : Form
    {
        private int timerMTLU010cnt = 0;
        private int timerMTLU020cnt = 0;

        #region オブジェクト定義
        private CmnObject CmnObj;
        private パレット入庫指示 パレット入庫指示ThreadObj;
        private 原料在庫情報 原料在庫情報ThreadObj;
        private 原料自動投入指図 原料自動投入指図ThreadObj;
        private パレット出庫指図 パレット出庫指図ThreadObj;
        private 原料自動投入開始 原料自動投入開始ThreadObj;
        private 原料自動投入実績 原料自動投入実績ThreadObj;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CmsMain()
        {
            InitializeComponent();

            #region バージョン取得
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            System.Version ver = asm.GetName().Version;
            this.Text = "CMSインターフェース（ver " + ver.ToString() + "）";
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
            for(int i = 0; ; i++ )
            {
                Thread.Sleep(1000);
                Application.DoEvents();

                if (CmnObj.DBopenCheck() != "")
                {
                    if(i > 60)
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

            #region パレット入庫指示スレッド生成
            パレット入庫指示ThreadObj = new パレット入庫指示(this, CmnObj);
            Thread パレット入庫指示Thread = new Thread(new ThreadStart(パレット入庫指示ThreadObj.パレット入庫指示Func));
            パレット入庫指示Thread.IsBackground = true;
            パレット入庫指示Thread.Start();
            #endregion

            #region 原料在庫情報スレッド生成
            原料在庫情報ThreadObj = new 原料在庫情報(this, CmnObj);
            Thread 原料在庫情報Thread = new Thread(new ThreadStart(原料在庫情報ThreadObj.原料在庫情報Func));
            原料在庫情報Thread.IsBackground = true;
            原料在庫情報Thread.Start();
            #endregion

            #region 原料自動投入指図スレッド生成
            原料自動投入指図ThreadObj = new 原料自動投入指図(this, CmnObj);
            Thread 原料自動投入指図Thread = new Thread(new ThreadStart(原料自動投入指図ThreadObj.原料自動投入指図Func));
            原料自動投入指図Thread.IsBackground = true;
            原料自動投入指図Thread.Start();
            #endregion

            #region パレット出庫指図スレッド生成
            パレット出庫指図ThreadObj = new パレット出庫指図(this, CmnObj);
            Thread パレット出庫指図Thread = new Thread(new ThreadStart(パレット出庫指図ThreadObj.パレット出庫指図Func));
            パレット出庫指図Thread.IsBackground = true;
            パレット出庫指図Thread.Start();
            #endregion

            #region 原料自動投入開始スレッド生成
            原料自動投入開始ThreadObj = new 原料自動投入開始(this, CmnObj);
            Thread 原料自動投入開始Thread = new Thread(new ThreadStart(原料自動投入開始ThreadObj.原料自動投入開始Func));
            原料自動投入開始Thread.IsBackground = true;
            原料自動投入開始Thread.Start();
            #endregion

            #region 原料自動投入実績スレッド生成
            原料自動投入実績ThreadObj = new 原料自動投入実績(this, CmnObj);
            Thread 原料自動投入実績Thread = new Thread(new ThreadStart(原料自動投入実績ThreadObj.原料自動投入実績Func));
            原料自動投入実績Thread.IsBackground = true;
            原料自動投入実績Thread.Start();
            #endregion

            #endregion

            #region 監視タイマー起動
            timerログ監視.Interval = 60000;
            timerログ監視.Enabled = true;
            timer集配信監視.Interval = CmnObj.HULFT集配信監視間隔;
            timer集配信監視.Enabled = true;
            timerMTLU010.Interval = CmnObj.原料自動投入開始監視間隔;
            timerMTLU010.Enabled = true;
            timerMTLU020.Interval = CmnObj.原料自動投入実績監視間隔;
            timerMTLU020.Enabled = true;
            #endregion

            Trace.WriteLine("CMSインターフェース（ver " + ver.ToString() + "）を起動しました");
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
        private void CmsClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("CMSインターフェースを終了してよろしいですか？",
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
                timerMTLU010.Enabled = false;
                timerMTLU020.Enabled = false;
                #endregion

                #region スレッドへ終了通知

                #region パレット入庫指示スレッドへ終了通知
                try
                {
                    lock (CmnObj.パレット入庫指示Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.パレット入庫指示Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.パレット入庫指示Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("パレット入庫指示キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #region 原料在庫情報スレッドへ終了通知
                try
                {
                    lock (CmnObj.原料在庫情報Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.原料在庫情報Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.原料在庫情報Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("原料在庫情報キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #region 原料自動投入指図スレッドへ終了通知
                try
                {
                    lock (CmnObj.原料自動投入指図Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.原料自動投入指図Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.原料自動投入指図Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("原料自動投入指図キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #region パレット出庫指図スレッドへ終了通知
                try
                {
                    lock (CmnObj.パレット出庫指図Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.パレット出庫指図Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.パレット出庫指図Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("パレット出庫指図キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #region 原料自動投入開始スレッドへ終了通知
                try
                {
                    lock (CmnObj.原料自動投入開始Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.原料自動投入開始Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.原料自動投入開始Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("原料自動投入開始キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #region 原料自動投入実績スレッドへ終了通知
                try
                {
                    lock (CmnObj.原料自動投入実績Queue)    // 排他ロックを取得
                    {
                        string QueueData = CmnObject.QUE_スレッド終了;

                        CmnObj.原料自動投入実績Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                    }
                    CmnObj.原料自動投入実績Event.Set();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("原料自動投入実績キューへのセット例外" + ex.Message.ToString());
                }
                #endregion

                #endregion

                #region スレッド終了確認
                for (; ; )
                {
                    Thread.Sleep(100);
                    Application.DoEvents();

                    if (CmnObj.パレット入庫指示スレッド終了フラグ == true &&
                        CmnObj.原料在庫情報スレッド終了フラグ == true &&
                        CmnObj.原料自動投入指図スレッド終了フラグ == true &&
                        CmnObj.パレット出庫指図スレッド終了フラグ == true &&
                        CmnObj.原料自動投入開始スレッド終了フラグ == true &&
                        CmnObj.原料自動投入実績スレッド終了フラグ == true)
                    {
                        break;
                    }
                }
                #endregion

                Trace.WriteLine("CMSインターフェースを終了しました");

                this.FormClosing -= new FormClosingEventHandler(this.CmsClosing);
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
                    if (fileID == CmnObj.HulftIDパレット入庫指示)
                    {
                        #region パレット入庫指示スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.パレット入庫指示Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_展開要求 + CmnObject.カンマ + timestamp;
                                CmnObj.パレット入庫指示Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.パレット入庫指示Event.Set();

                            Trace.WriteLine("-----------------------------------------------------------------");
                            message = timestamp + "_" + CmnObj.HulftIDパレット入庫指示 + ".csv";
                            Trace.WriteLine("パレット入庫指示：1.集信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("パレット入庫指示キューへのセット例外：" + ex.Message.ToString());
                        }
                        #endregion
                    }
                    else if (fileID == CmnObj.HulftID原料在庫情報)
                    {
                        #region 原料在庫情報スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.原料在庫情報Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_展開要求 + CmnObject.カンマ + timestamp;
                                CmnObj.原料在庫情報Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.原料在庫情報Event.Set();

                            Trace.WriteLine("-----------------------------------------------------------------");
                            message = timestamp + "_" + CmnObj.HulftID原料在庫情報 + ".csv";
                            Trace.WriteLine("原料在庫情報：1.集信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("原料在庫情報キューへのセット例外：" + ex.Message.ToString());
                        }
                        #endregion
                    }
                    else if (fileID == CmnObj.HulftID原料自動投入指図)
                    {
                        #region 原料自動投入指図スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.原料自動投入指図Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_展開要求 + CmnObject.カンマ + timestamp;
                                CmnObj.原料自動投入指図Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.原料自動投入指図Event.Set();

                            Trace.WriteLine("-----------------------------------------------------------------");
                            message = timestamp + "_" + CmnObj.HulftID原料自動投入指図 + ".csv";
                            Trace.WriteLine("原料自動投入指図：1.集信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("原料自動投入指図キューへのセット例外：" + ex.Message.ToString());
                        }
                        #endregion
                    }
                    else if (fileID == CmnObj.HulftIDパレット出庫指図)
                    {
                        #region パレット出庫指図スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.パレット出庫指図Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_展開要求 + CmnObject.カンマ + timestamp;
                                CmnObj.パレット出庫指図Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.パレット出庫指図Event.Set();

                            Trace.WriteLine("-----------------------------------------------------------------");
                            message = timestamp + "_" + CmnObj.HulftIDパレット出庫指図 + ".csv";
                            Trace.WriteLine("パレット出庫指図：1.集信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("パレット出庫指図キューへのセット例外：" + ex.Message.ToString());
                        }
                        #endregion
                    }
                    else if (fileID == CmnObj.HulftID原料自動投入開始)
                    {
                        #region 原料自動投入開始スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.原料自動投入開始Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_抽出ファイルバックアップ要求 + CmnObject.カンマ + timestamp;
                                CmnObj.原料自動投入開始Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.原料自動投入開始Event.Set();
                            message = timestamp + "_" + CmnObj.HulftID原料自動投入開始 + ".csv";
                            Trace.WriteLine("原料自動投入開始：7.配信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("原料自動投入開始キューへのセット例外：" + ex.Message.ToString());
                        }
                        #endregion
                    }
                    else if (fileID == CmnObj.HulftID原料自動投入実績)
                    {
                        #region 原料自動投入実績スレッドへ処理通知
                        try
                        {
                            lock (CmnObj.原料自動投入実績Queue)    // 排他ロックを取得
                            {
                                string QueueData = CmnObject.QUE_抽出ファイルバックアップ要求 + CmnObject.カンマ + timestamp;
                                CmnObj.原料自動投入実績Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                            }
                            CmnObj.原料自動投入実績Event.Set();
                            message = timestamp + "_" + CmnObj.HulftID原料自動投入実績 + ".csv";
                            Trace.WriteLine("原料自動投入実績：7.配信完了 [" + message + "]");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("原料自動投入実績キューへのセット例外：" + ex.Message.ToString());
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
                    if (fileID == CmnObj.HulftIDパレット入庫指示)
                    {
                        message = "HULFT集信異常（パレット入庫指示）：" + status;
                    }
                    else if (fileID == CmnObj.HulftID原料在庫情報)
                    {
                        message = "HULFT集信異常（原料在庫情報）：" + status;
                    }
                    else if (fileID == CmnObj.HulftID原料自動投入指図)
                    {
                        message = "HULFT集信異常（原料自動投入指図）：" + status;
                    }
                    else if (fileID == CmnObj.HulftIDパレット出庫指図)
                    {
                        message = "HULFT集信異常（パレット出庫指図）：" + status;
                    }
                    else if (fileID == CmnObj.HulftID原料自動投入開始)
                    {
                        message = "HULFT配信異常（原料自動投入開始）：" + status;
                    }
                    else if (fileID == CmnObj.HulftID原料自動投入実績)
                    {
                        message = "HULFT配信異常（原料自動投入実績）：" + status;
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
        /// 原料自動投入開始監視
        /// </summary>
        private void timerMTLU010_Tick(object sender, EventArgs e)
        {
            if (CmnObj.原料自動投入開始処理中フラグ == true)
            {
                return;
            }
            string 配信ファイル = CmnObj.配信フォルダ + CmnObj.HulftID原料自動投入開始 + ".csv";
            if (File.Exists(配信ファイル))
            {
                timerMTLU010cnt++;
                if(timerMTLU010cnt > 60)
                {
                    Trace.WriteLine("原料自動投入開始：配信ファイル存在中につき配信処理中止");
                    timerMTLU010cnt = 0;
                }
                return;
            }

            timerMTLU010.Enabled = false;

            #region 原料自動投入開始スレッドへ処理通知
            try
            {
                lock (CmnObj.原料自動投入開始Queue)    // 排他ロックを取得
                {
                    string QueueData = CmnObject.QUE_抽出要求;
                    CmnObj.原料自動投入開始Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                }
                CmnObj.原料自動投入開始Event.Set();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("原料自動投入開始キューへのセット例外：" + ex.Message.ToString());
                return;
            }
            #endregion

            timerMTLU010.Enabled = true;
        }

        /// <summary>
        /// 原料自動投入実績監視
        /// </summary>
        private void timerMTLU020_Tick(object sender, EventArgs e)
        {
            if (CmnObj.原料自動投入実績処理中フラグ == true)
            {
                return;
            }
            string 配信ファイル = CmnObj.配信フォルダ + CmnObj.HulftID原料自動投入実績 + ".csv";
            if (File.Exists(配信ファイル))
            {
                timerMTLU020cnt++;
                if (timerMTLU020cnt > 60)
                {
                    Trace.WriteLine("原料自動投入実績：配信ファイル存在中につき配信処理中止");
                    timerMTLU020cnt = 0;
                }
                return;
            }

            timerMTLU020.Enabled = false;

            #region 原料自動投入実績スレッドへ処理通知
            try
            {
                lock (CmnObj.原料自動投入実績Queue)    // 排他ロックを取得
                {
                    string QueueData = CmnObject.QUE_抽出要求;
                    CmnObj.原料自動投入実績Queue.Enqueue(QueueData);    // キューにセット（先入先出）
                }
                CmnObj.原料自動投入実績Event.Set();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("原料自動投入実績キューへのセット例外：" + ex.Message.ToString());
                return;
            }
            #endregion

            timerMTLU020.Enabled = true;
        }
    }
}

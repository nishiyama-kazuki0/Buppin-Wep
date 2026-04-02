using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Threading;

namespace RbsMain
{
    internal static class Program
    {
        static private System.Threading.Mutex _mutex;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Mutexクラスの作成
            _mutex = new System.Threading.Mutex(false, @"Global\RBSIFAPP");

            //ミューテックスの所有権を要求する
            if (_mutex.WaitOne(0, false) == false)
            {
                //すでに起動していると判断する
                MessageBox.Show("多重起動はできません。", "RBSインターフェース起動失敗",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.ServiceNotification);
            }
            else
            {
                // ThreadExceptionイベントハンドラを登録する
                Application.ThreadException += new
                  ThreadExceptionEventHandler(Application_ThreadException);

                // UnhandledExceptionイベントハンドラを登録する
                Thread.GetDomain().UnhandledException += new
                  UnhandledExceptionEventHandler(Application_UnhandledException);

                //アプリケーション起動
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new RbsMain());

                //ミューテックスを解放する
                _mutex.ReleaseMutex();
            }

            //ミューテックスを破棄する
            _mutex.Close();
        }

        // 未処理例外をキャッチするイベントハンドラ
        // Windowsアプリケーション用
        public static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ShowErrorMessage(e.Exception, "Application_ThreadExceptionによる例外通知です。");
        }

        // 未処理例外をキャッチするイベントハンドラ
        // 主にコンソールアプリケーションorスレッド用
        public static void Application_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                ShowErrorMessage(ex, "Application_UnhandledExceptionによる例外通知です。");
            }
        }

        // ユーザーフレンドリなダイアログを表示するメソッド
        public static void ShowErrorMessage(Exception ex, string extraMessage)
        {
            // 上書き false 追記 true
            using (StreamWriter sw = new StreamWriter(@"C:\RBSIF\RBSIF_ERROR.LOG", false, Encoding.GetEncoding("Shift_JIS")))
            {
                sw.Write("\n\n————————\n" + extraMessage + "\n————————\n" +
                            "【エラー内容】\n" + ex.Message + "\n" +
                            "【スタックトレース】\n" + ex.StackTrace);
            }

            //MessageBox.Show(extraMessage + " \n————————\n\n" +
            //  "エラーが発生しました。\n\n" +
            //  "【エラー内容】\n" + ex.Message + "\n\n" +
            //  "【スタックトレース】\n" + ex.StackTrace);
        }
    }
}

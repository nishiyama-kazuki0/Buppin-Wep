using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CmsMain
{
	public class CmnObject
	{
		#region DB接続変数
		public string DB_DataSource;
		public string DB_InitialCatalog;
		public string DB_UserId;
		public string DB_Password;
		#endregion

		#region データテーブル変数
		// 定義用
		public DataTable def_MTLD010;       // パレット入庫指示
        public DataTable def_MTLD030;       // 原料在庫情報
        public DataTable def_MTLD040;       // 原料自動投入指図
        public DataTable def_MTLD050;       // パレット出庫指図
        // 登録用
        public DataTable dt_MTLD010;        // パレット入庫指示
        public DataTable dt_MTLD030;        // 原料在庫情報
        public DataTable dt_MTLD040;        // 原料自動投入指図
        public DataTable dt_MTLD050;        // パレット出庫指図
        // 抽出用
        public DataTable dt_MTLU010;        // 原料自動投入開始
        public DataTable dt_MTLU020;        // 原料自動投入実績
        #endregion

        #region 集配信監視変数
        public int HULFT集配信監視間隔;
		public int 原料自動投入開始監視間隔;
		public int 原料自動投入実績監視間隔;
        #endregion

        #region HULFT要求パス変数
        public string RequestPass;
        #endregion

        #region HULFTフォルダパス変数
        public string 集信フォルダ;
        public string 配信フォルダ;
        public string 保存フォルダ_MTLD010;
        public string 保存フォルダ_MTLD030;
        public string 保存フォルダ_MTLD040;
        public string 保存フォルダ_MTLD050;
        public string 保存フォルダ_MTLU010;
        public string 保存フォルダ_MTLU020;
        #endregion

        #region HULFTファイルID変数
        public string HulftIDパレット入庫指示;
        public string HulftID原料在庫情報;
        public string HulftID原料自動投入指図;
        public string HulftIDパレット出庫指図;
        public string HulftID原料自動投入開始;
        public string HulftID原料自動投入実績;
        #endregion

        #region 配信SEQ変数
        public int 原料自動投入開始_配信SEQ = 0;
        public int 原料自動投入実績_配信SEQ = 0;
        #endregion

        #region 処理中変数
        public bool 原料自動投入開始処理中フラグ = false;
        public bool 原料自動投入実績処理中フラグ = false;
        #endregion

        #region スレッド終了変数
        public bool パレット入庫指示スレッド終了フラグ = false;
        public bool 原料在庫情報スレッド終了フラグ = false;
        public bool 原料自動投入指図スレッド終了フラグ = false;
        public bool パレット出庫指図スレッド終了フラグ = false;
        public bool 原料自動投入開始スレッド終了フラグ = false;
        public bool 原料自動投入実績スレッド終了フラグ = false;
        #endregion

        #region 最終ログ書込日付
        public DateTime LastLogDate;
        #endregion

        #region 定数定義
        public const string HULFTEXEFILE = @"C:\HULFT Family\hulft8\bin\Utlsend.exe";
        public const string HULFT要求拡張子 = "*.REQ";
        public const string HULFT完了コード正常 = "0";
        public const string カンマ = ",";
		#endregion

		#region Event定義
		public ManualResetEvent パレット入庫指示Event;
        public ManualResetEvent 原料在庫情報Event;
        public ManualResetEvent 原料自動投入指図Event;
        public ManualResetEvent パレット出庫指図Event;
        public ManualResetEvent 原料自動投入開始Event;
        public ManualResetEvent 原料自動投入実績Event;
        #endregion

        #region Queue定義
        public Queue<string> パレット入庫指示Queue;
        public Queue<string> 原料在庫情報Queue;
        public Queue<string> 原料自動投入指図Queue;
        public Queue<string> パレット出庫指図Queue;
        public Queue<string> 原料自動投入開始Queue;
        public Queue<string> 原料自動投入実績Queue;
        #endregion

        #region Queue要求定義
        // 以下の種別でセットするデータは異なり可変（文字列カンマ区切り）
        public const int QUE種別 = 0;
        public const string QUE_展開要求 = "01";
        public const string QUE_抽出要求 = "02";
        public const string QUE_抽出ファイルバックアップ要求 = "03";
        public const string QUE_スレッド終了 = "99";
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CmnObject()
		{
            #region Event生成
            パレット入庫指示Event = new ManualResetEvent(false);
            原料在庫情報Event = new ManualResetEvent(false);
            原料自動投入指図Event = new ManualResetEvent(false);
            パレット出庫指図Event = new ManualResetEvent(false);
            原料自動投入開始Event = new ManualResetEvent(false);
            原料自動投入実績Event = new ManualResetEvent(false);
            #endregion

            #region Queue生成
            パレット入庫指示Queue = new Queue<string>();
            原料在庫情報Queue = new Queue<string>();
            原料自動投入指図Queue = new Queue<string>();
            パレット出庫指図Queue = new Queue<string>();
            原料自動投入開始Queue = new Queue<string>();
            原料自動投入実績Queue = new Queue<string>();
            #endregion
        }

        /// <summary>
        /// アプリケーション初期化情報取得
        /// </summary>
        public void InitAppConfig()
		{
			// DB接続設定
			DB_DataSource = ConfigurationManager.AppSettings["DB_DATA_SOURCE"];
			DB_InitialCatalog = ConfigurationManager.AppSettings["DB_INITIAL_CATALOG"];
			DB_UserId = ConfigurationManager.AppSettings["DB_USER_ID"];
			DB_Password = ConfigurationManager.AppSettings["DB_PASSWORD"];

            // 集信配信監視間隔
            HULFT集配信監視間隔 = int.Parse(ConfigurationManager.AppSettings["HULFT集配信監視間隔"]) * 1000;
            原料自動投入開始監視間隔 = int.Parse(ConfigurationManager.AppSettings["原料自動投入開始監視間隔"]) * 1000;
            原料自動投入実績監視間隔 = int.Parse(ConfigurationManager.AppSettings["原料自動投入実績監視間隔"]) * 1000;

            // HULFT要求パス取得
            RequestPass = ConfigurationManager.AppSettings["REQUEST_PASS"];

            // HULFTフォルダパス取得
            集信フォルダ = ConfigurationManager.AppSettings["集信フォルダ"];
            配信フォルダ = ConfigurationManager.AppSettings["配信フォルダ"];
            保存フォルダ_MTLD010 = ConfigurationManager.AppSettings["保存フォルダ_MTLD010"];
            保存フォルダ_MTLD030 = ConfigurationManager.AppSettings["保存フォルダ_MTLD030"];
            保存フォルダ_MTLD040 = ConfigurationManager.AppSettings["保存フォルダ_MTLD040"];
            保存フォルダ_MTLD050 = ConfigurationManager.AppSettings["保存フォルダ_MTLD050"];
            保存フォルダ_MTLU010 = ConfigurationManager.AppSettings["保存フォルダ_MTLU010"];
            保存フォルダ_MTLU020 = ConfigurationManager.AppSettings["保存フォルダ_MTLU020"];

			// HULFTファイルID取得
			HulftIDパレット入庫指示 = ConfigurationManager.AppSettings["HULFTID_パレット入庫指示"];
            HulftID原料在庫情報 = ConfigurationManager.AppSettings["HULFTID_原料在庫情報"];
            HulftID原料自動投入指図 = ConfigurationManager.AppSettings["HULFTID_原料自動投入指図"];
            HulftIDパレット出庫指図 = ConfigurationManager.AppSettings["HULFTID_パレット出庫指図"];
            HulftID原料自動投入開始 = ConfigurationManager.AppSettings["HULFTID_原料自動投入開始"];
            HulftID原料自動投入実績 = ConfigurationManager.AppSettings["HULFTID_原料自動投入実績"];
        }

        /// <summary>
        /// データベース接続確認
        /// </summary>
        public string DBopenCheck()
        {
            string retStr = "";

            #region 接続文字列設定
            var connectionString =
                "Data Source=" + DB_DataSource +
                ";Initial Catalog=" + DB_InitialCatalog +
                ";User Id=" + DB_UserId +
                ";Password=" + DB_Password;
            #endregion

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                }
                catch (Exception ex)
                {
                    retStr = ex.Message.ToString();
                    return retStr;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return retStr;
        }

        /// <summary>
        /// テーブル定義取得
        /// </summary>
        public string SelectTableDef()
		{
			string retStr = "";

			#region 接続文字列設定
			var connectionString =
				"Data Source=" + DB_DataSource +
				";Initial Catalog=" + DB_InitialCatalog +
				";User Id=" + DB_UserId +
				";Password=" + DB_Password;
			#endregion

			using (var connection = new SqlConnection(connectionString))
			using (var command = connection.CreateCommand())
			{
				try
				{
					connection.Open();

                    #region パレット入庫指示
                    command.CommandText =
                        @"SELECT c.name AS 項目名 FROM
							sys.objects t INNER JOIN sys.columns c ON t.object_id = c.object_id
							WHERE t.type = 'U' AND t.name = 'IF_RAW_RCV_STORAGE_PLAN' ORDER BY c.column_id";
					var adapter = new SqlDataAdapter(command);
                    def_MTLD010 = new DataTable();
					adapter.Fill(def_MTLD010);
                    #endregion

                    #region 原料在庫情報
                    command.CommandText =
                        @"SELECT c.name AS 項目名 FROM
							sys.objects t INNER JOIN sys.columns c ON t.object_id = c.object_id
							WHERE t.type = 'U' AND t.name = 'IF_RAW_RCV_STOCK_INFO' ORDER BY c.column_id";
                    adapter = new SqlDataAdapter(command);
                    def_MTLD030 = new DataTable();
                    adapter.Fill(def_MTLD030);
                    #endregion

                    #region 原料自動投入指図
                    command.CommandText =
                        @"SELECT c.name AS 項目名 FROM
							sys.objects t INNER JOIN sys.columns c ON t.object_id = c.object_id
							WHERE t.type = 'U' AND t.name = 'IF_RAW_RCV_AUTO_INPUT_PLAN' ORDER BY c.column_id";
                    adapter = new SqlDataAdapter(command);
                    def_MTLD040 = new DataTable();
                    adapter.Fill(def_MTLD040);
                    #endregion

                    #region パレット出庫指図
                    command.CommandText =
                        @"SELECT c.name AS 項目名 FROM
							sys.objects t INNER JOIN sys.columns c ON t.object_id = c.object_id
							WHERE t.type = 'U' AND t.name = 'IF_RAW_RCV_RETRIEVAL_PLAN' ORDER BY c.column_id";
                    adapter = new SqlDataAdapter(command);
                    def_MTLD050 = new DataTable();
                    adapter.Fill(def_MTLD050);
                    #endregion
                }
                catch (Exception ex)
				{
					retStr = ex.Message.ToString();
					return retStr;
				}
			}
			return retStr;
		}

		/// <summary>
		/// データテーブル生成
		/// </summary>
		public void CreateDataTable(CmnObject CmnObj)
		{
            #region パレット入庫指示
            dt_MTLD010 = new DataTable("IF_RAW_RCV_STORAGE_PLAN");
			foreach (DataRow dr in def_MTLD010.Rows)
			{
				if(dr["項目名"].ToString() == "SEQ")
					break;
                dt_MTLD010.Columns.Add(dr["項目名"].ToString(), typeof(string));
			}
            #endregion

            #region 原料在庫情報
            dt_MTLD030 = new DataTable("IF_RAW_RCV_STOCK_INFO");
            foreach (DataRow dr in def_MTLD030.Rows)
            {
                if (dr["項目名"].ToString() == "SEQ")
                    break;
                dt_MTLD030.Columns.Add(dr["項目名"].ToString(), typeof(string));
            }
            #endregion

            #region 原料自動投入指図
            dt_MTLD040 = new DataTable("IF_RAW_RCV_AUTO_INPUT_PLAN");
            foreach (DataRow dr in def_MTLD040.Rows)
            {
                if (dr["項目名"].ToString() == "SEQ")
                    break;
                dt_MTLD040.Columns.Add(dr["項目名"].ToString(), typeof(string));
            }
            #endregion

            #region パレット出庫指図
            dt_MTLD050 = new DataTable("IF_RAW_RCV_RETRIEVAL_PLAN");
            foreach (DataRow dr in def_MTLD050.Rows)
            {
                if (dr["項目名"].ToString() == "SEQ")
                    break;
                dt_MTLD050.Columns.Add(dr["項目名"].ToString(), typeof(string));
            }
            #endregion

            #region 原料自動投入開始
            dt_MTLU010 = new DataTable("IF_RAW_SND_AUTO_INPUT_START");
            #endregion

            #region 原料自動投入実績
            dt_MTLU020 = new DataTable("IF_RAW_SND_AUTO_INPUT_RESULT");
            #endregion
        }

        /// <summary>
        /// 配信データSEQ取得
        /// </summary>
        public string 配信データSEQ取得(string 配信ファイルID, ref int 配信データSEQ)
        {
            string retStr = "";

            #region 接続文字列設定
            var connectionString =
                "Data Source=" + DB_DataSource +
                ";Initial Catalog=" + DB_InitialCatalog +
                ";User Id=" + DB_UserId +
                ";Password=" + DB_Password;
            #endregion

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();

                    if (配信ファイルID == HulftID原料自動投入開始)
                    {
                        command.CommandText =
                            @"SELECT ISNULL(
                            (SELECT SEQ FROM IF_SND_AUTO_INPUT_START WHERE
                            SEQ = (SELECT MIN(SEQ) FROM IF_SND_AUTO_INPUT_START)), 0)";
                    }
                    else if (配信ファイルID == HulftID原料自動投入実績)
                    {
                        command.CommandText =
                            @"SELECT ISNULL(
                            (SELECT SEQ FROM IF_SND_AUTO_INPUT_RESULT_H WHERE
                            SEQ = (SELECT MIN(SEQ) FROM IF_SND_AUTO_INPUT_RESULT_H)), 0)";
                    }
                    配信データSEQ = (int)command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    retStr = ex.Message.ToString();
                    return retStr;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return retStr;
        }

        /// <summary>
        /// 配信データ削除（例外対応）
        /// </summary>
        public string 配信データ削除(string 配信ファイルID)
        {
            string retStr = "";

            #region 接続文字列設定
            var connectionString =
                "Data Source=" + DB_DataSource +
                ";Initial Catalog=" + DB_InitialCatalog +
                ";User Id=" + DB_UserId +
                ";Password=" + DB_Password;
            #endregion

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        if (配信ファイルID == HulftID原料自動投入開始)
                        {
                            command.CommandText = @"DELETE IF_RAW_SND_AUTO_INPUT_START";
                        }
                        else if (配信ファイルID == HulftID原料自動投入実績)
                        {
                            command.CommandText = @"DELETE IF_RAW_SND_AUTO_INPUT_RESULT";
                        }
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    retStr = ex.Message.ToString();
                    return retStr;
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return retStr;
        }

        /// <summary>
        /// 配信ファイル名登録1
        /// </summary>
        public void 配信ファイル名登録1(string ファイル名, int SEQ)
        {
            string retStr = "";

            #region 接続文字列設定
            var connectionString =
                "Data Source=" + DB_DataSource +
                ";Initial Catalog=" + DB_InitialCatalog +
                ";User Id=" + DB_UserId +
                ";Password=" + DB_Password;
            #endregion

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();

                    command.CommandText = @"UPDATE BK_IF_SND_AUTO_INPUT_START SET ファイル名 = @ファイル名 WHERE SEQ = @SEQ";
                    command.Parameters.Add(new SqlParameter("@ファイル名", SqlDbType.Char, -1)).Value = ファイル名;
                    command.Parameters.Add(new SqlParameter("@SEQ", SqlDbType.Int, -1)).Value = SEQ;

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    retStr = ex.Message.ToString();
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return;
        }

        /// <summary>
        /// 配信ファイル名登録2
        /// </summary>
        public void 配信ファイル名登録2(string ファイル名, int SEQ, string ADD)
        {
            string retStr = "";

            #region 接続文字列設定
            var connectionString =
                "Data Source=" + DB_DataSource +
                ";Initial Catalog=" + DB_InitialCatalog +
                ";User Id=" + DB_UserId +
                ";Password=" + DB_Password;
            #endregion

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                try
                {
                    connection.Open();

                    command.CommandText = @"UPDATE BK_IF_SND_AUTO_INPUT_RESULT_" + ADD + " SET ファイル名 = @ファイル名 WHERE SEQ = @SEQ";
                    command.Parameters.Add(new SqlParameter("@ファイル名", SqlDbType.Char, -1)).Value = ファイル名;
                    command.Parameters.Add(new SqlParameter("@SEQ", SqlDbType.Int, -1)).Value = SEQ;

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    retStr = ex.Message.ToString();
                }
                finally
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            return;
        }
    }
}

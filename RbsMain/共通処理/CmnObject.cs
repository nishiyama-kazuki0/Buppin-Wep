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

namespace RbsMain
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
		public DataTable def_RBSU010;       // 端数在庫実績
        public DataTable def_RBSU020;       // 投入実績
        public DataTable def_RBSU030;       // 投入NG実績
        // 登録用
        public DataTable dt_RBSU010;        // 端数在庫実績
        public DataTable dt_RBSU020;        // 投入実績
        public DataTable dt_RBSU030;        // 投入NG実績
        // 抽出用
        public DataTable dt_RBSD010;        // 正梱投入指示
        public DataTable dt_RBSD020;        // 端数投入指示
        #endregion

        #region 集配信監視変数
        public int HULFT集配信監視間隔;
		public int 正梱投入指示監視間隔;
		public int 端数投入指示監視間隔;
        #endregion

        #region HULFT要求パス変数
        public string RequestPass;
        #endregion

        #region HULFTフォルダパス変数
        public string 集信フォルダ;
        public string 配信フォルダ;
        public string 保存フォルダ_RBSD010;
        public string 保存フォルダ_RBSD020;
        public string 保存フォルダ_RBSU010;
        public string 保存フォルダ_RBSU020;
        public string 保存フォルダ_RBSU030;
        #endregion

        #region HULFTファイルID変数
        public string HulftID正梱投入指示;
        public string HulftID端数投入指示;
        public string HulftID端数在庫実績;
        public string HulftID投入実績;
        public string HulftID投入NG実績;
        #endregion

        #region 配信SEQ変数
        public int 正梱投入指示_配信SEQ = 0;
        public int 端数投入指示_配信SEQ = 0;
        #endregion

        #region 処理中変数
        public bool 正梱投入指示処理中フラグ = false;
        public bool 端数投入指示処理中フラグ = false;
        #endregion

        #region スレッド終了変数
        public bool 正梱投入指示スレッド終了フラグ = false;
        public bool 端数投入指示スレッド終了フラグ = false;
        public bool 端数在庫実績スレッド終了フラグ = false;
        public bool 投入実績スレッド終了フラグ = false;
        public bool 投入NG実績スレッド終了フラグ = false;
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
		public ManualResetEvent 正梱投入指示Event;
        public ManualResetEvent 端数投入指示Event;
        public ManualResetEvent 端数在庫実績Event;
        public ManualResetEvent 投入実績Event;
        public ManualResetEvent 投入NG実績Event;
        #endregion

        #region Queue定義
        public Queue<string> 正梱投入指示Queue;
        public Queue<string> 端数投入指示Queue;
        public Queue<string> 端数在庫実績Queue;
        public Queue<string> 投入実績Queue;
        public Queue<string> 投入NG実績Queue;
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
            正梱投入指示Event = new ManualResetEvent(false);
            端数投入指示Event = new ManualResetEvent(false);
            端数在庫実績Event = new ManualResetEvent(false);
            投入実績Event = new ManualResetEvent(false);
            投入NG実績Event = new ManualResetEvent(false);
            #endregion

            #region Queue生成
            正梱投入指示Queue = new Queue<string>();
            端数投入指示Queue = new Queue<string>();
            端数在庫実績Queue = new Queue<string>();
            投入実績Queue = new Queue<string>();
            投入NG実績Queue = new Queue<string>();
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
            正梱投入指示監視間隔 = int.Parse(ConfigurationManager.AppSettings["正梱投入指示監視間隔"]) * 1000;
            端数投入指示監視間隔 = int.Parse(ConfigurationManager.AppSettings["端数投入指示監視間隔"]) * 1000;

            // HULFT要求パス取得
            RequestPass = ConfigurationManager.AppSettings["REQUEST_PASS"];

            // HULFTフォルダパス取得
            集信フォルダ = ConfigurationManager.AppSettings["集信フォルダ"];
            配信フォルダ = ConfigurationManager.AppSettings["配信フォルダ"];
            保存フォルダ_RBSD010 = ConfigurationManager.AppSettings["保存フォルダ_RBSD010"];
            保存フォルダ_RBSD020 = ConfigurationManager.AppSettings["保存フォルダ_RBSD020"];
            保存フォルダ_RBSU010 = ConfigurationManager.AppSettings["保存フォルダ_RBSU010"];
            保存フォルダ_RBSU020 = ConfigurationManager.AppSettings["保存フォルダ_RBSU020"];
            保存フォルダ_RBSU030 = ConfigurationManager.AppSettings["保存フォルダ_RBSU030"];

            // HULFTファイルID取得
            HulftID正梱投入指示 = ConfigurationManager.AppSettings["HULFTID_正梱投入指示"];
            HulftID端数投入指示 = ConfigurationManager.AppSettings["HULFTID_端数投入指示"];
            HulftID端数在庫実績 = ConfigurationManager.AppSettings["HULFTID_端数在庫実績"];
            HulftID投入実績 = ConfigurationManager.AppSettings["HULFTID_投入実績"];
            HulftID投入NG実績 = ConfigurationManager.AppSettings["HULFTID_投入NG実績"];
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

                    #region 端数在庫実績
                    command.CommandText =
                        @"SELECT c.name AS 項目名 FROM
							sys.objects t INNER JOIN sys.columns c ON t.object_id = c.object_id
							WHERE t.type = 'U' AND t.name = 'IF_RAW_RCV_PART_STOCK_RESULT' ORDER BY c.column_id";
					var adapter = new SqlDataAdapter(command);
                    def_RBSU010 = new DataTable();
					adapter.Fill(def_RBSU010);
                    #endregion

                    #region 投入実績
                    command.CommandText =
                        @"SELECT c.name AS 項目名 FROM
							sys.objects t INNER JOIN sys.columns c ON t.object_id = c.object_id
							WHERE t.type = 'U' AND t.name = 'IF_RAW_RCV_INPUT_RESULT' ORDER BY c.column_id";
                    adapter = new SqlDataAdapter(command);
                    def_RBSU020 = new DataTable();
                    adapter.Fill(def_RBSU020);
                    #endregion

                    #region 投入NG実績
                    command.CommandText =
                        @"SELECT c.name AS 項目名 FROM
							sys.objects t INNER JOIN sys.columns c ON t.object_id = c.object_id
							WHERE t.type = 'U' AND t.name = 'IF_RAW_RCV_NG_INPUT_RESULT' ORDER BY c.column_id";
                    adapter = new SqlDataAdapter(command);
                    def_RBSU030 = new DataTable();
                    adapter.Fill(def_RBSU030);
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
            #region 端数在庫実績
            dt_RBSU010 = new DataTable("IF_RAW_RCV_PART_STOCK_RESULT");
			foreach (DataRow dr in def_RBSU010.Rows)
			{
				if(dr["項目名"].ToString() == "SEQ")
					break;
                dt_RBSU010.Columns.Add(dr["項目名"].ToString(), typeof(string));
			}
            #endregion

            #region 投入実績
            dt_RBSU020 = new DataTable("IF_RAW_RCV_INPUT_RESULT");
            foreach (DataRow dr in def_RBSU020.Rows)
            {
                if (dr["項目名"].ToString() == "SEQ")
                    break;
                dt_RBSU020.Columns.Add(dr["項目名"].ToString(), typeof(string));
            }
            #endregion

            #region 投入NG実績
            dt_RBSU030 = new DataTable("IF_RAW_RCV_NG_INPUT_RESULT");
            foreach (DataRow dr in def_RBSU030.Rows)
            {
                if (dr["項目名"].ToString() == "SEQ")
                    break;
                dt_RBSU030.Columns.Add(dr["項目名"].ToString(), typeof(string));
            }
            #endregion

            #region 正梱投入指示
            dt_RBSD010 = new DataTable("IF_RAW_SND_FULL_INPUT_ORDER");
            #endregion

            #region 端数投入指示
            dt_RBSD020 = new DataTable("IF_RAW_SND_PART_INPUT_ORDER");
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

                    if (配信ファイルID == HulftID正梱投入指示)
                    {
                        command.CommandText =
                            @"SELECT ISNULL(
                            (SELECT SEQ FROM IF_SND_FULL_INPUT_ORDER_H WHERE
                            SEQ = (SELECT MIN(SEQ) FROM IF_SND_FULL_INPUT_ORDER_H)), 0)";
                    }
                    else if (配信ファイルID == HulftID端数投入指示)
                    {
                        command.CommandText =
                            @"SELECT ISNULL(
                            (SELECT SEQ FROM IF_SND_PART_INPUT_ORDER_H WHERE
                            SEQ = (SELECT MIN(SEQ) FROM IF_SND_PART_INPUT_ORDER_H)), 0)";
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
                        if (配信ファイルID == HulftID正梱投入指示)
                        {
                            command.CommandText = @"DELETE IF_RAW_SND_FULL_INPUT_ORDER";
                        }
                        else if (配信ファイルID == HulftID端数投入指示)
                        {
                            command.CommandText = @"DELETE IF_RAW_SND_PART_INPUT_ORDER";
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
        public void 配信ファイル名登録1(string ファイル名, int SEQ, string ADD)
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

                    command.CommandText = @"UPDATE BK_IF_SND_FULL_INPUT_ORDER_" + ADD + " SET ファイル名 = @ファイル名 WHERE SEQ = @SEQ";
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

                    command.CommandText = @"UPDATE BK_IF_SND_PART_INPUT_ORDER_" + ADD + " SET ファイル名 = @ファイル名 WHERE SEQ = @SEQ";
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

using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;

namespace CmsMain
{
	public class パレット入庫指示
	{
		private CmsMain CmsMain;
		private CmnObject CmnObj;
		private string QueueData;
		private string[] ReqData;
        private string timestamp;
        private string ファイル名;
        private string 集信ファイル;
		private string 保存ファイル;
		private int 登録件数;
		private int 異常件数;

		public パレット入庫指示(CmsMain paraCmsMain, CmnObject paraCmnObj)
		{
			CmsMain = paraCmsMain;
			CmnObj = paraCmnObj;
		}

		public void パレット入庫指示Func()
		{
			for (; ; )
			{
				#region 処理要求をQueueから取り出してブレイク、無ければ開始イベント待ち
				for (; ; )
				{
					try
					{
						lock (CmnObj.パレット入庫指示Queue)    // 排他ロックを取得
						{
							if (CmnObj.パレット入庫指示Queue.Count > 0)
							{
								QueueData = CmnObj.パレット入庫指示Queue.Dequeue();	// キューから取り出す（先入先出）
							}
							else
                            {
								QueueData = "";
							}
						}
						if (String.IsNullOrEmpty(QueueData))
						{
							#region 開始イベント待ち
							CmnObj.パレット入庫指示Event.WaitOne();
							CmnObj.パレット入庫指示Event.Reset();
							#endregion
						}
						else
                        {
							ReqData = QueueData.Split(',');     // 要求データセットしてブレイク
							break;
						}
					}
					catch (Exception ex)
					{
						CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
							, new object[] { "パレット入庫指示キューからの取り出し例外：" + ex.Message.ToString() });
						continue;
					}
				}
				#endregion

				#region 終了要求
				if (ReqData[CmnObject.QUE種別] == CmnObject.QUE_スレッド終了)
				{
					break;
				}
				#endregion

				if (ReqData[CmnObject.QUE種別] == CmnObject.QUE_展開要求)
				{
                    timestamp = ReqData[1];

                    ファイル名 = timestamp + "_" + CmnObj.HulftIDパレット入庫指示 + ".csv";
                    集信ファイル = CmnObj.集信フォルダ + ファイル名;
                    保存ファイル = CmnObj.保存フォルダ_MTLD010 + ファイル名;

                    #region 集信ファイル存在チェック
                    try
                    {
						if (!File.Exists(集信ファイル))
						{
							CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
								, new object[] { "パレット入庫指示：集信ファイル不在異常 " + ファイル名 });
                            continue;
                        }
                    }
					catch (Exception ex)
					{
						CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
							, new object[] { "パレット入庫指示：File.Exists例外 " + ex.Message.ToString() });
                        continue;
                    }
                    #endregion

                    #region 変数初期化
                    登録件数 = 0;
					異常件数 = 0;
					CmnObj.dt_MTLD010.Clear();
                    #endregion

                    #region 2.展開開始ログ
                    CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                    , new object[] { "パレット入庫指示：2.展開開始 [" + ファイル名 + "]" });
                    #endregion

                    #region ファイルを読み込みデータテーブル格納
                    try
                    {
                        using (StreamReader sr = new StreamReader(集信ファイル, Encoding.GetEncoding("Shift_JIS")))
                        {
                            DataRow dr;
                            string lineData = "";
							string[] values;
                            int cnt = 0;

                            while ((lineData = sr.ReadLine()) != null)
                            {
                                cnt++;
                                try
                                {
                                    values = lineData.Split(',');
                                    dr = CmnObj.dt_MTLD010.NewRow();

                                    #region データテーブル格納
                                    dr["階層識別子"] = values[0];
                                    dr["COL1"] = values[1];
                                    dr["COL2"] = values[2];
                                    dr["COL3"] = values[3];
                                    dr["COL4"] = values[4];
                                    dr["ファイル識別ID"] = "ファイル識別ID";
                                    dr["ファイル名"] = ファイル名;
                                    #endregion

                                    CmnObj.dt_MTLD010.Rows.Add(dr);
                                    登録件数++;
                                }
                                catch (Exception ex)
                                {
                                    CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                        , new object[] { "パレット入庫指示：データテーブル格納例外 " + cnt.ToString() + "レコード " + ex.Message.ToString() });
                                    異常件数++;
                                }
                            }
                        }
						if(異常件数 != 0)
						{
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                            , new object[] { "パレット入庫指示：StreamReader例外 " + ex.Message.ToString() });
                        continue;
                    }
                    #endregion

                    #region データテーブルコミット
                    try
                    {
                        #region 3.データ格納ログ
                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                            , new object[] { "パレット入庫指示：3.データ格納 登録件数->" + 登録件数.ToString() });
                        #endregion

                        CmnObj.dt_MTLD010.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                            , new object[] { "パレット入庫指示：データテーブルコミット例外 " + ex.Message.ToString() });
                        continue;
                    }
                    #endregion

                    #region DBトランザクション処理

                    #region DB接続文字列設定
                    var connectionString =
						"Data Source=" + CmnObj.DB_DataSource +
						";Initial Catalog=" + CmnObj.DB_InitialCatalog +
						";User Id=" + CmnObj.DB_UserId +
						";Password=" + CmnObj.DB_Password;
					#endregion

					using (var connection = new SqlConnection(connectionString))
					{
						#region DB接続
						try
						{
							connection.Open();
						}
						catch (Exception ex)
						{
							CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
								, new object[] { "パレット入庫指示：DBトランザクション例外 " + ex.Message });
                            continue;
                        }
                        #endregion

                        // IF登録トランザクション開始
                        using (var tran = connection.BeginTransaction())
                        {
                            try
                            {
                                #region 4.バルクインサート開始ログ
                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "パレット入庫指示：4.バルクインサート開始 [IF_RAW_RCV_STORAGE_PLAN]" });
                                #endregion

                                // バルクインサート
                                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, tran))
                                {
                                    bulkCopy.BulkCopyTimeout = 600; // in seconds
                                    bulkCopy.DestinationTableName = "IF_RAW_RCV_STORAGE_PLAN";
                                    bulkCopy.WriteToServer(CmnObj.dt_MTLD010);
                                }

                                #region 5.バルクインサート完了ログ
                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "パレット入庫指示：5.バルクインサート完了" });
                                #endregion

                                #region データテーブルクリア
                                CmnObj.dt_MTLD010.Clear();
                                #endregion

                                #region 6.IFデータ登録開始ログ
                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "パレット入庫指示：6.IFデータ登録開始 [IF_RCV_STORAGE_PLAN]" });
                                #endregion

                                // IFデータ登録（コミット含む）
                                using (var command = new SqlCommand() { Connection = connection, Transaction = tran, CommandType = CommandType.StoredProcedure })
                                {
                                    // データソースで実行するストアドプロシージャを指定
                                    command.CommandText = "[prep_IF_RCV_STORAGE_PLAN]";
                                    command.Parameters.Clear();

                                    // OUTPUTの設定
                                    command.Parameters.Add("@MES", System.Data.SqlDbType.VarChar, 1024);
                                    command.Parameters["@MES"].Direction = System.Data.ParameterDirection.Output;

                                    // RETURNの設定
                                    command.Parameters.Add("RET", System.Data.SqlDbType.Int);
                                    command.Parameters["RET"].Direction = System.Data.ParameterDirection.ReturnValue;

                                    // ストアドの実行
                                    command.ExecuteNonQuery();

                                    // OUTPUTの取得
                                    string retStr = command.Parameters["@MES"].Value.ToString();

                                    // RETURNの取得
                                    int ret = (int)command.Parameters["RET"].Value;

                                    if (ret == 0)
                                    {
                                        // コミット
                                        tran.Commit();

                                        #region 7.IFデータ登録完了ログ
                                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                            , new object[] { "パレット入庫指示：7.IFデータ登録完了" });
                                        #endregion
                                    }
                                    else
                                    {
                                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                            , new object[] { "パレット入庫指示：IF登録ストアド例外 " + retStr });

                                        // ロールバック
                                        tran.Rollback();

                                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                            , new object[] { "パレット入庫指示：IF登録ロールバック実行" });
                                        continue;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "パレット入庫指示：IF登録トランザクション例外 " + ex.Message });

                                //ロールバック
                                tran.Rollback();

                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "パレット入庫指示：IF登録ロールバック実行" });
                                continue;
                            }
                        }

                        // WMS登録トランザクション開始
                        using (var tran = connection.BeginTransaction())
                        {
                            try
                            {
                                #region 8.WMSデータ登録開始ログ
                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "パレット入庫指示：8.WMSデータ登録開始" });
                                #endregion

                                // WMSデータ登録（コミット含む）
                                using (var command = new SqlCommand() { Connection = connection, Transaction = tran, CommandType = CommandType.StoredProcedure })
                                {
                                    // データソースで実行するストアドプロシージャを指定
                                    command.CommandText = "[upc_IF_RCV_MTLD010]";
                                    command.Parameters.Clear();

                                    // INPUTの設定--->今回は無し
                                    //command.Parameters.Add("@SEQ", System.Data.SqlDbType.Int).Value = System.Data.SqlTypes.SqlInt32.Null;

                                    // ストアドの実行
                                    command.ExecuteNonQuery();

                                    // コミット
                                    tran.Commit();
                                }
                                #region 9.WMSデータ登録完了ログ
                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "パレット入庫指示：9.WMSデータ登録完了" });
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "パレット入庫指示：WMS登録トランザクション例外 " + ex.Message });

                                //ロールバック
                                tran.Rollback();

                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "パレット入庫指示：WMS登録ロールバック実行" });
                            }
                        }

                        // IF退避削除トランザクション開始（WMS登録成否に限らず実行）
                        using (var tran = connection.BeginTransaction())
						{
							try
							{
								#region 10.IFデータ退避削除開始ログ
								CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
									, new object[] { "パレット入庫指示：10.IFデータ退避削除開始" });
								#endregion

								// IFデータ退避削除（コミット含む）
								using (var command = new SqlCommand() { Connection = connection, Transaction = tran, CommandType = CommandType.StoredProcedure })
								{
									// データソースで実行するストアドプロシージャを指定
									command.CommandText = "[post_IF_RCV_ALL]";
									command.Parameters.Clear();

									// INPUTの設定
									command.Parameters.Add("@FILEID", System.Data.SqlDbType.VarChar).Value = CmnObj.HulftIDパレット入庫指示;

									// OUTPUTの設定
									command.Parameters.Add("@MES", System.Data.SqlDbType.VarChar, 1024);
									command.Parameters["@MES"].Direction = System.Data.ParameterDirection.Output;

									// RETURNの設定
									command.Parameters.Add("RET", System.Data.SqlDbType.Int);
									command.Parameters["RET"].Direction = System.Data.ParameterDirection.ReturnValue;

									// ストアドの実行
									command.ExecuteNonQuery();

									// OUTPUTの取得
									string retStr = command.Parameters["@MES"].Value.ToString();

									// RETURNの取得
									int ret = (int)command.Parameters["RET"].Value;

									if (ret == 0)
									{
										// コミット
										tran.Commit();
									}
									else
									{
										CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
											, new object[] { "パレット入庫指示：IF退避削除ストアド例外 " + retStr });

										// ロールバック
										tran.Rollback();

										CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
											, new object[] { "パレット入庫指示：IF退避削除ロールバック実行" });
									}
								}

								#region 11.IFデータ退避削除完了ログ
								CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
									, new object[] { "パレット入庫指示：11.IFデータ退避削除完了" });
								#endregion
							}
							catch (Exception ex)
							{
								CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
									, new object[] { "パレット入庫指示：IF退避削除トランザクション例外 " + ex.Message });

								//ロールバック
								tran.Rollback();

								CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
									, new object[] { "パレット入庫指示：IF退避削除ロールバック実行" });
							}
						}
					}
                    #endregion

                    #region 集信ファイルを保存フォルダへ移動
                    string exMsg = "";
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            File.Copy(集信ファイル, 保存ファイル, true);
                            exMsg = "";
                            break;
                        }
                        catch (Exception ex)
                        {
                            exMsg = ex.Message.ToString();
                            Thread.Sleep(1000);
                            Application.DoEvents();
                        }
                    }
                    if (exMsg != "")
                    {
                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                            , new object[] { "パレット入庫指示：移動(Copy)例外 " + exMsg });
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            File.Delete(集信ファイル);
                            exMsg = "";
                            break;
                        }
                        catch (Exception ex)
                        {
                            exMsg = ex.Message.ToString();
                            Thread.Sleep(1000);
                            Application.DoEvents();
                        }
                    }
                    if (exMsg != "")
                    {
                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                            , new object[] { "パレット入庫指示：移動(Delete)例外 " + exMsg });
                    }
                    #endregion

                    #region 12.展開終了ログ
                    CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                    , new object[] { "パレット入庫指示：12.展開終了 [" + ファイル名 + "]" });
                    #endregion
				}
			}

			#region 終了完了
			CmnObj.パレット入庫指示スレッド終了フラグ = true;
			#endregion
		}
	}
}

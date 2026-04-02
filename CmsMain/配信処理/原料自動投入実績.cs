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
	public class 原料自動投入実績
	{
		private CmsMain CmsMain;
		private CmnObject CmnObj;
		private string QueueData;
		private string[] ReqData;
        private string ファイル名;
        private string 配信ファイル;
        private string 保存ファイル;
        private int 配信データSEQ;
		private int 配信データ件数;

        public 原料自動投入実績(CmsMain paraCmsMain, CmnObject paraCmnObj)
		{
			CmsMain = paraCmsMain;
			CmnObj = paraCmnObj;
		}

		public void 原料自動投入実績Func()
		{
			for (; ; )
			{
				#region 処理要求をQueueから取り出してブレイク、無ければ開始イベント待ち
				for (; ; )
				{
					try
					{
						lock (CmnObj.原料自動投入実績Queue)    // 排他ロックを取得
						{
							if (CmnObj.原料自動投入実績Queue.Count > 0)
							{
								QueueData = CmnObj.原料自動投入実績Queue.Dequeue();	// キューから取り出す（先入先出）
							}
							else
                            {
								QueueData = "";
							}
						}
						if (String.IsNullOrEmpty(QueueData))
						{
							#region 開始イベント待ち
							CmnObj.原料自動投入実績Event.WaitOne();
							CmnObj.原料自動投入実績Event.Reset();
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
							, new object[] { "原料自動投入実績キューからの取り出し例外：" + ex.Message.ToString() });
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

				if (ReqData[CmnObject.QUE種別] == CmnObject.QUE_抽出要求)
				{
					CmnObj.原料自動投入実績処理中フラグ = true;

                    #region 配信データSEQ取得（ゼロ件の場合は後継処理スルー）
                    配信データSEQ = 0;
					string retStr = CmnObj.配信データSEQ取得(CmnObj.HulftID原料自動投入実績, ref 配信データSEQ);
					if (retStr != "")
					{
						CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
							, new object[] { "原料自動投入実績：配信データSEQ取得例外 " + retStr });
						配信データSEQ = 0;
					}
					if (配信データSEQ == 0)
					{
						CmnObj.原料自動投入実績処理中フラグ = false;
						continue;
					}
                    CmnObj.原料自動投入実績_配信SEQ = 配信データSEQ;
                    #endregion

                    #region 1.配信開始ログ
                    CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
						, new object[] { "-----------------------------------------------------------------" });
					CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
						, new object[] { "原料自動投入実績：1.配信開始 [IF_SND_AUTO_INPUT_RESULT]" });
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
								, new object[] { "原料自動投入実績：DBトランザクション例外 " + ex.Message });
							CmnObj.原料自動投入実績処理中フラグ = false;
							continue;
						}
						#endregion

						// IF抽出トランザクション開始
						using (var tran = connection.BeginTransaction())
						{
							try
							{
								// IFデータ抽出
								using (var command = new SqlCommand() { Connection = connection, Transaction = tran, CommandType = CommandType.StoredProcedure })
								{
									// データソースで実行するストアドプロシージャを指定
									command.CommandText = "[prep_IF_SND_AUTO_INPUT_RESULT]";
									command.Parameters.Clear();

                                    // INPUTの設定
                                    command.Parameters.Add("@SEQ", System.Data.SqlDbType.Int).Value = 配信データSEQ;

                                    // OUTPUTの設定
                                    command.Parameters.Add("@CNT", System.Data.SqlDbType.Int);
									command.Parameters["@CNT"].Direction = System.Data.ParameterDirection.Output;
									command.Parameters.Add("@MES", System.Data.SqlDbType.VarChar, 1024);
									command.Parameters["@MES"].Direction = System.Data.ParameterDirection.Output;

									// RETURNの設定
									command.Parameters.Add("RET", System.Data.SqlDbType.Int);
									command.Parameters["RET"].Direction = System.Data.ParameterDirection.ReturnValue;

									// ストアドの実行
									command.ExecuteNonQuery();

									// OUTPUTの取得
									配信データ件数 = (int)command.Parameters["@CNT"].Value;
									retStr = command.Parameters["@MES"].Value.ToString();

									// RETURNの取得
									int ret = (int)command.Parameters["RET"].Value;

									if (ret == 0 && 配信データ件数 > 0)
									{
										// トランザクションコミット
										tran.Commit();
									}
									else
									{
										if (ret != 0)
										{
											CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
												, new object[] { "原料自動投入実績：IF抽出ストアド例外 " + retStr });
										}
										else
										{
											CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
												, new object[] { "原料自動投入実績：IF抽出ストアド 配信データ件数ゼロ" });
										}

										// ロールバック
										tran.Rollback();

										CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
											, new object[] { "原料自動投入実績：IF抽出ロールバック実行" });

										CmnObj.原料自動投入実績処理中フラグ = false;
										continue;
									}
								}
							}
							catch (Exception ex)
							{
								CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
									, new object[] { "原料自動投入実績：IF抽出トランザクション例外 " + ex.Message });

								// ロールバック
								tran.Rollback();

								CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
									, new object[] { "原料自動投入実績：IF抽出ロールバック実行" });

								CmnObj.原料自動投入実績処理中フラグ = false;
								continue;
							}
						}

                        #region 2.配信データ件数ログ
                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
							, new object[] { "原料自動投入実績：2.配信データ件数->" + 配信データ件数.ToString() });
						#endregion

						// 抽出データテーブルクリア
						CmnObj.dt_MTLU020.Clear();

						// 抽出データセット
						using (var command = connection.CreateCommand())
						{
							try
							{
								command.CommandText =
                                    @"SELECT * FROM IF_RAW_SND_AUTO_INPUT_RESULT ORDER BY SEQ";
								var adapter = new SqlDataAdapter(command);
								adapter.Fill(CmnObj.dt_MTLU020);
							}
							catch (Exception ex)
							{
								CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
									, new object[] { "原料自動投入実績：配信データ取得例外 " + ex.Message });

								#region 配信データ削除（例外対応）
								retStr = CmnObj.配信データ削除(CmnObj.HulftID原料自動投入実績);
								if (retStr != "")
								{
									CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
										, new object[] { "原料自動投入実績：配信データ削除例外 " + retStr });
								}
								#endregion

								CmnObj.原料自動投入実績処理中フラグ = false;
								continue;
							}
						}

						// データテーブルコミット
						CmnObj.dt_MTLU020.AcceptChanges();
					}
                    #endregion

                    #region 配信ファイル作成
                    ファイル名 = CmnObj.HulftID原料自動投入実績 + ".csv";
                    配信ファイル = CmnObj.配信フォルダ + ファイル名;

					try
					{
						using (StreamWriter sw = new StreamWriter(配信ファイル, false, Encoding.GetEncoding("Shift_JIS")))
						{
							// データレコード登録
							StringBuilder sb = new StringBuilder();
							foreach (DataRow dr in CmnObj.dt_MTLU020.Rows)
							{
								if((string)dr[0] == "L")            // 1階層目
                                {
                                    sb.Append((DBNull.Value.Equals(dr[0]) ? "" : (String)dr[0]));
                                    sb.Append(CmnObject.カンマ);
                                    sb.Append((DBNull.Value.Equals(dr[1]) ? "" : (String)dr[1]));
                                }
                                else if ((string)dr[0] == "B")      // 2階層目
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        sb.Append((DBNull.Value.Equals(dr[i]) ? "" : (String)dr[i]));
                                        if (i < 3)
                                        {
                                            sb.Append(CmnObject.カンマ);
                                        }
                                    }
                                }
                                else if ((string)dr[0] == "T")      // 3階層目
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        sb.Append((DBNull.Value.Equals(dr[i]) ? "" : (String)dr[i]));
                                        if (i < 4)
                                        {
                                            sb.Append(CmnObject.カンマ);
                                        }
                                    }
                                }
                                else if ((string)dr[0] == "S")      // 4階層目
                                {
                                    for (int i = 0; i < 5; i++)
                                    {
                                        sb.Append((DBNull.Value.Equals(dr[i]) ? "" : (String)dr[i]));
                                        if (i < 4)
                                        {
                                            sb.Append(CmnObject.カンマ);
                                        }
                                    }
                                }
                                sw.WriteLine(sb.ToString());
								sb.Clear();
							}
						}
					}
					catch (Exception ex)
					{
						CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
							, new object[] { "原料自動投入実績：配信ファイル作成例外 " + ex.Message });

                        #region 配信データ削除（例外対応）
                        retStr = CmnObj.配信データ削除(CmnObj.HulftID原料自動投入実績);
                        if (retStr != "")
                        {
                            CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                , new object[] { "原料自動投入実績：配信データ削除例外 " + retStr });
                        }
                        #endregion

                        #region 配信ファイルを削除
                        try
                        {
							File.Delete(配信ファイル);
						}
						catch
						{
						}
						#endregion

						CmnObj.原料自動投入実績処理中フラグ = false;
						continue;
					}
					#endregion

					#region 3.配信ファイル作成ログ
					CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
						, new object[] { "原料自動投入実績：3.配信ファイル作成 [" + ファイル名 + "]" });
                    #endregion

                    #region HULFT配信要求(EXE起動終了するまで待つ)
                    // 引数セット(第1引数:起動区分,第2引数:ファイルID)
                    string strCmd = " -f " + CmnObj.HulftID原料自動投入実績;
                    try
                    {
                        // ProcessStartInfo の新しいインスタンスを生成する
                        System.Diagnostics.ProcessStartInfo hPsInfo = (
                            new System.Diagnostics.ProcessStartInfo()
                            );
                        // 起動するアプリケーションを設定する
                        hPsInfo.FileName = CmnObject.HULFTEXEFILE;
                        // コマンドライン引数を設定する
                        hPsInfo.Arguments = strCmd;
                        // 起動時のウィンドウの状態を設定する(非表示)
                        hPsInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        // ProcessStartInfo を指定して起動する
                        System.Diagnostics.Process hProcess
                            = System.Diagnostics.Process.Start(hPsInfo);
                        // 完了するまで待つ
                        hProcess.WaitForExit();
                        // 不要になった時点で破棄する
                        hProcess.Close();
                        hProcess.Dispose();

                        #region 4.HULFT配信実行ログ
                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                            , new object[] { "原料自動投入実績：4.HULFT配信実行 [" + ファイル名 + "]" });
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                            , new object[] { "原料自動投入実績：HULFT配信実行例外 " + ex.Message });

                        #region 配信データ削除（例外対応）
                        retStr = CmnObj.配信データ削除(CmnObj.HulftID原料自動投入実績);
                        if (retStr != "")
                        {
                            CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                , new object[] { "原料自動投入実績：配信データ削除例外 " + retStr });
                        }
                        #endregion

                        #region 配信ファイルは削除しない
                        #endregion

                        CmnObj.原料自動投入実績処理中フラグ = false;
                        continue;
                    }
                    #endregion

                    #region DBトランザクション処理

                    #region DB接続文字列設定
                    connectionString =
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
                                , new object[] { "原料自動投入実績：DBトランザクション例外 " + ex.Message });
                            CmnObj.原料自動投入実績処理中フラグ = false;
                            continue;
                        }
                        #endregion

                        // IF退避削除トランザクション開始
                        using (var tran = connection.BeginTransaction())
                        {
                            try
                            {
                                #region 5.IFデータ退避削除開始ログ
                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "原料自動投入実績：5.IFデータ退避削除開始" });
                                #endregion

                                // IFデータ退避削除（コミット含む）
                                using (var command = new SqlCommand() { Connection = connection, Transaction = tran, CommandType = CommandType.StoredProcedure })
                                {
                                    // データソースで実行するストアドプロシージャを指定
                                    command.CommandText = "[post_IF_SND_ALL]";
                                    command.Parameters.Clear();

                                    // INPUTの設定
                                    command.Parameters.Add("@FILEID", System.Data.SqlDbType.VarChar).Value = CmnObj.HulftID原料自動投入実績;

                                    // OUTPUTの設定
                                    command.Parameters.Add("@MES", System.Data.SqlDbType.VarChar, 1024);
                                    command.Parameters["@MES"].Direction = System.Data.ParameterDirection.Output;

                                    // RETURNの設定
                                    command.Parameters.Add("RET", System.Data.SqlDbType.Int);
                                    command.Parameters["RET"].Direction = System.Data.ParameterDirection.ReturnValue;

                                    // ストアドの実行
                                    command.ExecuteNonQuery();

                                    // OUTPUTの取得
                                    retStr = command.Parameters["@MES"].Value.ToString();

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
                                            , new object[] { "原料自動投入実績：IF退避削除ストアド例外 " + retStr });

                                        // ロールバック
                                        tran.Rollback();

                                        CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                            , new object[] { "原料自動投入実績：IF退避削除ロールバック実行" });
                                    }
                                }

                                #region 6.IFデータ退避削除完了ログ
                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "原料自動投入実績：6.IFデータ退避削除完了" });
                                #endregion
                            }
                            catch (Exception ex)
                            {
                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "原料自動投入実績：IF退避削除トランザクション例外 " + ex.Message });

                                //ロールバック
                                tran.Rollback();

                                CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                                    , new object[] { "原料自動投入実績：IF退避削除ロールバック実行" });
                            }
                        }
                    }
                    #endregion

                    CmnObj.原料自動投入実績処理中フラグ = false;
				}
				else if (ReqData[CmnObject.QUE種別] == CmnObject.QUE_抽出ファイルバックアップ要求)
				{
                    #region 配信ファイル名登録
                    string timestamp = ReqData[1];
                    if (CmnObj.原料自動投入実績_配信SEQ != 0)
					{
                        ファイル名 = timestamp + "_" + CmnObj.HulftID原料自動投入実績 + ".csv";
                        CmnObj.配信ファイル名登録2(ファイル名, CmnObj.原料自動投入実績_配信SEQ, "H");
                        CmnObj.配信ファイル名登録2(ファイル名, CmnObj.原料自動投入実績_配信SEQ, "D1");
                        CmnObj.配信ファイル名登録2(ファイル名, CmnObj.原料自動投入実績_配信SEQ, "D2");
                        CmnObj.配信ファイル名登録2(ファイル名, CmnObj.原料自動投入実績_配信SEQ, "D3");
                        CmnObj.原料自動投入実績_配信SEQ = 0;
                    }
                    #endregion

                    #region 配信ファイルを保存フォルダへ移動
                    ファイル名 = CmnObj.HulftID原料自動投入実績 + ".csv";
                    配信ファイル = CmnObj.配信フォルダ + ファイル名;
                    保存ファイル = CmnObj.保存フォルダ_MTLU020 + timestamp + "_" + ファイル名;

                    string exMsg = "";
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            File.Copy(配信ファイル, 保存ファイル, true);
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
                            , new object[] { "原料自動投入実績：移動(Copy)例外 " + exMsg });
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            File.Delete(配信ファイル);
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
                            , new object[] { "原料自動投入実績：移動(Delete)例外 " + exMsg });
                    }
                    #endregion

                    #region 8.配信バックアップログ
                    CmsMain.Invoke(new dlgAccTrace(CmsMain.AccTrace)
                    , new object[] { "原料自動投入実績：8.配信バックアップ [" + ファイル名 + "]" });
                    #endregion
                }
            }

			#region 終了完了
			CmnObj.原料自動投入実績スレッド終了フラグ = true;
			#endregion
		}
	}
}

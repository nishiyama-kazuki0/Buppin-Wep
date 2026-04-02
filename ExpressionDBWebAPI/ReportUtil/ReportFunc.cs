using Anotar.Serilog;
using ClosedXML.Excel;
using com.google.zxing;
using com.google.zxing.qrcode;
using DocumentFormat.OpenXml.EMMA;
using ExpressionDBWebAPI.Common;
using ExpressionDBWebAPI.Controllers;
using OpenAI.Assistants;
using PDFtoPrinter;
using Serilog;
using SharedModels;
using System.Collections.Concurrent;
using System.Drawing.Printing;
using System.Drawing; // System.Drawing.Common が必要（.NET 6以降はNuGetで追加）
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using BarcodeFormat = ZXing.BarcodeFormat;
using System.IO;
using System.Threading.Tasks;
using BlazorDownloadFile;
using Microsoft.AspNetCore.Mvc;



namespace ExpressionDBWebAPI.ReportUtil;

//TODO アドバンスソフトウェア以外の帳票発行も対応可能とするため、インターフェースを実装するようにする。
//TODO 可能であればプロジェクトを分ける
/// <summary>
/// 帳票発行機能クラス
/// ExcutionControllerから呼び出すため、呼び出されるメソッドはstatic限定とする。
/// </summary>
public class ReportFunc 
{
    #region 入荷票



    /// <summary>
    /// 入荷票出力
    /// </summary>
    public static bool 入荷票出力(string DEVICE_ID, string USER_ID, string BASE_ID, int BASE_TYPE, string CONSIGNOR_ID, string 受付No)
    {
        return Proc入荷票出力(DEVICE_ID, USER_ID, BASE_ID, BASE_TYPE, CONSIGNOR_ID, 受付No, false);
    }

    /// <summary>
    /// 入荷票再出力
    /// </summary>
    public static bool 入荷票再出力(string DEVICE_ID, string USER_ID, string BASE_ID, int BASE_TYPE, string CONSIGNOR_ID, string 受付No)
    {
        return Proc入荷票出力(DEVICE_ID, USER_ID, BASE_ID, BASE_TYPE, CONSIGNOR_ID, 受付No, true);
    }

    /// <summary>
    /// 入荷票出力
    /// </summary>
    /// <param name="objReport"></param>
    /// <param name="dt"></param>
    private static bool Proc入荷票出力(string DEVICE_ID, string USER_ID, string BASE_ID, int BASE_TYPE, string CONSIGNOR_ID, string 受付No, bool 再発行)
    {
        LogTo.Information($"DEVICE_ID:{DEVICE_ID},USER_ID:{USER_ID},BASE_ID:{BASE_ID},BASE_TYPE:{BASE_TYPE},CONSIGNOR_ID:{CONSIGNOR_ID},受付No:{受付No},再発行:{再発行}");
        try
        {
            // プリンター名を取得
            string printerName = GetPrinterName(DEVICE_ID);
            (
                bool isPdfOutputOnly
                , string savePdfFolderpath
                , string toPrinterExePath
                , int chankItemCount
                , int chankPageCount
                , int printProcInterval
                , int printProcTimeout
            )
                 = GetPrintProcSettings();

            ClassNameSelect select = new()
            {
                viewName = "VW_PRINT_入荷票"
            };
            select.whereParam.Add("BASE_ID", new WhereParam { val = BASE_ID });
            select.whereParam.Add("BASE_TYPE", new WhereParam { val = BASE_TYPE.ToString() });
            select.whereParam.Add("CONSIGNOR_ID", new WhereParam { val = CONSIGNOR_ID });
            select.whereParam.Add("受付No", new WhereParam { val = 受付No });
            select.orderByParam.Add(new OrderByParam { field = "原産地コード" });
            select.orderByParam.Add(new OrderByParam { field = "ヘッダ" });
            select.orderByParam.Add(new OrderByParam { field = "明細No" });
            List<ResponseValue> responseValues = CommonController.GetResponseValue(select);

            // 件数で分割して、複数ファイルの出力、印刷に対応する
            List<string> replf = [];
            // 分割ページ数を超過した場合は、何行目までProc入荷票出力を行ったか保持し、次のループでその行からrepオブジェクトを作成するように変更する
            //whileで行数をカウントして、全体のカウントよりも大きい小さいを判断する
            if (responseValues.Count > 0)
            {
                string basePath = CommonInfo.AppDataPath;

                int startCount = 0;
                while (responseValues.Count > startCount)
                {
                    CReport rep = new(basePath);

                    rep.Open("5-3 保管_物品管理ラベル.xlsx");

                    rep.Start("FormatSheet");

                    startCount = Proc入荷票出力(rep, responseValues, 再発行, startCount, chankItemCount, chankPageCount);
                    //startCount = restartCount;

                    rep.End();

                    //PDF出力を行う
                    //string fileNamePath = @$"{savePdfFolderpath}\{DateTime.Now:yyyyMMddHHmmssfff}在庫一覧リスト.pdf";
                    string fileNamePath = @$"{savePdfFolderpath}\{Guid.NewGuid().ToString().Replace("-", string.Empty)}_ArrivalsReport.pdf";
                    rep.gSubSavePDF(fileNamePath);

                    rep.Dispose();//メモリ解放を行いたいため、この時点でDisposeする

                    replf.Add(fileNamePath);
                }

            }
            //プリンターへ印刷指示
            PrintProcExecTask(
                replf
                , isPdfOutputOnly
                , printerName
                , toPrinterExePath
                , printProcInterval
                , printProcTimeout
                , printOrientation: PrintOrientation.Virtual//入荷リストは縦向き印字設定のプリンター名を指定
                );
            return true;
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 入荷票出力
    /// </summary>
    /// <param name="objReport"></param>
    /// <param name="dt"></param>
    private static int Proc入荷票出力(CReport objReport, List<ResponseValue> dt, bool 再発行, int reStartCout, int chankItemCount, int chankPageCount)
    {
        DateTime dteNow = DateTime.Now;
        bool bMainHeader = true;
        ResponseValue drHeader = new();
        if (null != dt && dt.Count > 0)
        {
            for (int i = 0; dt[0].Columns.Count > i; i++)
            {
                drHeader.Values.Add(dt[0].Columns[i], "");
            }
        }

        int endCount = chankItemCount + reStartCout >= dt.Count ? dt.Count : chankItemCount + reStartCout;
        int retCount = endCount >= dt.Count ? dt.Count : endCount; //念のためセットしおく
        //LogTo.Information($"入荷票メソッド内 endCount: {endCount},reStartCout:{reStartCout},");
        // 明細データを全件ループ
        for (int i = reStartCout; endCount > i; i++)
        {
            // ヘッダ部分が変わっていたら
            if (drHeader.Values["ヘッダ"].ToString() != dt[i].Values["ヘッダ"].ToString())
            {
                foreach (KeyValuePair<string, object> value in dt[i].Values)
                {
                    drHeader.Values[value.Key] = value.Value;
                }

                if (bMainHeader)
                {
                    bMainHeader = false;

                    // -----------------------------------------------------------------
                    // メインヘッダ出力
                    // -----------------------------------------------------------------
                    objReport.pCellReport.Page.Repeat(1, 13);
                    objReport.gSubField("C出力日付", dteNow.ToString("yyyy/MM/dd"));
                    objReport.gSubField("C出力時刻", dteNow.ToString("HH:mm:ss"));
                    objReport.gSubField("再発行", 再発行 ? "再発行" : "");
                    objReport.gSubField("出力順", drHeader);
                    objReport.gSubField("受付日時", drHeader);
                    objReport.gSubField("車番", drHeader);
                    objReport.gSubField("エチレン区分", drHeader);
                    objReport.gSubField("特別管理品区分", drHeader);
                    objReport.gSubField("受付No", drHeader);
                    objReport.gSubField("トラック区分", drHeader);
                    objReport.gSubField("積荷区分", drHeader);
                    objReport.gSubField("温度帯区分", drHeader);
                    objReport.pCellReport.Page.Next(false);
                }

                objReport.pCellReport.Page.Repeat(14, 9);
                objReport.gSubField("入荷No", drHeader);
                objReport.gSubField("送状No", drHeader);
                objReport.gSubField("品名", drHeader);
                objReport.gSubField("出荷者", drHeader);
                objReport.gSubField("産地名", drHeader);
                objReport.pCellReport.Page.Next(false);

                // 明細数リセット
            }

            // -----------------------------------------------------------------
            // 明細出力
            // -----------------------------------------------------------------
            objReport.pCellReport.Page.Repeat(23, 3);
            objReport.gSubField("連携状態", dt[i]);
            objReport.gSubField("明細No", dt[i]);
            objReport.gSubField("荷姿", dt[i]);
            objReport.gSubField("等級", dt[i]);
            objReport.gSubField("階級", dt[i]);
            objReport.gSubField("荷印", dt[i]);
            objReport.gSubField("作業者説明", dt[i]);
            objReport.gSubField("入数", dt[i]);
            objReport.gSubField("ｹｰｽ数", dt[i]);
            objReport.gSubField("ﾊﾞﾗ数", dt[i]);
            objReport.gSubFieldCode39("バーコード", dt[i], 5, 240, 90);
            objReport.gSubField("換算値", dt[i]);
            objReport.gSubField("配送先件数", dt[i]);
            objReport.gSubField("出荷数量", dt[i]);

            //int pageCount = objReport.pCellReport.Document.;
            //LogTo.Information($"入荷票メソッド内 pageCount: {pageCount}");
            int nNextPg = i + 1;
            retCount = nNextPg;
            if (dt.Count <= nNextPg)
            {
                // 印字終了文字の出力
                objReport.pCellReport.Page.Next(false);
                objReport.pCellReport.Page.Repeat(25, 2);
                objReport.pCellReport.Page.Next(false);
            }
            else if (drHeader.Values["入荷No"].ToString() != dt[nNextPg].Values["入荷No"].ToString())
            {
                // 入荷Noが違う場合は改頁する
                objReport.pCellReport.Page.Next(true);

                // 改頁後メインヘッダは出力する
                bMainHeader = true;
            }
            else
            {
                //改行しない場合はチャンクの明細数を超えてもページ内に含めたいのでendCountを加算する
                endCount
                    = endCount <= nNextPg
                    && drHeader.Values["入荷No"].ToString() == dt[nNextPg].Values["入荷No"].ToString()//上のifの条件の反例なので、不要だが念のため追加しておく
                    ? endCount + 1 : endCount;
                objReport.pCellReport.Page.Next(false);
            }

            //// 明細数カウントアップ
            //intCntDetail += 1;

            //// -----------------------------------------------------------------
            //// 改ページ判断
            //// -----------------------------------------------------------------
            //int nNextPg = i + 1;
            //if (intMaxDetail <= intCntDetail ||
            //    dt.Count <= nNextPg ||
            //    drHeader.Values["ヘッダ"].ToString() != dt[i].Values["ヘッダ"].ToString())
            //{
            //    if (dt.Count <= nNextPg)
            //    {
            //        // 印字終了文字の出力
            //        objReport.pCellReport.Page.Next(false);
            //        objReport.pCellReport.Page.Repeat(25, 2);
            //    }

            //    objReport.pCellReport.Page.Next(true);
            //}
            //else
            //{
            //    objReport.pCellReport.Page.Next(false);
            //}
        }
        return retCount;
    }

    #endregion

    #region 在庫一覧リスト

    /// <summary>
    /// 在庫一覧リスト出力
    /// </summary>
    public static bool 在庫一覧リスト出力(string DEVICE_ID, string USER_ID, string BASE_ID, int BASE_TYPE, string CONSIGNOR_ID, string 入庫日From, string 入庫日To, string 入荷No, string 明細No, string 品名コード, string 課コード, string 出荷者コード, string 産地コード, string パレットNo, string 賞味期限From, string 賞味期限To, string 倉庫コード, string ゾーンコード, string ロケーションNo, string 滞留期間, int 滞留期間whereType, string 在庫有無, string 資材管理)
    {
        return Proc在庫一覧リスト出力(DEVICE_ID, USER_ID, BASE_ID, BASE_TYPE, CONSIGNOR_ID, 入庫日From, 入庫日To, 入荷No, 明細No, 品名コード, 課コード, 出荷者コード, 産地コード, パレットNo, 賞味期限From, 賞味期限To, 倉庫コード, ゾーンコード, ロケーションNo, 滞留期間, 滞留期間whereType, 在庫有無, 資材管理, false);
    }

    /// <summary>
    /// 在庫一覧リスト出力
    /// </summary>
    public static bool 在庫一覧リスト再出力(string DEVICE_ID, string USER_ID, string BASE_ID, int BASE_TYPE, string CONSIGNOR_ID, string 入庫日From, string 入庫日To, string 入荷No, string 明細No, string 品名コード, string 課コード, string 出荷者コード, string 産地コード, string パレットNo, string 賞味期限From, string 賞味期限To, string 倉庫コード, string ゾーンコード, string ロケーションNo, string 滞留期間, int 滞留期間whereType, string 在庫有無, string 資材管理)
    {
        return Proc在庫一覧リスト出力(DEVICE_ID, USER_ID, BASE_ID, BASE_TYPE, CONSIGNOR_ID, 入庫日From, 入庫日To, 入荷No, 明細No, 品名コード, 課コード, 出荷者コード, 産地コード, パレットNo, 賞味期限From, 賞味期限To, 倉庫コード, ゾーンコード, ロケーションNo, 滞留期間, 滞留期間whereType, 在庫有無, 資材管理, true);
    }

    /// <summary>
    /// 在庫一覧リスト出力
    /// </summary>
    /// <param name="objReport"></param>
    /// <param name="dt"></param>
    private static bool Proc在庫一覧リスト出力(string DEVICE_ID, string USER_ID, string BASE_ID, int BASE_TYPE, string CONSIGNOR_ID, string 入庫日From, string 入庫日To, string 入荷No, string 明細No, string 品名コード, string 課コード, string 出荷者コード, string 産地コード, string パレットNo, string 賞味期限From, string 賞味期限To, string 倉庫コード, string ゾーンコード, string ロケーションNo, string 滞留期間, int 滞留期間whereType, string 在庫有無, string 資材管理, bool 再発行)
    {
        LogTo.Information($"DEVICE_ID:{DEVICE_ID},USER_ID:{USER_ID},BASE_ID:{BASE_ID},BASE_TYPE:{BASE_TYPE},CONSIGNOR_ID:{CONSIGNOR_ID},入庫日From:{入庫日From},入庫日To:{入庫日To},入荷No:{入荷No},明細No:{明細No},品名コード:{品名コード},課コード:{課コード},出荷者コード:{出荷者コード},産地コード:{産地コード},パレットNo:{パレットNo},賞味期限From:{賞味期限From},賞味期限To:{賞味期限To},倉庫コード:{倉庫コード},ゾーンコード:{ゾーンコード},ロケーションNo:{ロケーションNo},滞留期間:{滞留期間},滞留期間whereType:{滞留期間whereType},在庫有無:{在庫有無},資材管理:{資材管理},再発行:{再発行}");
        try
        {
            // プリンター名を取得
            string printerName = GetPrinterName(DEVICE_ID);
            (
                bool isPdfOutputOnly
                , string savePdfFolderpath
                , string toPrinterExePath
                , int chankItemCount
                , int chankPageCount
                , int printProcInterval
                , int printProcTimeout
            )
                 = GetPrintProcSettings();
            LogTo.Information(@$"Proc在庫一覧リスト出力_isPdfOutput:[{isPdfOutputOnly}],savePdfFolderpath:[{savePdfFolderpath}],file:[{savePdfFolderpath}\{DateTime.Now:yyyyMMddHHmmss}在庫一覧リスト.pdf]");

            ClassNameSelect select = new()
            {
                viewName = "VW_PRINT_在庫一覧リスト"
            };
            select.whereParam.Add("BASE_ID", new WhereParam { val = BASE_ID });
            select.whereParam.Add("BASE_TYPE", new WhereParam { val = BASE_TYPE.ToString() });
            select.whereParam.Add("CONSIGNOR_ID", new WhereParam { val = CONSIGNOR_ID });
            if (!string.IsNullOrEmpty(入庫日From))
            {
                select.whereParam.Add("入庫日From", new WhereParam { field = "入庫日", val = 入庫日From, whereType = enumWhereType.Above });
            }
            if (!string.IsNullOrEmpty(入庫日To))
            {
                select.whereParam.Add("入庫日To", new WhereParam { field = "入庫日", val = 入庫日To, whereType = enumWhereType.Below });
            }
            if (!string.IsNullOrEmpty(入荷No))
            {
                select.whereParam.Add("入荷No", new WhereParam { val = 入荷No, whereType = enumWhereType.LikeStart });
            }
            if (!string.IsNullOrEmpty(明細No))
            {
                select.whereParam.Add("明細No", new WhereParam { val = 明細No, whereType = enumWhereType.LikeStart });
            }
            if (!string.IsNullOrEmpty(品名コード))
            {
                select.whereParam.Add("品名コード", new WhereParam { linkingVals = new List<string>(品名コード.Split(",")), orLinking = true });
            }
            if (!string.IsNullOrEmpty(課コード))
            {
                select.whereParam.Add("課コード", new WhereParam { linkingVals = new List<string>(課コード.Split(",")), orLinking = true });
            }
            if (!string.IsNullOrEmpty(出荷者コード))
            {
                select.whereParam.Add("出荷者コード", new WhereParam { linkingVals = new List<string>(出荷者コード.Split(",")), orLinking = true });
            }
            if (!string.IsNullOrEmpty(産地コード))
            {
                select.whereParam.Add("産地コード", new WhereParam { linkingVals = new List<string>(産地コード.Split(",")), orLinking = true });
            }
            if (!string.IsNullOrEmpty(パレットNo))
            {
                select.whereParam.Add("パレットNo", new WhereParam { val = パレットNo, whereType = enumWhereType.LikeStart });
            }
            if (!string.IsNullOrEmpty(賞味期限From))
            {
                select.whereParam.Add("賞味期限From", new WhereParam { field = "賞味期限", val = 賞味期限From, whereType = enumWhereType.Above });
            }
            if (!string.IsNullOrEmpty(賞味期限To))
            {
                select.whereParam.Add("賞味期限To", new WhereParam { field = "賞味期限", val = 賞味期限To, whereType = enumWhereType.Below });
            }
            if (!string.IsNullOrEmpty(倉庫コード))
            {
                select.whereParam.Add("倉庫コード", new WhereParam { val = 倉庫コード });
            }
            if (!string.IsNullOrEmpty(ゾーンコード))
            {
                select.whereParam.Add("ゾーンコード", new WhereParam { val = ゾーンコード });
            }
            if (!string.IsNullOrEmpty(ロケーションNo))
            {
                select.whereParam.Add("ロケーションNo", new WhereParam { val = ロケーションNo });
            }
            if (!string.IsNullOrEmpty(在庫有無))
            {
                select.whereParam.Add("在庫有無", new WhereParam { val = 在庫有無 });
            }
            if (!string.IsNullOrEmpty(資材管理))
            {
                select.whereParam.Add("資材管理", new WhereParam { val = 資材管理 });
            }
            if (!string.IsNullOrEmpty(滞留期間))
            {
                if (!Enum.TryParse(滞留期間whereType.ToString(), out enumWhereType type))
                {
                    type = enumWhereType.Equal;
                }
                select.whereParam.Add("滞留期間", new WhereParam { val = 滞留期間, whereType = type });
            }
            select.orderByParam.Add(new OrderByParam { field = "大ヘッダ" });
            select.orderByParam.Add(new OrderByParam { field = "ヘッダ" });
            List<ResponseValue> responseValues = CommonController.GetResponseValue(select);

            // 件数で分割して、複数ファイルの出力、印刷に対応する
            List<string> replf = [];
            if (responseValues.Count > 0)
            {
                string basePath = CommonInfo.AppDataPath;
                int startCount = 0;
                while (responseValues.Count > startCount)
                {
                    CReport rep = new(basePath);

                    rep.Open("物品管理ラベル.xlsx");

                    rep.Start("FormatSheet");

                    startCount = Proc在庫一覧リスト出力(rep, responseValues, 再発行, startCount, chankItemCount, chankPageCount);

                    rep.End();
                    //PDF出力を行う
                    //string fileNamePath = @$"{savePdfFolderpath}\{DateTime.Now:yyyyMMddHHmmssfff}在庫一覧リスト.pdf";
                    string fileNamePath = @$"{savePdfFolderpath}\{Guid.NewGuid().ToString().Replace("-", string.Empty)}_StocksReport.pdf";
                    rep.gSubSavePDF(fileNamePath);

                    rep.Dispose();//メモリ解放を行いたいため、この時点でDisposeする

                    replf.Add(fileNamePath);
                }
                // 印刷
                //rep.gSubPrint(printerName
                //    , isPdfOutput
                //    , @$"{savePdfFolderpath}\{DateTime.Now:yyyyMMddHHmmssfff}在庫一覧リスト.pdf"
                //    , toPrinterExePath
                //    );
            }
            //プリンターへ印刷指示
            PrintProcExecTask(
                replf
                , isPdfOutputOnly
                , printerName
                , toPrinterExePath
                , printProcInterval
                , printProcTimeout
                , printOrientation: PrintOrientation.Horizon //在庫リストは横向き印字設定のプリンター名を指定
                );

            return true;
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
            return false;
        }
    }



    //// </summary>
    ///// <param name="objReport"></param>
    ///// <param name="dt"></param>
    //public static bool 物品管理ラベル発行(string DEVICE_ID, string USER_ID, string PalletNo, string 社員コード, string 管理責任者, string 部署, string プロジェクト名, string 保管開始日, string 保管終了日, string 仕掛番号, string 内容)
    //{

    //    // 部署コード判定
    //    string 部署名;

    //    if (部署 == "174")
    //    {
    //        部署名 = "情報技術一課";
    //    }
    //    else if (部署 == "171")
    //    {
    //        部署名 = "情報技術二課";
    //    }
    //    else if (部署 == "172")
    //    {
    //        部署名 = "情報技術三課";
    //    }
    //    else if (部署 == "221")
    //    {
    //        部署名 = "制御技術課";
    //    }
    //    else if (部署 == "226")
    //    {
    //        部署名 = "計装技術課";
    //    }
    //    else
    //    {
    //        部署名 = 部署; // そのまま使う
    //    }

    //    string 部門 = "";
    //    string 課 = "";

    //    if (部署名.Contains("一課"))
    //    {
    //        部門 = 部署名.Replace("一課", ""); // 情報技術
    //        課 = "一";
    //    }
    //    if (部署名.Contains("二課"))
    //    {
    //        部門 = 部署名.Replace("二課", ""); // 情報技術
    //        課 = "二";
    //    }
    //    if (部署名.Contains("三課"))
    //    {
    //        部門 = 部署名.Replace("三課", ""); // 情報技術
    //        課 = "三";
    //    }



    //    // 1) バーコード画像生成（CODE_39）
    //    var writer = new BarcodeWriterPixelData
    //    {
    //        Format = BarcodeFormat.CODE_39,
    //        Options = new EncodingOptions
    //        {
    //            Width = 600,         // 印字品質優先で少し広めに
    //            Height = 160,        // 高さも十分に
    //            Margin = 5,          // トリミングされないよう少し余白
    //            PureBarcode = true
    //        }
    //    };

    //    // PalletNo -> ピクセルデータ
    //    var pixelData = writer.Write(PalletNo);

    //    // ピクセルデータからBitmapを作る (BGRA32想定)
    //    using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
    //    var rect = new Rectangle(0, 0, pixelData.Width, pixelData.Height);
    //    var bmpData = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
    //    try
    //    {
    //        // ZXingのpixelデータをBitmapへコピー
    //        System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bmpData.Scan0, pixelData.Pixels.Length);
    //    }
    //    finally
    //    {
    //        bitmap.UnlockBits(bmpData);
    //    }

    //    // 2) PNGにエンコードしてメモリに積む
    //    using var ms = new MemoryStream();
    //    bitmap.Save(ms, ImageFormat.Png);
    //    ms.Position = 0;

    //    // 3) 既存Excelを開いて値を書き込み + 画像を貼り付け
    //    string basePath = CommonInfo.AppDataPath; // テンプレートのフルパス
    //    if (!File.Exists(basePath)) return false;




    //    var 内容分割 = Split25(内容);

    //    using (var workbook = new XLWorkbook(basePath))
    //    {
    //        var ws = workbook.Worksheet("物品管理ラベル");

    //        // 文字セルの書き込み
    //        ws.Cell("B2").Value = PalletNo;
    //        ws.Cell("O3").Value = 管理責任者;
    //        ws.Cell("B3").Value = 部門;
    //        ws.Cell("H3").Value = 課;
    //        ws.Cell("G8").Value = プロジェクト名;
    //        ws.Cell("B15").Value = 保管開始日;
    //        ws.Cell("B16").Value = 保管終了日;
    //        ws.Cell("G7").Value = 仕掛番号;
    //        ws.Cell("B9").Value = 内容;

    //        // 1行目
    //        ws.Cell("B9").Value = 内容分割.Count > 0 ? 内容分割[0] : "";

    //        // 2行目（25文字超えたら）
    //        ws.Cell("B10").Value = 内容分割.Count > 1 ? 内容分割[1] : "";

    //        // 3行目
    //        ws.Cell("B11").Value = 内容分割.Count > 2 ? 内容分割[2] : "";

    //        // 2行目（25文字超えたら）
    //        ws.Cell("B12").Value = 内容分割.Count > 3 ? 内容分割[3] : "";

    //        // 3行目
    //        ws.Cell("B13").Value = 内容分割.Count > 4 ? 内容分割[4] : "";

    //        // 2行目（25文字超えたら）
    //        ws.Cell("B14").Value = 内容分割.Count > 5 ? 内容分割[5] : "";


    //        // S17にはバーコードの「画像」を貼る
    //        // 既に何か入れているなら消す（任意）
    //        // ws.Cell("S17").Clear();

    //        var picture = ws.AddPicture(ms)
    //                        .MoveTo(ws.Cell("L17"));  // セルS17の左上にアンカー

    //        // 4) サイズ調整（テンプレートの枠に合わせて適宜調整）
    //        //   セル結合や列幅/行高に合わせて WithSize を微調整してください
    //        picture.WithSize(300, 80);  // 例: 300x80ピクセル

    //        var today = DateTime.Now.ToString("yyyy-MM-dd");
    //        // 保存
    //        var outPath = $"物品管理ラベル{USER_ID}_{today}.xlsx";
    //        workbook.SaveAs(outPath);

    //        //CReport rep = new CReport();

    //        //rep.gSubSavePDF(outPath);

    //    }


    //    return true;



    //}




    // --- ここから追加（ダウンロード用の公開メソッド + 本体ヘルパー）---

    /// <summary>
    /// Controller から呼んで File(...) 返却するためのダウンロード用ラッパー
    /// bytes/fileName/mime を返すだけ。既存処理は一切壊さない。
    /// </summary>
    // </summary>
    /// <param name="objReport"></param>
    /// <param name="dt"></param>
    public static (byte[] bytes, string fileName, string mime) 物品管理ラベル発行(string DEVICE_ID, string USER_ID, string PalletNo, string 社員コード, string 管理責任者, string 部署, string プロジェクト名, string 保管開始日, string 保管終了日, string 仕掛番号, string 内容,string 部, string 課)
    {
        try
        {
            // CommonInfo.AppDataPath がファイル/フォルダ両対応になるようフォールバック
            string basePath = CommonInfo.AppDataPath;
            string templatePath = System.IO.File.Exists(basePath)
                ? basePath
                : System.IO.Path.Combine(basePath, "5-3 保管_物品管理ラベル.xlsx");

            if (!System.IO.File.Exists(templatePath))
            {
                var alt = System.IO.Path.Combine(basePath, "物品管理ラベル.xlsx");
                if (System.IO.File.Exists(alt)) templatePath = alt;
            }

            if (!System.IO.File.Exists(templatePath))
            {
                LogTo.Warning($"テンプレートが見つかりません path:[{templatePath}] base:[{basePath}]");
                return (Array.Empty<byte>(), "", "");
            }

            // メモリに .xlsx を生成
            var bytes = Create物品管理ラベルExcelBytes(
                templatePath, DEVICE_ID, USER_ID, PalletNo, 社員コード, 管理責任者,
                部署, プロジェクト名, 保管開始日, 保管終了日, 仕掛番号, 内容,部,課);

            if (bytes is null || bytes.Length == 0)
                return (Array.Empty<byte>(), "", "");

            var fileName = $"物品管理ラベル{USER_ID}_{DateTime.Now:yyyyMMdd}.xlsx";
            const string mime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return (bytes, fileName, mime);
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
            return (Array.Empty<byte>(), "", "");
        }
    }

    /// <summary>
    /// .xlsx をメモリに作って byte[] を返す本体。クラス内専用の private でOK。
    /// </summary>
    private static byte[] Create物品管理ラベルExcelBytes(
        string templateFullPath,
        string DEVICE_ID, string USER_ID, string PalletNo, string 社員コード, string 管理責任者,
        string 部署, string プロジェクト名, string 保管開始日, string 保管終了日, string 仕掛番号, string 内容,string 部,string 課)
    {
       
        // --- バーコード（ZXing：完全修飾で名前衝突回避） ---
        var writer = new ZXing.BarcodeWriterPixelData
        {
            Format = ZXing.BarcodeFormat.CODE_39,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = 600,
                Height = 160,
                Margin = 5,
                PureBarcode = true
            }
        };
        var pixelData = writer.Write(PalletNo);

        // --- PNG化（System.Drawing を現状維持） ---
        using var barcodePng = new System.IO.MemoryStream();
        using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        {
            var rect = new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height);
            var bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bmpData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }
            bitmap.Save(barcodePng, System.Drawing.Imaging.ImageFormat.Png);
        }
        barcodePng.Position = 0; // ←重要

        // --- Excel 編集（ClosedXML） ---
        using var workbook = new ClosedXML.Excel.XLWorkbook(templateFullPath);
        var ws = workbook.Worksheet("物品管理ラベル");

        ws.Cell("B2").Value = PalletNo;
        ws.Cell("O3").Value = 管理責任者;
        ws.Cell("B3").Value = 部;
        ws.Cell("H3").Value = 課;
        ws.Cell("G8").Value = プロジェクト名;
        ws.Cell("B15").Value = 保管開始日;
        ws.Cell("B16").Value = 保管終了日;
        ws.Cell("G7").Value = 仕掛番号;

        var 内容分割 = Split25(内容);
        ws.Cell("B9").Value = 内容分割.Count > 0 ? 内容分割[0] : "";
        ws.Cell("B10").Value = 内容分割.Count > 1 ? 内容分割[1] : "";
        ws.Cell("B11").Value = 内容分割.Count > 2 ? 内容分割[2] : "";
        ws.Cell("B12").Value = 内容分割.Count > 3 ? 内容分割[3] : "";
        ws.Cell("B13").Value = 内容分割.Count > 4 ? 内容分割[4] : "";
        ws.Cell("B14").Value = 内容分割.Count > 5 ? 内容分割[5] : "";

        var pic = ws.AddPicture(barcodePng).MoveTo(ws.Cell("J17"));
        pic.WithSize(300, 80);


        //２枚目のラベルにも同じ内容
        ws.Cell("B20").Value = PalletNo;
        ws.Cell("O21").Value = 管理責任者;
        ws.Cell("B21").Value = 部;
        ws.Cell("H21").Value = 課;
        ws.Cell("G26").Value = プロジェクト名;
        ws.Cell("B33").Value = 保管開始日;
        ws.Cell("B34").Value = 保管終了日;
        ws.Cell("G25").Value = 仕掛番号;

        var 内容分割2 = Split25(内容);
        ws.Cell("B27").Value = 内容分割2.Count > 0 ? 内容分割2[0] : "";
        ws.Cell("B28").Value = 内容分割2.Count > 1 ? 内容分割2[1] : "";
        ws.Cell("B29").Value = 内容分割2.Count > 2 ? 内容分割2[2] : "";
        ws.Cell("B30").Value = 内容分割2.Count > 3 ? 内容分割2[3] : "";
        ws.Cell("B31").Value = 内容分割2.Count > 4 ? 内容分割2[4] : "";
        ws.Cell("B32").Value = 内容分割2.Count > 5 ? 内容分割2[5] : "";

        var pic2 = ws.AddPicture(barcodePng).MoveTo(ws.Cell("J35"));
        pic2.WithSize(300, 80);


        using var outMs = new System.IO.MemoryStream();
        workbook.SaveAs(outMs);
        return outMs.ToArray();
    }

    //    // --- ここまで追加 ---


    // 25文字ずつ分割
    static List<string> Split25(string text)
    {
        var list = new List<string>();
        if (string.IsNullOrEmpty(text)) return list;

        for (int i = 0; i < text.Length; i += 25)
        {
            list.Add(text.Substring(i, Math.Min(25, text.Length - i)));
        }
        return list;
    }





    // --- ここから追加（ダウンロード用の公開メソッド + 本体ヘルパー）---

    /// <summary>
    /// Controller から呼んで File(...) 返却するためのダウンロード用ラッパー
    /// bytes/fileName/mime を返すだけ。既存処理は一切壊さない。
    /// </summary>
    // </summary>
    /// <param name="objReport"></param>
    /// <param name="dt"></param>
    public static (byte[] bytes2, string fileName2, string mime2) バーコードラベル発行(string DEVICE_ID, string USER_ID, string 棚番)
    {
        try
        {
            // CommonInfo.AppDataPath がファイル/フォルダ両対応になるようフォールバック
            string basePath = CommonInfo.AppDataPath2;
            string templatePath = System.IO.File.Exists(basePath)
                ? basePath
                : System.IO.Path.Combine(basePath, "バーコード印刷用.xlsx");

            if (!System.IO.File.Exists(templatePath))
            {
                var alt = System.IO.Path.Combine(basePath, "バーコード印刷.xlsx");
                if (System.IO.File.Exists(alt)) templatePath = alt;
            }

            if (!System.IO.File.Exists(templatePath))
            {
                LogTo.Warning($"テンプレートが見つかりません path:[{templatePath}] base:[{basePath}]");
                return (Array.Empty<byte>(), "", "");
            }

            // メモリに .xlsx を生成
            var bytes = CreateバーコードラベルExcelBytes(
                templatePath, DEVICE_ID, USER_ID, 棚番);

            if (bytes is null || bytes.Length == 0)
                return (Array.Empty<byte>(), "", "");

            var fileName = $"バーコード印刷.xlsx";
            const string mime2 = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return (bytes, fileName, mime2);
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
            return (Array.Empty<byte>(), "", "");
        }
    }

    /// <summary>
    /// .xlsx をメモリに作って byte[] を返す本体。クラス内専用の private でOK。
    /// </summary>
    private static byte[] CreateバーコードラベルExcelBytes(string templateFullPath, string DEVICE_ID, string USER_ID, string 棚番)
    {

        // --- バーコード（ZXing：完全修飾で名前衝突回避） ---
        var writer = new ZXing.BarcodeWriterPixelData
        {
            Format = ZXing.BarcodeFormat.CODE_39,
            Options = new ZXing.Common.EncodingOptions
            {
                Width = 600,
                Height = 160,
                Margin = 5,
                PureBarcode = true
            }
        };
        var pixelData = writer.Write(棚番);

        // --- PNG化（System.Drawing を現状維持） ---
        using var barcodePng = new System.IO.MemoryStream();
        using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        {
            var rect = new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height);
            var bmpData = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bmpData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }
            bitmap.Save(barcodePng, System.Drawing.Imaging.ImageFormat.Png);
        }
        barcodePng.Position = 0; // ←重要

        // --- Excel 編集（ClosedXML） ---
        using var workbook = new ClosedXML.Excel.XLWorkbook(templateFullPath);
        var ws = workbook.Worksheet("バーコードラベル");

        ws.Cell("B4").Value = 棚番;

        var pic = ws.AddPicture(barcodePng).MoveTo(ws.Cell("A1"));
        pic.WithSize(300, 80);


    　　

        using var outMs = new System.IO.MemoryStream();
        workbook.SaveAs(outMs);
        return outMs.ToArray();
    }

















    /// <summary>
    /// 在庫一覧リスト出力
    /// </summary>
    /// <param name="objReport"></param>
    /// <param name="dt"></param>
    private static int Proc在庫一覧リスト出力(CReport objReport, List<ResponseValue> dt, bool 再発行, int reStartCout, int chankItemCount, int chankPageCount)
    {
        DateTime dteNow = DateTime.Now;
        ResponseValue drHeader = new();
        ResponseValue drHeader2 = new();
        if (null != dt && dt.Count > 0)
        {
            for (int i = 0; dt[0].Columns.Count > i; i++)
            {
                drHeader.Values.Add(dt[0].Columns[i], "$$$");
                drHeader2.Values.Add(dt[0].Columns[i], "$$$");
            }
        }
        int endCount = chankItemCount + reStartCout >= dt.Count ? dt.Count : chankItemCount + reStartCout;
        int retCount = endCount >= dt.Count ? dt.Count : endCount; //念のためセットしおく
        // 明細データを全件ループ
        for (int i = reStartCout; endCount > i; i++)
        {
            // 改ページチェック
            if (drHeader.Values["大ヘッダ"].ToString() != dt[i].Values["大ヘッダ"].ToString())
            {
                foreach (KeyValuePair<string, object> value in dt[i].Values)
                {
                    drHeader.Values[value.Key] = value.Value;
                }
                // -----------------------------------------------------------------
                // メインヘッダ出力
                // -----------------------------------------------------------------
                objReport.pCellReport.Page.Repeat(1, 6);
                objReport.gSubField("C出力日付", dteNow.ToString("yyyy/MM/dd"));
                objReport.gSubField("C出力時刻", dteNow.ToString("HH:mm:ss"));
                objReport.gSubField("倉庫コード", drHeader);
                objReport.gSubField("倉庫名", drHeader);
                objReport.gSubField("ゾーンコード", drHeader);
                objReport.gSubField("ゾーン名", drHeader);
                objReport.gSubField("パレット数", drHeader);
                objReport.pCellReport.Page.Next(false);
            }

            if (drHeader2.Values["ヘッダ"].ToString() != dt[i].Values["ヘッダ"].ToString())
            {
                foreach (KeyValuePair<string, object> value in dt[i].Values)
                {
                    drHeader2.Values[value.Key] = value.Value;
                }
                // -----------------------------------------------------------------
                // ヘッダ出力
                // -----------------------------------------------------------------
                objReport.pCellReport.Page.Repeat(7, 7);
                objReport.gSubField("品名", drHeader2);
                objReport.gSubField("出荷者", drHeader2);
                objReport.gSubField("産地", drHeader2);
                objReport.gSubField("課名", drHeader2);
                objReport.gSubField("入荷No", drHeader2);
                objReport.pCellReport.Page.Next(false);
            }

            // -----------------------------------------------------------------
            // 明細出力
            // -----------------------------------------------------------------
            objReport.pCellReport.Page.Repeat(14, 2);
            objReport.gSubField("明細No", dt[i]);
            objReport.gSubField("等階級", dt[i]);
            objReport.gSubField("換算値", dt[i]);
            objReport.gSubField("説明", dt[i]);
            objReport.gSubField("入数", dt[i]);
            objReport.gSubField("ケース数", dt[i]);
            objReport.gSubField("バラ数", dt[i]);
            objReport.gSubField("ロケーション名", dt[i]);
            objReport.gSubField("パレットNo", dt[i]);
            objReport.gSubField("入庫日時", dt[i]);

            // -----------------------------------------------------------------
            // 改ページ判断
            // -----------------------------------------------------------------
            int nNextPg = i + 1;
            retCount = nNextPg;
            if (dt.Count <= nNextPg ||
                drHeader.Values["大ヘッダ"].ToString() != dt[nNextPg].Values["大ヘッダ"].ToString())
            {
                // 印字終了文字の出力
                objReport.pCellReport.Page.Next(false);
                objReport.pCellReport.Page.Repeat(16, 3);
                objReport.pCellReport.Page.Next(true);
            }
            else
            {
                //改行しない場合はチャンクの明細数を超えてもページ内に含めたいのでendCountを加算する
                endCount
                    = endCount <= nNextPg
                    && drHeader.Values["大ヘッダ"].ToString() == dt[nNextPg].Values["大ヘッダ"].ToString() //上のifの条件の反例なので、不要だが念のため追加しておく
                    ? endCount + 1 : endCount;
                objReport.pCellReport.Page.Next(false);
            }
        }
        return retCount;
    }

    #endregion

    /// <summary>
    /// 印刷に使用するプリンター名を取得する
    /// </summary>
    /// <param name="DEVICE_ID"></param>
    /// <returns></returns>
    private static string GetPrinterName(string DEVICE_ID)
    {
        string PrinterName = string.Empty;
        ClassNameSelect ptselect = new()
        {
            viewName = "VW_プリンタ名取得"
        };
        ptselect.whereParam.Add("DEVICE_ID", new WhereParam { val = DEVICE_ID });
        List<ResponseValue> ptresponseValues = CommonController.GetResponseValue(ptselect);
        if (ptresponseValues is not null && ptresponseValues.Count > 0)
        {
            PrinterName = ptresponseValues[0].Values["プリンタ名"].ToString()!;
        }
        return PrinterName;
    }

    /// <summary>
    /// 印刷関連の設定値をappsettings.jsonから読み取る
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    private static (bool isPdfOutputOnly
        , string savePdfFolderPath
        , string toPrinterExePath
        , int chankItemCount
        , int chankPageCount
        , int printProcInterval
        , int printProcTimeout
        ) GetPrintProcSettings()
    {
        //TODO 戻り値は、変数が多くなると見にくいので、素直にreadOnlyの印刷設定用のクラスを返すべきでは？ただし、tupleのほうがパフォーマンスに優れる気がする。一度ベンチをとるべき。
        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        string strPdfFolderPath = config.GetValue<string>("PrintOutputSetting:SavePdfFolderPath") ?? throw new NullReferenceException();
        if (!Directory.Exists(strPdfFolderPath))
        {
            _ = Directory.CreateDirectory(strPdfFolderPath);
        }
        int chankItemCount = config.GetValue<int>("PrintOutputSetting:ChankItemCount");
        int chankPageCount = config.GetValue<int>("PrintOutputSetting:ChankPageCount");
        int printProcInterval = config.GetValue<int>("PrintOutputSetting:PrintProcInterval");
        int printProcTimeout = config.GetValue<int>("PrintOutputSetting:PrintProcTimeout");
        string toPrinterExePath = config.GetValue<string>("PrintOutputSetting:ToPrinterExePath") ?? throw new NullReferenceException();
        return (config.GetValue<bool>("PrintOutputSetting:IsPdfOutputOnly")
            , strPdfFolderPath
            , toPrinterExePath
            , chankItemCount
            , chankPageCount
            , printProcInterval
            , printProcTimeout
            );
    }
    /// <summary>
    /// プリンタ名、セマフォの管理用
    /// </summary>
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> semaphoreDic = new();
    /// <summary>
    /// プリンターへ印刷指示を行うタスク
    /// </summary>
    /// <param name="pdfFiles"></param>
    /// <param name="isPdfOutputOnly"></param>
    /// <param name="printerName"></param>
    /// <param name="toPrinterExePath"></param>
    /// <param name="printProcInterval"></param>
    /// <param name="printProcTimeout"></param>
    private static void PrintProcExecTask(
        List<string> pdfFiles
        , bool isPdfOutputOnly
        , string printerName
        , string toPrinterExePath
        , int printProcInterval
        , int printProcTimeout
        , PrintOrientation printOrientation = PrintOrientation.Invalid
        )
    {
        CancellationTokenSource tokenSource = new();
        // タイムアウトの設定（ms）
        tokenSource.CancelAfter(printProcTimeout);
        CancellationToken ct = tokenSource.Token;
        //プリンター名をキーとして、semaphoreslimで印刷タスクを制御する
        SemaphoreSlim ss;
        if (semaphoreDic.TryGetValue(printerName, out SemaphoreSlim? semaphore))
        {
            //LogTo.Information($"2回目以降のsemaphoreDic.TryGetValue printerName:[{printerName}]");
            ss = semaphore;
        }
        else
        {
            //初回のみセマフォを生成する
            //LogTo.Information($"初回のみセマフォを生成するsemaphoreDic printerName:[{printerName}]");
            ss = new SemaphoreSlim(1, 1); //1スレッドに1件のみ
            semaphoreDic[printerName] = ss;
        }
        Task task = Task.Run(async () =>
        {
            if (isPdfOutputOnly)
            {
                return;
            }
            ss.Wait();//待たせる
            try
            {
                foreach (string rf in pdfFiles)
                {
                    // キャンセル要求があったかどうかを確認
                    ct.ThrowIfCancellationRequested();
                    // 印刷
                    gSubPrint(
                        printerName
                        //, isPdfOutputOnly
                        //, @$"{savePdfFolderpath}\{DateTime.Now:yyyyMMddHHmmssfff}在庫一覧リスト.pdf"
                        , rf
                        , toPrinterExePath
                        , GetPrintOrientStr(printOrientation)//プリンター名の後ろにつけているであろう_V,_Hの文字列を取得する
                        , printProcTimeout
                        );
                    await Task.Delay(printProcInterval);//ms待たせる
                }
            }
            catch (Exception e)
            {
                LogTo.Fatal(e.Message);
            }
            finally
            {
                _ = ss.Release();//セマフォは必ずリリースする
            }

        }, tokenSource.Token);
        _ = Task.Run(() =>
        {
            try
            {
                if (!task.Wait(printProcTimeout))
                {
                    tokenSource.Cancel();
                    throw new OperationCanceledException("印刷タスクがタイムアウトしました。");
                };

                // タスクが正常に完了した場合
                // ここに成功時の処理を記述
                LogTo.Information($"印刷タスク_全完了");
            }
            catch (OperationCanceledException e)
            {
                //Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
                LogTo.Warning($"印刷タスク_タイムアウト_{e.Message}");
                task.Dispose();
                return;
            }
            finally
            {
                tokenSource.Dispose();
                _ = ss.Release();//念のためここでもセマフォをリリースしておく。
            }

        });
    }

    /// <summary>
    /// 印字向き指定のenum
    /// </summary>
    public enum PrintOrientation : int
    {
        /// <summary>
        /// 指定なし
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// 縦
        /// </summary>
        Virtual = 1,
        /// <summary>
        /// 横
        /// </summary>
        Horizon = 2
    }
    private static string GetPrintOrientStr(PrintOrientation en)
    {
        return printOrientStr[(int)en];
    }

    /// <summary>
    /// プリンタードライバー名称の後ろにつけて判断する
    /// </summary>
    private static readonly string[] printOrientStr = new string[] {
        "",
        "_V",
        "_H"
    };

    /// <summary>
    /// 印刷処理
    /// </summary>
    public static void gSubPrint(
        string printerName
        //, bool isPdfOutputOnly
        , string savePdfFilename
        , string toPrinterExePath
        , string printOrientStr //縦横の印刷のプリンター名は_V,_Hをつけるルールとする
        , int printProcTimeout //ms
        )
    {
        try
        {

            Log.Information($"印刷開始 printerName:[{printerName}],toPrinterExePath:[{toPrinterExePath}]");

            if (!File.Exists(savePdfFilename))
            {
                Log.Warning($"印刷対象のPDFファイルが存在しません。 savePdfFilename:[{savePdfFilename}]");
                return;
            }

            //エクセルを使用して印刷する
            //pCellReport.Report.PrintOut(printerName, "1-99999999");

            //※pCellReport.Report.PrintOutはエクセルがインストールされていないと印刷ができなかったので、
            //pdfを出力して、pdfファイルを印刷する方針に変更

            //プリンタ名が空白の場合はデフォルトプリンターへ印刷してしまうので、印刷処理は実施しない
            if (string.IsNullOrWhiteSpace(printerName))
            {
                Log.Warning($"印刷未実施 printerName:[{printerName}]");
                return;
            }

            ////Adobe.exeを起動して印刷する
            //TODO 実行前に Adobe.exeが残っている場合はKillする必要がないか検討。原則は、印刷が完了するならfinalyでkillするので大丈夫なはず。
            //string exePath = @toPrinterExePath; //Adobe Reader DCのexeパスが入っているとする
            //if (File.Exists(exePath))
            //{
            //    string arguments = @$"/n /s /t {savePdfFilename} {printerName}";//コマンドライン引数を定義
            //    ProcessStartInfo startInfo = new(exePath, arguments);
            //    System.Diagnostics.Process process = new()
            //    {
            //        StartInfo = startInfo
            //    };
            //    try
            //    {
            //        if (process.Start())
            //        {
            //            //印刷プロセス自体は呼び出し元で別タスクとして動作しているはずなので、ここで待機するようにする。
            //            //process.WaitForExit();
            //            //秒待機する。//Adobeが印刷を待つため　
            //            Thread.Sleep(10000); //TODO Adobe印刷を採用するなら設定ファイルに定義してパラメータ化する
            //        }
            //        else
            //        {
            //            Log.Warning(@$"印刷実行ができませんでした。 printerName:[{printerName}],savePdfFilename[{savePdfFilename}]");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Fatal(ex.Message);
            //    }
            //    finally
            //    {
            //        //TODO 他複数端末が同時にAdobeを起動して印刷を実行している可能性はないか、要検証。
            //        //Adobeのプロセスをkillする
            //        System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcessesByName("AcroRd32");
            //        foreach (System.Diagnostics.Process p in ps)
            //        {
            //            // プロセスを強制的に終了させる
            //            p.Kill();
            //        }
            //    }

            //}
            //else
            //{
            //    //TODO Adobeがインストールされていない処理
            //}

            //PdfToPrinter_nugetを使用してPDFを印刷する
            string filePath = @savePdfFilename;
            string networkPrinterName = $"{printerName}{printOrientStr}";//縦横印刷用のプリンター名を設定(後ろに_V,_H文字がついているプリンター名が存在することが前提)
            TimeSpan printTimeout = TimeSpan.FromMilliseconds(printProcTimeout);//印刷タイムアウトを設定 //TODO 本格採用の場合はパラメータ化する

            PrinterSettings printerSettings = new() //プリンターが利用可能かどうか確認するために使用。※windows環境のみ判定可能である点に注意
            {
                PrinterName = networkPrinterName
            };
            if (!printerSettings.IsValid)
            {
                Log.Warning($"printerSettings.IsValid == false networkPrinterName:[{networkPrinterName}]");
                //縦横印刷用のプリンター名が利用不可なら、そのままの名称で印刷を実施する
                networkPrinterName = printerName;
            }
            PDFtoPrinterPrinter printer = new();
            Task t = printer.Print(new PrintingOptions(networkPrinterName, filePath), printTimeout);
            t.Wait();//印刷完了まで待機する

            Log.Information($"印刷完了 printerName:[{printerName}]");

        }
        catch (Exception ex)
        {
            Log.Fatal(ex.Message);
        }
    }

}



using AdvanceSoftware.VBReport;
using AdvanceSoftware.VBReport.BarCode;
using Anotar.Serilog;
using Serilog;
using SharedModels;


namespace ExpressionDBWebAPI.ReportUtil;

public class CReport
{
    private readonly string basePath;

    public CellReport pCellReport { get; set; }

    /// <summary>
    /// コンストラクタ
    /// 西山
    /// バーコードを作成？
    /// </summary>
    /// <param name="_basePath"></param>
    public CReport(string _basePath = "")
    {
        pCellReport = new CellReport();
        basePath = _basePath;
        // 実行中のEXEのディレクトリパスを取得
        string exePath = AppDomain.CurrentDomain.BaseDirectory;
        // ライセンスファイルのフルパスを組み立て
        string licenseFilePath = Path.Combine(exePath, "AdvanceSoftware.VBReport.License.xml");
        // ファイルが存在するかチェック
        if (File.Exists(licenseFilePath))
        {
            // ファイルが存在する場合、ライセンスファイルを指定
            pCellReport.SetLicense(@licenseFilePath);
        }
    }
    /// <summary>
    /// pCellReportをdisposeするメソッド。明示的にメモリ解放のため追加。
    /// </summary>
    public void Dispose()
    {
        pCellReport.Dispose();
    }

    /// <summary>
    /// レポートのフィールドに値をセットする。
    /// </summary>
    /// <param name="strFieldName">レポート側のデータを送るフィールド名</param>
    /// <param name="row">DataRow</param>
    public void gSubField(string strFieldName, ResponseValue row)
    {
        if (row.Values != null && row.Values.ContainsKey(strFieldName))
        {
            gSubField(strFieldName, row.Values[strFieldName]);
        }
        else
        {
            gSubField(strFieldName, "");
        }
    }
    public void gSubField(string strFieldName, object objValue, bool blnNum = false)
    {
        pCellReport.Cell("**" + strFieldName).Value = objValue is DBNull ? blnNum ? 0 : "" : objValue is null ? "" : objValue;
    }

    /// <summary>
    /// レポートのフィールドに値をセットして値を戻す
    /// </summary>
    /// <param name="strFieldName">レポート側のデータを送るフィールド名</param>
    /// <param name="row">DataRow</param>
    /// <returns></returns>
    public int gIntField(string strFieldName, ResponseValue row)
    {
        int nVal = 0;
        if (row.Values != null && row.Values.ContainsKey(strFieldName))
        {
            _ = int.TryParse(row.Values[strFieldName].ToString(), out nVal);
        }
        gSubField(strFieldName, nVal);
        return nVal;
    }
    public long gLongField(string strFieldName, ResponseValue row)
    {
        long lVal = 0;
        if (row.Values != null && row.Values.ContainsKey(strFieldName))
        {
            _ = long.TryParse(row.Values[strFieldName].ToString(), out lVal);
        }
        gSubField(strFieldName, lVal);
        return lVal;
    }

    /// <summary>
    /// レポートのフィールドにバーコードをセットする
    /// </summary>
    /// <param name="strFieldName">レポート側のデータを送るフィールド名</param>
    /// <param name="objBarCode">BarCodeクラスオブジェクト</param>
    public void gSubFieldBarcode(string strFieldName, BarCode objBarCode)
    {
        BarCodeSizeD size;
        size = objBarCode.GetScaleSize(Unit.Millimeter);
        pCellReport.Cell("**" + strFieldName).Drawing.AddImage(objBarCode.GetBarCodeImage(System.Drawing.Imaging.ImageFormat.Png), size.Width, size.Height);
    }


    public void gSubFieldCode39(string strFieldName, ResponseValue row, double dblElement = 7.0, double dblHeight = 0.0, double dblCharSize = 0.0)
    {
        if (row.Values != null && row.Values.ContainsKey(strFieldName))
        {
            gSubFieldCode39(strFieldName, row.Values[strFieldName].ToString(), dblElement, dblHeight, dblCharSize);
        }
        else
        {
            gSubFieldCode39(strFieldName, "", dblElement, dblHeight, dblCharSize);
        }
    }

    //<summary>
    //レポートのフィールドにバーコード(CODE39)を出力する
    //</summary>
    //<param name="strFieldName">レポート側のデータを送るフィールド名</param>
    //<param name="strValue">バーコードのデータ(文字列)</param>
    //<param name="dblElement">最小モジュール値　※大きくなるほどバーコードの横幅が大きくなります(デフォルト 7)。6～10程度で指定してください。</param>
    //<param name="dblHeight">バーコードの高さ(ピクセル)　※0を指定すると自動調整</param>
    //<param name="dblCharSize">バーコード文字のサイズ(ピクセル)　※0を指定すると自動調整 ※0より小さい数値を指定すると文字表示なし</param>
    public void gSubFieldCode39(string strFieldName, string strValue, double dblElement = 7.0, double dblHeight = 0.0, double dblCharSize = -1.0)
    {

        if (dblHeight < 0.0)
        {
            dblHeight = 2.0;
        }

        BarCode objBC = new();
        // 実行中のEXEのディレクトリパスを取得
        string exePath = AppDomain.CurrentDomain.BaseDirectory;
        // ライセンスファイルのフルパスを組み立て
        string licenseFilePath = Path.Combine(exePath, "AdvanceSoftware.VBReport.License.xml");
        // ファイルが存在するかチェック
        if (File.Exists(licenseFilePath))
        {
            // ファイルが存在する場合、ライセンスファイルを指定
            objBC.SetLicense(@licenseFilePath);
        }
        // バーコードのデータを設定
        //先頭と末尾にアスタリスクがある場合は削る
        if (strValue.Contains("*"))
        {
            strValue = strValue.Replace("*", string.Empty);

        }
        objBC.Value = strValue;
        // バーコード種別(CODE39)
        objBC.Type = BarCodeType.Code39;
        // サイズ単位(ミリメートル)
        objBC.Unit = Unit.Pixel;
        // x 方向の解像度
        objBC.DpiX = 600;
        // y 方向の解像度
        objBC.DpiY = 600;
        // バーコードの最小モジュール値（ピクセル）
        objBC.Element = dblElement;
        // バーコードの高さ(0は自動調整)
        objBC.BarHeight = dblHeight;
        // チェックデジットは付加しない
        objBC.CheckCharMode = false;
        // フォント指定
        objBC.Font = new System.Drawing.Font("ＭＳ ゴシック", 9);
        // フォントの高さ指定
        if (dblCharSize >= 0.0)
        {
            objBC.CharSize = dblCharSize;
        }
        // バーコードメッセージの付加
        objBC.ShowMessage = dblCharSize >= 0.0;

        // バーコードの画像サイズ取得
        BarCodeSizeD size;
        size = objBC.GetScaleSize(Unit.Millimeter);

        // バーコード出力
        pCellReport.Cell("**" + strFieldName).Drawing.AddImage(objBC.GetBarCodeImage(System.Drawing.Imaging.ImageFormat.Png), size.Width, size.Height);
    }

    //<summary>
    //レポートのフィールドにバーコード(JAN13)を出力する
    //</summary>
    //<param name="strFieldName">レポート側のデータを送るフィールド名</param>
    //<param name="row">DataRow</param>
    //<param name="dblElement">最小モジュール値　※大きくなるほどバーコードの横幅が大きくなります(デフォルト 7)。6～10程度で指定してください。</param>
    //<param name="dblHeight">バーコードの高さ(ピクセル)　※0を指定すると自動調整</param>
    //<param name="dblCharSize">バーコード文字のサイズ(ピクセル)　※0を指定すると文字表示なし</param>
    public void gSubFieldCodeJAN13(string strFieldName, ResponseValue row, double dblElement = 7.0, double dblHeight = 0.0, double dblCharSize = 0.0)
    {
        if (row.Values != null && row.Values.ContainsKey(strFieldName))
        {
            gSubFieldCodeJAN13(strFieldName, row.Values[strFieldName].ToString(), dblElement, dblHeight, dblCharSize);
        }
        else
        {
            gSubFieldCodeJAN13(strFieldName, "", dblElement, dblHeight, dblCharSize);
        }
    }

    //<summary>
    //レポートのフィールドにバーコード(JAN13)を出力する
    //</summary>
    //<param name="strFieldName">レポート側のデータを送るフィールド名</param>
    //<param name="strValue">バーコードのデータ(文字列)</param>
    //<param name="dblElement">最小モジュール値　※大きくなるほどバーコードの横幅が大きくなります(デフォルト 7)。6～10程度で指定してください。</param>
    //<param name="dblHeight">バーコードの高さ(ピクセル)　※0を指定すると自動調整</param>
    //<param name="dblCharSize">バーコード文字のサイズ(ピクセル)　※0を指定すると自動調整 ※0より小さい数値を指定すると文字表示なし</param>
    public void gSubFieldCodeJAN13(string strFieldName, string strValue, double dblElement = 7.0, double dblHeight = 0.0, double dblCharSize = -1.0, int intZoom = 100)
    {

        if (dblHeight < 0.0)
        {
            dblHeight = 2.0;
        }

        BarCode objBC = new();
        // 実行中のEXEのディレクトリパスを取得
        string exePath = AppDomain.CurrentDomain.BaseDirectory;
        // ライセンスファイルのフルパスを組み立て
        string licenseFilePath = Path.Combine(exePath, "AdvanceSoftware.VBReport.License.xml");
        // ファイルが存在するかチェック
        if (File.Exists(licenseFilePath))
        {
            // ファイルが存在する場合、ライセンスファイルを指定
            pCellReport.SetLicense(@licenseFilePath);
        }
        // バーコードのデータを設定
        //先頭と末尾にアスタリスクがある場合は削る
        if (strValue.Contains("*"))
        {
            strValue = strValue.Replace("*", string.Empty);
        }
        objBC.Value = strValue;
        // バーコード種別(CODE39)
        objBC.Type = BarCodeType.JAN13;
        // サイズ単位(ミリメートル)
        objBC.Unit = Unit.Pixel;
        // x 方向の解像度
        objBC.DpiX = 600;
        // y 方向の解像度
        objBC.DpiY = 600;
        // バーコードの最小モジュール値（ピクセル）
        objBC.Element = dblElement;
        // バーコードの高さ(0は自動調整)
        objBC.BarHeight = dblHeight;

        // バーコードイメージの倍率（省略可）JAN / UPC の場合、設定範囲は 80% ～ 200% です。
        objBC.Zoom = intZoom <= 80 ? 80 : intZoom >= 200 ? 200 : intZoom;

        // チェックデジットは付加しない
        objBC.CheckCharMode = false;
        // フォント指定
        objBC.Font = new System.Drawing.Font("ＭＳ ゴシック", 9);
        // フォントの高さ指定
        if (dblCharSize >= 0.0)
        {
            objBC.CharSize = dblCharSize;
        }
        // バーコードメッセージの付加
        objBC.ShowMessage = dblCharSize >= 0.0;

        // バーコードの画像サイズ取得
        BarCodeSizeD size;
        size = objBC.GetScaleSize(Unit.Millimeter);

        //' バーコード出力
        pCellReport.Cell("**" + strFieldName).Drawing.AddImage(objBC.GetBarCodeImage(System.Drawing.Imaging.ImageFormat.Png), size.Width, size.Height);
    }

    public void Open(string fileName)
    {
        try
        {
            string designFilePath = Path.Combine(basePath, fileName);    // テンプレート Excel ファイル

            // 帳票作成処理（デザインファイル使用）
            pCellReport.FileName = designFilePath;
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
        }
    }

    public void Start(string strSheetName)
    {
        try
        {
            pCellReport.Report.Start(ReportMode.Speed);
            pCellReport.Report.File();


            // 帳票ページの作成
            pCellReport.Page.Start(strSheetName, "1-99999");
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
        }
    }

    public void End()
    {
        try
        {
            pCellReport.Page.End();

            // 帳票終了処理
            pCellReport.Report.End();
        }
        catch (Exception ex) { LogTo.Fatal(ex.Message); }
    }

    /// <summary>
    /// 本クラスにReportEndを行ったデータをPDFファイルとして保存する
    /// </summary>
    /// <param name="savePdfFilename"></param>
    public void gSubSavePDF(string savePdfFilename)
    {
        try
        {
            Log.Information($"pdf出力開始 fileName:[{savePdfFilename}]");
            //PDFファイルを保存する
            pCellReport.Report.SavePdf(savePdfFilename);
            Log.Information($"pdf出力完了 fileName:[{savePdfFilename}]");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex.Message);
        }
    }


    public byte[] gByPdfData()
    {
        //【1】-------------------------------------------------------------
        // Report.End 後に Report.SaveAs で Excel ファイルを出力します。
        // 引数にはファイルパスか、出力先のメモリストリームが指定可能です。
        // ※ Report.Start メソッドの引数に ReportMode.Legacy を設定した場
        // 合、実行環境に Excel のインストールが必要です。
        // -----------------------------------------------------------------
        MemoryStream memoryStream = new();
        pCellReport.Report.SavePdf(memoryStream);

        //【2】-------------------------------------------------------------
        // PDF 形式の帳票を出力。
        // -----------------------------------------------------------------
        return memoryStream.ToArray();
    }

    public byte[] gByExcelData()
    {
        //【1】-------------------------------------------------------------
        // Report.End 後に Report.SaveAs で Excel ファイルを出力します。
        // 引数にはファイルパスか、出力先のメモリストリームが指定可能です。
        // ※ Report.Start メソッドの引数に ReportMode.Legacy を設定した場
        // 合、実行環境に Excel のインストールが必要です。
        // -----------------------------------------------------------------
        MemoryStream memoryStream = new();
        pCellReport.Report.SaveAs(memoryStream);

        //【2】-------------------------------------------------------------
        // Excel 形式の帳票を出力。
        // -----------------------------------------------------------------
        return memoryStream.ToArray();
    }
}

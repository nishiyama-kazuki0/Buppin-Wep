using ExpressionDBBlazorShared.Data;
using Microsoft.JSInterop;

namespace ExpressionDBBlazorShared.Services;

/// <summary>
/// スキャン情報
/// 西山　バーコードの情報
/// </summary>
public class ScanData
{
    public string strDecodeResult;          // 読み取り結果
                                            //  SUCCESS 成功
                                            //  SUCCESS_TEMPORARY 読み取り成功＋アラートあり(OCR 警告画面表示「常に表示」時のみ)
                                            //  UPDATE_COLLECTION_DATA 撮像ごとの読み取り成功(累積読み時のみ)
                                            //  ALERT アラート発生(OCR のみ)
                                            //  TIMEOUT タイムアウト
                                            //  CANCELED 読み取り中止
                                            //  FAILED 読み取り失敗
    public string strCodeType;              // 読み取りコード種
                                            //  UPC/EAN/JAN
                                            //  Code128
                                            //  Code39
                                            //  ITF
                                            //  Datamatrix
                                            //  QRCode
                                            //  PDF417
                                            //  Industrial 2of5
                                            //  Codabar(NW7)
                                            //  COOP2of5
                                            //  Code93
                                            //  Composite AB(GS1-Databar)
                                            //  Composite AB(EAN/UPC)
                                            //  Composite(GS1-128)
                                            //  Postal
                                            //  OCR
    public string strStringData;            // 読み取りデータの文字列
    public ScanData()
    {
        strDecodeResult = "";
        strCodeType = "";
        strStringData = "";
    }
}

public class HtService
{
    //public readonly IJSRuntime JS;
    //public static ScanData ScanData { get; set; } = new ScanData();
    public static string DebugText { get; set; } = "";

    public delegate Task HtScanEventArg(ScanData? scanData);
    public static event HtScanEventArg? HtScanEvent;

    private static IScannerService _scannerService = null!;

    public HtService(DeviceInfoService deviceInfo, IJSRuntime js)
    {
        //JS = js;
        _scannerService = ScannerFactory.CreateScannerService(deviceInfo, js);
        _ = _scannerService.CreateScanDataObject();//ScanDataオブジェクトの作成
    }
    /// <summary>
    /// サービス作成処理 //Todoコンストラクタで行う処理を別メソッドとして作成。コンストラクタでやりたくない場合はコンストラクタの処理を削除してこちらを呼び出すようにすれば良い。
    /// </summary>
    /// <param name="deviceInfo"></param>
    /// <param name="js"></param>
    public void CreateHtService(DeviceInfoService deviceInfo, IJSRuntime js)
    {
        _scannerService = ScannerFactory.CreateScannerService(deviceInfo, js);
        _ = _scannerService.CreateScanDataObject();//ScanDataオブジェクトの作成
    }
    #region 元処理
    ///// <summary>
    ///// 読み取り結果を受け取るためのコールバック関数登録
    ///// </summary>
    ///// <param name="callbackFuncName">コールバック関数名</param>
    ///// <returns></returns>
    //public async void SetReadCallback(string callbackFuncName = "scanCallbackFunction")
    //{
    //    try
    //    {
    //        bool result = await JS.InvokeAsync<bool>("KJS.Scanner.setReadCallback", callbackFuncName);
    //    }
    //    catch (Exception)
    //    {
    //        //TODO エラーログ出力
    //    }
    //}

    ///// <summary>
    ///// ブザー開始
    ///// </summary>
    ///// <param name="tone">ブザーの音調(1-16)</param>
    ///// <param name="onPeriod">ブザーのON の時間(1-5000(ms))</param>
    ///// <param name="offPeriod">ブザーのOFF の時間(1-5000(ms))</param>
    ///// <param name="repeatCount">ON/OFF の繰り返し回数(1-10)</param>
    ///// <returns></returns>
    //public async Task<bool> StartBuzzer(short tone, int onPeriod, int offPeriod, short repeatCount)
    //{
    //    bool result = false;
    //    try
    //    {
    //        result = await JS.InvokeAsync<bool>("KJS.Notification.startBuzzer", tone, onPeriod, offPeriod, repeatCount);
    //    }
    //    catch (Exception)
    //    {
    //        //TODO エラーログ出力
    //    }
    //    return result;
    //}

    ///// <summary>
    ///// ブザー停止
    ///// </summary>
    ///// <returns></returns>
    //public async void StopBuzzer()
    //{
    //    try
    //    {
    //        await JS.InvokeVoidAsync("KJS.Notification.stopBuzzer");
    //    }
    //    catch
    //    {
    //        //TODO エラーログ出力
    //    }
    //}

    ///// <summary>
    ///// バイブレーション開始
    ///// </summary>
    ///// <param name="onPeriod">バイブレータのON の時間(1-5000(ms))</param>
    ///// <param name="offPeriod">バイブレータのOFF の時間(1-5000(ms))</param>
    ///// <param name="repeatCount">ON/OFF の繰り返し回数(1-10)</param>
    ///// <returns></returns>
    //public async Task<bool> StartVibrator(int onPeriod, int offPeriod, short repeatCount)
    //{
    //    bool result = false;
    //    try
    //    {
    //        result = await JS.InvokeAsync<bool>("KJS.Notification.startVibrator", onPeriod, offPeriod, repeatCount);
    //    }
    //    catch (Exception)
    //    {
    //        //TODO エラーログ出力
    //    }
    //    return result;
    //}

    ///// <summary>
    ///// バイブレーション停止
    ///// </summary>
    //public async void StopVibrator()
    //{
    //    try
    //    {
    //        await JS.InvokeVoidAsync("KJS.Notification.stopVibrator");
    //    }
    //    catch
    //    {
    //        //TODO エラーログ出力
    //    }
    //}

    ///// <summary>
    ///// 読み取り開始
    ///// </summary>
    ///// <returns></returns>
    //public async Task<int> StartRead()
    //{
    //    int result = -1;
    //    try
    //    {
    //        result = await JS.InvokeAsync<int>("KJS.Scanner.startRead");
    //    }
    //    catch (Exception)
    //    {
    //        //TODO エラーログ出力
    //    }
    //    return result;
    //}

    ///// <summary>
    ///// 読み取りロック解除
    ///// </summary>
    ///// <returns></returns>
    //public async Task<bool> UnLockScanner()
    //{
    //    bool result = false;
    //    try
    //    {
    //        result = await JS.InvokeAsync<bool>("KJS.Scanner.unlockScanner");
    //    }
    //    catch (Exception)
    //    {
    //        //TODO エラーログ出力
    //    }
    //    return result;
    //}

    ///// <summary>
    ///// 読み取り停止
    ///// </summary>
    //public async void StopRead()
    //{
    //    try
    //    {
    //        await JS.InvokeVoidAsync("KJS.Scanner.stopRead");
    //    }
    //    catch
    //    {
    //        //TODO エラーログ出力
    //    }
    //}
    #endregion

    /// <summary>
    ///  読み取り結果を受け取るためのコールバック関数登録の呼び出し
    /// </summary>
    /// <param name="callbackFuncName"></param>
    public void SetReadCallback(string callbackFuncName = "scanCallbackFunction")
    {
        _scannerService.SetReadCallback(callbackFuncName);
    }
    /// <summary>
    /// ブザー開始呼び出し
    /// </summary>
    /// <param name="tone"></param>
    /// <param name="onPeriod"></param>
    /// <param name="offPeriod"></param>
    /// <param name="repeatCount"></param>
    /// <returns></returns>
    public async Task<bool> StartBuzzer(short tone, int onPeriod, int offPeriod, short repeatCount)
    {
        return await _scannerService.StartBuzzer(tone, onPeriod, offPeriod, repeatCount);
    }

    /// <summary>
    /// ブザー停止呼び出し
    /// </summary>
    public void StopBuzzer()
    {
        _scannerService.StopBuzzer();
    }

    /// <summary>
    /// 読み取り開始呼び出し
    /// </summary>
    /// <returns></returns>
    public async Task<int> StartRead()
    {
        return await _scannerService.StartRead();
    }

    /// <summary>
    /// 読み取りロック解除
    /// </summary>
    /// <returns></returns>
    public async Task<bool> UnLockScanner()
    {
        return await _scannerService.UnLockScanner();
    }

    /// <summary>
    /// 読み取り停止
    /// </summary>
    public void StopRead()
    {
        _scannerService.StopRead();
    }

    /// <summary>
    /// バイブレーション開始
    /// </summary>
    /// <param name="onPeriod">バイブレータのON の時間(1-5000(ms))</param>
    /// <param name="offPeriod">バイブレータのOFF の時間(1-5000(ms))</param>
    /// <param name="repeatCount">ON/OFF の繰り返し回数(1-10)</param>
    /// <returns></returns>
    public async Task<bool> StartVibrator(int onPeriod, int offPeriod, short repeatCount)
    {
        return await _scannerService.StartVibrator(onPeriod, offPeriod, repeatCount);
    }

    /// <summary>
    /// バイブレーション停止
    /// </summary>
    public void StopVibrator()
    {
        _scannerService.StopVibrator();
    }

    [JSInvokable]
    public static void CallScanFunction(string result, string code, string scantext)
    {
        _scannerService.setScanData(new ScanData() { strDecodeResult = result, strCodeType = code, strStringData = scantext });

  

        DebugText = "CallScanFunction";
        // イベントが登録されている場合はイベントを発生させる
        _ = (HtScanEvent?.Invoke(_scannerService.getScanData())); // イベントの発生
    }

}

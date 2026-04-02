using Microsoft.JSInterop;

namespace ExpressionDBBlazorShared.Services;

/// <summary>
/// HT(Keyence)用スキャン機能実装クラス
/// </summary>
internal class KeyenceScannerService : IScannerService
{
    public readonly IJSRuntime JS;
    public static ScanData? ScanData { get; set; }
    public Task<ScanData> CreateScanDataObject()
    {
        ScanData = new ScanData();
        return Task.FromResult(ScanData);
    }
    public void setScanData(ScanData scanData)
    {
        ScanData = scanData;
    }
    public ScanData? getScanData()
    {
        return ScanData;
    }
    public KeyenceScannerService(IJSRuntime js)
    {
        JS = js;
    }

    /// <summary>
    /// 読み取り結果を受け取るためのコールバック関数登録
    /// 西山
    /// </summary>
    /// <param name="callbackFuncName">コールバック関数名</param>
    /// <returns></returns>
    public async void SetReadCallback(string callbackFuncName = "scanCallbackFunction")
    {
        try
        {
            bool result = await JS.InvokeAsync<bool>("KJS.Scanner.setReadCallback", callbackFuncName);
        }
        catch (Exception)
        {
            //TODO エラーログ出力
        }
    }

    /// <summary>
    /// ブザー開始
    /// </summary>
    /// <param name="tone">ブザーの音調(1-16)</param>
    /// <param name="onPeriod">ブザーのON の時間(1-5000(ms))</param>
    /// <param name="offPeriod">ブザーのOFF の時間(1-5000(ms))</param>
    /// <param name="repeatCount">ON/OFF の繰り返し回数(1-10)</param>
    /// <returns></returns>
    public async Task<bool> StartBuzzer(short tone, int onPeriod, int offPeriod, short repeatCount)
    {
        bool result = false;
        try
        {
            result = await JS.InvokeAsync<bool>("KJS.Notification.startBuzzer", tone, onPeriod, offPeriod, repeatCount);
        }
        catch (Exception)
        {
            //TODO エラーログ出力
        }
        return result;
    }

    /// <summary>
    /// ブザー停止
    /// </summary>
    /// <returns></returns>
    public async void StopBuzzer()
    {
        try
        {
            await JS.InvokeVoidAsync("KJS.Notification.stopBuzzer");
        }
        catch
        {
            //TODO エラーログ出力
        }
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
        bool result = false;
        try
        {
            result = await JS.InvokeAsync<bool>("KJS.Notification.startVibrator", onPeriod, offPeriod, repeatCount);
        }
        catch (Exception)
        {
            //TODO エラーログ出力
        }
        return result;
    }

    /// <summary>
    /// バイブレーション停止
    /// </summary>
    public async void StopVibrator()
    {
        try
        {
            await JS.InvokeVoidAsync("KJS.Notification.stopVibrator");
        }
        catch
        {
            //TODO エラーログ出力
        }
    }

    /// <summary>
    /// 読み取り開始
    /// 西山　読取2
    /// </summary>
    /// <returns></returns>
    public async Task<int> StartRead()
    {
        int result = -1;
        try
        {
            result = await JS.InvokeAsync<int>("KJS.Scanner.startRead");
        }
        catch (Exception)
        {
            //TODO エラーログ出力
        }
        return result;
    }

    /// <summary>
    /// 読み取りロック解除
    /// </summary>
    /// <returns></returns>
    public async Task<bool> UnLockScanner()
    {
        bool result = false;
        try
        {
            result = await JS.InvokeAsync<bool>("KJS.Scanner.unlockScanner");
        }
        catch (Exception)
        {
            //TODO エラーログ出力
        }
        return result;
    }

    /// <summary>
    /// 読み取り停止
    /// </summary>
    public async void StopRead()
    {
        try
        {
            await JS.InvokeVoidAsync("KJS.Scanner.stopRead");
        }
        catch
        {
            //TODO エラーログ出力
        }
    }
}

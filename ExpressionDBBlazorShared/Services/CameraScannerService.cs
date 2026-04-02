using Microsoft.JSInterop;

namespace ExpressionDBBlazorShared.Services;

/// <summary>
/// HT以外用(カメラ)スキャン機能実装クラス
/// </summary>
internal class CameraScannerService : IScannerService
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
    public CameraScannerService(IJSRuntime js)
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
            //bool result = await JS.InvokeAsync<bool>("Quagga.onDetected", "scanCallbackFunction");//QuaggaJSではkjsのようにコールバック関数の登録はない   
            await Task.Delay(0);
        }
        catch (Exception ex)
        {
            //TODO エラーログ出力
            _ = ex.Message;
        }
    }

    public Task<bool> StartBuzzer(short tone, int onPeriod, int offPeriod, short repeatCount)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// ブザー停止
    /// </summary>
    /// <returns></returns>
    public async void StopBuzzer()
    {
        try
        {
            //await JS.InvokeVoidAsync("KJS.Notification.stopBuzzer");//TODO　Quaggaにはないのでとりあえずコメントアウト
            await Task.Delay(0);
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
            //result = await JS.InvokeAsync<bool>("KJS.Notification.startVibrator", onPeriod, offPeriod, repeatCount);//TODO　Quaggaにはないのでとりあえずコメントアウト
            await Task.Delay(0);
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
            //await JS.InvokeVoidAsync("KJS.Notification.stopVibrator");//TODO　Quaggaにはないのでとりあえずコメントアウト
            await Task.Delay(0);
        }
        catch
        {
            //TODO エラーログ出力
        }
    }

    /// <summary>
    /// 読み取り開始
    /// </summary>
    /// <returns></returns>
    public async Task<int> StartRead()
    {
        int result = -1;
        try
        {
            result = await JS.InvokeAsync<int>("QuaggaStartRead", "scanCallbackFunction");
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
        bool result;
        try
        {
            //result = await JS.InvokeAsync<bool>("KJS.Scanner.unlockScanner");//TODO　存在しない場合どうする？一旦強制的にtrueを返すように
            await Task.Delay(0);
        }
        catch (Exception)
        {
            //TODO エラーログ出力
        }
        result = true;
        return result;
    }

    /// <summary>
    /// 読み取り停止
    /// </summary>
    public async void StopRead()
    {
        try
        {
            await JS.InvokeVoidAsync("QuagaStopRead");
        }
        catch
        {
            //TODO エラーログ出力
        }
    }

    Task<bool> IScannerService.StartVibrator(int onPeriod, int offPeriod, short repeatCount)
    {
        throw new NotImplementedException();
    }

}

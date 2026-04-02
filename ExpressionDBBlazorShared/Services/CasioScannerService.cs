using Microsoft.JSInterop;

namespace ExpressionDBBlazorShared.Services;

/// <summary>
/// HT(Casio)用スキャン機能実装クラス
/// </summary>
internal class CasioScannerService : IScannerService
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
    public CasioScannerService(IJSRuntime js)
    {
        JS = js;
    }

    /// <summary>
    /// 読み取り結果を受け取るためのコールバック関数登録
    /// </summary>
    /// <param name="callbackFuncName">コールバック関数名</param>
    /// <returns></returns>
    public async void SetReadCallback(string callbackFuncName = "scanCallbackFunction")
    {
        try
        {
            await Task.Delay(0);
            //TODO 実装
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
            await Task.Delay(0);
            //TODO 実装
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
            await Task.Delay(0);
            //TODO 実装
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
            await Task.Delay(0);
            //TODO 実装
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
            await Task.Delay(0);
            //TODO 実装
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
            await Task.Delay(0);
            //TODO 実装
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
            await Task.Delay(0);
            //TODO 実装
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
            await Task.Delay(0);
            //TODO 実装
        }
        catch
        {
            //TODO エラーログ出力
        }
    }
}

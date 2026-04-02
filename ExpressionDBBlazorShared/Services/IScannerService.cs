namespace ExpressionDBBlazorShared.Services;

/// <summary>
/// バーコードスキャン用インターフェース
/// </summary>
internal interface IScannerService
{
    public static ScanData? ScanData { get; set; }
    public delegate Task ScanEventArg(ScanData scanData);
    public static ScanEventArg? ScanEvent;

    /// <summary>
    /// ScanDataオブジェクト作成クエリ
    /// </summary>
    /// <returns></returns>
    public Task<ScanData> CreateScanDataObject();
    public void setScanData(ScanData scanData);
    public ScanData? getScanData();
    /// <summary>
    /// 読み取り結果を受け取るためのコールバック関数登録
    /// </summary>
    /// <param name="callbackFuncName">コールバック関数名</param>
    public void SetReadCallback(string callbackFuncName);
    /// <summary>
    /// ブザー開始
    /// </summary>
    /// <param name="tone">ブザーの音調(1-16)</param>
    /// <param name="onPeriod">ブザーのON の時間(1-5000(ms))</param>
    /// <param name="offPeriod">ブザーのOFF の時間(1-5000(ms))</param>
    /// <param name="repeatCount">ON/OFF の繰り返し回数(1-10)</param>
    /// <returns></returns>
    public Task<bool> StartBuzzer(short tone, int onPeriod, int offPeriod, short repeatCount);
    /// <summary>
    /// ブザー停止
    /// </summary>
    /// <returns></returns>
    public void StopBuzzer();
    /// <summary>
    /// バイブレーション開始
    /// </summary>
    /// <param name="onPeriod">バイブレータのON の時間(1-5000(ms))</param>
    /// <param name="offPeriod">バイブレータのOFF の時間(1-5000(ms))</param>
    /// <param name="repeatCount">ON/OFF の繰り返し回数(1-10)</param>
    /// <returns></returns>
    public Task<bool> StartVibrator(int onPeriod, int offPeriod, short repeatCount);
    /// <summary>
    /// バイブレーション停止
    /// </summary>
    public void StopVibrator();
    /// <summary>
    /// 読み取り開始
    /// </summary>
    /// <returns></returns>
    public Task<int> StartRead();

    /// <summary>
    /// 読み取りロック解除
    /// </summary>
    /// <returns></returns>
    public Task<bool> UnLockScanner();

    /// <summary>
    /// 読み取り停止
    /// </summary>
    public void StopRead();
}

using Microsoft.JSInterop;

namespace ZennohBlazorShared.Services
{
    /// <summary>
    /// スキャン情報
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
        public readonly IJSRuntime JS;
        public static ScanData ScanData { get; set; } = new ScanData();
        public static string DebugText { get; set; } = "";

        public delegate Task HtScanEventArg(ScanData scanData);
        public static event HtScanEventArg? HtScanEvent;

        public HtService(IJSRuntime js)
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

        [JSInvokable]
        public static void CallScanFunction(string result, string code, string scantext)
        {
            ScanData = new ScanData() { strDecodeResult = result, strCodeType = code, strStringData = scantext };

            DebugText = "CallScanFunction";
            // イベントが登録されている場合はイベントを発生させる
            _ = (HtScanEvent?.Invoke(ScanData)); // イベントの発生
        }
    }
}

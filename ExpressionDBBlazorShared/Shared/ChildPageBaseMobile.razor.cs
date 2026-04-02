using Blazor.DynamicJS;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Util;
using Microsoft.AspNetCore.Components;
using SharedModels;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 子画面Bodyページに適用する親クラス。モバイル用。
/// モバイル側ページで共通にしたい処理を記述する
/// </summary>
public partial class ChildPageBaseMobile : ChildPageBase
{

    // StepExtendコンポーネント（画面のrazorで@refで受け取る）
    protected StepsExtend? stepsExtend { get; set; } = null;

    // StepExtend用Attributes変数（画面のrazorで@attributesでで受け取る）
    protected IDictionary<string, object> StepsExtendAttributes { get; set; } = new Dictionary<string, object>();

    // Bodyコンテンツの初期スクロールサイズ
    protected int iBodyScrollSize = 60;

    protected bool IsBarProc { get; private set; } = false; //子ページ継承先でバーコードスキャン処理中か判断せるために用意

    /// <summary>
    /// コンポーネントが初期化されるときに呼び出されます。
    /// 子ページで全体で使用したい処理を記載
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        //イベント削除
        HtService.HtScanEvent -= HtService_HtScanEventMain;

        //イベント追加
        HtService.HtScanEvent += HtService_HtScanEventMain;
        htService!.SetReadCallback();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

        // StepExtendコンポーネントが定義される画面は、初期表示時にその分スクロールする
        if (stepsExtend != null)
        {
            ScrollPageFirst();
        }
    }

    protected override void Dispose()
    {
        base.Dispose();

        //イベント削除
        HtService.HtScanEvent -= HtService_HtScanEventMain;
    }

    /// <summary>
    /// スキャンされた時の処理
    /// 西山
    /// </summary>
    /// <param name="scanData"></param>
    protected async Task HtService_HtScanEventMain(ScanData scanData)
    {
        try
        {
            // ファンクション処理中は無視する
            if (ChildBaseService.IsFuncProc)
            {
                return;
            }
            // バーコード処理中は無視する
            if (IsBarProc)
            {
                return;
            }

            IsBarProc = true;

            await HtService_HtScanEvent(scanData);
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
        finally
        {
            IsBarProc = false;
        }
    }

    #region virtual

    /// <summary>
    /// スキャンされた時の処理
    /// </summary>
    /// <param name="scanData"></param>
    protected virtual async Task HtService_HtScanEvent(ScanData scanData)
    {
        await Task.Delay(0);
        //継承画面でオーバーライドする
    }

    /// <summary>
    /// ブザー再生
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task ブザー再生(ComponentProgramInfo info)
    {
        await Task.Delay(0);
        int tone = 1;
        int onPeriod = 1;
        int offPeriod = 1;
        int repeatCount = 1;
        //SystemParameterService sysParams = await SessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
        if (_sysParams is not null)
        {
            tone = _sysParams.HT_DefaultBuzzerTone;
            onPeriod = _sysParams.HT_DefaultBuzzerOnPeriod;
            offPeriod = _sysParams.HT_DefaultBuzzerOffPeriod;
            repeatCount = _sysParams.HT_DefaultBuzzerReratCount;
        }

        Dictionary<string, object> attr = new(GetAttributes(info.ComponentName));
        if (attr.TryGetValue("Tone", out object? value))
        {
            tone = ConvertUtil.ConvertInt(value.ToString()!);
        }
        if (attr.TryGetValue("OnPeriod", out value))
        {
            onPeriod = ConvertUtil.ConvertInt(value.ToString()!);
        }
        if (attr.TryGetValue("OffPeriod", out value))
        {
            offPeriod = ConvertUtil.ConvertInt(value.ToString()!);
        }
        if (attr.TryGetValue("RepeatCount", out value))
        {
            repeatCount = ConvertUtil.ConvertInt(value.ToString()!);
        }

        _ = htService!.StartBuzzer((short)tone, onPeriod, offPeriod, (short)repeatCount);
    }
    /// <summary>
    /// ブザー再生(エラーアラーム用)
    /// 繰り返し回数が0の場合は再生しない
    /// </summary>
    /// <returns></returns>
    public override async Task ブザー再生_エラー()
    {
        // 
        int errorBuzzerOffPeriod = _sysParams!.HT_ErrorBuzzerOffPeriod;
        int errorBuzzerOnPeriod = _sysParams!.HT_ErrorBuzzerOnPeriod;
        int errorBuzzerReratCount = _sysParams!.HT_ErrorBuzzerReratCount;
        int errorBuzzerTone = _sysParams!.HT_ErrorBuzzerTone;
        if (errorBuzzerReratCount > 0)
        {
            _ = htService!.StartBuzzer((short)errorBuzzerTone, errorBuzzerOnPeriod, errorBuzzerOffPeriod, (short)errorBuzzerReratCount);
        }

        await Task.Delay(0);
    }
    /// <summary>
    /// バイブレーション開始
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task バイブレーション開始(ComponentProgramInfo info)
    {
        await Task.Delay(0);
        int onPeriod = 1;
        int offPeriod = 1;
        int repeatCount = 1;
        //SystemParameterService sysParams = await SessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
        if (_sysParams is not null)
        {
            onPeriod = _sysParams.HT_DefaultVibrationOnPeriod;
            offPeriod = _sysParams.HT_DefaultVibrationOffPeriod;
            repeatCount = _sysParams.HT_DefaultVibrationReratCount;
        }

        Dictionary<string, object> attr = new(GetAttributes(info.ComponentName));
        if (attr.TryGetValue("OnPeriod", out object? value))
        {
            onPeriod = ConvertUtil.ConvertInt(value.ToString()!);
        }
        if (attr.TryGetValue("OffPeriod", out value))
        {
            offPeriod = ConvertUtil.ConvertInt(value.ToString()!);
        }
        if (attr.TryGetValue("RepeatCount", out value))
        {
            repeatCount = ConvertUtil.ConvertInt(value.ToString()!);
        }

        _ = htService!.StartVibrator(onPeriod, offPeriod, (short)repeatCount);
    }
    /// <summary>
    /// バイブレーション開始(エラーアラーム用)
    /// 繰り返し回数が0の場合は再生しない
    /// </summary>
    /// <returns></returns>
    public override async Task バイブレーション開始_エラー()
    {
        // 
        int errorVibrationOffPeriod = _sysParams!.HT_ErrorVibrationOffPeriod;
        int errorVibrationOnPeriod = _sysParams!.HT_ErrorVibrationOnPeriod;
        int errorVibrationReratCount = _sysParams!.HT_ErrorVibrationReratCount;
        if (errorVibrationReratCount > 0)
        {
            _ = htService!.StartVibrator(errorVibrationOnPeriod, errorVibrationOffPeriod, (short)errorVibrationReratCount);
        }
        await Task.Delay(0);
    }
    /// <summary>
    /// Htの通知が押下された時の処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    protected override async Task OnClickHtNotify(object? sender, object? e)
    {
        try
        {
            // パラメータ取得
            IList<ComponentsInfo> lstComponents = await ComService!.GetComponetnsInfo("DialogHtNotifyInfo");
            ComponentsInfo? infoTitle = lstComponents.FirstOrDefault(_ => _.ComponentName == "AttributesDialog" && _.AttributesName == "DialogTitle");
            ComponentsInfo? infoHeight = lstComponents.FirstOrDefault(_ => _.ComponentName == "AttributesDialog" && _.AttributesName == "DialogHeight");
            ComponentsInfo? infoPageSize = lstComponents.FirstOrDefault(_ => _.ComponentName == "AttributesDialog" && _.AttributesName == "PageSize");
            ComponentsInfo? infoReadSize = lstComponents.FirstOrDefault(_ => _.ComponentName == "AttributesDialog" && _.AttributesName == "ReadSize");

            // ダイアログパラメータ生成
            Dictionary<string, object> dlgParam = [];
            if (infoPageSize != null)
            {
                dlgParam.Add("PageSize", infoPageSize.Value);
            }
            if (infoReadSize != null)
            {
                dlgParam.Add("ReadSize", infoReadSize.Value);
            }

            string strDialogTitle = "通知一覧";
            string strDialogHeight = "400px";
            if (infoTitle != null)
            {
                strDialogTitle = infoTitle.Value;
            }
            if (infoHeight != null)
            {
                strDialogHeight = infoHeight.Value;
            }

            // ダイアログ出力
            _ = await DialogService.OpenSideAsync<DialogHtNotifyInfo>(
                strDialogTitle,
                dlgParam,
                options: new SideDialogOptions { CloseDialogOnOverlayClick = false, Position = DialogPosition.Bottom, ShowMask = true, Height = $"{strDialogHeight}" }
            );
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }
    /// <summary>
    /// Htのホームボタンが押下された時の処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    protected override async Task OnClickHtHomeNavigate(object? sender, object? e)
    {
        await Task.Delay(0);
        try
        {
            // ホームボタンへ遷移を行う処理。
            // YesNoダイアログを表示して確認する
            bool retb = await ComService.DialogShowYesNo("HTﾒﾆｭｰへ戻ります。続行しますか?");
            if (retb)
            {
                // ロック情報を削除するストアドを呼び出す。//非同期とする
                _ = ComService.ExecLogoutFunc(GetType().Name, true);

                //HT用メニュー画面へ遷移する
                NavigationManager.NavigateTo("mobile_menu");
            }

        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    #endregion

    /// <summary>
    /// パレットバーコード判定
    /// 西山
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    protected bool IsPalletBarcode(string code)
    {
        bool ret = code.Length is
            SharedConst.LEN_PALLET_NO_BARCODE or
            SharedConst.LEN_PALLET_NO_BARCODE2 or
            SharedConst.LEN_PALLET_NO_BARCODE3 or
            SharedConst.LEN_PALLET_NO_BARCODE4;//テスト的に追加


        return ret;
    }

    /// <summary>
    /// ロケーションバーコード判定
    /// 西山
    /// 桁数判定をしている
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    protected bool IsLocationBarcode(string code)
    {
        bool ret = code.Length is
            not SharedConst.LEN_ZONE_ID and
            not SharedConst.LEN_PALLET_NO_BARCODE and
            not SharedConst.LEN_PALLET_NO_BARCODE2 and
            not SharedConst.LEN_PALLET_NO_BARCODE3 and 
            not SharedConst.LEN_PALLET_NO_BARCODE4; //テスト的に追加
        return ret;
    }
    /// <summary>
    /// ページ初期表示時のスクロール処理（データ読み込み時の初期表示位置へのスクロールの際も使用）
    /// </summary>
    protected async void ScrollPageFirst()
    {
        using DynamicJSRuntime js = _js ?? await JS!.CreateDymaicRuntimeAsync();
        dynamic window = js.GetWindow();
        dynamic element = window.document.getElementById(SharedConst.STR_BODY_ID);
        element.scrollTop = iBodyScrollSize;
    }
}

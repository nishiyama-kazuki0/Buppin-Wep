using Blazor.DynamicJS;
using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen.Blazor;
using SharedModels;
using static SharedModels.SharedConst;

namespace ExpressionDBBlazorShared.Shared;//西山　ボタンパラメータ
public partial class ButtonFuncRadzen : IAsyncDisposable
{
    [Inject]
    private CommonService ComService { get; set; } = null!;
    private readonly int N_FUNCPROC_QUEUEDEL_TIMER = 100;
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    /// <value>The unique identifier.</value>
    public required string UniqueID { get; set; }

    //引数
    [Parameter]
    public bool IsHandy { get; set; } = false;

    /// <summary>
    /// TODO OpenDialogを使用するにあたり注意事項
    /// DialogMsgYesNoContent、DialogMsgOKContentはfalseにしています
    /// ButtonClickF1メソッドの
    /// await OnClickResultF1.InvokeAsync(string.Empty);
    /// 前でBusyDialogを表示して、finallyでCloseしていることで、DialogMsgYesNoContent等OpenDialogをawaitして戻り値を取得している箇所でBusyDialogのClose値（現在はnull)
    /// が取得できてしまうため、DialogMsgYesNoContentはファンクションボタン押したときにBusyDialogを表示させない
    /// </summary>
    [Parameter]
    public bool IsBusyDialog { get; set; } = true;
    /// <summary>
    /// 戻り値を戻すダイアログは以下の手順で処理を実装すること
    /// １．ビジーダイアログをClose
    /// ２．SetIsBusyDialogCloseにてIsBusyDialogCloseフラグをfalseに設定
    /// ３．自身のダイアログのCloseの引数を設定することで値を戻す
    /// 
    /// ※DialogMsgYesNoContent、DialogMsgOKContentの様に処理中ダイアログを表示させる必要がない場合は、
    /// 　IsBusyDialogをfalseに設定しておくことで本フラグは使用しなくてよい
    /// </summary>
    private bool IsBusyDialogClose = true;

    [Parameter]
    public string button1text { get; set; } = string.Empty;
    [Parameter]
    public string button2text { get; set; } = string.Empty;
    [Parameter]
    public string button3text { get; set; } = string.Empty;
    [Parameter]
    public string button4text { get; set; } = string.Empty;
    [Parameter]
    public string button5text { get; set; } = string.Empty;
    [Parameter]
    public string button6text { get; set; } = string.Empty;
    [Parameter]
    public string button7text { get; set; } = string.Empty;
    [Parameter]
    public string button8text { get; set; } = string.Empty;
    [Parameter]
    public string button9text { get; set; } = string.Empty;
    [Parameter]
    public string button10text { get; set; } = string.Empty;
    [Parameter]
    public string button11text { get; set; } = string.Empty;
    [Parameter]
    public string button12text { get; set; } = string.Empty;
    [Parameter]
    public string buttonNotifytext { get; set; } = string.Empty;
    [Parameter]
    public string buttonHomeNavigatetext { get; set; } = string.Empty;
    [Parameter]
    public string buttonPageUptext { get; set; } = string.Empty;
    [Parameter]
    public string buttonPageDowntext { get; set; } = string.Empty;

    [Parameter]
    public string IconName1 { get; set; } = string.Empty;
    [Parameter]
    public string IconName2 { get; set; } = string.Empty;
    [Parameter]
    public string IconName3 { get; set; } = string.Empty;
    [Parameter]
    public string IconName4 { get; set; } = string.Empty;
    [Parameter]
    public string IconName5 { get; set; } = string.Empty;
    [Parameter]
    public string IconName6 { get; set; } = string.Empty;
    [Parameter]
    public string IconName7 { get; set; } = string.Empty;
    [Parameter]
    public string IconName8 { get; set; } = string.Empty;
    [Parameter]
    public string IconName9 { get; set; } = string.Empty;
    [Parameter]
    public string IconName10 { get; set; } = string.Empty;
    [Parameter]
    public string IconName11 { get; set; } = string.Empty;
    [Parameter]
    public string IconName12 { get; set; } = string.Empty;
    [Parameter]
    public string IconNameNotify { get; set; } = string.Empty;
    [Parameter]
    public string IconNameHomeNavigate { get; set; } = string.Empty;
    [Parameter]
    public string IconNamePageUp { get; set; } = string.Empty;
    [Parameter]
    public string IconNamePageDown { get; set; } = string.Empty;

    //PC,HT画面のボタンのmargin-bottomの高さ
    [Parameter]
    public string PCButtonMarginBottom { get; set; } = "1px";
    [Parameter]
    public string HTButtonMarginBottom { get; set; } = "1px";

    //HT画面のファンクションボタンの背景色の色
    [Parameter]
    public string HT_Button1BackgroundColor { get; set; } = "#000000";
    [Parameter]
    public string HT_Button2BackgroundColor { get; set; } = "#D3D3D3";
    [Parameter]
    public string HT_Button3BackgroundColor { get; set; } = "#FFA500";
    [Parameter]
    public string HT_Button4BackgroundColor { get; set; } = "#000000";

    //HT画面のファンクションボタンの文字の色
    [Parameter]
    public string HT_Button1TextColor { get; set; } = "#ffffff";
    [Parameter]
    public string HT_Button4TextColor { get; set; } = "#ffffff";

    //EventCallback
    [Parameter]
    public EventCallback<string> OnClickResultF1 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF2 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF3 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF4 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF5 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF6 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF7 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF8 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF9 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF10 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF11 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickResultF12 { get; set; }
    [Parameter]
    public EventCallback<string> OnClickHtNotify { get; set; }
    [Parameter]
    public EventCallback<string> OnClickHtHomeNavigate { get; set; }
    [Parameter]
    public EventCallback<string> OnClickPageUp { get; set; }
    [Parameter]
    public EventCallback<string> OnClickPageDown { get; set; }
    [Parameter]
    public bool pageupvisible { get; set; } = false;
    [Parameter]
    public bool pagedownvisible { get; set; } = false;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? Attributes { get; set; }

    //Blazor.DynamicJS
    private DynamicJSRuntime? _js;
    public async ValueTask DisposeAsync()
    {
        await JS.InvokeVoidAsync("removeScrollListener", UniqueID, SharedConst.STR_BODY_ID);//なぜかファンクションのイベント解除よりも先に実行しないとログアウト時にエラーになるので注意。
        //return _js?.DisposeAsync() ?? ValueTask.CompletedTask;
        if (IsHandy)
        {
            await JS.InvokeVoidAsync("removeFuncKeyHTListener", UniqueID);
        }
        else
        {
            await JS.InvokeVoidAsync("removeFuncKeyListener", UniqueID);
        }
        _ = _js?.DisposeAsync() ?? ValueTask.CompletedTask;
    }

    private bool isBusyFunc { get; set; } = false;
    private bool isBusyF1 { get; set; } = false;
    private bool isBusyF2 { get; set; } = false;
    private bool isBusyF3 { get; set; } = false;
    private bool isBusyF4 { get; set; } = false;
    private bool isBusyF5 { get; set; } = false;
    private bool isBusyF6 { get; set; } = false;
    private bool isBusyF7 { get; set; } = false;
    private bool isBusyF8 { get; set; } = false;
    private bool isBusyF9 { get; set; } = false;
    private bool isBusyF10 { get; set; } = false;
    private bool isBusyF11 { get; set; } = false;
    private bool isBusyF12 { get; set; } = false;
    private bool isBusyNotify { get; set; } = false;
    private bool isBusyHomeNavigate { get; set; } = false;
    private bool isBusyPageUp { get; set; } = false;
    private bool isBusyPageDown { get; set; } = false;

    private RadzenButton? RadzenButtonF1;
    private RadzenButton? RadzenButtonF2;
    private RadzenButton? RadzenButtonF3;
    private RadzenButton? RadzenButtonF4;
    private RadzenButton? RadzenButtonF5;
    private RadzenButton? RadzenButtonF6;
    private RadzenButton? RadzenButtonF7;
    private RadzenButton? RadzenButtonF8;
    private RadzenButton? RadzenButtonF9;
    private RadzenButton? RadzenButtonF10;
    private RadzenButton? RadzenButtonF11;
    private RadzenButton? RadzenButtonF12;
    private RadzenButton? RadzenButtonNotify;
    private RadzenButton? RadzenButtonHomeNavigate;
    private RadzenButton? RadzenButtonPageUp;
    private RadzenButton? RadzenButtonPageDown;

    [Inject]
    private ISessionStorageService? _sessionStorage { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        UniqueID = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("/", "-").Replace("+", "-")[..10];
        pageupvisible = false;
        pagedownvisible = false;
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(0);
        //if (null == _sessionStorage)
        //{
        //    return;
        //}
        SystemParameterService sysParams = SystemParamService;
        if (null != sysParams)
        {
            PCButtonMarginBottom = sysParams.PC_ButtonMarginBottom;
            HTButtonMarginBottom = sysParams.HT_ButtonMarginBottom;

            HT_Button1BackgroundColor = sysParams.HT_Button1BackgroundColor;
            HT_Button2BackgroundColor = sysParams.HT_Button2BackgroundColor;
            HT_Button3BackgroundColor = sysParams.HT_Button3BackgroundColor;
            HT_Button4BackgroundColor = sysParams.HT_Button4BackgroundColor;

            HT_Button1TextColor = sysParams.HT_Button1TextColor;
            HT_Button4TextColor = sysParams.HT_Button4TextColor;

        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        ////検索処理などで再度レンダリングが必要な場合に備えて、スクロールボタン表示非表示を確認する
        //_ = InvokeAsync(async () =>
        //{
        //    Task<bool> bret = CheckVisibleButtonPageUpDownAsync();
        //    if (await bret)
        //    {
        //        // コンポーネントを再レンダリングする
        //        StateHasChanged();
        //    }
        //    return;
        //});

        if (!firstRender)
        {
            return;
        }

        _js = await JS.CreateDymaicRuntimeAsync();
        //_ = _js.GetWindow();

        DotNetObjectReference<ButtonFuncRadzen> dotNetReference = DotNetObjectReference.Create(this);
        if (IsHandy)
        {
            await JS.InvokeVoidAsync("initializeFuncKeyHTListener", dotNetReference, UniqueID);
        }
        else
        {
            await JS.InvokeVoidAsync("initializeFuncKeyListener", dotNetReference, UniqueID);
        }

        await JS!.InvokeVoidAsync("initializeScrollListener", DotNetObjectReference.Create(this), UniqueID, SharedConst.STR_BODY_ID);


        ////JSスクロールイベントリスナーを定義する
        //w.document.getElementById(SharedConst.STR_BODY_ID).addEventListener("scroll", (Action<dynamic>)(async e =>
        //{
        //    Task<bool> bret = CheckVisibleButtonPageUpDownAsync();
        //    if (await bret)
        //    {
        //        // コンポーネントを再レンダリングする
        //        StateHasChanged();
        //    }
        //}));

        //await JS.InvokeVoidAsync("removeKeyListener");
        ////Jsキーリスナーの定義
        //window.addEventListener("keydown", (Action<dynamic>)(async e =>
        //{
        //    if (AssemblyState.Debug)
        //    {
        //        NotificationService.Notify(new NotificationMessage()
        //        {
        //            Style = "position: absolute; bottom: -1000px;",
        //            Severity = ToastType.Info,
        //            Summary = "test",
        //            Detail = $"onkey : {(int)e.keyCode}",
        //            Duration = 3000
        //        });
        //    }

        //    switch ((int)e.keyCode)
        //    {
        //        case 112:
        //            e.preventDefault();
        //            await ButtonClickF1(null);
        //            break;
        //        case 113:
        //            e.preventDefault();
        //            await ButtonClickF2(null);
        //            break;
        //        case 114:
        //            e.preventDefault();
        //            await ButtonClickF3(null);
        //            break;
        //        case 115:
        //            e.preventDefault();
        //            await ButtonClickF4(null);
        //            break;
        //        case 116:
        //            e.preventDefault();
        //            await ButtonClickF5(null);
        //            break;
        //        case 117:
        //            e.preventDefault();
        //            await ButtonClickF6(null);
        //            break;
        //        case 118:
        //            e.preventDefault();
        //            await ButtonClickF7(null);
        //            break;
        //        case 119:
        //            e.preventDefault();
        //            await ButtonClickF8(null);
        //            break;
        //        case 120:
        //            e.preventDefault();
        //            await ButtonClickF9(null);
        //            break;
        //        case 121:
        //            e.preventDefault();
        //            await ButtonClickF9(null);
        //            break;
        //        case 122:
        //            e.preventDefault();
        //            await ButtonClickF11(null);
        //            break;
        //        case 123:
        //            //ファンクションキーのみ元のイベントをキャンセルする
        //            if (AssemblyState.Debug == true)
        //            {
        //            }
        //            else
        //            {
        //                e.preventDefault();
        //            }
        //            await ButtonClickF12(null);
        //            break;
        //        default:
        //            //それ以外のキーは通常通りのイベント
        //            break;
        //    }

        //}));

    }

    [JSInvokable("OnKeyDown")]
    public async void OnKeyDown(int keyCode)
    {
        switch (keyCode)
        {
            case 112:
                await ButtonClickF1(null);
                break;
            case 113:
                await ButtonClickF2(null);
                break;
            case 114:
                await ButtonClickF3(null);
                break;
            case 115:
                await ButtonClickF4(null);
                break;
            case 116:
                await ButtonClickF5(null);
                break;
            case 117:
                await ButtonClickF6(null);
                break;
            case 118:
                await ButtonClickF7(null);
                break;
            case 119:
                await ButtonClickF8(null);
                break;
            case 120:
                await ButtonClickF9(null);
                break;
            case 121:
                await ButtonClickF10(null);
                break;
            case 122:
                await ButtonClickF11(null);
                break;
            case 123:
                await ButtonClickF12(null);
                break;
            case 189:
                await ButtonClickNotify(null);
                break;
            case 190:
                await ButtonClickHomeNavigate(null);
                break;
            case 33:
                await ButtonClickPageUp(null);
                break;
            case 34:
                await ButtonClickPageDown(null);
                break;
            default:
                //それ以外のキーは通常通りのイベント
                break;
        }
    }

    protected async Task ButtonClickF1(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button1text) && string.IsNullOrWhiteSpace(IconName1))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF1)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF1 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF1 is not null)
            {
                await RadzenButtonF1.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF1.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF1 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF1 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF2(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button2text) && string.IsNullOrWhiteSpace(IconName2))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF2)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF2 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF2 is not null)
            {
                await RadzenButtonF2.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF2.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF2 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF2 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF3(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button3text) && string.IsNullOrWhiteSpace(IconName3))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF3)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF3 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF3 is not null)
            {
                await RadzenButtonF3.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF3.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF3 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF3 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF4(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button4text) && string.IsNullOrWhiteSpace(IconName4))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF4)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF4 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF4 is not null)
            {
                await RadzenButtonF4.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF4.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF4 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF4 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF5(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button5text) && string.IsNullOrWhiteSpace(IconName5))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF5)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF5 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF5 is not null)
            {
                await RadzenButtonF5.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF5.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF5 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF5 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF6(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button6text) && string.IsNullOrWhiteSpace(IconName6))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF6)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF6 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF6 is not null)
            {
                await RadzenButtonF6.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF6.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF6 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF6 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF7(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button7text) && string.IsNullOrWhiteSpace(IconName7))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF7)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF7 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF7 is not null)
            {
                await RadzenButtonF7.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF7.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF7 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF7 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF8(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button8text) && string.IsNullOrWhiteSpace(IconName8))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF8)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF8 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF8 is not null)
            {
                await RadzenButtonF8.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF8.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF8 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF8 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF9(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button9text) && string.IsNullOrWhiteSpace(IconName9))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF9)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF9 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF9 is not null)
            {
                await RadzenButtonF9.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF9.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF9 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF9 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF10(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button10text) && string.IsNullOrWhiteSpace(IconName10))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF10)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF10 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF10 is not null)
            {
                await RadzenButtonF10.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF10.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF10 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF10 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF11(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button11text) && string.IsNullOrWhiteSpace(IconName11))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF11)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF11 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF11 is not null)
            {
                await RadzenButtonF11.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF11.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF11 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF11 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }
    protected async Task ButtonClickF12(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(button12text) && string.IsNullOrWhiteSpace(IconName12))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyF12)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyF12 = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonF12 is not null)
            {
                await RadzenButtonF12.Element.FocusAsync(false);
            }

            if (IsBusyDialog)
            {
                //処理中ダイアログの表示
                _ = ComService.DialogShowBusy();
            }

            await OnClickResultF12.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyF12 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyF12 = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            {
                // 処理中ダイアログを閉じる
                _ = ComService!.DialogClose();
            }
            IsBusyDialogClose = true;
        }
    }

    /// <summary>
    /// F1ボタン処理を公開
    /// HTのバーコード読込時にファンクションボタンを処理する場合に使用
    /// ※isBusyFuncでファンクションボタン処理中は処理を飛ばす処理を行わないと、
    /// 　バーコード読込時のファンクション処理でエラーメッセージを表示した時に親画面のファンクションが走ってしまうため
    /// </summary>
    /// <returns></returns>
    public async Task PublicButtonClickF1()
    {
        await ButtonClickF1(null);
    }

    public async Task PublicButtonClickF5()
    {
        await ButtonClickF5(null);
    }

    public async Task PublicButtonClickF6()
    {
        await ButtonClickF6(null);
    }

    /// <summary>
    /// 処理を実行する間ファンクションボタンを無効にする
    /// MainLayoutの通知ログ一覧、ユーザー設定、ログアウトを行ったときに呼ぶ
    /// </summary>
    /// <param name="function"></param>
    public async Task ProcFunction(Func<Task> function)
    {
        try
        {
            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;

            // 実行
            await function();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;
        }
    }

    /// <summary>
    /// IsBusyDialogフラグを設定する
    /// </summary>
    /// <param name="val"></param>
    public void SetIsBusyDialogClose(bool val)
    {
        IsBusyDialogClose = val;
    }

    /// <summary>
    /// Notify
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected async Task ButtonClickNotify(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(buttonNotifytext) && string.IsNullOrWhiteSpace(IconNameNotify))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyNotify)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyNotify = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonNotify is not null)
            {
                await RadzenButtonNotify.Element.FocusAsync(false);
            }

            //if (IsBusyDialog)
            //{
            //    //処理中ダイアログの表示
            //    _ = ComService.DialogShowBusy();
            //}

            await OnClickHtNotify.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyNotify = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyNotify = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            //if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            //{
            //    // 処理中ダイアログを閉じる
            //    _ = ComService!.DialogClose();
            //}
            IsBusyDialogClose = true;
        }
    }

    /// <summary>
    /// HomeNavigate
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected async Task ButtonClickHomeNavigate(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(buttonHomeNavigatetext) && string.IsNullOrWhiteSpace(IconNameHomeNavigate))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyHomeNavigate)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyHomeNavigate = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonHomeNavigate is not null)
            {
                await RadzenButtonHomeNavigate.Element.FocusAsync(false);
            }

            //if (IsBusyDialog)
            //{
            //    //処理中ダイアログの表示
            //    _ = ComService.DialogShowBusy();
            //}

            await OnClickHtHomeNavigate.InvokeAsync(string.Empty);

            // ファンクション処理中のメッセージキューを削除
            await Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyHomeNavigate = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyHomeNavigate = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            //if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            //{
            //    // 処理中ダイアログを閉じる
            //    _ = ComService!.DialogClose();
            //}
            IsBusyDialogClose = true;
        }
    }

    /// <summary>
    /// PageUp
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected async Task ButtonClickPageUp(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(buttonPageUptext) && string.IsNullOrWhiteSpace(IconNamePageUp))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyPageUp)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyPageUp = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonPageUp is not null)
            {
                await RadzenButtonPageUp.Element.FocusAsync(false);
            }

            _ = InvokeAsync(async () =>
            {
                dynamic w = _js!.GetWindow();

                //一度フォーカスをBODYに設定する

                //w.document.getElementById(MSTR_BODY_ID).focus(); //TODO フッターフォーカス有り時にスクロールさせる対応とのことですが、効果がなかったので一旦コメントアウト


                int Height = w.document.getElementById(SharedConst.STR_BODY_ID).offsetHeight;

                Dictionary<string, object> dict = new(){
                    { "top", -Height },
                    { "left", 0 },
                    { "behavior", "smooth" }
                };


                await w.document.getElementById(SharedConst.STR_BODY_ID).scrollBy(dict);
            });


            //_ = OnClickPageUp.InvokeAsync(string.Empty);

            //フォーカスをBODYからはずす
            //w.document.getElementById(MSTR_BODY_ID).blur(); //TODO フッターフォーカス有り時にスクロールさせる対応とのことですが、効果がなかったので一旦コメントアウト
            // ファンクション処理中のメッセージキューを削除
            //_ = Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyPageUp = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyPageUp = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            //if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            //{
            //    // 処理中ダイアログを閉じる
            //    _ = ComService.DialogClose();
            //}
            IsBusyDialogClose = true;
        }
    }
    /// <summary>
    /// PageDown
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    protected async Task ButtonClickPageDown(MouseEventArgs? args)
    {
        //子コンポーネントで前処理が必要なら記載
        //親コンポーネントへイベント通知
        if (string.IsNullOrWhiteSpace(buttonPageDowntext) && string.IsNullOrWhiteSpace(IconNamePageDown))
        {
            return;
        }

        try
        {
            if (isBusyFunc || isBusyPageDown)
            {
                return;
            }

            ChildBaseService.IsFuncProc = true;
            isBusyFunc = true;
            isBusyPageDown = true;

            //// フォーカスを外す（キー入力の場合、テキストエリアなどにフォーカスがあるとバインドされないため）
            //dynamic window = _js.GetWindow();
            //window.document.activeElement.blur();
            if (RadzenButtonPageDown is not null)
            {
                await RadzenButtonPageDown.Element.FocusAsync(false);
            }

            _ = InvokeAsync(async () =>
            {
                dynamic w = _js!.GetWindow();

                //一度フォーカスをBODYに設定する

                //w.document.getElementById(MSTR_BODY_ID).focus(); //TODO フッターフォーカス有り時にスクロールさせる対応とのことですが、効果がなかったので一旦コメントアウト


                int Height = w.document.getElementById(SharedConst.STR_BODY_ID).offsetHeight;

                Dictionary<string, object> dict = new(){
                    { "top", Height },
                    { "left", 0 },
                    { "behavior", "smooth" }
                };


                await w.document.getElementById(SharedConst.STR_BODY_ID).scrollBy(dict);
            });

            //_ = OnClickPageDown.InvokeAsync(string.Empty);
            //フォーカスをBODYからはずす
            //w.document.getElementById(MSTR_BODY_ID).blur(); //TODO フッターフォーカス有り時にスクロールさせる対応とのことですが、効果がなかったので一旦コメントアウト
            //ファンクション処理中のメッセージキューを削除
            //_ = Task.Delay(N_FUNCPROC_QUEUEDEL_TIMER);

            isBusyPageDown = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            // コンポーネントを再レンダリングする
            StateHasChanged();
        }
        catch (Exception ex)
        {
            isBusyPageDown = false;
            isBusyFunc = false;
            ChildBaseService.IsFuncProc = false;

            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        finally
        {
            //if (!isBusyFunc && IsBusyDialog && IsBusyDialogClose)
            //{
            //    // 処理中ダイアログを閉じる
            //    _ = ComService.DialogClose();
            //}
            IsBusyDialogClose = true;
        }
    }
    /// <summary>
    /// PageUpDownボタン表示非表示制御
    /// </summary>
    /// <returns></returns>
    private async Task<bool> CheckVisibleButtonPageUpDownAsync()
    {
        await Task.Delay(0);
        decimal decscrollTop;
        decimal decscrollHeight;
        decimal decoffsetHeight;
        decimal decscrollButtom;
        bool preUpVisibleState = pageupvisible;
        bool preDownVisibleState = pagedownvisible;
        _js ??= await JS.CreateDymaicRuntimeAsync();
        dynamic w = _js!.GetWindow();
        if (w == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(buttonPageUptext) || !string.IsNullOrWhiteSpace(IconNamePageUp))
        {
            decscrollTop = w.document.getElementById(SharedConst.STR_BODY_ID).scrollTop;
            pageupvisible = decscrollTop > 0;
        }

        if (!string.IsNullOrWhiteSpace(buttonPageDowntext) || !string.IsNullOrWhiteSpace(IconNamePageDown))
        {
            decscrollHeight = w.document.getElementById(SharedConst.STR_BODY_ID).scrollHeight;
            decoffsetHeight = w.document.getElementById(SharedConst.STR_BODY_ID).offsetHeight;
            decscrollTop = w.document.getElementById(SharedConst.STR_BODY_ID).scrollTop;
            decscrollButtom = Math.Abs(decscrollHeight - (decoffsetHeight + decscrollTop));
            pagedownvisible = decscrollButtom > 1;
        }
        bool bret = preUpVisibleState != pageupvisible || preDownVisibleState != pagedownvisible;
        if (bret)
        {
            StateHasChanged();
        }
        return bret;
    }
}

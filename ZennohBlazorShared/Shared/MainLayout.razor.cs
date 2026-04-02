using Blazor.DynamicJS;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedModels;
using System.Text.RegularExpressions;
using System.Timers;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;

namespace ZennohBlazorShared.Shared;
public partial class MainLayout : LayoutComponentBase, IAsyncDisposable
{
    [Inject]
    private ApplicationVersion? applicationVersion { get; set; } = null!;
    [Inject]
    protected HtService? htService { get; set; }

    /// <summary>
    /// MainLayoutフッターのFuncButtonの情報
    /// </summary>
    public IDictionary<string, object> AttributesFuncButton { get; set; } = new Dictionary<string, object>();

    public string PageName { get; set; } = "WMS";

    /// <summary>
    /// HT端末かどうか（true:HT端末　false:HT端末以外）
    /// </summary>
    public bool IsHandy = false;

    /// <summary>
    /// ログイン中かどうか（true:ログイン　false:ログアウト）
    /// </summary>
    public bool IsLogin = false;

    /// <summary>
    /// 操作日時の排他ロックのためのSemaphoreSlim
    /// </summary>
    private static readonly SemaphoreSlim SemaphoreOpeDate = new(1);

    private ButtonFuncRadzen? buttonFunc { get; set; }

    #region Event Notify

    //Bodyイベント通知用
    private async Task OnClickResultF1(string result)
    {
        await ChildBaseService.EventMainLayoutF1ClickAsync(this);
    }

    private async Task OnClickResultF2(string result)
    {
        await ChildBaseService.EventMainLayoutF2ClickAsync(this);
    }

    private async Task OnClickResultF3(string result)
    {
        await ChildBaseService.EventMainLayoutF3ClickAsync(this);
    }

    private async Task OnClickResultF4(string result)
    {
        await ChildBaseService.EventMainLayoutF4ClickAsync(this);
    }

    private async Task OnClickResultF5(string result)
    {
        await ChildBaseService.EventMainLayoutF5ClickAsync(this);
    }

    private async Task OnClickResultF6(string result)
    {
        await ChildBaseService.EventMainLayoutF6ClickAsync(this);
    }

    private async Task OnClickResultF7(string result)
    {
        await ChildBaseService.EventMainLayoutF7ClickAsync(this);
    }

    private async Task OnClickResultF8(string result)
    {
        await ChildBaseService.EventMainLayoutF8ClickAsync(this);
    }

    private async Task OnClickResultF9(string result)
    {
        await ChildBaseService.EventMainLayoutF9ClickAsync(this);
    }

    private async Task OnClickResultF10(string result)
    {
        await ChildBaseService.EventMainLayoutF10ClickAsync(this);
    }

    private async Task OnClickResultF11(string result)
    {
        await ChildBaseService.EventMainLayoutF11ClickAsync(this);
    }

    private async Task OnClickResultF12(string result)
    {
        await ChildBaseService.EventMainLayoutF12ClickAsync(this);
    }

    private async Task OnClickHtNotify(string result)
    {
        await ChildBaseService.EventMainLayoutHtNotifyClickAsync(this);
    }

    private async Task OnClickHtHomeNavigate(string result)
    {
        await ChildBaseService.EventMainLayoutHtHomeNavigateClickAsync(this);
    }
    //PageUp
    private async Task OnClickPageUp(string result)
    {
        await ChildBaseService.EventMainLayoutPageUpClickAsync(this);
    }
    //PageDown
    private async Task OnClickPageDown(string result)
    {
        await ChildBaseService.EventMainLayoutPageDownClickAsync(this);
    }

    private async Task OnClickUser()
    {
        try
        {
            if (buttonFunc == null)
            {
                return;
            }
            await buttonFunc.ProcFunction(
                async () =>
                {
                    await ChildBaseService.EventMainLayoutUserSettingClickAsync(this);
                }
            );
        }
        catch (Exception ex)
        {
            _ = CommService.PostLogAsync(ex.Message);
        }
    }

    #endregion

    private string? versionText { get; set; }
    private string? userCode { get; set; }
    private string? userName { get; set; }
    private string? password { get; set; }
    private string? LoginUserName { get; set; }
    private string? AffiliationName { get; set; }
    private int NotifyCategory { get; set; }
    private string LogonReadCodeChar { get; set; } = "$$$";

    private string? ArrivalIcon { get; set; }
    private string? ShipmentIcon { get; set; }
    private string? GetUnfinishedDataFlg { get; set; } = "false";
    private bool ArrivalIconVisible { get; set; } = false;
    private bool ShipmentIconVisible { get; set; } = false;
    private string? ArrivalStyle { get; set; }
    private string? ShipmentStyle { get; set; }

    private bool sidebarLeftExpanded = true;
    private readonly int notifyCount = 1;
    private Data.LoginInfo[]? allUser;
    // 通知状態確認タイマー
    private System.Timers.Timer? timeLogNotify;
    // ログイン状態監視タイマー
    private System.Timers.Timer? timeLogin;
    // ログイン状態監視タイマー
    private System.Timers.Timer? timeUnfinished;
    private CustomLoginForm? loginForm { get; set; }
    private readonly System.Timers.Timer? timeHandyLogin;
    private readonly List<Task<bool>> DefineInitialTask = new();

    public string MainLayoutMenuFontSize { get; set; } = "140%";
    public string MainLayoutMenuFontWeight { get; set; } = "bold";
    public string MainLayoutAffiliationFontSize { get; set; } = "100%";
    public string MainLayoutAffiliationFontWeight { get; set; } = "normal";
    public string MainLayoutUserFontSize { get; set; } = "100%";
    public string MainLayoutUserFontWeight { get; set; } = "normal";

    //Blazor.DynamicJS
    private DynamicJSRuntime? _js;
    public ValueTask DisposeAsync()
    {
        _ = JS.InvokeVoidAsync("removeEnterKeyPressListener");
        return _js?.DisposeAsync() ?? ValueTask.CompletedTask;
    }

    protected override void OnInitialized()
    {
        ChildBaseService.EventChangeChild -= OnChangeChild;
        ChildBaseService.EventChangeChild += OnChangeChild;
        base.OnInitialized();
    }

    /// <summary>
    /// Bodyページで変更があった時の処理を記載
    /// Bodyページより変更通知イベントにて発火する
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnChangeChild(object? sender, object? e)
    {
        // コンポーネントを再レンダリングする
        StateHasChanged();

    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // 初回起動時"/"以外のURLが指定された場合、"/"に遷移させる
        Uri uri = new(NavigationManager.Uri);
        if (uri.AbsolutePath != "/")
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        // ユーザ情報を取得する
        _ = InvokeAsync(async () =>
        {
            allUser = await CommService.GetLoginInfoAsync();
        });

        //TODO 多言語設定　一旦日本語
        //languageContainer.SetLanguage(System.Globalization.CultureInfo.GetCultureInfo("ja-JP"));

        // バージョンの表示
        versionText = applicationVersion!.Version;

        // 端末情報を取得する
        _js = await JS.CreateDymaicRuntimeAsync();
        dynamic window = _js.GetWindow();
        dynamic ua = window.navigator.userAgent;
        string strUa = (string)ua;
        //TODO DEFINE_DEVICE_TYPESのJUDGE_STRINGでモバイル機か判断する。今後の展開で実装。
        // 端末がHTかどうか
        IsHandy = Regex.IsMatch(strUa, @"BTA500|android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini", RegexOptions.IgnoreCase);
        ChildBaseService.IsHandy = IsHandy;

        // JavaScriptのイベントを対応付け
        DotNetObjectReference<MainLayout> dotNetReference = DotNetObjectReference.Create(this);
        await JS.InvokeVoidAsync("initializeEnterKeyPressListener", dotNetReference);

        // システムパラメータ取得してセッションストレージに保持する
        SystemParameter sysParams = await CommService.GetSystemParameter();
        await SessionStorage.SetItemAsync(SharedConst.KEY_SYSTEM_PARAM, sysParams);
        // メインレイアウト関連情報はセットしなおす
        MainLayoutMenuFontSize = IsHandy ? sysParams.MainLayoutMenuFontSizeHT : sysParams.MainLayoutMenuFontSizePC;
        MainLayoutMenuFontWeight = IsHandy ? sysParams.MainLayoutMenuFontWeightHT : sysParams.MainLayoutMenuFontWeightPC;
        MainLayoutAffiliationFontSize = sysParams.MainLayoutAffiliationFontSizePC;
        MainLayoutAffiliationFontWeight = sysParams.MainLayoutAffiliationFontWeightPC;
        MainLayoutUserFontSize = IsHandy ? sysParams.MainLayoutUserFontSizeHT : sysParams.MainLayoutUserFontSizePC;
        MainLayoutUserFontWeight = IsHandy ? sysParams.MainLayoutUserFontWeightHT : sysParams.MainLayoutUserFontWeightPC;
        LogonReadCodeChar = sysParams.LogonReadCodeChar;
        ChildBaseService.FontSizeValidationSammary = IsHandy ? sysParams.HT_ValidationSummaryFontSize : sysParams.PC_ValidationSummaryFontSize;
        ChildBaseService.FontWeightValidationSammary = IsHandy ? sysParams.HT_ValidationSummaryFontWeight : sysParams.PC_ValidationSummaryFontWeight;
        //入荷未完了、出荷未完了表示関連
        ArrivalIcon = sysParams.HT_GetUnfinishedArrivalIcon;
        ShipmentIcon = sysParams.HT_GetUnfinishedShipmentIcon;
        GetUnfinishedDataFlg = sysParams.HT_GetUnfinishedDataFlg;
        ArrivalStyle = $"background-color:{sysParams.HT_GetUnfinishedArrivalBackGroundColor};color:{sysParams.HT_GetUnfinishedArrivalColor}";
        ShipmentStyle = $"background-color:{sysParams.HT_GetUnfinishedShipmentBackGroundColor};color:{sysParams.HT_GetUnfinishedShipmentColor}";
        ArrivalIconVisible = false;
        ShipmentIconVisible = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        //await CommService.PostLogAsync($"OnAfterRenderAsync IsHandy:{IsHandy}, IsLogin:{IsLogin}, firstRender:{firstRender}");

        if (IsHandy && !IsLogin)
        {
            // HTスキャンイベント登録
            HtService.HtScanEvent -= HtService_HtScanEvent;
            HtService.HtScanEvent += HtService_HtScanEvent;
            htService!.SetReadCallback();
            _ = htService!.UnLockScanner();//読み取りロック解除。なぜか読み取り不可となる場合があるため対策用
        }

        // 操作日時を更新
        await UpdateOperationDate();

        if (!firstRender)
        {
            return;
        }

        if (IsHandy && !IsLogin)
        {
            // HTの場合、ログイン画面は最下部にスクロール
            if (_js != null)
            {
                dynamic window = _js!.GetWindow();
                window.scrollTo(0, 1000);   // スクロールサイズは十分大きな値としておく
            }
        }

        try
        {
            _js ??= await JS.CreateDymaicRuntimeAsync();
            dynamic w = _js.GetWindow();

            //windowの閉じるが押されたときのイベントbeforeunloadを定義
            ////Jsリスナーの定義
            w.addEventListener("beforeunload", (Action<dynamic>)(async e =>
            {
                //ログアウト
                if (IsLogin)
                {
                    //ログアウト時、キャンセルトークンの判定を無視させる。
                    await ProcLogout(isAutoLogout: false, isCancelIgnore: true);
                    //PWAでawait Task.Delayで待っていたので特に待機処理は不要の認識
                }
            }));
        }
        catch (Exception ex)
        {
            _ = CommService.PostLogAsync(ex.Message);
        }


    }

    /// <summary>
    /// JavaScript Enterのkeypressイベント
    /// </summary>
    /// <param name="activeId">Enterキーが押下された時のactiveElementのID</param>
    [JSInvokable("OnEnterKeyPress")]
    public void OnEnterKeyPress(string activeId)
    {
        // ログイン後にEnterキーが押下された場合、最後にフォーカスが当たっていたElementにフォーカスを戻す
        if (!string.IsNullOrEmpty(ChildBaseService.LastFocusId) && !activeId.Contains(ChildBaseService.LastFocusId))
        {
            if (_js != null)
            {
                dynamic window = _js!.GetWindow();
                dynamic element = window.document.getElementById(ChildBaseService.LastFocusId);
                element?.focus();
            }
        }
    }

    /// <summary>
    /// ログイン処理
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async Task OnLogin(LoginArgs args)
    {
        int notifyDuration = SharedConst.DEFAULT_NOTIFY_DURATION;

        // ユーザ情報が取得出来ていなかった場合はログイン処理前に取得する
        if (allUser == null || allUser.Count() <= 0)
        {
            allUser = await CommService.GetLoginInfoAsync();
        }
        LoginInfo? login = allUser.FirstOrDefault(_ =>
            _.Id == args.Username
            && _.Password == args.Password
        );
        if (login is null)
        {
            // ユーザーが取得できなかったためNG
            NotificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Error,
                Summary = "ログイン失敗",
                Detail = "ユーザー取得にに失敗しました。",
                Duration = notifyDuration
            });
            return;
        }

        //以下、ログイン成功時の処理

        //ログイン処理に入ったなら、ローカルストレージの情報をすべてクリアしておく。PC,HTいずれも途中で何かの理由でブラウザが閉じられたことを考慮。
        await CommService.ClearAllLocalStorageValue();

        // ログイン情報取得し、セッションストレージへセット
        await SessionStorage.SetItemAsync(SharedConst.KEY_LOGIN_INFO, login);

        // 画面表示情報更新
        LoginUserName = login.UserName;
        AffiliationName = login.AffiliationName;

        // ユーザーが取得できたためLogin
        bool bBusyShow = false;

        try
        {
            _ = CommService.DialogShowBusy("取得中..");
            bBusyShow = true;
            DefineInitialTask.Clear();

            // システムパラメータ取得してローカルストレージに保持する
            SystemParameter sysParams = await CommService.GetSystemParameter();
            await SessionStorage.SetItemAsync(SharedConst.KEY_SYSTEM_PARAM, sysParams);
            // メインレイアウト関連情報はセットしなおす
            MainLayoutMenuFontSize = IsHandy ? sysParams.MainLayoutMenuFontSizeHT : sysParams.MainLayoutMenuFontSizePC;
            MainLayoutMenuFontWeight = IsHandy ? sysParams.MainLayoutMenuFontWeightHT : sysParams.MainLayoutMenuFontWeightPC;
            MainLayoutAffiliationFontSize = sysParams.MainLayoutAffiliationFontSizePC;
            MainLayoutAffiliationFontWeight = sysParams.MainLayoutAffiliationFontWeightPC;
            MainLayoutUserFontSize = IsHandy ? sysParams.MainLayoutUserFontSizeHT : sysParams.MainLayoutUserFontSizePC;
            MainLayoutUserFontWeight = IsHandy ? sysParams.MainLayoutUserFontWeightHT : sysParams.MainLayoutUserFontWeightPC;
            LogonReadCodeChar = sysParams.LogonReadCodeChar;
            ChildBaseService.FontSizeValidationSammary = IsHandy ? sysParams.HT_ValidationSummaryFontSize : sysParams.PC_ValidationSummaryFontSize;
            ChildBaseService.FontWeightValidationSammary = IsHandy ? sysParams.HT_ValidationSummaryFontWeight : sysParams.PC_ValidationSummaryFontWeight;
            notifyDuration = sysParams.NotifyPopupDuration;

            // ログインストアド実行
            bool retb = await CommService.ExecLoginFunc(GetType().Name, applicationVersion!.Version);
            if (!retb)
            {
                NotificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "ログイン失敗",
                    Detail = "ログインに失敗しました。",
                    Duration = notifyDuration
                });
                //後続は実行したくないため戻る
                return;
            }

            // PC画面間パラメータをリセット
            _ = CommService.ClearAllPCTransParam();

            // 画面毎の設定関連情報はログイン時に保持しておく
            // System.Type等の変数はJsonシリアライズ化で失敗しセッションストレージに保持できないため、
            // サービスに保持してその値を使用する（サービス変数はアプリが終了されるまで保持される）
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    // メニュー情報を取得してセッションストレージに保持する
                    List<MenuInfo> lstMenuInfo = await CommService.GetMenuInfoAllAsync(login.AuthorityLevel);
                    await SessionStorage.SetItemAsync(SharedConst.KEY_MENU_INFO, lstMenuInfo);
                }
                catch (Exception ex)
                {
                    _ = CommService.PostLogAsync($"OnLogin_GetMenuInfoAllAsync_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    Dictionary<string, string> PageNames = await CommService.GetPageTitleAsyncAll();
                    CommService.PageNameAll = PageNames;
                }
                catch (Exception ex)
                {
                    _ = CommService.PostLogAsync($"OnLogin_GetPageTitleAsyncAll_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    Dictionary<string, List<ComponentsInfo>> ComponentsInfos = await CommService.GetComponetnsInfoAll();
                    CommService.ComponentsInfoAll = ComponentsInfos;
                }
                catch (Exception ex)
                {
                    _ = CommService.PostLogAsync($"OnLogin_GetComponetnsInfoAll_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    Dictionary<string, List<ComponentColumnsInfo>> ComponentColumnsInfos = await CommService.GetGridColumnsDataAll();
                    CommService.ComponentColumnsAll = ComponentColumnsInfos;
                }
                catch (Exception ex)
                {
                    _ = CommService.PostLogAsync($"OnLogin_GetGridColumnsDataAll_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    Dictionary<string, List<ComponentProgramInfo>> ComponentProgramInfos = await CommService.GetComponentProgramInfoAll();
                    CommService.ComponentProgramAll = ComponentProgramInfos;
                }
                catch (Exception ex)
                {
                    _ = CommService.PostLogAsync($"OnLogin_GetComponentProgramInfoAll_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    List<MstAreaData> MstAreaInfos = await CommService.GetArea();
                    CommService.MstAreaInfoAll = MstAreaInfos;
                }
                catch (Exception ex)
                {
                    _ = CommService.PostLogAsync($"OnLogin_GetArea_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    List<MstZoneData> MstZoneInfos = await CommService.GetZone();
                    CommService.MstZoneInfoAll = MstZoneInfos;
                }
                catch (Exception ex)
                {
                    _ = CommService.PostLogAsync($"OnLogin_GetZone_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    List<MstLocationData> MstLocationInfos = await CommService.GetLocation();
                    CommService.MstLocationInfoAll = MstLocationInfos;
                }
                catch (Exception ex)
                {
                    _ = CommService.PostLogAsync($"OnLogin_GetLocation_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            // DEFINE関連データが取得できるまで待機
            _ = await Task.WhenAll(DefineInitialTask);

            _ = CommService.DialogClose();
            bBusyShow = false;

            // DefineInitialTask 内のタスクがすべて完了したかどうかを判断
            if (DefineInitialTask.All(_ =>
                _.IsCompletedSuccessfully == true
                && _.Result == true //他の条件はエラーが発生してもtrueとなってしまうので、タスクの戻り値を設けて確認するように修正。
                && _.IsFaulted == false
                && _.IsCanceled == false)
                )
            {
                _ = CommService.PostLogAsync("ログインパラメータ取得_成功。");
                NotificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "ログイン成功",
                    Detail = "ログインしました。",
                    Duration = notifyDuration
                });
            }
            else
            {
                _ = CommService.PostLogAsync("ログインパラメータ取得_失敗。");
                NotificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "ログイン失敗",
                    Detail = "パラメータ取得に失敗しました。",
                    Duration = notifyDuration
                });
                //後続は実行したくないため戻る
                return;
            }
            //ログイン成功とし、メイン画面を表示する
            IsLogin = true;

            if (IsHandy)
            {
                // HTスキャンイベント解除
                HtService.HtScanEvent -= HtService_HtScanEvent;
                // HT用メニュー画面に遷移
                await OnNavigateMobile();
                //未完了データ取得フラグを見てデータ取得を開始
                if (GetUnfinishedDataFlg == "true")
                {
                    //処理の起動有無を確認し、未完了データ取得処理を開始
                    StartUnfinishedDataTimer(sysParams.HT_GetUnfinishedDataInterval);
                }
            }
            else
            {
                // PCの場合は通知状態確認タイマーを起動する
                StartLogNotifyTimer(sysParams.LogNotifyInterval);
            }

            // 操作日時を更新
            await UpdateOperationDate();

            // 自動ログアウトモードに応じて、ログイン監視タイマースタート
            if (sysParams.AutoLogoutDeviceGroup == 999 ||
                (!IsHandy && sysParams.AutoLogoutDeviceGroup == 1) ||
                (IsHandy && sysParams.AutoLogoutDeviceGroup == 2))
            {
                StartLoginTimer(sysParams.AutoLogoutCheckInterval);
            }

        }
        catch (Exception ex)
        {
            _ = CommService.PostLogAsync(ex.Message);

            NotificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Error,
                Summary = "ログイン失敗",
                Detail = "ログインに失敗しました。",
                Duration = notifyDuration
            });
        }
        finally
        {
            if (bBusyShow)
            {
                _ = CommService.DialogClose();
            }
        }
    }

    /// <summary>
    /// ログアウト処理
    /// </summary>
    /// <returns></returns>
    private async Task OnClickLogout()
    {
        try
        {
            if (buttonFunc == null)
            {
                return;
            }
            await buttonFunc.ProcFunction(
                async () =>
                {
                    bool? ret = await CommService.DialogShowYesNo("ログアウトします。よろしいですか。");
                    bool retb = ret is not null && (bool)ret;

                    if (retb)
                    {

                        await ProcLogout();
                    }
                    else
                    {
                    }
                }
            );
        }
        catch (Exception ex)
        {
            _ = CommService.PostLogAsync(ex.Message);
        }
    }

    private async Task OnClickNotified()
    {
        try
        {
            if (buttonFunc == null)
            {
                return;
            }
            await buttonFunc.ProcFunction(
                async () =>
                {
                    // ダイアログ情報を取得
                    string strDialogTitle = "通知一覧";
                    int intDialogWidth = 1600;
                    int intDialogHeight = 900;

                    // 通知画面に遷移
                    dynamic window = _js!.GetWindow();
                    int innerWidth = (int)window.innerWidth;
                    int innerHeight = (int)window.innerHeight;
                    _ = await DialogService.OpenAsync<DialogAlarmInfo>(
                        $"{strDialogTitle}",
                        null,
                        new DialogOptions()
                        {
                            Width = $"{Math.Min(intDialogWidth, innerWidth)}px",
                            Height = $"{Math.Min(intDialogHeight, innerHeight)}px",
                            Resizable = true,
                            Draggable = true
                        }
                    );
                }
            );
        }
        catch (Exception ex)
        {
            _ = CommService.PostLogAsync(ex.Message);
        }
    }

    /// <summary>
    /// HTメニュー画面に遷移
    /// </summary>
    /// <returns></returns>
    private async Task OnNavigateMobile()
    {
        await Task.Delay(0);
        sidebarLeftExpanded = false;
        //await CommService.PostLogAsync($"OnNavigateMobile IsHandy:{IsHandy}, IsLogin:{IsLogin}");

        NavigationManager.NavigateTo("mobile_menu");
    }

    /// <summary>
    /// ヘッダートグルボタンクリック時の処理として定義
    /// </summary>
    private async void OnClickSidebarToggle()
    {
        if (IsHandy)
        {
            await OnNavigateMobile();
        }
        else
        {
            sidebarLeftExpanded = !sidebarLeftExpanded;
        }
    }

    private void OnUserNameChanged(string args)
    {
        IEnumerable<Data.LoginInfo>? getUser = allUser?.Where(_ =>
            _.Id == args
        );
        string? userName = (getUser is not null && getUser.Any()) ? getUser.First().UserName : string.Empty;
        loginForm?.SetWorkename(userName!);
    }

    /// <summary>
    /// スキャンされた時の処理
    /// </summary>
    /// <param name="scanData"></param>
    protected async Task HtService_HtScanEvent(ScanData scanData)
    {
        try
        {
            string value = scanData.strStringData;

            // $$$が存在する場合は、$$$より前をユーザIDとしてログインする
            int pos = value.IndexOf(LogonReadCodeChar);
            if (-1 != pos)
            {
                value = value[..pos];
                // ユーザ情報が取得出来ていなかった場合はログイン処理前に取得してユーザIDが一致するパスワードで自動ログインさせる
                if (allUser == null || allUser.Count() <= 0)
                {
                    allUser = await CommService.GetLoginInfoAsync();
                }
                IEnumerable<Data.LoginInfo> usr = allUser.Where(_ => _.Id == value);
                if (usr.Any())
                {
                    await OnLogin(new LoginArgs { Username = value, Password = usr.First().Password });
                }
            }
        }
        catch (Exception ex)
        {
            _ = CommService.PostLogAsync(ex.Message);
        }
    }

    /// <summary>
    /// ログアウト処理
    /// </summary>
    /// <param name="isAutoLogout"></param>
    /// <returns></returns>
    public async Task ProcLogout(bool isAutoLogout = false, bool isCancelIgnore = false)
    {
        // タイマー停止
        StopLogNotifyTimer();
        StopLoginTimer();
        StopUnfinishedDataTimer();

        // ログアウトストアド実行
        await CommService.ExecLogoutFunc(GetType().Name, isCancelIgnore);

        // ログイン情報を削除する
        await SessionStorage.RemoveItemAsync(SharedConst.KEY_LOGIN_INFO);

        // 入荷で入力した倉庫コード、ゾーンコード、ロケーションコードを削除する
        await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_AREA_ID);
        await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ZONE_ID);
        await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_LOCATION_ID);

        NavigationManager.NavigateTo("/");
        // コンポーネントを再レンダリングする
        StateHasChanged();
        // 変数初期化
        userName = string.Empty;
        LoginUserName = string.Empty;
        AffiliationName = string.Empty;
        IsLogin = false;

        // 通知時間取得
        int notifyDuration = SharedConst.DEFAULT_NOTIFY_DURATION;
        if (await SessionStorage.ContainKeyAsync(SharedConst.KEY_SYSTEM_PARAM))
        {
            SystemParameter sysParams = await SessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
            notifyDuration = sysParams.NotifyPopupDuration;
        }

        // トップページに遷移する
        // ※ダイアログ表示中であっても全てのダイアログを閉じさせるため
        NavigationManager.NavigateTo("/");

        // 通知
        if (isAutoLogout)
        {
            NotificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Success,
                Summary = "自動ログアウト",
                Detail = "操作しない状態が続いたためログアウトしました。",
                Duration = notifyDuration
            });
        }
        else
        {
            NotificationService.Notify(new NotificationMessage()
            {
                Severity = NotificationSeverity.Success,
                Summary = "ログアウト成功",
                Detail = "ログアウトしました。",
                Duration = notifyDuration
            });
        }

        if (IsHandy)
        {
            // HTスキャンイベント登録
            HtService.HtScanEvent -= HtService_HtScanEvent;
            HtService.HtScanEvent += HtService_HtScanEvent;
        }

        // 全ユーザ情報を取得する（ログインパスワードが変更されているまたは、作業者が追加されている場合があるため）
        allUser = await CommService.GetLoginInfoAsync();
    }

    /// <summary>
    /// 通知ログ監視タイマーの起動
    /// </summary>
    private void StartLogNotifyTimer(int Intartval)
    {
        timeLogNotify = new System.Timers.Timer(Intartval);
        timeLogNotify.Elapsed += OnLogNotifyTimedEvent;
        timeLogNotify.AutoReset = true;
        timeLogNotify.Enabled = true;
    }

    /// <summary>
    /// 通知ログ監視タイマーの停止
    /// </summary>
    private void StopLogNotifyTimer()
    {
        if (timeLogNotify is not null)
        {
            timeLogNotify.Enabled = false;
        }
    }

    /// <summary>
    /// 通知ログタイマー
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void OnLogNotifyTimedEvent(object? source, ElapsedEventArgs e)
    {
        _ = GetLogNotifyStatus();
        StateHasChanged();
    }

    /// <summary>
    /// 通知状態取得
    /// </summary>
    /// <returns></returns>
    private async Task GetLogNotifyStatus()
    {
        ClassNameSelect select = new()
        {
            viewName = "VW_通知状態",
        };
        List<IDictionary<string, object>> datas = await CommService!.GetSelectData(select);
        if (null != datas && datas.Count > 0)
        {
            IDictionary<string, object> dic = datas.First();
            NotifyCategory = CommService.GetValueInt(dic, "通知状態");
        }
    }

    /// <summary>
    /// ログイン監視タイマーの起動
    /// </summary>
    private void StartLoginTimer(int Intartval)
    {
        StopLoginTimer();
        timeLogin = new System.Timers.Timer(Intartval);
        timeLogin.Elapsed += OnLoginTimedEvent;
        timeLogin.AutoReset = true;
        timeLogin.Enabled = true;
    }

    /// <summary>
    /// ログイン監視タイマーの停止
    /// </summary>
    private void StopLoginTimer()
    {
        if (timeLogin is not null)
        {
            timeLogin.Enabled = false;
            timeLogin.Elapsed -= OnLoginTimedEvent;
        }
    }

    /// <summary>
    /// ログインタイマーイベント
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void OnLoginTimedEvent(object? source, ElapsedEventArgs e)
    {
        _ = CheckLoginStatus();
    }

    /// <summary>
    /// ログイン状態チェック
    /// </summary>
    /// <returns></returns>
    private async Task CheckLoginStatus()
    {
        bool isProcLogout = false;
        try
        {
            if (await SessionStorage.ContainKeyAsync(SharedConst.KEY_SYSTEM_PARAM))
            {
                // システムパラメータ取得
                SystemParameter sysParams = await SessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);

                // 操作日時をセッションストレージから取出しチェック
                await SemaphoreOpeDate.WaitAsync();
                try
                {
                    if (await SessionStorage.ContainKeyAsync(SharedConst.KEY_OPERATION_DATE))
                    {
                        DateTime opeDate = await SessionStorage.GetItemAsync<DateTime>(SharedConst.KEY_OPERATION_DATE);
                        // 操作日時から自動ログアウト時間[分]経過していたらログアウト
                        if (opeDate.AddMinutes(sysParams.AutoLogoutTime) < DateTime.Now)
                        {
                            isProcLogout = true;
                        }
                    }
                }
                finally
                {
                    _ = SemaphoreOpeDate.Release();
                }
            }
        }
        catch (Exception ex)
        {
            _ = CommService.PostLogAsync(ex.Message);
        }

        if (isProcLogout)
        {
            // ログアウト処理
            await ProcLogout(isAutoLogout: true);
            StateHasChanged();
        }
    }

    /// <summary>
    /// 操作日時を更新
    /// </summary>
    /// <returns></returns>
    public async Task UpdateOperationDate()
    {
        await SemaphoreOpeDate.WaitAsync();
        try
        {
            await SessionStorage.SetItemAsync(SharedConst.KEY_OPERATION_DATE, DateTime.Now);
        }
        catch (Exception ex)
        {
            _ = CommService.PostLogAsync(ex.Message);
        }
        finally
        {
            _ = SemaphoreOpeDate.Release();
        }
    }

    /// <summary>
    /// ButtonFuncRadzenのF1ボタン処理呼出
    /// HTのバーコード読込時にファンクションボタンを処理する場合に使用
    /// </summary>
    /// <returns></returns>
    public async Task ButtonClickF1()
    {
        if (buttonFunc == null)
        {
            return;
        }
        await buttonFunc.PublicButtonClickF1();
    }

    /// <summary>
    /// IsBusyDialogフラグを設定する
    /// </summary>
    /// <param name="val"></param>
    public void SetIsBusyDialogClose(bool val)
    {
        if (buttonFunc == null)
        {
            return;
        }
        buttonFunc.SetIsBusyDialogClose(val);
    }

    /// <summary>
    /// 未完了監視タイマーの起動
    /// </summary>
    private void StartUnfinishedDataTimer(int Intartval)
    {
        StopUnfinishedDataTimer();
        timeUnfinished = new System.Timers.Timer(Intartval);
        timeUnfinished.Elapsed += OnUnfinishedDataTimedEvent;
        timeUnfinished.AutoReset = true;
        timeUnfinished.Enabled = true;
    }

    /// <summary>
    /// 未完了データ監視タイマーの停止
    /// </summary>
    private void StopUnfinishedDataTimer()
    {
        if (timeUnfinished is not null)
        {
            timeUnfinished.Enabled = false;
            timeUnfinished.Elapsed -= OnUnfinishedDataTimedEvent;
        }
    }

    /// <summary>
    /// 未完了データチェックタイマーイベント
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void OnUnfinishedDataTimedEvent(object? source, ElapsedEventArgs e)
    {
        _ = GetUnfinishedData();
        StateHasChanged();
    }

    /// <summary>
    /// 未完了データ取得
    /// </summary>
    /// <returns></returns>
    private async Task GetUnfinishedData()
    {

        ClassNameSelect select = new()
        {
            viewName = "VW_HT_未完了データ数取得",
        };
        List<IDictionary<string, object>> datas = await CommService!.GetSelectData(select);
        foreach (IDictionary<string, object> data in datas)
        {
            if (CommService.GetValueString(data, "データ名称") == "入荷検品未完了データ数")
            {
                ArrivalIconVisible = CommService.GetValueInt(data, "未完了データ数") > 0;
            }
            else if (CommService.GetValueString(data, "データ名称") == "出荷未完了データ数")
            {
                ShipmentIconVisible = CommService.GetValueInt(data, "未完了データ数") > 0;
            }
        }
    }

}

using Blazor.DynamicJS;
using DocumentFormat.OpenXml.EMMA;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Util;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;
using SharedModels;
using Sotsera.Blazor.Toaster.Core.Models;
using System.Timers;
using static SharedModels.SharedConst;

namespace ExpressionDBBlazorShared.Shared;
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
    private async Task OnClickResultF1(string result) => await ChildBaseService.EventMainLayoutF1ClickAsync(this);
    private async Task OnClickResultF2(string result) => await ChildBaseService.EventMainLayoutF2ClickAsync(this);
    private async Task OnClickResultF3(string result) => await ChildBaseService.EventMainLayoutF3ClickAsync(this);
    private async Task OnClickResultF4(string result) => await ChildBaseService.EventMainLayoutF4ClickAsync(this);
    private async Task OnClickResultF5(string result) => await ChildBaseService.EventMainLayoutF5ClickAsync(this);
    private async Task OnClickResultF6(string result) => await ChildBaseService.EventMainLayoutF6ClickAsync(this);
    private async Task OnClickResultF7(string result) => await ChildBaseService.EventMainLayoutF7ClickAsync(this);
    private async Task OnClickResultF8(string result) => await ChildBaseService.EventMainLayoutF8ClickAsync(this);
    private async Task OnClickResultF9(string result) => await ChildBaseService.EventMainLayoutF9ClickAsync(this);
    private async Task OnClickResultF10(string result) => await ChildBaseService.EventMainLayoutF10ClickAsync(this);
    private async Task OnClickResultF11(string result) => await ChildBaseService.EventMainLayoutF11ClickAsync(this);
    private async Task OnClickResultF12(string result) => await ChildBaseService.EventMainLayoutF12ClickAsync(this);

    private async Task OnClickHtNotify(string result) => await ChildBaseService.EventMainLayoutHtNotifyClickAsync(this);
    private async Task OnClickHtHomeNavigate(string result) => await ChildBaseService.EventMainLayoutHtHomeNavigateClickAsync(this);    
    private async Task OnClickPageUp(string result) => await ChildBaseService.EventMainLayoutPageUpClickAsync(this);//PageUp    
    private async Task OnClickPageDown(string result) => await ChildBaseService.EventMainLayoutPageDownClickAsync(this);//PageDown

    // 列名（DB の実カラム名に合わせて修正可）
    private const string COL_MNG_NO = "管理責任者";
    private const string COL_SHELFID = "棚ID";

    // 同画面内でのキャッシュ（全件取得結果を保持）
    //ITEM_STORAGEテーブル用
    private List<IDictionary<string, object>>? _itemStorageCache;
    // 同画面内でのキャッシュ（全件取得結果を保持）
    //MST_SHELF用
    private List<IDictionary<string, object>>? _itemStorageCache2;

    /// <summary>
    /// null/DBNull/空白/大小文字差を吸収して比較するヘルパー
    /// </summary>
    private static bool EqualStr(object? o, string target)
    {
        var s = (o == null || o is DBNull) ? "" : o.ToString() ?? "";
        return string.Equals(s.Trim(), (target ?? "").Trim(), StringComparison.OrdinalIgnoreCase);
    }

    // 読取った管理番号の一致する行データを返す
    //見つかったら管理番号の値（string）を返す。なければ null を返す
    private async Task<IDictionary<string, object>?> ITEM_STORAGE_ROW(string managementNo, bool forceReload = false)
    {
        managementNo = (managementNo ?? "").Trim();

        if (_itemStorageCache == null || forceReload)
        {
            ClassNameSelect select = new()
            {
                viewName = "ITEM_STORAGE",
            };
            _itemStorageCache = await ComService!.GetSelectData(select);
        }

        var datas = _itemStorageCache ?? new();

        var row = datas
            .Where(r => r.ContainsKey(COL_MNG_NO))
            .FirstOrDefault(r => EqualStr(r[COL_MNG_NO], managementNo));

        return row; // 見つからなければ null、見つかれば行をそのまま返す
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
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }
    #endregion

    private string? versionText { get; set; }
    public static string? userCode { get; set; }//他クラスでも使いたいのでnishiyama
    private string? userName { get; set; }
    private string? password { get; set; }
    public static string? LoginUserName { get; set; }//ログイン者を取得したいので
                                                     //他クラスでも使いたいのでnishiyama


    private string? AffiliationName { get; set; }
    private TYPE_NOTIFY_CATEGORY NotifyCategory { get; set; } = TYPE_NOTIFY_CATEGORY.NONE;

    private string? ArrivalIcon { get; set; }
    private string? ShipmentIcon { get; set; }
    private string? GetUnfinishedDataFlg { get; set; } = "false";//TODO 文字列判断はやめてboolにするべき
    private bool ArrivalIconVisible { get; set; } = false;
    private bool ShipmentIconVisible { get; set; } = false;
    private string? ArrivalStyle { get; set; }
    private string? ShipmentStyle { get; set; }

    private bool sidebarLeftExpanded = true;
    private readonly int notifyCount = 1;
    private LoginInfo[]? allUser;
    // 通知状態確認タイマー
    private System.Timers.Timer? timeLogNotify;
    // ログイン状態監視タイマー
    private System.Timers.Timer? timeLogin;
    // ログイン状態監視タイマー
    private System.Timers.Timer? timeUnfinished;
    private CustomLoginForm? loginForm { get; set; }
    private readonly System.Timers.Timer? timeHandyLogin;
    private readonly List<Task<bool>> DefineInitialTask = [];

    private string MainLayoutMenuFontSize { get; set; } = "140%";
    private string MainLayoutMenuFontWeight { get; set; } = "bold";
    private string MainLayoutAffiliationFontSize { get; set; } = "100%";
    private string MainLayoutAffiliationFontWeight { get; set; } = "normal";
    private string MainLayoutUserFontSize { get; set; } = "100%";
    private string MainLayoutUserFontWeight { get; set; } = "normal";

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
            allUser = await ComService.GetLoginInfoAsync();
        });

        //TODO userAgentを判断する文字列を取得する

        //TODO 多言語設定　一旦日本語
        //languageContainer.SetLanguage(System.Globalization.CultureInfo.GetCultureInfo("ja-JP"));

        // バージョンの表示
        versionText = applicationVersion!.Version;

        // 端末情報を取得する
        _js ??= await JS.CreateDymaicRuntimeAsync();
        dynamic window = _js.GetWindow();
        dynamic ua = window.navigator.userAgent;
        string strUa = (string)ua;
        await DeviceInfo.SetupDeviceInfo(strUa);
        //_ = WebComService.PostLogAsync($"DeviceInfo.DeviceGroupId:{DeviceInfo.DeviceGroupId}");

        // JavaScriptのイベントを対応付け
        DotNetObjectReference<MainLayout> dotNetReference = DotNetObjectReference.Create(this);
        await JS.InvokeVoidAsync("initializeEnterKeyPressListener", dotNetReference);

        // システムパラメータ取得してセッションストレージに保持する
        await SystemParamService.LoadSystemParameters();
        await SessionStorage.SetItemAsync(SharedConst.KEY_SYSTEM_PARAM, SystemParamService);
        // メインレイアウト関連情報はセットしなおす
        MainLayoutMenuFontSize = DeviceInfo.IsHandy() ? SystemParamService.MainLayoutMenuFontSizeHT : SystemParamService.MainLayoutMenuFontSizePC;
        MainLayoutMenuFontWeight = DeviceInfo.IsHandy() ? SystemParamService.MainLayoutMenuFontWeightHT : SystemParamService.MainLayoutMenuFontWeightPC;
        MainLayoutAffiliationFontSize = SystemParamService.MainLayoutAffiliationFontSizePC;
        MainLayoutAffiliationFontWeight = SystemParamService.MainLayoutAffiliationFontWeightPC;
        MainLayoutUserFontSize = DeviceInfo.IsHandy() ? SystemParamService.MainLayoutUserFontSizeHT : SystemParamService.MainLayoutUserFontSizePC;
        MainLayoutUserFontWeight = DeviceInfo.IsHandy() ? SystemParamService.MainLayoutUserFontWeightHT : SystemParamService.MainLayoutUserFontWeightPC;
        ChildBaseService.FontSizeValidationSammary = DeviceInfo.IsHandy() ? SystemParamService.HT_ValidationSummaryFontSize : SystemParamService.PC_ValidationSummaryFontSize;
        ChildBaseService.FontWeightValidationSammary = DeviceInfo.IsHandy() ? SystemParamService.HT_ValidationSummaryFontWeight : SystemParamService.PC_ValidationSummaryFontWeight;
        //入荷未完了、出荷未完了表示関連
        ArrivalIcon = SystemParamService.HT_GetUnfinishedArrivalIcon;
        ShipmentIcon = SystemParamService.HT_GetUnfinishedShipmentIcon;
        GetUnfinishedDataFlg = SystemParamService.HT_GetUnfinishedDataFlg;
        ArrivalStyle = $"background-color:{SystemParamService.HT_GetUnfinishedArrivalBackGroundColor};color:{SystemParamService.HT_GetUnfinishedArrivalColor}";
        ShipmentStyle = $"background-color:{SystemParamService.HT_GetUnfinishedShipmentBackGroundColor};color:{SystemParamService.HT_GetUnfinishedShipmentColor}";
        ArrivalIconVisible = false;
        ShipmentIconVisible = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        //await WebComService.PostLogAsync($"OnAfterRenderAsync IsHandy:{IsHandy}, IsLogin:{IsLogin}, firstRender:{firstRender}");

        if (DeviceInfo.IsHandy() && !IsLogin)
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

        if (DeviceInfo.IsHandy() && !IsLogin)
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
            _ = WebComService.PostLogAsync(ex.Message);
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

        //デバイスタイプが取得できていない場合は、ログオン時に再度取得を試みる
        if (DeviceInfo.DeviceType == SharedConst.TYPE_DEVICE_TYPE.NONE)
        {
            // 端末情報を取得する
            _js ??= await JS.CreateDymaicRuntimeAsync();
            dynamic window = _js.GetWindow();
            dynamic ua = window.navigator.userAgent;
            string strUa = (string)ua;
            await DeviceInfo.SetupDeviceInfo(strUa);
        }

        // ユーザ情報が取得出来ていなかった場合はログイン処理前に取得する
        if (allUser == null || allUser.Count() <= 0)
        {
            allUser = await ComService.GetLoginInfoAsync();
        }
        LoginInfo? login = allUser.FirstOrDefault(_ =>
            _.Id == args.Username
            && _.Password == args.Password
        );
        if (login is null)
        {
            // ユーザーが取得できなかったためNG
            ComService!.ShowNotifyMessege(ToastType.Error, "ログイン失敗", "ユーザー取得にに失敗しました。");
            return;
        }

        //以下、ログイン成功時の処理

        //ログイン処理に入ったなら、ローカルストレージの情報をすべてクリアしておく。PC,HTいずれも途中で何かの理由でブラウザが閉じられたことを考慮。
        await ComService.ClearAllLocalStorageValue();

        // ログイン情報取得し、セッションストレージへセット
        await SessionStorage.SetItemAsync(SharedConst.KEY_LOGIN_INFO, login);



     


        // 画面表示情報更新
        LoginUserName = login.UserName;
        AffiliationName = login.AffiliationName;

        // ユーザーが取得できたためLogin
        bool bBusyShow = false;

        try
        {
            _ = ComService.DialogShowBusy("取得中..");
            bBusyShow = true;
            DefineInitialTask.Clear();

            // システムパラメータ取得してローカルストレージに保持する
            await SystemParamService.LoadSystemParameters();
            await SessionStorage.SetItemAsync(SharedConst.KEY_SYSTEM_PARAM, SystemParamService);
            // メインレイアウト関連情報はセットしなおす
            MainLayoutMenuFontSize = DeviceInfo.IsHandy() ? SystemParamService.MainLayoutMenuFontSizeHT : SystemParamService.MainLayoutMenuFontSizePC;
            MainLayoutMenuFontWeight = DeviceInfo.IsHandy() ? SystemParamService.MainLayoutMenuFontWeightHT : SystemParamService.MainLayoutMenuFontWeightPC;
            MainLayoutAffiliationFontSize = SystemParamService.MainLayoutAffiliationFontSizePC;
            MainLayoutAffiliationFontWeight = SystemParamService.MainLayoutAffiliationFontWeightPC;
            MainLayoutUserFontSize = DeviceInfo.IsHandy() ? SystemParamService.MainLayoutUserFontSizeHT : SystemParamService.MainLayoutUserFontSizePC;
            MainLayoutUserFontWeight = DeviceInfo.IsHandy() ? SystemParamService.MainLayoutUserFontWeightHT : SystemParamService.MainLayoutUserFontWeightPC;
            ChildBaseService.FontSizeValidationSammary = DeviceInfo.IsHandy() ? SystemParamService.HT_ValidationSummaryFontSize : SystemParamService.PC_ValidationSummaryFontSize;
            ChildBaseService.FontWeightValidationSammary = DeviceInfo.IsHandy() ? SystemParamService.HT_ValidationSummaryFontWeight : SystemParamService.PC_ValidationSummaryFontWeight;

            // ログインストアド実行
            bool retb = await ComService.ExecLoginFunc(GetType().Name, applicationVersion!.Version);
            if (!retb)
            {
                ComService!.ShowNotifyMessege(ToastType.Error, "ログイン失敗", "ログインに失敗しました。");
                //後続は実行したくないため戻る
                return;
            }

            // PC画面間パラメータをリセット
            _ = ComService.ClearAllPCTransParam();

            // 画面毎の設定関連情報はログイン時に保持しておく
            // System.Type等の変数はJsonシリアライズ化で失敗しセッションストレージに保持できないため、
            // サービスに保持してその値を使用する（サービス変数はアプリが終了されるまで保持される）
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    // メニュー情報を取得してセッションストレージに保持する
                    List<MenuInfo> lstMenuInfo = await ComService.GetMenuInfoAllAsync(login.AuthorityLevel);
                    await SessionStorage.SetItemAsync(SharedConst.KEY_MENU_INFO, lstMenuInfo);
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync($"OnLogin_GetMenuInfoAllAsync_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    await ComService.SetPageTitleAsyncAll();
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync($"OnLogin_GetPageTitleAsyncAll_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    await ComService.SetComponetnsInfoAll();
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync($"OnLogin_GetComponetnsInfoAll_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    await ComService.SetGridColumnsDataAll();
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync($"OnLogin_GetGridColumnsDataAll_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    await ComService.SetComponentProgramInfoAll();
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync($"OnLogin_GetComponentProgramInfoAll_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    List<MstAreaData> MstAreaInfos = await ComService.GetArea();
                    ComService.MstAreaInfoAll = MstAreaInfos;
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync($"OnLogin_GetArea_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    List<MstShelf> MstShelfInfos = await ComService.GetShelf();
                    ComService.MstShelfAll = MstShelfInfos;
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync($"OnLogin_GetShelf_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    List<MstZoneData> MstZoneInfos = await ComService.GetZone();
                    ComService.MstZoneInfoAll = MstZoneInfos;
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync($"OnLogin_GetZone_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            DefineInitialTask.Add(Task.Run(async () =>
            {
                try
                {
                    List<MstLocationData> MstLocationInfos = await ComService.GetLocation();
                    ComService.MstLocationInfoAll = MstLocationInfos;
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync($"OnLogin_GetLocation_ex:{ex.Message}");
                    return false;
                }
                return true;
            }));
            // DEFINE関連データが取得できるまで待機
            _ = await Task.WhenAll(DefineInitialTask);

            _ = ComService.DialogClose();
            bBusyShow = false;

            // DefineInitialTask 内のタスクがすべて完了したかどうかを判断
            if (DefineInitialTask.All(_ =>
                _.IsCompletedSuccessfully == true
                && _.Result == true //他の条件はエラーが発生してもtrueとなってしまうので、タスクの戻り値を設けて確認するように修正。
                && _.IsFaulted == false
                && _.IsCanceled == false)
                )
            {
                _ = WebComService.PostLogAsync("ログインパラメータ取得_成功。");
                ComService!.ShowNotifyMessege(ToastType.Success, "ログイン成功", "ログインしました。");


                //nishiyama Add
                string managementNo = (LoginUserName ?? string.Empty).Trim();

                // 全件取得済みキャッシュから検索して棚を取得
                var row = await ITEM_STORAGE_ROW(managementNo);

                //もしそのデータがなければスルーする
                if (row != null)
                {

                    // 例：管理番号、棚、品名、数量、社員コードなどをモデルに詰める
                    string 保管終了日 = row.TryGetValue("保管終了日", out var mng)
                    ? (mng?.ToString() ?? "").Trim()
                    : "";


                    string 管理番号 = row.TryGetValue("管理番号", out var shelfVal)
                        ? (shelfVal?.ToString() ?? "").Trim()
                        : "";

                    string プロジェクト名 = row.TryGetValue("プロジェクト名", out var ship)
                        ? (ship?.ToString() ?? "").Trim()
                        : "";

                    string date = DateTime.Today.ToString("yyyy/MM/dd");

                   
                    DateTime 保管期間 = DateTime.Parse(保管終了日);
                    DateTime todayP = DateTime.Parse(date);

                    //保管期間が今日までなら
                    if (date == 保管終了日)
                    {
                        ComService!.ShowNotifyMessege(ToastType.Warning,"お知らせ",$"管理番号「{管理番号}」の物品が保管期間が今日までです。");
                    }
                    //保管期間が過ぎているなら
                    else if(保管期間 < todayP)
                    {
                        ComService!.ShowNotifyMessege(ToastType.Error, "お知らせ", $"管理番号「{管理番号}」の物品が保管期間が過ぎています。");
                    }


                }
            }
            else
            {
                _ = WebComService.PostLogAsync("ログインパラメータ取得_失敗。");
                ComService!.ShowNotifyMessege(ToastType.Error, "ログイン失敗", "パラメータ取得に失敗しました。");
                //後続は実行したくないため戻る
                return;
            }
            //ログイン成功とし、メイン画面を表示する
            IsLogin = true;

            if (DeviceInfo.IsHandy())
            {
                // HTスキャンイベント解除
                HtService.HtScanEvent -= HtService_HtScanEvent;
                // HT用メニュー画面に遷移
                await OnNavigateMobile();
                //未完了データ取得フラグを見てデータ取得を開始
                if (GetUnfinishedDataFlg == "true")
                {
                    //処理の起動有無を確認し、未完了データ取得処理を開始
                    StartUnfinishedDataTimer(SystemParamService.HT_GetUnfinishedDataInterval);
                }
            }
            else
            {
                // PCの場合は通知状態確認タイマーを起動する
                StartLogNotifyTimer(SystemParamService.LogNotifyInterval);
            }

            // 操作日時を更新
            await UpdateOperationDate();

            // 自動ログアウトモードに応じて、ログイン監視タイマースタート
            if (SystemParamService.AutoLogoutDeviceGroup == 999 ||
                (!DeviceInfo.IsHandy() && SystemParamService.AutoLogoutDeviceGroup == 1) ||
                (DeviceInfo.IsHandy() && SystemParamService.AutoLogoutDeviceGroup == 2))
            {
                StartLoginTimer(SystemParamService.AutoLogoutCheckInterval);
            }

        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
            ComService!.ShowNotifyMessege(ToastType.Error, $"ログイン失敗", "ログインに失敗しました。");
        }
        finally
        {
            if (bBusyShow)
            {
                _ = ComService.DialogClose();
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
                    bool? ret = await ComService.DialogShowYesNo("ログアウトします。よろしいですか。");
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
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    /// <summary>
    /// 通知一覧画面を表示
    /// </summary>
    /// <returns></returns>
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
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    /// <summary>
    /// ヘルプ画面の表示
    /// </summary>
    /// <returns></returns>
    private async Task OnClickHelpButton()
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
                    string strDialogTitle = "問い合わせ";
                    int intDialogWidth = 1600;
                    int intDialogHeight = 900;

                    // 通知画面に遷移
                    dynamic window = _js!.GetWindow();
                    int innerWidth = (int)window.innerWidth;
                    int innerHeight = (int)window.innerHeight;
                    _ = await DialogService.OpenAsync<DialogChatHelp>(
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
            _ = WebComService.PostLogAsync(ex.Message);
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
        //await WebComService.PostLogAsync($"OnNavigateMobile IsHandy:{IsHandy}, IsLogin:{IsLogin}");

        NavigationManager.NavigateTo("mobile_menu");
    }

    /// <summary>
    /// ヘッダートグルボタンクリック時の処理として定義
    /// </summary>
    private async void OnClickSidebarToggle()
    {
        if (DeviceInfo.IsHandy())
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
        IEnumerable<LoginInfo>? getUser = allUser?.Where(_ =>
            _.Id == args
        );
        string? userName = getUser is not null && getUser.Any() ? getUser.First().UserName : string.Empty;
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
            int pos = value.IndexOf(SystemParamService.LogonReadCodeChar);
            if (-1 != pos)
            {
                value = value[..pos];
                // ユーザ情報が取得出来ていなかった場合はログイン処理前に取得してユーザIDが一致するパスワードで自動ログインさせる
                if (allUser == null || allUser.Count() <= 0)
                {
                    allUser = await ComService.GetLoginInfoAsync();
                }
                IEnumerable<LoginInfo> usr = allUser.Where(_ => _.Id == value);
                if (usr.Any())
                {
                    await OnLogin(new LoginArgs { Username = value, Password = usr.First().Password });
                }
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
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
        await ComService.ExecLogoutFunc(GetType().Name, isCancelIgnore);

        // ログイン情報を削除する
        await SessionStorage.RemoveItemAsync(SharedConst.KEY_LOGIN_INFO);
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
            //SystemParameterService sysParams = await SessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
            notifyDuration = SystemParamService.NotifyPopupDuration;
        }

        // トップページに遷移する
        // ※ダイアログ表示中であっても全てのダイアログを閉じさせるため
        NavigationManager.NavigateTo("/");

        // 通知
        if (isAutoLogout)
        {
            ComService!.ShowNotifyMessege(ToastType.Success, "自動ログアウト", "操作しない状態が続いたためログアウトしました。");

        }
        else
        {
            ComService!.ShowNotifyMessege(ToastType.Success, $"ログアウト成功", "ログアウトしました。");
        }

        if (DeviceInfo.IsHandy())
        {
            // HTスキャンイベント登録
            HtService.HtScanEvent -= HtService_HtScanEvent;
            HtService.HtScanEvent += HtService_HtScanEvent;
            _ = htService!.UnLockScanner();
        }

        // 全ユーザ情報を取得する（ログインパスワードが変更されているまたは、作業者が追加されている場合があるため）
        allUser = await ComService.GetLoginInfoAsync();
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
        List<IDictionary<string, object>> datas = await ComService!.GetSelectData(select);
        if (null != datas && datas.Count > 0)
        {
            IDictionary<string, object> dic = datas.First();
            NotifyCategory = (TYPE_NOTIFY_CATEGORY)ConvertUtil.GetValueInt(dic, "通知状態");
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
                //SystemParameterService sysParams = await SessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);

                // 操作日時をセッションストレージから取出しチェック
                await SemaphoreOpeDate.WaitAsync();
                try
                {
                    if (await SessionStorage.ContainKeyAsync(SharedConst.KEY_OPERATION_DATE))
                    {
                        DateTime opeDate = await SessionStorage.GetItemAsync<DateTime>(SharedConst.KEY_OPERATION_DATE);
                        // 操作日時から自動ログアウト時間[分]経過していたらログアウト
                        if (opeDate.AddMinutes(SystemParamService.AutoLogoutTime) < DateTime.Now)
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
            _ = WebComService.PostLogAsync(ex.Message);
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
            _ = WebComService.PostLogAsync(ex.Message);
        }
        finally
        {
            _ = SemaphoreOpeDate.Release();
        }
    }

    /// <summary>
    /// ButtonFuncRadzenのF1ボタン処理呼出
    /// HTのバーコード読込時にファンクションボタンを処理する場合に使用
    /// 西山
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

    // <summary>
    /// ButtonFuncRadzenのF5ボタン処理呼出
    /// 棚卸する際読取をし自動的に処理してほしいので実装
    /// 西山
    /// </summary>
    public async Task ButtonClickF5()
    {
        if (buttonFunc == null)
        {
            return;
        }
        await buttonFunc.PublicButtonClickF5();
    }

    public async Task ButtonClickF6()
    {
        if (buttonFunc == null)
        {
            return;
        }
        await buttonFunc.PublicButtonClickF6();
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
        List<IDictionary<string, object>> datas = await ComService!.GetSelectData(select);
        foreach (IDictionary<string, object> data in datas)
        {
            if (ConvertUtil.GetValueString(data, "データ名称") == "入荷検品未完了データ数")
            {
                ArrivalIconVisible = ConvertUtil.GetValueInt(data, "未完了データ数") > 0;
            }
            else if (ConvertUtil.GetValueString(data, "データ名称") == "出荷未完了データ数")
            {
                ShipmentIconVisible = ConvertUtil.GetValueInt(data, "未完了データ数") > 0;
            }
        }
    }


}

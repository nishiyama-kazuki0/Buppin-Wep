using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Util;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedModels;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// HT用の通知ログ一覧ダイアログ
/// </summary>
public partial class DialogHtNotifyInfo : ChildPageBaseMobile
{
    private const string STR_INIT_FOCUS_MARK = "InitFocusMark";
    private const int DEFAULT_PAGE_SIZE = 5;
    private const int DEFAULT_READ_SIZE = 10;

    [Inject]
    private ISessionStorageService? _sessionStorage { get; set; }

    [Parameter]
    public string PageSize { get; set; } = DEFAULT_PAGE_SIZE.ToString();
    [Parameter]
    public string ReadSize { get; set; } = DEFAULT_READ_SIZE.ToString();

    /// <summary>
    /// DataCardMessageListコンポーネント
    /// </summary>
    protected DataCardMessageList? cardMessageList;
    /// <summary>
    /// カードデータ
    /// </summary>
    protected List<DataCardMessageListInfo>? _cardMessageValuesList { get; set; } = [];

    public new async ValueTask DisposeAsync()
    {
        await JS.InvokeVoidAsync("removeHtStepKeyListener");
        await base.DisposeAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(0);
        //必要であれば処理を記載
        // システム設定値を取得しておく
        //TODO できればストレージからパラメータの取得はやめる
        //_sysParams = await SessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
        if (null != _sysParams)
        {
            DialogShowYesNoWidth = _sysParams.DialogShowYesNoWidth;
            DialogShowYesNoHeight = _sysParams.DialogShowYesNoHeight;
        }

        //// 最終フォーカスID初期化
        //ChildBaseService.LastFocusId = string.Empty;

        _ = InvokeAsync(async () =>
        {
            // ページタイトル取得・更新
            pageName = await ComService.GetPageTitleAsync(ClassName);
            OnUpdateParentPageTitle(pageName);
        });

        _ = InvokeAsync(async () =>
        {
            //処理中ダイアログの表示を遅らせる。親ダイアログのリサイズをふせぐため
            await Task.Delay(500);
            //処理中ダイアログの表示
            _ = ComService.DialogShowBusy();
            //初期化中フラグON
            ChildBaseService.BasePageInitilizing = true;
            //TODO ログは役目が終わったら削除
            _ = WebComService!.PostLogAsync("コンポーネント情報初期化_開始");
            // コンポーネント情報初期化
            await InitComponentsAsync();
            //_ = WebComService!.PostLogAsync("グリッド初期化_開始");
            //// グリッド初期化
            //await InitDataGridAsync();
            //_ = WebComService!.PostLogAsync("検索条件初期化_開始");
            //// 検索条件初期化
            //await InitSearchConditionAsync();
            _ = WebComService!.PostLogAsync("プログラム情報初期化_開始");
            // プログラム情報初期化
            await InitProgramInfoAsync();
            _ = WebComService!.PostLogAsync("attributes情報初期化_開始");
            // attributes情報初期化
            await InitAttributesAsync();
            _ = WebComService!.PostLogAsync("MainLayoutのAttributesを更新して、通知イベントを発火する_開始");
            //MainLayoutのAttributesを更新して、通知イベントを発火する
            if (Attributes.ContainsKey(STR_ATTRIBUTE_FUNC))
            {
                OnUpdateParentAttributes(Attributes[STR_ATTRIBUTE_FUNC]);
            }
            else
            {
                // AttributeFuncButtonキーが登録されていない画面は空を渡す
                OnUpdateParentAttributes(new Dictionary<string, object>());
            }
            _ = WebComService!.PostLogAsync("ExecProgram実行_開始");
            await ExecProgram();
            //初期化中フラグOFF
            ChildBaseService.BasePageInitilizing = false;
            //Blazor へ状態変化を通知
            StateHasChanged();
            // 処理中ダイアログを閉じる
            _ = ComService.DialogClose();
        });
        //await base.OnInitializedAsync(); //ベースを実行するとなぜかうまくいかないので、コメント化
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

        await JS.InvokeVoidAsync("removeHtStepKeyListener");
        DotNetObjectReference<DialogHtNotifyInfo> dotNetReference = DotNetObjectReference.Create(this);
        await JS!.InvokeVoidAsync("initializeHtStepKeyListener", dotNetReference);

        // 初期フォーカスに合わせる
        SetElementIdFocus(STR_INIT_FOCUS_MARK);
    }

    public override async Task AfterInitializedAsync(ComponentProgramInfo info)
    {
        if (int.TryParse(PageSize, out int pageSize))
        {
            cardMessageList.PageSize = pageSize;
        }

        await ReadNotifyInfo();
    }

    [JSInvokable("OnKeyDownDataSelect")]
    public async void OnKeyDown(int keyCode)
    {
        if (keyCode is 37 or 39)
        {
            // ← →
            if (cardMessageList is not null)
            {
                _ = keyCode is 37 ? cardMessageList.BackPage() : cardMessageList.NextPage();
            }
        }
        await Task.Delay(0);
    }

    protected void SetElementIdFocus(string id)
    {
        dynamic window = _js!.GetWindow();
        dynamic element = window.document.getElementById(id);
        element?.focus(); // カーソルを合わせる
    }

    /// <summary>
    /// F4クリックイベント
    /// </summary>
    /// <param name="sender"></param>
    private void OnClickResultF4(object? sender)
    {
        try
        {
            // 閉じる
            DialogService.CloseSide();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    /// <summary>
    /// HtNotifyクリックイベント
    /// </summary>
    /// <param name="sender"></param>
    private void OnClickHtNotify(object? sender)
    {
        try
        {
            // 閉じる
            DialogService.CloseSide();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    /// <summary>
    /// 通知一覧の読込
    /// </summary>
    private async Task ReadNotifyInfo()
    {
        try
        {
            // 通知データ件数取得
            int readSize = DEFAULT_READ_SIZE;
            if (!int.TryParse(ReadSize, out readSize))
            {
                readSize = DEFAULT_READ_SIZE;
            }

            // 一覧データ初期化
            _cardMessageValuesList = [];

            // 一覧データ取得
            Dictionary<string, WhereParam> whereParam = [];
            if (await _sessionStorage!.ContainKeyAsync(SharedConst.KEY_LOGIN_INFO))
            {
                LoginInfo login = await _sessionStorage.GetItemAsync<LoginInfo>(SharedConst.KEY_LOGIN_INFO);
                if (login != null)
                {
                    whereParam.Add(SharedConst.KEY_USER_ID, new WhereParam { val = login.Id, whereType = enumWhereType.Equal });
                }
            }
            ClassNameSelect select = new()
            {
                viewName = "VW_HT_通知一覧",
                whereParam = whereParam,
                selectTopCnt = readSize,
            };
            select.orderByParam.Add(new OrderByParam { field = "SEQ", desc = true });
            List<IDictionary<string, object>> datas = await ComService!.GetSelectData(select);

            // 一覧データセット
            if (null != datas && datas.Count > 0)
            {
                foreach (IDictionary<string, object> rows in datas)
                {
                    DataCardMessageListInfo newCard = new();
                    foreach (KeyValuePair<string, object> row in rows)
                    {
                        if (row.Key == "発生日時")
                        {
                            newCard.OccurredTime = ConvertUtil.GetValueString(row.Value);
                        }
                        else if (row.Key == "区分")
                        {
                            newCard.ResultCategory = ConvertUtil.GetValueInt(row.Value);
                        }
                        else if (row.Key == "メッセージ")
                        {
                            newCard.Message = ConvertUtil.GetValueString(row.Value);
                        }

                    }
                    _cardMessageValuesList.Add(newCard);
                }
            }

            // 画面更新
            StateHasChanged();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }
}

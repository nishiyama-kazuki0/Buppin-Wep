using Blazor.DynamicJS;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SharedModels;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;

namespace ZennohBlazorShared.Shared
{
    public partial class ChildPageBase : ComponentBase, IAsyncDisposable
    {
        public const string STR_ATTRIBUTE_GRID = "AttributesGrid";
        public const string STR_ATTRIBUTE_SEARCH = "AttributesSearch";
        public const string STR_ATTRIBUTE_FUNC = "AttributesFuncButton";
        public const string STR_ATTRIBUTE_SEARCH_DATE_INIT_MODE = "AttributesSearchDateInitMode";
        public const string STR_ATTRIBUTE_EDIT_DATE_INIT_MODE = "AttributesEditDateInitMode";
        public const string STR_VIEW_MODEL_BIND = "ViewModelBind";
        public const string STR_LOCAL_STORAGE_SET = "LocalStorageSet";
        public const string STR_LOCAL_STORAGE_GET = "LocalStorageGet";
        public const string STR_WHERE_PARAM = "WhereParam";
        public const string STR_WHERE_PARAM_CONDITION = "WhereParamCondition";
        public const string STR_ORDERBY_PARAM = "OrderByParam";
        public const string STR_LISTCARD_HIDDEN = "ListCardHidden";
        public const string STR_LISTCARD_VIEW_MODEL_BIND = "ListCardViewModelBind";
        public const string STR_GRIDSELECT_VIEW_MODEL_BIND = "GridSelectViewModelBind";
        public const string STR_ARRIVAL_COMP_BUZZER_INFO = "ArrivalCompBuzzerInfo";
        public const string STR_MAIN_GRID_SETTINGS = "MainGridSettings";
        public const string STR_GRID_COLUMN_HIDDEN = "GridColumnHidden";

        //Blazor.DynamicJS
        [Inject]
        protected IJSRuntime? JS { get; set; }
        protected DynamicJSRuntime? _js;
        public ValueTask DisposeAsync()
        {
            Dispose();

            return _js?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
        [Inject]
        protected CommonService ComService { get; set; } = null!;

        /// <summary>
        /// DEFINE_COMPONENTSテーブル情報
        /// </summary>
        protected IList<ComponentsInfo> _componentsInfo { get; set; } = new List<ComponentsInfo>();

        /// <summary>
        /// DEFINE_COMPONENT_COLUMNSテーブル情報
        /// </summary>
        protected IList<ComponentColumnsInfo> _componentColumns { get; set; } = new List<ComponentColumnsInfo>();

        /// <summary>
        /// DEFINE_COMPONENT_PROGRAMSテーブル情報
        /// </summary>
        protected IList<ComponentProgramInfo> _componentProgram { get; set; } = new List<ComponentProgramInfo>();

        /// <summary>
        /// グリッド本体
        /// </summary>
        protected DataGridExtend2? _gridObject { get; set; } = null;

        /// <summary>
        /// グリッドカラム定義
        /// </summary>
        protected IList<ComponentColumnsInfo> _gridColumns { get; set; } = new List<ComponentColumnsInfo>();

        /// <summary>
        /// グリッドデータ
        /// </summary>
        protected List<IDictionary<string, object>> _gridData { get; set; } = new List<IDictionary<string, object>>();

        /// <summary>
        /// グリッド選択データ
        /// </summary>
        protected IList<IDictionary<string, object>>? _gridSelectedData { get; set; }

        /// <summary>
        /// 検索条件コンポーネント情報
        /// </summary>
        protected List<List<CompItemInfo>> _searchCompItems { get; set; } = new();

        /// <summary>
        /// Attributes情報
        /// </summary>
        protected IDictionary<string, IDictionary<string, object>> Attributes { get; set; } = new Dictionary<string, IDictionary<string, object>>();

        /// <summary>
        /// Formコンポーネント
        /// </summary>
        protected EditForm? editForm;

        /// <summary>
        /// 各Bodyページで取得したページタイトルの名称
        /// </summary>
        protected string pageName { get; set; } = string.Empty;

        /// <summary>
        /// クラス名
        /// </summary>
        public string ClassName => GetType().Name;

        /// <summary>
        /// ストアド呼出処理で使用するデータ
        /// </summary>
        protected Dictionary<string, object>? _storedData = null;

        /// <summary>
        /// ストアド呼出処理で使用するデータ
        /// </summary>
        protected List<Dictionary<string, object>>? _storedTableData = null;

        /// <summary>
        /// 連携ストレージデータ
        /// </summary>
        protected Dictionary<string, object> _streageData = new();

        /// <summary>
        /// MainLayoutのオブジェクト
        /// CascadingValue this で流れ込んでくる
        /// MainLayoutの変数を操作したいときなどに使用する
        /// </summary>
        [CascadingParameter]
        public MainLayout? ContainerMainLayout { get; set; }

        /// <summary>
        /// DEFINE_MENUを取得するためのパラメータ
        /// ※ページタイトル等の取得に必要
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery]
        public string MenuId { get; set; } = string.Empty;

        /// <summary>
        /// システム設定
        /// </summary>
        protected SystemParameter? _sysParams { get; set; }


        protected string UploadFilePath { get; set; } = string.Empty;

        /// <summary>
        /// 初期化後検索(AfterInitializedSearchAsync)を実行するかどうかのフラグ
        /// </summary>
        protected bool _isAfterInitializedSearch = false;

        //確認メッセージダイアログの幅,高さ（YesとNo）
        public int DialogShowYesNoWidth { get; set; } = 350;
        public int DialogShowYesNoHeight { get; set; } = 200;

        #region Event 

        /// <summary>
        /// コンポーネントにパラメーターが設定されるときに呼び出されます。非同期で実行できます。
        /// 子ページで全体で使用したい処理を記載
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override Task SetParametersAsync(Microsoft.AspNetCore.Components.ParameterView parameters)
        {
            //必要であれば処理を記載

            return base.SetParametersAsync(parameters);
        }
        /// <summary>
        /// ShouldRender:onAfterRenderの前にレンダリングして良いか判断するBlazor基底のライフサイクルコンポーネント
        /// true:レンダリング可能,false:レンダリング抑制
        /// </summary>
        /// <returns>true:レンダリング可能,false:レンダリング抑制</returns>
        protected override bool ShouldRender()
        {
            return !ChildBaseService.BasePageInitilizing;
        }

        /// <summary>
        /// コンポーネントが初期化されるときに呼び出されます。非同期で実行できるものはAsyncを付けます。
        /// 子ページで全体で使用したい処理を記載
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            //必要であれば処理を記載
            // システム設定値を取得しておく
            _sysParams = await SessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
            if (null != _sysParams)
            {
                DialogShowYesNoWidth = _sysParams.DialogShowYesNoWidth;
                DialogShowYesNoHeight = _sysParams.DialogShowYesNoHeight;
            }

            // 最終フォーカスID初期化
            ChildBaseService.LastFocusId = string.Empty;

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
                _ = ComService!.PostLogAsync("コンポーネント情報初期化_開始");
                // コンポーネント情報初期化
                await InitComponentsAsync();
                _ = ComService!.PostLogAsync("グリッド初期化_開始");
                // グリッド初期化
                await InitDataGridAsync();
                _ = ComService!.PostLogAsync("検索条件初期化_開始");
                // 検索条件初期化
                await InitSearchConditionAsync();
                _ = ComService!.PostLogAsync("プログラム情報初期化_開始");
                // プログラム情報初期化
                await InitProgramInfoAsync();
                _ = ComService!.PostLogAsync("attributes情報初期化_開始");
                // attributes情報初期化
                await InitAttributesAsync();
                _ = ComService!.PostLogAsync("MainLayoutのAttributesを更新して、通知イベントを発火する_開始");
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
                _ = ComService!.PostLogAsync("ExecProgram実行_開始");
                await ExecProgram();
                //初期化中フラグOFF
                ChildBaseService.BasePageInitilizing = false;
                //Blazor へ状態変化を通知
                StateHasChanged();
                // 処理中ダイアログを閉じる
                _ = ComService.DialogClose();
            });
            await base.OnInitializedAsync();
        }
        /// <summary>
        /// コンポーネントが初期化されるときに呼び出されます。
        /// 子ページで全体で使用したい処理を記載
        /// </summary>
        protected override void OnInitialized()
        {
            //必要であれば処理を記載

            //イベント削除
            ChildBaseService.EventMainLayoutF1 -= OnClickResultF1;
            ChildBaseService.EventMainLayoutF2 -= OnClickResultF2;
            ChildBaseService.EventMainLayoutF3 -= OnClickResultF3;
            ChildBaseService.EventMainLayoutF4 -= OnClickResultF4;
            ChildBaseService.EventMainLayoutF5 -= OnClickResultF5;
            ChildBaseService.EventMainLayoutF6 -= OnClickResultF6;
            ChildBaseService.EventMainLayoutF7 -= OnClickResultF7;
            ChildBaseService.EventMainLayoutF8 -= OnClickResultF8;
            ChildBaseService.EventMainLayoutF9 -= OnClickResultF9;
            ChildBaseService.EventMainLayoutF10 -= OnClickResultF10;
            ChildBaseService.EventMainLayoutF11 -= OnClickResultF11;
            ChildBaseService.EventMainLayoutF12 -= OnClickResultF12;
            ChildBaseService.EventMainLayoutHtNotify -= OnClickHtNotify;
            ChildBaseService.EventMainLayoutHtHomeNavigate -= OnClickHtHomeNavigate;
            ChildBaseService.EventMainLayoutPageUp -= OnClickPageUp;
            ChildBaseService.EventMainLayoutPageDown -= OnClickPageDown;
            NavigationManager.LocationChanged -= HandleLocationChanged!;

            //イベント追加
            ChildBaseService.EventMainLayoutF1 += OnClickResultF1;
            ChildBaseService.EventMainLayoutF2 += OnClickResultF2;
            ChildBaseService.EventMainLayoutF3 += OnClickResultF3;
            ChildBaseService.EventMainLayoutF4 += OnClickResultF4;
            ChildBaseService.EventMainLayoutF5 += OnClickResultF5;
            ChildBaseService.EventMainLayoutF6 += OnClickResultF6;
            ChildBaseService.EventMainLayoutF7 += OnClickResultF7;
            ChildBaseService.EventMainLayoutF8 += OnClickResultF8;
            ChildBaseService.EventMainLayoutF9 += OnClickResultF9;
            ChildBaseService.EventMainLayoutF10 += OnClickResultF10;
            ChildBaseService.EventMainLayoutF11 += OnClickResultF11;
            ChildBaseService.EventMainLayoutF12 += OnClickResultF12;
            ChildBaseService.EventMainLayoutHtNotify += OnClickHtNotify;
            ChildBaseService.EventMainLayoutHtHomeNavigate += OnClickHtHomeNavigate;
            ChildBaseService.EventMainLayoutPageUp += OnClickPageUp;
            ChildBaseService.EventMainLayoutPageDown += OnClickPageDown;
            NavigationManager.LocationChanged += HandleLocationChanged!;

            base.OnInitialized();
        }

        /// <summary>
        /// コンポーネントにパラメーターが設定された後に呼び出されます。非同期で実行できるものはAsyncを付けます。
        /// 子ページで全体で使用したい処理を記載
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            //必要であれば処理を記載

            await base.OnParametersSetAsync();
        }

        /// <summary>
        /// コンポーネントがレンダリングされた後に呼び出されます。非同期で実行できるものはAsyncを付けます。
        /// 子ページで全体で使用したい処理を記載
        /// </summary>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {
                return;
            }

            _js = await JS!.CreateDymaicRuntimeAsync();

        }
        /// <summary>
        /// コンポーネントがレンダリングされた後に呼び出されます。
        /// 子ページで全体で使用したい処理を記載
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (!firstRender)
            {
                return;
            }

        }

        #endregion

        #region Method

        /// <summary>
        /// ボディページからログアウト用を行う。
        /// Htの画面で使用する
        /// </summary>
        protected async Task OnLogout()
        {
            // ログアウト処理
            await ContainerMainLayout!.ProcLogout();
        }

        /// <summary>
        /// MainLayoutのAttributesを更新する
        /// </summary>
        /// <param name="atb"></param>
        protected void OnUpdateParentAttributes(IDictionary<string, object>? atb)
        {
            if (atb is null || ContainerMainLayout is null)
            {
                return;
            }

            if (atb.ContainsKey("IsHandy"))
            {
                atb["IsHandy"] = ContainerMainLayout.IsHandy;
            }
            else
            {
                atb.Add("IsHandy", ContainerMainLayout.IsHandy);
            }

            ContainerMainLayout.AttributesFuncButton = atb;
            ChildBaseService.EventCangeChildAsync(null!);
        }
        /// <summary>
        /// MainLayoutのPageNameを更新する
        /// </summary>
        /// <param name="pageName"></param>
        protected void OnUpdateParentPageTitle(string pageName)
        {
            if (ContainerMainLayout is null)
            {
                return;
            }
            ContainerMainLayout.PageName = pageName;
            ChildBaseService.EventCangeChildAsync(null!);
        }

        /// <summary>
        /// URLが変更された時の処理
        /// NavigationManager.LocationChangedイベントに紐付ける
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleLocationChanged(object sender, LocationChangedEventArgs e)
        {
            // 画面が切り替わった時の処理をここに実装する

        }

        /// <summary>
        /// コンポーネント情報初期化
        /// </summary>
        /// <returns></returns>
        protected virtual async Task InitComponentsAsync()
        {
            // DEFINE_COMPONENTSテーブル情報取得
            _componentsInfo = await ComService!.GetComponetnsInfo(GetType().Name);
        }

        /// <summary>
        /// グリッド初期化
        /// </summary>
        /// <returns></returns>
        protected virtual async Task InitDataGridAsync()
        {
            // カラム設定情報取得
            _componentColumns = await ComService!.GetGridColumnsData(GetType().Name);

            _gridColumns = _componentColumns.Where(_ => _.ComponentName == STR_ATTRIBUTE_GRID).ToList();
        }

        /// <summary>
        /// 検索条件初期化
        /// </summary>
        protected virtual async Task InitSearchConditionAsync()
        {
            // カラム設定データから検索条件のみを抽出し、並び変える
            List<ComponentColumnsInfo> listInfo = _gridColumns
                .Where(_ => _.IsSearchCondition == true)
                .OrderBy(_ => _.SearchLayoutGroup)
                .ThenBy(_ => _.SearchLayoutDispOrder)
                .ToList();

            // 検索条件コンポーネント情報を作成
            _searchCompItems = await ComService.GetCompItemInfo(listInfo, new(), _componentColumns, _componentsInfo);
        }

        /// <summary>
        /// attributes情報初期化
        /// </summary>
        /// <returns></returns>
        protected virtual async Task InitAttributesAsync()
        {
            await Task.Delay(0);

            // Attributesクリア
            Attributes.Clear();

            // コンポーネントの種類を追加
            _componentsInfo.GroupBy(_ => _.ComponentName).ToList().ForEach(group =>
            {
                Attributes.Add(group.Key, new Dictionary<string, object>());
            });

            // 各コンポーネントのAttributesを設定
            for (int i = 0; _componentsInfo.Count > i; i++)
            {
                try
                {
                    IDictionary attribute = (IDictionary)Attributes[_componentsInfo[i].ComponentName];
                    if (attribute != null)
                    {
                        object? value = null;
                        switch (_componentsInfo[i].ValueObjectType)
                        {
                            case (int)ComponentsInfo.EnumValueObjectType.ValueIndicator:
                                // 値をデータ型より変換
                                value = Convert.ChangeType(_componentsInfo[i].Value, _componentsInfo[i].Type);
                                break;
                            case (int)ComponentsInfo.EnumValueObjectType.VariableIndicator:
                                // 変数文字列から変数を取得
                                Type type = _componentsInfo[i].Type;
                                if (null == type)
                                {
                                    type = typeof(ChildPageBase);
                                }
                                PropertyInfo? pi = type.GetProperty(_componentsInfo[i].Value, BindingFlags.NonPublic | BindingFlags.Instance);
                                if (pi != null)
                                {
                                    value = pi.GetValue(this, null);
                                }
                                break;
                            case (int)ComponentsInfo.EnumValueObjectType.EnumStringIndicator:
                                // Enumを文字列から値に変換
                                string strEnumStr = _componentsInfo[i].Value;
                                string strEnumStrPos = strEnumStr[(strEnumStr.LastIndexOf('.') + 1)..];
                                value = typeof(CommonService).GetMethod("GetEnumValue")!.MakeGenericMethod(_componentsInfo[i].Type).Invoke(null, new object[] { strEnumStrPos });
                                break;
                            case (int)ComponentsInfo.EnumValueObjectType.ClassNameIndicator:
                                //クラス名タイプ？ダイアログのクラス名の判断に使用している
                                value = _componentsInfo[i].Type;
                                break;
                        }
                        if (value != null)
                        {
                            attribute.Add(_componentsInfo[i].AttributesName, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = ComService.PostLogAsync(ex.Message);
                }
            }
        }

        /// <summary>
        /// プログラム情報初期化
        /// </summary>
        /// <returns></returns>
        protected virtual async Task InitProgramInfoAsync()
        {
            // プログラム情報取得
            _componentProgram = await ComService!.GetComponentProgramInfo(GetType().Name);
        }

        /// <summary>
        /// 検索条件リセット
        /// </summary>
        protected void ResetSearchCondition()
        {
            foreach (List<CompItemInfo> listItem in _searchCompItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    if (item.CompObj?.Instance is CompBase comp)
                    {
                        comp.ResetValue();
                    }
                }
            }
        }

        /// <summary>
        /// 検索条件クリア
        /// </summary>
        protected void ClearSearchCondition()
        {
            foreach (List<CompItemInfo> listItem in _searchCompItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    if (item.CompObj?.Instance is CompBase comp)
                    {
                        comp.ClearValue();
                    }
                }
            }
        }

        /// <summary>
        /// Attributes情報の取得、キーが無い場合は空を戻す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected IDictionary<string, object> GetAttributes(string key)
        {
            return Attributes.ContainsKey(key) ? Attributes[key] : new Dictionary<string, object>();
        }

        #endregion

        #region virtual

        /// <summary>
        /// 終了処理
        /// </summary>
        protected virtual void Dispose()
        {
            //イベント削除

            ChildBaseService.EventMainLayoutF1 -= OnClickResultF1;
            ChildBaseService.EventMainLayoutF2 -= OnClickResultF2;
            ChildBaseService.EventMainLayoutF3 -= OnClickResultF3;
            ChildBaseService.EventMainLayoutF4 -= OnClickResultF4;
            ChildBaseService.EventMainLayoutF5 -= OnClickResultF5;
            ChildBaseService.EventMainLayoutF6 -= OnClickResultF6;
            ChildBaseService.EventMainLayoutF7 -= OnClickResultF7;
            ChildBaseService.EventMainLayoutF8 -= OnClickResultF8;
            ChildBaseService.EventMainLayoutF9 -= OnClickResultF9;
            ChildBaseService.EventMainLayoutF10 -= OnClickResultF10;
            ChildBaseService.EventMainLayoutF11 -= OnClickResultF11;
            ChildBaseService.EventMainLayoutF12 -= OnClickResultF12;
            ChildBaseService.EventMainLayoutHtNotify -= OnClickHtNotify;
            ChildBaseService.EventMainLayoutHtHomeNavigate -= OnClickHtHomeNavigate;
            ChildBaseService.EventMainLayoutPageUp -= OnClickPageUp;
            ChildBaseService.EventMainLayoutPageDown -= OnClickPageDown;
            NavigationManager.LocationChanged -= HandleLocationChanged!;
        }

        /// <summary>
        /// FunctionButton1が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF1(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton2が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF2(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton3が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF3(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton4が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF4(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton5が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF5(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton6が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF6(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton7が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF7(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton8が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF8(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton9が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF9(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton10が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF10(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton11が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF11(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// FunctionButton12が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickResultF12(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする

            await ExecProgram();
        }
        /// <summary>
        /// Htの通知が押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickHtNotify(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする
        }
        /// <summary>
        ///  Htのホームボタンが押下された時の処理(dotボタンに割り当て)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickHtHomeNavigate(object? sender, object? e)
        {
            await Task.Delay(0);
            //継承画面でオーバーライドする
        }
        /// <summary>
        /// PageUpが押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickPageUp(object? sender, object? e)
        {
            //await Task.Delay(0);
            //継承画面でオーバーライドする

            //await ExecProgram();
        }
        /// PageDownが押下された時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected virtual async Task OnClickPageDown(object? sender, object? e)
        {
            //await Task.Delay(0);
            //継承画面でオーバーライドする

            //await ExecProgram();
        }
        #endregion

        /// <summary>
        /// MainLayoutのAttributesを更新する
        /// </summary>
        /// <param name="atb"></param>
        public void UpdateFuncButton(IDictionary<string, object> atb)
        {
            Attributes[STR_ATTRIBUTE_FUNC] = atb;
            OnUpdateParentAttributes(Attributes[STR_ATTRIBUTE_FUNC]);
        }

        /// <summary>
        /// 画面名取得
        /// </summary>
        /// <returns></returns>
        public string GetPageName()
        {
            return pageName;
        }

        /// <summary>
        /// グリッドに表示するデータを取得しグリッドにセットする
        /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
        /// </summary>
        /// <returns></returns>
        public virtual async Task RefreshGridData(Dictionary<string, WhereParam> whereParam, string strViewName = "", string attributeName = STR_ATTRIBUTE_GRID, bool bInitSelect = false)
        {
            await RefreshGridDataInitSel(whereParam, strViewName, attributeName, bInitSelect);
        }

        /// <summary>
        /// グリッドに表示するデータを取得しグリッドにセットする
        /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
        /// 
        /// 初期選択データ指定用
        /// </summary>
        /// <returns></returns>
        public virtual async Task RefreshGridDataInitSel(Dictionary<string, WhereParam> whereParam, string strViewName = "", string attributeName = STR_ATTRIBUTE_GRID, bool bInitSelect = false, string strInitSelectKey = "", string strInitSelectVal = "")
        {
            try
            {
                // グリッドクリア
                _ = Attributes[attributeName]["Data"] = _gridData = new List<IDictionary<string, object>>();

                // ViewNameが指定されている場合はView名で値を取得
                ClassNameSelect select = new()
                {
                    className = string.IsNullOrEmpty(strViewName) ? GetType().Name : string.Empty,
                    viewName = string.IsNullOrEmpty(strViewName) ? string.Empty : strViewName,
                    whereParam = whereParam,
                    orderByParam = OrderByParamGet()
                };
                _gridData = await ComService!.GetSelectGridData(_gridColumns, select);
                _ = Attributes[attributeName]["Data"] = _gridData;

                if (bInitSelect)
                {
                    if (string.IsNullOrEmpty(strInitSelectKey))
                    {
                        // データが存在すれば一件目を選択状態にする
                        _gridSelectedData = _gridData.Count > 0
                            ? new List<IDictionary<string, object>>
                            {
                        _gridData[0]
                            }
                            : (IList<IDictionary<string, object>>?)null;
                    }
                    else
                    {
                        IEnumerable<IDictionary<string, object>> data = _gridData.Where(dict => dict.ContainsKey(strInitSelectKey) && dict[strInitSelectKey].ToString() == strInitSelectVal);
                        if (null != data && data.Any())
                        {
                            _gridSelectedData = new List<IDictionary<string, object>>
                            {
                                data.First()
                            };
                        }
                        else
                        {
                            // 指定したキー、値のデータが存在しない場合は１件目を設定する
                            _gridSelectedData = _gridData.Count > 0
                                ? new List<IDictionary<string, object>>
                                {
                            _gridData[0]
                                }
                                : (IList<IDictionary<string, object>>?)null;
                        }
                    }
                }
                else
                {
                    // 選択データクリア
                    _gridSelectedData = null;
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// DEFIME_COMPONENTSのCOMPONENT_NAMEが[OrderByParam]に設定されている項目名から
        /// OrderByParamを作成する
        /// 
        /// 【DEFIME_COMPONENTSテーブルの決めごと】
        /// ATTRIBUTES_NAMEはViewで取得される名称とする
        /// VALUEは順番(0～):asc/descとする(指定がない場合はasc)
        /// VALUE_OBJECT_TYPEは0:文字列とする
        /// 設定が10個を超える場合、VALUEの先頭は00～99とする
        /// </summary>
        /// <param name="componentName"></param>
        /// <returns></returns>
        protected List<OrderByParam> OrderByParamGet(string componentName = STR_ORDERBY_PARAM)
        {
            List<OrderByParam> orderbyParam = new();
            Dictionary<string, object> attributes = (Dictionary<string, object>)GetAttributes(componentName);
            foreach (KeyValuePair<string, object> att in attributes.OrderBy(_ => _.Value))
            {
                if (!string.IsNullOrEmpty(att.Value.ToString()))
                {
                    string val = att.Value.ToString()!;

                    // 順序指定があった場合の処理
                    bool bDesc = false;
                    string[] vals = val.Split(':');
                    if (vals.Length > 1)
                    {
                        if (vals[1].ToLower() == "desc")
                        {
                            bDesc = true;
                        }
                    }
                    OrderByParam param = new() { field = att.Key, desc = bDesc };

                    // 数値変換できなかったら後ろに追加
                    if (!int.TryParse(vals[0], out int pos))
                    {
                        orderbyParam.Add(param);
                    }
                    else
                    {
                        if (orderbyParam.Count > pos)
                        {
                            orderbyParam.Insert(pos, param);
                        }
                        else
                        {
                            orderbyParam.Add(param);
                        }
                    }
                }
            }
            return orderbyParam;
        }

        /// <summary>
        /// グリッドに表示するデータを取得しデータ数を戻す
        /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
        /// </summary>
        /// <returns></returns>
        protected async Task<int> GetDataCount(Dictionary<string, WhereParam> whereParam, string strViewName = "")
        {
            try
            {
                // ViewNameが指定されている場合はView名で値を取得
                ClassNameSelect select = new()
                {
                    className = string.IsNullOrEmpty(strViewName) ? GetType().Name : string.Empty,
                    viewName = string.IsNullOrEmpty(strViewName) ? string.Empty : strViewName,
                    whereParam = whereParam
                };

                List<IDictionary<string, object>> data = await ComService!.GetSelectData(select);
                return data.Count;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
                return 0;
            }
        }

        #region DEFINE_COMPONENT_PROGRAM関連

        /// <summary>
        /// DEFINE_COMPONENT_PROGRAMの設定でメソッドを順に呼び出す
        /// </summary>
        /// <param name="MethodName"></param>
        /// <returns></returns>
        public async Task ExecProgram([CallerMemberName] string MethodName = "")
        {
            try
            {
                //権限チェック
                if (!await CheckAuthorities(MethodName))
                {
                    return;
                }

                //ログを送信
                _ = ComService!.PostLogAsync("ExecProgram_Start");
                List<ComponentProgramInfo> programs = _componentProgram.Where(_ => _.CurrentMethodName == MethodName).OrderBy(_ => _.ExecOrderRank).ToList();
                foreach (ComponentProgramInfo? program in programs)
                {
                    object[] parameters = new object[] { program };
                    //ログを送信
                    _ = ComService!.PostLogAsync($"ExecProgram_Start_CurrentMethodName:[{program.CurrentMethodName}],program.CallMethodName[{program.CallMethodName}]");
                    MethodInfo? info = typeof(ChildPageBase).GetMethod(program.CallMethodName);
                    if (null != info)
                    {
                        // メソッドがAsyncかを判断してAsyncの場合はawaitする
                        bool isAsyncMethod = info.CustomAttributes.Any(attr => attr.AttributeType == typeof(AsyncStateMachineAttribute));
                        //ログを送信
                        _ = ComService!.PostLogAsync($"ExecProgram_awaitOrAsync_" +
                            $"program.IsProgramReturn[{program.IsProgramReturn}]" +
                            $"program.IsAsync[{program.IsAsync}]" +
                            $",CurrentMethodName:[{program.CurrentMethodName}]" +
                            $",program.CallMethodName[{program.CallMethodName}]");
                        if (0 != program.IsProgramReturn)
                        {
                            dynamic? value =
                                isAsyncMethod ?
                                    (program.IsAsync ?
                                    info.Invoke(this, parameters)
                                    : await (dynamic?)info.Invoke(this, parameters)
                                    )
                                : info.Invoke(this, parameters);

                            if (program.RetrunDataType == typeof(bool))
                            {
                                bool bValue = value is Task<bool> task ? await task : (bool)value;
                                //ログを送信
                                _ = ComService!.PostLogAsync($"ExecProgram_returnValue_" +
                                    $"bValue[{bValue}]" +
                                    $"program.IsProgramReturn[{program.IsProgramReturn}]" +
                                    $"program.IsAsync[{program.IsAsync}]" +
                                    $",CurrentMethodName:[{program.CurrentMethodName}]" +
                                    $",program.CallMethodName[{program.CallMethodName}]"
                                    );
                                if (false == bValue)
                                {
                                    // Falseの場合は、それ以降の処理は行わない
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (isAsyncMethod)
                            {
                                if (program.IsAsync)
                                {
                                    _ = (dynamic?)info.Invoke(this, parameters);
                                }
                                else
                                {
                                    await (dynamic?)info.Invoke(this, parameters);
                                }
                            }
                            else
                            {
                                _ = info.Invoke(this, parameters);
                            }
                        }
                    }
                    //ログを送信
                    _ = ComService!.PostLogAsync($"ExecProgram_End_CurrentMethodName:[{program.CurrentMethodName}],program.CallMethodName[{program.CallMethodName}]");
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            //ログを送信
            _ = ComService!.PostLogAsync("ExecProgram_Completed");
            return;
        }

        #region DEFINE_COMPONENT_PROGRAM呼び出しメソッド

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            await Task.Delay(0);
        }

        /// <summary>
        /// 初期化後検索
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task AfterInitializedSearchAsync(ComponentProgramInfo info)
        {
            if (_isAfterInitializedSearch)
            {
                // レンダリング抑制解除
                ChildBaseService.BasePageInitilizing = false;

                // Blazor へ状態変化を通知
                StateHasChanged();

                // グリッド更新
                ClassNameSelect custom = new();
                Dictionary<string, (object, WhereParam)> items = ComService.GetCompItemInfoValues(_searchCompItems);
                foreach (KeyValuePair<string, (object, WhereParam)> item in items)
                {
                    custom.whereParam.Add(item.Key, item.Value.Item2);
                }
                await RefreshGridData(custom.whereParam);
            }
        }

        /// <summary>
        /// 確認ダイアログ表示
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task<bool> DialogShowYesNo(ComponentProgramInfo info)
        {
            Dictionary<string, object> attr = new();
            string title = "確認";
            int width = DialogShowYesNoWidth;
            int height = DialogShowYesNoHeight;
            if (Attributes.ContainsKey(info.ComponentName))
            {
                attr = (Dictionary<string, object>)Attributes[info.ComponentName];
                if (attr.TryGetValue("Width", out object? value))
                {
                    width = int.Parse(value.ToString()!);
                }
                if (attr.TryGetValue("Height", out value))
                {
                    height = int.Parse(value.ToString()!);
                }
            }
            bool? ret = await ComService.DialogShowYesNo(attr, title, width, height);
            bool retb = ret is not null && (bool)ret;

            return retb;
        }

        /// <summary>
        /// 通知を表示
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task ShowNotify(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            string? title = pageName;
            Dictionary<string, object> prms = new(GetAttributes(info.ComponentName));
            if (prms.TryGetValue("NotifyTitle", out object? obj)) //TODO 固定は暫定。ゆくゆくは可変でとれるようにしたい
            {
                title = obj?.ToString();
            }
            string? message = string.Empty;
            if (prms.TryGetValue("NotifyMessage", out object? obj2)) //TODO 固定は暫定。ゆくゆくは可変でとれるようにしたい
            {
                message = obj2?.ToString();
            }

            NotificationSeverity sr = NotificationSeverity.Info;
            //TODO 一旦infoのみ。ゆくゆくはCOMPONENTSにNotificationSeverity用の文字列をセットして取得するとする
            //if (prms.TryGetValue("Severity", out object? obj)) 
            //{ 
            //    sr = ;
            //}
            if (!string.IsNullOrWhiteSpace(message))
            {
                ShowNotifyMessege(sr, title ?? string.Empty, message);
            }
        }

        /// <summary>
        /// ログアウト
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task<bool> ログアウト(ComponentProgramInfo info)
        {
            bool? ret = await ComService.DialogShowYesNo("ログアウトします。よろしいですか。");
            bool retb = ret is not null && (bool)ret;

            if (retb)
            {
                // ログアウト
                await DisposeAsync();
                await OnLogout();
            }

            return retb;
        }

        /// <summary>
        /// 画面遷移用のパラメータをセットする
        /// </summary>
        /// <param name="info"></param>
        public virtual async Task SetNavigateParam(ComponentProgramInfo info)
        {
            if (Attributes.ContainsKey(info.ComponentName))
            {
                // StorageのKeyを取得する
                string storageKey = string.Empty;
                foreach (KeyValuePair<string, object> keyval in Attributes[info.ComponentName])
                {
                    storageKey = keyval.Value.ToString()!;
                    break;
                }

                // StorageのKeyが有れば
                if (!string.IsNullOrEmpty(storageKey))
                {
                    // PC画面遷移用パラメータクリア
                    _ = ComService.ClearPCTransParam(storageKey);

                    // グリッドに選択行があれば、LocalStorageにセット
                    if (_gridSelectedData != null && _gridSelectedData?.Count() > 0)
                    {
                        //await LocalStorage.SetItemAsync<IDictionary<string, object>>(storageKey, _gridSelectedData[0]);
                        _ = ComService.SetPCTransParam(storageKey, _gridSelectedData[0]);
                    }
                }
            }
            await Task.Delay(0);
        }

        /// <summary>
        /// 画面遷移
        /// </summary>
        /// <param name="info"></param>
        public void NavigateTo(ComponentProgramInfo info)
        {
            _ = new Dictionary<string, object>();
            if (Attributes.ContainsKey(info.ComponentName))
            {
                foreach (KeyValuePair<string, object> nav in Attributes[info.ComponentName])
                {
                    NavigationManager?.NavigateTo(nav.Value.ToString()!);
                    break;
                }
            }
        }

        /// <summary>
        /// ブザー再生
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task ブザー再生(ComponentProgramInfo info)
        {
            // 
            await Task.Delay(0);
        }
        /// <summary>
        /// ブザー再生(エラーアラーム用)
        /// ChildBaseMobileでオーバーライドする
        /// </summary>
        /// <returns></returns>
        public virtual async Task ブザー再生_エラー()
        {
            // 
            await Task.Delay(0);
        }


        /// <summary>
        /// バイブレーション開始
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task バイブレーション開始(ComponentProgramInfo info)
        {
            // 
            await Task.Delay(0);
        }
        /// <summary>
        /// バイブレーション開始(エラーアラーム用)
        /// ChildBaseMobileでオーバーライドする
        /// </summary>
        /// <returns></returns>
        public virtual async Task バイブレーション開始_エラー()
        {
            // 
            await Task.Delay(0);
        }

        /// <summary>
        /// ブラウザの戻る処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task ブラウザ戻る(ComponentProgramInfo info)
        {
            if (JS is not null)
            {
                await JS.InvokeVoidAsync("history.back");
            }
        }

        /// <summary>
        /// バリデート
        /// </summary>
        /// <returns></returns>
        public virtual bool バリデートチェック(ComponentProgramInfo info)
        {
            if (editForm is null)
            {
                return true;
            }
            //EditContextのValidate()メソッドを実行することでSubmitと同等のイベントが発火
            return editForm.EditContext!.Validate();
        }

        /// <summary>
        /// グリッド更新
        /// </summary>
        public virtual async Task グリッド更新(ComponentProgramInfo info)
        {
            try
            {
                // レンダリング抑制解除
                ChildBaseService.BasePageInitilizing = false;

                //Blazor へ状態変化を通知
                StateHasChanged();

                ClassNameSelect custom = new();
                Dictionary<string, (object, WhereParam)> items = ComService.GetCompItemInfoValues(_searchCompItems);
                foreach (KeyValuePair<string, (object, WhereParam)> item in items)
                {
                    custom.whereParam.Add(item.Key, item.Value.Item2);
                }
                await RefreshGridData(custom.whereParam, attributeName: info.ComponentName);
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 画面クリア処理
        /// </summary>
        public virtual void 画面クリア(ComponentProgramInfo info)
        {
            // 検索条件クリア
            ClearSearchCondition();

            // グリッドクリア
            Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>();

            // 選択データクリア
            _gridSelectedData = null;
        }

        /// <summary>
        /// ファイルアップロード
        /// </summary>
        public virtual async Task<bool> ファイルアップロード(ComponentProgramInfo info)
        {
            Dictionary<string, object> dlgParam = new(GetAttributes(info.ComponentName));
            string dlgTitle = "ファイル取込";
            string strHeight = "300";

            // 以下はダイアログでは必要ないので取り出した後削除する
            if (dlgParam.TryGetValue("DialogTitle", out object? obj))
            {
                dlgTitle = obj.ToString()!;
                _ = dlgParam.Remove("DialogTitle");
            }
            if (dlgParam.TryGetValue("DialogHeight", out obj))
            {
                strHeight = obj.ToString()!;
                _ = dlgParam.Remove("DialogHeight");
            }

            // サイドダイアログを表示する際に処理中ダイアログが表示されているとバリデートがかからないため閉じる
            DialogService.Close();
            ContainerMainLayout!.SetIsBusyDialogClose(false);

            // ダイアログ表示
            UploadFilePath = await DialogService.OpenSideAsync<UploadDialog>(
                    dlgTitle,
                    dlgParam,
                    options: new SideDialogOptions { CloseDialogOnOverlayClick = false, Position = DialogPosition.Bottom, ShowMask = true, Height = $"{strHeight}px" }
            );

            return !string.IsNullOrEmpty(UploadFilePath);
        }

        /// <summary>
        /// 選択行チェック処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task<bool> 選択行チェック(ComponentProgramInfo info)
        {
            // チェック
            if (_gridSelectedData == null || _gridSelectedData?.Count() == 0)
            {
                // メッセージ取得
                string msg = "行が選択されていません。";
                if (Attributes.ContainsKey(info.ComponentName))
                {
                    if (Attributes[info.ComponentName].TryGetValue("MessageContent", out object? value))
                    {
                        msg = value.ToString()!;
                    }
                }
                // ダイアログ表示
                await ComService.DialogShowOK($"{msg}", pageName);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 確定前チェック処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする

            return true;
        }

        /// <summary>
        /// 確定前処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task 確定前処理(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }

        /// <summary>
        /// サイドダイアログ表示_データ編集
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> サイドダイアログ表示_データ編集(ComponentProgramInfo info)
        {
            try
            {
                // ダイアログパラメータ生成
                Dictionary<string, object> dlgParam = new(GetAttributes(info.ComponentName));

                // ダイアログタイトルを取得
                string strDialogTitle = string.Empty;
                Type? tDialogType = null;
                enumDialogMode dlgMode = enumDialogMode.Add;
                string strWidth = "500";
                if (dlgParam.TryGetValue("DialogTitle", out object? obj))
                {
                    strDialogTitle = obj.ToString()!;
                }
                if (dlgParam.TryGetValue("Mode", out obj))
                {
                    dlgMode = (enumDialogMode)obj;
                }

                // 以下はダイアログでは必要ないので取り出した後削除する
                if (dlgParam.TryGetValue("DialogWidth", out obj))
                {
                    strWidth = obj.ToString()!;
                    _ = dlgParam.Remove("DialogWidth");
                }
                if (dlgParam.TryGetValue("DialogType", out obj))
                {
                    tDialogType = (Type?)obj;
                    _ = dlgParam.Remove("DialogType");
                }

                // 編集モードの場合は選択行の一行目をセットする
                if (dlgMode == enumDialogMode.Edit)
                {
                    dlgParam["InitialData"] = (_gridSelectedData is not null && _gridSelectedData.Count > 0) ? _gridSelectedData[0] : new Dictionary<string, object>();
                }

                // サイドダイアログを表示する際に処理中ダイアログが表示されているとバリデートがかからないため閉じる
                DialogService.Close();
                ContainerMainLayout!.SetIsBusyDialogClose(false);

                // Type変換できなかった場合は基底クラス
                Type? componentType = tDialogType;
                componentType ??= typeof(DialogCommonInputContent);
                MethodInfo method = DialogService.GetType().GetMethod("OpenSideAsync")!;
                MethodInfo genericMethod = method.MakeGenericMethod(componentType);

                // ダイアログ表示
                dynamic retObj = await (Task<dynamic>)genericMethod.Invoke(DialogService, new object[] {
                    strDialogTitle!,
                    dlgParam,
                    new SideDialogOptions { Width = $"{strWidth}px", CloseDialogOnOverlayClick = false, Position = DialogPosition.Right, ShowMask = true }
                })!;

                // ダイアログからtrueが返った場合のみtrueを返す
                bool retVal = false;
                if (retObj is not null and bool)
                {
                    retVal = (bool)retObj;
                }

                return retVal;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// ダイアログ閉じる
        /// </summary>
        /// <returns></returns>
        public virtual async Task ダイアログ閉じる(ComponentProgramInfo info)
        {
            try
            {
                // 閉じる
                DialogService.Close();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            await Task.Delay(0);
        }

        /// <summary>
        /// ダイアログ閉じる_返却あり
        /// ※null以外の返却値をダイアログから受け取りたい場合に利用する
        /// </summary>
        /// <returns></returns>
        public virtual async Task ダイアログ閉じる_返却あり(ComponentProgramInfo info)
        {
            try
            {
                // 閉じる
                DialogService.Close(0);
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            await Task.Delay(0);
        }

        /// <summary>
        /// サイドダイアログ閉じる
        /// </summary>
        /// <returns></returns>
        public virtual async Task サイドダイアログ閉じる(ComponentProgramInfo info)
        {
            try
            {
                // 閉じる
                DialogService.CloseSide();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            await Task.Delay(0);
        }

        /// <summary>
        /// ストアドデータ設定_選択データ
        /// </summary>
        /// <returns></returns>
        public virtual async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
        {
            try
            {
                Dictionary<string, object> dlgParam = new(GetAttributes(info.ComponentName));

                _storedData = new Dictionary<string, object>();
                if (_gridSelectedData is not null)
                {
                    foreach (IDictionary<string, object> rows in _gridSelectedData)
                    {
                        foreach (KeyValuePair<string, object> data in rows)
                        {
                            if (null != _storedData)
                            {
                                _storedData[data.Key] = data.Value;
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            await Task.Delay(0);
        }

        /// <summary>
        /// ストアドデータ設定_引数データ作成_ファイル取込
        /// </summary>
        /// <returns></returns>
        public virtual async Task ストアドデータ設定_引数データ作成_ファイル取込(ComponentProgramInfo info)
        {
            try
            {
                Dictionary<string, object> dlgParam = new(GetAttributes(info.ComponentName));
                _storedData = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object> pam in dlgParam)
                {
                    _storedData[pam.Key] = pam.Key == "FilePath" ? UploadFilePath : pam.Value;
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            await Task.Delay(0);
        }

        /// <summary>
        /// ストアドデータ設定_テーブルデータ作成
        /// </summary>
        /// <returns></returns>
        public virtual async Task ストアドデータ設定_テーブルデータ作成(ComponentProgramInfo info)
        {
            try
            {
                _storedTableData = new List<Dictionary<string, object>>();
                if (_gridSelectedData is not null)
                {
                    foreach (IDictionary<string, object> rows in _gridSelectedData)
                    {
                        Dictionary<string, object> rowdata = new();
                        foreach (KeyValuePair<string, object> data in rows)
                        {
                            rowdata[data.Key] = data.Value;
                        }
                        _storedTableData.Add(rowdata);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            await Task.Delay(0);
        }

        /// <summary>
        /// ストアド呼び出し
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> ストアド呼び出し(ComponentProgramInfo info)
        {
            try
            {
                Dictionary<string, object> dlgParam = new(GetAttributes(info.ComponentName));

                // 結果変数
                bool retb = false;
                List<ExecResult> lstResult = new();
                int notifyDuration = _sysParams is null ? SharedConst.DEFAULT_NOTIFY_DURATION : _sysParams.NotifyPopupDuration;
                string strSummary = pageName.Replace("\\n", "");

                // ダイアログタイトルを取得
                string strProgramName = string.Empty;
                string strResultMessage = string.Empty;
                if (!string.IsNullOrEmpty(info.ProcessProgramName))
                {
                    strProgramName = info.ProcessProgramName;
                }
                else
                {
                    if (dlgParam.TryGetValue("ProgramName", out object? prg))
                    {
                        strProgramName = prg.ToString()!;
                    }
                }
                if (dlgParam.TryGetValue("ResultMessage", out object? obj))
                {
                    strResultMessage = obj.ToString()!;
                }
                if (!string.IsNullOrEmpty(strProgramName))
                {
                    // RequestValueにデータを作成する
                    RequestValue rv = RequestValue.CreateRequestProgram(strProgramName);
                    if (null != _storedData)
                    {
                        foreach (KeyValuePair<string, object> data in _storedData)
                        {
                            _ = rv.SetArgumentValue(data.Key, data.Value, "");
                        }
                    }
                    if (null != _storedTableData)
                    {
                        List<List<ArgumentValue>> argumentValues = new();
                        foreach (Dictionary<string, object> datas in _storedTableData)
                        {
                            List<ArgumentValue> argumentValue = new();
                            foreach (KeyValuePair<string, object> data in datas)
                            {
                                argumentValue.Add(ArgumentValue.CreateArgumentValue(data.Key, data.Value, ""));
                            }
                            argumentValues.Add(argumentValue);
                        }
                        _ = rv.SetArgumentDataset(string.Empty, argumentValues);
                    }

                    // WebAPIでストアド実行
                    ExecResult[]? results = await ComService.SetRequestValue(ClassName, rv, timeout: info.TimeoutValue);
                    if (results == null)
                    {
                        // 実行結果がnullの場合は異常
                        NotificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = $"{strSummary}",
                            Detail = "WebAPIへのアクセスが異常終了しました。電波強度を確認して下さい。",
                            Duration = notifyDuration
                        });
                        return false;
                    }
                    else
                    {
                        // 実行結果が返った場合
                        lstResult = new List<ExecResult>(results);
                    }
                }

                // 実行結果を異常・正常・確認に分ける (負数：以上 , 0:成功,1:確認メッセージ,2:警告メッセージ) //TODO enumなどで定義するべき
                List<ExecResult> lstError = lstResult.Where(_ => _.RetCode < 0).OrderBy(_ => _.ExecOrderRank).ToList();
                List<ExecResult> lstSuccess = lstResult.Where(_ => _.RetCode is 0 or 2).OrderBy(_ => _.ExecOrderRank).ToList();
                List<ExecResult> lstConfirm = lstResult.Where(_ => _.RetCode == 1).OrderBy(_ => _.ExecOrderRank).ToList();

                if (lstError.Count() > 0)
                {
                    // 異常結果がある場合
                    retb = false;
                    //Mobileでエラーアラーム用 //TODO ここでモバイル用かどうか判断したくないが暫定
                    if (ChildBaseService.IsHandy)
                    {
                        _ = ブザー再生_エラー();
                        _ = バイブレーション開始_エラー();
                    }
                    // 異常メッセージを全て通知
                    foreach (ExecResult result in lstError)
                    {
                        NotificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = $"{strSummary}",
                            Detail = result.Message,
                            Duration = notifyDuration
                        });
                    }
                }
                else
                {
                    // 異常結果が無い場合
                    retb = true;

                    // 正常結果のメッセージがある場合、全て通知
                    foreach (ExecResult result in lstSuccess)
                    {
                        if (!string.IsNullOrEmpty(result.Message))
                        {
                            NotificationService.Notify(new NotificationMessage()
                            {
                                Severity = result.RetCode == 0 ? NotificationSeverity.Success : NotificationSeverity.Warning,
                                Summary = $"{strSummary}",
                                Detail = result.Message,
                                Duration = notifyDuration
                            });
                        }
                    }

                    // 確認結果がある場合、全ての確認ダイアログ表示
                    foreach (ExecResult result in lstConfirm)
                    {
                        //TODO 警告のブザーが必要な場合などはメソッドを作ってここに記述する
                        bool? ret = await ComService.DialogShowYesNo(result.Message);
                        retb = ret is not null && (bool)ret;
                        if (!retb)
                        {
                            break;
                        }
                    }
                }

                // 正常終了で結果メッセージがある場合は通知
                if (retb)
                {
                    if (!string.IsNullOrEmpty(strResultMessage))
                    {
                        NotificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = $"{strSummary}",
                            Detail = strResultMessage,
                            Duration = notifyDuration
                        });
                    }
                }

                return retb;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 確定後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task 確定後処理(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }

        /// <summary>
        /// LocalStreageへデータ登録
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task 連携データ設定(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }

        /// <summary>
        /// LocalStreageからデータ取得
        /// 
        /// DEFIME_COMPONENTSのCOMPONENT_NAMEが[LocalStrageGet]に設定されている項目名から
        /// LocalStorageから値を取得し_streageDataに値をセットする
        /// 取得したキーは取得した時点で削除する
        /// 
        /// 【DEFIME_COMPONENTSテーブルの決めごと】
        /// ATTRIBUTES_NAMEは連携データ設定で作成した名称とする
        /// VALUEは使用しない
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual async Task 連携データ取得(ComponentProgramInfo info)
        {
            _streageData.Clear();
            Dictionary<string, object> attributes = (Dictionary<string, object>)GetAttributes(info.ComponentName);
            foreach (KeyValuePair<string, object> att in attributes)
            {
                string param = await LocalStorage.GetItemAsync<string>(att.Key.ToString());
                if (param != null)
                {
                    _streageData[att.Key] = param;
                }
                // 取得したキーはクリアする
                await LocalStorage.RemoveItemAsync(att.Key);
            }
        }

        public virtual async Task 前ステップへ(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }

        public virtual async Task 次ステップへ(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }

        public virtual async Task F1画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F2画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F3画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F4画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F5画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F6画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F7画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F8画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F9画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F10画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F11画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }
        public virtual async Task F12画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            //継承画面でオーバーライドする
        }

        public virtual async Task PageUp(ComponentProgramInfo info)
        {
            await Task.Delay(0);
        }
        public virtual async Task PageDown(ComponentProgramInfo info)
        {
            await Task.Delay(0);
        }
        public virtual async Task メソッド実行(ComponentProgramInfo info)
        {
            await Task.Delay(0);
        }

        public virtual async Task メソッド実行2(ComponentProgramInfo info)
        {
            await Task.Delay(0);
        }

        public virtual async Task メソッド実行3(ComponentProgramInfo info)
        {
            await Task.Delay(0);
        }
        #endregion

        #endregion

        /// <summary>
        /// Notifyでメッセージ表示
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="title"></param>
        /// <param name="message"></param>
        protected void ShowNotifyMessege(NotificationSeverity severity, string title, string message)
        {
            NotificationService.Notify(new NotificationMessage()
            {
                Severity = severity,
                Summary = $"{title}",
                Detail = message,
                Duration = _sysParams is null ? SharedConst.DEFAULT_NOTIFY_DURATION : _sysParams.NotifyPopupDuration
            });
        }

        /// <summary>
        /// 権限チェック(画面ボタンからの呼び出し用)
        /// </summary>
        /// <param name="MethodName"></param>
        /// <returns></returns>
        public virtual async Task<bool> CheckAuthorities([CallerMemberName] string MethodName = "")
        {
            try
            {
                LoginInfo login = new();
                if (await SessionStorage.ContainKeyAsync(SharedConst.KEY_LOGIN_INFO))
                {
                    login = await SessionStorage.GetItemAsync<LoginInfo>(SharedConst.KEY_LOGIN_INFO);
                }
                else
                {
                    return false;
                }
                if (_componentProgram.Any(_ => _.CurrentMethodName == MethodName
                                            && _.AuthorityLevelLower >= login.AuthorityLevel)
                )
                {
                    await ComService.DialogShowOK("権限制限がされているため実行できません。", pageName);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
                return false;
            }
        }
    }
}
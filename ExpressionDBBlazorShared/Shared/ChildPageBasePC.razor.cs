using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 子画面Bodyページに適用する親クラス。PC用。
/// PC側ページで共通にしたい処理を記述する
/// </summary>
public partial class ChildPageBasePC : ChildPageBase
{
    public const string STR_ATTRIBUTE_USER_SETTING = "AttributesUserSetting";
    public const string STR_ATTRIBUTE_USER_SETTING_NOT_SAVE_KEY = "AttributesUserSettingNotSaveKey";
    public const string STR_ATTRIBUTE_TRANS_PARAM_RCV = "AttriburesTransParamRcv";
    public const string STR_ATTRIBUTE_SEARCH_INITIAL_VALUE = "AttributesSearchInitialValue";

    // StepExtendコンポーネント（画面のrazorで@refで受け取る）
    protected TabsExtend? tabsExtend { get; set; } = null;

    // StepExtend用Attributes変数（画面のrazorで@attributesでで受け取る）
    protected IDictionary<string, object> TabsExtendAttributes { get; set; } = new Dictionary<string, object>();

    // ユーザー設定で
    // 条件保存を利用するかどうか
    protected bool UseSaveSearch = false;

    /// <summary>
    /// コンポーネントが初期化されるときに呼び出されます。非同期で実行できるものはAsyncを付けます。
    /// 子ページで全体で使用したい処理を記載
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        //必要であれば処理を記載

        //AttributesFuncButton?.Add("IsHandy", false);
        if (Attributes.ContainsKey(STR_ATTRIBUTE_FUNC))
        {
            Attributes[STR_ATTRIBUTE_FUNC].Add("IsHandy", false);
        }
        //if (ContainerMainLayout is not null)
        //{
        //    ContainerMainLayout.IsHandy = false;
        //}

        await base.OnInitializedAsync();
    }

    /// <summary>
    /// コンポーネントが初期化されるときに呼び出されます。
    /// 子ページで全体で使用したい処理を記載
    /// </summary>
    protected override void OnInitialized()
    {
        //イベント削除
        ChildBaseService.EventMainLayoutUserSetting -= OnClickUserSetting;

        //イベント追加
        ChildBaseService.EventMainLayoutUserSetting += OnClickUserSetting;

        base.OnInitialized();
    }

    /// <summary>
    /// 終了処理
    /// </summary>
    protected override void Dispose()
    {
        //イベント削除
        ChildBaseService.EventMainLayoutUserSetting -= OnClickUserSetting;

        base.Dispose();
    }

    /// <summary>
    /// グリッド初期化
    /// </summary>
    /// <returns></returns>
    protected override async Task InitDataGridAsync()
    {
        await base.InitDataGridAsync();

        // 検索条件保存機能を利用するかどうか取得
        UseSaveSearch = false;
        ComponentsInfo? compInfo = _componentsInfo.FirstOrDefault(_ => _.ComponentName == STR_ATTRIBUTE_USER_SETTING && _.AttributesName == "UseSaveSearch");
        if (compInfo != null && bool.TryParse(compInfo.Value, out bool result))
        {
            UseSaveSearch = result;
        }

        // ユーザ設定情報を取得し、グリッドカラムの表示・非表示を設定
        if (UseSaveSearch)
        {
            List<UserComponentSettingsInfo> lstUserInfo = await ComService.GetUserComponentSettingsInfoList(ClassName, STR_MAIN_GRID_SETTINGS, STR_GRID_COLUMN_HIDDEN);
            foreach (ComponentColumnsInfo colInfo in _gridColumns)
            {
                UserComponentSettingsInfo? userInfo = lstUserInfo.FirstOrDefault(_ => _.PropertyKey == colInfo.Property);
                colInfo.UserSetHidden = userInfo != null;
            }
        }
    }

    /// <summary>
    /// 検索条件初期化
    /// </summary>
    protected override async Task InitSearchConditionAsync()
    {
        // 画面遷移パラメータを取得する
        ComponentsInfo? infoKey = _componentsInfo.FirstOrDefault(_ => _.ComponentName == STR_ATTRIBUTE_TRANS_PARAM_RCV && _.AttributesName == "StorageKey");
        string storageKey = string.Empty;
        if (infoKey != null)
        {
            storageKey = infoKey.Value.ToString();
        }

        // 検索条件初期値を取得
        Dictionary<string, object>? InitialData = null;
        if (!string.IsNullOrEmpty(storageKey))
        {
            // PC画面遷移用パラメータ取得
            object? param = ComService.GetPCTransParam(storageKey);
            if (param != null)
            {
                InitialData = (Dictionary<string, object>)param;
            }
        }
        if (InitialData != null)
        {
            // 初期化後検索フラグを立てる
            _isAfterInitializedSearch = true;

            // PC画面遷移用パラメータクリア
            _ = ComService.ClearPCTransParam(storageKey);
        }
        else
        {
            InitialData = [];

            // LocalStorageからパラメータが取得出来なかった場合
            if (UseSaveSearch)
            {
                // 検索条件保存を利用する場合、検索条件初期値を取得
                InitialData = await ComService.GetUserComponentSettingsInfo(ClassName, STR_ATTRIBUTE_GRID);

                // 検索条件を保存しない項目名は除外する
                foreach (ComponentsInfo infoNotSave in _componentsInfo.Where(_ => _.ComponentName == STR_ATTRIBUTE_USER_SETTING_NOT_SAVE_KEY).ToList())
                {
                    if (bool.TryParse(infoNotSave.Value, out bool bnResult))
                    {
                        if (bnResult && InitialData.ContainsKey(infoNotSave.AttributesName))
                        {
                            _ = InitialData.Remove(infoNotSave.AttributesName);
                        }
                    }
                }
            }

            // 初期値がセットされていない場合、DEFINE_COMPONENTSの初期値を設定
            if (InitialData.Count == 0)
            {
                List<ComponentsInfo> lstInitInfo = _componentsInfo.Where(_ => _.ComponentName == STR_ATTRIBUTE_SEARCH_INITIAL_VALUE).ToList();
                foreach (ComponentsInfo initInfo in lstInitInfo)
                {
                    if (!string.IsNullOrEmpty(initInfo.Value))
                    {
                        // 【注意】複数の初期値を設定できるコンポーネントは、カンマ(,)区切りで初期値が登録されている前提です
                        string[] values = initInfo.Value.Split(',');
                        InitialData[initInfo.AttributesName] = values.Length > 1 ? new List<string>(values) : values[0];
                    }
                }
            }
        }

        // カラム設定データから検索条件のみを抽出し、並び変える
        List<ComponentColumnsInfo> listInfo = _gridColumns
            .Where(_ => _.IsSearchCondition == true)
            .OrderBy(_ => _.SearchLayoutGroup)
            .ThenBy(_ => _.SearchLayoutDispOrder)
            .ToList();

        // 検索条件コンポーネント情報を作成
        _searchCompItems = await ComService.GetCompItemInfo(listInfo, InitialData, _componentColumns, _componentsInfo);
    }

    /// <summary>
    /// ユーザー設定ボタンが押下された時の処理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    protected virtual async Task OnClickUserSetting(object? sender, object? e)
    {
        // バリデート
        if (editForm is not null)
        {
            if (false == editForm!.EditContext.Validate())
            {
                return;
            }
        }

        // 検索条件を保存しない項目名のComponentsInfoリスト取得
        List<ComponentsInfo> lstNotSave = [];
        foreach (ComponentsInfo infoNotSave in _componentsInfo.Where(_ => _.ComponentName == STR_ATTRIBUTE_USER_SETTING_NOT_SAVE_KEY).ToList())
        {
            if (bool.TryParse(infoNotSave.Value, out bool result))
            {
                if (result)
                {
                    lstNotSave.Add(infoNotSave);
                }
            }
        }

        // 入力中の検索条件を取得
        List<UserComponentSettingsInfo> settingsInfo = ComService.GetSettingsInfo(_searchCompItems, lstNotSave);

        // データグリッドの非表示に設定されたカラム情報を取得
        List<string> lstHiddenColName = [];
        if (_gridObject != null && _gridObject.MainGrid != null)
        {
            int valueKeyId = 1;
            foreach (Radzen.Blazor.RadzenDataGridColumn<IDictionary<string, object>>? col in _gridObject.MainGrid.ColumnsCollection)
            {
                if (col.Pickable == true && col.GetVisible() == false)
                {
                    UserComponentSettingsInfo info = new()
                    {
                        ComponentName = STR_MAIN_GRID_SETTINGS,
                        ViewName = STR_GRID_COLUMN_HIDDEN,
                        PropertyKey = col.Title,
                        ValueKeyId = valueKeyId
                    };
                    settingsInfo.Add(info);
                    valueKeyId++;
                }
            }
        }

        // ダイアログパラメータ生成
        Dictionary<string, object> dlgParam = new()
        {
            [DialogUserSetting.STR_IS_VISIBLE_SAVE_SEARCH] = UseSaveSearch,
            [DialogUserSetting.STR_SETTINGS_CLASS_NAME] = ClassName,
            [DialogUserSetting.STR_SETTINGS_INFO] = settingsInfo,
        };

        // ダイアログ情報を取得
        string strDialogTitle = "ユーザー設定";
        int intDialogWidth = 500;
        int intDialogHeight = UseSaveSearch ? 610 : 500;

        // ユーザー設定ダイアログ表示
        dynamic window = _js!.GetWindow();
        int innerWidth = (int)window.innerWidth;
        int innerHeight = (int)window.innerHeight;
        _ = await DialogService.OpenAsync<DialogUserSetting>(
            $"{strDialogTitle}",
            dlgParam,
            new DialogOptions()
            {
                Width = $"{Math.Min(intDialogWidth, innerWidth)}px",
                Height = $"{Math.Min(intDialogHeight, innerHeight)}px",
                Resizable = true,
                Draggable = true
            }
        );
    }
}

using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Shared;
using System.Runtime;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// バッチメンテナンス明細画面
/// </summary>
public partial class BatchMaintenanceDetail : ChildPageBasePC
{
    /// <summary>
    /// 検索条件初期化
    /// </summary>
    protected override async Task InitSearchConditionAsync()
    {
        // ベースとほぼ変わらないが、最後の「検索条件コンポーネント情報を作成」部分だけ変更している

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
        // 入力不可にしたいため、GetCompItemInfoのパラメータのIsEditをFalseにする
        _searchCompItems = await ComService.GetCompItemInfo(listInfo, InitialData, _componentColumns, _componentsInfo, true, true);
    }
}
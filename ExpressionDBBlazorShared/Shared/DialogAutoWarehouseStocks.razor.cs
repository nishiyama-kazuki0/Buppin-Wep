using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using SharedModels;
using System.Text;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 自動倉庫状況:在庫
/// </summary>
public partial class DialogAutoWarehouseStocks : ChildPageBasePC
{
    #region private

    // Dialogで表示するために必要なパラメータ
    #region [Parameter]定義

    /// <summary>
    /// モード
    /// </summary>
    [Parameter]
    public enumDialogMode Mode { get; set; } = 0;
    /// <summary>
    /// ダイアログタイトル
    /// </summary>
    [Parameter]
    public string DialogTitle { get; set; } = string.Empty;
    /// <summary>
    /// ラベル幅
    /// </summary>
    [Parameter]
    public string DialogLabelWidth { get; set; } = string.Empty;
    /// <summary>
    /// 情報エリア折り畳み
    /// </summary>
    [Parameter]
    public bool InfoAllowCollapse { get; set; } = false;
    /// <summary>
    /// 情報エリアタイトル
    /// </summary>
    [Parameter]
    public string InfoTitle { get; set; } = string.Empty;
    /// <summary>
    /// 情報エリアアイコン
    /// </summary>
    [Parameter]
    public string InfoIconName { get; set; } = string.Empty;
    /// <summary>
    /// 入力エリア折り畳み
    /// </summary>
    [Parameter]
    public bool InputAllowCollapse { get; set; } = false;
    /// <summary>
    /// 入力エリアタイトル
    /// </summary>
    [Parameter]
    public string InputTitle { get; set; } = string.Empty;
    /// <summary>
    /// 入力エリアアイコン
    /// </summary>
    [Parameter]
    public string InputIconName { get; set; } = string.Empty;
    /// <summary>
    /// 確定時のプログラム名
    /// </summary>
    [Parameter]
    public string ProgramName { get; set; } = string.Empty;
    /// <summary>
    /// DEFINE_COMPONENTSテーブル情報
    /// </summary>
    [Parameter]
    public IList<ComponentsInfo> ComponentsInfos { get; set; } = [];
    /// <summary>
    /// グリッドカラム定義
    /// </summary>
    [Parameter]
    public IList<ComponentColumnsInfo> Components { get; set; } = [];
    /// <summary>
    /// 初期データ
    /// </summary>
    [Parameter]
    public Dictionary<string, object> InitialData { get; set; } = [];
    /// <summary>
    /// セル選択時のカラム名
    /// </summary>
    [Parameter]
    public string? CellColumn { get; set; }
    #endregion

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

        // 初期化後検索フラグを立てる
        _isAfterInitializedSearch = true;

        // PC画面遷移用パラメータクリア
        _ = ComService.ClearPCTransParam(storageKey);

        // 棚一覧のLOCATION_IDを組み立てる
        // キー値を追加するため、本フォーム用のローカル変数を定義する
        // (InitialDataに追加すると元のデータにも反映されてしまう)
        Dictionary<string, object>? InitialDataSet = new(InitialData);
        if (InitialData != null ? InitialData.Count() > 0 : false)
        {
            StringBuilder sb = new();

            // 倉庫＋列＋連＋段
            object? tmp = null;
            if (InitialData.TryGetValue("倉庫", out tmp)) sb.Append((tmp?.ToString() ?? "").PadLeft(2, '0'));
            sb.Append("-");
            if (InitialData.TryGetValue("列", out tmp)) sb.Append((tmp?.ToString() ?? "").PadLeft(2, '0'));
            sb.Append("-");
            sb.Append((CellColumn?.ToString() ?? "").PadLeft(2, '0'));
            sb.Append("-");
            if (InitialData.TryGetValue("段/番地", out tmp)) sb.Append((tmp?.ToString() ?? "").PadLeft(2, '0'));

            InitialDataSet["ロケーション"] = sb.ToString();
        }

        // カラム設定データから検索条件のみを抽出し、並び変える
        List<ComponentColumnsInfo> listInfo = _gridColumns
            .Where(_ => _.IsSearchCondition == true)
            .OrderBy(_ => _.SearchLayoutGroup)
            .ThenBy(_ => _.SearchLayoutDispOrder)
            .ToList();

        // 検索条件コンポーネント情報を作成
        // 入力不可にしたいため、GetCompItemInfoのパラメータのIsEditをFalseにする
        _searchCompItems = await ComService.GetCompItemInfo(listInfo, InitialDataSet, _componentColumns, _componentsInfo, true, true);
    }

    /// <summary>
    /// F4クリックイベント
    /// </summary>
    /// <param name="sender"></param>
    private void OnClickResultF4(object? sender)
    {
        // 閉じる
        DialogService.CloseSide();
    }

    #endregion
}

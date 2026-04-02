using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Shared;

public partial class DropDwonDataGridExtend : ComponentBase
{
    /// <summary>
    /// グリッドに表示するデータ
    /// </summary>
    [Parameter]
    public List<IDictionary<string, object>> Data { get; set; } = [];
    /// <summary>
    /// グリッドに設定するカラムデータ
    /// </summary>
    [Parameter]
    public IDictionary<string, (Type, int, TextAlign)> Columns { get; set; } = new Dictionary<string, (Type, int, TextAlign)>();
    /// <summary>
    /// グリッドで選択しているデータ
    /// </summary>
    public IList<IDictionary<string, object>>? selectedData;

    [Parameter]
    public string TextProperty { get; set; } = string.Empty;
    [Parameter]
    public string ValueProperty { get; set; } = string.Empty;

    /// <summary>
    /// データ選択方式
    /// Single : 1行のみ選択
    /// Multiple : 複数行選択
    /// </summary>
    [Parameter]
    public DataGridSelectionMode SelectionMode { get; set; } = DataGridSelectionMode.Single;
    /// <summary>
    /// ページ分割有時のUI配置位置
    /// </summary>
    [Parameter]
    public HorizontalAlign PagerHorizontalAlign { get; set; } = HorizontalAlign.Justify;
    /// <summary>
    /// カラム表示選択コンボ有無
    /// </summary>
    [Parameter]
    public bool AllowColumnPicking { get; set; } = false;
    /// <summary>
    /// ページ分割で表示の有無
    /// </summary>
    [Parameter]
    public bool AllowPageing { get; set; } = true;
    /// <summary>
    /// カラム間のドラッグドロップ並び替え移動有無
    /// </summary>
    [Parameter]
    public bool AllowColumnReorder { get; set; } = true;
    /// <summary>
    /// カラム幅のドラッグサイズの有無
    /// </summary>
    [Parameter]
    public bool AllowColumnResize { get; set; } = true;
    /// <summary>
    /// ページ分割有時のページ別集計(1ページに何行)の表示有無
    /// </summary>
    [Parameter]
    public bool ShowPagingSummary { get; set; } = true;
    /// <summary>
    /// フィルター方法種類
    /// </summary>
    [Parameter]
    public FilterMode FilterMode { get; set; } = FilterMode.Advanced;
    /// <summary>
    /// ソート機能有無
    /// </summary>
    [Parameter]
    public bool AllowSorting { get; set; } = true;
    /// <summary>
    /// 複数カラムのソート機能有無
    /// </summary>
    [Parameter]
    public bool AllowMultiColumnSorting { get; set; } = true;
    /// <summary>
    /// データ出力有無
    /// </summary>
    [Parameter]
    public bool AllowDataExport { get; set; } = true;
    /// <summary>
    /// グリッド表示設定の表示有無
    /// </summary>
    [Parameter]
    public bool VisibleSettings { get; set; } = false;
    /// <summary>
    /// ページサイズ
    /// </summary>
    [Parameter]
    public int PageSize { get; set; } = 10;
    /// <summary>
    /// 密度
    /// </summary>
    [Parameter]
    public Density Density { get; set; } = Density.Default;

    private IDictionary<string, object>? AttributesStringText { get; set; } = new Dictionary<string, object>();

    //RadzenDataGrid<IDictionary<string, object>> grid;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? Attributes { get; set; }

    [Inject]
    private ISessionStorageService _sessionStorage { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        //日本語化。取り急ぎべたで対応
        AttributesStringText.Add("FilterText", "フィルター");
        AttributesStringText.Add("AndOperatorText", "かつ");
        AttributesStringText.Add("OrOperatorText", "または");
        AttributesStringText.Add("ApplyFilterText", "適用");
        AttributesStringText.Add("ClearFilterText", "クリア");
        AttributesStringText.Add("EqualsText", "等しい");
        AttributesStringText.Add("NotEqualsText", "等しくない");
        AttributesStringText.Add("LessThanText", "より小さい");
        AttributesStringText.Add("LessThanOrEqualsText", "以下");
        AttributesStringText.Add("GreaterThanText", "より大きい");
        AttributesStringText.Add("GreaterThanOrEqualsText", "以上");
        AttributesStringText.Add("EndsWithText", "後方一致");
        AttributesStringText.Add("ContainsText", "含む");
        AttributesStringText.Add("DoesNotContainText", "含まない");
        AttributesStringText.Add("StartsWithText", "前方一致");
        AttributesStringText.Add("IsNotNullText", "NULLではない");
        AttributesStringText.Add("IsNullText", "NULLである");
        AttributesStringText.Add("IsEmptyText", "空白");
        AttributesStringText.Add("IsNotEmptyText", "空白ではない");

        AttributesStringText.Add("ColumnsShowingText", "列表示");

        PageSize = SystemParamService.DataGridPageSize;

        await base.OnInitializedAsync();
    }

    protected void ChangeSelectCheckClick(bool args)
    {
    }

    public string GetColumnPropertyExpression(string name)
    {
        string expression = $@"it[""{name}""].ToString()";
        return expression;
    }
    public string GetColumnWidthExpression(int wid)
    {
        return $"{wid}px";
    }
}
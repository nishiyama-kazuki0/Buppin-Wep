using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Shared;

public partial class CompGroupFieldCustom
{
    [Inject]
    private ISessionStorageService _sessionStorage { get; set; } = null!;

    protected SystemParameterService _sysParam => SystemParamService;

    /// <summary>
    /// グループタイトル
    /// </summary>
    [Parameter]
    public string GroupTitle { get; set; } = string.Empty;

    /// <summary>
    /// アイコン名称（search,）
    /// </summary>
    [Parameter]
    public string IconName { get; set; } = string.Empty;

    /// <summary>
    /// Fieldsetの折り畳み（false:OFF、true:ON）
    /// </summary>
    [Parameter]
    public bool AllowCollapse { get; set; } = false;

    /// <summary>
    /// ラベル幅
    /// </summary>
    [Parameter]
    public string LabelWidth { get; set; } = "150px";

    /// <summary>
    /// 表示・非表示
    /// </summary>
    [Parameter]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// コンポーネント情報リスト
    /// </summary>
    [Parameter]
    public List<List<CompItemInfo>> CopmItems { get; set; } = [];

    /// <summary>
    /// 必須表示の付加文字
    /// </summary>
    [Parameter]
    public string RequiredDisplaySuffix { get; set; } = "＊";

    /// <summary>
    /// タイトルのフォントサイズ
    /// </summary>
    [Parameter]
    public string TitleFontSize { get; set; } = "100%";
    /// <summary>
    /// タイトルのフォント幅
    /// </summary>
    [Parameter]
    public string TitleFontWeight { get; set; } = "bold";
    /// <summary>
    /// ラベルのフォントサイズ
    /// </summary>
    [Parameter]
    public string LabelFontSize { get; set; } = "100%";
    /// <summary>
    /// ラベルのフォント幅
    /// </summary>
    [Parameter]
    public string LabelFontWeight { get; set; } = "bold";

    /// <summary>
    /// 必須表示の付加文字の色
    /// </summary>
    [Parameter]
    public string RequiredDisplaySuffixColorPC { get; set; } = "red";

    private IDictionary<string, object>? AttributesStringText { get; set; } = new Dictionary<string, object>();

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(0);
        // セッションストレージのシステムパラメータからパラメータを取得する

        RequiredDisplaySuffix = _sysParam.RequiredDisplaySuffixPC;
        TitleFontSize = _sysParam.PC_GroupFieldTitleFontSize;
        TitleFontWeight = _sysParam.PC_GroupFieldTitleFontWeight;
        LabelFontSize = _sysParam.PC_GroupFieldLabelFontSize;
        LabelFontWeight = _sysParam.PC_GroupFieldLabelFontWeight;
        LabelWidth = _sysParam.PC_GroupFieldLabelWidth;
        RequiredDisplaySuffixColorPC = _sysParam.RequiredDisplaySuffixColorPC;

    }
}

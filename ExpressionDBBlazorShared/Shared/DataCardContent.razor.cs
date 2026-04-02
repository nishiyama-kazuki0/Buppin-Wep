using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Shared;

public partial class DataCardContent
{
    [Inject]
    private ISessionStorageService? _sessionStorage { get; set; }

    [Parameter]
    public IDictionary<string, DataCardListInfo> Values { get; set; } = new Dictionary<string, DataCardListInfo>();
    [Parameter]
    public int SubNum { get; set; } = 1;
    [Parameter]
    public int TotalNum { get; set; } = 1;
    [Parameter]
    public bool IsDispCountNum { get; set; } = false;

    //HT　データカードのmarginの値
    [Parameter]
    public string DataCardContentMarginTop { get; set; } = "0px";
    [Parameter]
    public string DataCardContentMarginRight { get; set; } = "0px";
    [Parameter]
    public string DataCardContentMarginLeft { get; set; } = "0px";
    [Parameter]
    public string DataCardContentMarginBottom { get; set; } = "0px";

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? Attributes { get; set; }

    // HT関連設定情報
    public int ColumnSize { get; set; } = 5;
    public string FontSizeLabel { get; set; } = "125%";
    public string FontSizeTextBox { get; set; } = "150%";
    public string FontWeightBold { get; set; } = "bold";
    public string CaseBaraMargin { get; set; } = "3px";
    public string CaseBaraContains { get; set; } = "ｹｰｽ/ﾊﾞﾗ";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        try
        {

            //TODO できればストレージからパラメータの取得はやめる
            SystemParameterService sysParams = SystemParamService;

            ColumnSize = sysParams.CardColumnSize;
            FontSizeLabel = sysParams.CardFontSizeLabel;
            FontSizeTextBox = sysParams.CardFontSizeTextBox;
            FontWeightBold = sysParams.CardFontWeightBold;
            CaseBaraMargin = sysParams.CardCaseBaraMargin;
            CaseBaraContains = sysParams.CardCaseBaraContains;

            DataCardContentMarginTop = sysParams.HT_DataCardContentMarginTop;
            DataCardContentMarginRight = sysParams.HT_DataCardContentMarginRight;
            DataCardContentMarginLeft = sysParams.HT_DataCardContentMarginLeft;
            DataCardContentMarginBottom = sysParams.HT_DataCardContentMarginBottom;

        }
        catch
        {
            //TODO エラーログ出力
        }
    }
}
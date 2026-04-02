using Blazored.SessionStorage;
using DocumentFormat.OpenXml.Spreadsheet;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace ExpressionDBBlazorShared.Shared;

public partial class DataCardMessageList
{
    [Inject]
    private ISessionStorageService? _sessionStorage { get; set; }
    [Inject]
    private CommonService? ComService { get; set; }
    private bool ProcDataSetFlg = false;
    public List<DataCardMessageListInfo> _cardValues = [];
    [Parameter]
    public List<DataCardMessageListInfo> CardValues
    {
        get => _cardValues;
        set
        {
            _cardValues = value;
            if (value is not null && value.Count > 0 && SelectedData is null && ProcDataSetFlg == false)
            {
                ProcDataSetFlg = true;
                _ = radzenData?.FirstPage();
                if (InitPageIndex > 0)
                {
                    // 初期ページが設定されている場合はページを切り替える
                    _ = (radzenData?.GoToPage(InitPageIndex));
                }
                else
                {
                    OnPageChanged(new PagerEventArgs { PageIndex = 0 });
                }
                ProcDataSetFlg = false;
            }
        }
    }

    /// <summary>
    /// グリッドで選択しているデータ
    /// </summary>
    [Parameter]
    public IList<DataCardMessageListInfo>? SelectedData { get; set; }
    /// <summary>
    /// グリッドの選択データ変更イベントのコールバック
    /// </summary>
    [Parameter]
    public EventCallback<IList<DataCardMessageListInfo>> SelectedDataChanged { get; set; }

    /// <summary>
    /// データ仮想化の許可
    /// </summary>
    [Parameter]
    public bool AllowVirtualization { get; set; } = false;

    /// <summary>
    /// ページ分割有時のページ別集計(1ページに何行)の表示有無
    /// </summary>
    [Parameter]
    public bool ShowPagingSummary { get; set; } = true;
    /// <summary>
    /// ページサイズ
    /// </summary>
    [Parameter]
    public int PageSize { get; set; } = 1;
    /// <summary>
    /// ページ分割有時のUI配置位置
    /// </summary>
    [Parameter]
    public HorizontalAlign PagerHorizontalAlign { get; set; } = HorizontalAlign.Left;
    /// <summary>
    /// ページャーの表示位置
    /// </summary>
    [Parameter]
    public PagerPosition PagerPosition { get; set; } = PagerPosition.Top;
    //HT データカード内のStackのmarginの値
    [Parameter]
    public string HT_InsideDataCardContentMarginTop { get; set; } = "0px";
    [Parameter]
    public string HT_InsideDataCardContentMarginRight { get; set; } = "0px";
    [Parameter]
    public string HT_InsideDataCardContentMarginLeft { get; set; } = "0px";
    /// <summary>
    /// HT　データカードリストの高さ
    /// </summary>
    [Parameter]
    public string HT_DataCardListHeight { get; set; } = "400px";

    private IDictionary<string, object>? AttributesStringText { get; set; } = new Dictionary<string, object>();

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? Attributes { get; set; }

    public int InitPageIndex = 0;

    private RadzenDataList<DataCardMessageListInfo>? radzenData { get; set; }

    protected override async Task OnInitializedAsync()
    {
        //日本語化。取り急ぎべたで対応
        AttributesStringText.Add("FilterText", "フィルター");

        AttributesStringText.Add("PagingSummaryFormat", "ページ {0} / {1} (総件数:{2})");


        SystemParameterService sysParams = SystemParamService;

        HT_DataCardListHeight = sysParams.HT_DataCardListHeight;
        HT_InsideDataCardContentMarginTop = sysParams.HT_InsideDataCardContentMarginTop;
        HT_InsideDataCardContentMarginLeft = sysParams.HT_InsideDataCardContentMarginLeft;
        HT_InsideDataCardContentMarginRight = sysParams.HT_InsideDataCardContentMarginRight;

        await base.OnInitializedAsync();
    }

    protected async void OnPageChanged(PagerEventArgs args)
    {
        List<DataCardMessageListInfo>? list = CardValues?.Skip(PageSize * args.PageIndex).Take(PageSize).ToList();
        SelectedData = list.Any() ? list : null;
        await SelectedDataChanged.InvokeAsync(SelectedData);

        StateHasChanged();
    }

    public async Task NextPage()
    {
        await (radzenData?.NextPage());
    }

    public async Task BackPage()
    {
        await (radzenData?.PrevPage());
    }



    [Parameter]
    public bool FirstFocus { get; set; }
    [Parameter]
    public string? Id { get; set; }
    [Parameter]
    public string? Id2 { get; set; }
    [Parameter]
    public string? NextId { get; set; }
    private readonly string PackingQuantity = string.Empty;
}
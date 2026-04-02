using Blazored.SessionStorage;
using DocumentFormat.OpenXml.Spreadsheet;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace ExpressionDBBlazorShared.Shared;

public partial class DataCardList
{
    [Inject]
    private ISessionStorageService? _sessionStorage { get; set; }
    [Inject]
    private CommonService? ComService { get; set; }
    private bool ProcDataSetFlg = false;
    public List<IDictionary<string, DataCardListInfo>> _cardValues = [];
    [Parameter]
    public List<IDictionary<string, DataCardListInfo>> CardValues
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
    public IList<IDictionary<string, DataCardListInfo>>? SelectedData { get; set; }
    /// <summary>
    /// グリッドの選択データ変更イベントのコールバック
    /// </summary>
    [Parameter]
    public EventCallback<IList<IDictionary<string, DataCardListInfo>>> SelectedDataChanged { get; set; }

    /// <summary>
    /// OnPageChangedのコールバック
    /// </summary>
    [Parameter]
    public Action<PagerEventArgs>? OnPageChangedCallback { get; set; }

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

    private RadzenDataList<IDictionary<string, DataCardListInfo>>? radzenData { get; set; }

    protected override async Task OnInitializedAsync()
    {
        //TODO 日本語化。取り急ぎべたで対応
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
        List<IDictionary<string, DataCardListInfo>>? list = CardValues?.Skip(PageSize * args.PageIndex).Take(PageSize).ToList();
        SelectedData = list.Any() ? list : null;
        await SelectedDataChanged.InvokeAsync(SelectedData);

        OnPageChangedCallback?.Invoke(args);

        // カード内の入数によりバラ入力の有効無効を切り替える
        if (IsInCaseBara)
        {
            if (SelectedData.Count > 0)
            {
                if (SelectedData[0].TryGetValue("入数", out DataCardListInfo? val))
                {
                    PackingQuantity = val.Value;
                    StateHasChanged();
                }
            }
        }
    }

    public async Task NextPage()
    {
        await (radzenData?.NextPage());
    }

    public async Task BackPage()
    {
        await (radzenData?.PrevPage());
    }

    /// <summary>
    /// ケースバラ入力用処理
    /// </summary>
    [Parameter]
    public bool IsInCaseBara { get; set; } = false;
    [Parameter]
    public string? InCase { get; set; }
    [Parameter]
    public EventCallback<string> InCaseChanged { get; set; }
    private async void OnValueChanged(string args)
    {
        InCase = args;
        await InCaseChanged.InvokeAsync(InCase);
    }
    [Parameter]
    public string? InBara { get; set; }
    [Parameter]
    public EventCallback<string> InBaraChanged { get; set; }
    private async void OnValue2Changed(string args)
    {
        InBara = args;
        await InBaraChanged.InvokeAsync(InBara);
    }
    [Parameter]
    public bool FirstFocus { get; set; }
    [Parameter]
    public string? Id { get; set; }
    [Parameter]
    public string? Id2 { get; set; }
    [Parameter]
    public string? NextId { get; set; }
    private string PackingQuantity = string.Empty;
}
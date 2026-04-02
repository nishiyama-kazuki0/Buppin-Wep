using Blazor.DynamicJS;
using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using SharedModels;
using System.Security.Cryptography;
using System.Text;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// データグリッドコンポーネント（トグルボタン対応）
/// </summary>
public partial class DataGridExtend3 : ComponentBase
{
    public const string KEY_DATA_TYPE = "ToggleButton.Type";
    public const string KEY_DATA_VALUE = "ToggleButton.Value";

    private readonly ProgressBarStyle[] PROGRESS_STYLES = {
        ProgressBarStyle.Danger,
        ProgressBarStyle.Warning,
        ProgressBarStyle.Success,
        ProgressBarStyle.Primary,
        ProgressBarStyle.Secondary
    };

    [Inject]
    private CommonService ComService { get; set; } = null!;

    [Inject]
    private ISessionStorageService _sessionStorage { get; set; } = null!;

    [Inject]
    private TooltipService TtpService { get; set; } = null!;

    /// <summary>
    /// グリッドに表示するデータ
    /// </summary>
    [Parameter]
    public List<IDictionary<string, object>> Data { get; set; } = [];
    /// <summary>
    /// グリッドに設定するカラムデータ
    /// </summary>
    [Parameter]
    public List<ComponentColumnsInfo> Columns { get; set; } = [];
    /// <summary>
    /// HTフラグ
    /// </summary>
    [Parameter]
    public bool IsHandy { get; set; } = false;
    /// <summary>
    /// グリッドで選択しているデータ
    /// </summary>
    [Parameter]
    public IList<IDictionary<string, object>>? SelectedData { get; set; }

    /// <summary>
    /// グリッドの選択データ変更イベントのコールバック
    /// </summary>
    [Parameter]
    public EventCallback<IList<IDictionary<string, object>>> SelectedDataChanged { get; set; }

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
    public bool AllowPageing { get; set; } = false;
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
    /// フィルター機能有無
    /// </summary>
    [Parameter]
    public bool AllowFiltering { get; set; } = true;
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
    public bool AllowDataExport { get; set; } = false;
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

    /// <summary>
    /// Exportファイル名
    /// </summary>
    [Parameter]
    public string ExportCsvName { get; set; } = string.Empty;

    /// <summary>
    /// ヘッダーに表示するテキストのタイトル
    /// </summary>
    [Parameter]
    public string HeaderTextTitle { get; set; } = string.Empty;
    /// <summary>
    /// ヘッダーに表示するテキスト
    /// </summary>
    [Parameter]
    public string HeaderTextValue { get; set; } = string.Empty;
    /// <summary>
    /// 凡例の表示非表示
    /// </summary>
    [Parameter]
    public bool VisbleLegend { get; set; } = false;
    /// <summary>
    /// 凡例マークの幅
    /// </summary>
    [Parameter]
    public int LegendMarkWidth { get; set; } = 50;
    /// <summary>
    /// 凡例テキストの幅
    /// </summary>
    [Parameter]
    public int LegendTextWidth { get; set; } = 150;

    private IDictionary<string, object>? AttributesStringText { get; set; } = new Dictionary<string, object>();

    //RadzenDataGrid<IDictionary<string, object>> grid;

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? Attributes { get; set; }

    [Parameter]
    public Action<DataGridCellRenderEventArgs<IDictionary<string, object>>>? OnCellRenderCallback { get; set; }
    //public EventCallback<DataGridCellRenderEventArgs<IDictionary<string, object>>> OnCellRenderCallback { get; set; }

    [Parameter]
    public string DataGridHeaderFontSize { get; set; } = "100%";

    [Parameter]
    public string DataGridColumnFontSize { get; set; } = "100%";

    [Parameter]
    public string DataGridCellFontSize { get; set; } = "100%";

    [Parameter]
    public string DataGridCellFontWeight { get; set; } = "normal";

    [Parameter]
    public string DataGridFooterFontSize { get; set; } = "100%";
    
    ComponentColumnsInfo? typeCol;
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
        AttributesStringText.Add("SelectedItemsText", "件選択");
        AttributesStringText.Add("PagingSummaryFormat", "ページ {0} / {1} (総件数:{2})");

        AttributesStringText.Add("ColumnsShowingText", "列表示");
        AttributesStringText.Add("EmptyText", "表示データなし");

        //各画面など個別で後からパラメータを書き換える場合は、後のawait base.OnInitializedAsync();内でパラメータ属性プロパティを書き換えるとする
        SystemParameterService sysParams = SystemParamService;
        DataGridHeaderFontSize = DeviceInfo.IsHandy() ? sysParams.DataGridHeaderFontSizeHT : sysParams.DataGridHeaderFontSizePC;
        DataGridColumnFontSize = DeviceInfo.IsHandy() ? sysParams.DataGridColumnFontSizeHT : sysParams.DataGridColumnFontSizePC;
        DataGridCellFontSize = DeviceInfo.IsHandy() ? sysParams.DataGridCellFontSizeHT : sysParams.DataGridCellFontSizePC;
        DataGridCellFontWeight = DeviceInfo.IsHandy() ? sysParams.DataGridCellFontWeightHT : sysParams.DataGridCellFontWeightPC;
        DataGridFooterFontSize = DeviceInfo.IsHandy() ? sysParams.DataGridFooterFontSizeHT : sysParams.DataGridFooterFontSizePC;

        LegendMarkWidth = sysParams.PC_DataGridProgressMarkWidth;
        LegendTextWidth = sysParams.PC_DataGridProgressTextWidth;

        PageSize = sysParams.DataGridPageSize;

        await base.OnInitializedAsync();
    }

    protected void ChangeSelectCheckClick(bool args)
    {
    }

    /// <summary>
    /// エクスポート処理
    /// </summary>
    /// <param name="args"></param>
    protected async void ExportClick(RadzenSplitButtonItem args)
    {
        try
        {
            // セパレートファイル種類確認
            SeparatedFileInfo? info = null;
            if (args != null)
            {
                info = SharedConst.SeparatedFileInfos.FirstOrDefault(_ => _.Extension == args.Value);
            }
            info ??= SharedConst.SeparatedFileInfos[0];

            // 確認
            bool? ret = await ComService.DialogShowYesNo($"グリッド情報を{info?.Name}出力しますか？");
            if (true != ret)
            {
                return;
            }

            string file = string.IsNullOrEmpty(ExportCsvName) ? "" : ExportCsvName + "_";
            string filename = $"{file}{DateTime.Now:yyyyMMddhhmmss}";

            // 出力フラグが立っている列名を取得する
            List<string> expCol = Columns.Where(_ => _.IsDataExport).Select(_ => _.Property).ToList();
            List<List<string>> expCsvs =
            [
                // タイトルを追加
                expCol.ToList()
            ];
            foreach (IDictionary<string, object> row in Data)
            {
                List<string> datas = [];
                for (int i = 0; expCol.Count > i; i++)
                {
                    datas.Add(row[expCol[i]].ToString()!);
                }
                expCsvs.Add(datas);
            }

            if (info != null)
            {
                string strCsv = string.Empty;
                for (int i = 0; expCsvs.Count > i; i++)
                {
                    // 一行目はコメント
                    if (0 == i)
                    {
                        strCsv += "#";
                    }
                    strCsv += string.Join(info.Delimiter, expCsvs[i]) + Environment.NewLine;
                }
                using MemoryStream memoryStream = new();
                using StreamWriter writer = new(memoryStream, info.FileEncoding);
                writer.Write(strCsv);
                writer.Flush();
                byte[] bytes = memoryStream.ToArray();

                _ = await BlazorDownloadFileService.DownloadFile($"{filename}.{info.Extension}", bytes, System.Net.Mime.MediaTypeNames.Application.Octet);
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, SharedConst.TYPE_LOGGER.FATAL);
        }
    }

    public string GetColumnPropertyExpression(string name, Type type)
    {
        string expression = $@"it[""{name}""].ToString()";

        if (type == typeof(int))
        {
            return $"int.Parse({expression})";
        }
        else if (type == typeof(decimal))
        {
            return $"Decimal.Parse({expression})";
        }
        else if (type == typeof(DateTime))
        {
            return $"DateTime.Parse({expression})";
        }

        return expression;
    }

    public string GetColumnWidthExpression(int wid)
    {
        return $"{wid}px";
    }

    private void OnCellRender(DataGridCellRenderEventArgs<IDictionary<string, object>> args)
    {
        OnCellRenderCallback?.Invoke(args);
    }

    /// <summary>
    /// グリッドの選択データ変更イベント
    /// </summary>
    /// <param name="args"></param>
    private async void OnValueChanged(IList<IDictionary<string, object>> args)
    {
        SelectedData = args;
        await SelectedDataChanged.InvokeAsync(SelectedData);
    }

    /// <summary>
    /// RadzenNumericセルの変更イベント
    /// </summary>
    /// <param name="args"></param>
    private void OnCellChanged_Numeric(IDictionary<string, object> row, string key, int val)
    {
        try
        {
            row[key] = val;
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, SharedConst.TYPE_LOGGER.FATAL);
        }
    }

    public void SelectUp()
    {
        if (SelectedData is not null && SelectedData.Count > 0)
        {
            int idx = Data.IndexOf(SelectedData[0]);
            if (idx > 0)
            {
                SelectedData = [Data[idx - 1]];
            }
            else if (idx < 0)
            {
                SelectedData = [Data[0]];
            }
            OnValueChanged(SelectedData);
        }
    }

    public void SelectDown()
    {
        if (SelectedData is not null && SelectedData.Count > 0)
        {
            int idx = Data.IndexOf(SelectedData[0]);
            if (idx < Data.Count - 1)
            {
                SelectedData = [Data[idx + 1]];
            }
            else if (idx < 0)
            {
                SelectedData = [Data[0]];
            }
            OnValueChanged(SelectedData);
        }
    }

    private void ShowTooltip(ElementReference elementReference, string text, TooltipOptions? options = null)
    {
        TtpService.Open(elementReference, text, options);
    }
}
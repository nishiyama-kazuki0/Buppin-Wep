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

public partial class DataGridExtend2 : ComponentBase
{
    [Inject]
    private CommonService ComService { get; set; } = null!;

    [Inject]
    private ISessionStorageService _sessionStorage { get; set; } = null!;

    [Inject]
    protected IJSRuntime? JS { get; set; }
    protected DynamicJSRuntime? _js;
    public ValueTask DisposeAsync()
    {
        return _js?.DisposeAsync() ?? ValueTask.CompletedTask;
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _js = await JS!.CreateDymaicRuntimeAsync();
        }
    }

    /// <summary>
    /// データグリッド本体
    /// ※RadzenDataGridをref参照しているため、ReadOnly属性は付加できないが
    /// 　変数は参照のみと書き換えないこと！！
    /// </summary>
    internal RadzenDataGrid<IDictionary<string, object>>? MainGrid = null;

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
    /// HTの上下ボタンでグリッドを選択させるかどうか
    /// </summary>
    [Parameter]
    public bool IsHandyGridSelect { get; set; } = false;
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
    /// グリッドで選択しているセルデータ(セル選択モードのときのみデータが入る)
    /// </summary>
    [Parameter]
    public IList<Tuple<IDictionary<string, object>, RadzenDataGridColumn<IDictionary<string, object>>>> SelectedCellData { get; set; } = [];

    /// <summary>
    /// グリッドの選択セルデータ変更イベントのコールバック(セル選択モードのときのみ発生)
    /// </summary>
    [Parameter]
    public EventCallback<IList<Tuple<IDictionary<string, object>, RadzenDataGridColumn<IDictionary<string, object>>>>> SelectedCellDataChanged { get; set; }

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
    /// 仮想表示
    /// グリッド高さの指定があるときのみ効果あり
    /// </summary>
    [Parameter]
    public bool AllowVirtualization { get; set; } = false;
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
    /// グリッドの高さ(単位の文字列も必要。空文字の場合はStyleを指定しない)
    /// </summary>
    [Parameter]
    public string GridHeight { get; set; } = string.Empty;

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

    [Parameter]
    public string BaraColor { get; set; } = "#ff0000";

    /// <summary>
    /// Export権限レベル設定
    /// </summary>
    [Parameter]
    public int ExportAuthorityLevelLower { get; set; } = -1;

    /// <summary>
    /// 列名に本文字列が含まれている場合はセルのバックグランド色を示すとする
    /// </summary>
    [Parameter]
    public string ColorBKCellName { get; set; } = string.Empty;
    /// <summary>
    /// 列名に本文字列が含まれている場合は行のバックグランド色を示すとする
    /// </summary>
    [Parameter]
    public string ColorBKRowName { get; set; } = string.Empty;
    /// <summary>
    /// 列名に本文字列が含まれている場合はセルの文字色を示すとする
    /// </summary>
    [Parameter]
    public string ColorFRCellName { get; set; } = string.Empty;
    /// <summary>
    /// 列名に本文字列が含まれている場合は行の文字色を示すとする
    /// </summary>
    [Parameter]
    public string ColorFRRowName { get; set; } = string.Empty;
    /// <summary>
    /// 色を示す列名の場合に分割文字列と判断する文字
    /// </summary>
    [Parameter]
    public string ColorSplitString { get; set; } = string.Empty;
    /// <summary>
    /// 色を示す列名の場合に分割文字列と判断する文字
    /// </summary>
    [Parameter]
    public string UnSelectableCellName { get; set; } = string.Empty;
    /// <summary>
    /// データ選択方式
    /// false：SelectionModeによる行選択
    /// true：セル選択モード(SingleかMultipleかはSelectionModeによる)
    /// </summary>
    [Parameter]
    public bool SelectionCell { get; set; } = false;


    protected override async Task OnInitializedAsync()
    {
        //日本語化初期値。変更したい場合はDEFINE_COMPONENTSから属性スプラッティングで指定。すること。
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
        GridHeight = DeviceInfo.IsHandy() ? sysParams.DataGridHeightHT : sysParams.DataGridHeightPC;
        BaraColor = sysParams.HT_DataGridBaraColor;
        PageSize = sysParams.DataGridPageSize;

        //配色設定に必要なシステムパラメータ
        ColorBKCellName = sysParams.ColorBKGridCellContainColumnName;
        ColorBKRowName = sysParams.ColorBKGridRowContainColumnName;
        ColorFRCellName = sysParams.ColorFRGridCellContainColumnName;
        ColorFRRowName = sysParams.ColorFRGridRowContainColumnName;
        ColorSplitString = sysParams.ColorGridColumnNameSplitString;
        UnSelectableCellName = sysParams.UnSelectableGridCellContainColumnName;

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
            LoginInfo login = new();
            if (await SessionStorage.ContainKeyAsync(SharedConst.KEY_LOGIN_INFO))
            {
                login = await SessionStorage.GetItemAsync<LoginInfo>(SharedConst.KEY_LOGIN_INFO);
            }
            if (ExportAuthorityLevelLower >= login.AuthorityLevel)
            {
                await ComService.DialogShowOK("権限制限がされているため実行できません。");
                return;
            }
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
        List<string> classList = new();
        StringBuilder stylestringSb = new();

        //カラム名は、色を変更したい[カラム名_色コード]と定義する前提とする。アンダースコアがスプリット文字として、対象のカラム名のバックグランドカラーとフォアカラーを変更する。
        //DEFINE_COMPONENT_COLUMNSとVIEWの両方に定義されている必要がある。
        //セルレンダーイベントはwidthが0は発生しない点に注意すること。

        string? rowbkColorCode = null;
        string? cellbkColorCode = null;
        string? cellfrColorCode = null;

        //colorBKRowNameの列が存在すれば、行の背景色を変更を試みるとする。
        if (args.Data.TryGetValue($"{ColorBKRowName}", out object? valuerb))
        {
            rowbkColorCode = valuerb.ToString();
            if (!string.IsNullOrEmpty(rowbkColorCode))
            {
                _ = stylestringSb.Append($"--bg-color:{rowbkColorCode};");
            }
        }
        else
        {
            //行の色指定がなければセルの色指定を試みるとする。
            //セル背景色の変更
            if (args.Data.TryGetValue($"{args.Column.Title}{ColorSplitString}{ColorBKCellName}", out object? valuecb))
            {
                cellbkColorCode = valuecb.ToString();
                if (!string.IsNullOrEmpty(cellbkColorCode))
                {
                    _ = stylestringSb.Append($"--bg-color:{cellbkColorCode};");
                }
            }

            //セル文字色の変更
            if (args.Data.TryGetValue($"{args.Column.Title}{ColorSplitString}{ColorFRCellName}", out object? valuecf))
            {
                cellfrColorCode = valuecf.ToString();
                if (!string.IsNullOrEmpty(cellfrColorCode))
                {
                    //_ = stylestringSb.Append($"color:{cellfrColorCode};");
                    //TODO フォントカラーはなぜかグリッドせるではcolor:で変化がないので、文字で指定があれば現状すべて白文字としている
                    //args.Attributes.Add("class", "white-text");
                }
            }
        }

        //スタイルの文字列が空文字でない場合はスタイルを追加する。
        string retstr = stylestringSb.ToString();
        if (!string.IsNullOrEmpty(retstr))
        {
            classList.Add("rz-set-background-color");
            args.Attributes.Add("style", stylestringSb.ToString());
        }

        //セル選択フラグ
        if (SelectionCell)
        {
            bool unselectable = false;
            if (args.Data.TryGetValue($"{args.Column.Title}{ColorSplitString}{UnSelectableCellName}", out object? valueus))
            {
                if (valueus.ToString() == "1")
                {
                    unselectable = true;
                }
            }

            if (unselectable)
            {
                //選択不可
                classList.Add("rz-unselectable-cell");
            }
            else
            {
                //選択可能
                classList.Add("rz-selectable-cell");

                //選択状態かチェック
                if (SelectedCellData.Any(i => i.Item1 == args.Data && i.Item2 == args.Column))
                {
                    classList.Add("rz-selected-cell");
                }
            }
        }

        if (classList.Count > 0)
        {
            StringBuilder sb = new();
            //クラス設定を行なう。クラス間は半角スペースを忘れずに
            for (int i = 0; i < classList.Count; i++)
            {
                if (i > 0) sb.Append(" ");
                sb.Append(classList[i].ToString());
            }
            args.Attributes.Add("class", sb.ToString());
        }

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
    /// グリッドのセルクリックイベント
    /// </summary>
    /// <param name="args"></param>
    private async void OnCellClick(DataGridCellMouseEventArgs<IDictionary<string, object>> args)
    {
        //セル選択モードの場合
        if (SelectionCell)
        {
            if (SelectionMode == DataGridSelectionMode.Single)
            {
                //シングルの場合は後優先にするため、前回までの選択をクリアする
                SelectedCellData.Clear();
            }
            
            var cellData = SelectedCellData.FirstOrDefault(i => i.Item1 == args.Data && i.Item2 == args.Column);
            if (cellData != null)
            {
                SelectedCellData.Remove(cellData);
            }
            else
            {
                SelectedCellData.Add(new Tuple<IDictionary<string, object>, RadzenDataGridColumn<IDictionary<string, object>>>(args.Data, args.Column));
            }

            await SelectedCellDataChanged.InvokeAsync(SelectedCellData);
        }
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

            if (IsHandy && IsHandyGridSelect)
            {
                try
                {
                    // セル内に追加されたdivタグにフォーカスを移動することでスクロールさせる
                    int indx = Data.IndexOf(SelectedData[0]);
                    dynamic window = _js!.GetWindow();
                    dynamic element = window.document.getElementById($"FocusUpper_{indx}");
                    element?.focus();
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync(ex.Message, SharedConst.TYPE_LOGGER.FATAL);
                }
            }
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

            if (IsHandy && IsHandyGridSelect)
            {
                try
                {
                    // セル内に追加されたdivタグにフォーカスを移動することでスクロールさせる
                    int indx = Data.IndexOf(SelectedData[0]);
                    dynamic window = _js!.GetWindow();
                    dynamic element = window.document.getElementById($"FocusLower_{indx}");
                    element?.focus();
                }
                catch (Exception ex)
                {
                    _ = WebComService.PostLogAsync(ex.Message, SharedConst.TYPE_LOGGER.FATAL);
                }
            }
        }
    }
}
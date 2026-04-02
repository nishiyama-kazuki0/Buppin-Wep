using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using SharedModels;
namespace ExpressionDBBlazorShared.Shared;

public partial class CompDropDownDataGridSingle : CompBase
{
    public RadzenDropDownDataGrid<string> grid = new();
    ///// <summary>
    ///// グリッドに設定するカラムデータ
    ///// </summary>
    [Parameter]
    public List<ComponentColumnsInfo> Columns { get; set; } = [];
    [Parameter]
    public IEnumerable<ValueTextInfo> Data { get; set; } = [];
    [Parameter]
    public string ModelName { get; set; } = string.Empty;

    [Parameter]
    public string InitialValue { get; set; } = string.Empty;

    public string _inputValue = string.Empty;
    [Parameter]
    public string InputValue
    {
        get => _inputValue;
        set
        {
            if (_inputValue != value)
            {
                _inputValue = value;
                // 親に変更を伝達する
                _ = ValueChanged.InvokeAsync(_inputValue);
            }
        }
    }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public Action<object>? OnChangeCallback { get; set; } = null;

    [Parameter]
    public string TextProperty { get; set; } = "Text";
    [Parameter]
    public string ValueProperty { get; set; } = "Value";
    [Parameter]
    public string Placeholder { get; set; } = "検索...";
    [Parameter]
    public bool FilteringAll { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // システムパラメータ取得
        if (null != _sysParam)
        {
            CompFontSize = _sysParam.PC_DropDownDataGridFontSize;
            CompFontWeight = _sysParam.PC_DropDownDataGridFontWeight;
            CompHeight = _sysParam.PC_DropDownDataGridHeight;
        }

        // style判別のためClassにクラス名セット
        Class = GetType().Name;

        // スタイル設定
        _Style += $"--rz-chip-font-size:{CompFontSize};";
        _Style += $"font-weight:{CompFontWeight};";
        if (!string.IsNullOrEmpty(CompHeight))
        {
            _Style += $"height:{CompHeight};";
        }

        InputValue = InitialValue;
    }

    private void OnChange(bool value)
    {
    }

    /// <summary>
    /// 入力値のリセット
    /// </summary>
    public override void ResetValue()
    {
        // 初期値設定
        InputValue = InitialValue;
        // 再レンダリング
        StateHasChanged();
    }

    /// <summary>
    /// 入力値クリア
    /// </summary>
    public override void ClearValue()
    {
        // 入力値クリア
        InputValue = string.Empty;
        _ = grid.Reset();
        // 再レンダリング
        StateHasChanged();
    }

    /// <summary>
    /// 選択データ取得
    /// </summary>
    /// <returns></returns>
    public ValueTextInfo? GetSelectedData()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0] : null;
    }
    public override void AddWhereParam(Dictionary<string, (object, WhereParam)> retWhereParam, string keyNmae)
    {
        if (!string.IsNullOrEmpty(InputValue))
        {
            retWhereParam[keyNmae] = (InputValue, new WhereParam { val = InputValue, whereType = enumWhereType.Equal });
        }
    }
    public override void SetInitValue(object? initVal)
    {
        if (initVal is not null)
        {
            InputValue = (string)initVal;
        }
    }
    public override void AddkeyValuePair(Dictionary<string, object> retKeyValuePairs, string keyName, bool bnGetEmpty)
    {

        if (!string.IsNullOrEmpty(InputValue))
        {
            retKeyValuePairs[keyName] = InputValue;
        }
        else
        {
            if (bnGetEmpty)
            {
                retKeyValuePairs[keyName] = string.Empty;
            }
        }
    }
    public override void AddParam(
        Dictionary<string, object> retParam
        , ComponentColumnsInfo info
        , IList<ComponentColumnsInfo> ComponentColumns
        , object? initVal
        , List<ValueTextInfo>? data
        , ref bool bnDispTitle
        , enumDateInitMode initMode
        , DateTime? dtWms
        , DateTime? dtWmsAdd
        , IList<ComponentsInfo> components
        )
    {
        List<ComponentColumnsInfo> _dropDownColumns = [];
        List<ComponentColumnsInfo> columns = ComponentColumns.Where(_ => _.ComponentName == info.Property).ToList();
        for (int i = 0; columns.Count > i; i++)
        {
            _dropDownColumns.Add(new ComponentColumnsInfo() { Property = $"Value{i + 1}", Title = columns[i].Title, Type = columns[i].Type, Width = columns[i].Width, TextAlign = columns[i].TextAlign });
        }
        if (initVal is not null)
        {
            retParam.Add("InitialValue", initVal);
        }
        retParam.Add("Columns", _dropDownColumns);
        retParam.Add("Data", data!);
        //他コンポーネント名に紐付くプロパティ値をDynamicコンポーネント用パラメータに追加する
        AddParamComponets(retParam, components, info);
    }

    private void OnChange(object value)
    {
        OnChangeCallback?.Invoke(value);
    }
}

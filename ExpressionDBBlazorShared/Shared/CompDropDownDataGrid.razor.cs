using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using SharedModels;
namespace ExpressionDBBlazorShared.Shared;

public partial class CompDropDownDataGrid : CompBase
{
    public RadzenDropDownDataGrid<IEnumerable<string>> grid = new();
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
    public IEnumerable<string> InitialValues { get; set; } = [];

    public IEnumerable<string> _Values = [];
    [Parameter]
    public IEnumerable<string> Values
    {
        get => _Values;
        set
        {
            if (_Values != value)
            {
                _Values = value;
                // 親に変更を伝達する
                _ = ValuesChanged.InvokeAsync(_Values);
            }
        }
    }

    [Parameter]
    public EventCallback<IEnumerable<string>> ValuesChanged { get; set; }

    [Parameter]
    public string TextProperty { get; set; } = "Text";
    [Parameter]
    public string ValueProperty { get; set; } = "Value";
    [Parameter]
    public string Placeholder { get; set; } = "検索...";
    [Parameter]
    public bool FilteringAll { get; set; } = true;

    //PC　DropDownDataGridのチェックボックスの幅
    [Parameter]
    public string CheckBoxWidth { get; set; } = "60px";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // システムパラメータ取得
        CompFontSize = _sysParam.PC_DropDownDataGridFontSize;
        CompFontWeight = _sysParam.PC_DropDownDataGridFontWeight;
        CompHeight = _sysParam.PC_DropDownDataGridHeight;

        CheckBoxWidth = _sysParam.PC_DropDownDataGridCheckBoxWidth;

        // style判別のためClassにクラス名セット
        Class = GetType().Name;

        // スタイル設定
        _Style += $"--rz-chip-font-size:{CompFontSize};";
        _Style += $"font-weight:{CompFontWeight};";
        if (!string.IsNullOrEmpty(CompHeight))
        {
            _Style += $"height:{CompHeight};";
        }

        Values = InitialValues;
    }

    //[Parameter]
    //public EventCallback<object> OnChangeCallback { get; set; }

    //private async void OnChange(object value)
    //{
    //    await OnChangeCallback.InvokeAsync(value);

    //    object str = value is IEnumerable<object> ? string.Join(", ", (IEnumerable<object>)value) : value;
    //    System.Console.WriteLine(str);
    //    //console.Log($"Value changed to {str}");
    //}
    private void OnChange(bool value)
    {
    }

    /// <summary>
    /// 入力値のリセット
    /// </summary>
    public override void ResetValue()
    {
        // 初期値設定
        Values = InitialValues;
        // 再レンダリング
        StateHasChanged();
    }

    /// <summary>
    /// 入力値クリア
    /// </summary>
    public override void ClearValue()
    {
        // 入力値クリア
        Values = [];
        // 再レンダリング
        StateHasChanged();
    }

    /// <summary>
    /// 選択データ取得
    /// </summary>
    /// <returns></returns>
    public List<ValueTextInfo> GetSelectedData()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => Values.Contains(_.Value)).ToList();
        return r;
    }
    public override void AddWhereParam(Dictionary<string, (object, WhereParam)> retWhereParam, string keyNmae)
    {
        if (Values?.Count() > 0)
        {
            List<string> vals = new(Values);
            retWhereParam[keyNmae] = (vals, new WhereParam { field = keyNmae, linkingVals = vals, whereType = enumWhereType.Equal, orLinking = true });
        }
    }
    public override void SetInitValue(object? initVal)
    {
        if (initVal is not null)
        {
            Values = (List<string>)initVal;
        }
    }
    public override void AddkeyValuePair(Dictionary<string, object> retKeyValuePairs, string keyName, bool bnGetEmpty)
    {
        if (Values?.Count() > 0)
        {
            List<string> vals = new(Values);
            retKeyValuePairs[keyName] = vals;
        }
        else
        {
            if (bnGetEmpty)
            {
                retKeyValuePairs[keyName] = new List<string>();
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
            if (typeof(List<string>).IsAssignableFrom(initVal.GetType()))
            {
                retParam.Add("InitialValues", initVal);
            }
            else
            {
                retParam.Add("InitialValues", new List<string> { initVal.ToString() });
            }
        }
        retParam.Add("Columns", _dropDownColumns);
        retParam.Add("Data", data!);
        //他コンポーネント名に紐付くプロパティ値をDynamicコンポーネント用パラメータに追加する
        AddParamComponets(retParam, components, info);
    }
}

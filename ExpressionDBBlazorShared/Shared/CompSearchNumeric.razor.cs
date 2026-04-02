using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using SharedModels;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// CompSearchNumericコンポーネント
/// </summary>
public partial class CompSearchNumeric : CompBase
{
    private decimal _initialValue = 0;
    /// <summary>
    /// 初期値
    /// </summary>
    [Parameter]
    public string InitialValue
    {
        get => _initialValue.ToString(Format);
        set
        {
            if (!decimal.TryParse(value, out _initialValue))
            {
                _initialValue = 0;
            }
        }
    }

    private decimal _inputValue = 0;
    /// <summary>
    /// 入力値
    /// </summary>
    [Parameter]
    public string InputValue
    {
        get => _inputValue.ToString(Format);
        set
        {
            if (!decimal.TryParse(value, out _inputValue))
            {
                _inputValue = 0;
            }
        }
    }

    /// <summary>
    /// 初期値（条件）
    /// </summary>
    [Parameter]
    public string DropInitialValue { get; set; } = string.Empty;

    /// <summary>
    /// 選択値（条件）
    /// </summary>
    [Parameter]
    public string DropInputValue { get; set; } = string.Empty;

    /// <summary>
    /// 表示フォーマット
    /// </summary>
    [Parameter]
    public string Format { get; set; } = "F0";

    /// <summary>
    /// 増減ステップ
    /// </summary>
    [Parameter]
    public string Step { get; set; } = "1";

    /// <summary>
    /// 最小値（nullは制限なし）
    /// </summary>
    [Parameter]
    public decimal? Min { get; set; } = null;

    /// <summary>
    /// 最小値（nullは制限なし）
    /// </summary>
    [Parameter]
    public decimal? Max { get; set; } = null;

    /// <summary>
    /// 最大最小の表示フォーマット
    /// </summary>
    [Parameter]
    public string FormatMinMax { get; set; } = "{0:F0}";

    /// <summary>
    /// DropDownのフォントサイズ
    /// </summary>
    [Parameter]
    public string CompDropDownFontSize { get; set; } = "100%";

    /// <summary>
    /// DropDownのフォント幅
    /// </summary>
    [Parameter]
    public string CompDropDownFontWeight { get; set; } = "normal";

    /// <summary>
    /// DropDownの幅
    /// </summary>
    [Parameter]
    public string CompDropDownWidth { get; set; } = string.Empty;

    /// <summary>
    /// DropDownの高さ
    /// </summary>
    [Parameter]
    public string CompDropDownHeight { get; set; } = string.Empty;

    /// <summary>
    /// DropDownのスタイル
    /// </summary>
    [Parameter]
    public string DropDownStyle { get; set; } = string.Empty;

    private readonly string _dropValueProperty = "Value";
    private readonly string _dropTextProperty = "Text";
    private readonly List<ValueTextInfo> _dropData =
    [
        new ValueTextInfo(){ Value = ((int)enumWhereType.Equal).ToString(), Text = "等しい" },
        new ValueTextInfo(){ Value = ((int)enumWhereType.NotEqual).ToString(), Text = "等しくない" },
        new ValueTextInfo(){ Value = ((int)enumWhereType.Below).ToString(), Text = "以下" },
        new ValueTextInfo(){ Value = ((int)enumWhereType.Above).ToString(), Text = "以上" },
    ];
    private readonly bool _allowClear = true;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // フォントスタイル設定
        CompFontSize = _sysParam.PC_NumericFontSize;
        CompFontWeight = _sysParam.PC_NumericFontWeight;
        CompWidth = _sysParam.PC_NumericWidth;
        CompHeight = _sysParam.PC_NumericHeight;
        CompDropDownFontSize = _sysParam.PC_DropDownFontSize;
        CompDropDownFontWeight = _sysParam.PC_DropDownFontWeight;
        CompDropDownWidth = _sysParam.PC_DropDownWidth;
        CompDropDownHeight = _sysParam.PC_DropDownHeight;

        // style判別のためClassにクラス名セット
        Class = GetType().Name;

        // スタイル設定
        if (!string.IsNullOrEmpty(CompWidth))
        {
            _Style += $"width:{CompWidth};";
        }
        if (!string.IsNullOrEmpty(CompHeight))
        {
            _Style += $"height:{CompHeight};";
        }

        // DropDownスタイル設定
        DropDownStyle += $"font-size:{CompDropDownFontSize};";
        DropDownStyle += $"font-weight:{CompDropDownFontWeight};";
        if (!string.IsNullOrEmpty(CompDropDownWidth))
        {
            DropDownStyle += $"width:{CompDropDownWidth};";
        }
        if (!string.IsNullOrEmpty(CompDropDownHeight))
        {
            DropDownStyle += $"height:{CompDropDownHeight};";
        }
    }

    /// <summary>
    /// 入力値のリセット
    /// </summary>
    public override void ResetValue()
    {
        // 初期値設定
        InputValue = InitialValue;
        DropInputValue = DropInitialValue;
        // 再レンダリング
        StateHasChanged();
    }

    /// <summary>
    /// 入力値クリア
    /// </summary>
    public override void ClearValue()
    {
        // 入力値クリア
        _inputValue = 0;
        DropInputValue = string.Empty;
        // 再レンダリング
        StateHasChanged();
    }
    public override void AddWhereParam(Dictionary<string, (object, WhereParam)> retWhereParam, string keyNmae)
    {
        if (!string.IsNullOrEmpty(DropInputValue))
        {
            if (Enum.TryParse(DropInputValue, out enumWhereType type))
            {
                retWhereParam[keyNmae] = (InputValue, new WhereParam { val = InputValue, whereType = type });
            }

        }
    }
    public override void SetInitValue(object? initVal)
    {
        if (initVal is not null)
        {
            InputValue = ((List<string>)initVal)[0];
            DropInputValue = ((List<string>)initVal)[1];
        }
    }
    public override void AddkeyValuePair(Dictionary<string, object> retKeyValuePairs, string keyName, bool bnGetEmpty)
    {
        if (!string.IsNullOrEmpty(DropInputValue) && Enum.TryParse(DropInputValue, out enumWhereType type))
        {
            retKeyValuePairs[keyName] = new List<string>() { InputValue, ((int)type).ToString() };
        }
        else
        {
            if (bnGetEmpty)
            {
                retKeyValuePairs[keyName] = new List<string>() { string.Empty, string.Empty };
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
        if (initVal is not null)
        {
            if (typeof(List<string>).IsAssignableFrom(initVal.GetType()))
            {
                retParam.Add("InitialValue", ((List<string>)initVal)[0]);
                retParam.Add("DropInitialValue", ((List<string>)initVal)[1]);
            }
            else
            {
                retParam.Add("InitialValue", initVal);
                retParam.Add("DropInitialValue", ((int)enumWhereType.Equal).ToString());
            }
        }
        if (info.ValueMin is not null)
        {
            retParam.Add("Min", info.ValueMin);
        }
        if (info.ValueMax is not null)
        {
            retParam.Add("Max", info.ValueMax);
        }
        if (!string.IsNullOrEmpty(info.FormatString))
        {
            retParam.Add("FormatMinMax", info.FormatString);
        }
        //他コンポーネント名に紐付くプロパティ値をDynamicコンポーネント用パラメータに追加する
        AddParamComponets(retParam, components, info);
    }
}

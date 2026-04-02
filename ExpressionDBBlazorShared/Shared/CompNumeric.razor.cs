using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using SharedModels;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// TextBoxコンポーネント
/// </summary>
public partial class CompNumeric : CompBase
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
    /// 最大値（nullは制限なし）
    /// </summary>
    [Parameter]
    public decimal? Max { get; set; } = null;

    /// <summary>
    /// 最大最小の表示フォーマット
    /// </summary>
    [Parameter]
    public string FormatMinMax { get; set; } = "{0:F0}";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // システムパラメータ取得
        CompFontSize = _sysParam.PC_NumericFontSize;
        CompFontWeight = _sysParam.PC_NumericFontWeight;
        CompWidth = _sysParam.PC_NumericWidth;
        CompHeight = _sysParam.PC_NumericHeight;

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
        _inputValue = 0;
        // 再レンダリング
        StateHasChanged();
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
        if (initVal is not null)
        {
            retParam.Add("InitialValue", initVal);
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

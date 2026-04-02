using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using SharedModels;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// TextBoxコンポーネント
/// </summary>
public partial class CompTextBox2 : CompBase
{
    /// <summary>
    /// 初期値
    /// </summary>
    [Parameter]
    public string InitialValue { get; set; } = string.Empty;

    /// <summary>
    /// 入力値
    /// </summary>
    [Parameter]
    public string InputValue { get; set; } = string.Empty;

    /// <summary>
    /// 最大文字数（nullは制限なし）
    /// </summary>
    [Parameter]
    public long? MaxLength { get; set; } = null;

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
    /// 正規表現文字列
    /// </summary>
    [Parameter]
    public string? RegexPattern { get; set; } = null;

    /// <summary>
    /// 入力バイト数チェック
    /// </summary>
    [Parameter]
    public bool IsByteLength { get; set; } = true;

    /// <summary>
    /// SQL検索タイプ
    /// </summary>
    [Parameter]
    public enumWhereType SearchWhereType { get; set; } = enumWhereType.LikeStart;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // システムパラメータ取得
        if (null != _sysParam)
        {
            CompFontSize = _sysParam.PC_TextBoxFontSize;
            CompFontWeight = _sysParam.PC_TextBoxFontWeight;
            CompWidth = _sysParam.PC_TextBoxWidth;
            CompHeight = _sysParam.PC_TextBoxHeight;
        }

        // スタイル設定
        _Style += $"font-size:{CompFontSize};";
        _Style += $"font-weight:{CompFontWeight};";
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
        InputValue = string.Empty;
        // 再レンダリング
        StateHasChanged();
    }
    public override void AddWhereParam(Dictionary<string, (object, WhereParam)> retWhereParam, string keyNmae)
    {
        if (!string.IsNullOrEmpty(InputValue))
        {
            retWhereParam[keyNmae] = (InputValue, new WhereParam { val = InputValue, whereType = SearchWhereType });
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
        retParam.Add("MaxLength", info.MaxLength!);
        if (info.ValueMin is not null)
        {
            retParam.Add("Min", info.ValueMin);
        }
        if (info.ValueMax is not null)
        {
            retParam.Add("Max", info.ValueMax);
        }
        if (info.RegularExpressionString is not null)
        {
            retParam.Add("RegexPattern", info.RegularExpressionString);
        }
        //他コンポーネント名に紐付くプロパティ値をDynamicコンポーネント用パラメータに追加する
        AddParamComponets(retParam, components, info);
    }
}

using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using SharedModels;

namespace ExpressionDBBlazorShared.Shared;

public partial class CompRadioButtonList : CompBase
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
    /// 選択データ
    /// </summary>
    [Parameter]
    public IEnumerable<ValueTextInfo> Data { get; set; } = [];

    /// <summary>
    /// ValueProperty
    /// </summary>
    [Parameter]
    public string ValueProperty { get; set; } = "Value";

    /// <summary>
    /// TextProperty
    /// </summary>
    [Parameter]
    public string TextProperty { get; set; } = "Text";

    /// <summary>
    /// グループ名称
    /// </summary>
    [Parameter]
    public string FieldsetText { get; set; } = string.Empty;

    /// <summary>
    /// 表示方向
    /// </summary>
    [Parameter]
    public Orientation Orientation { get; set; } = Orientation.Horizontal;

    /// <summary>
    /// タイトルのフォントサイズ
    /// </summary>
    [Parameter]
    public string TitleFontSize { get; set; } = "100%";
    /// <summary>
    /// タイトルのフォント幅
    /// </summary>
    [Parameter]
    public string TitleFontWeight { get; set; } = "normal";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // システムパラメータ取得
        if (null != _sysParam)
        {
            CompFontSize = _sysParam.PC_RadioButtonFontSize;
            CompFontWeight = _sysParam.PC_RadioButtonFontWeight;
            TitleFontSize = _sysParam.PC_RadioButtonTitleFontSize;
            TitleFontWeight = _sysParam.PC_RadioButtonTitleFontWeight;
        }

        // スタイル設定
        _Style += $"font-size:{CompFontSize};";
        _Style += $"font-weight:{CompFontWeight};";
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
        retParam.Add("Data", data!);
        retParam.Add("FieldsetText", info.Title);
        bnDispTitle = false;
        //他コンポーネント名に紐付くプロパティ値をDynamicコンポーネント用パラメータに追加する
        AddParamComponets(retParam, components, info);
    }
}

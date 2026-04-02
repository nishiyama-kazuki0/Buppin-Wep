using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using SharedModels;
namespace ExpressionDBBlazorShared.Shared;

public partial class CompCheckBoxList : CompBase
{
    /// <summary>
    /// 初期値
    /// </summary>
    [Parameter]
    public IEnumerable<string> InitialValues { get; set; } = [];

    /// <summary>
    /// 入力値
    /// </summary>
    [Parameter]
    public IEnumerable<string> InputValues { get; set; } = [];

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
            CompFontSize = _sysParam.PC_CheckBoxFontSize;
            CompFontWeight = _sysParam.PC_CheckBoxFontWeight;
            TitleFontSize = _sysParam.PC_CheckBoxTitleFontSize;
            TitleFontWeight = _sysParam.PC_CheckBoxTitleFontWeight;
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
        InputValues = InitialValues;
        // 再レンダリング
        StateHasChanged();
    }

    /// <summary>
    /// 入力値クリア
    /// </summary>
    public override void ClearValue()
    {
        // 入力値クリア
        InputValues = [];
        // 再レンダリング
        StateHasChanged();
    }

    public override void AddWhereParam(Dictionary<string, (object, WhereParam)> retWhereParam, string keyNmae)
    {
        if (InputValues?.Count() > 0)
        {
            List<string> vals = new(InputValues);
            retWhereParam[keyNmae] = (vals, new WhereParam { field = keyNmae, linkingVals = vals, whereType = enumWhereType.Equal, orLinking = true });
        }
    }

    public override void SetInitValue(object? initVal)
    {
        if (initVal is not null)
        {
            InputValues = (List<string>)initVal;
        }
    }
    public override void AddkeyValuePair(Dictionary<string, object> retKeyValuePairs, string keyName, bool bnGetEmpty)
    {
        if (InputValues?.Count() > 0)
        {
            List<string> vals = new(InputValues);
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
        retParam.Add("Data", data!);
        retParam.Add("FieldsetText", info.Title);

        bnDispTitle = false;
        //他コンポーネント名に紐付くプロパティ値をDynamicコンポーネント用パラメータに追加する
        AddParamComponets(retParam, components, info);
    }
}

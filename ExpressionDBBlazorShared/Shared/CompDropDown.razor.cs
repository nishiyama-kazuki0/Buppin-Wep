using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using SharedModels;

namespace ExpressionDBBlazorShared.Shared;

public partial class CompDropDown : CompBase
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
    /// クリアボタン ON/OFF
    /// </summary>
    [Parameter]
    public bool AllowClear { get; set; } = true;

    /// <summary>
    /// フィルタリング ON/OFF
    /// </summary>
    [Parameter]
    public bool AllowFiltering { get; set; } = false;

    /// <summary>
    /// OnChangeイベント
    /// </summary>
    public event EventHandler<object>? ChangeSelectValue;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // システムパラメータ取得
        if (null != _sysParam)
        {
            CompFontSize = _sysParam.PC_DropDownFontSize;
            CompFontWeight = _sysParam.PC_DropDownFontWeight;
            CompWidth = _sysParam.PC_DropDownWidth;
            CompHeight = _sysParam.PC_DropDownHeight;
        }

        // style判別のためClassにクラス名セット
        Class = GetType().Name;

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

    #region 選択データ取得

    /// <summary>
    /// 選択データ取得
    /// </summary>
    /// <returns></returns>
    public string GetInputText()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Text : string.Empty;
    }

    /// <summary>
    /// 選択データのValue1取得
    /// </summary>
    /// <returns></returns>
    public string GetInputValue1()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Value1 : string.Empty;

    }

    /// <summary>
    /// 選択データのValue2取得
    /// </summary>
    /// <returns></returns>
    public string GetInputValue2()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Value2 : string.Empty;

    }

    /// <summary>
    /// 選択データのValue3取得
    /// </summary>
    /// <returns></returns>
    public string GetInputValue3()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Value3 : string.Empty;

    }

    /// <summary>
    /// 選択データのValue4取得
    /// </summary>
    /// <returns></returns>
    public string GetInputValue4()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Value4 : string.Empty;

    }

    /// <summary>
    /// 選択データのValue5取得
    /// </summary>
    /// <returns></returns>
    public string GetInputValue5()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Value5 : string.Empty;

    }

    /// <summary>
    /// 選択データのValue6取得
    /// </summary>
    /// <returns></returns>
    public string GetInputValue6()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Value6 : string.Empty;

    }

    /// <summary>
    /// 選択データのValue7取得
    /// </summary>
    /// <returns></returns>
    public string GetInputValue7()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Value7 : string.Empty;

    }

    /// <summary>
    /// 選択データのValue8取得
    /// </summary>
    /// <returns></returns>
    public string GetInputValue8()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Value8 : string.Empty;

    }

    /// <summary>
    /// 選択データのValue9取得
    /// </summary>
    /// <returns></returns>
    public string GetInputValue9()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Value9 : string.Empty;

    }

    /// <summary>
    /// 選択データのValue10取得
    /// </summary>
    /// <returns></returns>
    public string GetInputValue10()
    {
        List<ValueTextInfo> r = [];
        r = Data.Where(_ => _.Value == InputValue).ToList();
        return r.Count() > 0 ? r[0].Value10 : string.Empty;

    }
    #endregion

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

        //他コンポーネント名に紐付くプロパティ値をDynamicコンポーネント用パラメータに追加する
        AddParamComponets(retParam, components, info);
    }

    /// <summary>
    /// Changeイベント
    /// </summary>
    /// <param name="args"></param>
    private void OnChange(object args)
    {
        ChangeSelectValue?.Invoke(this, args);
    }
}

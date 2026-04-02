using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using SharedModels;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 日時(yyyy/MM/dd HH:mm:ss)検索条件用コンポーネント
/// </summary> 
public partial class CompDateTimePicker : CompBase
{
    private DateTime? _initialValue = null;
    /// <summary>
    /// 日時初期値(yyyyMMddHHmmss)
    /// </summary>
    [Parameter]
    public string InitialValue
    {
        get => _initialValue == null ? string.Empty : _initialValue?.ToString(ValueFormat)!;
        set => _initialValue = DateTime.TryParseExact(value, ValueFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dtVal)
                ? dtVal
                : DateTime.TryParseExact(value, DispFormat, null, System.Globalization.DateTimeStyles.None, out dtVal) ? dtVal : null;
    }
    private DateTime? _inputValue = null;
    /// <summary>
    /// 日時入力値(yyyyMMddHHmmss)
    /// </summary>
    [Parameter]
    public string? InputValue
    {
        get => _inputValue == null ? string.Empty : _inputValue?.ToString(ValueFormat)!;
        set => _inputValue = DateTime.TryParseExact(value, ValueFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dtVal)
                ? dtVal
                : DateTime.TryParseExact(value, DispFormat, null, System.Globalization.DateTimeStyles.None, out dtVal) ? dtVal : null;
    }
    //public IEnumerable<string> Values
    //{
    //    get
    //    {
    //        List<string> vals = [];
    //        if (InputValue is not null)
    //        {
    //            vals.Add(InputValue);
    //        }
    //        if (InputValueTo is not null)
    //        {
    //            vals.Add(InputValueTo);
    //        }
    //        return vals;
    //    }
    //}

    /// <summary>
    /// 値フォーマット
    /// </summary>
    [Parameter]
    public string ValueFormat { get; set; } = "yyyyMMddHHmmss";

    /// <summary>
    /// 表示フォーマット
    /// </summary>
    [Parameter]
    public string DispFormat { get; set; } = "yyyy/MM/dd HH:mm:ss";

    /// <summary>
    /// 時間入力 ON/OFF
    /// </summary>
    [Parameter]
    public bool TimeOnly { get; set; } = false;

    /// <summary>
    /// 時間表示 ON/OFF
    /// </summary>
    [Parameter]
    public bool ShowTime { get; set; } = true;

    /// <summary>
    /// 秒表示 ON/OFF
    /// </summary>
    [Parameter]
    public bool ShowSeconds { get; set; } = true;

    /// <summary>
    /// 時間増減ステップ
    /// </summary>
    [Parameter]
    public string HoursStep { get; set; } = "1";

    /// <summary>
    /// 分増減ステップ
    /// </summary>
    [Parameter]
    public string MinutesStep { get; set; } = "1";

    /// <summary>
    /// 秒増減ステップ
    /// </summary>
    [Parameter]
    public string SecondsStep { get; set; } = "1";

    /// <summary>
    /// クリアボタン ON/OFF
    /// </summary>
    [Parameter]
    public bool AllowClear { get; set; } = true;

    /// <summary>
    /// 初期化モード
    /// </summary>
    [Parameter]
    public enumDateInitMode InitMode { get; set; } = enumDateInitMode.None;

    /// <summary>
    /// WMS作業日
    /// </summary>
    [Parameter]
    public DateTime? WmsDate { get; set; } = null;

    /// <summary>
    /// WMS作業日加算
    /// </summary>
    [Parameter]
    public DateTime? WmsDateAdd { get; set; } = null;

    ///// <summary>
    ///// セパレータのスタイル
    ///// </summary>
    //[Parameter]
    //public string SeparatorStyle { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // システムパラメータ取得
        if (null != _sysParam)
        {
            CompFontSize = _sysParam.PC_DateTimePickerFontSize;
            CompFontWeight = _sysParam.PC_DateTimePickerFontWeight;
            CompWidth = _sysParam.PC_DateTimePickerWidth;
            CompHeight = _sysParam.PC_DateTimePickerHeight;
        }

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

        //// セパレータスタイル設定
        //SeparatorStyle += $"font-size:{CompFontSize}";
        //SeparatorStyle += $"font-weight:{CompFontWeight}";
    }

    /// <summary>
    /// 入力値のリセット
    /// </summary>
    public override void ResetValue()
    {
        // 初期値設定
        InputValue = InitialValue;

        // 入力値が無い場合、初期化モードによって初期値を設定する
        if (_inputValue == null )
        {
            DateTime now = DateTime.Now;
            switch (InitMode)
            {
                case enumDateInitMode.All:
                case enumDateInitMode.From:
                case enumDateInitMode.To:
                    _inputValue = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    break;
                case enumDateInitMode.AllWms:
                case enumDateInitMode.FromWms:
                case enumDateInitMode.ToWms:
                    if (WmsDate != null)
                    {
                        _inputValue = new DateTime(((DateTime)WmsDate).Year, ((DateTime)WmsDate).Month, ((DateTime)WmsDate).Day, 0, 0, 0);
                    }
                    break;
                case enumDateInitMode.AllWmsAdd:
                case enumDateInitMode.FromWmsAdd:
                case enumDateInitMode.ToWmsAdd:
                    if (WmsDateAdd != null)
                    {
                        _inputValue = new DateTime(((DateTime)WmsDateAdd).Year, ((DateTime)WmsDateAdd).Month, ((DateTime)WmsDateAdd).Day, 0, 0, 0);
                    }
                    break;
                default:
                    break;
            }
        }
        // 再レンダリング
        StateHasChanged();
    }

    /// <summary>
    /// 入力値クリア
    /// </summary>
    public override void ClearValue()
    {
        // 入力値クリア
        InputValue = null;
        // 再レンダリング
        StateHasChanged();
    }
    /// <summary>
    /// 入力表示値取得
    /// </summary>
    /// <returns></returns>
    public string GetDispInputValue()
    {
        return _inputValue == null ? string.Empty : _inputValue?.ToString(DispFormat)!;
    }
    public override void AddWhereParam(Dictionary<string, (object, WhereParam)> retWhereParam, string keyNmae)
    {
        if (!string.IsNullOrEmpty(InputValue))
        {
            retWhereParam[keyNmae] = (InputValue, new WhereParam {val = GetDispInputValue(), whereType = enumWhereType.Below });
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
            string str = InputValue ?? string.Empty;
            retKeyValuePairs[keyName] = str;
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
            retParam.Add("InitialValue",initVal);
        }
        retParam.Add("InitMode", initMode);
        if (dtWms is not null)
        {
            retParam.Add("WmsDate", dtWms);
        }
        if (dtWmsAdd is not null)
        {
            retParam.Add("WmsDateAdd", dtWmsAdd);
        }
        //他コンポーネント名に紐付くプロパティ値をDynamicコンポーネント用パラメータに追加する
        AddParamComponets(retParam, components, info);
    }
}

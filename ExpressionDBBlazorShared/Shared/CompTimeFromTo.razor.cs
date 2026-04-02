using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using SharedModels;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 時間(HH:mm)検索条件用コンポーネント
/// </summary>
public partial class CompTimeFromTo : CompBase
{
    private DateTime? _initialValueFrom = null;
    /// <summary>
    /// 時間From初期値(HHmm)
    /// </summary>
    [Parameter]
    public string InitialValueFrom
    {
        get => _initialValueFrom == null ? string.Empty : _initialValueFrom?.ToString(ValueFormat)!;
        set => _initialValueFrom = DateTime.TryParseExact(value, ValueFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dtVal)
                ? dtVal
                : DateTime.TryParseExact(value, DispFormat, null, System.Globalization.DateTimeStyles.None, out dtVal) ? dtVal : null;
    }

    private DateTime? _initialValueTo = null;
    /// <summary>
    /// 時間To初期値(HHmm)
    /// </summary>
    [Parameter]
    public string InitialValueTo
    {
        get => _initialValueTo == null ? string.Empty : _initialValueTo?.ToString(ValueFormat)!;
        set => _initialValueTo = DateTime.TryParseExact(value, ValueFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dtVal)
                ? dtVal
                : DateTime.TryParseExact(value, DispFormat, null, System.Globalization.DateTimeStyles.None, out dtVal) ? dtVal : null;
    }

    private DateTime? _inputValueFrom = null;
    /// <summary>
    /// 時間From入力値(HHmm)
    /// </summary>
    [Parameter]
    public string? InputValueFrom
    {
        get => _inputValueFrom == null ? string.Empty : _inputValueFrom?.ToString(ValueFormat)!;
        set => _inputValueFrom = DateTime.TryParseExact(value, ValueFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dtVal)
                ? dtVal
                : DateTime.TryParseExact(value, DispFormat, null, System.Globalization.DateTimeStyles.None, out dtVal) ? dtVal : null;
    }

    private DateTime? _inputValueTo = null;
    /// <summary>
    /// 時間From入力値(HHmm)
    /// </summary>
    [Parameter]
    public string? InputValueTo
    {
        get => _inputValueTo == null ? string.Empty : _inputValueTo?.ToString(ValueFormat)!;
        set => _inputValueTo = DateTime.TryParseExact(value, ValueFormat, null, System.Globalization.DateTimeStyles.None, out DateTime dtVal)
                ? dtVal
                : DateTime.TryParseExact(value, DispFormat, null, System.Globalization.DateTimeStyles.None, out dtVal) ? dtVal : null;
    }

    public IEnumerable<string> Values
    {
        get
        {
            List<string> vals = [];
            if (InputValueFrom is not null)
            {
                vals.Add(InputValueFrom);
            }
            if (InputValueTo is not null)
            {
                vals.Add(InputValueTo);
            }
            return vals;
        }
    }

    /// <summary>
    /// 値フォーマット
    /// </summary>
    [Parameter]
    public string ValueFormat { get; set; } = "HHmm";

    /// <summary>
    /// 表示フォーマット
    /// </summary>
    [Parameter]
    public string DispFormat { get; set; } = "HH:mm";

    /// <summary>
    /// 時間入力 ON/OFF
    /// </summary>
    [Parameter]
    public bool TimeOnly { get; set; } = true;

    /// <summary>
    /// 時間表示 ON/OFF
    /// </summary>
    [Parameter]
    public bool ShowTime { get; set; } = true;

    /// <summary>
    /// 秒表示 ON/OFF
    /// </summary>
    [Parameter]
    public bool ShowSeconds { get; set; } = false;

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

    /// <summary>
    /// セパレータのスタイル
    /// </summary>
    [Parameter]
    public string SeparatorStyle { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // システムパラメータ取得
        if (null != _sysParam)
        {
            CompFontSize = _sysParam.PC_TimePickerFontSize;
            CompFontWeight = _sysParam.PC_TimePickerFontWeight;
            CompWidth = _sysParam.PC_TimePickerWidth;
            CompHeight = _sysParam.PC_TimePickerHeight;
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

        // セパレータスタイル設定
        SeparatorStyle += $"font-size:{CompFontSize}";
        SeparatorStyle += $"font-weight:{CompFontWeight}";
    }

    /// <summary>
    /// 入力値のリセット
    /// </summary>
    public override void ResetValue()
    {
        // 初期値設定
        InputValueFrom = InitialValueFrom;
        InputValueTo = InitialValueTo;

        // 入力値が無い場合、初期化モードによって初期値を設定する
        if (_inputValueFrom == null && _inputValueTo == null)
        {
            DateTime now = DateTime.Now;
            switch (InitMode)
            {
                case enumDateInitMode.All:
                    _inputValueFrom = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    _inputValueTo = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
                    break;
                case enumDateInitMode.From:
                    _inputValueFrom = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    break;
                case enumDateInitMode.To:
                    _inputValueTo = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
                    break;
                case enumDateInitMode.AllWms:
                    if (WmsDate != null)
                    {
                        _inputValueFrom = new DateTime(((DateTime)WmsDate).Year, ((DateTime)WmsDate).Month, ((DateTime)WmsDate).Day, 0, 0, 0);
                        _inputValueTo = new DateTime(((DateTime)WmsDate).Year, ((DateTime)WmsDate).Month, ((DateTime)WmsDate).Day, 23, 59, 59);
                    }
                    break;
                case enumDateInitMode.FromWms:
                    if (WmsDate != null)
                    {
                        _inputValueFrom = new DateTime(((DateTime)WmsDate).Year, ((DateTime)WmsDate).Month, ((DateTime)WmsDate).Day, 0, 0, 0);
                    }
                    break;
                case enumDateInitMode.ToWms:
                    if (WmsDate != null)
                    {
                        _inputValueTo = new DateTime(((DateTime)WmsDate).Year, ((DateTime)WmsDate).Month, ((DateTime)WmsDate).Day, 23, 59, 59);
                    }
                    break;
                case enumDateInitMode.AllWmsAdd:
                    if (WmsDateAdd != null)
                    {
                        _inputValueFrom = new DateTime(((DateTime)WmsDateAdd).Year, ((DateTime)WmsDateAdd).Month, ((DateTime)WmsDateAdd).Day, 0, 0, 0);
                        _inputValueTo = new DateTime(((DateTime)WmsDateAdd).Year, ((DateTime)WmsDateAdd).Month, ((DateTime)WmsDateAdd).Day, 23, 59, 59);
                    }
                    break;
                case enumDateInitMode.FromWmsAdd:
                    if (WmsDateAdd != null)
                    {
                        _inputValueFrom = new DateTime(((DateTime)WmsDateAdd).Year, ((DateTime)WmsDateAdd).Month, ((DateTime)WmsDateAdd).Day, 0, 0, 0);
                    }
                    break;
                case enumDateInitMode.ToWmsAdd:
                    if (WmsDateAdd != null)
                    {
                        _inputValueTo = new DateTime(((DateTime)WmsDateAdd).Year, ((DateTime)WmsDateAdd).Month, ((DateTime)WmsDateAdd).Day, 23, 59, 59);
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
        InputValueFrom = null;
        InputValueTo = null;
        // 再レンダリング
        StateHasChanged();
    }

    /// <summary>
    /// From入力表示値取得
    /// </summary>
    /// <returns></returns>
    public string GetDispInputValueFrom()
    {
        return _inputValueFrom == null ? string.Empty : _inputValueFrom?.ToString(DispFormat)!;
    }

    /// <summary>
    /// To入力表示値取得
    /// </summary>
    /// <returns></returns>
    public string GetDispInputValueTo()
    {
        return _inputValueTo == null ? string.Empty : _inputValueTo?.ToString(DispFormat)!;
    }
    public override void AddWhereParam(Dictionary<string, (object, WhereParam)> retWhereParam, string keyNmae)
    {
        if (!string.IsNullOrEmpty(InputValueFrom))
        {
            retWhereParam[keyNmae + "From"] = (InputValueFrom, new WhereParam { field = keyNmae, val = GetDispInputValueFrom(), whereType = enumWhereType.Above });
        }
        if (!string.IsNullOrEmpty(InputValueTo))
        {
            retWhereParam[keyNmae + "To"] = (InputValueTo, new WhereParam { field = keyNmae, val = GetDispInputValueTo(), whereType = enumWhereType.Below });
        }
    }
    public override void SetInitValue(object? initVal)
    {
        if (initVal is not null)
        {
            InputValueFrom = ((List<string>)initVal)[0];
            InputValueTo = ((List<string>)initVal)[1];
        }
    }
    public override void AddkeyValuePair(Dictionary<string, object> retKeyValuePairs, string keyName, bool bnGetEmpty)
    {

        if (!string.IsNullOrEmpty(InputValueFrom) || !string.IsNullOrEmpty(InputValueTo))
        {
            string strFrom = InputValueFrom ?? string.Empty;
            string strTo = InputValueTo ?? string.Empty;
            retKeyValuePairs[keyName] = new List<string>() { strFrom, strTo };
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
                retParam.Add("InitialValueFrom", ((List<string>)initVal)[0]);
                retParam.Add("InitialValueTo", ((List<string>)initVal)[1]);
            }
            else
            {
                retParam.Add("InitialValueFrom", initVal.ToString().Replace("/", "").Replace(":", "").Replace(" ", ""));
                retParam.Add("InitialValueTo", initVal.ToString().Replace("/", "").Replace(":", "").Replace(" ", ""));
            }
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

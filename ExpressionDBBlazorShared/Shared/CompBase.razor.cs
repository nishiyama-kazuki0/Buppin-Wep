using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Util;
using Microsoft.AspNetCore.Components;
using SharedModels;
using System.Reflection;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 日付関連コンポーネントの初期化モード
/// </summary>
public enum enumDateInitMode
{
    /// <summary>初期化なし</summary>
    None,
    /// <summary>全てシステム日付</summary>
    All,
    /// <summary>Fromのみシステム日付</summary>
    From,
    /// <summary>Toのみシステム日付</summary>
    To,
    /// <summary>全てWMS作業日</summary>
    AllWms,
    /// <summary>FromのみWMS作業日</summary>
    FromWms,
    /// <summary>FromのみWMS作業日</summary>
    ToWms,
    /// <summary>全てWMS作業日加算</summary>
    AllWmsAdd,
    /// <summary>FromのみWMS作業日加算</summary>
    FromWmsAdd,
    /// <summary>FromのみWMS作業日加算</summary>
    ToWmsAdd,
}

public abstract partial class CompBase : ComponentBase
{
    [Inject]
    private ISessionStorageService _sessionStorage { get; set; } = null!;

    protected SystemParameterService _sysParam => SystemParamService;

    /// <summary>
    /// 有効無効
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; } = false;

    public string _Style = string.Empty;
    /// <summary>
    /// コンポーネントに設定するStyle
    /// </summary>
    [Parameter]
    public string Style
    {
        get => (Disabled ? "background-color:var(--rz-input-disabled-background-color);color:var(--rz-input-disabled-color);" : "") + $"{_Style}";
        set => _Style = value;
    }

    /// <summary>
    /// コンポーネントに設定するClass
    /// </summary>
    [Parameter]
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// タイトル
    /// </summary>
    [Parameter]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 必須フラグ
    /// </summary>
    [Parameter]
    public bool Required { get; set; } = false;

    /// <summary>
    /// コンポーネントのフォントサイズ
    /// </summary>
    [Parameter]
    public string CompFontSize { get; set; } = "100%";

    /// <summary>
    /// コンポーネントのフォント幅
    /// </summary>
    [Parameter]
    public string CompFontWeight { get; set; } = "normal";

    /// <summary>
    /// コンポーネント幅
    /// </summary>
    [Parameter]
    public string CompWidth { get; set; } = string.Empty;

    /// <summary>
    /// コンポーネント高さ
    /// </summary>
    [Parameter]
    public string CompHeight { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // 入力値リセット
        ResetValue();
    }

    protected override bool ShouldRender()
    {
        //true:レンダリング可能,false : レンダリング不可
        return !ChildBaseService.BasePageInitilizing;
    }

    /// <summary>
    /// 入力値のリセット
    /// </summary>
    public virtual void ResetValue()
    {
        //継承コンポーネントでオーバーライドする
    }

    /// <summary>
    /// 入力値のクリア
    /// </summary>
    public virtual void ClearValue()
    {
        //継承コンポーネントでオーバーライドする
    }

    /// <summary>
    /// 再描画
    /// </summary>
    public void Refresh()
    {
        StateHasChanged();
    }

    /// <summary>
    /// DynamicComponentに渡すパラメータを追加する。DEFINE_COMPONENTSに登録している値を追加する処理
    /// </summary>
    /// <param name="retParam"></param>
    /// <param name="components"></param>
    /// <param name="info"></param>
    protected virtual void AddParamComponets(
        Dictionary<string, object> retParam
        , IList<ComponentsInfo> components
        , ComponentColumnsInfo info
        )
    {
        //TODO 以下類似の処理が散見されるので、他のにstaticメソッドを作成して共通化する
        //他コンポーネント名に紐付くプロパティ値をDynamicコンポーネント用パラメータに追加する
        //自分のコンポーネントかどうかどうかはコンポーネントクラス名+カラムinfoのプロパティキー名で特定する。
        //したがって、DBテーブル内のComponennt名はコンポーネントクラス名+カラムinfoのプロパティキー名で格納しておくこと
        foreach (ComponentsInfo? c in components.Where(_ => _.ComponentName == GetType().Name + info.Property))
        {
            switch (c.ValueObjectType)
            {
                case (int)ComponentsInfo.EnumValueObjectType.ValueIndicator:
                    // 値をデータ型より変換
                    retParam.Add(c.AttributesName, Convert.ChangeType(c.Value, c.Type));
                    break;
                case (int)ComponentsInfo.EnumValueObjectType.VariableIndicator:
                    // 変数文字列から変数を取得
                    Type type = c.Type;
                    if (null == type)
                    {
                        type = typeof(ChildPageBase);
                    }
                    PropertyInfo? pi = type.GetProperty(c.Value, BindingFlags.NonPublic | BindingFlags.Instance);
                    if (pi != null)
                    {
                        object? v = pi.GetValue(this, null);
                        if (v is not null)
                        {
                            retParam.Add(c.AttributesName, v);
                        }

                    }
                    break;
                case (int)ComponentsInfo.EnumValueObjectType.EnumStringIndicator:
                    // Enumを文字列から値に変換
                    string strEnumStr = c.Value;
                    string strEnumStrPos = strEnumStr[(strEnumStr.LastIndexOf('.') + 1)..];
                    object? value = typeof(ConvertUtil).GetMethod("GetEnumValue")!.MakeGenericMethod(c.Type).Invoke(null, new object[] { strEnumStrPos });
                    if (value is not null)
                    {
                        retParam.Add(c.AttributesName, value);
                    }

                    break;
                case (int)ComponentsInfo.EnumValueObjectType.ClassNameIndicator:
                    retParam.Add(c.AttributesName, c.Type);
                    break;
            }
        }
    }

    /// <summary>
    /// WhereParamを追加する
    /// パフォーマンス向上のため、追加したいオブジェクト(retWhereParam)を引数で渡し追加していく方針をとっている
    /// </summary>
    public abstract void AddWhereParam(Dictionary<string, (object, WhereParam)> retWhereParam, string keyName);
    /// <summary>
    /// InitValをセットする
    /// </summary>
    /// <param name="initVal"></param>
    public abstract void SetInitValue(object? initVal);
    /// <summary>
    /// keyValuepairを追加する
    /// パフォーマンス向上のため、追加したいオブジェクト(retKeyValuePairs)を引数で渡し追加していく方針をとる
    /// </summary>
    /// <param name="retkeyValuePairs"></param>
    /// <param name="bnGetEmpty"></param>
    public abstract void AddkeyValuePair(Dictionary<string, object> retKeyValuePairs, string keyName, bool bnGetEmpty);
    //TODO 一旦抽象メソッド化したが、使わない引数などがあり、微妙。
    /// <summary>
    /// パラメータを追加する。
    /// パフォーマンス向上のため、追加したいオブジェクト(retParam)を引数で渡し追加していく方針をとる
    /// </summary>
    /// <param name="retParam"></param>
    /// <param name="info"></param>
    /// <param name="ComponentColumns"></param>
    /// <param name="initVal"></param>
    /// <param name="data"></param>
    /// <param name="bnDispTitle"></param>
    /// <param name="initMode"></param>
    /// <param name="dtWms"></param>
    /// <param name="dtWmsAdd"></param>
    /// <param name="components">渡される時は呼び出し元画面のCLASS_NAMEで絞られているとする</param>
    public abstract void AddParam(
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
        );
}

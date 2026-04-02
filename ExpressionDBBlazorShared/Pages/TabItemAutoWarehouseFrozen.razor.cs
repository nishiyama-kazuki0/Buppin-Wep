using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Shared;
using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// 自動倉庫状況(冷凍)
/// </summary>
public partial class TabItemAutoWarehouseFrozen : TabItemBase
{
    /// <summary>
    /// 画面表示用に使用するコンポーネントを定義
    /// </summary>
    public const string STR_COLOR_PARAM = "ColorParam";

    [Inject]
    protected CustomForSuntoryService CstService { get; set; } = null!;

    /// <summary>
    /// 色説明の表示非表示
    /// </summary>
    public bool VisibleColor { get; set; } = false;

    /// <summary>
    /// 色説明マークの幅
    /// </summary>
    public string ColorWidth { get; set; } = "60px";

    /// <summary>
    /// 色説明マークの高さ
    /// </summary>
    public string ColorHeight { get; set; } = "30px";

    /// <summary>
    /// ロケーション区分
    /// </summary>
    protected IList<LocationTypesInfo> _locationTypes { get; set; } = [];

    /// <summary>
    /// コンポーネントが初期化されるときに呼び出されます。非同期で実行できるものはAsyncを付けます。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        //ロケーション区分を取得
        _locationTypes = await CstService.GetLocationTypesInfoAsync();

        await base.OnInitializedAsync();
    }

    /// <summary>
    /// attributes情報初期化
    /// </summary>
    /// <returns></returns>
    protected override async Task InitAttributesAsync()
    {
        await base.InitAttributesAsync();

        //画面表示用のパラメータ取得
        IDictionary<string, object> colorParam = GetAttributes(STR_COLOR_PARAM);

        object? value;
        if (colorParam.TryGetValue(nameof(VisibleColor), out value))
        {
            VisibleColor = bool.Parse(value.ToString()!);
        }
        if (colorParam.TryGetValue(nameof(ColorWidth), out value))
        {
            ColorWidth = $"{value.ToString()!}px";
        }
        if (colorParam.TryGetValue(nameof(ColorHeight), out value))
        {
            ColorHeight = $"{value.ToString()!}px";
        }
    }

}

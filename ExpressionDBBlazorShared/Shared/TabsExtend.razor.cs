using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// ステップ画面用UIコンポーネント
/// </summary>
public partial class TabsExtend : ComponentBase
{
    [Inject]
    private ISessionStorageService _sessionStorage { get; set; } = null!;

    #region パラメータ
    /// <summary>
    /// タブ画面用クラスリスト
    /// </summary>
    [Parameter]
    public List<TabItemInfo> TabItems { get; set; } = [];
    /// <summary>
    /// メインページ
    /// </summary>
    [Parameter]
    public required ChildPageBase Parent { get; set; }
    /// <summary>
    /// フォントサイズ
    /// </summary>
    [Parameter]
    public string TabsExtendFontSize { get; set; } = "100%";

    /// <summary>
    /// フォント幅
    /// </summary>
    [Parameter]
    public string TabsExtendFontWeight { get; set; } = "normal";
    #endregion

    // RadzenTabsのTabs
    private RenderFragment? tabs;
    public required RadzenTabs radzenTabs;

    /// <summary>
    /// ステップ位置
    /// </summary>
    /// <returns></returns>
    public int SelectedIndex => radzenTabs.SelectedIndex;

    #region OnInitializedAsync
    /// <summary>
    /// 初期処理
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        tabs = new RenderFragment(builder =>
        {
            int seq = 0;
            for (int i = 0; TabItems.Count > i; i++)
            {
                int ii = i;
                builder.OpenComponent<RadzenTabsItem>(seq++);
                builder.AddAttribute(seq++, "Text", TabItems[i].Title);
                builder.AddAttribute(seq++, "ChildContent", new RenderFragment(builder2 =>
                {
                    builder2.OpenComponent(0, TabItems[ii].TabItem.GetType());
                    builder2.AddAttribute(1, "TabsExtend", this);
                    builder2.CloseComponent();
                }));
                builder.CloseComponent();
            }
        });

        SystemParameterService sysParams = SystemParamService;
        TabsExtendFontSize = sysParams.PC_TabsExtendFontSize;
        TabsExtendFontWeight = sysParams.PC_TabsExtendFontWeight;

        await base.OnInitializedAsync();
    }
    #endregion
}

/// <summary>
/// ステップ用データクラス
/// </summary>
public class TabItemInfo
{
    // ステップタイトル文字列
    public string Title { get; set; } = "";
    // ステップ用UIコンポーネント
    public TabItemBase TabItem { get; set; } = new TabItemBase();
}

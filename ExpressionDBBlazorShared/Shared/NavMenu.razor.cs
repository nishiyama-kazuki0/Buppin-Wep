using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Data;
using Microsoft.AspNetCore.Components;
using SharedModels;

namespace ExpressionDBBlazorShared.Shared;
public partial class NavMenu
{
    [Inject]
    private ISessionStorageService? _sessionStorage { get; set; }

    //引数
    /// <summary>
    /// 検索テキストの表示有無
    /// </summary>
    [Parameter]
    public bool IsVisibleSearchText { get; set; } = true;

    /// <summary>
    /// HT端末かどうか
    /// </summary>
    [Parameter]
    public bool IsHandy { get; set; } = false;

    /// <summary>
    /// メニューオブジェクトリスト
    /// </summary>
    private List<MenuInfo> MenuItems = [];

    /// <summary>
    /// 初期表示イベント
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        await GetMenuItems();
    }

    /// <summary>
    /// ツールチップメッセージを表示する
    /// </summary>
    /// <param name="elementReference"></param>
    /// <param name="toolTipMessage"></param>
    /// <param name="options"></param>
    public void ShowTooltip(ElementReference elementReference, string toolTipMessage, TooltipOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(toolTipMessage))
        {
            return;
        }

        TooltipService.Open(elementReference, toolTipMessage, options);
    }

    /// <summary>
    /// メニュー一覧を取得する
    /// </summary>
    /// <returns></returns>
    private async Task GetMenuItems()
    {
        if (null != _sessionStorage && await _sessionStorage.ContainKeyAsync(SharedConst.KEY_LOGIN_INFO))
        {
            // ログインユーザIDが、半角スペースの場合のみ、PC、HT両方のメニューを表示する
            LoginInfo login = await _sessionStorage.GetItemAsync<LoginInfo>(SharedConst.KEY_LOGIN_INFO);
            MenuItems = await ComService.GetMenuInfoListNavAsync(IsHandy, login.AllFeatureEnable);
        }
        else
        {
            MenuItems = await ComService.GetMenuInfoListNavAsync(IsHandy);
        }
    }

    /// <summary>
    /// メニュー検索テキストから該当メニュー名を絞り込む(曖昧検索)
    /// </summary>
    /// <returns></returns>
    private async Task ChangeSearchText(string searchMenuText)
    {
        //一度すべてのメニューを取得しなおす
        await GetMenuItems();
        if (MenuItems is null || string.IsNullOrWhiteSpace(searchMenuText))
        {
            return;
        }
        //検索テキストに含まれているメニューを表示する
        List<MenuInfo> mInfos = MenuItems.Where(_ => _.menuName.Contains(searchMenuText)
        || _.subMenuList.Any(_ => _.menuName.Contains(searchMenuText))
        ).ToList();
        foreach (MenuInfo mInfo in mInfos)
        {
            List<MenuInfo> msl = mInfo.subMenuList.Where(_ => _.menuName.Contains(searchMenuText)).ToList();
            if (msl.Any())
            {
                mInfo.subMenuList = msl;
            }
        }
        if (mInfos.Any())
        {
            MenuItems = mInfos;
        }
        else
        {
            MenuItems.Clear();
        }
    }
}

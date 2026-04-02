using ExpressionDBBlazorShared.Shared;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// インデックス
/// </summary>
public partial class Index : ChildPageBasePC
{
    protected override async Task OnInitializedAsync()
    {
        //HTログインの場合は、親の初期化イベント呼び出さないとする。
        //HTメニューへのNavigate遷移の兼ね合いで、ファンクションボタンが表示されないときが発生したため、直接原因ではないかもしれないが、対策。
        if (!DeviceInfo.IsHandy())
        {
            await base.OnInitializedAsync();
        }
    }
}

using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Shared;

public partial class TabItemBase : ChildPageBasePC
{
    /// <summary>
    /// タブ管理UIコンポーネント
    /// </summary>
    [Parameter]
    public TabsExtend? TabsExtend { get; set; }
}

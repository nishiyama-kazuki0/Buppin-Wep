using Microsoft.AspNetCore.Components;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    public partial class TabItemBase : ChildPageBasePC
    {
        /// <summary>
        /// タブ管理UIコンポーネント
        /// </summary>
        [Parameter]
        public TabsExtend? TabsExtend { get; set; }
    }
}

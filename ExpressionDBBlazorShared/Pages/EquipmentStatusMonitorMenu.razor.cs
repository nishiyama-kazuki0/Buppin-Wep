using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Shared;
using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Pages;
/// <summary>
/// 設備状況監視画面
/// </summary>
public partial class EquipmentStatusMonitorMenu : ChildPageBasePC
{
    public override void 画面クリア(ComponentProgramInfo info)
    {
        base.画面クリア(info);
        // クリア
        Attributes[STR_ATTRIBUTE_GRID]["HeaderTextValue"] = string.Empty;
    }
    //protected override void OnInitialized()
    //{
    //        //_CustomDataGrid = true;
    //}
}
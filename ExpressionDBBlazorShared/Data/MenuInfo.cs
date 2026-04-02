namespace ExpressionDBBlazorShared.Data;

public class MenuInfo
{
    public string menuId { get; set; } = string.Empty;
    public string menuName { get; set; } = string.Empty;
    public string menuUrlString { get; set; } = string.Empty;
    public string iconName { get; set; } = string.Empty;
    public string ToolTipMessage { get; set; } = string.Empty;
    public int dispOrder { get; set; } = 0;
    public string className { get; set; } = string.Empty;
    public string parentMenuId { get; set; } = string.Empty;
    public int DispDivision { get; set; } = 0;
    public List<MenuInfo> subMenuList = [];
}

using System.ComponentModel.DataAnnotations.Schema;

namespace ExpressionDBWebAPI.Data;

/// <summary>
/// データベースモデル　DEFINE_PAGE_VALUES
/// </summary>
public class DEFINE_PAGE_VALUES_Model
{
    [Column("CLASS_NAME")]
    public string CLASS_NAME { get; set; } = string.Empty;
    [Column("PAGE_NAME")]
    public string PAGE_NAME { get; set; } = string.Empty;
    [Column("STEP_INDEX_NUM")]
    public int STEP_INDEX_NUM { get; set; }
    [Column("COMPONENT_NAME")]
    public string COMPONENT_NAME { get; set; } = string.Empty;
    [Column("VIEW_NAME")]
    public string VIEW_NAME { get; set; } = string.Empty;
    [Column("URL")]
    public string URL { get; set; } = string.Empty;
    [Column("CALLER_MENU_ID")]
    public string CALLER_MENU_ID { get; set; } = string.Empty;
}

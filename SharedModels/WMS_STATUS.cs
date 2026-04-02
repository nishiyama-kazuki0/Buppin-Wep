using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

/// <summary>
/// VIEWモデル
/// </summary>
public class WMS_STATUS_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "WMS_STATUS";

    /// <summary>
    /// システム状態区分
    /// </summary>
    [Column("SYSTEM_STATUS_TYPE")]
    public int SystemStatusType { get; set; } = 9;
}

using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

public class VW_COMMAND_Model : AbstractViewModelBaseExtension
{
    /// <summary>
    /// VIEW名
    /// </summary>
    public const string VIEW_NAME = "VW_COMMAND";

    [Column("ID")]
    public int Id { get; set; }

    [Column("サブコマンドID")]
    public int SubCommandId { get; set; }

    [Column("グループID")]
    public int GroupId { get; set; }

    [Column("グループ連番")]
    public int GroupSeq { get; set; }

    [Column("種別")]
    public int Type { get; set; }

    [Column("PARAM01")]
    public string Parameter01 { get; set; } = string.Empty;

    [Column("PARAM02")]
    public string Parameter02 { get; set; } = string.Empty;

    [Column("PARAM03")]
    public string Parameter03 { get; set; } = string.Empty;

    /// <summary>種別</summary>
    public enum CommandType
    {
        /// <summary>デバイス操作</summary>
        Device = 0,
        /// <summary>シーケンスステップ変更</summary>
        StepChange = 1 
    }
}

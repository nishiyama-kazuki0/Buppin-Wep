using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

public class CycleProcessInfo
{
    [Column("CYCLE_ID")]
    public int CYCLE_ID { get; set; }
    [Column("CATEGORY")]
    public int CATEGORY { get; set; }
    [Column("PROCESS_PROGRAM_NAME")]
    public string PROCESS_PROGRAM_NAME { get; set; } = string.Empty;
    [Column("SORT_ORDER")]
    public int SORT_ORDER { get; set; } = 0;
    [Column("START_DATETIME")]
    public DateTime START_DATETIME { get; set; }
    [Column("INTERVAL")]
    public int INTERVAL { get; set; } = 0;
    [Column("IS_PROCESSING")]
    public bool IS_PROCESSING { get; set; } = false;
    [Column("OBSERVE_ID")]
    public int OBSERVE_ID { get; set; } = 0;
    [Column("IS_EXCLUSIVE")]
    public bool IS_EXCLUSIVE { get; set; } = false;
    [Column("TARGET_TABLE_NAME")]
    public string TARGET_TABLE_NAME { get; set; } = string.Empty;
    [Column("TARGET_COLUMNS_NUM")]
    public int TARGET_COLUMNS_NUM { get; set; }
    [Column("TARGET_PATH")]
    public string TARGET_PATH { get; set; } = string.Empty;
    [Column("BACK_UP_PATH")]
    public string BACK_UP_PATH { get; set; } = string.Empty;
    [Column("TARGET_FILE_NAME")]
    public string TARGET_FILE_NAME { get; set; } = string.Empty;
    [Column("SEPSTR")]
    public string SEPSTR { get; set; } = string.Empty;
    [Column("HEADER_ENABLE")]
    public bool HEADER_ENABLE { get; set; } = false;
}
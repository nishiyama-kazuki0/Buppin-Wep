using System.ComponentModel.DataAnnotations.Schema;

namespace ExpressionDBWebAPI.Data;

/// <summary>
/// データベースモデル　MST_DEVICES
/// </summary>
public class MST_DEVICES_Model
{
    [Column("DEVICE_ID")]
    public string DEVICE_ID { get; set; } = string.Empty;
    [Column("DEVICE_TYPE")]
    public byte DEVICE_TYPE { get; set; }
    [Column("DEVICE_NAME")]
    public string DEVICE_NAME { get; set; } = string.Empty;
    [Column("IP")]
    public string IP { get; set; } = string.Empty;
    [Column("PORT")]
    public decimal PORT { get; set; }
    [Column("PRINTER_ID")]
    public string PRINTER_ID { get; set; } = string.Empty;
    [Column("REGIST_DATETIME")]
    public string REGIST_DATETIME { get; set; } = string.Empty;
    [Column("REGIST_DEVICE_ID")]
    public string REGIST_DEVICE_ID { get; set; } = string.Empty;
    [Column("REGIST_PNAME")]
    public string REGIST_PNAME { get; set; } = string.Empty;
    [Column("REGIST_USER_ID")]
    public string REGIST_USER_ID { get; set; } = string.Empty;
    [Column("UPDATE_DATETIME")]
    public string UPDATE_DATETIME { get; set; } = string.Empty;
    [Column("UPDATE_DEVICE_ID")]
    public string UPDATE_DEVICE_ID { get; set; } = string.Empty;
    [Column("UPDATE_PNAME")]
    public string UPDATE_PNAME { get; set; } = string.Empty;
    [Column("UPDATE_USER_ID")]
    public string UPDATE_USER_ID { get; set; } = string.Empty;
}

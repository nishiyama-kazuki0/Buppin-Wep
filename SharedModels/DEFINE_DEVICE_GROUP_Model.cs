using System.ComponentModel.DataAnnotations.Schema;

namespace SharedModels;

public class DEFINE_DEVICE_GROUP_Model : AbstractViewModelBase
{
    [Column("DEVICE_TYPE")]
    public int DEVICE_TYPE { get; set; }
    [Column("DEVICE_TYPE_NAME")]
    public string DEVICE_TYPE_NAME { get; set; } = null!;
    [Column("JUDGE_STRING")]
    public string JUDGE_STRING { get; set; } = null!;
    [Column("DEVICE_TYPE_GROUP_ID")]
    public int DEVICE_TYPE_GROUP_ID { get; set; }
    [Column("SORT_ORDER")]
    public int SORT_ORDER { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace ExpressionDBWebAPI.Data;

//TODO ファイルを分けて継承する。ColumnsDefineクラスを継承するようなクラスにすることが理想と考える
/// <summary>
/// SQLSERVERパラメータの情報を格納するEntityModelクラス。
/// </summary>
internal class ParameterTypeModel
{
    [Column("name")]
    public string name { get; set; } = null!;
    [Column("type")]
    public string type { get; set; } = null!;
    [Column("max_length")]
    public int max_length { get; set; }
    [Column("precision")]
    public int precision { get; set; }
    [Column("is_output")]
    public bool is_output { get; set; }
    [Column("parameter_id")]
    public int parameter_id { get; set; }
    [Column("is_user_defined")]
    public bool is_user_defined { get; set; }
}

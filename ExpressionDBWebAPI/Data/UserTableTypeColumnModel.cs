using System.ComponentModel.DataAnnotations.Schema;

namespace ExpressionDBWebAPI.Data;

//TODO ファイルを分けて継承する。ColumnsDefineクラスを継承するようなクラスにすることが理想と考える
/// <summary>
/// SQLSERVERユーザー定義テーブル型のカラム情報を格納するEntityModelクラス
/// </summary>
internal class UserTableTypeColumnModel
{
    [Column("ColumnName")]
    public string ColumnName { get; set; } = null!;
    [Column("DataType")]
    public string DataType { get; set; } = null!;
    [Column("MaxLength")]
    public int MaxLength { get; set; }
    [Column("Precision")]
    public int Precision { get; set; }
    [Column("Scale")]
    public int Scale { get; set; }
    [Column("IsNullable")]
    public bool IsNullable { get; set; } = false;
    [Column("ColumnId")]
    public int column_id { get; set; }
}

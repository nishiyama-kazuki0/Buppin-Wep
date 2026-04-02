using ExpressionDBWebAPI.Common;
using System.Data;

namespace ExpressionDBWebAPI.Data;

public class ColumnsDefine
{
    public string ColumnName { get; set; }
    public DbType DataType { get; set; }
    public string DataTypeStr { get; set; }
    public int MaxLength { get; set; }
    public int Precision { get; set; }
    public int Scale { get; set; }
    public bool IsNullable { get; set; }
    public ColumnsDefine()
    {
        ColumnName = "";
    }

    public void SetDictonary(IDictionary<string, object> dic)
    {
        Type t = GetType();
        int intVal;
        foreach (System.Reflection.PropertyInfo f in t.GetProperties())
        {
            if (dic.TryGetValue(f.Name, out object? val) == false)
            {
                continue;
            }
            if (val is null)
            {
                continue;
            }
            switch (f.Name)
            {
                case nameof(ColumnName):
                    ColumnName = val.ToString() ?? "";
                    break;
                case nameof(DataType):
                    DataTypeStr = val.ToString() ?? "";
                    DataType = DataSource.GetDbType(val.ToString() ?? ""); //TODO 必要か検討。なくせる場合はDataSource、IDataSourceからも削除する。
                    break;
                case nameof(MaxLength):
                    _ = int.TryParse(val.ToString(), out intVal);
                    MaxLength = intVal;
                    break;
                case nameof(Precision):
                    _ = int.TryParse(val.ToString(), out intVal);
                    Precision = intVal;
                    break;
                case nameof(Scale):
                    _ = int.TryParse(val.ToString(), out intVal);
                    Scale = intVal;
                    break;
                case nameof(IsNullable):
                    _ = bool.TryParse(val.ToString(), out bool boolVal);
                    IsNullable = boolVal;
                    break;
                default:
                    break;
            }
        }
    }
}

using System.Text;

namespace ExpressionDBCycleProcessApp.Maintenance;

public class FieldSummary
{
    public string[] Para { get; set; }
    /// <summary></summary>
    public string SummaryAll {
        get {
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(Summary))
            {
                sb.Append(Summary);
            }
            foreach (var r in Para)
            {
                sb.AppendLine();
                sb.Append(r);
            }
            return sb.ToString();
        } 
    }
    public string Summary { get; set; }
    /// <summary></summary>
    public string Name2 => (Para != null && Para.Length >= 1) ? Para[0] : "";
    /// <summary>デバイス</summary>
    public string Device => (Para != null && Para.Length >= 2) ? Para[1] : "";
    /// <summary></summary>
    public string Comment1 => (Para != null && Para.Length >= 3) ? Para[2] : "";
    /// <summary></summary>
    public string Comment2 => (Para != null && Para.Length >= 4) ? Para[3] : "";

    public override string ToString() => SummaryAll;
}


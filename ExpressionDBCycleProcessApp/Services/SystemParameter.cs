namespace ExpressionDBCycleProcessApp.Services;

/// <summary>
/// DEFINE_SYSTEM_PARAMETERSテーブル情報
/// </summary>
public class SystemParameter
{
    /// <summary>
    /// パラメータKEY
    /// </summary>
    public string ParameterKey { get; set; }
    /// <summary>
    /// パラメータKEY名称
    /// </summary>
    public string KeyName { get; set; }
    /// <summary>
    /// 値
    /// </summary>
    public string ParameterValue { get; set; }
    public SystemParameter()
    {
        ParameterKey = "";
        KeyName = "";
        ParameterValue = "";
    }
}

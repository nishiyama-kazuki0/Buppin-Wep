namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// ログイン情報
/// </summary>
public class LoginInfo
{
    /// <summary>
    /// 作業者ID
    /// </summary>
    public string? Id { get; set; } = string.Empty;
    /// <summary>
    /// 作業者名
    /// </summary>
    public string? UserName { get; set; } = string.Empty;
    /// <summary>
    /// パスワード
    /// </summary>
    public string? Password { get; set; } = string.Empty;
    /// <summary>
    /// 権限
    /// </summary>
    public int AuthorityLevel { get; set; } = 0;
    /// <summary>
    /// 所属
    /// </summary>
    public int AffiliationId { get; set; } = 0;
    /// <summary>
    /// 所属名
    /// </summary>
    public string? AffiliationName { get; set; } = string.Empty;
    /// <summary>
    /// 全機能有効
    /// </summary>
    public bool AllFeatureEnable { get; set; } = false;
}

using System.ComponentModel.DataAnnotations.Schema;

namespace ExpressionDBWebAPI.Data;

/// <summary>
/// プロセスファンクション一式を格納するEntityModelクラス
/// </summary>
internal class ProcessFunctionModel
{
    /// <summary>
    /// ファンクションの種類
    /// </summary>
    public enum enumFunctionType : int
    {
        /// <summary>
        /// 該当なし
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// .NET メソッド
        /// </summary>
        DotNetFunction = 1,
        /// <summary>
        /// ストアドファンクション
        /// </summary>
        StoredFunction = 2,
        /// <summary>
        /// 外部バッチファイル実行
        /// </summary>
        BatchFileExec = 3
    }
    /// <summary>
    /// DBログの挙動タイプ
    /// </summary>
    public enum enumDBLogType : int
    {
        /// <summary>
        /// 通常(すべてログ登録)
        /// </summary>
        All = 0,
        /// <summary>
        /// 操作ログなし
        /// </summary>
        NoneOperateLog = 1,
        /// <summary>
        /// 通知ログなし
        /// </summary>
        NoneNotifyLog = 2,
        /// <summary>
        /// すべてのDBログなし
        /// </summary>
        NoneLog = 9
    }
    [Column("PROGRAM_NAME")]
    public string PROGRAM_NAME { get; set; } = null!;
    [Column("PROGRAM_IS_RETURN")]
    public bool PROGRAM_IS_RETURN { get; set; }
    [Column("PROGRAM_RETURN_DATA_TYPE")]
    public string PROGRAM_RETURN_DATA_TYPE { get; set; } = null!;
    [Column("PROGRAM_IS_TRANSACTION")]
    public bool PROGRAM_IS_TRANSACTION { get; set; }
    [Column("FUNCTION_COUNT")]
    public int FUNCTION_COUNT { get; set; }
    [Column("PROGRAM_TIMEOUT_VALUE")]//ms
    public int PROGRAM_TIMEOUT_VALUE { get; set; }
    [Column("PROGRAM_RETRY_COUNT")]
    public int PROGRAM_RETRY_COUNT { get; set; }
    [Column("PROGRAM_SEMAPHORE_LOCK_COUNT")]
    public int PROGRAM_SEMAPHORE_LOCK_COUNT { get; set; }
    [Column("PROGRAM_SEMAPHORE_MAX_COUNT")]
    public int PROGRAM_SEMAPHORE_MAX_COUNT { get; set; }
    [Column("LOG_TYPE ")]
    public int LOG_TYPE { get; set; }
    [Column("ASSEMBLY_NAME")]
    public string ASSEMBLY_NAME { get; set; } = null!;
    [Column("CLASS_NAME")]
    public string CLASS_NAME { get; set; } = null!;
    [Column("FUNCTION_NAME")]
    public string FUNCTION_NAME { get; set; } = null!;
    [Column("EXEC_ORDER_RANK", Order = 1)]//OrderExec Order Rankでソートする。実行する順番に関わるため。
    public int EXEC_ORDER_RANK { get; set; }
    [Column("FUNCTION_TYPE")]
    public int FUNCTION_TYPE { get; set; }
    [Column("IS_FUNCTION_RETURN")]
    public bool IS_FUNCTION_RETURN { get; set; }
    [Column("FUNCTION_RETRUN_DATA_TYPE")]
    public string FUNCTION_RETRUN_DATA_TYPE { get; set; } = null!;
    [Column("EXEC_TARGET_PATH")]
    public string EXEC_TARGET_PATH { get; set; } = null!;
    [Column("IS_FUNCTION_TRANSACTION")]
    public bool IS_FUNCTION_TRANSACTION { get; set; }
    [Column("ARGUMENT_COUNT")]
    public int ARGUMENT_COUNT { get; set; }
    [Column("ARGUMENT_NAME1")]
    public string ARGUMENT_NAME1 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME1")]
    public string ARGUMENT_TYPE_NAME1 { get; set; } = null!;
    [Column("ARGUMENT_NAME2")]
    public string ARGUMENT_NAME2 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME2")]
    public string ARGUMENT_TYPE_NAME2 { get; set; } = null!;
    [Column("ARGUMENT_NAME3")]
    public string ARGUMENT_NAME3 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME3")]
    public string ARGUMENT_TYPE_NAME3 { get; set; } = null!;
    [Column("ARGUMENT_NAME4")]
    public string ARGUMENT_NAME4 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME4")]
    public string ARGUMENT_TYPE_NAME4 { get; set; } = null!;
    [Column("ARGUMENT_NAME5")]
    public string ARGUMENT_NAME5 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME5")]
    public string ARGUMENT_TYPE_NAME5 { get; set; } = null!;
    [Column("ARGUMENT_NAME6")]
    public string ARGUMENT_NAME6 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME6")]
    public string ARGUMENT_TYPE_NAME6 { get; set; } = null!;
    [Column("ARGUMENT_NAME7")]
    public string ARGUMENT_NAME7 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME7")]
    public string ARGUMENT_TYPE_NAME7 { get; set; } = null!;
    [Column("ARGUMENT_NAME8")]
    public string ARGUMENT_NAME8 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME8")]
    public string ARGUMENT_TYPE_NAME8 { get; set; } = null!;
    [Column("ARGUMENT_NAME9")]
    public string ARGUMENT_NAME9 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME9")]
    public string ARGUMENT_TYPE_NAME9 { get; set; } = null!;
    [Column("ARGUMENT_NAME10")]
    public string ARGUMENT_NAME10 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME10")]
    public string ARGUMENT_TYPE_NAME10 { get; set; } = null!;
    [Column("ARGUMENT_NAME11")]
    public string ARGUMENT_NAME11 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME11")]
    public string ARGUMENT_TYPE_NAME11 { get; set; } = null!;
    [Column("ARGUMENT_NAME12")]
    public string ARGUMENT_NAME12 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME12")]
    public string ARGUMENT_TYPE_NAME12 { get; set; } = null!;
    [Column("ARGUMENT_NAME13")]
    public string ARGUMENT_NAME13 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME13")]
    public string ARGUMENT_TYPE_NAME13 { get; set; } = null!;
    [Column("ARGUMENT_NAME14")]
    public string ARGUMENT_NAME14 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME14")]
    public string ARGUMENT_TYPE_NAME14 { get; set; } = null!;
    [Column("ARGUMENT_NAME15")]
    public string ARGUMENT_NAME15 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME15")]
    public string ARGUMENT_TYPE_NAME15 { get; set; } = null!;
    [Column("ARGUMENT_NAME16")]
    public string ARGUMENT_NAME16 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME16")]
    public string ARGUMENT_TYPE_NAME16 { get; set; } = null!;
    [Column("ARGUMENT_NAME17")]
    public string ARGUMENT_NAME17 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME17")]
    public string ARGUMENT_TYPE_NAME17 { get; set; } = null!;
    [Column("ARGUMENT_NAME18")]
    public string ARGUMENT_NAME18 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME18")]
    public string ARGUMENT_TYPE_NAME18 { get; set; } = null!;
    [Column("ARGUMENT_NAME19")]
    public string ARGUMENT_NAME19 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME19")]
    public string ARGUMENT_TYPE_NAME19 { get; set; } = null!;
    [Column("ARGUMENT_NAME20")]
    public string ARGUMENT_NAME20 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME20")]
    public string ARGUMENT_TYPE_NAME20 { get; set; } = null!;
    [Column("ARGUMENT_NAME21")]
    public string ARGUMENT_NAME21 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME21")]
    public string ARGUMENT_TYPE_NAME21 { get; set; } = null!;
    [Column("ARGUMENT_NAME22")]
    public string ARGUMENT_NAME22 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME22")]
    public string ARGUMENT_TYPE_NAME22 { get; set; } = null!;
    [Column("ARGUMENT_NAME23")]
    public string ARGUMENT_NAME23 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME23")]
    public string ARGUMENT_TYPE_NAME23 { get; set; } = null!;
    [Column("ARGUMENT_NAME24")]
    public string ARGUMENT_NAME24 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME24")]
    public string ARGUMENT_TYPE_NAME24 { get; set; } = null!;
    [Column("ARGUMENT_NAME25")]
    public string ARGUMENT_NAME25 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME25")]
    public string ARGUMENT_TYPE_NAME25 { get; set; } = null!;
    [Column("ARGUMENT_NAME26")]
    public string ARGUMENT_NAME26 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME26")]
    public string ARGUMENT_TYPE_NAME26 { get; set; } = null!;
    [Column("ARGUMENT_NAME27")]
    public string ARGUMENT_NAME27 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME27")]
    public string ARGUMENT_TYPE_NAME27 { get; set; } = null!;
    [Column("ARGUMENT_NAME28")]
    public string ARGUMENT_NAME28 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME28")]
    public string ARGUMENT_TYPE_NAME28 { get; set; } = null!;
    [Column("ARGUMENT_NAME29")]
    public string ARGUMENT_NAME29 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME29")]
    public string ARGUMENT_TYPE_NAME29 { get; set; } = null!;
    [Column("ARGUMENT_NAME30")]
    public string ARGUMENT_NAME30 { get; set; } = null!;
    [Column("ARGUMENT_TYPE_NAME30")]
    public string ARGUMENT_TYPE_NAME30 { get; set; } = null!;

    /// <summary>
    /// プログラムタイムアウト秒数。ストアド実行connectionのメソッドタイムアウトの単位は秒数のため、それに合わせるためのメソッド。
    /// SQlCommandでは、0は無効(無限)。負数はデフォルト値を使用する。
    /// </summary>
    public int? ProgramTimeOutSecond => timeoutSecond < 0 ? null : timeoutSecond;
    private int timeoutSecond => (int)Math.Round(TimeSpan.FromMilliseconds(PROGRAM_TIMEOUT_VALUE).TotalSeconds);

    private const string structuredStr = "structured";
    /// <summary>
    /// structuredを含む場合はtrue
    /// </summary>
    public bool IsStructuredContain =>
        string.Equals(ARGUMENT_TYPE_NAME1, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME2, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME3, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME4, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME5, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME6, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME7, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME8, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME9, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME10, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME11, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME12, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME13, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME14, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME15, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME16, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME17, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME18, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME19, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME20, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME21, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME22, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME23, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME24, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME25, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME26, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME27, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME28, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME29, structuredStr, StringComparison.OrdinalIgnoreCase)
        || string.Equals(ARGUMENT_TYPE_NAME30, structuredStr, StringComparison.OrdinalIgnoreCase)
        ;
}

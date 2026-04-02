using Microsoft.JSInterop;

namespace ExpressionDBBlazorShared;

/// <summary>
/// アセンブリの状態
/// </summary>
public class AssemblyState
{
    /// <summary>
    /// DEBUGシンボル定義状態
    /// </summary>
    private const bool DEBUG =
#if DEBUG
        true;
#else
   false;
#endif
    /// <summary>
    /// DEBUGシンボル定義状態
    /// </summary>
    public static readonly bool Debug = DEBUG;
    [JSInvokable]
    public static bool DebugMethod()
    {
        return Debug;
    }
    /// <summary>
    /// TRACEシンボル定義状態
    /// </summary>
    private const bool TRACE =
#if TRACE
        true;
#else
   false;
#endif
    /// <summary>
    /// TRACEシンボル定義状態
    /// </summary>
    public static readonly bool Trace = TRACE;
}

namespace ExpressionDBWebAPI.Common;

public class CommonInfo
{
    /// <summary>
    /// ルートパス
    /// </summary>
    public static string RootPath = "";
    /// <summary>
    /// AppDataパス
    /// </summary>
    public static string AppDataPath => Path.Combine(RootPath, "App_Data\\物品管理ラベル.xlsx");//.xlsxまで指定してあげる

    public static string AppDataPath2 => Path.Combine(RootPath, "App_Data\\バーコード印刷用.xlsx");//.xlsxまで指定してあげる

}

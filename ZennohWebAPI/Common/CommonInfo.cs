namespace ZennohWebAPI.Common
{
    public class CommonInfo
    {
        /// <summary>
        /// ルートパス
        /// </summary>
        public static string RootPath = "";
        /// <summary>
        /// AppDataパス
        /// </summary>
        public static string AppDataPath { get { return Path.Combine(CommonInfo.RootPath, "App_Data"); } }
    }
}

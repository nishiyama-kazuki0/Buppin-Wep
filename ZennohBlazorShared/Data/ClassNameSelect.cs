using System.Data;

namespace ZennohBlazorShared.Data
{
    public enum enumWhereType
    {
        Equal,
        NotEqual,
        Above,
        Below,
        Big,
        Small,
        LikeStart,
        LikeEnd,
        LikePartial,
        EqualZeroSuppress,
        NotEqualZeroSuppress,
        AboveZeroSuppress,
        BelowZeroSuppress,
        BigZeroSuppress,
        SmallZeroSuppress,
        LikeStartZeroSuppress,
        LikeEndZeroSuppress,
        LikePartialZeroSuppress,
    }
    public enum EnumTSQLhints : int //TODO TSQL以外には適用されないのでここに記述するべきではないが暫定対応
    {
        NONE = 0
        , KEEPIDENTITY = 1
        , KEEPDEFAULTS = 2
        , HOLDLOCK = 3
        , IGNORE_CONSTRAINTS = 4
        , IGNORE_TRIGGERS = 5
        , NOLOCK = 6
        , NOWAIT = 7
        , PAGLOCK = 8
        , READCOMMITTED = 9
        , READCOMMITTEDLOCK = 10
        , READPAST = 11
        , REPEATABLEREAD = 12
        , ROWLOCK = 13
        , SERIALIZABLE = 14
        , SNAPSHOT = 15
        , TABLOCK = 16
        , TABLOCKX = 17
        , UPDLOCK = 18
        , XLOCK = 19
    }
    public class ClassNameSelect
    {
        public string viewName { get; set; }
        public string className { get; set; }
        public string columnsDefineName { get; set; }
        public string componentName { get; set; }
        public int selectTopCnt { get; set; }
        public List<string> selectParam { get; set; }
        public Dictionary<string, WhereParam> whereParam { get; set; }
        public List<OrderByParam> orderByParam { get; set; }
        public bool tableFuncFlg { get; set; }
        public string GetHintStr => hintsStr[(int)tsqlHints];//TODO TSQL以外には適用されないのでここに記述するべきではないが暫定対応
        private readonly string[] hintsStr = new string[] {
            "",
            "KEEPIDENTITY",
            "KEEPDEFAULTS",
            "HOLDLOCK",
            "IGNORE_CONSTRAINTS",
            "IGNORE_TRIGGERS",
            "NOLOCK",
            "NOWAIT",
            "PAGLOCK",
            "READCOMMITTED",
            "READCOMMITTEDLOCK",
            "READPAST",
            "REPEATABLEREAD",
            "ROWLOCK",
            "SERIALIZABLE",
            "SNAPSHOT",
            "TABLOCK",
            "TABLOCKX",
            "UPDLOCK",
            "XLOCK",
        };//TODO TSQL以外には適用されないのでここに記述するべきではないが暫定対応
        public EnumTSQLhints tsqlHints { get; set; } = EnumTSQLhints.NONE;//TODO TSQL以外には適用されないのでここに記述するべきではないが暫定対応

        public ClassNameSelect()
        {
            viewName = "";
            className = "";
            columnsDefineName = "";
            componentName = "";
            selectTopCnt = 0;
            selectParam = new List<string>();
            whereParam = new Dictionary<string, WhereParam>();
            orderByParam = new List<OrderByParam>();
            tableFuncFlg = false;
            tsqlHints = EnumTSQLhints.NONE;
        }
    }
    public class WhereParam
    {
        public string field { get; set; }
        public string val { get; set; }
        public SqlDbType? type { get; set; }
        public enumWhereType whereType { get; set; }
        public List<string> linkingVals { get; set; }
        public bool orLinking { get; set; }
        public int? size { get; set; }
        public bool tableFuncWithWhere { get; set; }
        public bool isSizeSpecNeeded =>
            type is SqlDbType.Char
            or SqlDbType.VarChar
            or SqlDbType.NVarChar
            or SqlDbType.NChar
            or SqlDbType.Binary
            or SqlDbType.VarBinary
            ;
        public WhereParam()
        {
            field = "";
            val = "";
            type = null;
            whereType = enumWhereType.Equal;
            linkingVals = new List<string>();
            orLinking = false;
            size = null;
            tableFuncWithWhere = false;
        }

        /// <summary>
        /// Where句タイプ毎の比較文字を取得する
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public string GetWhereType()
        {
            string str = "=";
            switch (whereType)
            {
                case enumWhereType.Equal:
                case enumWhereType.EqualZeroSuppress:
                    str = "=";
                    break;
                case enumWhereType.NotEqual:
                case enumWhereType.NotEqualZeroSuppress:
                    str = "<>";
                    break;
                case enumWhereType.Above:
                case enumWhereType.AboveZeroSuppress:
                    str = ">=";
                    break;
                case enumWhereType.Below:
                case enumWhereType.BelowZeroSuppress:
                    str = "<=";
                    break;
                case enumWhereType.Big:
                case enumWhereType.BigZeroSuppress:
                    str = ">";
                    break;
                case enumWhereType.Small:
                case enumWhereType.SmallZeroSuppress:
                    str = "<";
                    break;
                case enumWhereType.LikeStart:
                case enumWhereType.LikeEnd:
                case enumWhereType.LikePartial:
                case enumWhereType.LikeStartZeroSuppress:
                case enumWhereType.LikeEndZeroSuppress:
                case enumWhereType.LikePartialZeroSuppress:
                    str = "like";
                    break;
            }
            return str;
        }

        /// <summary>
        /// Where句タイプ毎に付属文字を付加する
        /// </summary>
        /// <param name="where"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ProcessValueWhereType(string value)
        {
            string str = value;
            switch (whereType)
            {
                case enumWhereType.Equal:
                case enumWhereType.NotEqual:
                    break;
                case enumWhereType.LikeStart:
                case enumWhereType.LikeStartZeroSuppress:
                    str = value + "%";
                    break;
                case enumWhereType.LikeEnd:
                case enumWhereType.LikeEndZeroSuppress:
                    str = "%" + value;
                    break;
                case enumWhereType.LikePartial:
                case enumWhereType.LikePartialZeroSuppress:
                    str = "%" + value + "%";
                    break;
            }
            return str;
        }
    }
    /// <summary>
    /// OrderBy句を設定するためのクラス
    /// 
    /// </summary>
    public class OrderByParam
    {
        /// <summary>フィールド名</summary>
        public string field { get; set; }
        /// <summary>並び順 false:ASC true:DESC</summary>
        public bool desc { get; set; }

        public OrderByParam()
        {
            field = "";
            desc = false;
        }
    }
}

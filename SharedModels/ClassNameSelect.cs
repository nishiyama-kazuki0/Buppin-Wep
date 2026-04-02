namespace SharedModels;

/// <summary>
/// SQL文にするときのWhere句の種類
/// </summary>
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
/// <summary>
/// SELECT文発行用情報定義
/// </summary>
public class ClassNameSelect
{
    /// <summary>SELECT文のFROM句に指定するオブジェクト名。(未指定の場合、テーブル「DEFINE_PAGE_VALUES」からclassNameを条件にVIEW_NAMEを取得して使用する)</summary>
    public string viewName { get; set; }
    public string className { get; set; }
    /// <summary>SELECT文で指定する列名をviewName以外から取得する場合に指定する。</summary>
    public string columnsDefineName { get; set; }
    public string componentName { get; set; }
    public int selectTopCnt { get; set; }
    public List<string> selectParam { get; set; }
    public Dictionary<string, WhereParam> whereParam { get; set; }
    public List<OrderByParam> orderByParam { get; set; }
    public bool tableFuncFlg { get; set; }
    public string GetHintStr => hintsStr[(int)tsqlHints];//TODO TSQL以外には適用されないのでここに記述するべきではないが暫定対応
    public string modelTypeName { get; set; } = string.Empty;
    public Type GetModelType => Type.GetType(modelTypeName) ?? typeof(object);
    //SQLコマンド実行タイムアウト時間[s]
    public int CommandTimeout { get; set; }
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
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ClassNameSelect()
    {
        viewName = "";
        className = "";
        columnsDefineName = "";
        componentName = "";
        selectTopCnt = 0;
        selectParam = [];
        whereParam = [];
        orderByParam = [];
        tableFuncFlg = false;
        tsqlHints = EnumTSQLhints.NONE;
        CommandTimeout = 30;
    }
}

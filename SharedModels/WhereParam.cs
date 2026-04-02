using System.Data;

namespace SharedModels;

public class WhereParam
{
    public string field { get; set; }
    public string val { get; set; }
    public DbType? type { get; set; }
    public string dbTypeString { get; set; }
    public enumWhereType whereType { get; set; }
    public List<string> linkingVals { get; set; }
    public bool orLinking { get; set; }
    public int? size { get; set; }
    public bool tableFuncWithWhere { get; set; }
    public WhereParam()
    {
        field = "";
        val = "";
        type = null;
        dbTypeString = string.Empty;
        whereType = enumWhereType.Equal;
        linkingVals = [];
        orLinking = false;
        size = null;
        tableFuncWithWhere = false;
    }

    /// <summary>
    /// Where句タイプ毎の比較文字を取得する
    /// 
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
                str = "like"; //TODO Oracleの場合は問題ないのか。要検証
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
                str = value + "%"; //TODO Oracleの場合は問題ないのか。要検証
                break;
            case enumWhereType.LikeEnd:
            case enumWhereType.LikeEndZeroSuppress:
                str = "%" + value;　//TODO Oracleの場合は問題ないのか。要検証
                break;
            case enumWhereType.LikePartial:
            case enumWhereType.LikePartialZeroSuppress:
                str = "%" + value + "%";　//TODO Oracleの場合は問題ないのか。要検証
                break;
        }
        return str;
    }
}


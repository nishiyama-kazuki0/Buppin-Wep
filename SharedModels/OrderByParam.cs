namespace SharedModels;

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


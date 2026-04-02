namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// 倉庫情報
/// </summary>
public class MstShelf
{
    /// <summary>
    /// エリアID
    /// </summary>
    public string 棚ID { get; set; }
   
    public string ID { get; set; }

    public MstShelf()
    {
        ID = "";
        棚ID = "";
    }

    //public static void GetValueTextInfo(ref List<ValueTextInfo> lstInfo, List<MstShelf> data)
    //{
    //    lstInfo.Clear();
    //    foreach (MstShelf item in data)
    //    {
    //        ValueTextInfo info = new()
    //        {
    //            Value = item.ID,
    //            Text = item.棚ID,

    //        };
    //        lstInfo.Add(info);
    //    }
    //}

    public static List<ValueTextInfo> GetValueTextInfo(List<MstShelf> data)
    {
        List<ValueTextInfo> lstInfo = new();
        foreach (MstShelf item in data)
        {
            ValueTextInfo info = new()
            {
                Value = item.ID,
                Text = item.棚ID,
            };
            lstInfo.Add(info);
        }
        return lstInfo;
    }

    //public static List<ValueTextInfo> GetValueTextInfo(List<MstShelf> data)
    //{
    //    List<ValueTextInfo> lstInfo = [];
    //    GetValueTextInfo(ref lstInfo, data);
    //    return lstInfo;
    //}
}

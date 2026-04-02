namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// 
/// これは既存のクラスを使って部署テーブルを参照しています。
/// </summary>
public class MstAreaData
{
    /// <summary>
    /// エリアID
    /// </summary>
    public string AreaId { get; set; }
    /// <summary>
    /// エリア名称
    /// </summary>
    public string AreaName { get; set; }
    public MstAreaData()
    {
        AreaId = "";
        AreaName = "";
    }

    public static void GetValueTextInfo(ref List<ValueTextInfo> lstInfo, List<MstAreaData> data)
    {
        lstInfo.Clear();
        foreach (MstAreaData item in data)
        {
            ValueTextInfo info = new()
            {
                Value = item.AreaId,
                Text = item.AreaName,
            };
            lstInfo.Add(info);
        }
    }
    public static List<ValueTextInfo> GetValueTextInfo(List<MstAreaData> data)
    {
        List<ValueTextInfo> lstInfo = [];
        GetValueTextInfo(ref lstInfo, data);
        return lstInfo;
    }
}

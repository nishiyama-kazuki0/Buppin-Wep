namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// ゾーン情報
/// </summary>
public class MstZoneData
{
    /// <summary>
    /// エリアID
    /// </summary>
    public string AreaId { get; set; }
    /// <summary>
    /// ゾーンID
    /// </summary>
    public string 棚ID { get; set; }
    /// <summary>
    /// ゾーン名称
    /// </summary>
    public string ZoneName { get; set; }
    public MstZoneData()
    {
        AreaId = "";
        棚ID = "";
        ZoneName = "";
    }

    public static List<ValueTextInfo> GetValueTextInfo(List<MstZoneData> data)
    {
        List<ValueTextInfo> lstInfo = [];
        foreach (MstZoneData item in data)
        {
            ValueTextInfo info = new()
            {
                Value = item.棚ID,
                Text = item.ZoneName,
            };
            lstInfo.Add(info);
        }
        return lstInfo;
    }
}

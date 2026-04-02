namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// ロケーション情報
/// </summary>
public class MstLocationData
{
    /// <summary>
    /// エリアID
    /// </summary>
    public string AreaId { get; set; }
    /// <summary>
    /// ゾーンID
    /// </summary>
    public string ZoneId { get; set; }
    /// <summary>
    /// ゾーンID
    /// </summary>
    public string LocationId { get; set; }
    /// <summary>
    /// ゾーン名称
    /// </summary>
    public string LocationName { get; set; }
    public MstLocationData()
    {
        AreaId = "";
        ZoneId = "";
        LocationId = "";
        LocationName = "";
    }

    public static List<ValueTextInfo> GetValueTextInfo(List<MstLocationData> data)
    {
        List<ValueTextInfo> lstInfo = [];
        foreach (MstLocationData item in data)
        {
            ValueTextInfo info = new()
            {
                Value = item.LocationId,
                Text = item.LocationName,
            };
            lstInfo.Add(info);
        }
        return lstInfo;
    }
}

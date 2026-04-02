namespace ZennohBlazorShared.Data
{
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
        public string ZoneId { get; set; }
        /// <summary>
        /// ゾーン名称
        /// </summary>
        public string ZoneName { get; set; }
        public MstZoneData()
        {
            AreaId = "";
            ZoneId = "";
            ZoneName = "";
        }

        public static List<ValueTextInfo> GetValueTextInfo(List<MstZoneData> data)
        {
            List<ValueTextInfo> lstInfo = new();
            foreach (MstZoneData item in data)
            {
                ValueTextInfo info = new()
                {
                    Value = item.ZoneId,
                    Text = item.ZoneName,
                };
                lstInfo.Add(info);
            }
            return lstInfo;
        }
    }
}

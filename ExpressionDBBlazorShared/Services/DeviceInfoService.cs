using SharedModels;
using static SharedModels.SharedConst;

namespace ExpressionDBBlazorShared.Services;

public class DeviceInfoService
{
    /// <summary>
    /// HttpClient
    /// </summary>
    private readonly CommonWebComService _webComService;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="webComService"></param>
    public DeviceInfoService(CommonWebComService webComService)
    {
        _webComService = webComService;
    }

    /// <summary>
    /// デバイスグループタイプ
    /// </summary>
    public TYPE_DEVICE_TYPE_GROUP DeviceGroupId { get; private set; } = TYPE_DEVICE_TYPE_GROUP.NONE;
    /// <summary>
    /// デバイスタイプ
    /// </summary>
    public TYPE_DEVICE_TYPE DeviceType { get; private set; } = TYPE_DEVICE_TYPE.NONE;
    /// <summary>
    /// ブラウザに設定されているユーザーエージェント文字列
    /// </summary>
    public string UserAgent { get; private set; } = string.Empty;

    /// <summary>
    /// ＨＴ端末かどうか。ＨＴ(モバイル機)用のレイアウトや機能を有無の判定に使用する。
    /// </summary>
    /// <returns></returns>
    public bool IsHandy()
    {
        return DeviceGroupId == TYPE_DEVICE_TYPE_GROUP.HT;
    }
    /// <summary>
    /// userAgent文字列をもとにDeviceInfoServiceのプロパティを設定する。(サービスはAddScopeされている前提)
    /// </summary>
    /// <param name="ua"></param>
    /// <returns></returns>
    public async Task SetupDeviceInfo(string ua)
    {
        await Task.Delay(0);
        UserAgent = ua;
        (TYPE_DEVICE_TYPE_GROUP group, TYPE_DEVICE_TYPE type) = await GetDeviceTypeGroup(ua);
        DeviceGroupId = group;
        DeviceType = type;
    }
    /// <summary>
    /// userAgent文字列をもとにDBからTYPE_DEVICE_TYPE_GROUPとTYPE_DEVICE_TYPEを取得する。
    /// </summary>
    /// <param name="ua"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    private async Task<(TYPE_DEVICE_TYPE_GROUP group, TYPE_DEVICE_TYPE type)> GetDeviceTypeGroup(string ua)
    {
        ClassNameSelect select = new()
        {
            viewName = "VW_DEFINE_DEVICE_GROUP",
            tsqlHints = EnumTSQLhints.NOLOCK,
        };
        select.orderByParam.Add(new OrderByParam { field = "SORT_ORDER" });

        IEnumerable<DEFINE_DEVICE_GROUP_Model>? resItems = await _webComService.GetGenericResponseValue<DEFINE_DEVICE_GROUP_Model>(select);

        if (resItems is null)
        {
            throw new NullReferenceException("GetDeviceTypeGroup_resItems is null");
        }

        foreach (DEFINE_DEVICE_GROUP_Model item in resItems)
        {
            string jstr = item.JUDGE_STRING ?? string.Empty;//念のためnull対策
            if (ua.Contains(jstr) && !string.IsNullOrWhiteSpace(jstr))
            {
                //JUDGE_STRINGがuaに含まれていたら、DEVICE_GROUP_IDを返す
                return (
                    (TYPE_DEVICE_TYPE_GROUP)item.DEVICE_TYPE_GROUP_ID
                    , (TYPE_DEVICE_TYPE)item.DEVICE_TYPE
                    );
            }
        }
        //uaがどれにも属さない場合はNONEとする //NONEの場合はPCとしておく。
        return (TYPE_DEVICE_TYPE_GROUP.PC, TYPE_DEVICE_TYPE.NONE);
    }
}

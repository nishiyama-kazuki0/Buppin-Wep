using Blazored.LocalStorage;
using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Shared;
using ExpressionDBBlazorShared.Util;
using SharedModels;
using Sotsera.Blazor.Toaster;
using Sotsera.Blazor.Toaster.Core.Models;
using System.Collections.Frozen;
using static SharedModels.SharedConst;

namespace ExpressionDBBlazorShared.Services;

/// <summary>
/// 案件ごとのカスタムサービスを記述する
/// ※CommonServiceは汎用的なものしか記載しない
/// </summary>
public class CustomForSuntoryService
{
    /// <summary>
    /// CommonWebComService
    /// </summary>
    private readonly CommonWebComService _webComService;
    /// <summary>
    /// ILocalStorageService
    /// </summary>
    private readonly ILocalStorageService _localStorage;
    /// <summary>
    /// ISessionStorageService
    /// </summary>
    private readonly ISessionStorageService _sessionStorage;
    /// <summary>
    /// DialogService
    /// </summary>
    private readonly DialogService _dialogService;
    /// <summary>
    /// SystemParameterService
    /// </summary>
    private readonly SystemParameterService _sysParams;
    /// <summary>
    /// toaster
    /// </summary>
    private readonly IToaster _toasterService;


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="httpClient"></param>
    public CustomForSuntoryService(CommonWebComService commonWebComService, ILocalStorageService localStorage, ISessionStorageService sessionStorage, DialogService dialogService, SystemParameterService sysparam, IToaster _toaster)
    {
        _webComService = commonWebComService;
        _localStorage = localStorage;
        _sessionStorage = sessionStorage;
        _dialogService = dialogService;
        _sysParams = sysparam;
        _toasterService = _toaster;
    }

    /// <summary>
    /// ピック予定情報を取得する
    /// </summary>
    /// <returns></returns>
    public async Task<List<PickPlansInfo>> GetPickPlansInfoAsync()
    {
        List<PickPlansInfo> lstPickPlansInfo = [];
        try
        {
            ClassNameSelect select = new()
            {
                viewName = "VW_HT_ピッキング_残予定"
            };
            ResponseValue[]? resValues = await _webComService.GetResponseValue(select);
            if (null != resValues)
            {
                foreach (ResponseValue resValue in resValues)
                {
                    if (resValue != null && resValue.Values != null && resValue.Columns != null)
                    {
                        PickPlansInfo pickPlansInfo = new()
                        {
                            DeliveryCd = ConvertUtil.GetValueString(resValue, "倉庫配送先ｺｰﾄﾞ"),
                            AreaCd = ConvertUtil.GetValueString(resValue, "倉庫ｺｰﾄﾞ"),
                            ZoneCd = ConvertUtil.GetValueString(resValue, "ｿﾞｰﾝｺｰﾄﾞ")
                        };
                        lstPickPlansInfo.Add(pickPlansInfo);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return lstPickPlansInfo;
    }

    /// <summary>
    /// ロケーション区分を取得する(自動倉庫状況(常温・冷凍)画面に区分の説明を表示するため)
    /// </summary>
    /// <returns></returns>
    public async Task<List<LocationTypesInfo>> GetLocationTypesInfoAsync()
    {
        List<LocationTypesInfo> lstLocationTypesInfo = [];

        try
        {
            ClassNameSelect select = new()
            {
                viewName = "DEFINE_LOCATION_TYPES"
            };
            ResponseValue[]? resValues = await _webComService.GetResponseValue(select);
            if (null != resValues)
            {
                foreach (ResponseValue resValue in resValues)
                {
                    if (resValue != null && resValue.Values != null && resValue.Columns != null)
                    {
                        LocationTypesInfo locationTypesInfo = new()
                        {
                            LocationType = ConvertUtil.GetValueInt(resValue, "LOCATION_TYPE"),
                            LocationTypeName = ConvertUtil.GetValueString(resValue, "LOCATION_TYPE_NAME"),
                            CellForeColor = ConvertUtil.GetValueString(resValue, "CELL_FORECOLOR"),
                            CellBackColor = ConvertUtil.GetValueString(resValue,"CELL_BACKCOLOR"),
                            CellUnSelectable = ConvertUtil.GetValueBool(resValue,"CELL_UNSELECTABLE")
                        };
                        lstLocationTypesInfo.Add(locationTypesInfo);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return lstLocationTypesInfo;
    }
}

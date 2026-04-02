using Blazored.LocalStorage;
using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Shared;
using ExpressionDBBlazorShared.Util;
using SharedModels;
using Sotsera.Blazor.Toaster;
using Sotsera.Blazor.Toaster.Core.Models;
using System.Collections.Frozen;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using static SharedModels.SharedConst;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ExpressionDBBlazorShared.Services;

public class CommonService
{

    // 画面毎の設定関連情報はログイン時に保持しておく
    public FrozenDictionary<string, string> PageNameAll { get; set; }
    /// <summary>
    /// DEINFE_COMPONENTSテーブル情報を保持
    /// キー：CLASS_NAME
    /// </summary>
    public FrozenDictionary<string, List<ComponentsInfo>> ComponentsInfoAll { get; set; }
    /// <summary>
    /// DEINFE_COMPONENT_COLUMNSテーブル情報を保持
    /// キー：CLASS_NAME
    /// </summary>
    public FrozenDictionary<string, List<ComponentColumnsInfo>> ComponentColumnsAll { get; set; }
    /// <summary>
    /// DEINFE_COMPONENT_PROGRAMSテーブル情報を保持
    /// キー：CLASS_NAME
    /// </summary>
    public FrozenDictionary<string, List<ComponentProgramInfo>> ComponentProgramAll { get; set; }

    // 画面ごとに読み込まないマスタ情報を保持
    public List<MstAreaData> MstAreaInfoAll { get; set; } = [];
    public List<MstZoneData> MstZoneInfoAll { get; set; } = [];
    public List<MstLocationData> MstLocationInfoAll { get; set; } = [];

    public List<MstShelf> MstShelfAll { get; set; } = [];

    //ここの値のみDBから取得不可

    //メッセージダイアログの幅,高さ（処理中、読込中）
    public int MessageDialogWidth { get; set; } = 200;
    public int MessageDialogHeight { get; set; } = 155;
    //確認メッセージダイアログの幅,高さ（OKのみ）
    public int DialogShowOKWidth { get; set; } = 350;
    public int DialogShowOKHeight { get; set; } = 200;
    //確認メッセージダイアログの幅（YesとNo）
    public int DialogShowYesNoWidth { get; set; } = 350;
    public int DialogShowYesNoHeight { get; set; } = 200;
    //確認メッセージダイアログの幅（パスワード）
    public int DialogShowPasswordWidth { get; set; } = 350;
    public int DialogShowPasswordHeight { get; set; } = 600;

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

    private readonly ChildPageBase _childBaseService;


    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="httpClient"></param>
    public CommonService(CommonWebComService commonWebComService, ILocalStorageService localStorage, ISessionStorageService sessionStorage, DialogService dialogService, SystemParameterService sysparam, IToaster _toaster)
    {
        _webComService = commonWebComService;
        _localStorage = localStorage;
        _sessionStorage = sessionStorage;
        _dialogService = dialogService;
        _sysParams = sysparam;
        _toasterService = _toaster;
    }


    /// <summary>
    /// ログインストアド実行
    /// </summary>
    /// <returns></returns>
    public async Task<bool> ExecLoginFunc(string className, string versionString)
    {
        RequestValue rv
            = RequestValue.CreateRequestProgram("ユーザーログイン")
            .SetArgumentValue("CLIENT_APP_VERSION", versionString, "");
        ExecResult[]? results = await _webComService.SetRequestValue(className, rv);

        if (results == null || results.Length <= 0)
        {
            //TODO webAPI異常終了。電波強度の確認メッセージを表示したい
            return false;
        }
        //ストアドでエラー扱いの場合は、エラーメッセージを表示する
        if (results.Min(_ => _.RetCode) < 0)
        {
            // ログイン失敗メッセージ
            await DialogShowOK(results[0].Message, height: 260);//TODO 高さは暫定
            return false;
        }
        return true;
    }

    /// <summary>
    /// ログアウトストアド実行
    /// </summary>
    /// <returns></returns>
    public async Task ExecLogoutFunc(string className, bool isCancelIgnore = false)
    {
        RequestValue rv = RequestValue.CreateRequestProgram("ユーザーログアウト");
        rv.IsCancelTokenIgnore = isCancelIgnore;
        _ = await _webComService.SetRequestValue(className, rv);
    }
    /// <summary>
    /// 排他ロック情報削除ストアドの実行
    /// </summary>
    /// <param name="className"></param>
    /// <param name="isCancelIgnore"></param>
    /// <returns></returns>
    public async Task ExecDleteLockInfoFunc(string className, bool isCancelIgnore = false)
    {
        RequestValue rv = RequestValue.CreateRequestProgram("排他ロック情報削除");
        rv.IsCancelTokenIgnore = isCancelIgnore;
        _ = await _webComService.SetRequestValue(className, rv);
    }


    public async Task ストアド実行棚移動(string className, bool isCancelIgnore = false)
    {
        RequestValue rv = RequestValue.CreateRequestProgram("物品マスタ移動");
        rv.IsCancelTokenIgnore = isCancelIgnore;
        _ = await _webComService.SetRequestValue(className, rv);
    }
    /// <summary>
    /// IDから採番された文字列を取得する
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public async Task<string> GetManagementId(string className, SharedConst.WorkCategory category)
    {
        string id = string.Empty;
        RequestValue rv = RequestValue.CreateRequestProgram(SharedConst.KEY_PROGRAM_NAME_MANAGEMENT_ID);
        _ = rv.SetArgumentValue("WORK_CATEGORY", (short)category, "");
        ExecResult[]? results = await _webComService.SetRequestValue(className, rv);
        if (results != null && results.Length > 0)
        {
            id = results[0].Message;
        }
        return id;
    }

    /// <summary>
    /// MST_ZONEテーブル情報を取得する
    /// 西山変更
    /// MST_Departmentテーブルから(部署)情報を取得
    /// </summary>
    /// <returns></returns>
    public async Task<List<MstAreaData>> GetArea(bool dispId = false, List<(string key, WhereParam wp)>? wpList = null)
    {
        ClassNameSelect select = new()
        {
            viewName = "MST_Department"
        };
        //TODO できればストレージからパラメータの取得はやめる
        //_ = await _sessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
        //select.whereParam[SharedConst.KEY_BASE_ID] = new WhereParam { val = _sysParams.BaseId };
        //select.whereParam[SharedConst.KEY_BASE_TYPE] = new WhereParam { val = _sysParams.BaseType.ToString() };
        if (wpList is not null)
        {
            foreach ((string key, WhereParam wp) in wpList)
            {
                select.whereParam.Add(key, wp);
            }
        }
        select.orderByParam.Add(new OrderByParam { field = "ID" });

        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        List<MstAreaData> data = [];
        if (resItems is null)
        {
            throw new NullReferenceException("GetArea_resItems is null");
        }

        foreach (ResponseValue item in resItems)
        {
            MstAreaData newRow = new()
            {
                AreaId = ConvertUtil.GetValueString(item, "ID"),
                AreaName = dispId ? ConvertUtil.GetValueString(item, "ID") + " " + ConvertUtil.GetValueString(item, "AFFILIATION_ID") : ConvertUtil.GetValueString(item, "AFFILIATION_ID"),
            };

            data.Add(newRow);
        }

        return data;
    }



    /// <summary>
    /// MST_ZONEテーブル情報を取得する
    /// 
    /// MST_SHELFテーブル情報を取得
    /// 棚の情報を取得する
    /// </summary>
    /// <returns></returns>
    public async Task<List<MstZoneData>> GetZone(bool dispId = false, List<(string key, WhereParam wp)>? wpList = null)
    {
        ClassNameSelect select = new()
        {
            viewName = "MST_SHELF"
        };
        //TODO できればストレージからパラメータの取得はやめる
        //_ = await _sessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
        //select.whereParam[SharedConst.KEY_BASE_ID] = new WhereParam { val = _sysParams.BaseId };
        //select.whereParam[SharedConst.KEY_BASE_TYPE] = new WhereParam { val = _sysParams.BaseType.ToString() };
        if (wpList is not null)
        {
            foreach ((string key, WhereParam wp) in wpList)
            {
                select.whereParam.Add(key, wp);
            }
        }
        select.orderByParam.Add(new OrderByParam { field = "棚ID" });
        //select.orderByParam.Add(new OrderByParam { field = "ZONE_ID" });

        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        List<MstZoneData> data = [];
        if (resItems is null)
        {
            throw new NullReferenceException("GetZone_resItems is null");
        }

        foreach (ResponseValue item in resItems)
        {
            MstZoneData newRow = new()
            {
                //AreaId = ConvertUtil.GetValueString(item, "AREA_ID"),
                棚ID = ConvertUtil.GetValueString(item, "棚ID"),
               // ZoneName = dispId ? ConvertUtil.GetValueString(item, "棚ID") + " " + ConvertUtil.GetValueString(item, "ZONE_NAME") : ConvertUtil.GetValueString(item, "ZONE_NAME"),
            };

            data.Add(newRow);
        }

        return data;
    }

    /// <summary>
    /// MST_LOCATIONSテーブル情報を取得する
    /// </summary>
    /// <returns></returns>
    public async Task<List<MstLocationData>> GetLocation(bool dispId = false, List<(string key, WhereParam wp)>? wpList = null)
    {
        ClassNameSelect select = new()
        {
            viewName = "MST_SHELF"
        };
        //TODO できればストレージからパラメータの取得はやめる
        //_ = await _sessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
        //select.whereParam[SharedConst.KEY_BASE_ID] = new WhereParam { val = _sysParams.BaseId };
        //select.whereParam[SharedConst.KEY_BASE_TYPE] = new WhereParam { val = _sysParams.BaseType.ToString() };
        if (wpList is not null)
        {
            foreach ((string key, WhereParam wp) in wpList)
            {
                select.whereParam.Add(key, wp);
            }
        }
        select.orderByParam.Add(new OrderByParam { field = "ID" });

        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        List<MstLocationData> data = [];
        if (resItems is null)
        {
            throw new NullReferenceException("GetArea_resItems is null");
        }

        foreach (ResponseValue item in resItems)
        {
            MstLocationData newRow = new()
            {
                LocationId = ConvertUtil.GetValueString(item, "ID"),
                LocationName = dispId ? ConvertUtil.GetValueString(item, "ID") + " " + ConvertUtil.GetValueString(item, "棚ID") : ConvertUtil.GetValueString(item, "棚ID"),
            };

            data.Add(newRow);
        }

        return data;
    }

    /// <returns></returns>
    public async Task<List<MstShelf>> GetShelf(bool dispId = false, List<(string key, WhereParam wp)>? wpList = null)
    {
        ClassNameSelect select = new()
        {
            viewName = "MST_SHELF"
        };
        //TODO できればストレージからパラメータの取得はやめる
        //_ = await _sessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
        //select.whereParam[SharedConst.KEY_BASE_ID] = new WhereParam { val = _sysParams.BaseId };
        //select.whereParam[SharedConst.KEY_BASE_TYPE] = new WhereParam { val = _sysParams.BaseType.ToString() };
        if (wpList is not null)
        {
            foreach ((string key, WhereParam wp) in wpList)
            {
                select.whereParam.Add(key, wp);
            }
        }
        select.orderByParam.Add(new OrderByParam { field = "ID" });
        //select.orderByParam.Add(new OrderByParam { field = "ZONE_ID" });

        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        List<MstShelf> data = [];
        if (resItems is null)
        {
            throw new NullReferenceException("GetZone_resItems is null");
        }

        foreach (ResponseValue item in resItems)
        {
            MstShelf newRow = new()
            {
                //AreaId = ConvertUtil.GetValueString(item, "AREA_ID"),
                ID = ConvertUtil.GetValueString(item, "ID"),
                棚ID = dispId ? ConvertUtil.GetValueString(item, "ID") + " " + ConvertUtil.GetValueString(item, "棚ID") : ConvertUtil.GetValueString(item, "棚ID"),
            };

            data.Add(newRow);
        }

        return data;
    }

    /// <summary>
    /// ログイン情報を取得する
    /// </summary>
    /// <returns></returns>
    public async Task<LoginInfo[]> GetLoginInfoAsync()
    {
        // 取得カラム名を指定する（共通）
        ClassNameSelect select = new()
        {
            viewName = "VW_ログイン情報"
        };

        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        List<LoginInfo> data = [];

        if (null != resItems)
        {
            foreach (ResponseValue item in resItems)
            {
                LoginInfo newRow = new()
                {
                    Id = ConvertUtil.GetValueString(item, "USER_ID"),
                    UserName = ConvertUtil.GetValueString(item, "USER_NAME"),
                    Password = ConvertUtil.GetValueString(item, "PASSWORD"),
                    AuthorityLevel = ConvertUtil.GetValueInt(item, "AUTHORITY_LEVEL"),
                    AffiliationId = ConvertUtil.GetValueInt(item, "AFFILIATION_ID"),
                    AffiliationName = ConvertUtil.GetValueString(item, "AFFILIATION_NAME"),
                    AllFeatureEnable = ConvertUtil.GetValueBool(item, "ALL_FEATURE_ENABLE"),
                };
                data.Add(newRow);
            }
        }
        return data.ToArray();
    }

    /// <summary>
    /// クラス名からDEFINE_COMPNENTSテーブル情報を取得する
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    public async Task<List<ComponentsInfo>> GetComponetnsInfo(string className)
    {
        await Task.Delay(0);//警告の抑制
        FrozenDictionary<string, List<ComponentsInfo>> info = ComponentsInfoAll;
        List<ComponentsInfo> data = [];
        if (info != null && info.TryGetValue(className, out List<ComponentsInfo>? temp))
        {
            data = temp;
        }
        return data;
    }

    /// <summary>
    /// クラス名からDEFINE_COMPNENT_COLUMNSテーブル情報を取得する
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    public async Task<List<ComponentColumnsInfo>> GetGridColumnsData(string className)
    {
        await Task.Delay(0);//警告の抑制
        FrozenDictionary<string, List<ComponentColumnsInfo>> info = ComponentColumnsAll;
        List<ComponentColumnsInfo> data = [];
        if (info != null && info.TryGetValue(className, out List<ComponentColumnsInfo>? temp))
        {
            data = temp;
        }

        return data;
    }

    /// <summary>
    /// クラス名からDEFINE_COMPONENT_PROGRAMSの情報を取得する
    /// </summary>
    /// <param name="columnInfos"></param>
    /// <param name="select"></param>
    /// <returns></returns>
    public async Task<List<ComponentProgramInfo>> GetComponentProgramInfo(string className)
    {
        await Task.Delay(0);//警告の抑制
        FrozenDictionary<string, List<ComponentProgramInfo>> info = ComponentProgramAll;
        List<ComponentProgramInfo> data = [];
        if (info != null && info.TryGetValue(className, out List<ComponentProgramInfo>? temp))
        {
            data = temp;
        }

        return data;
    }

    /// <summary>
    /// 倉庫マスタ情報を取得
    /// </summary>
    /// <returns></returns>
    public async Task<List<MstAreaData>> GetMstAreaInfoAll()
    {
        await Task.Delay(0);//警告の抑制
        return MstAreaInfoAll;
    }

    /// <summary>
    /// エリアマスタ情報を取得
    /// </summary>
    /// <returns></returns>
    public async Task<List<MstZoneData>> GetMstZoneInfoAll()
    {
        await Task.Delay(0);//警告の抑制
        return MstZoneInfoAll;
    }

    /// <summary>
    /// ロケーションマスタ情報を取得
    /// </summary>
    /// <returns></returns>
    public async Task<List<MstLocationData>> GetMstLocationInfoAll()
    {
        await Task.Delay(0);//警告の抑制
        return MstLocationInfoAll;
    }

    public async Task<List<MstShelf>> GetMstShelfAll()
    {
        await Task.Delay(0);//警告の抑制
        return MstShelfAll;
    }

    /// <summary>
    /// MST_USER_COMPONENT_SETTINGSテーブル情報を取得し、UserComponentSettingsInfoのリストを返す
    /// </summary>
    /// <param name="className"></param>
    /// <param name="componentName"></param>
    /// <param name="viewName"></param>
    /// <returns></returns>
    public async Task<List<UserComponentSettingsInfo>> GetUserComponentSettingsInfoList(string className, string componentName = "", string viewName = "")
    {
        // ログインユーザ情報取得
        LoginInfo login = await _sessionStorage.ContainKeyAsync(SharedConst.KEY_LOGIN_INFO)
            ? await _sessionStorage.GetItemAsync<LoginInfo>(SharedConst.KEY_LOGIN_INFO)
            : new LoginInfo();

        // 取得カラム名を指定する（共通）
        ClassNameSelect select = new()
        {
            viewName = "MST_USER_COMPONENT_SETTINGS"
        };
        select.whereParam.Add("USER_ID", new WhereParam { val = login.Id, whereType = enumWhereType.Equal });
        select.whereParam.Add("CLASS_NAME", new WhereParam { val = className, whereType = enumWhereType.Equal });
        if (!string.IsNullOrEmpty(componentName))
        {
            select.whereParam.Add("COMPONENT_NAME", new WhereParam { val = componentName, whereType = enumWhereType.Equal });
        }
        if (!string.IsNullOrEmpty(viewName))
        {
            select.whereParam.Add("VIEW_NAME", new WhereParam { val = viewName, whereType = enumWhereType.Equal });
        }
        select.orderByParam.Add(new OrderByParam { field = "COMPONENT_NAME" });
        select.orderByParam.Add(new OrderByParam { field = "VIEW_NAME" });
        select.orderByParam.Add(new OrderByParam { field = "PROPERTY_KEY" });
        select.orderByParam.Add(new OrderByParam { field = "VALUE_KEY_ID" });

        List<UserComponentSettingsInfo> lstInfo = [];
        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);

        if (null != resItems)
        {
            foreach (ResponseValue item in resItems)
            {
                UserComponentSettingsInfo newRow = new()
                {
                    ComponentName = ConvertUtil.GetValueString(item, "COMPONENT_NAME"),
                    ViewName = ConvertUtil.GetValueString(item, "VIEW_NAME"),
                    PropertyKey = ConvertUtil.GetValueString(item, "PROPERTY_KEY"),
                    ValueKeyId = ConvertUtil.GetValueInt(item, "VALUE_KEY_ID"),
                    Value = ConvertUtil.GetValueString(item, "VALUE"),
                    ValueDataType = ConvertUtil.GetValueString(item, "VALUE_DATA_TYPE"),
                };
                lstInfo.Add(newRow);
            }
        }

        return lstInfo;
    }

    /// <summary>
    /// クラス名からMST_USER_COMPONENT_SETTINGSテーブル情報を取得する
    /// </summary>
    /// <param name="className"></param>
    /// <param name="componentName"></param>
    /// <param name="viewName"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, object>> GetUserComponentSettingsInfo(string className, string componentName = "", string viewName = "")
    {
        // ログインユーザ情報取得
        LoginInfo login = await _sessionStorage.ContainKeyAsync(SharedConst.KEY_LOGIN_INFO)
            ? await _sessionStorage.GetItemAsync<LoginInfo>(SharedConst.KEY_LOGIN_INFO)
            : new LoginInfo();

        // 取得カラム名を指定する（共通）
        ClassNameSelect select = new()
        {
            viewName = "MST_USER_COMPONENT_SETTINGS"
        };
        select.whereParam.Add("USER_ID", new WhereParam { val = login.Id, whereType = enumWhereType.Equal });
        select.whereParam.Add("CLASS_NAME", new WhereParam { val = className, whereType = enumWhereType.Equal });
        if (!string.IsNullOrEmpty(componentName))
        {
            select.whereParam.Add("COMPONENT_NAME", new WhereParam { val = componentName, whereType = enumWhereType.Equal });
        }
        if (!string.IsNullOrEmpty(viewName))
        {
            select.whereParam.Add("VIEW_NAME", new WhereParam { val = viewName, whereType = enumWhereType.Equal });
        }
        select.orderByParam.Add(new OrderByParam { field = "COMPONENT_NAME" });
        select.orderByParam.Add(new OrderByParam { field = "VIEW_NAME" });
        select.orderByParam.Add(new OrderByParam { field = "PROPERTY_KEY" });
        select.orderByParam.Add(new OrderByParam { field = "VALUE_KEY_ID" });

        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        List<UserComponentSettingsInfo> lstInfo = [];
        Dictionary<string, object> data = [];

        if (null != resItems)
        {
            string strPropertyKey = string.Empty;
            foreach (ResponseValue item in resItems)
            {
                UserComponentSettingsInfo newRow = new()
                {
                    ComponentName = ConvertUtil.GetValueString(item, "COMPONENT_NAME"),
                    ViewName = ConvertUtil.GetValueString(item, "VIEW_NAME"),
                    PropertyKey = ConvertUtil.GetValueString(item, "PROPERTY_KEY"),
                    ValueKeyId = ConvertUtil.GetValueInt(item, "VALUE_KEY_ID"),
                    Value = ConvertUtil.GetValueString(item, "VALUE"),
                    ValueDataType = ConvertUtil.GetValueString(item, "VALUE_DATA_TYPE"),
                };
                lstInfo.Add(newRow);
            }

            var infos = lstInfo.GroupBy(_ => new
            {
                _.ComponentName,
                _.ViewName,
                _.PropertyKey,
                _.ValueDataType
            })
                .OrderBy(_ => _.Key.ComponentName)
                .ThenBy(_ => _.Key.ViewName)
                .ThenBy(_ => _.Key.PropertyKey);
            foreach (var items in infos)
            {

                Type type = Type.GetType(items.Key.ValueDataType);
                if (type != null)
                {
                    if (typeof(CompCheckBoxList) == type || typeof(CompDateFromTo) == type || typeof(CompTimeFromTo) == type || typeof(CompDateTimeFromTo) == type || typeof(CompSearchNumeric) == type || typeof(CompDropDownDataGrid) == type)
                    {
                        List<string> lstValue = [];
                        foreach (UserComponentSettingsInfo? item in items)
                        {
                            lstValue.Add(item.Value);
                        }
                        data[items.Key.PropertyKey] = lstValue;
                    }
                    else
                    {
                        foreach (UserComponentSettingsInfo? item in items)
                        {
                            data[item.PropertyKey] = item.Value;
                            break;
                        }
                    }
                }
            }
        }

        return data;
    }

    /// <summary>
    /// ClassNameSelectクラスからテーブルまたはVIEWデータを取得する
    /// </summary>
    /// <param name="select"></param>
    /// <returns></returns>
    public async Task<List<IDictionary<string, object>>> GetSelectData(ClassNameSelect select)
    {
        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        List<IDictionary<string, object>> data = [];

        if (null != resItems)
        {
            foreach (ResponseValue item in resItems)
            {
                Dictionary<string, object> newRow = [];
                for (int i = 0; item.Columns.Count > i; i++)
                {
                    newRow.Add(item.Columns[i], ConvertUtil.GetValueString(item, item.Columns[i]));
                }
                data.Add(newRow);
            }
        }
        return data;
    }



    /// <summary>
    /// ClassNameSelectクラスからテーブルまたはVIEWデータを取得する
    /// ただし、グリッド列のPropertyに登録されているデータのみ
    /// </summary>
    /// <param name="columnInfos"></param>
    /// <param name="select"></param>
    /// <returns></returns>
    public async Task<List<IDictionary<string, object>>> GetSelectGridData(IList<ComponentColumnsInfo> columnInfos, ClassNameSelect select)
    {
        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);//TODO タイムアウトの設定
        List<IDictionary<string, object>> data = [];

        if (null != resItems)
        {
            foreach (ResponseValue item in resItems)
            {
                Dictionary<string, object> newRow = [];
                // グリッド列のプロパティ名と一致するデータのみをDictionaryに追加する
                // グリッドにデータをバインドする際にエラーになる
                for (int i = 0; columnInfos.Count > i; i++)
                {
                    // columnInfosのPropertyの改行はcolumnInfosを取得する時に置換している
                    // View情報の列名と一致させるために"\\n"に変換してデータ取得を行う
                    // キーは改行をコードに置換した値とする
                    string property = columnInfos[i].Property.Replace("\n", "\\n");
                    string value = ConvertUtil.GetValueString(item, property).Replace("\\n", '\n'.ToString());
                    newRow.Add(columnInfos[i].Property, value);
                }
                data.Add(newRow);
            }
        }
        return data;
    }

    /// <summary>
    /// VIEW名称を指定して、ValueTextInfoを取得する
    /// </summary>
    /// <param name="vName"></param>
    /// <returns></returns>
    public async Task<List<ValueTextInfo>> GetValueTextInfo(string vName)
    {
        // 取得カラム名を指定する（共通）
        ClassNameSelect select = new()
        {
            viewName = vName
        };
        select.orderByParam.Add(new OrderByParam { field = "SORT_ORDER" });

        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        List<ValueTextInfo> lstInfo = [];

        if (null != resItems)
        {
            HashSet<string> columnsToExclude = ["VALUE_MEMBER", "TEXT_MEMBER", "SORT_ORDER", "BASE_ID", "BASE_TYPE", "CONSIGNOR_ID"];
            foreach (ResponseValue item in resItems)
            {
                ValueTextInfo info = new()
                {
                    Value = ConvertUtil.GetValueString(item, "VALUE_MEMBER"),
                    Text = ConvertUtil.GetValueString(item, "TEXT_MEMBER"),
                };
                List<string> columns = item.Columns.Except(columnsToExclude).ToList();
                for (int i = 0; columns.Count > i; i++)
                {
                    //パフォーマンス改善優先のため、ここはリフレクションは使用せずべた書きとした。//TODO もっと良い書き方があればそっちを採用する。
                    switch (i)
                    {
                        case 0: info.Value1 = item.Values[columns[i]]?.ToString()!; break;
                        case 1: info.Value2 = item.Values[columns[i]]?.ToString()!; break;
                        case 2: info.Value3 = item.Values[columns[i]]?.ToString()!; break;
                        case 3: info.Value4 = item.Values[columns[i]]?.ToString()!; break;
                        case 4: info.Value5 = item.Values[columns[i]]?.ToString()!; break;
                        case 5: info.Value6 = item.Values[columns[i]]?.ToString()!; break;
                        case 6: info.Value7 = item.Values[columns[i]]?.ToString()!; break;
                        case 7: info.Value8 = item.Values[columns[i]]?.ToString()!; break;
                        case 8: info.Value9 = item.Values[columns[i]]?.ToString()!; break;
                        case 9: info.Value10 = item.Values[columns[i]]?.ToString()!; break;
                    }
                    //リフレクション使用の元コード
                    //PropertyInfo? p = typeof(ValueTextInfo)?.GetProperty($"Value{i + 1}", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    //p?.SetValue(info, item.Values[columns[i]]?.ToString());
                }
                lstInfo.Add(info);
            }
        }
        return lstInfo;
    }

    /// <summary>
    /// ViewNameを取得する
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    public async Task<string> GetViewNameAsync(string className)
    {
        string? viewName = string.Empty;
        try
        {
            ClassNameSelect select = new()
            {
                viewName = "DEFINE_PAGE_VALUES"
                ,
                tsqlHints = EnumTSQLhints.NOLOCK
            };
            select.selectParam.Add("VIEW_NAME");
            select.whereParam.Add("CLASS_NAME", new WhereParam { val = className, whereType = enumWhereType.Equal });

            ResponseValue[]? resValues = await _webComService.GetResponseValue(select);
            if (null != resValues && resValues.Count() > 0)
            {
                if (resValues[0].Values.TryGetValue("VIEW_NAME", out object? obj))
                {
                    viewName = obj?.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return viewName;
    }

    /// <summary>
    /// ローカルストレージからキーをもとに値を取得する
    /// </summary>
    /// <param name="strKey"></param>
    /// <returns></returns>
    public async Task<string> GetLocalStorage(string strKey)
    {
        string param = string.Empty;
        try
        {
            param = await _localStorage.GetItemAsync<string>(strKey.ToString());
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        // 取得したキーはクリアする
        await _localStorage.RemoveItemAsync(strKey);

        return param;
    }

    /// <summary>
    /// ローカルストレージにキーバリューをセットする
    /// </summary>
    /// <param name="strKey"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task SetLocalStorage(string strKey, object value)
    {
        await _localStorage.SetItemAsync(strKey, value?.ToString());
    }
    /// <summary>
    /// すべてのローカルストレージの値をクリアする
    /// </summary>
    /// <returns></returns>
    public async Task ClearAllLocalStorageValue()
    {
        await _localStorage.ClearAsync();
    }

    #region メニュー関連

    /// <summary>
    /// メニュー情報を全て取得する
    /// </summary>
    /// <returns></returns>
    public async Task<List<MenuInfo>> GetMenuInfoAllAsync(int login_auth_level)
    {
        List<MenuInfo> lstMenuInfo = [];
        try
        {
            ClassNameSelect select = new()
            {
                viewName = "VW_メニューマスタ"
                ,
                tsqlHints = EnumTSQLhints.NOLOCK
            };
            //権限レベルを絞る
            select.whereParam.Add("AUTHORITY_LOWER", new WhereParam { val = login_auth_level.ToString(), whereType = enumWhereType.Below });//最低権限レベル以下で絞る
            ResponseValue[]? resValues = await _webComService.GetResponseValue(select);
            if (resValues is null)
            {
                throw new NullReferenceException("GetMenuInfoAllAsync_resValues is null");
            }
            foreach (ResponseValue resValue in resValues)
            {
                SetMenuInfo(resValue, out MenuInfo menuInfo);
                if (null != menuInfo)
                {
                    lstMenuInfo.Add(menuInfo);
                }
            }

        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return lstMenuInfo;
    }

    /// <summary>
    /// Navメニュー情報一覧取得
    /// </summary>
    /// <param name="isHandy"></param>
    /// <param name="bnGetAll"></param>
    /// <returns></returns>
    public async Task<List<MenuInfo>> GetMenuInfoListNavAsync(bool isHandy, bool bnGetAll = false)
    {
        List<MenuInfo> lstMenuInfo = [];
        try
        {
            List<MenuInfo> lstPc = [];
            List<MenuInfo> lstHt = [];

            if (isHandy)
            {
                // HTメニュー情報取得・追加
                // ※HTメニューはトップから２階層目と３階層目を取得
                MenuInfo infoHtTop = await GetMenuInfoHtTopAsync();
                if (infoHtTop != null)
                {
                    lstHt = await GetMenuInfo((int)TYPE_DEVICE_TYPE_GROUP.HT, infoHtTop.menuId);
                    lstMenuInfo.AddRange(lstHt);
                }
            }
            else
            {
                if (bnGetAll)
                {
                    // PCメニュー情報取得・追加
                    lstPc = await GetMenuInfo((int)TYPE_DEVICE_TYPE_GROUP.PC, string.Empty);
                    lstMenuInfo.AddRange(lstPc);

                    // HTメニュー情報取得・追加
                    // ※HTメニューはトップから２階層目と３階層目を取得
                    MenuInfo infoHtTop = await GetMenuInfoHtTopAsync();
                    if (infoHtTop != null)
                    {
                        lstHt = await GetMenuInfo(infoHtTop.DispDivision, infoHtTop.menuId);
                        lstMenuInfo.AddRange(lstHt);
                    }
                }
                else
                {
                    // PCメニュー情報取得・追加
                    lstPc = await GetMenuInfo((int)TYPE_DEVICE_TYPE_GROUP.PC, string.Empty);
                    lstMenuInfo.AddRange(lstPc);
                }
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return lstMenuInfo;
    }

    /// <summary>
    /// HTトップメニュー情報一覧を取得
    /// </summary>
    /// <returns></returns>
    public async Task<List<MenuInfo>> GetMenuInfoListHtTopAsync()
    {
        List<MenuInfo> lstMenuInfo = [];
        try
        {
            // HTメニュー情報取得・追加
            // ※HTメニューはトップから２階層目と３階層目を取得
            MenuInfo infoHtTop = await GetMenuInfoHtTopAsync();
            if (infoHtTop != null)
            {
                lstMenuInfo = await GetMenuInfo((int)TYPE_DEVICE_TYPE_GROUP.HT, infoHtTop.menuId);
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return lstMenuInfo;
    }

    /// <summary>
    /// HTメニューのトップ情報のみ取得する
    /// </summary>
    /// <returns></returns>
    public async Task<MenuInfo> GetMenuInfoHtTopAsync()
    {
        MenuInfo menuInfo = null!;
        try
        {
            // ストレージからメニュー情報を取得
            List<MenuInfo> menuAll = await _sessionStorage.GetItemAsync<List<MenuInfo>>(SharedConst.KEY_MENU_INFO);

            // HTメニューのトップ情報のみ取得する
            List<MenuInfo> lst = menuAll.Where(_ => _.DispDivision == (int)TYPE_DEVICE_TYPE_GROUP.HT && _.parentMenuId == string.Empty).OrderBy(_ => _.dispOrder).ToList();
            if (lst.Count() > 0)
            {
                menuInfo = lst[0];
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return menuInfo;
    }

    /// <summary>
    /// 指定したtypeDeviceGroupId,menuIdのメニュー情報一覧を返す
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="menuId"></param>
    /// <returns></returns>
    private async Task<List<MenuInfo>> GetMenuInfo(int groupId, string menuId)
    {
        List<MenuInfo> lstMenuInfo = [];
        try
        {
            // ストレージからメニュー情報を取得
            List<MenuInfo> menuAll = await _sessionStorage.GetItemAsync<List<MenuInfo>>(SharedConst.KEY_MENU_INFO);

            // メニュー情報の取得・追加
            List<MenuInfo> lst = menuAll.Where(_ => _.DispDivision == groupId && _.parentMenuId == menuId).OrderBy(_ => _.dispOrder).ToList();
            lstMenuInfo.AddRange(lst);

            // サブメニュー情報の取得・追加
            foreach (MenuInfo mInfo in lstMenuInfo)
            {
                List<MenuInfo> lstSub = menuAll.Where(_ => _.DispDivision == mInfo.DispDivision && _.parentMenuId == mInfo.menuId).OrderBy(_ => _.dispOrder).ToList();
                mInfo.subMenuList.AddRange(lstSub);
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }

        return lstMenuInfo;
    }

    /// <summary>
    /// クラス名からメニュー情報一覧を取得
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    public async Task<List<MenuInfo>> GetMenuInfoListAtClassAsync(string className)
    {
        List<MenuInfo> lstMenuInfo = [];
        try
        {
            if (string.IsNullOrEmpty(className))
            {
                return lstMenuInfo;
            }

            // ストレージからメニュー情報を取得
            List<MenuInfo> menuAll = await _sessionStorage.GetItemAsync<List<MenuInfo>>(SharedConst.KEY_MENU_INFO);

            // メニュー情報の取得・追加
            List<MenuInfo> lst = menuAll.Where(_ => _.className == className).OrderBy(_ => _.dispOrder).ToList();
            lstMenuInfo.AddRange(lst);

            // サブメニュー情報の取得・追加
            foreach (MenuInfo mInfo in lstMenuInfo)
            {
                List<MenuInfo> lstSub = menuAll.Where(_ => _.DispDivision == mInfo.DispDivision && _.parentMenuId == mInfo.menuId).OrderBy(_ => _.dispOrder).ToList();
                mInfo.subMenuList.AddRange(lstSub);
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }

        return lstMenuInfo;
    }

    /// <summary>
    /// MenuInfoにDBから取得した値をセットする
    /// </summary>
    /// <param name="resValue"></param>
    /// <param name="menuInfo"></param>
    private void SetMenuInfo(ResponseValue resValue, out MenuInfo menuInfo)
    {
        menuInfo = null!;
        if (resValue != null && resValue.Values != null && resValue.Columns != null)
        {
            menuInfo = new MenuInfo
            {
                menuId = ConvertUtil.GetValueString(resValue, "MENU_ID"),
                menuName = ConvertUtil.GetValueString(resValue, "MENU_NAME_STRING_KEY"),
                menuUrlString = ConvertUtil.GetValueString(resValue, "URL"),
                iconName = ConvertUtil.GetValueString(resValue, "ICON_NAME"),
                ToolTipMessage = ConvertUtil.GetValueString(resValue, "TOOL_TIP_MESSAGE"),
                dispOrder = ConvertUtil.GetValueInt(resValue, "SORT_ORDER"),
                className = ConvertUtil.GetValueString(resValue, "CLASS_NAME"),
                parentMenuId = ConvertUtil.GetValueString(resValue, "PARENT_MENU_ID"),
                DispDivision = ConvertUtil.GetValueInt(resValue, "DEVICE_GROUP_ID"),
              
            };

            if(menuInfo != null )
            {
                
            }
        }
    }


    #endregion

    #region ページタイトル

    /// <summary>
    /// ページタイトルを取得する
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    public async Task<string> GetPageTitleAsync(string className)
    {
        await Task.Delay(0);//警告の抑制
        string? pageName = string.Empty;
        try
        {
            FrozenDictionary<string, string> info = PageNameAll;
            if (null != info && info.ContainsKey(className))
            {
                pageName = info[className];
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return pageName;
    }

    #endregion


    #region PC画面遷移用パラメータ

    private readonly Dictionary<string, object> _transParamPC = [];

    /// <summary>
    /// PC画面遷移用パラメータ取得
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object? GetPCTransParam(string key)
    {
        try
        {
            object? ret = null;
            if (_transParamPC.ContainsKey(key))
            {
                ret = _transParamPC[key];
            }
            return ret;
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            return null;
        }
    }

    /// <summary>
    /// PC画面遷移用パラメータ設定
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetPCTransParam(string key, object value)
    {
        try
        {
            _ = _transParamPC.Remove(key);
            _transParamPC[key] = value;
            return true;
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            return false;
        }
    }
    /// <summary>
    /// PC画面遷移用パラメータクリア
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ClearPCTransParam(string key)
    {
        try
        {
            _ = _transParamPC.Remove(key);
            return true;
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            return false;
        }
    }

    /// <summary>
    /// PC画面遷移用パラメータ全件クリア
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ClearAllPCTransParam()
    {
        try
        {
            _transParamPC.Clear();
            return true;
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            return false;
        }
    }

    #endregion

    public Dictionary<string, (object, WhereParam)> GetCompItemInfoValues(List<List<CompItemInfo>> compItems)
    {
        Dictionary<string, (object, WhereParam)> result = [];

        foreach (List<CompItemInfo> listItem in compItems)
        {
            foreach (CompItemInfo item in listItem)
            {
                if (item.CompObj?.Instance is CompBase compBase)
                {
                    compBase.AddWhereParam(result, item.TitleLabel);
                }
            }
        }

        return result;
    }

    public void SetCompItemInputValues(List<List<CompItemInfo>> compItems, Dictionary<string, object> InitialData)
    {
        foreach (List<CompItemInfo> listItem in compItems)
        {
            foreach (CompItemInfo item in listItem)
            {
                // 初期値取得
                if (InitialData.TryGetValue(item.TitleLabel, out object? initVal))
                {
                }
                if (item.CompObj?.Instance is CompBase compBase)
                {
                    //TODO Setメソッドの中でnot null判定をしているが、そもそもこのメソッド中でnotnullのチェックしていればチェックする必要なのでは？またはコントロールによってnullをセットしたい場合があるか検討。
                    compBase.SetInitValue(initVal);
                }
            }
        }
    }

    public async Task<List<List<CompItemInfo>>> GetCompItemInfo(
        List<ComponentColumnsInfo> listInfo
        , Dictionary<string, object> InitialData
        , IList<ComponentColumnsInfo> ComponentColumns
        , IList<ComponentsInfo> Components
        , bool bSearch = true
        , bool bEdit = false
        )
    {
        List<List<CompItemInfo>> result = [];
        int layoutGroup = -1;
        List<CompItemInfo> listItem = [];
        //<ViewName,data>
        Dictionary<string, Task<List<ValueTextInfo>>> d = [];
        foreach (ComponentColumnsInfo info in listInfo)
        {
            string vname = bSearch ? info.SearchValuesViewName : info.EditValuesViewName;
            if (!string.IsNullOrEmpty(vname))
            {
                //WebAPIにリクエストを送るため、ループより先に非同期で値を取得しておく
                d[vname] = GetValueTextInfo(vname);
            }
        }

        // WMS作業日、WMS作業加算取得
        DateTime? dtWms = null;
        DateTime? dtWmsAdd = null;
        if (listInfo.Count > 0)
        {
            dtWms = await GetWmsDate();
            dtWmsAdd = await GetWmsDateAdd();
        }

        foreach (ComponentColumnsInfo info in listInfo)
        {
            string typeKey;
            Type type;
            int dialogLayoutGroup;
            bool Required = false;
            bool DispRequired = false;
            string ViewName = string.Empty;
            string AttrName = string.Empty;

            if (bSearch)
            {
                typeKey = info.SearchDataTypeKey;
                type = Type.GetType(info.SearchDataTypeKey);
                dialogLayoutGroup = info.SearchLayoutGroup;
                Required = info.InputRequired;
                ViewName = info.SearchValuesViewName;
                AttrName = ChildPageBase.STR_ATTRIBUTE_SEARCH_DATE_INIT_MODE;
            }
            else
            {
                typeKey = info.EditDataTypeKey;
                type = Type.GetType(info.EditDataTypeKey);
                dialogLayoutGroup = info.EditDialogLayoutGroup;
                Required = info.EditInputRequired;//西山
                ViewName = info.EditValuesViewName;
                AttrName = ChildPageBase.STR_ATTRIBUTE_EDIT_DATE_INIT_MODE;
            }
            if (type is null)
            {
                continue;
            }
            if (dialogLayoutGroup != layoutGroup)
            {
                layoutGroup = dialogLayoutGroup;
                listItem = [];
                result.Add(listItem);
            }

            // 検索条件取得用VIEWデータ取得
            List<ValueTextInfo>? data = null;
            if (!string.IsNullOrEmpty(ViewName))
            {
                //ループ前で取得したDBの値をawaitで取り出す
                data = await d[ViewName];
            }

            // 初期値取得
            //string initVal = string.Empty;
            //object? initVal = null;
            if (InitialData.TryGetValue(info.Property, out object? initVal))
            {
                //initVal = obj;
            }

            // タイトルラベル
            string strTitle = info.Title;

            // タイトルラベル表示非表示
            bool bnDispTitle = true;

            // 日付コンポーネントの初期化モードをDEFINE_COMPONENTSから取得する
            enumDateInitMode initMode = enumDateInitMode.None;
            ComponentsInfo? componentsInfo = Components.FirstOrDefault(_ => _.ComponentName == AttrName && _.AttributesName == info.Property);
            if (componentsInfo != null)
            {
                initMode = ConvertUtil.GetValueEnum<enumDateInitMode>(componentsInfo.Value);
            }

            // コンポーネントに渡すパラメータ生成
            Dictionary<string, object> param = new()
                {
                    { "Title", info.Title },
                    { "Required", Required }
                };
            if (bEdit)
            {
                param.Add("Disabled", true);
            }

            // 必須Suffixの表示非表示
            if (Required && !bEdit)
            {
                DispRequired = true;
            }

            if (Activator.CreateInstance(type) is CompBase compBase)
            {
                //TODO 出来ればインスタンス生成はしたくない。Staticメソッドのようにできないか。
                compBase.AddParam(param, info, ComponentColumns, initVal, data, ref bnDispTitle, initMode, dtWms, dtWmsAdd, Components);
            }

            // コンポーネント情報生成
            CompItemInfo item = new()
            {
                CompType = type,
                CompParam = param,
                TitleLabel = strTitle,
                DispTitleLabel = bnDispTitle,
                DispRequiredSuffix = DispRequired,
                ComponentName = info.ComponentName,
                ViewName = info.ViewName,
                ValueDataType = typeKey,
            };
            listItem.Add(item);

        }
        return result;
    }

    /// <summary>
    /// コンポーネントの入力値をDictionary<string, object>で取得
    /// ※複数選択値があるコンポーネントのobjectはList<string>で返る
    /// </summary>
    /// <param name="compItems"></param>
    /// <param name="bnGetEmpty">未入力の値も取得するか。true：取得する、false：取得しない</param>
    /// <returns></returns>
    public Dictionary<string, object> GetCompInputValues(List<List<CompItemInfo>> compItems, bool bnGetEmpty = false)
    {
        Dictionary<string, object> result = [];

        foreach (List<CompItemInfo> listItem in compItems)
        {
            foreach (CompItemInfo item in listItem)
            {
                if (item.CompObj?.Instance is CompBase compBase)
                {
                    compBase.AddkeyValuePair(result, item.TitleLabel, bnGetEmpty);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// コンポーネントの入力値をList<UserComponentSettingsInfo>で取得する
    /// </summary>
    /// <param name="compItems"></param>
    /// <param name="exclusionKeys"></param>
    /// <returns></returns>
    public List<UserComponentSettingsInfo> GetSettingsInfo(List<List<CompItemInfo>> compItems, List<ComponentsInfo> exclusionInfos)
    {
        List<UserComponentSettingsInfo> result = [];
        Dictionary<string, object> inputValues = GetCompInputValues(compItems);

        foreach (List<CompItemInfo> listItem in compItems)
        {
            foreach (CompItemInfo item in listItem)
            {
                // 除外リストに含まれる場合はスキップ
                if (exclusionInfos.Any(_ => _.AttributesName == item.TitleLabel))
                {
                    continue;
                }

                if (inputValues.TryGetValue(item.TitleLabel, out object? value))
                {
                    if (value is List<string> vals)
                    {
                        for (int i = 0; i < vals.Count; i++)
                        {
                            UserComponentSettingsInfo info = new()
                            {
                                ComponentName = item.ComponentName,
                                ViewName = item.ViewName,
                                PropertyKey = item.TitleLabel,
                                ValueKeyId = i + 1,
                                Value = vals[i],
                                ValueDataType = item.ValueDataType,
                            };
                            result.Add(info);
                        }
                    }
                    else
                    {
                        UserComponentSettingsInfo info = new()
                        {
                            ComponentName = item.ComponentName,
                            ViewName = item.ViewName,
                            PropertyKey = item.TitleLabel,
                            ValueKeyId = 1,
                            Value = value == null ? string.Empty : value.ToString(),
                            ValueDataType = item.ValueDataType,
                        };
                        result.Add(info);
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// コンポーネントにDisabled属性を設定する
    /// </summary>
    /// <param name="compItems"></param>
    /// <param name="bnDisabled"></param>
    public void SetCompItemListDisabled(List<List<CompItemInfo>> compItems, bool bnDisabled)
    {
        foreach (List<CompItemInfo> listItem in compItems)
        {
            foreach (CompItemInfo item in listItem)
            {
                if (item.CompObj != null && item.CompObj?.Instance is CompBase compBase)
                {
                    compBase.Disabled = bnDisabled;
                    compBase.Refresh();
                }
            }
        }
    }

    /// <summary>
    /// WMS作業日を取得する
    /// </summary>
    /// <returns></returns>
    public async Task<DateTime?> GetWmsDate()
    {
        DateTime? dtRet = null;
        try
        {
            ClassNameSelect select = new()
            {
                viewName = "WMS_STATUS"
            };
            ResponseValue[]? resValues = await _webComService.GetResponseValue(select);
            if (null != resValues && resValues.Length > 0)
            {
                string strDate = ConvertUtil.GetValueString(resValues[0], "WMS_DATE");
                if (strDate.Length == 8)
                {
                    strDate = strDate[..4] + "/" + strDate.Substring(4, 2) + "/" + strDate.Substring(6, 2);
                    if (DateTime.TryParse(strDate, out DateTime date))
                    {
                        dtRet = date;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return dtRet;
    }

    /// <summary>
    /// WMS作業日加算を取得する
    /// </summary>
    /// <returns></returns>
    public async Task<DateTime?> GetWmsDateAdd()
    {
        DateTime? dtRet = null;
        try
        {
            // WMS作業日を取得する
            dtRet = await GetWmsDate();

            if (dtRet != null)
            {
                // DEFINE_SYSTEM_PARAMETERSからWMS加算日付を取得する
                int days = 1;
                //TODO できればストレージからパラメータの取得はやめる
                //if (await _sessionStorage.ContainKeyAsync(SharedConst.KEY_SYSTEM_PARAM))
                //{
                //    SystemParameterService sysParams = await _sessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
                //}
                days = _sysParams.PC_DateInitWmsAddDays;
                dtRet = ((DateTime)dtRet).AddDays(days);
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return dtRet;
    }

    /// <summary>
    /// システム状態区分(WMS_STATUS)取得
    /// </summary>
    /// <returns>0:通常、1:日時更新中、9:オフライン</returns>
    public async Task<(int, bool)> GetSystemStatusType()
    {
        int status = -1;
        bool isAuto = false;
        try
        {
            ClassNameSelect select = new()
            {
                viewName = "WMS_STATUS"
            };
            ResponseValue[]? resValues = await _webComService.GetResponseValue(select);
            if (null != resValues && resValues.Length > 0)
            {
                status = ConvertUtil.GetValueInt(resValues[0], "SYSTEM_STATUS_TYPE");
                isAuto = ConvertUtil.GetValueBool(resValues[0], "IS_AUTO_DAILY_UPDATE");
            }
        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return (status, isAuto);
    }

    #region DEFINE関連全画面データ取得

    public async Task SetPageTitleAsyncAll()
    {
        PageNameAll = await GetPageTitleAsyncAll();
    }
    public async Task SetComponetnsInfoAll()
    {
        ComponentsInfoAll = await GetComponetnsInfoAll();
    }
    public async Task SetGridColumnsDataAll()
    {
        ComponentColumnsAll = await GetGridColumnsDataAll();
    }
    public async Task SetComponentProgramInfoAll()
    {
        ComponentProgramAll = await GetComponentProgramInfoAll();
    }

    /// <summary>
    /// ページタイトルを取得する
    /// </summary>
    /// <returns></returns>
    private async Task<FrozenDictionary<string, string>> GetPageTitleAsyncAll()
    {
        Dictionary<string, string> pageName = [];
        try
        {
            ClassNameSelect select = new()
            {
                viewName = "DEFINE_PAGE_VALUES"
                ,
                tsqlHints = EnumTSQLhints.NOLOCK
            };
            select.selectParam.Add("CLASS_NAME");
            select.selectParam.Add("PAGE_NAME");
            select.orderByParam.Add(new OrderByParam { field = "CLASS_NAME" });

            ResponseValue[]? resValues = await _webComService.GetResponseValue(select);
            if (resValues is null)
            {
                throw new Exception("GetPageTitleAsyncAll_resValues is null");
            }

            foreach (ResponseValue item in resValues)
            {
                string strClassName = ConvertUtil.GetValueString(item, "CLASS_NAME");
                pageName[strClassName] = ConvertUtil.GetValueString(item, "PAGE_NAME");
            }

        }
        catch (Exception ex)
        {
            _ = _webComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
        }
        return pageName.ToFrozenDictionary();
    }

    /// <summary>
    /// DEFINE_COMPNENTSテーブル情報を取得する
    /// </summary>
    /// <returns></returns>
    private async Task<FrozenDictionary<string, List<ComponentsInfo>>> GetComponetnsInfoAll()
    {
        // 取得カラム名を指定する（共通）
        ClassNameSelect select = new()
        {
            viewName = "DEFINE_COMPONENTS"
            ,
            tsqlHints = EnumTSQLhints.NOLOCK
        };
        select.orderByParam.Add(new OrderByParam { field = "CLASS_NAME" });

        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        Dictionary<string, List<ComponentsInfo>> data = [];
        List<ComponentsInfo> list = [];
        if (resItems is null)
        {
            throw new NullReferenceException("GetComponetnsInfoAll_resItems is null");
        }
        for (int i = 0; resItems.Length > i; i++)
        {
            ResponseValue item = resItems[i];
            string strClassName = ConvertUtil.GetValueString(item, "CLASS_NAME");
            ComponentsInfo newRow = new()
            {
                ComponentName = ConvertUtil.GetValueString(item, "COMPONENT_NAME"),
                AttributesName = ConvertUtil.GetValueString(item, "ATTRIBUTES_NAME"),
                Value = ConvertUtil.GetValueString(item, "VALUE"),
                ValueObjectType = ConvertUtil.GetValueByte(item, "VALUE_OBJECT_TYPE"),
                Type = ConvertUtil.GetValueType(item, "COMPONET_DATA_TYPE"),
                ValueMin = ConvertUtil.GetValueInt(item, "VALUE_MIN"),
                ValueMax = ConvertUtil.GetValueInt(item, "VALUE_MAX")
            };
            list.Add(newRow);
            bool bClassChange = false;
            if (resItems.Length > i + 1)
            {
                // 次のクラス名を判断する
                string strNextClassName = ConvertUtil.GetValueString(resItems[i + 1], "CLASS_NAME");
                if (strNextClassName != strClassName)
                {
                    bClassChange = true;
                }
            }
            else
            {
                bClassChange = true;
            }
            if (bClassChange)
            {
                data[strClassName] = list;
                list = [];
            }
        }
        return data.ToFrozenDictionary();
    }

    /// <summary>
    /// DEFINE_COMPNENT_COLUMNSテーブル情報を取得する
    /// 西山
    /// </summary>
    /// <returns></returns>
    private async Task<FrozenDictionary<string, List<ComponentColumnsInfo>>> GetGridColumnsDataAll()
    {
        ClassNameSelect select = new()
        {
            viewName = $"GetObjectComponetColumnsDefine(null, null, null)", //テーブルファンクションなので、dboは削除
            columnsDefineName = $"GetObjectComponetColumnsDefine"
        };
        select.OrderBy("CLASS_NAME", "COMPONENT_NAME", "VIEW_NAME", "ColumnId");

        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        Dictionary<string, List<ComponentColumnsInfo>> data = [];
        List<ComponentColumnsInfo> list = [];
        if (resItems is null)
        {
            throw new NullReferenceException("GetGridColumnsDataAll_resItems is null");
        }

        for (int i = 0; resItems.Length > i; i++)
        {
            ResponseValue item = resItems[i];
            string strClassName = ConvertUtil.GetValueString(item, "CLASS_NAME");
            ComponentColumnsInfo newRow = new()
            {
                ComponentName = ConvertUtil.GetValueString(item, "COMPONENT_NAME"),
                ViewName = ConvertUtil.GetValueString(item, "VIEW_NAME"),
                Property = ConvertUtil.GetValueString(item, "PROPERTY_KEY"),
                Title = ConvertUtil.GetValueString(item, "PROPERTY_KEY"),
                ValueMin = ConvertUtil.GetValueDecimalNull(item, "VALUE_MIN"),
                ValueMax = ConvertUtil.GetValueDecimalNull(item, "VALUE_MAX"),
                Width = ConvertUtil.GetValueInt(item, "WIDTH"),
                Type = ConvertUtil.GetValueType(item, "COMPONET_DATA_TYPE"),
                TypeText = ConvertUtil.GetValueString(item, "COMPONET_DATA_TYPE"),
                TextAlign = ConvertUtil.GetValueEnum<TextAlign>(item, "TEXT_ALIGN"),
                Resizable = ConvertUtil.GetValueBool(item, "IS_RESIZABLE"),
                Reorderable = ConvertUtil.GetValueBool(item, "IS_REORDERABLE"),
                Sortable = ConvertUtil.GetValueBool(item, "IS_SORTABLE"),
                Filterable = ConvertUtil.GetValueBool(item, "IS_FILTERABLE"),
                FormatString = ConvertUtil.GetValueString(item, "FORMAT_STRING"),
                IsEdit = ConvertUtil.GetValueBool(item, "IS_EDIT"),
                IsDataExport = ConvertUtil.GetValueBool(item, "IS_DATA_EXPORT"),
                IsSearchCondition = ConvertUtil.GetValueBool(item, "IS_SEARCH_CONDITION"),
                SummaryType = ConvertUtil.GetValueInt(item, "IS_SUMMARY"),
                SearchValuesViewName = ConvertUtil.GetValueString(item, "SEARCH_VALUES_VIEW_NAME"),
                SearchDataTypeKey = ConvertUtil.GetValueString(item, "SEARCH_DATA_TYPE_KEY"),
                InputRequired = ConvertUtil.GetValueBool(item, "SEARCH_INPUT_REQUIRED"),
                OrderbyRank = ConvertUtil.GetValueInt(item, "ORDERBY_RANK"),
                SearchLayoutGroup = ConvertUtil.GetValueInt(item, "SEARCH_LAYOUT_GROUP"),
                SearchLayoutDispOrder = ConvertUtil.GetValueInt(item, "SEARCH_LAYOUT_DISP_ORDER"),
                EditInputRequired = ConvertUtil.GetValueBool(item, "EDIT_INPUT_REQUIRED"),
                RegularExpressionString = ConvertUtil.GetValueString(item, "REGULAR_EXPRESSION_STRING"),
                EditDialogLayoutGroup = ConvertUtil.GetValueInt(item, "EDIT_DIALOG_LAYOUT_GROUP"),
                EditDialogLayoutDispOrder = ConvertUtil.GetValueInt(item, "EDIT_DIALOG_LAYOUT_DISP_ORDER"),
                EditType = ConvertUtil.GetValueInt(item, "EDIT_TYPE"),
                EditValuesViewName = ConvertUtil.GetValueString(item, "EDIT_VAUES_VIEW_NAME"),
                EditDataTypeKey = ConvertUtil.GetValueString(item, "EDIT_DATA_TYPE_KEY"),//西山
                InlineEdit = ConvertUtil.GetValueBool(item, "IS_INLINE_EDIT"),
                DataType = ConvertUtil.GetValueString(item, "DataType"),
                MaxLength = ConvertUtil.GetValueInt(item, "MaxLength"),
                Precision = ConvertUtil.GetValueInt(item, "Precision"),
                Scale = ConvertUtil.GetValueInt(item, "Scale"),
                IsNullable = ConvertUtil.GetValueBool(item, "IsNullable"),
            };

            // RadzenDataGridColumnsのPropertyはソート時に\nがある状態だとエラーが発生する
            // Sampleは改行コードを持たせていてエラーとなっていないため、それに合わせる
            newRow.Property = newRow.Property.Replace("\\n", '\n'.ToString());

            list.Add(newRow);
            bool bClassChange = false;
            if (resItems.Length > i + 1)
            {
                // 次のクラス名を判断する
                string strNextClassName = ConvertUtil.GetValueString(resItems[i + 1], "CLASS_NAME");
                if (strNextClassName != strClassName)
                {
                    bClassChange = true;
                }
            }
            else
            {
                bClassChange = true;
            }
            if (bClassChange)
            {
                data[strClassName] = list;
                list = [];
            }
        }

        return data.ToFrozenDictionary();
    }

    /// <summary>
    /// DEFINE_COMPONENT_PROGRAMSの情報を取得する
    /// </summary>
    /// <returns></returns>
    private async Task<FrozenDictionary<string, List<ComponentProgramInfo>>> GetComponentProgramInfoAll()
    {
        // 取得カラム名を指定する（共通）
        ClassNameSelect select = new()
        {
            viewName = "DEFINE_COMPONENT_PROGRAMS"
            ,
            tsqlHints = EnumTSQLhints.NOLOCK
        };
        select.orderByParam.Add(new OrderByParam { field = "CLASS_NAME" });

        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);
        Dictionary<string, List<ComponentProgramInfo>> data = [];
        List<ComponentProgramInfo> list = [];
        if (resItems is null)
        {
            throw new NullReferenceException("GetComponentProgramInfoAll_resItems is null");
        }
        for (int i = 0; resItems.Length > i; i++)
        {
            ResponseValue item = resItems[i];
            string strClassName = ConvertUtil.GetValueString(item, "CLASS_NAME");
            ComponentProgramInfo newRow = new()
            {
                CurrentMethodName = ConvertUtil.GetValueString(item, "CURRENT_METHOD_NAME"),
                CallMethodName = ConvertUtil.GetValueString(item, "CALL_METHOD_NAME"),
                ComponentName = ConvertUtil.GetValueString(item, "COMPONENT_NAME"),
                ExecOrderRank = ConvertUtil.GetValueByte(item, "EXEC_ORDER_RANK"),
                ProcessProgramName = ConvertUtil.GetValueString(item, "PROCESS_PROGRAM_NAME"),
                AuthorityLevelLower = ConvertUtil.GetValueByte(item, "AUTHORITY_LEVEL_LOWER"),
                PrgramCallType = ConvertUtil.GetValueByte(item, "PRGRAM_CALL_TYPE"),
                IsProgramReturn = ConvertUtil.GetValueByte(item, "IS_PROGRAM_RETURN"),
                RetrunDataType = ConvertUtil.GetValueType(item, "RETRUN_DATA_TYPE"),
                TimeoutValue = ConvertUtil.GetValueInt(item, "TIMEOUT_VALUE"),
                RetryCount = ConvertUtil.GetValueByte(item, "RETRY_COUNT"),
                ArgumentDataSetName = ConvertUtil.GetValueString(item, "ARGUMENT_DATA_SET_NAME"),
                IsAsync = ConvertUtil.GetValueBool(item, "IS_ASYNC")
            };
            list.Add(newRow);
            bool bClassChange = false;
            if (resItems.Length > i + 1)
            {
                // 次のクラス名を判断する
                string strNextClassName = ConvertUtil.GetValueString(resItems[i + 1], "CLASS_NAME");
                if (strNextClassName != strClassName)
                {
                    bClassChange = true;
                }
            }
            else
            {
                bClassChange = true;
            }
            if (bClassChange)
            {
                data[strClassName] = list;
                list = [];
            }
        }
        return data.ToFrozenDictionary();
    }

    #endregion

    //TODO 色を取得する処理はDEFINE_SYSTEM_PARAMで定義された色のカラム文字列で分割して処理するように変更する。いちいち定義しない。
    #region セル背景色
    /// <summary>
    /// HTML AttributesにRESULT_CATEGORYのセル背景色変更のためstyle追加
    /// </summary>
    /// <param name="strType"></param>
    /// <param name="attr"></param>
    public void AddAttrResultCatgory(string? strType, IDictionary<string, object> attr)
    {
        string strBkColor;
        bool bnWhite;
        switch (strType)
        {
            case "2":
                // 警告
                strBkColor = "#fac152";
                bnWhite = false;
                break;
            case "3":
                // 異常
                strBkColor = "#f9777f";
                bnWhite = false;
                break;
            default:
                // 正常、該当なし
                strBkColor = "#2cc8c8";
                bnWhite = false;
                break;
        }
        attr.Add("style", $"background-color: {strBkColor};");
        if (bnWhite)
        {
            attr.Add("class", "white-text");
        }
    }



    /// <summary>
    /// HTML AttributesにRESULT_CATEGORYのセル背景色変更のためstyle追加
    /// </summary>
    /// <param name="strType"></param>
    /// <param name="attr"></param>
    public void AddAttrResultCatgory2(string? strType, IDictionary<string, object> attr)
    {

        if (attr is null) throw new ArgumentNullException(nameof(attr));

        // 今日の日付（比較用）
        string today = DateTime.Today.ToString("yyyy/MM/dd");

        // strType を日付として解釈し、"yyyyMMdd" に変換
        string? strAsDate = null;
        if (DateTime.TryParse(strType, out var dt))
        {
            strAsDate = dt.ToString("yyyy/MM/dd");
        }

        // もし今日の日付と一致するなら、その色で上書き
        if (strAsDate == today)
        {
            attr["style"] = "background-color: #C0C0C0;"; // ← 好きな色に変更OK
            return;
        }

       

    }

    


    /// <summary>
    /// HTML Attributesに入荷状態のセル背景色変更のためstyle追加
    /// </summary>
    /// <param name="strType"></param>
    /// <param name="attr"></param>
    public void AddAttrArrivalStatus(string? strType, IDictionary<string, object> attr)
    {
        string strBkColor;
        bool bnWhite;
        switch (strType)
        {
            case "0":
                strBkColor = "#ff0000";     // 赤
                bnWhite = false;
                break;
            case "1":
                strBkColor = "#ffa500";     // 橙
                bnWhite = false;
                break;
            case "2":
                strBkColor = "#ffff00";     // 黄
                bnWhite = false;
                break;
            case "3":
                strBkColor = "#0000ff";     // 青
                bnWhite = true;
                break;
            case "4":
                strBkColor = "#008000";     // 緑
                bnWhite = true;
                break;
            default:
                return;
        }
        attr.Add("style", $"background-color: {strBkColor};");
        if (bnWhite)
        {
            attr.Add("class", "white-text");
        }
    }

    /// <summary>
    /// HTML Attributesに出荷状態のセル背景色変更のためstyle追加
    /// </summary>
    /// <param name="strType"></param>
    /// <param name="attr"></param>
    public void AddAttrShipmentStatus(string? strType, IDictionary<string, object> attr)
    {
        string strBkColor;
        bool bnWhite;
        switch (strType)
        {
            case "0":
                strBkColor = "#ff0000";     // 赤
                bnWhite = false;
                break;
            case "1":
                strBkColor = "#ffa500";     // 橙
                bnWhite = false;
                break;
            case "2":
                strBkColor = "#ffff00";     // 黄
                bnWhite = false;
                break;
            case "3":
                strBkColor = "#008000";     // 緑
                bnWhite = true;
                break;
            case "4":
                strBkColor = "#800080";     // 紫
                bnWhite = true;
                break;
            case "5":
                strBkColor = "#0000ff";     // 青
                bnWhite = true;
                break;
            default:
                return;
        }
        attr.Add("style", $"background-color: {strBkColor};");
        if (bnWhite)
        {
            attr.Add("class", "white-text");
        }
    }

    /// <summary>
    /// HTML Attributesに確定状態のセル背景色変更のためstyle追加
    /// </summary>
    /// <param name="strType"></param>
    /// <param name="attr"></param>
    public void AddAttrConfirmStatus(string? strType, IDictionary<string, object> attr)
    {
        string strBkColor;
        bool bnWhite;
        switch (strType)
        {
            case "True":
                strBkColor = "#223a70";     // 紺
                bnWhite = true;
                break;
            default:
                return;
        }
        attr.Add("style", $"background-color: {strBkColor};");
        if (bnWhite)
        {
            attr.Add("class", "white-text");
        }
    }

    /// <summary>
    /// HTML Attributesに実績送信状態のセル背景色変更のためstyle追加
    /// </summary>
    /// <param name="strType"></param>
    /// <param name="attr"></param>
    public void AddAttrSendResultsStatus(string? strType, IDictionary<string, object> attr)
    {
        string strBkColor;
        bool bnWhite;
        switch (strType)
        {
            case "True":
                strBkColor = "#223a70";     // 紺
                bnWhite = true;
                break;
            default:
                return;
        }
        attr.Add("style", $"background-color: {strBkColor};");
        if (bnWhite)
        {
            attr.Add("class", "white-text");
        }
    }

    /// <summary>
    /// HTML Attributesに開始時間、締切時間の警告・超過のセル背景色を追加
    /// </summary>
    /// <param name="strRemainingTime">残時間[分]</param>
    /// <param name="intWarnTime">警告時間[分]</param>
    /// <param name="attr"></param>
    public void AddAttrWarnExcessTime(string? strRemainingTime, int intWarnTime, IDictionary<string, object> attr)
    {
        if (!int.TryParse(strRemainingTime, out int intRemainingTime))
        {
            return;
        }

        if (0 >= intRemainingTime)
        {
            // 超過
            attr.Add("style", $"background-color: #ff0000;");
        }
        else if (intWarnTime >= intRemainingTime)
        {
            // 警告
            attr.Add("style", $"background-color: #ffff00;");
        }
    }
    #endregion

    #region メッセージ関連

    public async Task DialogShowOK(string message, string title = "確認", int? width = null, int? height = null)
    {
        await DialogShowOK(new Dictionary<string, object> { { "MessageContent", message } }, title, width ?? DialogShowOKWidth, height ?? DialogShowOKHeight);
    }

    public async Task DialogShowOK(Dictionary<string, object> attr, string title = "確認", int width = 350, int height = 200)
    {
        title = title.Replace("\\n", "");
        await _dialogService.OpenAsync<DialogMsgOKContent>(title,
            attr,
            new DialogOptions() { Width = $"{width}px", Height = $"{height}px", Resizable = false, Draggable = false, WrapperCssClass = "dialog-more-front" });
    }

    public async Task<bool> DialogShowYesNo(string message, string title = "確認", int? width = null, int? height = null)
    {
        return await DialogShowYesNo(new Dictionary<string, object> { { "MessageContent", message } }, title, width ?? DialogShowYesNoWidth, height ?? DialogShowYesNoHeight);
    }

    public async Task<bool> DialogShowYesNo(Dictionary<string, object> attr, string title = "確認", int width = 350, int height = 200)
    {
        title = title.Replace("\\n", "");
        bool? ret = await _dialogService.OpenAsync<DialogMsgYesNoContent>(title,
            attr,                                                                                       
            new DialogOptions() { Width = $"{width}px", Height = $"{height}px", Resizable = false, Draggable = false, WrapperCssClass = "dialog-more-front" });

        bool retb = ret is not null && (bool)ret;

        return retb;
    }

    public async Task<bool> DialogShowPassword(string message, string title = "確認", int? width = null, int? height = null)
    {
        return await DialogShowPassword(new Dictionary<string, object> { { "MessageContent", message } }, title, width ?? DialogShowPasswordWidth, height ?? DialogShowPasswordHeight);
    }

    public async Task<bool> DialogShowPassword(string message, int userAuthorityLevel = 5,string title = "確認", int? width = null, int? height = null)
    {
        return await DialogShowPassword(new Dictionary<string, object> { { "MessageContent", message }, { "UserAuthorityLevel", userAuthorityLevel } }, title, width ?? DialogShowPasswordWidth, height ?? DialogShowPasswordHeight);
    }

    public async Task<bool> DialogShowPassword(Dictionary<string, object> attr, string title = "確認", int width = 350, int height = 600)
    {
        title = title.Replace("\\n", "");
        bool? ret = await _dialogService.OpenAsync<DialogMsgPasswordContent>(title,
            attr,
            new DialogOptions() { Width = $"{width}px", Height = $"{height}px", Resizable = false, Draggable = false, WrapperCssClass = "dialog-more-front" });

        bool retb = ret is not null && (bool)ret;

        return retb;
    }

    public async Task DialogShowBusy(string message = "処理中..", int? width = null, int? height = null)
    {
        await DialogShowBusy(new Dictionary<string, object> { { "MessageContent", message } }, width ?? MessageDialogWidth, height ?? MessageDialogHeight);
    }

    public async Task DialogShowBusy(Dictionary<string, object> attr, int width = 200, int height = 155)
    {
        await _dialogService.OpenAsync<DialogBusyContent>(string.Empty,
            attr,
            new DialogOptions()
            {
                Width = $"{width}px",
                Height = $"{height}px",
                Resizable = false,
                Draggable = false,
                ShowTitle = false,
                ShowClose = false,
                Style = "min-height:auto;min-width:auto;width:auto",
                CloseDialogOnEsc = false,
                WrapperCssClass = "dialog-more-front"
            });
    }

    public async Task DialogClose()
    {
        _dialogService.Close();
        await Task.Delay(0);
    }

    /// <summary>
    /// Notifyでメッセージ表示
    /// </summary>
    /// <param name="severity"></param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    public void ShowNotifyMessege(ToastType severity, string title, string message)
    {
        _toasterService.Add(
            severity
            , message
            , title
            , configure =>
            {
                configure.VisibleStateDuration = _sysParams is null ? DEFAULT_NOTIFY_DURATION : _sysParams.NotifyPopupDuration;
            });
    }

    #endregion
}

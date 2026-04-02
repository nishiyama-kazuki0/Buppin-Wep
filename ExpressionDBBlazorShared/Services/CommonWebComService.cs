using Blazored.SessionStorage;
using ExpressionDBBlazorShared.Data;
using SharedModels;

namespace ExpressionDBBlazorShared.Services;

public class CommonWebComService : WebAPIService
{
    private readonly string strUrl = $"/Common";
    private readonly string strCommonGenericUrl = $"/CommonGeneric";
    private readonly string strExecutionUrl = $"/Execution";
    /// <summary>
    /// ISessionStorageService
    /// </summary>
    private readonly ISessionStorageService _sessionStorage;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="httpClient"></param>
    public CommonWebComService(HttpClient httpClient, ISessionStorageService sessionStorage) : base(httpClient)
    {
        _sessionStorage = sessionStorage;
    }
    public async Task<ResponseValue[]?> GetResponseValue(ClassNameSelect select, string path = "", int timeout = 100000)
    {
        // データを取得する場合、WHERE句の条件に支所場コード、所場区分、荷主IDは必ず追加するように対応
        // WebAPI側でSELECTする列情報に支所場コード等が含まれている場合は、WHERE句で使用するようにしています。
        if (await _sessionStorage.ContainKeyAsync(SharedConst.KEY_SYSTEM_PARAM))
        {
            //TODO できればストレージからパラメータの取得はやめる
            _ = await _sessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
            //select.whereParam[SharedConst.KEY_BASE_ID] = new WhereParam { val = sysParams.BaseId };
            //select.whereParam[SharedConst.KEY_BASE_TYPE] = new WhereParam { val = sysParams.BaseType.ToString() };
            //select.whereParam[SharedConst.KEY_CONSIGNOR_ID] = new WhereParam { val = sysParams.ConsignorId };
        }
        return await GetResponse(select, strUrl, path, timeout);
    }
    public async Task<IEnumerable<T>?> GetGenericResponseValue<T>(ClassNameSelect select, string path = "", int timeout = 100000)
    {
        // データを取得する場合、WHERE句の条件に支所場コード、所場区分、荷主IDは必ず追加するように対応
        // WebAPI側でSELECTする列情報に支所場コード等が含まれている場合は、WHERE句で使用するようにしています。
        if (await _sessionStorage.ContainKeyAsync(SharedConst.KEY_SYSTEM_PARAM))
        {
            //TODO できればストレージからパラメータの取得はやめる
            _ = await _sessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
            //select.whereParam[SharedConst.KEY_BASE_ID] = new WhereParam { val = sysParams.BaseId };
            //select.whereParam[SharedConst.KEY_BASE_TYPE] = new WhereParam { val = sysParams.BaseType.ToString() };
            //select.whereParam[SharedConst.KEY_CONSIGNOR_ID] = new WhereParam { val = sysParams.ConsignorId };
        }
        return await GetGenericResponse<T>(select, strCommonGenericUrl, path, timeout);
    }
    public async Task<ExecResult[]?> SetRequestValue(string className, RequestValue request, string path = "", int timeout = 100000)
    {
        // クラス名を設定
        _ = request.SetArgumentValue(SharedConst.KEY_CLASS_NAME, className, "");
        // ストアドを実行するときは、ユーザID、支所場コード、所場区分、荷主IDは必ず追加するように対応
        // ストアドに必要な場合は、DEFINE_PROCESS_FUNCTIONのARGUMENT_NAME1～30にそれぞれ設定すること
        // WebAPI側でARGUMENT_NAME1～30に設定されている名称がキーとなっている値を使用してストアドを実行しています。
        if (await _sessionStorage.ContainKeyAsync(SharedConst.KEY_LOGIN_INFO))
        {
            LoginInfo login = await _sessionStorage.GetItemAsync<LoginInfo>(SharedConst.KEY_LOGIN_INFO);
            _ = request.SetArgumentValue(SharedConst.KEY_USER_ID, login.Id, "");
        }
        if (await _sessionStorage.ContainKeyAsync(SharedConst.KEY_SYSTEM_PARAM))
        {
            //TODO できればストレージからパラメータの取得はやめる
            _ = await _sessionStorage.GetItemAsync<SystemParameterService>(SharedConst.KEY_SYSTEM_PARAM);
            //_ = request.SetArgumentValue(SharedConst.KEY_BASE_ID, sysParams.BaseId, "");
            //_ = request.SetArgumentValue(SharedConst.KEY_BASE_TYPE, sysParams.BaseType.ToString(), "");
            //_ = request.SetArgumentValue(SharedConst.KEY_CONSIGNOR_ID, sysParams.ConsignorId, "");
        }
        return await RequestExecute(request, strExecutionUrl, path, timeout);
    }
}

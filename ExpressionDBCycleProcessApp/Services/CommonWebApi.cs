using SharedModels;

namespace ExpressionDBCycleProcessApp.Services;

/// <summary>
/// (共通)WebAPIへのアクセス処理
/// </summary>
public class CommonWebApi : WebAPIService
{
    private readonly string strUrl = $"/Common";
    private readonly string strCommonGenericUrl = $"/CommonGeneric";
    private readonly string strExecutionUrl = $"/Execution";
    private readonly string strReceiveDevicesUrl = $"/ReceiveDevices";

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="httpClient"></param>
    public CommonWebApi(HttpClient httpClient) : base(httpClient)
    {
    }

    /// <summary>
    /// ClassNameSelectクラスによるWebAPIとのやり取り
    /// </summary>
    /// <param name="select"></param>
    /// <param name="path"></param>
    /// <param name="timeout">[ms]デフォルトはhttpclientの100秒</param>
    /// <returns></returns>
    public async Task<ResponseValue[]?> GetResponseValue(ClassNameSelect select, string path = "", int timeout = 100000)
    {
        return await GetResponse(select, strUrl, path, timeout);
    }

    /// <summary>
    /// ClassNameSelectクラスによるWebAPIとのやり取り（ジェネリクス）
    /// </summary>
    /// <param name="select"></param>
    /// <param name="path"></param>
    /// <param name="timeout">[ms]デフォルトはhttpclientの100秒</param>
    /// <returns></returns>
    public async Task<IEnumerable<T>?> GetGenericResponseValue<T>(ClassNameSelect select, string path = "", int timeout = 100000)
    {
        return await GetGenericResponse<T>(select, strCommonGenericUrl, path, timeout);
    }

    /// <summary>
    /// RequestValueクラスによるWebAPIとのやり取り（ストアド実行）
    /// </summary>
    /// <param name="className"></param>
    /// <param name="request"></param>
    /// <param name="path"></param>
    /// <param name="timeout">[ms]デフォルトはhttpclientの100秒</param>
    /// <returns></returns>
    public async Task<ExecResult[]?> SetRequestValue(string className, RequestValue request, string path = "", int timeout = 100000)
    {
        // クラス名を設定
        _ = request.SetArgumentValue(SharedConst.KEY_CLASS_NAME, className, "");
        return await RequestExecute(request, strExecutionUrl, path, timeout);
    }

    public async Task SendBynaryData(byte[] data)
    {
        await SendBynaryData(strReceiveDevicesUrl, data);
    }
}

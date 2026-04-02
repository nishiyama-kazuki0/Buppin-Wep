using Newtonsoft.Json;
using SharedModels;
using System.Net.Http.Json;
using System.Text;
using static SharedModels.SharedConst;

namespace ExpressionDBBlazorShared.Services;

/// <summary>
/// WebAPIサービスとやり取りするサービス
/// </summary>
public class WebAPIService
{
    /// <summary>
    /// HttpClient
    /// </summary>
    private readonly HttpClient _httpClient;


    /// <summary>
    /// ルートパス
    /// </summary>
    public static string RootPath = "";
    /// <summary>
    /// AppDataパス
    /// </summary>
    //public static string AppDataPath { get { return Path.Combine(WebAPIService.RootPath, "App_Data\\物品管理ラベル.xlsx"); } }

    public static string AppDataPath { get { return Path.Combine(WebAPIService.RootPath, "ExpressionDBWebAPI\\App_Data\\物品管理ラベル.xlsx"); } }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="httpClient"></param>
    public WebAPIService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// ClassNameSelectクラスによるWebAPIとのやり取り
    /// </summary>
    /// <param name="select"></param>
    /// <param name="url"></param>
    /// <param name="path"></param>
    /// <param name="timeout">[ms]デフォルトはhttpclientの100秒</param>
    /// <returns></returns>
    public async Task<ResponseValue[]?> GetResponse(ClassNameSelect select, string url, string path, int timeout = 100000)
    {
        try
        {
            CancellationTokenSource cts = timeout <= 0 ?
                new CancellationTokenSource() //無制限
                : new CancellationTokenSource(timeout);
            string json = JsonConvert.SerializeObject(select);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url + (string.IsNullOrEmpty(path) ? "" : "/" + path), content, cts.Token); //contentを引数で送信したいので、処理的にはGetだがPostとする
            ResponseValue[]? resItems = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ResponseValue[]>()
                : throw new Exception(response.ReasonPhrase);
            return resItems;
        }
        catch (Exception ex)
        {
            _ = PostLogAsync(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// ClassNameSelectクラスによるWebAPIとのやり取り
    /// </summary>
    /// <param name="select"></param>
    /// <param name="url"></param>
    /// <param name="path"></param>
    /// <param name="timeout">[ms]デフォルトはhttpclientの100秒</param>
    /// <returns></returns>
    public async Task<IEnumerable<T>?> GetGenericResponse<T>(ClassNameSelect select, string url, string path, int timeout = 100000)
    {
        try
        {
            CancellationTokenSource cts = timeout <= 0 ?
                new CancellationTokenSource() //無制限
                : new CancellationTokenSource(timeout);

            select.modelTypeName = typeof(T).AssemblyQualifiedName ?? string.Empty;
            string json = JsonConvert.SerializeObject(select);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url + (string.IsNullOrEmpty(path) ? "" : "/" + path), content, cts.Token); //contentを引数で送信したいので、処理的にはGetだがPostとする
            IEnumerable<T>? resItems = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<IEnumerable<T>?>()
                : throw new Exception(response.ReasonPhrase);
            return resItems;
        }
        catch (Exception ex)
        {
            _ = PostLogAsync(ex.Message);
            throw;
        }
    }

    /// <summary>
    /// RequestValueクラスによるWebAPIとのやり取り
    /// </summary>
    /// <param name="select"></param>
    /// <param name="url"></param>
    /// <param name="path"></param>
    /// <param name="timeout">[ms]デフォルトはhttpclientの100秒</param>
    /// <returns></returns>
    public async Task<ExecResult[]?> RequestExecute(RequestValue request, string url, string path, int timeout = 100000)
    {
        try
        {
            CancellationTokenSource cts = timeout <= 0 ?
                new CancellationTokenSource() //無制限
                : new CancellationTokenSource(timeout);
            string json = JsonConvert.SerializeObject(request);
            StringContent content = new(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url + (string.IsNullOrEmpty(path) ? "" : "/" + path), content, cts.Token);
            ExecResult[]? resItems = response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<ExecResult[]>()
                : throw new Exception(response.ReasonPhrase);
            return resItems;
        }
        catch (Exception ex)
        {
            _ = PostLogAsync(ex.Message);
            throw;
        }
    }
    /// <summary>
    /// LogControllerへログを送信する
    /// </summary>
    /// <param name="message"></param>
    /// <param name="timeout">[ms]デフォルトはhttpclientの100秒</param>
    /// <returns></returns>c
    public async Task PostLogAsync(string message, TYPE_LOGGER typeLogger = TYPE_LOGGER.INFO, int timeout = 100000)
    {
        try
        {
            CancellationTokenSource cts = timeout <= 0 ?
                new CancellationTokenSource() //無制限
                : new CancellationTokenSource(timeout);
            string jsonContent = JsonConvert.SerializeObject(new LoggerRequestValue() { Messgae = message, TypeLogger = typeLogger });
            StringContent content = new(jsonContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("/Log", content, cts.Token);
        }
        catch (Exception)
        {
            //ログ送信に失敗したとしても何もしない
            return;
        }
    }
    /// <summary>
    /// UploadUrlを得る
    /// </summary>
    /// <returns></returns>
    public string GetUploadUrl()
    {
        try
        {
            string strUrl = _httpClient.BaseAddress + "Upload/upload/single";
            return strUrl;
        }
        catch (Exception ex)
        {
            _ = PostLogAsync(ex.Message);
            return "";
        }
    }


}

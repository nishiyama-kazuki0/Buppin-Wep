using Newtonsoft.Json;
using SharedModels;
using System.Net.Http.Json;
using System.Text;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Services
{
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
        public async Task<ResponseValue[]?> GetResponseValue(ClassNameSelect select, string url, string path, int timeout = 100000)
        {
            try
            {
                CancellationTokenSource cts = timeout <= 0 ?
                    new CancellationTokenSource() //無制限
                    : new CancellationTokenSource(timeout);
                string json = JsonConvert.SerializeObject(select);
                StringContent content = new(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(url + (string.IsNullOrEmpty(path) ? "" : "/" + path), content, cts.Token);
                ResponseValue[]? resItems = null;
                if (response.IsSuccessStatusCode)
                {
                    resItems = await response.Content.ReadFromJsonAsync<ResponseValue[]>();
                }
                return resItems;
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
                return null;
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
        public async Task<ExecResult[]?> SetRequestValue(RequestValue request, string url, string path, int timeout = 100000)
        {
            try
            {
                CancellationTokenSource cts = timeout <= 0 ?
                    new CancellationTokenSource() //無制限
                    : new CancellationTokenSource(timeout);
                string json = JsonConvert.SerializeObject(request);
                StringContent content = new(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(url + (string.IsNullOrEmpty(path) ? "" : "/" + path), content, cts.Token);
                ExecResult[]? resItems = null;
                if (response.IsSuccessStatusCode)
                {
                    resItems = await response.Content.ReadFromJsonAsync<ExecResult[]>();
                }
                return resItems;
            }
            catch (Exception ex)
            {
                _ = PostLogAsync(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// LogControllerへログを送信する
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timeout">[ms]デフォルトはhttpclientの100秒</param>
        /// <returns></returns>
        public async Task PostLogAsync(string message, int timeout = 100000)
        {
            try
            {
                CancellationTokenSource cts = timeout <= 0 ?
                    new CancellationTokenSource() //無制限
                    : new CancellationTokenSource(timeout);
                StringContent content = new(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("/Log", content, cts.Token);
            }
            catch (Exception)
            {
                //ログ送信に失敗したとしても何もしない
                return;
            }
        }
        /// <summary>
        /// LogControllerへログを送信する
        /// </summary>
        /// <param name="message"></param>
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
}

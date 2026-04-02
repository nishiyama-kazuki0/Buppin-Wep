using Microsoft.Data.SqlClient;
using System.Data;

namespace ZennohWebAPI.Common
{
    public class CommonFunc
    {
        public static void SetValues<T>(T obj, IDictionary<string, object> values)
        {
            Type type = typeof(T);

            foreach (string key in values.Keys)
            {
                System.Reflection.PropertyInfo? prop = type.GetProperty(key);
                prop?.SetValue(obj, values[key]);
            }
        }
        /// <summary>
        /// HttpRequestからデバイス名を得る。HttpRequestがnullの場合は空文字を返す。DNSが存在せずホスト名が得られないときは、IPv4文字列を返す。
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static string GetDeviceID(HttpRequest? req)
        {
            using SqlConnection con = DataSource.GetNewConnection();
            return GetDeviceID(req, con);
        }
        /// <summary>
        /// HttpRequestからデバイス名を得る。HttpRequestがnullの場合は空文字を返す。DNSが存在せずホスト名が得られないときは、IPv4文字列を返す。
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static string GetDeviceID(HttpRequest? req, SqlConnection connection)
        {
            string deviceID = string.Empty;
            if (req is null)
            {
                // ファイル取込等で直接呼ばれている場合は、DEVICE_IDは呼び出しもとで設定されているものとします
                return deviceID;
            }

            // 接続先のデバイス名を取得する
            System.Net.IPAddress? remoteIpAddress = req.HttpContext.Connection.RemoteIpAddress;
            if (remoteIpAddress is not null)
            {
                //LogTo.Information($"remoteIpAddress:{remoteIpAddress}");
                string? remotePcName;
                //try
                //{
                //    remotePcName = System.Net.Dns.GetHostEntry(remoteIpAddress).HostName;
                //}
                //catch (Exception ex)
                //{
                //    LogTo.Warning($"System.Net.Dns.GetHostEntry_FATL_ERR:{ex.Message}");
                //    //TODO この対応で他の整合性が合うか確認する必要がある
                //    //DNSでホスト名が取れない場合はIPアドレスを格納しておく
                //    remotePcName = remoteIpAddress.MapToIPv4().ToString();
                //}
                string remoteIp = remoteIpAddress.MapToIPv4().ToString();

                remotePcName = DataSource.GetScalarValueNonQuery(
                    connection
                    , "SELECT DEVICE_ID FROM MST_DEVICES WITH (NOLOCK) WHERE IP = @remoteIp" //TODO SQL Server以外はSQLを見直す必要あり
                    , new Dictionary<string, object?>() {
                        { "remoteIp", new SqlParameter($"{DataSource.ParamPrefixStr}remoteIp"
                                                                                , SqlDbType.VarChar
                                                                                , 15
                                                                                ){Value = $"{remoteIp}"}
                        }
                    }
                    , null
                );

                deviceID = string.IsNullOrWhiteSpace(remotePcName) ? remoteIp : remotePcName;
                //_ = reqValue.SetArgumentValue("DEVICE_ID", remotePcName, "");
            }
            else
            {
                //LogTo.Information($"remoteIpAddress is null");
                deviceID = "UnkownClientDevice";
            }

            return deviceID;
        }
    }
}

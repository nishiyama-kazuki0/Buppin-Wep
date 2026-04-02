using Anotar.Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ZennohCycleProcessApp.Data;
using ZennohWebAPI.Common;

namespace ZennohWebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CycleProcessController : ControllerBase
    {
        /// <summary>
        /// CycleProcessInfoのリストを取得する
        /// 呼び出しURLはApiController属性なので、CycleProcessとなる
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<CycleProcessInfo>> Get()
        {
            try
            {
                IEnumerable<CycleProcessInfo> ret = GetCycleProcessInfo(category: 1); //周期処理から呼ばれたものは1とした。他にも送信受信などで分ける必要がある場合は、カテゴリーを増やす

                return Ok(ret); // 200 OK ステータスコードとデータを返す
            }
            catch (Exception ex)
            {
                LogTo.Fatal(ex.Message);
                return StatusCode(500, ex.Message); // 500 Internal Server Error ステータス
            }
        }

        /// <summary>
        /// データ出力用に定義したCycleProcessInfoのリストを取得する
        /// </summary>
        /// <returns></returns>
        [NonAction]
        internal static IEnumerable<CycleProcessInfo> GetCycleProcessInfo(object? cycleId = null, object? category = null)
        {
            return DataSource.GetEntityCollection<CycleProcessInfo>(
                "SELECT * FROM GetCycleProcessState(@TargetCycleId,@TargetCategory) ORDER BY SORT_ORDER"
                , new Dictionary<string, object?>() {
                       { "TargetCycleId", new SqlParameter($"{DataSource.ParamPrefixStr}TargetCycleId"
                                                                                , SqlDbType.TinyInt
                                                                                ){Value = cycleId ?? DBNull.Value}//nullは指定なし
                        },
                        { "TargetCategory", new SqlParameter($"{DataSource.ParamPrefixStr}TargetCategory"
                                                                                , SqlDbType.TinyInt
                                                                                ){Value = category ?? DBNull.Value}//nullは指定なし 
                        },
                    }
                );
        }
    }
}

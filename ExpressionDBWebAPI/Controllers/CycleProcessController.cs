using Anotar.Serilog;
using ExpressionDBWebAPI.Common;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Data;

namespace ExpressionDBWebAPI.Controllers;

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
        string[] paramName = ["TargetCycleId", "TargetCategory"];
        return DataSource.GetEntityCollection<CycleProcessInfo>(
             DataSource.CreateTableFunctionQuery("GetCycleProcessState", paramName, [("SORT_ORDER", false)])
            , DataSource.CreateParamTable(new Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)>
                    {
                        { "TargetCycleId", (cycleId ,null, typeof(int),null) },
                        { "TargetCategory", (category ,null, typeof(int),null) }
                    }
                )
            );
    }
}

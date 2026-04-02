using Anotar.Serilog;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Reflection;

namespace ExpressionDBWebAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class CommonGenericController : ControllerBase
{

    /// <summary>
    /// Tのリストを取得する
    /// 呼び出しURLはApiController属性なので、CommonGenericとなる
    /// contentを引数で受信したいので、処理的にはGetだがPostとする
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<IEnumerable<AbstractViewModelBase>> Post([FromBody] ClassNameSelect selectInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            // リフレクションを使用してジェネリックメソッドを取得

            MethodInfo methodInfo = typeof(CommonController).GetMethod("GetGenericResponseValue");
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(selectInfo.GetModelType);
            IEnumerable<AbstractViewModelBase> ret = (IEnumerable<AbstractViewModelBase>)genericMethod.Invoke(null, new object[] { selectInfo });

            cancellationToken.ThrowIfCancellationRequested();
            return Ok(ret); // 200 OK ステータスコードとデータを返す
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
            return StatusCode(500, ex.Message); // 500 Internal Server Error ステータス
        }
    }

}

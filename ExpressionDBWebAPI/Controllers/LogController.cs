using Anotar.Serilog;
using ExpressionDBWebAPI.Common;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using static SharedModels.SharedConst;

namespace ExpressionDBWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    /// <summary>
    /// POSTでクライアントから送信された文字列をserilogのlogto.infomationでログに出力する。
    /// クライアント側は応答の結果は気にしないとする。PostAsync
    /// 端末名、送信元の情報もメッセージに乗せて受信とする。できればログレベルも可変にしたい。
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [HttpPost]
    public IActionResult Post([FromBody] LoggerRequestValue req, CancellationToken cancellationToken = default)
    {
        try //webAPI自身が落ちては困るので、念のためtry-catchしておく
        {
            cancellationToken.ThrowIfCancellationRequested();
            string deviceId = CommonFunc.GetDeviceID(Request);
            switch (req.TypeLogger)
            {
                case TYPE_LOGGER.INFO:
                    LogTo.Information($"ClientDeviceId:{deviceId},postMsg:{req.Messgae}");
                    break;
                case TYPE_LOGGER.WARM:
                    LogTo.Warning($"ClientDeviceId:{deviceId},postMsg:{req.Messgae}");
                    break;
                case TYPE_LOGGER.FATAL:
                    LogTo.Fatal($"ClientDeviceId:{deviceId},postMsg:{req.Messgae}");
                    break;
                default:
                    LogTo.Information($"ClientDeviceId:{deviceId},postMsg:{req.Messgae}");
                    break;
            }

        }
        catch (Exception ex) { LogTo.Fatal(ex.Message); }
        return NoContent();
    }

}

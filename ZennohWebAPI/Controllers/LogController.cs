using Anotar.Serilog;
using Microsoft.AspNetCore.Mvc;
using ZennohWebAPI.Common;

namespace ZennohWebAPI.Controllers
{
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
        public IActionResult Post([FromBody] string message, CancellationToken cancellationToken = default)
        {
            try //webAPI自身が落ちては困るので、念のためtry-catchしておく
            {
                cancellationToken.ThrowIfCancellationRequested();
                string deviceId = CommonFunc.GetDeviceID(Request);
                LogTo.Information($"ClientDeviceId:{deviceId},postMsg:{message}");
            }
            catch (Exception ex) { LogTo.Fatal(ex.Message); }
            return NoContent();
        }

    }
}

using Anotar.Serilog;
using Microsoft.AspNetCore.Mvc;

namespace ExpressionDBWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ReceiveDevicesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post()
    {
        using var memoryStream = new MemoryStream();
        await Request.Body.CopyToAsync(memoryStream);

        byte[] receivedData = memoryStream.ToArray();
        int length = receivedData.Length;

        // 必要に応じて処理・保存
        LogTo.Warning($"受信サイズ: {length} バイト");

        return Ok(new { Message = $"Received {length} bytes" });
    }
}

using Anotar.Serilog;
using ExpressionDBWebAPI.Common;
using ExpressionDBWebAPI.ReportUtil;
using Microsoft.AspNetCore.Mvc;

namespace ExpressionDBWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IWebHostEnvironment environment;

    public ReportsController(IWebHostEnvironment environment)
    {
        this.environment = environment;
    }

    // Single file upload
    [HttpPost("upload/single")]
    public async Task<IActionResult> Single(IFormFile file)
    {
        try
        {
            LogTo.Information("{@file}", file);

            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select a file");
            }

            // 接続先のデバイス名を取得する
            string remotePcName = CommonFunc.GetDeviceID(Request);

            string path = await UploadFile(file, remotePcName);
            return StatusCode(200, path);
        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    [NonAction]
    public async Task<string> UploadFile(IFormFile file, string deviceId)
    {
        string filePath = $"/Upload/{deviceId}";
        string uploadPath = environment.WebRootPath + filePath;
        if (!Directory.Exists(uploadPath))
        {
            _ = Directory.CreateDirectory(uploadPath);
        }

        string fullPath = Path.Combine(uploadPath, file.FileName);
        using (FileStream stream = new(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fullPath;
    }

    // using Microsoft.AspNetCore.Mvc;

  

}

using ZennohBlazorShared;

namespace ZennohBlazorWasmApp;

public class PlatformNameProvider : IPlatformNameProvider
{
    public string GetPlatformName()
    {
        return "ASP.NET Core Blazor WebAssembly";
    }
}

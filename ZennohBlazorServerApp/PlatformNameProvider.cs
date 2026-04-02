using ZennohBlazorShared;

namespace ZennohBlazorServerApp;

public class PlatformNameProvider : IPlatformNameProvider
{
    public string GetPlatformName()
    {
        return "ASP.NET Core Blazor Server";
    }
}

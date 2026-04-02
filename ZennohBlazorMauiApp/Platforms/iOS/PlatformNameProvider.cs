using ZennohBlazorShared;

namespace ZennohBlazorMauiApp;
public class PlatformNameProvider : IPlatformNameProvider
{
    public string GetPlatformName()
    {
        return "iOS";
    }
}

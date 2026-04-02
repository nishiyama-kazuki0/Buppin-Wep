using Microsoft.JSInterop;
using static SharedModels.SharedConst;

namespace ExpressionDBBlazorShared.Services;

internal class ScannerFactory
{
    public static IScannerService CreateScannerService(DeviceInfoService deviceInfo, IJSRuntime js)
    {
        return deviceInfo.DeviceType switch
        {
            TYPE_DEVICE_TYPE.KEYENCE_DEVICE => new KeyenceScannerService(js),
            TYPE_DEVICE_TYPE.CASIO_DEVICE => new CasioScannerService(js),
            TYPE_DEVICE_TYPE.DENSOWAVE_DEVICE => new DensoWaveScannerService(js),
            _ => new CameraScannerService(js),
        };
    }
}

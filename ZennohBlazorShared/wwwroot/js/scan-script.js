function scanCallbackFunction(result) {
    if (result.mStringData.length > 0) {
        var decodeResult = result.mDecodeResult;
        var codeType = result.mCodeType;
        var stringData = result.mStringData;
        //alert("DecodResult:" + decodeResult + ",mCodeType:" + codeType + ",StringData:" + stringData);
        DotNet.invokeMethodAsync('ZennohBlazorShared', 'CallScanFunction', decodeResult, codeType, stringData);
    }
}

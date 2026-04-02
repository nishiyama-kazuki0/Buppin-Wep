function scanCallbackFunction(result) {
    if (result.mStringData.length > 0) {
        var decodeResult = result.mDecodeResult.toString();
        var codeType = result.mCodeType.toString();
        var stringData = result.mStringData.toString();
        //alert("DecodResult:" + decodeResult + ",mCodeType:" + codeType + ",StringData:" + stringData);
        DotNet.invokeMethodAsync('ExpressionDBBlazorShared', 'CallScanFunction', decodeResult, codeType, stringData);
    }
}

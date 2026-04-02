namespace SharedModels;

/// <summary>
/// 作業操作端末の実行パラメータのモデル
/// </summary>
public class WoExecSequenceParameter : AbstractViewModelBaseExtension
{
    /// <summary>
    /// 操作端末(1:人作業端末、2:1F入出庫端末、3:5F入出庫端末_常温、4:5F入出庫端末_冷凍)
    /// </summary>
    public int TerminalNo { get; set; } = 0;
    /// <summary>
    /// 対象パレットNO
    /// </summary>
    public string PalletNo { get; set; } = string.Empty;
    /// <summary>
    /// FORM_ST_NO
    /// </summary>
    public string FromSTNO { get; set; } = string.Empty;
    /// <summary>
    /// TO_ST_NO
    /// </summary>
    public string ToSTNO { get; set; } = string.Empty;
    /// <summary>
    /// FROM_AGV_ST_NO
    /// </summary>
    public string FromAGVSTNO { get; set; } = string.Empty;
    /// <summary>
    /// TO_AGV_ST_NO
    /// </summary>
    public string ToAGVSTNO { get; set; } = string.Empty;
    /// <summary>
    /// PD品作業開始バッチNO
    /// </summary>
    public string PDWorkStartBatchNo { get; set; } = string.Empty;
    /// <summary>
    /// 実績メンテ区分
    /// </summary>
    public string ResultMaintenanceClass { get; set; } = string.Empty;
    /// <summary>
    /// 入出庫STモード指示区分
    /// </summary>
    public string RequestSRSTModeClass { get; set; } = string.Empty;
    /// <summary>
    /// システム状態変更依頼区分
    /// </summary>
    public string RequestSystemStatusClass { get; set; } = string.Empty;
    /// <summary>
    /// 在庫区分（旧：廃棄区分）
    /// </summary>
    public string RequestTrashClass { get; set; } = string.Empty;
    /// <summary>
    /// GOT異常エラーコード
    /// </summary>
    public string ErrorCodeGOT { get; set; } = string.Empty;
    /// <summary>
    /// 単独入庫備考
    /// </summary>
    public string SingleStorageNote { get; set; } = string.Empty;


    /// <summary>
    /// プロパティ取得時変換
    /// </summary>
    /// <param name="prop">プロパティ取得値</param>
    /// <returns>変換後値</returns>
    private int GetIntParse(string prop)
    {
        return string.IsNullOrWhiteSpace(prop) ? 0 : int.Parse(prop);
    }
    /// <summary>
    /// プロパティ設定時変換
    /// </summary>
    /// <param name="value">プロパティ設定値</param>
    /// <returns>変換後値</returns>
    private string SetStringParse(int value)
    {
        return value == 0 ? string.Empty : value.ToString();
    }

    /// <summary>
    /// FORM_ST_NO
    /// </summary>
    public int FromSTNONumber
    {
        get => GetIntParse(FromSTNO);
        set => FromSTNO = SetStringParse(value);
    }
    /// <summary>
    /// TO_ST_NO
    /// </summary>
    public int ToSTNONumber
    {
        get => GetIntParse(ToSTNO);
        set => ToSTNO = SetStringParse(value);
    }
    /// <summary>
    /// FROM_AGV_ST_NO
    /// </summary>
    public int FromAGVSTNONumber
    {
        get => GetIntParse(FromAGVSTNO);
        set => FromAGVSTNO = SetStringParse(value);
    }
    /// <summary>
    /// TO_AGV_ST_NO
    /// </summary>
    public int ToAGVSTNONumber
    {
        get => GetIntParse(ToAGVSTNO);
        set => ToAGVSTNO = SetStringParse(value);
    }
    /// <summary>
    /// 実績メンテ区分
    /// </summary>
    public int ResultMaintenanceClassNumber
    {
        get => GetIntParse(ResultMaintenanceClass);
        set => ResultMaintenanceClass = SetStringParse(value);
    }
    /// <summary>
    /// 入出庫STモード指示区分
    /// </summary>
    public int RequestSRSTModeClassNumber
    {
        get => GetIntParse(RequestSRSTModeClass);
        set => RequestSRSTModeClass = SetStringParse(value);
    }
    /// <summary>
    /// システム状態変更依頼区分
    /// </summary>
    public int RequestSystemStatusClassNumber
    {
        get => GetIntParse(RequestSystemStatusClass);
        set => RequestSystemStatusClass = SetStringParse(value);
    }
    /// <summary>
    /// 在庫区分（旧：廃棄区分）
    /// </summary>
    public int RequestTrashClassNumber
    {
        get => GetIntParse(RequestTrashClass);
        //「0:通常在庫」対応の為、ゼロでもそのままセットする
        //set => RequestTrashClass = SetStringParse(value);
        set => RequestTrashClass = value.ToString();
    }
    /// <summary>
    /// GOT異常エラーコード
    /// </summary>
    public int ErrorCodeGOTNumber
    {
        get => GetIntParse(ErrorCodeGOT);
        set => ErrorCodeGOT = SetStringParse(value);
    }
}

using SharedModels;

namespace ExpressionDBCycleProcessApp.Services;

/// <summary>
/// DEFINE_SYSTEM_PARAMETERSテーブル情報
/// </summary>
public sealed class SystemParameterService
{
    #region パラメータプロパティ
    /// <summary>
    /// DBアクセスリトライ間隔[ミリ秒]
    /// </summary>
    public int PLC_DbRetryInterval { get; set; } = 3000;
    /// <summary>
    /// DBアクセスリトライ回数[回]
    /// </summary>
    public int PLC_DbRetryLimitNum { get; set; } = 0;

    /// <summary>
    /// AGV搬送指示待機間隔[ミリ秒]
    /// </summary>
    public int PLC_AgvRequestInterval { get; set; } = 3000;
    /// <summary>
    /// AGV搬送指示待機回数[回]
    /// ※0:固定にする
    /// </summary>
    public int PLC_AgvRequestLimitNum { get; set; } = 0;

    /// <summary>
    /// PLCデバイス値更新間隔[ミリ秒]
    /// </summary>
    public int PLC_DeviceRegisterInterval { get; set; } = 30000;

    /// <summary>
    /// システムパラメータ更新検知間隔[ミリ秒]
    /// </summary>
    public int PLC_UpdateSystemParameterInterval { get; set; } = 30000;

    /// <summary>
    /// 自社PTステーション段済み閾値枚数
    /// </summary>
    public int PLC_ThresholdOwnCompany { get; set; } = 8;
    /// <summary>
    /// 他社PTステーション段済み閾値枚数
    /// </summary>
    public int PLC_ThresholdOtherCompany { get; set; } = 8;

    /// <summary>
    /// 紙ゴミ警告回数_待機中
    /// </summary>
    public int TrashAlarmCountPaper1 { get; set; } = 15;
    /// <summary>
    /// 紙ゴミ警告回数_未出庫
    /// </summary>
    public int TrashAlarmCountPaper2 { get; set; } = 10;
    /// <summary>
    /// プラスチックゴミ警告回数_待機中
    /// </summary>
    public int TrashAlarmCountPlastic1 { get; set; } = 15;
    /// <summary>
    /// プラスチックゴミ警告回数_未出庫
    /// </summary>
    public int TrashAlarmCountPlastic2 { get; set; } = 10;
    /// <summary>
    /// ビニールゴミ警告回数_待機中
    /// </summary>
    public int TrashAlarmCountVinyl1 { get; set; } = 15;
    /// <summary>
    /// ビニールゴミ警告回数_未出庫
    /// </summary>
    public int TrashAlarmCountVinyl2 { get; set; } = 10;
    /// <summary>
    /// 紙ゴミ停止回数
    /// </summary>
    public int TrashStopCountPaper { get; set; } = 20;
    /// <summary>
    /// プラスチックゴミ停止回数
    /// </summary>
    public int TrashStopCountPlastic { get; set; } = 20;
    /// <summary>
    /// ビニールゴミ停止回数
    /// </summary>
    public int TrashStopCountVinyl { get; set; } = 20;
    /// <summary>
    /// 解凍機移載受付可下流コンベアバッファ数<br/>
    /// ※解凍機への移載受付を行う際の下流コンベア上のパレット数（バッファ数）閾値
    /// </summary>
    public int PLC_ThawThresholdCount { get; set; } = 5;

    //※追加のパラメータが必要な場合はこのリージョン内に記述すること。

    #endregion

    /// <summary>
    /// HttpClient
    /// </summary>
    private readonly CommonWebApi _webComService;

    /// <summary>
    /// コンストラクタ
    /// Jsonデシリアライズでエラーとなるため、引数なしのコンストラクタを一旦用意。
    /// </summary>
#pragma warning disable CS8618 //未使用のため抑制
    public SystemParameterService() { }
#pragma warning restore CS8618 //未使用のため抑制

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SystemParameterService(CommonWebApi webComService)
    {
        _webComService = webComService;
    }

    /// <summary>
    /// システムパラメータをDBより読み込む
    /// </summary>
    public async Task LoadSystemParameters()
    {
        ClassNameSelect select = new()
        {
            viewName = "DEFINE_SYSTEM_PARAMETERS"
            ,
            tsqlHints = EnumTSQLhints.NOLOCK
        };
        ResponseValue[]? resItems = await _webComService.GetResponseValue(select);

        Dictionary<string, SystemParameter> sysParam = [];
        if (resItems is null)
        {
            throw new NullReferenceException("GetSystemParameter_resItems is null");
        }

        foreach (ResponseValue item in resItems)
        {
            SystemParameter newRow = new()
            {
                ParameterKey = ConvertUtil.GetValueString(item, "PARAMETER_KEY"),
                KeyName = ConvertUtil.GetValueString(item, "KEY_NAME"),
                ParameterValue = ConvertUtil.GetValueString(item, "PARAMETER_VALUE"),
            };

            sysParam[newRow.ParameterKey] = newRow;
        }

        // DBアクセスリトライ間隔[ミリ秒]
        PLC_DbRetryInterval = GetSystemParameterInt(sysParam, "PLC_DbRetryInterval", PLC_DbRetryInterval);
        // DBアクセスリトライ回数[回]
        PLC_DbRetryLimitNum = GetSystemParameterInt(sysParam, "PLC_DbRetryLimit", PLC_DbRetryLimitNum);

        // AGV搬送指示待機間隔[ミリ秒]
        PLC_AgvRequestInterval = GetSystemParameterInt(sysParam, "PLC_AgvRequestInterval", PLC_AgvRequestInterval);
        // AGV搬送指示待機回数[回] ※0:固定にする
        PLC_AgvRequestLimitNum = GetSystemParameterInt(sysParam, "PLC_AgvRequestLimitNum", PLC_AgvRequestLimitNum);
        // PLCデバイス値更新間隔[ミリ秒]
        PLC_DeviceRegisterInterval = GetSystemParameterInt(sysParam, "PLC_DeviceRegisterInterval", PLC_DeviceRegisterInterval);
        // システムパラメータ更新検知間隔[ミリ秒]
        PLC_UpdateSystemParameterInterval = GetSystemParameterInt(sysParam, "PLC_UpdateSystemParameterInterval", PLC_UpdateSystemParameterInterval);

        // 自社PTステーション段済み閾値枚数
        PLC_ThresholdOwnCompany = GetSystemParameterInt(sysParam, "PLC_ThresholdOwnCompany", PLC_ThresholdOwnCompany);
        // 他社PTステーション段済み閾値枚数
        PLC_ThresholdOtherCompany = GetSystemParameterInt(sysParam, "PLC_ThresholdOtherCompany", PLC_ThresholdOtherCompany);

        // 紙ゴミ警告回数_待機中
        TrashAlarmCountPaper1 = GetSystemParameterInt(sysParam, "TrashAlarmCountPaper1", TrashAlarmCountPaper1);
        // 紙ゴミ警告回数_未出庫
        TrashAlarmCountPaper2 = GetSystemParameterInt(sysParam, "TrashAlarmCountPaper2", TrashAlarmCountPaper2);
        // プラスチックゴミ警告回数_待機中
        TrashAlarmCountPlastic1 = GetSystemParameterInt(sysParam, "TrashAlarmCountPlastic1", TrashAlarmCountPlastic1);
        // プラスチックゴミ警告回数_未出庫
        TrashAlarmCountPlastic2 = GetSystemParameterInt(sysParam, "TrashAlarmCountPlastic2", TrashAlarmCountPlastic2);
        // ビニールゴミ警告回数_待機中
        TrashAlarmCountVinyl1 = GetSystemParameterInt(sysParam, "TrashAlarmCountVinyl1", TrashAlarmCountVinyl1);
        // ビニールゴミ警告回数_未出庫
        TrashAlarmCountVinyl2 = GetSystemParameterInt(sysParam, "TrashAlarmCountVinyl2", TrashAlarmCountVinyl2);
        // 紙ゴミ停止回数
        TrashStopCountPaper = GetSystemParameterInt(sysParam, "TrashStopCountPaper", TrashStopCountPaper);
        // プラスチックゴミ停止回数
        TrashStopCountPlastic = GetSystemParameterInt(sysParam, "TrashStopCountPlastic", TrashStopCountPlastic);
        // ビニールゴミ停止回数
        TrashStopCountVinyl = GetSystemParameterInt(sysParam, "TrashStopCountVinyl", TrashStopCountVinyl);

        // 解凍機移載受付可下流コンベアバッファ数
        PLC_ThawThresholdCount = GetSystemParameterInt(sysParam, "PLC_ThawThresholdCount", PLC_ThawThresholdCount);

        //return this;//自分自身を返す
    }

    private string GetSystemParameter(Dictionary<string, SystemParameter> sysParam, string key, string def)
    {
        string val = def;
        if (sysParam.TryGetValue(key, out SystemParameter? param))
        {
            if (null != param)
            {
                val = param.ParameterValue;
            }
        }
        return val;
    }
    private int GetSystemParameterInt(Dictionary<string, SystemParameter> sysParam, string key, int def)
    {
        int val = def;
        if (sysParam.TryGetValue(key, out SystemParameter? param))
        {
            if (null != param)
            {
                val = ConvertUtil.GetValueInt(param.ParameterValue);
            }
        }
        return val;
    }

    //TODO 念のため、セッションストレージから取得するメソッド、セットするメソッドを作成しておく。
    public async Task<SystemParameterService> GenerateSystemParametersObj()
    {
        //パラメータを詰めたオブジェクトを作成する。
        //TODO DBから再度取得せず、値のみをクローンするような処理にしたい。

        SystemParameterService s = new(_webComService);
        await s.LoadSystemParameters();
        return s;
    }

}

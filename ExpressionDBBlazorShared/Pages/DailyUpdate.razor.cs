using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Shared;
using Sotsera.Blazor.Toaster.Core.Models;
using System.Timers;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// 日次更新画面
/// </summary>
public partial class DailyUpdate : ChildPageBasePC
{
    public const string MONITOR_SYSTEM_STATUS = "MonitorSystemStatus";

    /// <summary>
    /// システム状態
    /// </summary>
    private string SystemStatus { get; set; } = string.Empty;
    /// <summary>
    /// 作業日
    /// </summary>
    private string WorkDate { get; set; } = string.Empty;
    /// <summary>
    /// 自動日次処理
    /// </summary>
    private string AutoProcStatus { get; set; } = string.Empty;
    /// <summary>
    /// 次回自動処理日時
    /// </summary>
    private string NextProcTime { get; set; } = string.Empty;
    /// <summary>
    /// 前回処理日時
    /// </summary>
    private string LastProcTime { get; set; } = string.Empty;

    /// <summary>
    /// システム状態値　0:通常、1:日時更新中
    /// </summary>
    private int _systemStatus = -1;

    /// <summary>
    /// 自動日時更新 true:有効、false:無効
    /// </summary>
    private bool _isAutoDailyUpdate = false;

    /// <summary>
    /// 画面自動更新用タイマー
    /// </summary>
    private System.Timers.Timer? timeMonitorSystemScreen;

    /// <summary>
    /// システム状態監視間隔[ミリ秒]
    /// </summary>
    private int MonitorSystemStatusInterval { get; set; } = 5000;

    #region スタイル関連
    /// <summary>
    /// タイトルのフォントサイズ
    /// </summary>
    private string TitleFontSize { get; set; } = "100%";
    /// <summary>
    /// タイトルのフォント幅
    /// </summary>
    private string TitleFontWeight { get; set; } = "bold";
    /// <summary>
    /// ラベルのフォントサイズ
    /// </summary>
    private string LabelFontSize { get; set; } = "100%";
    /// <summary>
    /// ラベルのフォント幅
    /// </summary>
    private string LabelFontWeight { get; set; } = "bold";
    /// <summary>
    /// テキストのフォントサイズ
    /// </summary>
    private string TextFontSize { get; set; } = "100%";
    /// <summary>
    /// テキストのフォント幅
    /// </summary>
    private string TextFontWeight { get; set; } = "bold";
    /// <summary>
    /// テキストの幅
    /// </summary>
    private string TextWidth { get; set; } = string.Empty;
    /// <summary>
    /// テキストの高さ
    /// </summary>
    private string TextHeight { get; set; } = string.Empty;
    /// <summary>
    /// テキストスタイル
    /// </summary>
    private string TextBoxStyle { get; set; } = string.Empty;
    /// <summary>
    /// ラベル幅
    /// </summary>
    private string LabelWidth { get; set; } = "150px";
    #endregion

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (null != _sysParams)
        {
            TitleFontSize = _sysParams.PC_GroupFieldTitleFontSize;
            TitleFontWeight = _sysParams.PC_GroupFieldTitleFontWeight;
            LabelFontSize = _sysParams.PC_GroupFieldLabelFontSize;
            LabelFontWeight = _sysParams.PC_GroupFieldLabelFontWeight;
            LabelWidth = _sysParams.PC_GroupFieldLabelWidth;
            TextFontSize = _sysParams.PC_TextBoxFontSize;
            TextFontWeight = _sysParams.PC_TextBoxFontWeight;
            TextWidth = _sysParams.PC_TextBoxWidth;
            TextHeight = _sysParams.PC_TextBoxHeight;
            MonitorSystemStatusInterval = _sysParams.MonitorSystemStatusInterval;
        }

        // スタイル設定
        TextBoxStyle += $"font-size:{TextFontSize};";
        TextBoxStyle += $"font-weight:{TextFontWeight};";
        if (!string.IsNullOrEmpty(TextWidth))
        {
            TextBoxStyle += $"width:{TextWidth};";
        }
        if (!string.IsNullOrEmpty(TextHeight))
        {
            TextBoxStyle += $"height:{TextHeight};";
        }

        // システム状態取得
        (_systemStatus, _isAutoDailyUpdate) = await ComService.GetSystemStatusType();

        // システム状態監視タイマー起動
        StopMonitorSystemStatusTimer();
        StartMonitorSystemStatusTimer(MonitorSystemStatusInterval);
    }

    protected override void Dispose()
    {
        // タイマー停止
        StopMonitorSystemStatusTimer();

        base.Dispose();
    }

    /// <summary>
    /// コンポーネント情報初期化
    /// </summary>
    /// <returns></returns>
    protected override async Task InitComponentsAsync()
    {
        await base.InitComponentsAsync();

        // ラベル幅取得
        ComponentsInfo? info
            = _componentsInfo.FirstOrDefault(_ =>
            _.ComponentName == STR_ATTRIBUTE_SEARCH
            && _.AttributesName == "LabelWidth"
            );
        if (null != info)
        {
            LabelWidth = info.Value;
        }
    }

    /// <summary>
    /// 画面更新
    /// </summary>
    /// <returns></returns>
    public override async Task グリッド更新(ComponentProgramInfo info)
    {
        await base.グリッド更新(info);

        SystemStatus = string.Empty;
        WorkDate = string.Empty;
        AutoProcStatus = string.Empty;
        NextProcTime = string.Empty;
        LastProcTime = string.Empty;
        if (null != _gridData && _gridData.Count > 0)
        {
            if (_gridData[0].TryGetValue("システム状態", out object? value))
            {
                SystemStatus = value?.ToString()!;
            }
            if (_gridData[0].TryGetValue("作業日", out value))
            {
                WorkDate = value?.ToString()!;
            }
            if (_gridData[0].TryGetValue("自動日次処理", out value))
            {
                AutoProcStatus = value?.ToString()!;
            }
            if (_gridData[0].TryGetValue("次回自動処理日時", out value))
            {
                NextProcTime = value?.ToString()!;
            }
            if (_gridData[0].TryGetValue("前回処理日時", out value))
            {
                LastProcTime = value?.ToString()!;
            }
            // 編集モードでダイアログを表示するため選択状態にする
            _gridSelectedData = _gridData;
        }
    }

    /// <summary>
    /// システム状態監視タイマー起動
    /// </summary>
    private void StartMonitorSystemStatusTimer(int Intartval)
    {
        timeMonitorSystemScreen = new System.Timers.Timer(Intartval);
        timeMonitorSystemScreen.Elapsed += OnMonitorSystemStatusTimedEvent;
        timeMonitorSystemScreen.AutoReset = true;
        timeMonitorSystemScreen.Enabled = true;
    }

    /// <summary>
    /// システム状態監視タイマー停止
    /// </summary>
    private void StopMonitorSystemStatusTimer()
    {
        if (timeMonitorSystemScreen is not null)
        {
            timeMonitorSystemScreen.Enabled = false;
            timeMonitorSystemScreen.Elapsed -= OnMonitorSystemStatusTimedEvent;
        }
    }

    /// <summary>
    /// システム状態監視タイマー通知
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private async void OnMonitorSystemStatusTimedEvent(object? source, ElapsedEventArgs e)
    {
        try
        {
            // システム状態取得
            int status;
            (status, _isAutoDailyUpdate) = await ComService.GetSystemStatusType();
            if (_systemStatus != status)
            {
                if (_systemStatus == 0 && status == 1)
                {
                    // 自動実行モードの場合、日時更新開始を通知
                    if (_isAutoDailyUpdate)
                    {
                        // メッセージ取得
                        string msg = "日次処理が自動実行されました。";
                        if (Attributes.ContainsKey(MONITOR_SYSTEM_STATUS))
                        {
                            if (Attributes[MONITOR_SYSTEM_STATUS].TryGetValue("StartMessage", out object? value))
                            {
                                msg = value.ToString()!;
                            }
                        }
                        // 日次更新開始を通知
                        ComService!.ShowNotifyMessege(ToastType.Success, $"{pageName}", msg);

                    }

                    // 画面を更新する//西山16
                    await グリッド更新(new ComponentProgramInfo() { ComponentName = STR_ATTRIBUTE_GRID });
                    StateHasChanged();
                }
                else if (_systemStatus == 1 && status == 0)
                {
                    // メッセージ取得
                    string msg = "日次処理を終了しました。";
                    if (Attributes.ContainsKey(MONITOR_SYSTEM_STATUS))
                    {
                        if (Attributes[MONITOR_SYSTEM_STATUS].TryGetValue("StopMessage", out object? value))
                        {
                            msg = value.ToString()!;
                        }
                    }
                    // 日時更新終了を通知
                    ComService!.ShowNotifyMessege(ToastType.Success, $"{pageName}", msg);


                    // 画面を更新する
                    await グリッド更新(new ComponentProgramInfo() { ComponentName = STR_ATTRIBUTE_GRID });
                    StateHasChanged();
                }
                // 現在のシステム状態を更新
                _systemStatus = status;
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }
}
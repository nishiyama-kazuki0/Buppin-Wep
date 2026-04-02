using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Shared;
using Sotsera.Blazor.Toaster.Core.Models;
using static SharedModels.SharedConst;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// 業務開始終了画面
/// </summary>
public partial class WorkStatOrEndMenu : ChildPageBasePC
{
    #region override
    /// <summary>
    /// ストアドデータ設定_引数データ作成
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
    {
        await Task.Delay(0);
        try
        {
            switch (info.ComponentName)
            {
                case "F7StoredData":
                    SetStoredParam_コマンド実行();
                    break;
                case "F9StoredData":
                    SetStoredParam_解凍機発振開始();
                    break;
                case "F10StoredData":
                    SetStoredParam_解凍機発振終了();
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    #endregion
    #region private

    /// <summary>
    /// 業務終了時のストアドパラメータセット
    /// </summary>
    private void SetStoredParam_コマンド実行()
    {
        _storedData = [];
        _storedData["コマンド実行"] = 1;
    }
    /// <summary>
    /// 解凍機発振開始時のストアドパラメータセット
    /// </summary>
    private void SetStoredParam_解凍機発振開始()
    {
        _storedData = [];
        _storedData["MODE"] = 0;
        _storedData["CONFIRM"] = 0;
    }
    /// <summary>
    /// 解凍機発振終了時のストアドパラメータセット
    /// </summary>
    private void SetStoredParam_解凍機発振終了()
    {
        _storedData = [];
        _storedData["MODE"] = 1;
        _storedData["CONFIRM"] = 0;
    }
    /// <summary>
    /// ストアド呼び出し
    /// </summary>
    /// <returns></returns>
    public override async Task<bool> ストアド呼び出し(ComponentProgramInfo info)
    {
        try
        {
            Dictionary<string, object> dlgParam = new(GetAttributes(info.ComponentName));

            // 結果変数
            string strSummary = pageName.Replace("\\n", "");

            // ダイアログタイトルを取得
            string strProgramName = string.Empty;
            string strResultMessage = string.Empty;
            if (!string.IsNullOrEmpty(info.ProcessProgramName))
            {
                strProgramName = info.ProcessProgramName;
            }
            else
            {
                if (dlgParam.TryGetValue("ProgramName", out object? prg))
                {
                    strProgramName = prg.ToString()!;
                }
            }
            if (dlgParam.TryGetValue("ResultMessage", out object? obj))
            {
                strResultMessage = obj.ToString()!;
            }
            var lstResult = await ストアド呼び出し2(info, strSummary, strProgramName);
            if (lstResult == null) return false;

            var retb = await ストアド呼び出し後処理(lstResult, strSummary);
            if (retb)
            {
                _ = await 解凍機発振開始終了ストアド呼び出し(info);
            }
            // 正常終了で結果メッセージがある場合は通知
            if (retb)
            {
                if (!string.IsNullOrEmpty(strResultMessage))
                {
                    ComService!.ShowNotifyMessege(ToastType.Success, $"{strSummary}", strResultMessage);
                }
            }

            return retb;
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
            return false;
        }
    }
    private async Task<bool> 解凍機発振開始終了ストアド呼び出し(ComponentProgramInfo info)
    {
        _storedData["CONFIRM"] = 1;
        Dictionary<string, object> dlgParam = new(GetAttributes(info.ComponentName));

        // 結果変数
        string strSummary = pageName.Replace("\\n", "");

        // ダイアログタイトルを取得
        string strResultMessage = string.Empty;
        if (dlgParam.TryGetValue("ResultMessage", out object? obj))
        {
            strResultMessage = obj.ToString()!;
        }
        var lstResult = await ストアド呼び出し2(info, strSummary, "解凍機発振開始終了");
        if (lstResult == null) return false;
        var retb = await ストアド呼び出し後処理(lstResult, strSummary);
        // 正常終了で結果メッセージがある場合は通知
        if (retb)
        {
            if (!string.IsNullOrEmpty(strResultMessage))
            {
                ComService!.ShowNotifyMessege(ToastType.Success, $"{strSummary}", strResultMessage);
            }
        }

        return retb;
    }
    #endregion
}
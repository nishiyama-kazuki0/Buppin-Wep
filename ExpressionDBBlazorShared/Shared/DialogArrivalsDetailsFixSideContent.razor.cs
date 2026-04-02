using SharedModels;
using Sotsera.Blazor.Toaster.Core.Models;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 入荷予実数修正ダイアログ
/// </summary>
public partial class DialogArrivalsDetailsFixSideContent : DialogCommonInputContent
{
    /// <summary>
    /// F1ボタンクリックイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    protected override async Task OnClickResultF1(object? sender)
    {
        // チェック
        if (false == バリデートチェック())
        {
            return;
        }

        bool retb = false;
        try
        {
            // 入力エリアロック
            ComService.SetCompItemListDisabled(_inputItems, true);

            // 結果変数
            List<ExecResult> lstResult = [];
            int notifyDuration = _sysParams is null ? SharedConst.DEFAULT_NOTIFY_DURATION : _sysParams.NotifyPopupDuration;
            string strSummary = DialogTitle.Replace("\\n", "");
            string strResultMessage = "登録しました。";

            // 確認
            bool? retConfirm = await ComService.DialogShowYesNo("設定を確定しますか？", strSummary);
            retb = retConfirm is not null && (bool)retConfirm;
            if (!retb)
            {
                return;
            }

            // 登録
            retb = false;
            if (!string.IsNullOrEmpty(ProgramName))
            {
                // RequestValueにデータを作成する
                RequestValue rv = RequestValue.CreateRequestProgram(ProgramName);

                // InitialDataから取得
                if (InitialData.TryGetValue("入荷No", out object? value))
                {
                    _ = rv.SetArgumentValue("入荷No", value, "");
                }
                if (InitialData.TryGetValue("明細No", out value))
                {
                    _ = rv.SetArgumentValue("明細No", value, "");
                }

                // 入力エリアから取得
                Dictionary<string, object> inputData = ComService.GetCompInputValues(_inputItems, true);
                if (inputData.TryGetValue("検品実績数(ケース)", out value))
                {
                    _ = rv.SetArgumentValue("入荷ケース実績数", value, "");
                }
                if (inputData.TryGetValue("検品実績数(バラ)", out value))
                {
                    _ = rv.SetArgumentValue("入荷バラ実績数", value, "");
                }

                // WebAPIへアクセス
                retb = true;
                ExecResult[]? results = await WebComService.SetRequestValue(GetType().Name, rv);
                if (results == null)
                {
                    // 実行結果がnullの場合は異常
                    ComService!.ShowNotifyMessege(ToastType.Error, $"{strSummary}", "WebAPIへのアクセスが異常終了しました。ログを確認して下さい。");
                    return;
                }
                else
                {
                    // 実行結果が返った場合
                    lstResult = new List<ExecResult>(results);
                }
            }

            // 実行結果を異常・正常・確認に分ける
            List<ExecResult> lstError = lstResult.Where(_ => _.RetCode < 0).OrderBy(_ => _.ExecOrderRank).ToList();
            List<ExecResult> lstSuccess = lstResult.Where(_ => _.RetCode == 0).OrderBy(_ => _.ExecOrderRank).ToList();
            List<ExecResult> lstConfirm = lstResult.Where(_ => _.RetCode > 0).OrderBy(_ => _.ExecOrderRank).ToList();

            if (lstError.Count() > 0)
            {
                // 異常結果がある場合
                retb = false;

                // 異常メッセージを全て通知
                foreach (ExecResult result in lstError)
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, $"{strSummary}", result.Message);

                }
            }
            else
            {
                // 異常結果が無い場合
                retb = true;

                // 正常結果のメッセージがある場合、全て通知
                foreach (ExecResult result in lstSuccess)
                {
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        ComService!.ShowNotifyMessege(ToastType.Error, $"{strSummary}", result.Message);

                    }
                }

                // 確認結果がある場合、全ての確認ダイアログ表示
                foreach (ExecResult result in lstConfirm)
                {
                    bool? ret = await ComService.DialogShowYesNo(result.Message);
                    retb = ret is not null && (bool)ret;
                    if (!retb)
                    {
                        break;
                    }
                }
            }

            // 正常終了で結果メッセージがある場合は通知
            if (retb)
            {
                if (!string.IsNullOrEmpty(strResultMessage))
                {
                    ComService!.ShowNotifyMessege(ToastType.Success, $"{strSummary}", strResultMessage);

                }
            }
        }
        finally
        {
            // 入力エリアロック解除
            ComService.SetCompItemListDisabled(_inputItems, false);
        }

        // 正常終了ならダイアログ閉じる
        if (retb)
        {
            DialogService.CloseSide(retb);
        }
    }
}
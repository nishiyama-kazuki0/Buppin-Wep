using SharedModels;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 店別仕分実績メンテナンス
    /// </summary>
    public partial class DialogSortingByStoreResultFixContent : DialogCommonInputContent
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
                List<ExecResult> lstResult = new();
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
                    if (InitialData.TryGetValue("店別仕分指示ID", out object? value))
                    {
                        _ = rv.SetArgumentValue("店別仕分指示ID", value, "");
                    }

                    // 入力エリアから取得
                    Dictionary<string, object> inputData = ComService.GetCompInputValues(_inputItems, true);
                    if (inputData.TryGetValue("修正後仕分実績数(ケース)", out value))
                    {
                        _ = rv.SetArgumentValue("修正後ケース仕分実績数", value, "");
                    }
                    if (inputData.TryGetValue("修正後仕分実績数(バラ)", out value))
                    {
                        _ = rv.SetArgumentValue("修正後バラ仕分実績数", value, "");
                    }
                    if (inputData.TryGetValue("備考", out value))
                    {
                        _ = rv.SetArgumentValue("備考", value, "");
                    }

                    // WebAPIへアクセス
                    retb = true;
                    ExecResult[]? results = await ComService.SetRequestValue(GetType().Name, rv);
                    if (results == null)
                    {
                        // 実行結果がnullの場合は異常
                        NotificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = $"{strSummary}",
                            Detail = "WebAPIへのアクセスが異常終了しました。ログを確認して下さい。",
                            Duration = notifyDuration
                        });
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
                        NotificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = $"{strSummary}",
                            Detail = result.Message,
                            Duration = notifyDuration
                        });
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
                            NotificationService.Notify(new NotificationMessage()
                            {
                                Severity = NotificationSeverity.Success,
                                Summary = $"{strSummary}",
                                Detail = result.Message,
                                Duration = notifyDuration
                            });
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
                        NotificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = $"{strSummary}",
                            Detail = strResultMessage,
                            Duration = notifyDuration
                        });
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
}
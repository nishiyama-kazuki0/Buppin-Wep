using Microsoft.AspNetCore.Components;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 予定外入荷確定ダイアログ
    /// </summary>
    public partial class DialogArrivalsMaintenanceContent : ChildPageBasePC
    {
        private const string PROPKEY_ARRIVAL_SOURCE = "入荷元区分";
        private const string PROPKEY_REMARKS = "備考";

        private CompItemInfo? compArrivalSource;
        private CompItemInfo? compRemarks;
        private ButtonFuncRadzen? ButtonFuncRadzen;

        /// <summary>
        /// ヘッダデータ
        /// </summary>
        [Parameter]
        public Dictionary<string, object> HeaderData { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// 明細データ
        /// </summary>
        [Parameter]
        public List<IDictionary<string, object>> DetailData { get; set; } = new List<IDictionary<string, object>>();

        #region Override

        /// <summary>
        /// 検索条件初期化
        /// </summary>
        protected override async Task InitSearchConditionAsync()
        {
            // クリア
            _searchCompItems.Clear();

            // 入荷元区分 初期値取得
            string? strArrivalSource = string.Empty;
            {
                if (HeaderData.TryGetValue(PROPKEY_ARRIVAL_SOURCE, out object? obj))
                {
                    strArrivalSource = obj?.ToString();
                }
            }

            // 備考 初期値取得
            string? strRemarks = string.Empty;
            {
                if (HeaderData.TryGetValue(PROPKEY_REMARKS, out object? obj))
                {
                    strRemarks = obj?.ToString();
                }
            }


            // 入荷元区分
            List<ValueTextInfo>? dataArrivalSource = await ComService!.GetValueTextInfo("VW_DROPDOWN_入荷元区分");
            compArrivalSource = new CompItemInfo
            {
                CompType = typeof(CompDropDown),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_ARRIVAL_SOURCE },
                    { "Required", true },
                    { "InitialValue", strArrivalSource },
                    { "Data", dataArrivalSource }
                },
                TitleLabel = PROPKEY_ARRIVAL_SOURCE,
                DispTitleLabel = true,
                DispRequiredSuffix = true,
            };

            // 備考
            compRemarks = new CompItemInfo
            {
                CompType = typeof(CompTextArea),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_REMARKS },
                    { "Required", false },
                    { "InitialValue", strRemarks },
                    { "MaxLength", (long)256 },
                    { "Cols", 60 },
                    { "Rows", 3 },
                    //{ "RegexPattern" , SharedConst.STR_VARIDATE_FULL_WIDTH_CHAR_ONLY} //TODO :半角交じりでも上位連携はエラーになっていないようなので、一旦コメント化。必要な場合はコメントを戻す。
                },
                TitleLabel = PROPKEY_REMARKS,
                DispTitleLabel = true,
                DispRequiredSuffix = false,
            };

            // コンポーネント情報追加
            _searchCompItems.Add(new List<CompItemInfo> { compArrivalSource });
            _searchCompItems.Add(new List<CompItemInfo> { compRemarks });
        }

        /// <summary>
        /// 確定前処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task 確定前処理(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            try
            {
                // 入力チェック
                if (!CheckInput())
                {
                    return;
                }

                // 入力値取得
                if (compArrivalSource?.CompObj?.Instance is CompDropDown compDropDown)
                {
                    if (!string.IsNullOrEmpty(compDropDown.InputValue))
                    {
                        HeaderData[PROPKEY_ARRIVAL_SOURCE] = compDropDown.InputValue;
                    }
                }
                if (compRemarks?.CompObj?.Instance is CompTextArea compTextArea)
                {
                    if (!string.IsNullOrEmpty(compTextArea.InputValue))
                    {
                        HeaderData[PROPKEY_REMARKS] = compTextArea.InputValue;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// ストアドデータ設定_引数データ作成
        /// </summary>
        /// <returns></returns>
        public override async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            try
            {
                _storedData = new Dictionary<string, object>();

                // ヘッダデータセット
                if (HeaderData is not null)
                {
                    foreach (KeyValuePair<string, object> data in HeaderData)
                    {
                        _storedData[data.Key] = data.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// ストアドデータ設定_テーブルデータ作成
        /// </summary>
        /// <returns></returns>
        public override async Task ストアドデータ設定_テーブルデータ作成(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            try
            {
                _storedTableData = new List<Dictionary<string, object>>();

                // 明細データセット
                if (DetailData is not null)
                {
                    foreach (IDictionary<string, object> rows in DetailData)
                    {
                        Dictionary<string, object> rowdata = new();
                        foreach (KeyValuePair<string, object> data in rows)
                        {
                            rowdata[data.Key] = data.Value;
                        }
                        _storedTableData.Add(rowdata);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 確定後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task 確定後処理(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            // ビジーダイアログを先にCloseさせる
            DialogService.Close();
            ButtonFuncRadzen.SetIsBusyDialogClose(false);

            DialogService.Close(0);
        }

        #endregion

        #region private

        /// <summary>
        /// F1ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private async Task OnClickResultF1(object? sender)
        {
            await ExecProgram();
        }

        /// <summary>
        /// F4ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        private async Task OnClickResultF4(object? sender)
        {
            await ExecProgram();
        }

        /// <summary>
        /// 入力チェック
        /// </summary>
        /// <returns></returns>
        private bool CheckInput()
        {
            if (editForm is null)
            {
                return true;
            }

            //EditContextのValidate()メソッドを実行することでSubmitと同等のイベントが発火
            return editForm!.EditContext.Validate();
        }

        #endregion
    }
}

using Microsoft.AspNetCore.Components;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 入荷受付No選択
    /// </summary>
    public partial class DialogArrivalsReceptionNoSelect : ChildPageBasePC
    {
        private ButtonFuncRadzen ButtonFuncRadzen;

        [Parameter]
        public List<string> LstReceptionNo { get; set; } = new List<string>();

        #region override

        /// <summary>
        /// グリッド初期化
        /// </summary>
        /// <returns></returns>
        protected override async Task InitDataGridAsync()
        {
            try
            {
                await Task.Delay(0);

                ComponentColumnsInfo col = new()
                {
                    Property = "受付No"
                };
                _gridColumns.Add(col);
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            try
            {
                // レンダリング抑制解除
                ChildBaseService.BasePageInitilizing = false;

                // Blazor へ状態変化を通知
                StateHasChanged();

                // 受付Noをグリッドへ追加
                _ = Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>();
                for (int i = 0; i < LstReceptionNo.Count(); i++)
                {
                    Dictionary<string, object> newRow = new()
                    {
                        { "受付No", LstReceptionNo[i] },
                    };
                    _gridData.Add(newRow);
                }
                _ = Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// F5クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private async Task OnClickResultF5(object? sender)
        {
            // チェック
            if (_gridSelectedData == null || _gridSelectedData.Count() == 0)
            {
                await ComService.DialogShowOK($"受付Noが選択されていません。", pageName);
                return;
            }

            // グリッドの選択行取得
            string strReceptionNo = _gridSelectedData[0]["受付No"].ToString();

            // ビジーダイアログを先にCloseさせる
            DialogService.Close();
            ButtonFuncRadzen.SetIsBusyDialogClose(false);

            DialogService?.Close(strReceptionNo);
        }

        /// <summary>
        /// F12クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        private async Task OnClickResultF12(object? sender)
        {
            await base.ExecProgram();
        }

        #endregion
    }
}
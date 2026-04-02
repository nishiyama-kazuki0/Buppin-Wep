using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    //在庫ベースでのデータで検索するしたがって、在庫数が0のものは修正しない？

    /// <summary>
    /// 出荷引当一覧
    /// </summary>
    public partial class ShipmentsAllocateResult : ChildPageBasePC
    {
        #region private

        /// <summary>
        /// OnCellRenderのCallBack
        /// </summary>
        /// <param name="args"></param>
        private void CellRender(DataGridCellRenderEventArgs<IDictionary<string, object>> args)
        {
            try
            {
                if ("出荷状態名" == args.Column.Title)
                {
                    // 出荷状態の背景色変更
                    if (args.Data.TryGetValue("出荷状態", out object? value))
                    {
                        ComService.AddAttrShipmentStatus(value?.ToString(), args.Attributes);
                    }
                }
                else if ("確定状態名" == args.Column.Title)
                {
                    // 確定状態の背景色変更
                    if (args.Data.TryGetValue("確定状態", out object? value))
                    {
                        ComService.AddAttrConfirmStatus(value?.ToString(), args.Attributes);
                    }
                }
                else if ("実績送信状態名" == args.Column.Title)
                {
                    // 実績送信状態の背景色変更
                    if (args.Data.TryGetValue("実績送信状態", out object? value))
                    {
                        ComService.AddAttrSendResultsStatus(value?.ToString(), args.Attributes);
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        #endregion
    }
}
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 出荷予実明細
    /// </summary>
    public partial class ShipmentsDetails : ChildPageBasePC
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
                if ("入荷状態名" == args.Column.Title)
                {
                    // 入荷状態の背景色変更
                    if (args.Data.TryGetValue("入荷状態", out object? value))
                    {
                        ComService.AddAttrArrivalStatus(value?.ToString(), args.Attributes);
                    }
                }
                else if ("出荷状態名" == args.Column.Title)
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
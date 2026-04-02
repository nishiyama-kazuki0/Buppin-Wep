using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    public partial class StocksTheoryTotalling : ChildPageBasePC
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
                if ("一致状態" == args.Column.Title)
                {
                    // 一致状態の背景色変更
                    if (args.Data.TryGetValue("一致状態", out object? value))
                    {
                        ComService.AddAttrDifferenceStatus(value?.ToString(), args.Attributes);
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

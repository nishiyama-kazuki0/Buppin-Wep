using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 入荷作業実績管理 作業者別
    /// </summary>
    public partial class TabItemProductivityArrivalsPersons : TabItemBase
    {
        #region override

        /// <summary>
        /// グリッドに表示するデータを取得しグリッドにセットする
        /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
        /// </summary>
        /// <returns></returns>
        public override async Task RefreshGridData(Dictionary<string, WhereParam> whereParam, string strViewName = "", string attributeName = STR_ATTRIBUTE_GRID, bool bInitSelect = false)
        {
            try
            {
                // グリッドクリア
                _ = Attributes[attributeName]["Data"] = _gridData = new List<IDictionary<string, object>>();

                // TableFunction名取得
                string strFunc = await ComService!.GetViewNameAsync(ClassName);

                // TableFunctionの引数作成
                string strArg = "@BASE_ID, @BASE_TYPE, @CONSIGNOR_ID";
                strArg += whereParam.ContainsKey("期間From") ? ", @期間From" : ", (null)";
                strArg += whereParam.ContainsKey("期間To") ? ", @期間To" : ", (null)";
                strArg += whereParam.ContainsKey("作業者コード") ? ", @作業者コード" : ", (null)";

                // TableFunctionからデータ取得
                ClassNameSelect select = new()
                {
                    viewName = $"dbo.{strFunc}({strArg})",
                    columnsDefineName = strFunc,
                    whereParam = whereParam,
                    tableFuncFlg = true,
                };
                _gridData = await ComService!.GetSelectGridData(_gridColumns, select);
                _ = Attributes[attributeName]["Data"] = _gridData;

                if (bInitSelect)
                {
                    // データが存在すれば一件目を選択状態にする
                    _gridSelectedData = _gridData.Count > 0
                        ? new List<IDictionary<string, object>>
                        {
                    _gridData[0]
                        }
                        : (IList<IDictionary<string, object>>?)null;
                }
                else
                {
                    // 選択データクリア
                    _gridSelectedData = null;
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        #endregion
    }
}
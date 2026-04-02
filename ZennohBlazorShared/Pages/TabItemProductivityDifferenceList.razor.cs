using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 作業実績管理一覧 差異一覧
    /// </summary>
    public partial class TabItemProductivityDifferenceList : TabItemBase
    {
        #region override

        /// <summary>
        /// F5ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override async Task OnClickResultF5(object? sender, object? e)
        {
            try
            {
                // 選択行チェック
                if (_gridSelectedData == null || _gridSelectedData?.Count() == 0)
                {
                    await ComService.DialogShowOK($"備考を編集する対象が選択されていません。", pageName);
                    return;
                }

                // ダイアログパラメータ生成
                Dictionary<string, object> dlgParam = new()
                {
                    ["DetailData"] = new List<IDictionary<string, object>>(_gridSelectedData)
                };

                // ダイアログ情報を取得
                Dictionary<string, object> attr = new(GetAttributes("AttributesConfirmDialog"));
                string strDialogTitle = "作業実績備考編集";
                int intDialogWidth = 700;
                int intDialogHeight = 400;
                if (attr.TryGetValue("DialogTitle", out object? obj))
                {
                    strDialogTitle = obj.ToString()!;
                }
                if (attr.TryGetValue("DialogWidth", out obj))
                {
                    _ = int.TryParse(obj.ToString(), out intDialogWidth);
                }
                if (attr.TryGetValue("DialogHeight", out obj))
                {
                    _ = int.TryParse(obj.ToString(), out intDialogHeight);
                }

                // ダイアログ表示
                dynamic window = _js!.GetWindow();
                int innerWidth = (int)window.innerWidth;
                int innerHeight = (int)window.innerHeight;
                dynamic ret = await DialogService.OpenAsync<DialogProductivityDifferenceContent>(
                    $"{strDialogTitle}",
                    dlgParam,
                    new DialogOptions()
                    {
                        Width = $"{Math.Min(intDialogWidth, innerWidth)}px",
                        Height = $"{Math.Min(intDialogHeight, innerHeight)}px",
                        Resizable = true,
                        Draggable = true
                    }
                );

                if (ret is null)
                {
                    // キャンセル
                    return;
                }

                await グリッド更新(new Data.ComponentProgramInfo() { ComponentName = STR_ATTRIBUTE_GRID });
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

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
                strArg += whereParam.ContainsKey("日時From") ? ", @日時From" : ", (null)";
                strArg += whereParam.ContainsKey("日時To") ? ", @日時To" : ", (null)";
                strArg += whereParam.ContainsKey("作業区分") ? ", @作業区分" : ", (null)";
                strArg += whereParam.ContainsKey("入荷No") ? ", @入荷No" : ", (null)";
                strArg += whereParam.ContainsKey("明細No") ? ", @明細No" : ", (null)";

                // TableFunctionのWhere句指定
                if (whereParam.TryGetValue("品名コード", out WhereParam? value))
                {
                    value.tableFuncWithWhere = true;
                }
                if (whereParam.TryGetValue("等級コード", out value))
                {
                    value.tableFuncWithWhere = true;
                }
                if (whereParam.TryGetValue("階級コード", out value))
                {
                    value.tableFuncWithWhere = true;
                }
                if (whereParam.TryGetValue("産地コード", out value))
                {
                    value.tableFuncWithWhere = true;
                }

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
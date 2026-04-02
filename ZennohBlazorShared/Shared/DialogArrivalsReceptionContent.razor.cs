using Microsoft.AspNetCore.Components;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 入荷受付設定ダイアログ
    /// </summary>
    public partial class DialogArrivalsReceptionContent : ChildPageBasePC
    {
        private const string PROPKEY_ARRIVAL_NO = "入荷No";
        private const string PROPKEY_DETAIL_NO = "明細No";

        private ButtonFuncRadzen? ButtonFuncRadzen;

        /// <summary>
        /// 入荷受付設定選択データ
        /// </summary>
        [Parameter]
        public List<IDictionary<string, object>> SelectedArrivalsData { get; set; } = new List<IDictionary<string, object>>();

        #region override

        /// <summary>
        /// グリッドに表示するデータを取得しグリッドにセットする
        /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
        /// 
        /// 初期選択データ指定用
        /// </summary>
        /// <returns></returns>
        public override async Task RefreshGridDataInitSel(Dictionary<string, WhereParam> whereParam, string strViewName = "", string attributeName = STR_ATTRIBUTE_GRID, bool bInitSelect = false, string strInitSelectKey = "", string strInitSelectVal = "")
        {
            try
            {
                // グリッドクリア
                _ = Attributes[attributeName]["Data"] = _gridData = new List<IDictionary<string, object>>();

                // ViewNameが指定されている場合はView名で値を取得
                ClassNameSelect select = new()
                {
                    className = string.IsNullOrEmpty(strViewName) ? GetType().Name : string.Empty,
                    viewName = string.IsNullOrEmpty(strViewName) ? string.Empty : strViewName,
                    whereParam = whereParam,
                    orderByParam = OrderByParamGet()
                };

                List<IDictionary<string, object>> searchData = await ComService!.GetSelectGridData(_gridColumns, select);

                // 入荷受付設定に既に追加されている行データは検索結果から排除する
                foreach (IDictionary<string, object> gridItem in searchData)
                {
                    bool bnExists = false;
                    foreach (IDictionary<string, object> selItem in SelectedArrivalsData)
                    {
                        if (gridItem[PROPKEY_ARRIVAL_NO].ToString() == selItem[PROPKEY_ARRIVAL_NO].ToString() &&
                            gridItem[PROPKEY_DETAIL_NO].ToString() == selItem[PROPKEY_DETAIL_NO].ToString())
                        {
                            bnExists = true;
                            break;
                        }
                    }
                    if (!bnExists)
                    {
                        _gridData.Add(gridItem);
                    }
                }

                _ = Attributes[attributeName]["Data"] = _gridData;

                if (bInitSelect)
                {
                    if (string.IsNullOrEmpty(strInitSelectKey))
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
                        IEnumerable<IDictionary<string, object>> data = _gridData.Where(dict => dict.ContainsKey(strInitSelectKey) && dict[strInitSelectKey].ToString() == strInitSelectVal);
                        if (null != data && data.Any())
                        {
                            _gridSelectedData = new List<IDictionary<string, object>>
                            {
                                data.First()
                            };
                        }
                        else
                        {
                            // 指定したキー、値のデータが存在しない場合は１件目を設定する
                            _gridSelectedData = _gridData.Count > 0
                                ? new List<IDictionary<string, object>>
                                {
                            _gridData[0]
                                }
                                : (IList<IDictionary<string, object>>?)null;
                        }
                    }
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

        #region event

        /// <summary>
        /// F2クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        private async Task OnClickResultF2(object? sender)
        {
            await base.ExecProgram();
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
                await ComService.DialogShowOK($"入荷受付設定を登録する対象が選択されていません。", pageName);
                return;
            }

            // 確認
            bool? ret = await ComService.DialogShowYesNo("登録しますか？", pageName);
            if (true != ret)
            {
                return;
            }

            // グリッドの選択行を返却
            List<IDictionary<string, object>> retVal = new(_gridSelectedData);

            // ビジーダイアログを先にCloseさせる
            DialogService.Close();
            ButtonFuncRadzen.SetIsBusyDialogClose(false);

            DialogService.Close(retVal);
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
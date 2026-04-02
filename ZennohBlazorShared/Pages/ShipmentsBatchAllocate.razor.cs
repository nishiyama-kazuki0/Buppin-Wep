using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 出荷作業バッチ引当
    /// </summary>
    public partial class ShipmentsBatchAllocate : ChildPageBasePC
    {
        #region override

        /// <summary>
        /// 画面遷移用のパラメータをセットする
        /// </summary>
        /// <param name="info"></param>
        public override async Task SetNavigateParam(ComponentProgramInfo info)
        {
            if (Attributes.ContainsKey(info.ComponentName))
            {
                // StorageのKeyを取得する
                string storageKey = string.Empty;
                foreach (KeyValuePair<string, object> keyval in Attributes[info.ComponentName])
                {
                    storageKey = keyval.Value.ToString()!;
                    break;
                }

                // StorageのKeyが有れば
                if (!string.IsNullOrEmpty(storageKey))
                {
                    // PC画面遷移用パラメータクリア
                    _ = ComService.ClearPCTransParam(storageKey);

                    // グリッドに選択行があれば、PC画面間パラメータセット
                    if (_gridSelectedData != null && _gridSelectedData?.Count() > 0)
                    {
                        if (storageKey == "ToShipmentsSeparateAllocate")
                        {
                            // 出荷個別引当画面に遷移する場合、倉庫配送先コード(複数)と納品日(FromTo)のみを渡す
                            Dictionary<string, object> param = new();
                            List<string> lstDlvryCd = new();
                            string strDlvryDateFrom = string.Empty;
                            string strDlvryDateTo = string.Empty;

                            foreach (IDictionary<string, object> rows in _gridSelectedData)
                            {
                                if (rows.TryGetValue("倉庫配送先コード", out object? obj))
                                {
                                    if (obj != null && !string.IsNullOrEmpty((string)obj) && !lstDlvryCd.Contains((string)obj))
                                    {
                                        lstDlvryCd.Add((string)obj);
                                    }
                                }

                                if (rows.TryGetValue("納品日", out obj))
                                {
                                    if (obj != null && !string.IsNullOrEmpty((string)obj))
                                    {
                                        string strTmp = (string)obj;
                                        if (string.IsNullOrEmpty(strDlvryDateFrom) || string.Compare(strDlvryDateFrom, strTmp) > 0)
                                        {
                                            strDlvryDateFrom = strTmp;
                                        }
                                        if (string.IsNullOrEmpty(strDlvryDateTo) || string.Compare(strDlvryDateTo, strTmp) < 0)
                                        {
                                            strDlvryDateTo = strTmp;
                                        }
                                    }
                                }
                            }
                            param["倉庫配送先コード"] = lstDlvryCd;
                            param["納品日"] = new List<string>() { strDlvryDateFrom, strDlvryDateTo };

                            _ = ComService.SetPCTransParam(storageKey, param);
                        }
                        else
                        {
                            _ = ComService.SetPCTransParam(storageKey, _gridSelectedData[0]);
                        }
                    }
                }
            }
            await Task.Delay(0);
        }

        #endregion

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
                else if ("作業開始時間" == args.Column.Title)
                {
                    // 作業開始時間の背景色変更
                    if (args.Data.TryGetValue("作業開始_残分", out object? value))
                    {
                        ComService.AddAttrWarnExcessTime(value?.ToString(), _sysParams.ShipmentsStartWarnTime, args.Attributes);
                    }
                }
                else if ("出荷締切時間" == args.Column.Title)
                {
                    // 出荷締切時間の背景色変更
                    if (args.Data.TryGetValue("出荷締切_残分", out object? value))
                    {
                        ComService.AddAttrWarnExcessTime(value?.ToString(), _sysParams.ShipmentsDeadlineWarnTime, args.Attributes);
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
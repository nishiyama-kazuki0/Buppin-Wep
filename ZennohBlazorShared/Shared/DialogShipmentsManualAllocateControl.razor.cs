using Microsoft.AspNetCore.Components;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 在庫選択引当設定
    /// </summary>
    public partial class DialogShipmentsManualAllocateControl : ChildPageBasePC
    {
        private const string PROPKEY_出荷予定集約ID = "出荷予定集約ID";
        private const string PROPKEY_入荷No = "入荷No";
        private const string PROPKEY_明細No = "明細No";
        private const string PROPKEY_品名 = "品名";
        private const string PROPKEY_倉庫配送先名 = "倉庫配送先名";

        private const string PROPKEY_出庫ケース数 = "出庫ケース数";
        private const string PROPKEY_出庫バラ数 = "出庫バラ数";
        private const string PROPKEY_元出庫ケース数 = "元出庫ケース数";
        private const string PROPKEY_元出庫バラ数 = "元出庫バラ数";

        private const string PROPKEY_在庫減算有無 = "在庫減算有無";

        /// <summary>
        /// 初期データ
        /// </summary>
        [Parameter]
        public Dictionary<string, object> InitialData { get; set; } = new Dictionary<string, object>();

        private string m_出荷予定集約ID = string.Empty;

        /// <summary>
        /// 在庫減算有無DropDownコンポーネント
        /// </summary>
        private CompItemInfo? _cmbIsSubtraction = null;

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

                //Blazor へ状態変化を通知
                StateHasChanged();

                // 初期値設定
                SetInitialData();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

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
            await base.ExecProgram();
        }

        /// <summary>
        /// F12クリックイベント
        /// </summary>
        /// <param name="sender"></param>
        private async Task OnClickResultF12(object? sender)
        {
            await base.ExecProgram();
        }

        /// <summary>
        /// グリッド更新
        /// </summary>
        public override async Task グリッド更新(ComponentProgramInfo info)
        {
            try
            {
                ClassNameSelect custom = new();

                // 出荷管理IDを検索条件に追加
                custom.whereParam.Add(PROPKEY_出荷予定集約ID, new WhereParam { val = m_出荷予定集約ID, whereType = enumWhereType.Equal });

                // 検索条件取得
                Dictionary<string, (object, WhereParam)> items = ComService.GetCompItemInfoValues(_searchCompItems);
                foreach (KeyValuePair<string, (object, WhereParam)> item in items)
                {
                    // 入荷No、明細No、品名、倉庫配送先は検索条件に含めない
                    if (item.Key is not PROPKEY_入荷No and not PROPKEY_明細No and not PROPKEY_品名 and not PROPKEY_倉庫配送先名 and not PROPKEY_在庫減算有無)
                    {
                        custom.whereParam.Add(item.Key, item.Value.Item2);
                    }
                }
                await RefreshGridData(custom.whereParam, attributeName: info.ComponentName);

                // 出荷予定数をグリッドヘッダに表示
                if (_gridData != null && _gridData.Count > 0)
                {
                    string strCase = string.Empty;
                    string strBara = string.Empty;
                    if (_gridData[0].TryGetValue("出荷予定数(ケース)", out object? value))
                    {
                        strCase = value!.ToString();
                    }
                    if (_gridData[0].TryGetValue("出荷予定数(バラ)", out value))
                    {
                        strBara = value!.ToString();
                    }
                    Attributes[info.ComponentName]["HeaderTextValue"] = $"出荷予定数(ケース)：{strCase}　　出荷予定数(バラ)：{strBara}";
                }
                else
                {
                    Attributes[info.ComponentName]["HeaderTextValue"] = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            bool bEdit = false;
            foreach (IDictionary<string, object> rows in _gridData)
            {
                string case1 = string.Empty;
                string case2 = string.Empty;
                string bara1 = string.Empty;
                string bara2 = string.Empty;
                if (rows.TryGetValue(PROPKEY_出庫ケース数, out object? obj))
                {
                    case1 = obj is null ? "" : obj.ToString()!;
                }
                if (rows.TryGetValue(PROPKEY_出庫バラ数, out obj))
                {
                    bara1 = obj is null ? "" : obj.ToString()!;
                }
                if (rows.TryGetValue(PROPKEY_元出庫ケース数, out obj))
                {
                    case2 = obj is null ? "" : obj.ToString()!;
                }
                if (rows.TryGetValue(PROPKEY_元出庫バラ数, out obj))
                {
                    bara2 = obj is null ? "" : obj.ToString()!;
                }
                if (case1 != case2 || bara1 != bara2)
                {
                    bEdit = true;
                    break;
                }
            }
            if (!bEdit)
            {
                await ComService.DialogShowOK($"出庫ケース数、出庫バラ数を編集してください。", pageName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// ストアドデータ設定_引数データ作成
        /// </summary>
        /// <returns></returns>
        public override async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
        {
            try
            {
                _storedData = new Dictionary<string, object>();
                Dictionary<string, (object, WhereParam)> items = ComService.GetCompItemInfoValues(_searchCompItems);
                if (items is not null)
                {
                    // m_出荷予定集約ID
                    {
                        _storedData[PROPKEY_出荷予定集約ID] = m_出荷予定集約ID;
                    }
                    // 在庫減算有無 //0:引当のみ 1:減算有
                    {
                        _storedData[PROPKEY_在庫減算有無] = items.TryGetValue(PROPKEY_在庫減算有無, out (object, WhereParam) data) ? data.Item1 : "0";
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
            try
            {
                _storedTableData = new List<Dictionary<string, object>>();
                if (_gridData is not null)
                {
                    foreach (IDictionary<string, object> rows in _gridData)
                    {
                        string case1 = string.Empty;
                        string case2 = string.Empty;
                        string bara1 = string.Empty;
                        string bara2 = string.Empty;
                        if (rows.TryGetValue(PROPKEY_出庫ケース数, out object? obj))
                        {
                            case1 = obj is null ? "" : obj.ToString()!;
                        }
                        if (rows.TryGetValue(PROPKEY_出庫バラ数, out obj))
                        {
                            bara1 = obj is null ? "" : obj.ToString()!;
                        }
                        if (rows.TryGetValue(PROPKEY_元出庫ケース数, out obj))
                        {
                            case2 = obj is null ? "" : obj.ToString()!;
                        }
                        if (rows.TryGetValue(PROPKEY_元出庫バラ数, out obj))
                        {
                            bara2 = obj is null ? "" : obj.ToString()!;
                        }
                        if (case1 != case2 || bara1 != bara2)
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
                //if (_gridSelectedData is not null)
                //{
                //    foreach (IDictionary<string, object> rows in _gridSelectedData)
                //    {
                //        Dictionary<string, object> rowdata = new();
                //        foreach (KeyValuePair<string, object> data in rows)
                //        {
                //            rowdata[data.Key] = data.Value;
                //        }
                //        _storedTableData.Add(rowdata);
                //    }
                //}
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 検索条件初期化
        /// </summary>
        protected override async Task InitSearchConditionAsync()
        {
            //親の初期化を呼び出す
            await base.InitSearchConditionAsync();

            // 在庫減算有無を追加する
            List<ValueTextInfo>? dataIsSubtraction = await ComService!.GetValueTextInfo("VW_DROPDOWN_在庫減算有無");
            _cmbIsSubtraction = new CompItemInfo
            {
                CompType = typeof(CompDropDown),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_在庫減算有無 },
                    { "Required", true },
                    { "InitialValue", string.Empty },
                    { "Data", dataIsSubtraction }
                },
                TitleLabel = PROPKEY_在庫減算有無,
                DispTitleLabel = true,
                DispRequiredSuffix = true,
            };
            _searchCompItems.Add(new List<CompItemInfo> { _cmbIsSubtraction });//一番下段に追加される
        }

        /// <summary>
        /// コンポーネントに初期値設定
        /// </summary>
        private void SetInitialData()
        {
            // 出荷予定集約ID取得
            if (InitialData.TryGetValue(PROPKEY_出荷予定集約ID, out object value))
            {
                m_出荷予定集約ID = value.ToString();
            }

            // 各コンポーネントに初期値セット
            foreach (List<CompItemInfo> listItem in _searchCompItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    string strVal = string.Empty;
                    if (InitialData.TryGetValue(item.TitleLabel, out value))
                    {
                        strVal = value.ToString();
                    }

                    switch (item.TitleLabel)
                    {
                        case PROPKEY_入荷No:
                            // 入荷No
                            if (item?.CompObj?.Instance is CompTextBox txtArrivalNo)
                            {
                                txtArrivalNo.InputValue = strVal;
                                txtArrivalNo.Disabled = true;
                                txtArrivalNo.Refresh();
                            }
                            break;
                        case PROPKEY_明細No:
                            // 明細No
                            if (item?.CompObj?.Instance is CompTextBox txtDetailNo)
                            {
                                txtDetailNo.InputValue = strVal;
                                txtDetailNo.Disabled = true;
                                txtDetailNo.Refresh();
                            }
                            break;
                        case PROPKEY_品名:
                            // 品名
                            if (item?.CompObj?.Instance is CompTextBox txtProductName)
                            {
                                txtProductName.InputValue = strVal;
                                txtProductName.Disabled = true;
                                txtProductName.Refresh();
                            }
                            break;
                        case PROPKEY_倉庫配送先名:
                            // 倉庫配送先
                            if (item?.CompObj?.Instance is CompTextBox txtDeliveriesName)
                            {
                                txtDeliveriesName.InputValue = strVal;
                                txtDeliveriesName.Disabled = true;
                                txtDeliveriesName.Refresh();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

        }

    }
}

using Microsoft.AspNetCore.Components;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 外部倉庫入庫
    /// </summary>
    public partial class DialogArrivalsSatelliteStockupContent : ChildPageBasePC
    {
        private const string PROPKEY_販売予定日 = "販売予定日";
        private const string PROPKEY_入荷No = "入荷No";
        private const string PROPKEY_品名コード = "品名コード";
        private const string PROPKEY_産地コード = "産地コード";
        private const string PROPKEY_倉庫コード = "倉庫コード";
        private const string PROPKEY_ゾーンコード = "ゾーンコード";
        private const string PROPKEY_ロケーションコード = "ロケーションコード";

        private const string STORED_PARAM_倉庫コード = "AREA_ID";
        private const string STORED_PARAM_ゾーンコード = "ZONE_ID";
        private const string STORED_PARAM_ロケーションコード = "LOCATION_ID";

        private const string STORED_PARAM_入庫ケース数 = "入庫ケース数";
        private const string STORED_PARAM_入庫バラ数 = "入庫バラ数";
        private const string STORED_PARAM_元入庫ケース数 = "元入庫ケース数";
        private const string STORED_PARAM_元入庫バラ数 = "元入庫バラ数";

        /// <summary>
        /// 初期データ
        /// </summary>
        [Parameter]
        public Dictionary<string, object> InitialData { get; set; } = new Dictionary<string, object>();

        #region private変数

        /// <summary>
        /// 倉庫コードDropDownコンポーネント
        /// </summary>
        private CompDropDown? _cmbAreaCd = null;

        /// <summary>
        /// ゾーンコードDropDownコンポーネント
        /// </summary>
        private CompDropDown? _cmbZoneCd = null;

        /// <summary>
        /// ロケーションNoDropDownコンポーネント
        /// </summary>
        private CompDropDown? _cmbLocationCd = null;

        /// <summary>
        /// 倉庫マスタ情報
        /// </summary>
        private List<MstAreaData> _lstMstArea = new();

        /// <summary>
        /// ゾーンマスタ情報
        /// </summary>
        private List<MstZoneData> _lstMstZone = new();

        /// <summary>
        /// ロケーションマスタ情報
        /// </summary>
        private List<MstLocationData> _lstMstLocation = new();

        /// <summary>
        /// 倉庫ドロップダウンデータ
        /// </summary>
        private IList<ValueTextInfo> _dropdownArea { get; set; } = new List<ValueTextInfo>();

        /// <summary>
        /// ゾーンドロップダウンデータ
        /// </summary>
        private IList<ValueTextInfo> _dropdownZone { get; set; } = new List<ValueTextInfo>();

        /// <summary>
        /// ロケーションドロップダウンデータ
        /// </summary>
        private IList<ValueTextInfo> _dropdownLocation { get; set; } = new List<ValueTextInfo>();

        #endregion

        #region override

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

                // 倉庫、ゾーン情報取得
                await InitAreaZoneLocationData();

                // 初期値設定
                SetInitialData();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        protected override void Dispose()
        {
            base.Dispose();
            // イベント削除
            if (_cmbAreaCd != null)
            {
                _cmbAreaCd.ChangeSelectValue -= OnChangeArea;
            }
            if (_cmbZoneCd != null)
            {
                _cmbZoneCd.ChangeSelectValue -= OnChangeZone;
            }
        }

        /// <summary>
        /// グリッド更新
        /// </summary>
        public override async Task グリッド更新(ComponentProgramInfo info)
        {
            try
            {
                ClassNameSelect custom = new();
                Dictionary<string, (object, WhereParam)> items = ComService.GetCompItemInfoValues(_searchCompItems);
                foreach (KeyValuePair<string, (object, WhereParam)> item in items)
                {
                    // 倉庫コードとゾーンコードは検索条件に含めない
                    if (item.Key is not PROPKEY_倉庫コード and not PROPKEY_ゾーンコード and not PROPKEY_ロケーションコード)
                    {
                        custom.whereParam.Add(item.Key, item.Value.Item2);
                    }
                }
                await RefreshGridData(custom.whereParam, attributeName: info.ComponentName);
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
                if (rows.TryGetValue(STORED_PARAM_入庫ケース数, out object? obj))
                {
                    case1 = obj is null ? "" : obj.ToString()!;
                }
                if (rows.TryGetValue(STORED_PARAM_入庫バラ数, out obj))
                {
                    bara1 = obj is null ? "" : obj.ToString()!;
                }
                if (rows.TryGetValue(STORED_PARAM_元入庫ケース数, out obj))
                {
                    case2 = obj is null ? "" : obj.ToString()!;
                }
                if (rows.TryGetValue(STORED_PARAM_元入庫バラ数, out obj))
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
                await ComService.DialogShowOK($"入庫ケース数もしくは入庫バラ数を編集してください。", pageName);
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
                    // 倉庫コード
                    {
                        _storedData[STORED_PARAM_倉庫コード] = items.TryGetValue(PROPKEY_倉庫コード, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                    // ゾーンコード
                    {
                        _storedData[STORED_PARAM_ゾーンコード] = items.TryGetValue(PROPKEY_ゾーンコード, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                    // ロケーションコード
                    {
                        _storedData[STORED_PARAM_ロケーションコード] = items.TryGetValue(PROPKEY_ロケーションコード, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            await Task.Delay(0);
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
                        if (rows.TryGetValue(STORED_PARAM_入庫ケース数, out object? obj))
                        {
                            case1 = obj is null ? "" : obj.ToString()!;
                        }
                        if (rows.TryGetValue(STORED_PARAM_入庫バラ数, out obj))
                        {
                            bara1 = obj is null ? "" : obj.ToString()!;
                        }
                        if (rows.TryGetValue(STORED_PARAM_元入庫ケース数, out obj))
                        {
                            case2 = obj is null ? "" : obj.ToString()!;
                        }
                        if (rows.TryGetValue(STORED_PARAM_元入庫バラ数, out obj))
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
            await Task.Delay(0);
        }

        #endregion

        #region private

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
        /// コンポーネントに初期値設定
        /// </summary>
        private void SetInitialData()
        {
            // 各コンポーネントに初期値セット
            foreach (List<CompItemInfo> listItem in _searchCompItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    string strVal = string.Empty;
                    if (InitialData.TryGetValue(item.TitleLabel, out object value))
                    {
                        strVal = value.ToString();
                    }

                    switch (item.TitleLabel)
                    {
                        case PROPKEY_販売予定日:
                            // 販売予定日
                            if (item?.CompObj?.Instance is CompDateFromTo dftSalesDate)
                            {
                                dftSalesDate.InputValueFrom = strVal;
                                dftSalesDate.InputValueTo = strVal;
                                dftSalesDate.Refresh();
                            }
                            break;
                        case PROPKEY_入荷No:
                            // 入荷No
                            if (item?.CompObj?.Instance is CompTextBox txtArrivalNo)
                            {
                                txtArrivalNo.InputValue = strVal;
                                txtArrivalNo.Refresh();
                            }
                            break;
                        case PROPKEY_品名コード:
                            // 品名コード
                            if (item?.CompObj?.Instance is CompDropDownDataGrid ddgProductCd)
                            {
                                ddgProductCd.Values = new List<string> { strVal };
                                ddgProductCd.Refresh();
                            }
                            break;
                        case PROPKEY_産地コード:
                            // 産地コード
                            if (item?.CompObj?.Instance is CompDropDownDataGrid ddgOrigins)
                            {
                                ddgOrigins.Values = new List<string> { strVal };
                                ddgOrigins.Refresh();
                            }
                            break;
                        case PROPKEY_倉庫コード:
                            // 倉庫コード
                            if (item?.CompObj?.Instance is CompDropDown)
                            {
                                _cmbAreaCd = (CompDropDown)item.CompObj.Instance;
                                _cmbAreaCd.ChangeSelectValue += OnChangeArea;
                            }
                            break;
                        case PROPKEY_ゾーンコード:
                            // ゾーンコード
                            if (item?.CompObj?.Instance is CompDropDown)
                            {
                                _cmbZoneCd = (CompDropDown)item.CompObj.Instance;
                                _cmbZoneCd.ChangeSelectValue += OnChangeZone;
                            }
                            break;
                        case PROPKEY_ロケーションコード:
                            // ロケーションコード
                            if (item?.CompObj?.Instance is CompDropDown)
                            {
                                _cmbLocationCd = (CompDropDown)item.CompObj.Instance;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            // 倉庫Dropdown初期化
            SetDropdownArea();
            // ゾーンDropdown初期化
            SetDropdownZone();
            // ロケーションDropdown初期化
            SetDropdownLocation();
        }

        /// <summary>
        /// 倉庫、ゾーン、ロケーション情報を取得
        /// </summary>
        /// <returns></returns>
        private async Task InitAreaZoneLocationData()
        {
            try
            {
                // 倉庫
                _lstMstArea = await ComService!.GetArea(true);
                // ゾーン
                _lstMstZone = await ComService!.GetZone(true);
                // ロケーション
                _lstMstLocation = await ComService!.GetLocation(true);
            }
            catch (Exception ex)
            {
                ShowNotifyMessege(NotificationSeverity.Error, pageName, $"ﾏｽﾀ値取得に失敗しました。{ex.Message}");
            }

        }

        /// <summary>
        /// 倉庫コンボボックスの初期化
        /// </summary>
        protected void SetDropdownArea()
        {
            if (_cmbAreaCd is null)
            {
                return;
            }
            _dropdownArea.Clear();
            foreach (MstAreaData item in _lstMstArea)
            {
                ValueTextInfo info = new()
                {
                    Value = item.AreaId,
                    Text = item.AreaName,
                };
                _dropdownArea.Add(info);
            }
            _cmbAreaCd.Data = _dropdownArea;//TODO 警告の抑制。外部からのパラメータセットの抑制
            _cmbAreaCd.Refresh();

        }

        /// <summary>
        /// ゾーンコンボボックスの初期化
        /// </summary>
        protected void SetDropdownZone()
        {
            if (_cmbAreaCd is null
                || _cmbZoneCd is null
                )
            {
                return;
            }
            _dropdownZone.Clear();

            List<MstZoneData> lstZone = _lstMstZone.Where(_ => _.AreaId == _cmbAreaCd.InputValue).ToList();
            foreach (MstZoneData item in lstZone)
            {
                ValueTextInfo info = new()
                {
                    Value = item.ZoneId,
                    Text = item.ZoneName,
                };
                _dropdownZone.Add(info);
            }
            _cmbZoneCd.Data = _dropdownZone;//TODO 警告の抑制。外部からのパラメータセットの抑制

            // DropDownにInputValueが存在しない場合未選択
            if (!lstZone.Any(_ => _.ZoneId == _cmbZoneCd.InputValue))
            {
                _cmbZoneCd.InputValue = string.Empty;//TODO 警告の抑制。外部からのパラメータセットの抑制
            }

            // 再描画
            _cmbZoneCd.Refresh();

        }

        /// <summary>
        /// ロケーションコンボボックスの初期化
        /// </summary>
        protected void SetDropdownLocation()
        {
            if (_cmbAreaCd is null
                || _cmbZoneCd is null
                || _cmbLocationCd is null
                )
            {
                return;
            }
            _dropdownLocation.Clear();

            List<MstLocationData> lstLocation = _lstMstLocation.Where(_ => _.AreaId == _cmbAreaCd.InputValue && _.ZoneId == _cmbZoneCd.InputValue).ToList();
            foreach (MstLocationData item in lstLocation)
            {
                ValueTextInfo info = new()
                {
                    Value = item.LocationId,
                    Text = item.LocationName,
                };
                _dropdownLocation.Add(info);
            }
            _cmbLocationCd.Data = _dropdownLocation;//TODO 警告の抑制。外部からのパラメータセットの抑制

            // DropDownにInputValueが存在しない場合未選択
            if (!lstLocation.Any(_ => _.LocationId == _cmbLocationCd.InputValue))
            {
                _cmbLocationCd.InputValue = string.Empty; //TODO 警告の抑制。外部からのパラメータセットの抑制
            }

            // 再描画
            _cmbLocationCd.Refresh();

        }
        /// <summary>
        /// 倉庫の選択イベント
        /// </summary>
        /// <param name="value"></param>
        private void OnChangeArea(object? sender, object e)
        {
            SetDropdownZone();
        }
        /// <summary>
        /// ゾーンの選択イベント
        /// </summary>
        /// <param name="value"></param>
        private void OnChangeZone(object? sender, object e)
        {
            SetDropdownLocation();
        }
        #endregion
    }
}

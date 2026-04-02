using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 倉庫配送先マスタメンテナンスダイアログ
    /// </summary>
    public partial class DialogDeliveriesFixContent : DialogCommonInputContent
    {
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
        /// 初期化
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // 倉庫、ゾーン、ロケーション情報取得
            await InitAreaZoneLocationData();

            // Blazor へ状態変化を通知
            StateHasChanged();

            // メンバ変数、イベント関連付け
            foreach (List<CompItemInfo> listItem in _inputItems)
            {
                foreach (CompItemInfo item in listItem)
                {
                    if (item?.CompObj?.Instance is not CompDropDown)
                    {
                        continue;
                    }
                    if (item.TitleLabel == "倉庫コード")
                    {
                        _cmbAreaCd = (CompDropDown)item.CompObj.Instance;
                        _cmbAreaCd.ChangeSelectValue += OnChangeArea;
                    }
                    else if (item.TitleLabel == "ゾーンコード")
                    {
                        _cmbZoneCd = (CompDropDown)item.CompObj.Instance;
                        _cmbZoneCd.ChangeSelectValue += OnChangeZone;
                    }
                    else if (item.TitleLabel == "ロケーションNo")
                    {
                        _cmbLocationCd = (CompDropDown)item.CompObj.Instance;
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
        /// 終了処理
        /// </summary>
        protected override void Dispose()
        {
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
        #endregion

        #region private

        /// <summary>
        /// 倉庫、ゾーン、ロケーション情報を取得
        /// </summary>
        /// <returns></returns>
        private async Task InitAreaZoneLocationData()
        {
            try
            {
                // 倉庫 //外部倉庫のみとしたいためwhereparamで絞り込む
                _lstMstArea = await ComService!.GetArea(
                        true
                        , new List<(string key, WhereParam wp)>
                            {
                            ("AREA_TYPE", new WhereParam { val = "1", whereType = enumWhereType.Equal })
                            }
                    );
                // ゾーン
                _lstMstZone = await ComService!.GetZone(true);
                // ロケーション
                _lstMstLocation = await ComService!.GetLocation(true);
            }
            catch (Exception ex)
            {
                ShowNotifyMessege(NotificationSeverity.Error, DialogTitle, $"ﾏｽﾀ値取得に失敗しました。{ex.Message}");
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
            _cmbAreaCd.Data = _dropdownArea; //TODO 警告の抑制。外部からのパラメータセットの抑制
            _cmbAreaCd.Refresh();
        }

        /// <summary>
        /// ゾーンコンボボックスの初期化
        /// </summary>
        protected void SetDropdownZone()
        {
            if (_cmbAreaCd is null || _cmbZoneCd is null)
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
            SetDropdownLocation();
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
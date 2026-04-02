using SharedModels;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 在庫メンテナンスダイアログ
    /// </summary>
    public partial class DialogStocksFixContent : DialogCommonInputContent
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

        /// <summary>
        /// F1ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        protected override async Task OnClickResultF1(object? sender)
        {
            // チェック
            if (false == バリデートチェック())
            {
                return;
            }

            bool retb = false;
            try
            {
                // 入力エリアロック
                ComService.SetCompItemListDisabled(_inputItems, true);

                // 結果変数
                List<ExecResult> lstResult = new();
                int notifyDuration = _sysParams is null ? SharedConst.DEFAULT_NOTIFY_DURATION : _sysParams.NotifyPopupDuration;
                string strSummary = DialogTitle.Replace("\\n", "");
                string strResultMessage = "登録しました。";

                // 確認
                bool? retConfirm = await ComService.DialogShowYesNo("設定を確定しますか？", strSummary);
                retb = retConfirm is not null && (bool)retConfirm;
                if (!retb)
                {
                    return;
                }

                // 登録
                retb = false;
                if (!string.IsNullOrEmpty(ProgramName))
                {
                    // RequestValueにデータを作成する
                    RequestValue rv = RequestValue.CreateRequestProgram(ProgramName);

                    // 情報エリア、入力エリアからWebAPIに渡す値を作成する
                    Dictionary<string, (object, WhereParam)> itemInfos = ComService.GetCompItemInfoValues(_infoItems);
                    Dictionary<string, (object, WhereParam)> itemInputs = ComService.GetCompItemInfoValues(_inputItems);
                    // 入荷No
                    _ = itemInfos.TryGetValue("入荷No", out (object, WhereParam) keyval)
                        ? rv.SetArgumentValue("入荷No", keyval.Item1, "")
                        : rv.SetArgumentValue("入荷No", string.Empty, "");
                    // 明細No
                    _ = itemInfos.TryGetValue("明細No", out keyval)
                        ? rv.SetArgumentValue("明細No", keyval.Item1, "")
                        : rv.SetArgumentValue("明細No", string.Empty, "");
                    // 元パレットNo
                    _ = InitialData.TryGetValue("パレットNo", out object? obj)
                        ? rv.SetArgumentValue("元パレットNo", obj, "")
                        : rv.SetArgumentValue("元パレットNo", string.Empty, "");
                    // 変更後パレットNo
                    _ = itemInputs.TryGetValue("パレットNo", out keyval)
                        ? rv.SetArgumentValue("変更後パレットNo", keyval.Item1, "")
                        : rv.SetArgumentValue("変更後パレットNo", string.Empty, "");
                    // AREA_ID
                    _ = itemInputs.TryGetValue("倉庫コード", out keyval)
                        ? rv.SetArgumentValue("AREA_ID", keyval.Item1, "")
                        : rv.SetArgumentValue("AREA_ID", string.Empty, "");
                    // ZONE_ID
                    _ = itemInputs.TryGetValue("ゾーンコード", out keyval)
                        ? rv.SetArgumentValue("ZONE_ID", keyval.Item1, "")
                        : rv.SetArgumentValue("ZONE_ID", string.Empty, "");
                    // LOCATION_ID
                    _ = itemInputs.TryGetValue("ロケーションNo", out keyval)
                        ? rv.SetArgumentValue("LOCATION_ID", keyval.Item1, "")
                        : rv.SetArgumentValue("LOCATION_ID", string.Empty, "");
                    // 元ケース数
                    _ = InitialData.TryGetValue("在庫数(ケース)", out obj)
                        ? rv.SetArgumentValue("元ケース数", obj, "")
                        : rv.SetArgumentValue("元ケース数", string.Empty, "");
                    // 変更後ケース数
                    _ = itemInputs.TryGetValue("在庫数(ケース)", out keyval)
                        ? rv.SetArgumentValue("変更後ケース数", keyval.Item1, "")
                        : rv.SetArgumentValue("変更後ケース数", string.Empty, "");
                    // 元バラ数
                    _ = InitialData.TryGetValue("在庫数(バラ)", out obj)
                        ? rv.SetArgumentValue("元バラ数", obj, "")
                        : rv.SetArgumentValue("元バラ数", string.Empty, "");
                    // 変更後バラ数
                    _ = itemInputs.TryGetValue("在庫数(バラ)", out keyval)
                        ? rv.SetArgumentValue("変更後バラ数", keyval.Item1, "")
                        : rv.SetArgumentValue("変更後バラ数", string.Empty, "");
                    // 入荷元区分
                    _ = itemInputs.TryGetValue("入荷元区分", out keyval)
                        ? rv.SetArgumentValue("入荷元区分", keyval.Item1, "")
                        : rv.SetArgumentValue("入荷元区分", string.Empty, "");
                    // 備考
                    _ = itemInputs.TryGetValue("備考", out keyval)
                        ? rv.SetArgumentValue("備考", keyval.Item1, "")
                        : rv.SetArgumentValue("備考", string.Empty, "");

                    // WebAPIへアクセス
                    retb = true;
                    ExecResult[]? results = await ComService.SetRequestValue(GetType().Name, rv);
                    if (results == null)
                    {
                        // 実行結果がnullの場合は異常
                        NotificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = $"{strSummary}",
                            Detail = "WebAPIへのアクセスが異常終了しました。ログを確認して下さい。",
                            Duration = notifyDuration
                        });
                        return;
                    }
                    else
                    {
                        // 実行結果が返った場合
                        lstResult = new List<ExecResult>(results);
                    }
                }

                // 実行結果を異常・正常・確認に分ける
                List<ExecResult> lstError = lstResult.Where(_ => _.RetCode < 0).OrderBy(_ => _.ExecOrderRank).ToList();
                List<ExecResult> lstSuccess = lstResult.Where(_ => _.RetCode == 0).OrderBy(_ => _.ExecOrderRank).ToList();
                List<ExecResult> lstConfirm = lstResult.Where(_ => _.RetCode > 0).OrderBy(_ => _.ExecOrderRank).ToList();

                if (lstError.Count() > 0)
                {
                    // 異常結果がある場合
                    retb = false;

                    // 異常メッセージを全て通知
                    foreach (ExecResult result in lstError)
                    {
                        NotificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = $"{strSummary}",
                            Detail = result.Message,
                            Duration = notifyDuration
                        });
                    }
                }
                else
                {
                    // 異常結果が無い場合
                    retb = true;

                    // 正常結果のメッセージがある場合、全て通知
                    foreach (ExecResult result in lstSuccess)
                    {
                        if (!string.IsNullOrEmpty(result.Message))
                        {
                            NotificationService.Notify(new NotificationMessage()
                            {
                                Severity = NotificationSeverity.Success,
                                Summary = $"{strSummary}",
                                Detail = result.Message,
                                Duration = notifyDuration
                            });
                        }
                    }

                    // 確認結果がある場合、全ての確認ダイアログ表示
                    foreach (ExecResult result in lstConfirm)
                    {
                        bool? ret = await ComService.DialogShowYesNo(result.Message);
                        retb = ret is not null && (bool)ret;
                        if (!retb)
                        {
                            break;
                        }
                    }
                }

                // 正常終了で結果メッセージがある場合は通知
                if (retb)
                {
                    if (!string.IsNullOrEmpty(strResultMessage))
                    {
                        NotificationService.Notify(new NotificationMessage()
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = $"{strSummary}",
                            Detail = strResultMessage,
                            Duration = notifyDuration
                        });
                    }
                }
            }
            finally
            {
                // 入力エリアロック解除
                ComService.SetCompItemListDisabled(_inputItems, false);
            }

            // 正常終了ならダイアログ閉じる
            if (retb)
            {
                DialogService.CloseSide(retb);
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
                // 倉庫
                _lstMstArea = await ComService!.GetArea(true);
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
                || _cmbLocationCd is null)
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
                _cmbLocationCd.InputValue = string.Empty;//TODO 警告の抑制。外部からのパラメータセットの抑制
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
using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 入荷検品 入荷・明細№読取ステップ
    /// </summary>
    public partial class StepItemArrivalsInspectsSearch : StepItemBase
    {
        private StepItemArrivalsInspectsViewModel? model;
        #region 表示パラメータ
        //protected IList<ValueTextInfo> dropdownArrivalDetailNo { get; set; } = new List<ValueTextInfo>();
        #endregion 表示パラメータ

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "AreaCd";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemArrivalsInspectsViewModel?)PageVm;

            // 初期処理呼び出し
            await InitProcAsync();
        }

        /// <summary>
        /// スキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

            if (value.Length == SharedConst.LEN_ZONE_ID)
            {
                // ゾーンID
                model!.ZoneCd = value;
                await ScanZoneCd(value);
                SetElementIdFocus("Location");
            }
            else if (value.Length == SharedConst.LEN_NYUKA_MEISAI_NO)
            {

                model!.CarNumber = string.Empty;

                // 入荷明細NO
                model!.ArrivalDetailNoDisp = value;
                model!.ArrivalDetailNo = value;
                _ = OnChangeArrivalNo(value);
                // 入荷明細Noを読込んだ場合は確定処理を実行する
                await ContainerMainLayout!.ButtonClickF1();
            }
            else if (IsLocationBarcode(value))
            {
                // ロケーションID
                model!.LocationCd = value;
                await ScanLocationCd(value);
                SetElementIdFocus("CarNumber");
            }
            StateHasChanged();
        }

        /// <summary>
        /// 確定前チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            model!.ArrivalDetailNo = model!.ArrivalDetailNoDisp;
            model.CarNumber = string.Empty;

            // 入荷明細Noチェック
            _ = new
            // 入荷明細Noチェック
            List<IDictionary<string, object>>();
            ClassNameSelect select = new()
            {
                className = GetType().Name,
            };
            select.whereParam.Add("入荷明細No", new WhereParam { val = model!.ArrivalDetailNo, whereType = enumWhereType.Equal });
            List<IDictionary<string, object>> datas = await ComService!.GetSelectData(select);
            if (null == datas || datas.Count <= 0)
            {
                await ComService.DialogShowOK($"検品対象の入荷明細Noではありません。", pageName);
                SetElementIdFocus("ArrivalDetailNo");
                return false;
            }

            // STEP遷移チェックで入荷No、明細Noを渡すためグリッドの情報をViewModelにバインドさせる
            await GridSelectViewModelBind();
            return true;
        }

        public override async Task 確定前処理(ComponentProgramInfo info)
        {
            try
            {
                // 倉庫コード、ゾーンコード、ロケーションコード、車番を保持する
                await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_AREA_ID, model!.AreaCd);
                await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ZONE_ID, model!.ZoneCd);
                await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_LOCATION_ID, model!.LocationCd);
                await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_CARNUMBER, model!.CarNumber);
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            await 次ステップへ(info);
        }

        /// <summary>
        /// 戻る
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F4画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            // 入庫メニューに遷移
            NavigationManager.NavigateTo($"mobile_arrival_menu");
        }

        #endregion

        #region Event

        /// <summary>
        /// 倉庫の選択イベント
        /// </summary>
        /// <param name="value"></param>
        private void OnChangeArea(object value)
        {
            model!.LocationCd = string.Empty;
            model!.ZoneCd = string.Empty;
            SetDropdownZone(model!.AreaCd);
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);
        }

        /// <summary>
        /// ゾーンの選択イベント
        /// </summary>
        /// <param name="value"></param>
        private void OnChangeZone(object value)
        {
            model!.LocationCd = string.Empty;
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);
        }

        /// <summary>
        /// ロケーションの選択イベント
        /// </summary>
        /// <param name="value"></param>
        private void OnChangeLocation(object value)
        {
            // 何もしない
        }

        /// <summary>
        /// 車番変更イベント
        /// </summary>
        /// <param name="value"></param>
        private async void OnChangeCarNumber(string value)
        {
            await Task.Delay(0);//警告の抑制
            // 検索する為
            //if (value.Length < SharedConst.LEN_CAR_NUMBER)
            //{
            //    return;
            //}

            // 車番で検索する為、入荷明細Noを一旦削除
            //model!.ArrivalDetailNoDisp =
            model!.ArrivalDetailNo = string.Empty;
            model!.CarNumber = value;

            // 車番を入力して、消した時
            if (string.IsNullOrEmpty(model!.CarNumber))
            {
                // 検索結果なしにする
                model!.ArrivalDetailNo = "0";
            }

            // 車番桁数になったらデータを取得してグリッドを更新する
            _ = LoadGridData();
        }


        /// <summary>
        /// 入荷・明細No変更イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangeArrivalNo(string value)
        {
            await Task.Delay(0);//警告の抑制
            // 入荷明細の桁数が6～9桁以外の場合は、グリッドクリア
            if (value.Length is < SharedConst.LEN_NYUKA_NO or > SharedConst.LEN_NYUKA_MEISAI_NO)
            {
                _ = Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>();
                return;
            }

            // 入荷Noを取得する
            model!.ArrivalNoSearch = value[..SharedConst.LEN_NYUKA_NO];

            // データを取得する
            _ = LoadGridData();

            //// 1件だけ取得された
            //if (1 == _gridData.Count && !string.IsNullOrEmpty(model!.ArrivalDetailNo))
            //{
            //    model!.CarNumberDisp =
            //    model!.CarNumber = _gridData[0]["車番"].ToString() ?? "";
            //    StateHasChanged();
            //}
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            // 保持している倉庫コード、ゾーンコード、ロケーションコード、車番を取得する
            // 入庫メニューでクリアしている
            model!.AreaCd = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_AREA_ID);
            model!.ZoneCd = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ZONE_ID);
            model!.LocationCd = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_LOCATION_ID);
            model!.CarNumber = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_CARNUMBER);
            model!.CarNumberDisp = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_CARNUMBER);

            //dropdownArrivalDetailNo = await ComService!.GetValueTextInfo("VW_DROPDOWN_入荷明細No_入荷検品");

            // コンボボックス初期化
            await InitComboAreaZoneLocation();

            // コンボボックス更新
            SetDropdownZone(model!.AreaCd);
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);

            //// 車番が保持されていたらグリッドを更新する
            //if (!string.IsNullOrEmpty(model!.CarNumber))
            //{
            //    await LoadGridData();
            //}
            // 入荷明細Noが保持されていたらグリッドを更新する
            if (!string.IsNullOrEmpty(model!.ArrivalDetailNo))
            {
                _ = OnChangeArrivalNo(model!.ArrivalDetailNo);
            }
            else if (!string.IsNullOrEmpty(model!.ArrivalNoSearch))
            {
                //入荷明細Noがなく、検索用の入荷Noのみ存在する場合は、入荷明細Noに検索用入荷No6桁をセットしてテキストボックスに表示しておく。
                model!.ArrivalDetailNoDisp = model.ArrivalNoSearch;
                _ = OnChangeArrivalNo(model!.ArrivalNoSearch);
            }
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //model!.AreaCd = string.Empty;
            //model!.ZoneCd = string.Empty;
            //model!.LocationCd = string.Empty;
            //model!.CarNumber = string.Empty;
            //model!.CarNumberDisp = string.Empty;
            //model!.ArrivalDetailNo = string.Empty;
            //model!.ArrivalDetailNoDisp = string.Empty;      // 表示と検索k条件を別にする為
        }

        /// <summary>
        /// ゾーンコードスキャン処理
        /// </summary>
        /// <param name="zoneCd"></param>
        private async Task ScanZoneCd(string zoneCd)
        {
            MstZoneData? infoZone = _lstMstZone.FirstOrDefault(_ => _.ZoneId == zoneCd);
            if (infoZone != null)
            {
                model!.AreaCd = infoZone.AreaId;
                SetDropdownZone(model!.AreaCd);
                model!.ZoneCd = zoneCd;
                SetDropdownLocation();
                model!.LocationCd = string.Empty;
            }
            else
            {
                // エラーメッセージ
                await ShowNotExistZone(zoneCd);

                model!.ZoneCd = string.Empty;
                model!.LocationCd = string.Empty;
                SetDropdownLocation();
            }
        }

        /// <summary>
        /// ロケーションコードスキャン処理
        /// </summary>
        /// <param name="locationCd"></param>
        private async Task ScanLocationCd(string locationCd)
        {
            MstLocationData? infoLocation = _lstMstLocation.FirstOrDefault(_ => _.LocationId == locationCd);
            if (infoLocation != null)
            {
                model!.AreaCd = infoLocation.AreaId;
                SetDropdownZone(model!.AreaCd);
                model!.ZoneCd = infoLocation.ZoneId;
                SetDropdownLocation(model!.AreaCd, model!.ZoneCd);
                model!.LocationCd = locationCd;
            }
            else
            {
                // エラーメッセージ
                await ShowNotExistLocation(locationCd);

                model!.LocationCd = string.Empty;
            }
        }

        #endregion

        #region event

        /// <summary>
        /// OnCellRenderのCallBack
        /// </summary>
        /// <param name="args"></param>
        private void CellRender(DataGridCellRenderEventArgs<IDictionary<string, object>> args)
        {
            try
            {
                // 出荷状態の背景色変更
                if (@"入荷-\n明細No." == args.Column.Title)
                {
                    if (args.Data.TryGetValue("入荷状態", out object? value))
                    {
                        if (value.ToString() == "2") //TODO 固定値はリファクタリングしたい
                        {
                            args.Attributes.Add("class", $"{_sysParams.HTArrivalInspectStatusColor}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }
        /// <summary>
        /// ロケーションコンボボックスの有効無効判定。
        /// システムパラメータを参照し、コンボかテキスト表示かの閾値件数で判定を行う
        /// ただし、ＰＣの場合は常にコンボボックス表示としたいため、パラメータにかかわらず常にfalse
        /// </summary>
        /// <returns>true : テキスト表示,false:コンボボックス表示</returns>
        private bool IsLocationTextEnable()
        {
            StepItemArrivalsInspectsViewModel? model = (StepItemArrivalsInspectsViewModel?)PageVm;
            return GetCountLocationList(model!.AreaCd, model!.ZoneCd) > (_sysParams is null ? int.MaxValue : _sysParams.HT_LocComBoxMaxCount) && ChildBaseService.IsHandy;
        }
        /// <summary>
        /// ロケーション名を取得する。StepBaseのメソッドを直接呼ぶとなぜかできなかったので、一旦ページ側で定義。
        /// TODO 類似のメソッドができることがあるので、できれば共通化などでリファクタリングする
        /// </summary>
        /// <returns></returns>
        private string GetLocationName()
        {
            StepItemArrivalsInspectsViewModel? model = (StepItemArrivalsInspectsViewModel?)PageVm;
            return GetLocationName(model!.LocationCd);
        }
        #endregion
    }
}


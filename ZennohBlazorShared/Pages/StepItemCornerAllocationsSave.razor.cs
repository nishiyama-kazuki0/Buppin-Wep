using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// コーナー割付/倉庫配送先選択
    /// </summary>
    public partial class StepItemCornerAllocationsSave : StepItemBase
    {
        private StepItemCornerAllocationsViewModel? model;


        #region 表示パラメータ
        protected IList<ValueTextInfo> dropdownHaiso { get; set; } = new List<ValueTextInfo>();
        #endregion 表示パラメータ

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "DeliveryCd";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemCornerAllocationsViewModel?)PageVm;

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
            await Task.Delay(0);
            // 倉庫、ゾーン、ロケーションの名称をセットする
            MstAreaData? area = _lstMstArea.FirstOrDefault(_ =>
                _.AreaId == model!.AreaCd
            );
            model!.AreaNm = area?.AreaName ?? string.Empty;
            MstZoneData? zone = _lstMstZone.FirstOrDefault(_ =>
                _.AreaId == model!.AreaCd
                && _.ZoneId == model!.ZoneCd
            );
            model!.ZoneNm = zone?.ZoneName ?? string.Empty;
            MstLocationData? loca = _lstMstLocation.FirstOrDefault(_ =>
                _.AreaId == model!.AreaCd
                && _.ZoneId == model!.ZoneCd
                && _.LocationId == model!.LocationCd
            );
            model!.LocationNm = loca?.LocationName ?? string.Empty;

            return true;
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            model!.PalletNo =
            model!.DeliveryCd =
            model!.Delivery =
            model!.CornerId =
            model!.Corner = string.Empty;

            model!.AreaCd =
            model!.ZoneCd =
            model!.LocationCd =
            model!.AreaNm =
            model!.ZoneNm =
            model!.LocationNm = string.Empty;
            await 前ステップへ(info);
        }

        /// <summary>
        /// 解除
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F3画面遷移(ComponentProgramInfo info)
        {
            model!.PalletNo =
            model!.DeliveryCd =
            model!.Delivery =
            model!.CornerId =
            model!.Corner = string.Empty;

            model!.AreaCd =
            model!.ZoneCd =
            model!.LocationCd =
            model!.AreaNm =
            model!.ZoneNm =
            model!.LocationNm = string.Empty;
            await 前ステップへ(info);
        }
        /// <summary>
        /// 戻る
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F4画面遷移(ComponentProgramInfo info)
        {
            await 前ステップへ(info);
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
        /// 倉庫配送先の選択イベント
        /// </summary>
        /// <param name="value"></param>
        private void OnChangeDelivery(object value)
        {
            model!.DeliveryCd = (string)value;

            List<ValueTextInfo> lstInfo = dropdownHaiso.Where(_ => _.Value == (string)value).ToList();
            if (lstInfo != null && lstInfo.Count > 0)
            {
                model!.CornerId = lstInfo[0].Value1;
                model!.Corner = lstInfo[0].Value2;
            }
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            dropdownHaiso = await ComService!.GetValueTextInfo("VW_DROPDOWN_倉庫配送先コード_コーナー割付");

            // コンボボックス初期化
            await InitComboAreaZoneLocation();

            // コンボボックス更新
            SetDropdownZone(model!.AreaCd);
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            model!.DeliveryCd =
            model!.Delivery =
            model!.CornerId =
            model!.Corner = string.Empty;

            model!.AreaCd =
            model!.ZoneCd =
            model!.LocationCd =
            model!.AreaNm =
            model!.ZoneNm =
            model!.LocationNm = string.Empty;
        }

        /// <summary>
        /// データの読込
        /// </summary>
        /// <returns></returns>
        private async Task<int> LoadDataAsync()
        {
            int nData = 0;
            try
            {
                if (!string.IsNullOrEmpty(model!.DeliveryCd))
                {
                    nData = await LoadViewModelBind();
                }

                if (nData == 0)
                {
                    ClearData();
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
            return nData;
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
        /// <summary>
        /// ロケーションコンボボックスの有効無効判定。
        /// システムパラメータを参照し、コンボかテキスト表示かの閾値件数で判定を行う
        /// ただし、ＰＣの場合は常にコンボボックス表示としたいため、パラメータにかかわらず常にfalse
        /// </summary>
        /// <returns>true : テキスト表示,false:コンボボックス表示</returns>
        private bool IsLocationTextEnable()
        {
            StepItemCornerAllocationsViewModel? model = (StepItemCornerAllocationsViewModel?)PageVm;
            return GetCountLocationList(model!.AreaCd, model!.ZoneCd) > (_sysParams is null ? int.MaxValue : _sysParams.HT_LocComBoxMaxCount) && ChildBaseService.IsHandy;
        }
        /// <summary>
        /// ロケーション名を取得する。StepBaseのメソッドを直接呼ぶとなぜかできなかったので、一旦ページ側で定義。
        /// TODO 類似のメソッドができることがあるので、できれば共通化などでリファクタリングする
        /// </summary>
        /// <returns></returns>
        private string GetLocationName()
        {
            StepItemCornerAllocationsViewModel? model = (StepItemCornerAllocationsViewModel?)PageVm;
            return GetLocationName(model!.LocationCd);
        }

        #endregion

    }
}
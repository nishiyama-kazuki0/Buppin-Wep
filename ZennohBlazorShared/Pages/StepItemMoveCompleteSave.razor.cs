using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 切出搬送/切出先入力
    /// </summary>
    public partial class StepItemMoveCompleteSave : StepItemBase
    {
        private StepItemMoveCompleteViewModel? model;

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
            model = (StepItemMoveCompleteViewModel?)PageVm;
            // 初期処理
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
                SetElementIdFocus("LocationCd");
            }
            else if (IsLocationBarcode(value))
            {
                // ロケーションID
                model!.LocationCd = value;
                await ScanLocationCd(value);
                SetElementIdFocus("AreaCd");
            }
            else
            {
                await Task.Delay(0);
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
            //ﾛｹｰｼｮﾝはマスタ件数によってテキストとコンボを切り替えるようにしたため、テキストでバリデーションエラー後にコンボに切り替えると
            //バリデーションエラーが解消されないため、テキストの場合はバリデーションを行わない
            //そのため、下記のように確定前チェックで意図的にチェックを行うこととする。//TODO EditContentを継承して、バリデーションエラーメッセージをAllClearできるようになればよいが、EditContentクラスがSeals宣言のため、できない。今後のアップデートに期待
            if (string.IsNullOrEmpty(model!.LocationCd))
            {
                ShowNotifyMessege(NotificationSeverity.Error, pageName, "ﾛｹｰｼｮﾝは必須です。");
                return false;
            }
            return !string.IsNullOrEmpty(model!.AreaCd) &&
                !string.IsNullOrEmpty(model!.ZoneCd) &&
                !string.IsNullOrEmpty(model!.LocationCd);
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            //今回確定した切り出し搬送のロケーション情報をストレージに保存する
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_AREA_ID, model!.AreaCd);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_ZONE_ID, model!.ZoneCd);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_LOCATION_ID, model!.LocationCd);

            //bool? ret = await ComService.DialogShowYesNo("ｺｰﾅｰ別仕分を行いますか？");
            //bool retb = ret is not null && (bool)ret;
            bool retb = true; //20240430 必須でコーナー別仕分に遷移したいので、YesNoダイアログは表示しない対応。
            if (retb)//常にコーナー別仕分け画面へ遷移するとする
            {
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID, model!.AreaCd); //Todo できればBaseにSetするメソッドを追加してDEFINE_COMPONENTS等に設定で対応したい
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID, model!.ZoneCd);
                // コーナ別仕分画面に遷移
                NavigationManager.NavigateTo("sorting_by_corners");
            }
            else
            {
                if (model!.IsRireki)
                {
                    // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
                    await ShipInfoLocalStorage();
                    NavigationManager.NavigateTo(model!.GetFirstRirekiUrl());
                }
                else
                {
                    // 出庫メニューに遷移
                    NavigationManager.NavigateTo("mobile_ship_menu");
                }
            }
        }

        /// <summary>
        /// コーナー
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F2画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
            // コーナー搬送画面に遷移
            NavigationManager.NavigateTo("move_complete_corner");
        }

        /// <summary>
        /// 明細
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F3画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
            // パレット照会画面に遷移
            NavigationManager.NavigateTo("pallet_inventory_inquiry");
        }

        /// <summary>
        /// 戻る
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F4画面遷移(ComponentProgramInfo info)
        {
            try
            {
                if (model!.IsRireki)
                {
                    // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
                    await ShipInfoLocalStorage();
                    string uri = model!.GetFirstRirekiUrl();
                    if (string.IsNullOrWhiteSpace(uri))
                    {
                        //履歴のURIが取得できない場合は強制的に前ステップへ
                        await 前ステップへ(info);
                    }
                    else
                    {
                        NavigationManager.NavigateTo(uri);
                    }
                }
                else
                {
                    await 前ステップへ(info);
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync($"{ex.Message}");
                await 前ステップへ(info);
            }
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

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            // コンボボックス初期化
            await InitComboAreaZoneLocation();

            // ストレージに前回の切り出しロケーション情報があればモデルにセットする
            string areaPre = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_AREA_ID);
            if (model is not null && !string.IsNullOrEmpty(areaPre))
            {
                model.AreaCd = areaPre;
                //出庫メニューに戻るまでは、保持しておきたいので取り出し後に再度セットする
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_AREA_ID, model!.AreaCd);
            }
            string zonePre = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_ZONE_ID);
            if (model is not null && !string.IsNullOrEmpty(zonePre))
            {
                model.ZoneCd = zonePre;
                //出庫メニューに戻るまでは、保持しておきたいので取り出し後に再度セットする
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_ZONE_ID, model!.ZoneCd);
            }
            string locationPre = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_LOCATION_ID);
            if (model is not null && !string.IsNullOrEmpty(locationPre))
            {
                model.LocationCd = locationPre;
                //出庫メニューに戻るまでは、保持しておきたいので取り出し後に再度セットする
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_LOCATION_ID, model!.LocationCd);
            }

            // コンボボックス更新
            SetDropdownZone(model!.AreaCd);
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);

            _ = await LoadViewModelBind();
            // 混載状態を更新する
            _ = InvokeAsync(async () =>
            {
                PalletInfo info = await GetPalletInfo(model!.PalletNo);
                model!.IsMixed = info.IsMixed;
                StateHasChanged();
            });
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //TODO 正直不要か
            model!.AreaCd =
            model!.ZoneCd =
            model!.LocationCd = string.Empty;
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
            StepItemMoveCompleteViewModel? model = (StepItemMoveCompleteViewModel?)PageVm;
            return GetCountLocationList(model!.AreaCd, model!.ZoneCd) > (_sysParams is null ? int.MaxValue : _sysParams.HT_LocComBoxMaxCount) && ChildBaseService.IsHandy;
        }
        /// <summary>
        /// ロケーション名を取得する。StepBaseのメソッドを直接呼ぶとなぜかできなかったので、一旦ページ側で定義。
        /// TODO 類似のメソッドができることがあるので、できれば共通化などでリファクタリングする
        /// </summary>
        /// <returns></returns>
        private string GetLocationName()
        {
            StepItemMoveCompleteViewModel? model = (StepItemMoveCompleteViewModel?)PageVm;
            return GetLocationName(model!.LocationCd);
        }

        #endregion
    }
}
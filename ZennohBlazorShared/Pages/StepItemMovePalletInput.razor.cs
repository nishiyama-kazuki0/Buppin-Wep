using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット移動/移動先入力
    /// </summary>
    public partial class StepItemMovePalletInput : StepItemBase
    {
        private StepItemMovePalletViewModel? model;

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
            model = (StepItemMovePalletViewModel?)PageVm;

            // 初期処理呼び出し
            await InitProcAsync();
        }

        /// <summary>
        /// HTスキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            await Task.Delay(0);

            // 棚番の読取
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
            if (model!.IsZanKakuno == true)
            {
                // パレットピッキング【倉庫別】、パレットピッキング【倉庫配送先別】の【残格納】機能で遷移している場合は
                // パレット移動で確定すると切出搬送のステップ２に遷移する
                // ※パレットNoは摘取ピック画面読み込んでた先パレット
                //   摘取ピックを繰り返した場合は最後に確定した先パレット
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.SPalletNo);
                // 切出搬送画面に遷移
                NavigationManager.NavigateTo("move_complete");
            }
            else if (model!.IsCorner == true)
            {
                if (model!.IsRireki)
                {
                    // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
                    await ShipInfoLocalStorage();
                    string uriTo = model!.GetFirstRirekiUrl();

                    if (string.IsNullOrWhiteSpace(uriTo)
                        || (uriTo == "move_pallet" && ClassName.Equals(typeof(StepItemMovePalletInput).Name))
                        )//ﾋﾟｯｸボタンを押下して、さらに自分自身に戻ってくる場合は画面遷移が効かないので修正
                    {
                        await 前ステップへ(info);
                    }
                    else
                    {
                        NavigationManager.NavigateTo(uriTo);
                    }
                }
                else
                {
                    model!.PalletNo = string.Empty;
                    await 前ステップへ(info);
                }
            }
            else
            {
                model!.PalletNo = string.Empty;
                await 前ステップへ(info);
            }
        }

        /// <summary>
        /// ピック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F2画面遷移(ComponentProgramInfo info)
        {
            if (model!.IsZanKakuno == true || model!.IsCorner == true)
            {
                // パレットピッキング【倉庫別】【倉庫配送先別】の残格納ボタンから遷移されている場合はピックボタンは非表示なので処理させない
                // コーナー仕分から遷移されている場合はピックボタンは非表示なので処理させない
                return;
            }

            ClassNameSelect select = new()
            {
                viewName = "VW_HT_パレット移動入力_ピック",
            };
            select.whereParam.Add("パレットNo", new WhereParam { val = model!.PalletNo, whereType = enumWhereType.Equal });
            List<IDictionary<string, object>> datas = await ComService!.GetSelectData(select);
            if (null != datas && datas.Count > 0)
            {
                IDictionary<string, object> dic = datas.First();
                string areaCd = ComService.GetValueString(dic, "AREA_ID");
                string zoneCd = ComService.GetValueString(dic, "ZONE_ID");

                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID, areaCd);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID, zoneCd);

                // パレットピッキング【倉庫別】に遷移
                NavigationManager.NavigateTo("picking_pallet");
            }
            else
            {
                await ComService.DialogShowOK($"ﾋﾟｯｸ対象が存在しないﾊﾟﾚｯﾄです。", pageName);
                SetElementIdFocus("AreaCd");
                return;
            }
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

                    if (string.IsNullOrWhiteSpace(uri)
                        || (uri == "move_pallet" && ClassName.Equals(typeof(StepItemMovePalletInput).Name))
                        )//ﾋﾟｯｸボタンを押下して、さらに自分自身に戻ってくる場合は画面遷移が効かないので修正
                    {
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
                //何かエラーが発生した場合は強制的に前ステップとする
                _ = ComService.PostLogAsync(ex.Message);
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
            StepItemMovePalletViewModel? model = (StepItemMovePalletViewModel?)PageVm;
            model!.LocationCd = string.Empty;
            model!.ZoneCd = string.Empty;
            SetDropdownZone(model!.AreaCd);
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);

            // 推奨ロケーションクリア
            model!.RecomLocationL = string.Empty;
            model!.RecomLocationM = string.Empty;
            model!.RecomLocationH = string.Empty;

        }

        /// <summary>
        /// ゾーンの選択イベント
        /// </summary>
        /// <param name="value"></param>
        private async void OnChangeZone(object value)
        {
            await Task.Delay(0);
            StepItemMovePalletViewModel? model = (StepItemMovePalletViewModel?)PageVm;
            model!.LocationCd = string.Empty;
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);

            // 推奨ロケーションを表示する
            _ = LoadRecomLocationAsync();
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

            // ゾーン、ロケーションはステップ１から引き継いだ倉庫コード、ゾーンコードに紐づく値に更新する
            SetDropdownZone(model!.AreaCd);
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);

            // 混載状態を更新する
            _ = InvokeAsync(async () =>
            {
                PalletInfo info = await GetPalletInfo(model!.PalletNo);
                model!.IsMixed = info.IsMixed;
                model!.IsAlert = info.IsReserved;
                model!.AlertTitle = info.ReservedAlertTitle;
                model!.AlertText = info.ReservedAlertText;
                StateHasChanged();
            });

            // 推奨ロケ情報の読み込み
            _ = LoadRecomLocationAsync();

            // 在庫情報を取得してカードリストに表示する
            _ = LoadCardListData();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            if (model!.IsZanKakuno == true || model!.IsCorner == true)
            {
                // パレットピッキング【倉庫別】【倉庫配送先別】の残格納ボタンから遷移されている場合はピックボタンは非表示にする
                // コーナー仕分から遷移されている場合はピックボタンは非表示にする
                Attributes[STR_ATTRIBUTE_FUNC]["button2text"] = "";
                Attributes[STR_ATTRIBUTE_FUNC]["IconName2"] = "remove";
                UpdateFuncButton(Attributes[STR_ATTRIBUTE_FUNC]);
                //パレットNoから現在の倉庫コード、ゾーンコードを取得し、セットする。
                GetPalletLocation();
            }
            //model!.AreaCd =
            //model!.ZoneCd =
            model!.LocationCd = string.Empty;

        }
        /// <summary>
        /// パレット位置情報取得処理
        /// </summary>
        private async void GetPalletLocation()
        {
            model!.AreaCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID);
            SetDropdownZone(model!.AreaCd);
            model!.ZoneCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);
            StateHasChanged();
        }

        /// <summary>
        /// 推奨ロケ情報の読み込み
        /// </summary>
        /// <returns></returns>
        private async Task LoadRecomLocationAsync()
        {
            try
            {
                List<IDictionary<string, object>> datas = new();
                ClassNameSelect select = new()
                {
                    viewName = "VW_HT_推奨ロケ情報",
                };
                select.whereParam.Add("倉庫コード", new WhereParam { val = model!.AreaCd, whereType = enumWhereType.Equal });
                select.whereParam.Add("ゾーンコード", new WhereParam { val = model!.ZoneCd, whereType = enumWhereType.Equal });
                datas = await ComService!.GetSelectData(select);
                if (null != datas && datas.Count > 0)
                {
                    IDictionary<string, object> dic = datas.First();
                    model!.RecomLocationL = ComService.GetValueString(dic, "低荷推奨ロケ");
                    model!.RecomLocationM = ComService.GetValueString(dic, "中荷推奨ロケ");
                    model!.RecomLocationH = ComService.GetValueString(dic, "高荷推奨ロケ");
                }
                else
                {
                    model!.RecomLocationL = string.Empty;
                    model!.RecomLocationM = string.Empty;
                    model!.RecomLocationH = string.Empty;
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
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

                // 推奨ロケーションを表示する
                _ = LoadRecomLocationAsync();
            }
            else
            {
                // エラーメッセージ
                await ShowNotExistZone(zoneCd);

                model!.ZoneCd = string.Empty;
                model!.LocationCd = string.Empty;
                SetDropdownLocation();

                // 推奨ロケーションクリア
                model!.RecomLocationL = string.Empty;
                model!.RecomLocationM = string.Empty;
                model!.RecomLocationH = string.Empty;
            }
        }

        /// <summary>
        /// ロケーションコードスキャン処理
        /// </summary>
        /// <param name="locationCd"></param>
        private async Task ScanLocationCd(string locationCd)
        {
            MstLocationData? infoLocation = _lstMstLocation.SingleOrDefault(_ => _.LocationId == locationCd);
            if (infoLocation != null)
            {
                model!.AreaCd = infoLocation.AreaId;
                SetDropdownZone(model!.AreaCd);
                model!.ZoneCd = infoLocation.ZoneId;
                SetDropdownLocation(model!.AreaCd, model!.ZoneCd);
                model!.LocationCd = locationCd;

                // 推奨ロケーションを表示する
                _ = LoadRecomLocationAsync();
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
            StepItemMovePalletViewModel? model = (StepItemMovePalletViewModel?)PageVm;
            return GetCountLocationList(model!.AreaCd, model!.ZoneCd) > (_sysParams is null ? int.MaxValue : _sysParams.HT_LocComBoxMaxCount) && ChildBaseService.IsHandy;
        }
        /// <summary>
        /// ロケーション名を取得する。StepBaseのメソッドを直接呼ぶとなぜかできなかったので、一旦ページ側で定義。
        /// TODO 類似のメソッドができることがあるので、できれば共通化などでリファクタリングする
        /// </summary>
        /// <returns></returns>
        private string GetLocationName()
        {
            StepItemMovePalletViewModel? model = (StepItemMovePalletViewModel?)PageVm;
            return GetLocationName(model!.LocationCd);
        }
        #endregion
    }
}
using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// コーナー別仕分（コーナー仕分確定）
    /// </summary>
    public partial class StepItemSortingByCornersSave : StepItemBase
    {
        private StepItemSortingByCornersViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "SortingPalletNo";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemSortingByCornersViewModel?)PageVm;

            // 初期処理呼び出し
            await InitProcAsync();
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            (int, string) ret = await GetHikiateZumiZaikoCorner(model!.PalletNo);
            if (ret.Item1 > 0)
            {
                model!.SortingPalletNo = string.Empty;
                model!.SortingCase = string.Empty;
                model!.SortingBara = string.Empty;

                // 作業が残っている場合は、ステップ２へ戻る
                await 前ステップへ(info);
            }
            else
            {
                if (model!.IsRireki)
                {
                    if (await GetPalletZanStock(model!.PalletNo))
                    {
                        // パレット移動へ遷移する
                        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
                        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
                        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID, model!.AreaCd); //Todo できればBaseにSetするメソッドを追加してDEFINE_COMPONENTS等に設定で対応したい
                        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID, model!.ZoneCd);
                        // パレット移動画面に遷移
                        NavigationManager.NavigateTo("move_pallet");
                    }
                    else
                    {
                        // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
                        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
                        await ShipInfoLocalStorage();
                        NavigationManager.NavigateTo(model!.GetFirstRirekiUrl());
                    }
                }
                else
                {
                    model!.PalletNo = string.Empty;
                    // ステップ１へ戻る
                    await StepsExtend!.SetStep(0);

                }
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
            //if (model!.IsRireki)
            //{
            //    // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
            //    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            //    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
            //    await ShipInfoLocalStorage();
            //    NavigationManager.NavigateTo(model!.GetFirstRirekiUrl());
            //}
            //else
            //{
            //    await 前ステップへ(info);
            //}

            // 別画面から遷移した場合であっても「戻る」は前ステップに戻る
            await 前ステップへ(info);
        }

        /// <summary>
        /// HTスキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            // パレットNoの読取
            string value = scanData.strStringData;
            await OnChangeSortingPalletNo(value);
        }

        #endregion

        #region Event

        /// <summary>
        /// 倉庫の選択イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangeSortingPalletNo(object value)
        {
            model!.SortingPalletNo = (string)value;

            _ = await LoadDataAsync();
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            //VIEW名から列の情報を取得する
            _ = await LoadDataAsync();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            model!.SortingPalletNo = string.Empty;
            GetPalletLocation();
        }
        /// <summary>
        /// パレット位置情報取得処理
        /// </summary>
        private async void GetPalletLocation()
        {
            model!.AreaCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID);
            model!.ZoneCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);
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
                if (!string.IsNullOrEmpty(model!.SortingPalletNo))
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


            StateHasChanged();
            return nData;
        }

        /// <summary>
        /// パレット残在庫チェック
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetPalletZanStock(string strPalletNo)
        {
            try
            {
                bool ret = false;
                List<IDictionary<string, object>> datas = new();
                ClassNameSelect select = new()
                {
                    viewName = "VW_HT_コーナー別仕分_パレット在庫チェック",
                };
                select.whereParam.Add("パレットNo", new WhereParam { val = strPalletNo, whereType = enumWhereType.Equal });
                datas = await ComService!.GetSelectData(select);
                if (null != datas && datas.Count > 0)
                {
                    ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
                return false;
            }
        }

        #endregion
    }
}
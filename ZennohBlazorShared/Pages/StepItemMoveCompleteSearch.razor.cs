using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 切出搬送/パレットNo.読取
    /// </summary>
    public partial class StepItemMoveCompleteSearch : StepItemBase
    {
        private StepItemMoveCompleteViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "PalletNo";
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
            await 次ステップへ(info);
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
            // コーナー搬出画面に遷移
            NavigationManager.NavigateTo($"move_complete_corner");
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
                        // 出庫メニューに遷移
                        NavigationManager.NavigateTo($"mobile_ship_menu");
                    }
                    else
                    {
                        NavigationManager.NavigateTo(uri);
                    }

                }
                else
                {
                    // 出庫メニューに遷移
                    NavigationManager.NavigateTo($"mobile_ship_menu");
                }

            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
                NavigationManager.NavigateTo($"mobile_ship_menu");
            }
        }

        /// <summary>
        /// HTスキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

            if (IsPalletBarcode(value))
            {
                await OnChangePalletNo(value);

                await ContainerMainLayout.ButtonClickF1();
            }
            StateHasChanged();
        }

        #endregion

        #region event

        /// <summary>
        /// パレットNo変更イベント
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task OnChangePalletNo(object value)
        {
            model!.PalletNo = (string)value;
            await Task.Delay(0);
            StateHasChanged();

            //await LoadDataAsync();
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            _ = await LoadDataAsync();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //model!.PalletNo = string.Empty;
        }

        /// <summary>
        /// データの読込
        /// </summary>
        /// <returns></returns>
        private async Task<int> LoadDataAsync()
        {
            StepItemMoveCompleteViewModel? model = (StepItemMoveCompleteViewModel?)PageVm;
            int nData = 0;
            try
            {
                if (!string.IsNullOrEmpty(model!.PalletNo))
                {
                    nData = await LoadViewModelBind();
                }

                if (nData == 0)
                {
                    ClearData();
                }

                model!.IsMixed = model!.Mixed.Equals("1");

            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }


            StateHasChanged();
            return nData;
        }

        #endregion
    }
}
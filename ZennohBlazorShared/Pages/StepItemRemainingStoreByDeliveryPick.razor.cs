using Microsoft.AspNetCore.Components;
using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 残格納(倉庫配送先別)/残格納確定
    /// </summary>
    public partial class StepItemRemainingStoreByDeliveryPick : StepItemBase
    {
        private StepItemRemainingStoreByDeliveryViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = STR_INIT_FOCUS_MARK; ;
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemRemainingStoreByDeliveryViewModel?)PageVm;

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
            string value = scanData.strStringData;

            if (IsPalletBarcode(value))
            {
                // パレットNo
                OnChangeSPalletNo(value);
            }
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            // パレットピッキング(倉庫別)、パレットピッキング(倉庫配送先別)の残格納で摘取ピック画面へ来た時は
            // 摘取ピック後はパレット移動画面へ遷移

            // 作業化完了した場合は、パレット移動/移動先入力画面へ遷移します。
            // →パレット移動のパレットは残格納の元パレット
            // 　パレット移動で確定すると、切出搬送画面に遷移するが、その場合パレットは先パレットとする
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_SPALLETE_NO, model!.SPalletNo);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID, model!.AreaCd);　//Todo できればBaseにSetするメソッドを追加してDEFINE_COMPONENTS等に設定で対応したい
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID, model!.ZoneCd);
            // パレット移動画面に遷移
            NavigationManager.NavigateTo("move_pallet");
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
                // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
                await ShipInfoLocalStorage();
                string uri = model!.GetFirstRirekiUrl();
                if (string.IsNullOrWhiteSpace(uri))
                {
                    //残格納で履歴が取得できない場合は、強制的に出庫のメニューに戻る。TODO それでよいか
                    NavigationManager.NavigateTo("mobile_ship_menu");
                }
                else
                {
                    NavigationManager.NavigateTo(uri);
                }

            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
                NavigationManager.NavigateTo("mobile_ship_menu");
            }
        }

        #endregion

        #region Event

        /// <summary>
        /// 先パレットNo変更イベント
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private void OnChangeSPalletNo(object value)
        {
            model!.SPalletNo = (string)value;
            StateHasChanged();
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            // 混載状態を更新する
            _ = InvokeAsync(async () =>
            {
                PalletInfo info = await GetPalletInfo(model!.PalletNo);
                model!.IsMixed = info.IsMixed;
                StateHasChanged();
            });
            _ = await LoadViewModelBind();
            // グリッド情報読込
            await LoadGridData();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //model!.IsMixed = false;
            //model!.Mixed = string.Empty;
        }

        #endregion
    }
}
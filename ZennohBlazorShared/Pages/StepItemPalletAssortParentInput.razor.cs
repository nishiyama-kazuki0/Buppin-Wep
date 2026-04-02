using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット詰合せ/親パレット№読取
    /// </summary>
    public partial class StepItemPalletAssortParentInput : StepItemBase
    {
        private StepItemPalletAssortViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "PPalletNo";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemPalletAssortViewModel?)PageVm;

            // 初期処理呼び出し
            InitProc();
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
                // ﾊﾟﾚｯﾄNO
                await OnChangePalletNo(value);

                await ContainerMainLayout.ButtonClickF1();
            }
            StateHasChanged();
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
        /// 明細
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F3画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PPalletNo);
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
            await Task.Delay(0);
            // 在庫メニューへ遷移する
            NavigationManager.NavigateTo("mobile_inventory_control_menu");
        }

        #endregion

        #region Event
        /// <summary>
        /// パレットNoの入力イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangePalletNo(object value)
        {
            model!.PPalletNo = (string)value;

            await Task.Delay(0);
            StateHasChanged();
        }
        #endregion Event

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private void InitProc()
        {
            InitParam();

        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //if (!model!.NotClear)
            //{
            //    model!.PPalletNo = string.Empty;
            //}
            model!.NotClear = false;
        }

        #endregion
    }
}
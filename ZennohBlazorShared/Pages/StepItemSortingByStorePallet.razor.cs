using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 店別仕分【種まき】/パレット№読取
    /// </summary>
    public partial class StepItemSortingByStorePallet : StepItemBase
    {
        private StepItemSortingByStoreViewModel? model;

        protected IList<ValueTextInfo> dropdownCustomers { get; set; } = new List<ValueTextInfo>();

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
            model = (StepItemSortingByStoreViewModel?)PageVm;

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
        /// 戻る
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F4画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            // 店別仕分メニューに遷移
            NavigationManager.NavigateTo($"mobile_sorting_by_store_menu");
        }

        /// <summary>
        /// 確定前チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            if (0 >= await GetDataCount())
            {
                await ComService.DialogShowOK($"仕分可能なﾊﾟﾚｯﾄNo.ではありません。", pageName);
                SetElementIdFocus("PalletNo");
                return false;
            }

            return true;
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
                // ﾊﾟﾚｯﾄNO
                await OnChangePalletNo(value);
                await ContainerMainLayout.ButtonClickF1();
            }
            StateHasChanged();
        }

        /// <summary>
        /// 取引先の選択イベント
        /// </summary>
        /// <param name="value"></param>
        private void OnChangeCustomerCd(object value)
        {
            model!.CustomerCd = (string)value;
        }

        #endregion override

        #region Event

        /// <summary>
        /// パレットの選択イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangePalletNo(object value)
        {
            model!.PalletNo = (string)value;

            await Task.Delay(0);
            StateHasChanged();
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            await Task.Delay(0);

            InitParam();

            dropdownCustomers = await ComService!.GetValueTextInfo("VW_DROPDOWN_取引先コード_店別仕分_種まき");
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //model!.PalletNo = string.Empty;
        }

        #endregion private
    }
}
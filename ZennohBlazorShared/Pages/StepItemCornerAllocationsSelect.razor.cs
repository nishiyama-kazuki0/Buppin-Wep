using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// コーナー割付/コーナーパレットNo読取
    /// </summary>
    public partial class StepItemCornerAllocationsSelect : StepItemBase
    {
        private StepItemCornerAllocationsViewModel? model;

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
            model = (StepItemCornerAllocationsViewModel?)PageVm;

            // 初期処理
            await InitProcAsync();
        }

        ///// <summary>
        ///// 確定前チェック
        ///// </summary>
        ///// <param name="info"></param>
        ///// <returns></returns>
        //public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        //{
        //    return await LoadDataAsync() > 0;
        //}

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            _ = await LoadDataAsync();

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
            // 出庫メニューに遷移
            NavigationManager.NavigateTo($"mobile_ship_menu");
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
            }
        }

        #endregion

        #region Event

        /// <summary>
        /// パレットの選択イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangePalletNo(object value)
        {
            model!.PalletNo = (string)value;

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

            await Task.Delay(0);
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            model!.PalletNo = string.Empty;
            model!.Alert = "";
            model!.IsAlert = false;
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
                if (!string.IsNullOrEmpty(model!.PalletNo))
                {
                    nData = await LoadViewModelBind();
                }

                if (nData == 0)
                {
                    ClearData();
                }

                model!.IsAlert = model!.Alert.Equals("1");

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
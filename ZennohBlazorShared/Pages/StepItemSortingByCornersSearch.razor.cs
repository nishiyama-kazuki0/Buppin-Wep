using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// コーナー別仕分（切出済パレットNo.読取)
    /// </summary>
    public partial class StepItemSortingByCornersSearch : StepItemBase
    {
        private StepItemSortingByCornersViewModel? model;

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
            model = (StepItemSortingByCornersViewModel?)PageVm;

            // 初期処理呼び出し
            InitProc();
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

        /// <summary>
        /// 次ステップへ
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task 次ステップへ(ComponentProgramInfo info)
        {
            if (StepsExtend is not null)
            {
                // 入力ケース数、バラ数初期化
                // ※StepItemSortingByCornersInputのInitParamで初期化すると
                // 　StepItemSortingByCornersSaveから戻った場合も初期化されてしまうため
                // 　次ステップへで初期化
                model!.SortingCase = string.Empty;
                model!.SortingBara = string.Empty;

                await StepsExtend!.NextStep();
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
            await Task.Delay(0);
        }

        #endregion

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
            //if (!model!.IsRireki)
            //{
            //    model!.PalletNo = string.Empty;
            //}
        }

        #endregion
    }
}
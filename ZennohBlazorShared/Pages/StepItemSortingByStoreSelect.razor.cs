using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 店別仕分【種まき】/納品先選択
    /// </summary>
    public partial class StepItemSortingByStoreSelect : StepItemBase
    {
        private StepItemSortingByStoreViewModel? model;

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
            await 前ステップへ(info);
        }

        /// <summary>
        /// 確定前チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            if (_gridSelectedData is null || _gridSelectedData.Count == 0)
            {
                await ComService.DialogShowOK($"納品先が選択されていません。", pageName);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 確定前処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task 確定前処理(ComponentProgramInfo info)
        {
            await GridSelectViewModelBind();
        }

        /// <summary>
        /// スキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

            await Task.Delay(0);

            // 納品先QRｺｰﾄﾞ
            if (value.Length == SharedConst.LEN_DELIVER_CD)
            {
                foreach (IDictionary<string, object> rows in _gridData)
                {
                    if (rows["納品先ｺｰﾄﾞ"].ToString() == value)
                    {
                        _gridSelectedData = new List<IDictionary<string, object>>();
                        _gridSelectedData!.Add(rows);

                        await ContainerMainLayout.ButtonClickF1();
                    }
                }
            }
            StateHasChanged();
        }

        #endregion override

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {

            InitParam();

            if (!string.IsNullOrEmpty(model!.PalletNo) &&
                !string.IsNullOrEmpty(model!.ProductCd))
            {
                // データの読込
                if (!string.IsNullOrEmpty(model!.DeliveryCd))
                {
                    await LoadGridDataInitSel(strInitSelectKey: "納品先ｺｰﾄﾞ", strInitSelectVal: model!.DeliveryCd);
                }
                else
                {
                    await LoadGridData();
                }
            }
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //model!.DeliveryCd =
            //model!.Delivery =
            //model!.SijiCase =
            //model!.SijiBara =
            //model!.SumiCase =
            //model!.SumiBara = string.Empty;
        }

        #endregion private
    }
}
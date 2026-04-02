using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 店別仕分【種まき】/品名選択
    /// </summary>
    public partial class StepItemSortingByStoreProduct : StepItemBase
    {

        private StepItemSortingByStoreViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = STR_INIT_FOCUS_MARK;
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
            if (_gridSelectedData != null && _gridSelectedData.Count > 0)
            {
                model!.ProductCd = _gridSelectedData[0].TryGetValue("品名ｺｰﾄﾞ", out object? objProductCd) ? (string)(objProductCd ?? "") : "";
                model!.ProductName = _gridSelectedData[0].TryGetValue("品名", out object? objProductName) ? (string)(objProductName ?? "") : "";
                model!.GradeClass = _gridSelectedData[0].TryGetValue("等階級", out object? objGrade) ? (string)(objGrade ?? "") : "";
                model!.ProductAreaCd = _gridSelectedData[0].TryGetValue("産地コード", out object? objProductAreaCd) ? (string)(objProductAreaCd ?? "") : "";
                model!.ShipperCd = _gridSelectedData[0].TryGetValue("出荷者コード", out object? objShipperCd) ? (string)(objShipperCd ?? "") : "";
            }
            if (string.IsNullOrEmpty(model!.ProductCd))
            {
                await ComService.DialogShowOK($"品名が特定されていません。", pageName);
                return false;
            }
            return true;
        }

        #endregion override

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            await Task.Delay(0);

            InitParam();

            if (!string.IsNullOrEmpty(model!.PalletNo))
            {
                // データの読込
                if (!string.IsNullOrEmpty(model!.ProductCd))
                {
                    await LoadGridDataInitSel(strInitSelectKey: "品名ｺｰﾄﾞ", strInitSelectVal: model!.ProductCd);
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
            //model!.ProductCd =
            //model!.ProductName =
            //model!.GradeClass = string.Empty;
        }

        #endregion private

    }
}
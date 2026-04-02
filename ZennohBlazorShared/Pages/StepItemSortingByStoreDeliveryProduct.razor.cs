using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 店別仕分【摘取】/品名選択
    /// </summary>
    public partial class StepItemSortingByStoreDeliveryProduct : StepItemBase
    {
        private StepItemSortingByStoreDeliveryViewModel? model;

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
            model = (StepItemSortingByStoreDeliveryViewModel?)PageVm;

            // 初期処理呼び出し
            await InitProcAsync();
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
                await ComService.DialogShowOK($"品名が選択されていません。", pageName);
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

        #endregion override

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {

            InitParam();

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

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //model!.ProductName =
            //model!.ProductCd =
            //model!.GradeClass =
            //model!.InstCase =
            //model!.InstBara = string.Empty;
        }

        #endregion private

    }
}
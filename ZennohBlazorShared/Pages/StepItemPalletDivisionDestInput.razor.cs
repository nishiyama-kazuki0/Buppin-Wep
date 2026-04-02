using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット分割/先パレット№読取
    /// </summary>
    public partial class StepItemPalletDivisionDestInput : StepItemBase
    {
        private StepItemPalletDivisionViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "SPalletNo";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemPalletDivisionViewModel?)PageVm;

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
                // ﾊﾟﾚｯﾄNO
                model!.SPalletNo = scanData.strStringData;
            }
            StateHasChanged();
        }

        /// <summary>
        /// 確定前チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            _ = decimal.TryParse(model!.Case, out decimal dCase);
            _ = decimal.TryParse(model!.Bara, out decimal dBara);

            if (dCase == 0 && dBara == 0)
            {
                await ComService.DialogShowOK($"ｹｰｽ数＋ﾊﾞﾗ数は1以上を入力してください。", pageName);
                SetElementIdFocus("CaseIn");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            model!.MPalletNo = string.Empty;
            await 前ステップへ(info);
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
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.MPalletNo);
            await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_STOCK_ARRIVAL_DETAIL_NO, model!.ArrivalDetailNo);
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
            await 前ステップへ(info);
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
                PalletInfo info = await GetPalletInfo(model!.MPalletNo);
                model!.IsMixed = info.IsMixed;
                StateHasChanged();
            });
            // データの読込
            _ = await LoadViewModelBind();
        }

        /// <summary>
        /// パラメータの初期化
        /// </summary>
        /// <returns></returns>
        private void InitParam()
        {
            model!.SPalletNo = string.Empty;
            model!.STotalBara = string.Empty;
            model!.Case = string.Empty;
            model!.Bara = string.Empty;
        }

        #endregion
    }
}
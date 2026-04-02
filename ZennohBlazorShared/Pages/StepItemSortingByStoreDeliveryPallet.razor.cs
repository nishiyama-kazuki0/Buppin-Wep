using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 店別仕分【摘出】/パレットNo.読取
    /// </summary>
    public partial class StepItemSortingByStoreDeliveryPallet : StepItemBase
    {
        private StepItemSortingByStoreDeliveryViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "Delivery";
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
            await Task.Delay(0);
            if (_gridSelectedData != null && _gridSelectedData.Count > 0)
            {
                if (_gridSelectedData[0].TryGetValue("ﾊﾟﾚｯﾄNo", out object? obj))
                {
                    model!.PalletNo = (string)(obj ?? "");
                }
            }
            if (string.IsNullOrEmpty(model!.PalletNo))
            {
                await ComService.DialogShowOK($"パレットNo.が特定されていません。", pageName);
                SetElementIdFocus("Delivery");
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
        /// HTスキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

            await Task.Delay(0);

            if (_gridData.Count > 0)
            {
                bool isExists = false;
                foreach (IDictionary<string, object> rows in _gridData)
                {
                    if (rows["ﾊﾟﾚｯﾄNo"].ToString() == value)
                    {
                        _gridSelectedData = new List<IDictionary<string, object>>() { rows };
                        isExists = true;
                        break;
                    }
                }
                if (isExists)
                {
                    await ContainerMainLayout.ButtonClickF1();
                }
                else
                {
                    ShowNotifyMessege(NotificationSeverity.Error, pageName, "ﾊﾟﾚｯﾄNoが見つかりません。");
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

            // データの読込
            if (!string.IsNullOrEmpty(model!.PalletNo))
            {
                await LoadGridDataInitSel(strInitSelectKey: "ﾊﾟﾚｯﾄNo", strInitSelectVal: model!.PalletNo);
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
            //model!.PalletNo = string.Empty;
        }

        #endregion private
    }
}
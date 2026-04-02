using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 店別仕分【摘取】/納品先選択
    /// </summary>
    public partial class StepItemSortingByStoreDeliverySelect : StepItemBase
    {
        private StepItemSortingByStoreDeliveryViewModel? model;

        protected IList<ValueTextInfo> dropdownCustomers { get; set; } = new List<ValueTextInfo>();
        protected IList<ValueTextInfo> dropdownDelivery { get; set; } = new List<ValueTextInfo>();

        private List<ValueTextInfo> _deliveryAll { get; set; } = new List<ValueTextInfo>();

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "CustomerCd";
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
            if (string.IsNullOrEmpty(model!.CustomerCd))
            {
                await ComService.DialogShowOK($"取引先を選択してください。", pageName);
                SetElementIdFocus("CustomerCd");

                return false;
            }
            if (string.IsNullOrEmpty(model!.DeliveryCd))
            {
                await ComService.DialogShowOK($"納品先を選択してください。", pageName);
                SetElementIdFocus("DeliveryCd");

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
            await Task.Delay(0);
            // 店別仕分メニューに遷移
            NavigationManager.NavigateTo($"mobile_sorting_by_store_menu");
        }

        /// <summary>
        /// HTスキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

            if (value.Length == SharedConst.LEN_DELIVER_CD)
            {
                await OnChangeDelivery(value);

                if (!string.IsNullOrEmpty(model!.DeliveryCd))
                {
                    await ContainerMainLayout.ButtonClickF1();
                }
            }
            StateHasChanged();
        }
        #endregion override

        #region event

        /// <summary>
        /// 取引先の選択イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangeCustomerCd(object value)
        {
            await Task.Delay(0);
            model!.CustomerCd = (string)value;

            // 納品先を初期化
            if (string.IsNullOrEmpty(model!.CustomerCd))
            {
                dropdownDelivery.Clear();
            }
            else
            {
                dropdownDelivery = _deliveryAll.Where(_ => _.Value3 == model!.CustomerCd).ToList();
            }
            model!.DeliveryCd = string.Empty;
            model!.Delivery = string.Empty;
        }

        /// <summary>
        /// 納品先の選択イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangeDelivery(object value)
        {
            await Task.Delay(0);
            if (dropdownDelivery.Any(_ => _.Value == (string)value))
            {
                model!.DeliveryCd = (string)value;
                model!.Delivery = dropdownDelivery.Where(x => x.Value == model!.DeliveryCd).Select(_ => _.Text).FirstOrDefault() ?? "";
            }
        }

        #endregion event

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            // データの読込
            await InitDataAsync();

            // 納品先を初期化
            if (string.IsNullOrEmpty(model!.CustomerCd))
            {
                dropdownDelivery.Clear();
            }
            else
            {
                dropdownDelivery = _deliveryAll.Where(_ => _.Value3 == model!.CustomerCd).ToList();
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


        /// <summary>
        /// コンボボックスの初期化
        /// </summary>
        /// <returns></returns>
        private async Task InitDataAsync()
        {
            dropdownCustomers = await ComService!.GetValueTextInfo("VW_DROPDOWN_取引先コード_店別仕分_摘取");
            _deliveryAll = await ComService!.GetValueTextInfo("VW_DROPDOWN_納品先コード_店別仕分_摘取");
        }
        #endregion private
    }
}
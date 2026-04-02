using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット紐付
    /// </summary>
    public partial class StockupWorkPlans : ChildPageBaseMobile
    {
        /// <summary>
        /// コンポーネントが初期化されるときに呼び出されます。
        /// 子ページで全体で使用したい処理を記載
        /// </summary>
        protected override void OnInitialized()
        {
            // キーダウンイベントを受けるイベントの追加は行わない
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        protected override void Dispose()
        {
            // キーダウンイベントを受けるイベントの削除は行わない
        }

        /// <summary>
        /// 初期処理
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            StepItemStockupWorkPlansViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model.IsRireki)
            {
                if (model.LastRireki.Equals(typeof(StepItemArrivalsInspectsInput).Name))
                {
                    // 入荷検品入力
                    model.ArrivalManagementId = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_MANEGEMENT_ID);
                    model.ArrivalDetailNo = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_DETAIL_NO);
                    model.CaseIn = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_INCASE);
                    model.BaraIn = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_INBARA);
                    model.IsInitParam = true;
                    await stepsExtend?.SetStep(1)!;
                }
            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "入荷-明細No選択", StepItem = new StepItemStockupWorkPlansSelect() },
                new StepItemInfo() { Title = "ﾊﾟﾚｯﾄNo.入力", StepItem = new StepItemStockupWorkPlansInput() },
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}

using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット分割画面
    /// </summary>
    public partial class PalletDivision : ChildPageBaseMobile
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
            StepItemPalletDivisionViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model!.IsRireki)
            {
                if (model.LastRireki.Equals(typeof(StepItemPalletDivisionOrgInput).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // パレット分割/元パレットNo読取（他画面から戻ってきた）
                    model.MPalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    model.NotClear = true;
                }
                else if (model.LastRireki.Equals(typeof(StepItemPalletDivisionDestInput).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // パレット分割/先パレットNo読取（他画面から戻ってきた）
                    model.MPalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    model.ArrivalDetailNo = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_STOCK_ARRIVAL_DETAIL_NO);
                    if (!string.IsNullOrEmpty(model.MPalletNo))
                    {
                        await stepsExtend?.SetStep(1)!;
                    }
                    // 受け取ったら削除する
                    await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_STOCK_ARRIVAL_DETAIL_NO);
                }
            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "親ﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemPalletDivisionOrgInput() },
                new StepItemInfo() { Title = "子ﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemPalletDivisionDestInput() },
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}
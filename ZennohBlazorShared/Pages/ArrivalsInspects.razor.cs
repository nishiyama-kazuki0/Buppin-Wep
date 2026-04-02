using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 入荷検品画面
    /// </summary>
    public partial class ArrivalsInspects : ChildPageBaseMobile
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
            StepItemArrivalsInspectsViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model.IsRireki)
            {
                if (model.LastRireki.Equals(typeof(StepItemArrivalsInspectsInput).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // パレット紐付/パレットNo入力の確定で戻ってきた
                    model.AreaCd = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_AREA_ID);
                    model.ZoneCd = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ZONE_ID);
                    model.LocationCd = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_LOCATION_ID);
                    model.ArrivalNoSearch = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_NO);
                    model.ArrivalDetailNo = await SessionStorage.GetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_DETAIL_NO);
                    model.ArrivalDetailNoDisp = model.ArrivalDetailNo;
                    if (!string.IsNullOrEmpty(model.ArrivalDetailNo))
                    {
                        await stepsExtend?.SetStep(1)!;
                    }
                    //StepItem側で表示をモデルに代入するため、一旦ここの処理はコメント化する
                    //else if (string.IsNullOrEmpty(model.ArrivalDetailNo) && !string.IsNullOrEmpty(model.ArrivalNoSearch))
                    //{
                    //    model.ArrivalDetailNoDisp = model.ArrivalNoSearch;
                    //}
                    // 受け取ったら削除する
                    await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_DETAIL_NO);
                }
            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "入荷・明細No.読取", StepItem = new StepItemArrivalsInspectsSearch() },
                new StepItemInfo() { Title = "入荷検品入力", StepItem = new StepItemArrivalsInspectsInput() },
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}
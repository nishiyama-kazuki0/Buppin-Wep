using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット照会_店別仕分在庫
    /// </summary>
    public partial class SortingByStorePalletInventoryInquiry : ChildPageBaseMobile
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
            //TODO 整理
            StepItemSortingByStorePalletInventoryInquiryViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model.IsRireki)
            {
                if (model.LastRireki.Equals(typeof(StepItemArrivalsInspectsInput).Name) ||          // 入荷検品（ステップ２）

                    model.LastRireki.Equals(typeof(StepItemPalletDivisionOrgInput).Name) ||         // パレット分割（ステップ１・２）
                    model.LastRireki.Equals(typeof(StepItemPalletDivisionDestInput).Name) ||

                    model.LastRireki.Equals(typeof(StepItemPalletAssortParentInput).Name) ||        // パレット詰合せ（ステップ１・２）
                    model.LastRireki.Equals(typeof(StepItemPalletAssortChildInput).Name) ||

                    model.LastRireki.Equals(typeof(StepItemMovePalletInput).Name) ||                // パレット移動（ステップ２）

                    model.LastRireki.Equals(typeof(StepItemPickingPalletPick).Name) ||              // パレットピック(倉庫別)（ステップ２）
                    model.LastRireki.Equals(typeof(StepItemPickingPalletByDeliveryPick).Name) ||    // パレットピック(倉庫配送先別)（ステップ２）

                    model.LastRireki.Equals(typeof(StepItemMoveCompleteSave).Name) ||               // 切出搬送（ステップ２）

                    model.LastRireki.Equals(typeof(StepItemSortingByCornersInput).Name) ||          // コーナー別仕分（ステップ２・３）
                    model.LastRireki.Equals(typeof(StepItemSortingByCornersSave).Name) ||

                    model.LastRireki.Equals(typeof(StepItemMoveCompleteCornerSave).Name)            // コーナー搬送（ステップ２）
                    )
                {
                    model.MotoPalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    model.PalletNo = model.MotoPalletNo;
                    if (!string.IsNullOrEmpty(model.MotoPalletNo))
                    {
                        // パレットNoのみでは入荷明細Noが特定できないため、ステップ１のままとする
                    }
                }
            }
            else if (!string.IsNullOrEmpty(model.Caller)
                && (model.Caller.Equals(typeof(MobileMenu).Name)
                || model.Caller.Equals(typeof(MobileShipMenu).Name)
                || model.Caller.Equals(typeof(MobileArrivalMenu).Name)
                || model.Caller.Equals(typeof(MobileInventoryContorolMenu).Name)
                || model.Caller.Equals(typeof(MobilePickMenuItem).Name)
                || model.Caller.Equals(typeof(MobilePickMenuPallet).Name)
                || model.Caller.Equals(typeof(MobileSortingByStoreMenu).Name))
                )
            {
                //メニューから遷移の場合は履歴なし,かつパレットNoのみ取得
                string? pNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                if (!string.IsNullOrEmpty(pNo))
                {
                    model.MotoPalletNo = pNo;
                    model.PalletNo = model.MotoPalletNo;
                }
            }
            else
            {
                //他は在庫メニューボタンなどから遷移されたと考えるためストレージから取得は行わない
            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "ﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemSortingByStorePalletInventoryInquirySearch() },
                new StepItemInfo() { Title = "ﾊﾟﾚｯﾄ在庫明細", StepItem = new StepItemSortingByStorePalletInventoryInquiryDetail() },
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}

using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレットピック(倉庫配送先別)
    /// </summary>
    public partial class PickingPalletByDelivery : ChildPageBaseMobile
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

        protected override async Task OnInitializedAsync()
        {
            StepItemPickingPalletByDeliveryViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model.IsRireki)
            {
                model.DeliveryCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID);
                model.DeliveryCdTwoLineTitle = model.DeliveryCd;
                model.DeliveryName = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_NM);
                model.AreaCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID);
                model.ZoneCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);

                // 出庫作業の倉庫・ゾーン情報を保持（作業完了または、戻るボタンにて本機能に戻る際に使用）
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_SHIP_DELIVERY_ID, model.DeliveryCd);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_SHIP_AREA_ID, model.AreaCd);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_SHIP_ZONE_ID, model.ZoneCd);

                if (model.LastRireki.Equals(typeof(StepItemPickingPalletByDeliveryPick).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // パレットピッキング【倉庫配送先別】/ピック確定（他画面から戻ってきた）
                    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    await stepsExtend?.SetStep(1)!;
                }
                else if (model.LastRireki.Equals(typeof(StepItemPickingTargetSelectZone).Name))
                {
                    // パレットピッキング【倉庫配送先別】/ゾーン選択
                }
            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "ﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemPickingPalletByDeliverySearch() },
                new StepItemInfo() { Title = "ﾋﾟｯｸ確定", StepItem = new StepItemPickingPalletByDeliveryPick() }
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}
using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 倉庫別ピッキング/倉庫配送先・倉庫・ゾーン選択画面
    /// </summary>
    public partial class PickingTargetSelectByDelivery : ChildPageBaseMobile
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
            StepItemPickingTargetSelectByDeliveryViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model!.IsRireki)
            {
                if (model.LastRireki.Equals(typeof(StepItemPickingTargetSelectByDeliveryZone).Name)
                    || model.LastRireki.Equals(typeof(StepItemPickingPalletByDeliverySearch).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // パレットピッキング【倉庫配送先別】/ゾーン選択

                    model.DeliveryCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID);
                    model.AreaCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID);
                    model.ZoneCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);
                    model.DeliveryNm = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_NM);
                    model.AreaNm = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_NM);
                    model.ZoneNm = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_NM);

                    if (string.IsNullOrEmpty(model.DeliveryCd))
                    {
                        // ステップ１で倉庫配送先を選択する
                    }
                    else if (string.IsNullOrEmpty(model.AreaCd))
                    {
                        // ステップ２で倉庫を選択する
                        await stepsExtend?.SetStep(1)!;
                    }
                    else
                    {
                        // ステップ３でゾーンを選択する
                        await stepsExtend?.SetStep(2)!;
                    }
                }
            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "倉庫配送先", StepItem = new StepItemPickingTargetSelectByDeliverySelect() },
                new StepItemInfo() { Title = "倉庫選択", StepItem = new StepItemPickingTargetSelectByDeliveryArea() },
                new StepItemInfo() { Title = "ｿﾞｰﾝ選択", StepItem = new StepItemPickingTargetSelectByDeliveryZone() }
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}
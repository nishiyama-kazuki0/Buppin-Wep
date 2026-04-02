using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Shared;
using SharedModels;


namespace ExpressionDBBlazorShared.Pages
{
    /// <summary>
    /// パレット移動
    /// </summary>
    public partial class MobileItemRegistration : ChildPageBaseMobile
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
        /// 終了処理0
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
            StepItemMovePalletViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model.IsRireki)
            {
                if (model.LastRireki.Equals(typeof(StepItemDispatch).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // パレット移動/移動先入力（他画面から戻ってきた）
                    // コーナー別仕分の確定
                    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    // 最終履歴がコーナー別仕分の確定なら、確定後の画面遷移制御のフラグを立てる
                    //if (model.LastRireki.Equals(typeof(StepItemSortingByCornersSave).Name))
                    //{
                    //    model.IsCorner = true;
                    //}
                    //if (!string.IsNullOrEmpty(model.PalletNo))
                    //{
                    //    await stepsExtend?.SetStep(1)!;
                    //}
                }
                //else if (model.LastRireki.Equals(typeof(StepItemRemainingStorePick).Name) ||
                //    model.LastRireki.Equals(typeof(StepItemRemainingStoreByDeliveryPick).Name))
                //{
                //    // 残格納【倉庫別】/残格納【倉庫配送先別】の確定
                //    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                //    model.SPalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_SPALLETE_NO);
                //    model.IsZanKakuno = true;
                //    await stepsExtend?.SetStep(1)!;
                //}
                //else if (model.LastRireki.Equals(typeof(StepItemSortingByCornersSave).Name))
                //{
                //    // コーナー別仕分の確定
                //    model.PalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                //    model.IsCorner = true;
                //    await stepsExtend?.SetStep(1)!;

                //}
            }

            


            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "ﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemRegistration() },
                //new StepItemInfo() { Title = "移動先入力", StepItem = new StepItemMovePalletInput() },
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}
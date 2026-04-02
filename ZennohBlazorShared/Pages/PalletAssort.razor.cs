using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    public partial class PalletAssort : ChildPageBaseMobile
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
        protected override async Task OnInitializedAsync()
        {
            StepItemPalletAssortViewModel model = new()
            {
                Caller = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面),
                Rireki = BaseViewModel.GetRireki(await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴))
            };
            if (model!.IsRireki)
            {
                if (model.LastRireki.Equals(typeof(StepItemPalletAssortParentInput).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // パレット詰合せ/親パレットNo読取（他画面から戻ってきた）
                    model.PPalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    model.NotClear = true;
                }
                else if (model.LastRireki.Equals(typeof(StepItemPalletAssortChildInput).Name))
                {
                    model.RemoveRireki(model.LastRireki);
                    // パレット詰合せ/子パレットNo読取（他画面から戻ってきた）
                    model.PPalletNo = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO);
                    if (!string.IsNullOrEmpty(model.PPalletNo))
                    {
                        await stepsExtend?.SetStep(1)!;
                    }
                }
            }

            // StepsExtendにステップ画面を追加する
            List<StepItemInfo> list = new()
            {
                new StepItemInfo() { Title = "親ﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemPalletAssortParentInput() },
                new StepItemInfo() { Title = "子ﾊﾟﾚｯﾄNo.読取", StepItem = new StepItemPalletAssortChildInput() },
            };
            StepsExtendAttributes.Add("StepItems", list);
            StepsExtendAttributes.Add("StepItemVm", model);
        }
    }
}

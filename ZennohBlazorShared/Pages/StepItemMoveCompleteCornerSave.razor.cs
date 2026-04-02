using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;
namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// コーナー搬送/コーナー搬送完了
    /// </summary>
    public partial class StepItemMoveCompleteCornerSave : StepItemBase
    {
        private StepItemMoveCompleteCornerViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = STR_INIT_FOCUS_MARK;
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemMoveCompleteCornerViewModel?)PageVm;

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

            //TODO 確定チェック
            return true;
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            if (model!.IsRireki)
            {
                // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
                await ShipInfoLocalStorage();
                NavigationManager.NavigateTo(model!.GetFirstRirekiUrl());
            }
            else
            {
                model!.PalletNo = string.Empty;
                // メニューからコーナー搬送に来たときは前ステップへ遷移
                await 前ステップへ(info);
            }
        }

        /// <summary>
        /// 明細
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F3画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
            // パレット照会画面に遷移
            NavigationManager.NavigateTo("pallet_inventory_inquiry");
        }

        /// <summary>
        /// 戻る
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F4画面遷移(ComponentProgramInfo info)
        {
            try
            {
                if (model!.IsRireki)
                {
                    // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
                    await ShipInfoLocalStorage();
                    string uri = model!.GetFirstRirekiUrl();
                    if (string.IsNullOrWhiteSpace(uri))
                    {
                        //履歴が取得できないときは前ステップへ
                        await 前ステップへ(info);
                    }
                    else
                    {
                        NavigationManager.NavigateTo(uri);
                    }
                }
                else
                {
                    await 前ステップへ(info);
                }
            }
            catch (Exception ex)
            {
                //エラーの場合は前ステップへ
                _ = ComService.PostLogAsync($"{ex.Message}");
                await 前ステップへ(info);
            }
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            _ = await LoadViewModelBind();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
        }

        #endregion
    }
}
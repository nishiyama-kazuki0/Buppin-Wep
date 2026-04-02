using Microsoft.AspNetCore.Components;
using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレットピック(倉庫別)/ピック確定
    /// </summary>
    public partial class StepItemPickingPalletPick : StepItemBase
    {
        private StepItemPickingPalletViewModel? model;

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
            model = (StepItemPickingPalletViewModel?)PageVm;

            // 初期処理呼び出し
            await InitProcAsync();
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
            // 切出搬送画面に遷移
            NavigationManager.NavigateTo("move_complete");
        }

        /// <summary>
        /// 残格納
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F2画面遷移(ComponentProgramInfo info)
        {
            if (model!.FirstRireki == typeof(StepItemMovePalletInput).Name)
            {
                // パレット移動のピックボタンから遷移されてきている場合は残格納ボタンは非表示なので処理させない
                return;
            }
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
            await ShipInfoLocalStorage(false);
            // 残格納(倉庫別)画面に遷移
            NavigationManager.NavigateTo("remaining_store");
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
                if (model!.IsRireki && model.FirstRireki != typeof(StepItemPickingTargetSelectZone).Name)
                {
                    // 履歴は存在するがパレットピッキング【倉庫別】のゾーン選択画面から遷移されている場合は、通常の遷移なので無視する
                    // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
                    await ShipInfoLocalStorage();
                    string uri = model!.GetFirstRirekiUrl();
                    if (string.IsNullOrWhiteSpace(uri))
                    {
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
                _ = ComService.PostLogAsync(ex.Message);
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
            InitParam();

            // 混載状態を更新する
            _ = InvokeAsync(async () =>
            {
                PalletInfo info = await GetPalletInfo(model!.PalletNo);
                model!.IsMixed = info.IsMixed;
                StateHasChanged();
            });
            _ = await LoadViewModelBind();
            // グリッド情報読込
            await LoadGridData();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            if (model!.FirstRireki == typeof(StepItemMovePalletInput).Name)
            {
                // パレット移動のピックボタンから遷移されてきている場合は残格納ボタンは非表示にする
                Attributes[STR_ATTRIBUTE_FUNC]["button2text"] = "__";
                UpdateFuncButton(Attributes[STR_ATTRIBUTE_FUNC]);
            }
            model!.IsMixed = false;
            model!.Mixed = string.Empty;

            model!.Pick =
            model!.Remain = string.Empty;
        }

        #endregion
    }
}
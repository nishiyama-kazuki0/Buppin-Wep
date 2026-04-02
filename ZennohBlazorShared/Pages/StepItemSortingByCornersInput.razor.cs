using Microsoft.AspNetCore.Components;
using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// コーナー別仕分（ピック済在庫入力）
    /// </summary>
    public partial class StepItemSortingByCornersInput : StepItemBase
    {
        private StepItemSortingByCornersViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "SortingCase";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemSortingByCornersViewModel?)PageVm;

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
            _ = decimal.TryParse(model!.SortingCase, out decimal dCase);
            _ = decimal.TryParse(model!.SortingBara, out decimal dBara);

            if (dCase + dBara < 0)
            {
                await ComService.DialogShowOK($"ｹｰｽ数＋ﾊﾞﾗ数は0以上を入力してください。", pageName);
                SetElementIdFocus("SortingCase");
                return false;
            }

            if (_cardSelectedData is null || _cardSelectedData.Count == 0)
            {
                await ComService.DialogShowOK($"商品が選択されていません。", pageName);
                SetElementIdFocus("SortingCase");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 確定前処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task 確定前処理(ComponentProgramInfo info)
        {
            await ListCardViewModelBind();
        }

        /// <summary>
        /// コーナー搬送遷移
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F2画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PalletNo);
            // コーナー搬送画面に遷移
            NavigationManager.NavigateTo("move_complete_corner");
        }

        /// <summary>
        /// パレット照会遷移
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

            if (!string.IsNullOrEmpty(model!.PalletNo))
            {
                if (!string.IsNullOrEmpty(model!.ShippingAggrId))
                {
                    await LoadCardListDataInitSel(strInitSelectKey: "出荷予定集約ID", strInitSelectVal: model!.ShippingAggrId);
                }
                else
                {
                    await LoadCardListData();
                }

            }
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            model!.SortingCase = string.Empty;
            model!.SortingBara = string.Empty;
        }

        #endregion

        #region event

        /// <summary>
        /// DataCardListのPageChangedイベント
        /// </summary>
        /// <param name="args"></param>
        private void OnPageChanged(PagerEventArgs args)
        {
            try
            {
                // ページが切り替えられた際は、出庫ｹｰｽ/ﾊﾞﾗ数を入力ケースの初期値とする
                model!.SortingCase = string.Empty;
                model!.SortingBara = string.Empty;
                if (_cardSelectedData != null && _cardSelectedData.Count > 0)
                {
                    if (_cardSelectedData[0].TryGetValue("出庫\\nｹｰｽ/ﾊﾞﾗ数", out DataCardListInfo? info))
                    {
                        string[] vals = info.Value.Split('/');
                        if (vals.Length == 2)
                        {
                            model!.SortingCase = vals[0].Replace(",", "");
                            model!.SortingBara = vals[1].Replace(",", "");
                        }
                    }
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        #endregion
    }
}
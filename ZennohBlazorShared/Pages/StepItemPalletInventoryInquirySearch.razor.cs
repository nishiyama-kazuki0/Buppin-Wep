using Microsoft.AspNetCore.Components;
using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット照会/パレット№情報
    /// </summary>
    public partial class StepItemPalletInventoryInquirySearch : StepItemBase
    {
        private StepItemPalletInventoryInquiryViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "PalletNo";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemPalletInventoryInquiryViewModel?)PageVm;

            // 初期処理呼び出し
            await InitProcAsync();
        }

        /// <summary>
        /// HTスキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

            if (IsPalletBarcode(value))
            {
                // パレットNo
                model!.PalletNo = value;
                await OnChangePalletNo(value);
            }
            StateHasChanged();
        }

        /// <summary>
        /// 確定前チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            if ((_cardSelectedData == null) || (_cardSelectedData?.Count == 0))
            {
                await ComService.DialogShowOK($"入荷明細Noが選択されていません。", pageName);
                SetElementIdFocus("PalletNo");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 確定前処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override Task 確定前処理(ComponentProgramInfo info)
        {
            if (_cardSelectedData?.Count > 0)
            {
                if (_cardSelectedData[0].TryGetValue("入荷明細No", out DataCardListInfo? sel))
                {
                    model!.ArrivalDetailNo = sel.Value;
                }
                if (_cardSelectedData[0].TryGetValue("倉庫配送先ｺｰﾄﾞ", out sel))
                {
                    model!.DeliveryCd = sel.Value;
                }
                if (_cardSelectedData[0].TryGetValue("CARD_LIST_KEY", out sel))
                {
                    model!.CardListKey = sel.Value;
                }
            }

            return base.確定前処理(info);
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            await 次ステップへ(info);
        }

        /// <summary>
        /// 戻る
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F4画面遷移(ComponentProgramInfo info)
        {
            // 遷移履歴が存在すると元画面へ戻る
            if (model!.IsRireki)
            {
                string url = model.GetLastRirekiUrl();
                if (!string.IsNullOrEmpty(url))
                {
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrBackRireki());
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.MotoPalletNo);
                    await ShipInfoLocalStorage();
                    NavigationManager.NavigateTo(url);
                    return;
                }
            }
            // 在庫メニューへ遷移する
            NavigationManager.NavigateTo("mobile_inventory_control_menu");
        }

        #endregion

        #region Event

        /// <summary>
        /// 入荷-明細No.の変更処理
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangePalletNo(string value)
        {
            // 混載状態を更新する
            _ = InvokeAsync(async () =>
            {
                PalletInfo info = await GetPalletInfo(value);
                model!.IsMixed = info.IsMixed;
                StateHasChanged();
            });
            // データ取得
            if (string.IsNullOrEmpty(value))
            {
                ClearData();

                _cardSelectedData = null;
                _cardValuesList = new List<IDictionary<string, DataCardListInfo>>();
            }
            else
            {
                _ = await LoadViewModelBind();
                await LoadCardListData();
                //変更後初期表示位置に再スクロールを行うように。
                ScrollPageFirst();
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

            // パレットNoが指定された
            if (!string.IsNullOrEmpty(model!.PalletNo))
            {
                // 混載状態を更新する
                _ = InvokeAsync(async () =>
                {
                    PalletInfo info = await GetPalletInfo(model!.PalletNo);
                    model!.IsMixed = info.IsMixed;
                    StateHasChanged();
                });
                if (!string.IsNullOrEmpty(model!.CardListKey))
                {
                    await LoadCardListDataInitSel(strInitSelectKey: "CARD_LIST_KEY", strInitSelectVal: model!.CardListKey);
                }
                else
                {
                    await LoadCardListData();
                }

                _ = await LoadViewModelBind();
                //変更後初期表示位置に再スクロールを行うように。
                ScrollPageFirst();
            }
        }

        /// <summary>
        /// パラメータの初期化
        /// </summary>
        /// <returns></returns>
        private void InitParam()
        {
            //if (!model!.IsRireki)
            //{
            //    model!.PalletNo = string.Empty;
            //}
            //model!.Area =
            //model!.Zone =
            //model.Location =
            //model!.Status =
            //model!.AllocCorner = string.Empty;
        }

        #endregion
    }
}
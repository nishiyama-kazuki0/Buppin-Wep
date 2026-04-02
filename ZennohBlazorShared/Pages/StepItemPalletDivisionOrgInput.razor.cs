using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット分割/元パレット№読取"
    /// </summary>
    public partial class StepItemPalletDivisionOrgInput : StepItemBase
    {
        private StepItemPalletDivisionViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "MPalletNo";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemPalletDivisionViewModel?)PageVm;

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
                // ﾊﾟﾚｯﾄNO
                model!.MPalletNo = scanData.strStringData;
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
                SetElementIdFocus("MPalletNo");
                return false;
            }
            else if (_cardSelectedData?.Count > 0)
            {
                // 表示中在庫情報のパレットNoと異なっている場合は、カードを更新してエラーとする
                if (_cardSelectedData[0].TryGetValue("パレットNo", out DataCardListInfo? sel))
                {
                    if (model!.MPalletNo != sel.Value)
                    {
                        await LoadCardListData();
                        await ComService.DialogShowOK($"入荷明細Noが選択されていません。", pageName);
                        SetElementIdFocus("MPalletNo");
                        return false;
                    }
                }
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
                if (_cardSelectedData[0].TryGetValue("入荷No", out sel))
                {
                    model!.ArrivalNo = sel.Value;
                }
                if (_cardSelectedData[0].TryGetValue("明細No", out sel))
                {
                    model!.DetailNo = sel.Value;
                }
                if (_cardSelectedData[0].TryGetValue("パレットNo", out sel))
                {
                    model!.PalletNo = sel.Value;
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
        /// 明細
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F3画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.MPalletNo);
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
            await Task.Delay(0);
            // 在庫メニューへ遷移する
            NavigationManager.NavigateTo("mobile_inventory_control_menu");
        }

        #endregion

        #region Event

        /// <summary>
        /// パレットNoの変更処理
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangePalletNo(object value)
        {
            //if (!IsPalletBarcode(value.ToString()!))
            //{
            //    return;
            //}

            await LoadCardListData();
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            if (!string.IsNullOrEmpty(model!.MPalletNo))
            {
                if (!string.IsNullOrEmpty(model!.ArrivalDetailNo))
                {
                    await LoadCardListDataInitSel(strInitSelectKey: "入荷明細No", strInitSelectVal: model!.ArrivalDetailNo);
                }
                else
                {
                    await LoadCardListData();
                }
            }
        }

        /// <summary>
        /// パラメータの初期化
        /// </summary>
        /// <returns></returns>
        private void InitParam()
        {
            //if (!model!.NotClear)
            //{
            //    model!.MPalletNo = string.Empty;
            //}
            model!.NotClear = false;
        }

        #endregion
    }
}

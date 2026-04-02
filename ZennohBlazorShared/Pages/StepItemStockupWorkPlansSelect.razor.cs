using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット紐付/入荷-明細No.選択
    /// </summary>
    public partial class StepItemStockupWorkPlansSelect : StepItemBase
    {
        private StepItemStockupWorkPlansViewModel? model;
        #region 表示データプロパティー

        protected IList<ValueTextInfo> dropdownArrivalNo { get; set; } = new List<ValueTextInfo>();

        #endregion 表示データプロパティー

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "ArrivalDetailNo";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemStockupWorkPlansViewModel?)PageVm;

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

            if (value.Length == SharedConst.LEN_NYUKA_MEISAI_NO)
            {
                // 入荷明細NO
                model!.ArrivalDetailNo = value;
                await OnChangeArrivalNo(value);
                // 一件だけ取得出来ている場合は確定処理を実行する
                if (_gridData.Count == 1)
                {
                    await ContainerMainLayout.ButtonClickF1();
                }
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
            if (_cardSelectedData is null || _cardSelectedData.Count == 0)
            {
                await ComService.DialogShowOK($"入荷検品実績が選択されていません。", pageName);
                SetElementIdFocus("ArrivalDetailNo");
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
                if (_cardSelectedData[0].TryGetValue("入荷検品管理ID", out DataCardListInfo? sel))
                {
                    model!.ArrivalManagementId = sel.Value;
                }
                if (_cardSelectedData[0].TryGetValue("入荷No", out DataCardListInfo? sel2))
                {
                    model!.ArrivalNo = sel2.Value;
                }
                if (_cardSelectedData[0].TryGetValue("明細No", out DataCardListInfo? sel3))
                {
                    model!.DetailNo = sel3.Value;
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
            await Task.Delay(0);
            // 入庫メニューに遷移
            NavigationManager.NavigateTo($"mobile_arrival_menu");
        }

        #endregion

        #region Event

        /// <summary>
        /// 入荷-明細No.の変更処理
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangeArrivalNo(object value)
        {
            if (value.ToString()!.Length < SharedConst.LEN_NYUKA_MEISAI_NO)
            {
                //_cardValuesList?.Clear();
                //_cardSelectedData = null;
                //StateHasChanged();
                return;
            }

            //入荷-明細Noのスキャン、かつ読み取り入荷-明細Noの入荷検品実績が存在する場合は、③を実行します。

            // 入荷明細No桁数になったらデータを取得する
            await LoadCardListData();
            //if (_cardValuesList!.Count > 0)
            //{
            //}
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            // 入荷-明細No.コンボボックスの初期化
            await InitComboArrivalNo();

            if (!string.IsNullOrEmpty(model.ArrivalDetailNo))
            {
                // 入荷明細Noが保持されている場合、データを取得する
                if (!string.IsNullOrEmpty(model!.ArrivalManagementId))
                {
                    await LoadCardListDataInitSel(strInitSelectKey: "入荷検品管理ID", strInitSelectVal: model!.ArrivalManagementId);
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
            //model!.ArrivalDetailNo = string.Empty;
        }

        /// <summary>
        /// 入荷-明細Noコンボボックスの初期化
        /// </summary>
        /// <returns></returns>
        private async Task InitComboArrivalNo()
        {
            dropdownArrivalNo.Clear();
            List<ValueTextInfo> lst = await ComService.GetValueTextInfo("VW_DROPDOWN_入荷明細No");
            foreach (ValueTextInfo item in lst)
            {
                dropdownArrivalNo.Add(item);
            }
        }

        #endregion
    }
}

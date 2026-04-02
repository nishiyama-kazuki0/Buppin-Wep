using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレットピッキング【倉庫配送先別】/倉庫配送先選択
    /// </summary>
    public partial class StepItemPickingTargetSelectByDeliverySelect : StepItemBase
    {
        private StepItemPickingTargetSelectByDeliveryViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "DeliveryCd";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemPickingTargetSelectByDeliveryViewModel?)PageVm;

            // 初期処理呼び出し
            await InitProcAsync();
        }

        /// <summary>
        /// 選択行チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 選択行チェック(ComponentProgramInfo info)
        {
            // チェック
            if (_gridSelectedData == null || _gridSelectedData?.Count() == 0)
            {
                // メッセージ取得
                string msg = "倉庫配送先が選択されていません。";
                if (Attributes.ContainsKey(info.ComponentName))
                {
                    if (Attributes[info.ComponentName].TryGetValue("MessageContent", out object? value))
                    {
                        msg = value.ToString()!;
                    }
                }
                // ダイアログ表示
                await ComService.DialogShowOK($"{msg}", pageName);
                SetElementIdFocus("DeliveryCd");
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
            if (_gridSelectedData != null && _gridSelectedData.Count > 0)
            {
                if (_gridSelectedData[0].TryGetValue("倉庫配送先ｺｰﾄﾞ", out object? objDeliveryCd) &&
                    _gridSelectedData[0].TryGetValue("倉庫配送先名", out object? objDeliveryNm))
                {
                    model!.DeliveryCd = (string)(objDeliveryCd ?? "");
                    model!.DeliveryNm = (string)(objDeliveryNm ?? "");

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
            // 出庫メニュー/ピッキング【倉庫配送先別】に遷移
            NavigationManager.NavigateTo("mobile_pick_menu_item");
        }

        #endregion

        #region Event

        /// <summary>
        /// OnCellRenderのCallBack
        /// </summary>
        /// <param name="args"></param>
        private void CellRender(DataGridCellRenderEventArgs<IDictionary<string, object>> args)
        {
            try
            {
                // 取得した列情報の１列目のタイトルと一致するセルの背景色を変える
                if (_gridColumns.Count > 0 && _gridColumns[0].Title == args.Column.Title)
                {
                    if (args.Data.TryGetValue("緊急出荷区分", out object value))
                    {
                        if ("1" == value.ToString())
                        {
                            args.Attributes.Add("style", $"background-color: {_sysParams.EmergencyColor};");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// パレットNo変更イベント
        /// </summary>
        /// <param name="value"></param>
        private async void OnChangeDeliveryCd(string value)
        {
            model!.SearchDeliveryCd = value;
            // データの読込
            await LoadGridData();
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            // データの読込
            if (!string.IsNullOrEmpty(model!.DeliveryCd))
            {
                await LoadGridDataInitSel(strInitSelectKey: "倉庫配送先ｺｰﾄﾞ", strInitSelectVal: model!.DeliveryCd);
            }
            else
            {
                await LoadGridData();
            }
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //_gridSelectedData = null;
            //model!.DeliveryNm =
            //model!.DeliveryCd = string.Empty;
        }

        #endregion
    }
}
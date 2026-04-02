using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 摘取ピッキング【倉庫配送先別】/倉庫選択
    /// </summary>
    public partial class StepItemPickingTargetSelectItemByDeliveryArea : StepItemBase
    {
        private StepItemPickingTargetSelectItemByDeliveryViewModel? model;

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
            model = (StepItemPickingTargetSelectItemByDeliveryViewModel?)PageVm;

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
                string msg = "倉庫が選択されていません。";
                if (Attributes.ContainsKey(info.ComponentName))
                {
                    if (Attributes[info.ComponentName].TryGetValue("MessageContent", out object? value))
                    {
                        msg = value.ToString()!;
                    }
                }
                // ダイアログ表示
                await ComService.DialogShowOK($"{msg}", pageName);
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
                if (_gridSelectedData[0].TryGetValue("倉庫ｺｰﾄﾞ", out object? objAreaCd) &&
                    _gridSelectedData[0].TryGetValue("倉庫名", out object? objAreaNm))
                {
                    model!.AreaCd = (string)(objAreaCd ?? "");
                    model!.AreaNm = (string)(objAreaNm ?? "");
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
            await 前ステップへ(info);
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
            if (!string.IsNullOrEmpty(model!.AreaCd))
            {
                await LoadGridDataInitSel(strInitSelectKey: "倉庫ｺｰﾄﾞ", strInitSelectVal: model!.AreaCd);
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
            //model!.AreaCd =
            //model!.AreaNm = string.Empty;
        }

        #endregion
    }
}
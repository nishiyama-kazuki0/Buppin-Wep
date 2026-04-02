using Microsoft.AspNetCore.Components;
using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 摘取ピッキング【倉庫配送先別】/ゾーン選択
    /// </summary>
    public partial class StepItemPickingTargetSelectItemByDeliveryZone : StepItemBase
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
                string msg = "ｿﾞｰﾝが選択されていません。";
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
                if (_gridSelectedData[0].TryGetValue("ｿﾞｰﾝｺｰﾄﾞ", out object objZoneCd) &&
                    _gridSelectedData[0].TryGetValue("ｿﾞｰﾝ名", out object objZoneNm))
                {
                    model!.ZoneCd = (string)(objZoneCd ?? "");
                    model!.ZoneNm = (string)(objZoneNm ?? "");
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
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID, model!.DeliveryCd);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID, model!.AreaCd);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID, model!.ZoneCd);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_NM, model!.DeliveryNm);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_NM, model!.AreaNm);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_NM, model!.ZoneNm);
            // 摘取ピッキング（倉庫配送先別）画面に遷移
            NavigationManager.NavigateTo("picking_item_by_delivery");
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
            if (!string.IsNullOrEmpty(model!.ZoneCd))
            {
                await LoadGridDataInitSel(strInitSelectKey: "ｿﾞｰﾝｺｰﾄﾞ", strInitSelectVal: model!.ZoneCd);
            }
            else
            {
                await LoadGridData();
            }
            if (_gridData is null || _gridData.Count <= 0)
            {
                //ピッキング用のﾃﾞｰﾀが取得できない場合はエラーメッセージを表示する。
                //TODO メッセージ内容はべた書きではなく、できればDBから取得するようにする。
                ShowNotifyMessege(NotificationSeverity.Error, pageName, "表示ﾃﾞｰﾀの取得に失敗しました。");
            }
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //_gridSelectedData = null;
            //model!.ZoneCd =
            //model!.ZoneNm = string.Empty;
        }

        #endregion
    }
}
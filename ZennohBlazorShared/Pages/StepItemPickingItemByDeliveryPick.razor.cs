using Microsoft.AspNetCore.Components;
using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 摘取ピック(倉庫配送先別)/ピック数入力
    /// </summary>
    public partial class StepItemPickingItemByDeliveryPick : StepItemBase
    {
        private StepItemPickingItemByDeliveryViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "InCase";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemPickingItemByDeliveryViewModel?)PageVm;

            // 初期処理呼び出し
            await InitProcAsync();

            // StateHasChangedをコールしてからフォーカス制御しないと、フォーカス制御の結果は反映されない
            ChildBaseService.BasePageInitilizing = false;
            StateHasChanged();
            SetElementIdReFocus(FirstFocusId);
        }

        /// <summary>
        /// 確定前チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            _ = decimal.TryParse(model!.InCase, out decimal dCase);
            _ = decimal.TryParse(model!.InBara, out decimal dBara);

            if (dCase + dBara < 0)
            {
                await ComService.DialogShowOK($"ｹｰｽ数＋ﾊﾞﾗ数は0以上を入力してください。", pageName);
                SetElementIdFocus("InCase");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            (int, string) ret = await GetHikiateZumiZaikoDelivery(model!.MPalletNo, model!.DeliveryCd);
            if (ret.Item1 > 0)
            {
                // 作業が残っている場合は、ステップ２へ戻る
                await 前ステップへ(info);
            }
            else
            {
                //// 摘取ピック後は切出搬送に遷移する
                //await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                //await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
                //await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.SPalletNo);
                //// 切出搬送画面に遷移;
                //NavigationManager.NavigateTo("move_complete");

                //切り出し搬送への画面遷移は、切り出しボタンで行うので、ここで画面遷移する必要はない
                //またピック予定がなくなっても、切り出し搬送ボタンを押下したいので、それ以上戻らないとする
                // パレットNo読取ステップへ
                model.MPalletNo = string.Empty;
                if (StepsExtend is not null)
                {
                    await StepsExtend!.SetStep(0);
                }
            }
        }

        /// <summary>
        /// 切出
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F2画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.SPalletNo);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_NM, model!.DeliveryName);
            // 切出搬送画面に遷移
            NavigationManager.NavigateTo("move_complete");
        }

        /// <summary>
        /// コーナー(コーナー搬送へ遷移)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F3画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.SPalletNo);
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_NM, model!.DeliveryName);
            // コーナー搬送画面に遷移
            NavigationManager.NavigateTo("move_complete_corner");
        }

        /// <summary>
        /// 戻る
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F4画面遷移(ComponentProgramInfo info)
        {
            if (model!.IsRireki && model.FirstRireki != typeof(StepItemPickingTargetSelectItemByDeliveryZone).Name)
            {
                // 履歴は存在するがパレットピッキング【倉庫配送先別】のゾーン選択画面から遷移されている場合は、通常の遷移なので無視する
                // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
                await ShipInfoLocalStorage();
                string uri = model!.GetFirstRirekiUrl();
                if (string.IsNullOrWhiteSpace(uri))
                {
                    if (StepsExtend is not null)
                    {
                        await StepsExtend!.SetStep(0);
                    }
                }
                else
                {
                    NavigationManager.NavigateTo(uri);
                }

            }
            else
            {
                (int, string) ret = await GetHikiateZumiZaikoDelivery(model!.MPalletNo, model!.DeliveryCd);
                if (ret.Item1 > 1)//引当て済みの在庫が存在する指示のカウントが1の場合（引当済みの指示がある場合）
                {
                    //摘み取りピック対象を選択するため前STEPに
                    await 前ステップへ(info);
                }
                else
                {
                    //それ以外はパレットNo読取ステップへ遷移。
                    //model.MPalletNo = string.Empty;
                    if (StepsExtend is not null)
                    {
                        await StepsExtend!.SetStep(0);
                    }
                }
            }
        }

        /// <summary>
        /// スキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

            if (IsPalletBarcode(value))
            {
                await OnChangePalletNo(value);
            }
            StateHasChanged();
        }

        #endregion

        #region event
        /// <summary>
        /// パレットNo変更イベント
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private async Task OnChangePalletNo(object value)
        {
            await Task.Delay(0);

            model!.SPalletNo = (string)value;

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
                PalletInfo info = await GetPalletInfo(model!.MPalletNo);
                model!.IsMixed = info.IsMixed;
                StateHasChanged();
            });
            // データの読込
            _ = await LoadViewModelBind();
            //指示値を入力ボックスに初期表示するため
            model!.InCase = model!.SijiCase.Replace(",", "");
            model!.InBara = model!.SijiBara.Replace(",", "");

            //読み込み後にスクロールするとする
            ScrollPageFirst();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            model!.SPalletNo = string.Empty;

        }

        #endregion
    }
}
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 店別仕分【摘取】/仕分入力
    /// </summary>
    public partial class StepItemSortingByStoreDeliverySave : StepItemBase
    {
        private StepItemSortingByStoreDeliveryViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "Case";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemSortingByStoreDeliveryViewModel?)PageVm;

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
            _ = decimal.TryParse(model!.Case, out decimal dCase);
            _ = decimal.TryParse(model!.Bara, out decimal dBara);

            if (dCase + dBara < 0)
            {
                await ComService.DialogShowOK($"ｹｰｽ数＋ﾊﾞﾗ数は0以上を入力してください。", pageName);
                SetElementIdFocus("Case");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 確定前チェック
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task 確定前処理(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            // この画面はグリッドを表示していないがグリッドデータ取得処理を利用してコーナー搬送管理IDを保持しています
            // 全てを送信するためグリッド全てを選択状態にする
            _gridSelectedData = _gridData;
        }

        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {
            // 入力した納品先内に未仕分品名が存在する場合
            ClassNameSelect select = new()
            {
                viewName = "VW_HT_店別仕分摘取_未仕分チェック",
            };
            select.whereParam.Add("取引先コード", new WhereParam { val = model!.CustomerCd, whereType = enumWhereType.Equal });
            select.whereParam.Add("納品先コード", new WhereParam { val = model!.DeliveryCd, whereType = enumWhereType.Equal });
            List<IDictionary<string, object>> datas = await ComService!.GetSelectData(select);
            if (null != datas && datas.Count > 0)
            {
                // パレット選択画面へ
                if (StepsExtend is not null)
                {
                    model!.PalletNo =
                    model!.ProductCd =
                    model!.ProductName =
                    model!.ProductAreaCd =
                    model!.ShipperCd =
                    model!.GradeClass = string.Empty;

                    model!.CaseBara =
                    model!.SortingCaseBara =
                    model!.InstCase =
                    model!.InstBara =
                    model!.SumiCase =
                    model!.SumiBara =
                    model!.StoreSortingID = string.Empty;

                    model!.Case =
                    model!.Bara = string.Empty;

                    await StepsExtend!.SetStep(1);
                }
            }
            else
            {
                // 納品先選択画面へ遷移
                if (StepsExtend is not null)
                {
                    model!.CustomerCd =
                    model!.DeliveryCd =
                    model!.Delivery = string.Empty;

                    model!.PalletNo =
                    model!.ProductCd =
                    model!.ProductName =
                    model!.ProductAreaCd =
                    model!.ShipperCd =
                    model!.GradeClass = string.Empty;

                    model!.CaseBara =
                    model!.SortingCaseBara =
                    model!.InstCase =
                    model!.InstBara =
                    model!.SumiCase =
                    model!.SumiBara =
                    model!.StoreSortingID = string.Empty;

                    model!.Case =
                    model!.Bara = string.Empty;

                    await StepsExtend!.SetStep(0);
                }
            }
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

        #endregion override

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            // データの読込
            await LoadGridData();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            int nInstCase = 0;
            int nInstBara = 0;
            int nSumiCase = 0;
            int nSumiBara = 0;
            int.TryParse(model!.InstCase.Replace(",", ""), out nInstCase);
            int.TryParse(model!.InstBara.Replace(",", ""), out nInstBara);
            int.TryParse(model!.SumiCase.Replace(",", ""), out nSumiCase);
            int.TryParse(model!.SumiBara.Replace(",", ""), out nSumiBara);

            model!.Case = (nInstCase - nSumiCase).ToString();
            model!.Bara = (nInstBara - nSumiBara).ToString();
        }

        #endregion private

    }
}
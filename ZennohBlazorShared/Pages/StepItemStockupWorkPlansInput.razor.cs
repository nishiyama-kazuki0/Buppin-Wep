using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット紐付/パレット№入力
    /// </summary>
    public partial class StepItemStockupWorkPlansInput : StepItemBase
    {
        private StepItemStockupWorkPlansViewModel? model;

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
            model = (StepItemStockupWorkPlansViewModel?)PageVm;

            // ケース/バラ数取得
            (int, int) ret = await GetLinkZan(model!.ArrivalDetailNo, model!.ArrivalManagementId);
            model.CaseIn = ret.Item1 == 0 ? string.Empty : ret.Item1.ToString();
            model.BaraIn = ret.Item2 == 0 ? string.Empty : ret.Item2.ToString();

            // 初期処理呼び出し
            await InitProcAsync();
        }

        /// <summary>
        /// スキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

            //パレット紐付の場合は、QRのみに絞るようにする。入荷明細Noを読み間違える恐れがあるため。
            if (IsPalletBarcode(value) && scanData.strCodeType.Equals("QRCode")) //TODO できればべた文字の対応ではなくscanDataクラスに文字列を持たせて判断させるべき。
            {
                // パレットNo
                model!.PalletNo = value;
                await OnChangePalletNo(value);
                SetElementIdFocus("CaseIn");
            }
            else
            {
                ShowNotifyMessege(NotificationSeverity.Error, pageName, "不正なﾊﾟﾚｯﾄNo,ｺｰﾄﾞの読み込みです。");
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
            _ = decimal.TryParse(model!.CaseIn, out decimal dCase);
            _ = decimal.TryParse(model!.BaraIn, out decimal dBara);

            if (dCase == 0 && dBara == 0)
            {
                await ComService.DialogShowOK($"ｹｰｽ数＋ﾊﾞﾗ数は1以上を入力してください。", pageName);
                SetElementIdFocus("CaseIn");
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
            // 紐付残があるかチェック
            (int, int) ret = await GetLinkZan(model!.ArrivalDetailNo, model!.ArrivalManagementId);
            if (ret.Item1 == 0 && ret.Item2 == 0)
            {
                if (model!.IsRireki)
                {
                    // 遷移初めの機能に遷移 遷移履歴情報は初めの画面のみにクリア
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                    await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrFirstRireki());
                    await ShipInfoLocalStorage();

                    // 入荷明細Noの残数が無くなった場合はストレージに保持している入荷明細Noをクリアする
                    // クリアすることでArrivalsInspects.csで入荷明細Noが連携されたらステップ２に移行する処理を行わなくする
                    if (false == await GetArrivalDetailZan(model!.ArrivalDetailNo))
                    {
                        await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_DETAIL_NO);
                    }

                    NavigationManager.NavigateTo(model!.GetFirstRirekiUrl());
                }
                else
                {
                    model!.ArrivalDetailNo = string.Empty;
                    // メニューからコーナー搬送に来たときは前ステップへ遷移
                    await 前ステップへ(info);
                }
            }
            else
            {
                model.CaseIn = ret.Item1 == 0 ? string.Empty : ret.Item1.ToString();
                model.BaraIn = ret.Item2 == 0 ? string.Empty : ret.Item2.ToString();

                // 初期処理呼び出し
                await InitProcAsync();

                // 初期フォーカスに合わせる
                SetElementIdFocus(FirstFocusId);
            }
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

                    // 入荷明細Noの残数が無くなった場合はストレージに保持している入荷明細Noをクリアする
                    // クリアすることでArrivalsInspects.csで入荷明細Noが連携されたらステップ２に移行する処理を行わなくする
                    if (false == await GetArrivalDetailZan(model!.ArrivalDetailNo))
                    {
                        await SessionStorage.RemoveItemAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_DETAIL_NO);
                    }
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
                    // メニューからコーナー搬送に来たときは前ステップへ遷移
                    await 前ステップへ(info);
                }
            }
            catch (Exception ex)
            {
                //何かエラーが発生した場合は強制的に前ステップとする
                _ = ComService.PostLogAsync($"{ex.Message}");
                await 前ステップへ(info);
            }
        }

        #endregion

        #region Event

        /// <summary>
        /// パレットNo変更イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangePalletNo(string value)
        {
            await Task.Delay(0);//警告の抑制
            // 混載状態を更新する
            _ = InvokeAsync(async () =>
            {
                PalletInfo info = await GetPalletInfo(value);
                model!.IsMixed = info.IsMixed;
                StateHasChanged();
            });
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
            _ = await LoadViewModelBind();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            // 入力項目を初期化
            model!.PalletNo = "";
        }

        /// <summary>
        /// 摘取ピック紐付残数を取得
        /// </summary>
        /// <param name="strArrivalDetailNo"></param>
        /// <param name="strArrivalManagementId"></param>
        /// <returns></returns>
        private async Task<(int, int)> GetLinkZan(string strArrivalDetailNo, string strArrivalManagementId)
        {
            try
            {
                int intCase = 0;
                int intBara = 0;
                List<IDictionary<string, object>> datas = new();
                ClassNameSelect select = new()
                {
                    viewName = "VW_HT_パレット紐付_紐付残チェック",
                };
                select.whereParam.Add("入荷明細No", new WhereParam { val = strArrivalDetailNo, whereType = enumWhereType.Equal });
                select.whereParam.Add("入荷検品管理ID", new WhereParam { val = strArrivalManagementId, whereType = enumWhereType.Equal });
                datas = await ComService!.GetSelectData(select);
                if (null != datas && datas.Count > 0)
                {
                    IDictionary<string, object> dic = datas.First();
                    intCase = ComService.GetValueInt(dic, "ｹｰｽ残数");
                    intBara = ComService.GetValueInt(dic, "ﾊﾞﾗ残数");
                }

                return (intCase, intBara);
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
                return (0, 0);
            }
        }

        /// <summary>
        /// 摘取ピック引当済み在庫チェック
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetArrivalDetailZan(string strArrivalDetailNo)
        {
            try
            {
                bool ret = false;
                List<IDictionary<string, object>> datas = new();
                ClassNameSelect select = new()
                {
                    viewName = "VW_HT_パレット紐付_残チェック",
                };
                select.whereParam.Add("入荷明細No", new WhereParam { val = strArrivalDetailNo, whereType = enumWhereType.Equal });
                datas = await ComService!.GetSelectData(select);
                if (null != datas && datas.Count > 0)
                {
                    ret = true;
                }
                return ret;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
                return false;
            }
        }

        #endregion
    }
}

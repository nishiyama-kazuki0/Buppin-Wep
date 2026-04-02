using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 入荷検品 入荷検品入力ステップ
    /// </summary>
    public partial class StepItemArrivalsInspectsInput : StepItemBase
    {
        private StepItemArrivalsInspectsViewModel? model;

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
            model = (StepItemArrivalsInspectsViewModel?)PageVm;

            // 初期処理呼び出し
            await InitProcAsync();
        }

        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            _ = decimal.TryParse(model!.Case, out decimal dCase);
            _ = decimal.TryParse(model!.Bara, out decimal dBara);

            if (dCase == 0 && dBara == 0)
            {
                await ComService.DialogShowOK($"ｹｰｽ数＋ﾊﾞﾗ数は1以上を入力してください。", pageName);
                SetElementIdFocus("Case");
                return false;
            }

            return true;
        }

        public override async Task 確定前処理(ComponentProgramInfo info)
        {
            try
            {
                // 入荷検品管理IDを取得する
                string id = await ComService.GetManagementId(ClassName, SharedConst.workCategory.NyukaKenpin);
                //通信環境が悪いと上記idは空白になることがあるためチェックを行う
                if (string.IsNullOrWhiteSpace(id))
                {
                    ShowNotifyMessege(NotificationSeverity.Error, pageName, "入荷検品実績の登録に失敗しました。再実行ください。。");
                    throw new Exception("入荷検品実績の登録に失敗しました。");// boolで返すメソッドではないのでthrowする
                }
                model!.ArrivalManagementId = id;
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
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
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_MANEGEMENT_ID, model!.ArrivalManagementId);
            await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_AREA_ID, model!.AreaCd);
            await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ZONE_ID, model!.ZoneCd);
            await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_LOCATION_ID, model!.LocationCd);
            await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_DETAIL_NO, model!.ArrivalDetailNo);
            await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_ARRIVAL_NO, model!.ArrivalNoSearch);//検索用入荷Noを保持
            await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_INCASE, model!.Case);
            await SessionStorage.SetItemAsStringAsync(SharedConst.STR_SESSIONSTORAGE_ARRIVAL_INBARA, model!.Bara);
            // パレット紐付画面に遷移
            NavigationManager.NavigateTo("stockup_work_plans");
        }

        /// <summary>
        /// 明細
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F3画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            // 明細部分を表示する
            model!.IsDetail = !model!.IsDetail;
            StateHasChanged();
            SetElementIdFocus("Case");
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

        /// <summary>
        /// ブザー再生
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task ブザー再生(ComponentProgramInfo info)
        {
            int tone = 1;
            int onPeriod = 1;
            int offPeriod = 1;
            int repeatCount = 1;
            SystemParameter sysParams = await SessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
            if (sysParams is not null)
            {
                tone = sysParams.HT_DefaultBuzzerTone;
                onPeriod = sysParams.HT_DefaultBuzzerOnPeriod;
                offPeriod = sysParams.HT_DefaultBuzzerOffPeriod;
                repeatCount = sysParams.HT_DefaultBuzzerReratCount;
            }

            Dictionary<string, object> attr = new(GetAttributes(info.ComponentName));
            if (attr.TryGetValue("Tone", out object? value))
            {
                tone = ComService.ConvertInt(value.ToString()!);
            }
            if (attr.TryGetValue("OnPeriod", out value))
            {
                onPeriod = ComService.ConvertInt(value.ToString()!);
            }
            if (attr.TryGetValue("OffPeriod", out value))
            {
                offPeriod = ComService.ConvertInt(value.ToString()!);
            }
            if (attr.TryGetValue("RepeatCount", out value))
            {
                repeatCount = ComService.ConvertInt(value.ToString()!);
            }

            // 入荷No、受付Noが一致する情報が無い場合は、入荷票で検品している入荷Noが終了したこととして入荷No単位の作業完了音を鳴らす
            if (!await GetArrivalReceptZan(model!.ArrivalNo, model!.ReceptNo))
            {
                attr = new(GetAttributes(STR_ARRIVAL_COMP_BUZZER_INFO));
                if (attr.TryGetValue("Tone", out value))
                {
                    tone = ComService.ConvertInt(value.ToString()!);
                }
                if (attr.TryGetValue("OnPeriod", out value))
                {
                    onPeriod = ComService.ConvertInt(value.ToString()!);
                }
                if (attr.TryGetValue("OffPeriod", out value))
                {
                    offPeriod = ComService.ConvertInt(value.ToString()!);
                }
                if (attr.TryGetValue("RepeatCount", out value))
                {
                    repeatCount = ComService.ConvertInt(value.ToString()!);
                }
            }

            _ = htService!.StartBuzzer((short)tone, onPeriod, offPeriod, (short)repeatCount);
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
            model!.Case = string.Empty;
            model!.Bara = string.Empty;
        }

        /// <summary>
        /// パレット残在庫チェック
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GetArrivalReceptZan(string strArrivalNo, string strReceptNo)
        {
            try
            {
                bool ret = false;
                List<IDictionary<string, object>> datas = new();
                ClassNameSelect select = new()
                {
                    viewName = "VW_HT_入荷検品入力_受付残チェック",
                };
                select.whereParam.Add("入荷No", new WhereParam { val = strArrivalNo, whereType = enumWhereType.Equal });
                select.whereParam.Add("受付No", new WhereParam { val = strReceptNo, whereType = enumWhereType.Equal });
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


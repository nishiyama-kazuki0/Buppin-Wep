using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// パレット詰合せ/子親パレット№読取
    /// </summary>
    public partial class StepItemPalletAssortChildInput : StepItemBase
    {
        private StepItemPalletAssortViewModel? model;

        #region override

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            FirstFocusId = "InCPalletNo";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {
            model = (StepItemPalletAssortViewModel?)PageVm;

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
                model!.CPalletNo = scanData.strStringData;
                await ReadPalletInfo(value);
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
            if (_cardSelectedData == null || _cardSelectedData.Count == 0)
            {
                await ComService.DialogShowOK($"詰合せるパレットを読取または追加してください。", pageName);
                return false;
            }
            return true;
        }

        /// <summary>
        /// ストアドデータ設定-テーブルデータ作成
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task ストアドデータ設定_テーブルデータ作成(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            try
            {
                _storedTableData = new List<Dictionary<string, object>>();
                if (_cardValuesList is not null)
                {
                    foreach (IDictionary<string, DataCardListInfo> rows in _cardValuesList)
                    {
                        Dictionary<string, object> rowdata = new();
                        foreach (KeyValuePair<string, DataCardListInfo> data in rows)
                        {
                            rowdata[data.Key] = data.Value.Value;
                        }
                        _storedTableData.Add(rowdata);
                    }
                }
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
            model!.PPalletNo = string.Empty;
            await 前ステップへ(info);
        }

        /// <summary>
        /// 追加
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F2画面遷移(ComponentProgramInfo info)
        {
            await Task.Delay(0);//警告の抑制
            // 子パレットNoを追加する
            await ReadPalletInfo(model!.InCPalletNo);
            SetElementIdFocus("InCPalletNo");
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
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, model!.PPalletNo);
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
            await 前ステップへ(info);
        }

        /// <summary>
        /// パレットNoの変更処理
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangePalletNo(string value)
        {
            await Task.Delay(0);
            //追加ボタン押下時など、フォーカスが外れたときに余計に追加されたので下記コメントアウト
            //if (!IsPalletBarcode(value.ToString()!))
            //{
            //    return;
            //}
            //model!.CPalletNo = value;
            //await ReadPalletInfo(value);
        }

        /// <summary>
        /// カードのページ変更イベント
        /// </summary>
        /// <param name="args"></param>
        private async void OnPageChange(PagerEventArgs args)
        {
            await Task.Delay(0);
            //子パレットのカードが追加されるなら画面下部へスクロールする処理
            _ = InvokeAsync(async () =>
            {
                await Task.Delay(0);
                //ChildBaseService.IsFuncProc=falseになるまで待機する
                while (ChildBaseService.IsFuncProc || IsBarProc)
                {
                    await Task.Delay(300);//暫定
                }

                dynamic w = _js!.GetWindow();
                int Height = w.document.getElementById(SharedConst.STR_BODY_ID).offsetHeight;
                Dictionary<string, object> dict = new(){
                            { "top", Height },
                            { "left", 0 },
                            { "behavior", "smooth" }
                        };
                await w.document.getElementById(SharedConst.STR_BODY_ID).scrollBy(dict);

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

            // 混載状態を更新する
            _ = InvokeAsync(async () =>
            {
                PalletInfo info = await GetPalletInfo(model!.PPalletNo);
                model!.IsMixed = info.IsMixed;
                model!.AlertTitle = info.MixedAlertTitle;
                model!.AlertText = info.MixedAlertText;
                StateHasChanged();
            });

            // データの読込
            _ = await LoadViewModelBind();

        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            _cardValuesList?.Clear();
            model!.CPalletNo = string.Empty;
            model!.InCPalletNo = string.Empty;
        }

        /// <summary>
        /// 子パレット情報の読込
        /// </summary>
        private async Task ReadPalletInfo(string value)
        {
            try
            {
                if (_cardValuesList?.Count > 0)
                {
                    IEnumerable<IDictionary<string, DataCardListInfo>> child = _cardValuesList.Where(_ => _.TryGetValue("子ﾊﾟﾚｯﾄNo.", out DataCardListInfo info) && info.Value == value);
                    if (child.Any())
                    {
                        // 既に登録されているパレットNo
                        await ComService.DialogShowOK($"このパレットNoは既に読込んでいます。", pageName);
                        return;
                    }
                }

                Dictionary<string, WhereParam> whereParam = new()
                {
                    { "子ﾊﾟﾚｯﾄNo.", new WhereParam { val = value, whereType = enumWhereType.Equal } }
                };
                List<IDictionary<string, DataCardListInfo>> cardList = await GetCardListData("VW_HT_パレット詰合せ_子パレット情報", whereParam);
                if (cardList.Any())
                {
                    foreach (IDictionary<string, DataCardListInfo> card in cardList)
                    {
                        //_cardValuesList.Add(card);
                        _cardValuesList?.Insert(0, card);
                    }


                }
                else
                {
                    int notifyDuration = _sysParams is null ? SharedConst.DEFAULT_NOTIFY_DURATION : _sysParams.NotifyPopupDuration;
                    string strSummary = pageName.Replace("\\n", "");
                    // 子パレットでの検索結果が存在しない場合は通知
                    NotificationService.Notify(new NotificationMessage()
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = $"{strSummary}",
                        Detail = "追加対象が見つかりませんでした。",
                        Duration = notifyDuration
                    });
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        #endregion
    }
}
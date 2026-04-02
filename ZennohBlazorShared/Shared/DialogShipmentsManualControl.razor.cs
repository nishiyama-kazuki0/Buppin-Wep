using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using SharedModels;
using ZennohBlazorShared.Data;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// マニュアル出庫設定ダイアログ
    /// </summary>
    public partial class DialogShipmentsManualControl : ChildPageBasePC
    {
        private const string PROPKEY_納品日 = "納品日";
        private const string PROPKEY_出庫理由 = "出庫理由";
        private const string PROPKEY_倉庫配送先 = "倉庫配送先";
        private const string PROPKEY_加工区分 = "加工区分";
        private const string PROPKEY_加工場 = "加工場";
        private const string PROPKEY_緊急出荷区分 = "緊急出荷区分";
        private const string PROPKEY_摘要 = "摘要";
        private const string PROPKEY_備考 = "備考";

        private CompItemInfo? comp納品日;
        private CompItemInfo? comp出庫理由;
        private CompItemInfo? comp倉庫配送先;
        private CompItemInfo? comp加工区分;
        private CompItemInfo? comp加工場;
        private CompItemInfo? comp緊急出荷区分;
        private CompItemInfo? comp摘要;
        private CompItemInfo? comp備考;

        [Parameter]
        public IList<ComponentColumnsInfo> GridColumns { get; set; } = new List<ComponentColumnsInfo>();

        [Parameter]
        public List<IDictionary<string, object>> GridData { get; set; } = new List<IDictionary<string, object>>();

        [Inject]
        private ISessionStorageService? _sessionStorage { get; set; }

        //編集ダイアログの最大文字数,行,列数
        [Parameter]
        public int DialogContentMaxlength { set; get; } = 256;
        [Parameter]
        public int DialogContentCols { set; get; } = 60;
        [Parameter]
        public int DialogContentRows { set; get; } = 3;

        //編集ダイアログの概要の文字数
        [Parameter]
        public int DialogContentRemarksRows { get; set; } = 20;

        #region Override

        protected override async Task OnInitializedAsync()
        {
            if (null == _sessionStorage)
            {
                return;
            }
            SystemParameter sysParams = await _sessionStorage.GetItemAsync<SystemParameter>(SharedConst.KEY_SYSTEM_PARAM);
            if (null != sysParams)
            {
                DialogContentRemarksRows = sysParams.DialogContentRemarksRows;

                DialogContentMaxlength = sysParams.DialogContentMaxlength;
                DialogContentCols = sysParams.DialogContentCols;
                DialogContentRows = sysParams.DialogContentRows;
            }

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// F1ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private async Task OnClickResultF1(object? sender)
        {
            await base.ExecProgram();
        }

        /// <summary>
        /// F4ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        private async Task OnClickResultF4(object? sender)
        {
            await base.ExecProgram();
        }

        /// <summary>
        /// グリッド初期化
        /// </summary>
        /// <returns></returns>
        protected override async Task InitDataGridAsync()
        {
            _componentColumns = await ComService!.GetGridColumnsData(GetType().Name);
            _gridColumns = _componentColumns.Where(_ => _.ComponentName == STR_ATTRIBUTE_GRID).ToList();

            _gridData = GridData;
        }

        /// <summary>
        /// 検索条件初期化
        /// </summary>
        protected override async Task InitSearchConditionAsync()
        {
            // クリア
            _searchCompItems.Clear();

            // WMS作業日、WMS作業加算取得
            DateTime? dtWms = await ComService!.GetWmsDate(); ;
            DateTime? dtWmsAdd = await ComService!.GetWmsDateAdd();

            // 納品日
            comp納品日 = new CompItemInfo
            {
                CompType = typeof(CompDatePicker),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_納品日 },
                    { "Required", true },
                    { "InitialValue", string.Empty },
                    { "InitMode", enumDateInitMode.AllWmsAdd },
                    { "WmsDate", dtWms },
                    { "WmsDateAdd", dtWmsAdd },
                },
                TitleLabel = PROPKEY_納品日,
                DispTitleLabel = true,
                DispRequiredSuffix = true,
            };

            // 出庫理由
            List<ValueTextInfo>? dataReason = await ComService!.GetValueTextInfo("VW_DROPDOWN_理由コード区分");
            comp出庫理由 = new CompItemInfo
            {
                CompType = typeof(CompDropDown),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_出庫理由 },
                    { "Required", true },
                    { "InitialValue", string.Empty },
                    { "Data", dataReason }
                },
                TitleLabel = PROPKEY_出庫理由,
                DispTitleLabel = true,
                DispRequiredSuffix = true,
            };

            // 倉庫配送先
            List<ComponentColumnsInfo> columnsDeliveries = new();
            List<ComponentColumnsInfo> columns = _componentColumns.Where(_ => _.ComponentName == PROPKEY_倉庫配送先).ToList();
            for (int i = 0; columns.Count > i; i++)
            {
                columnsDeliveries.Add(new ComponentColumnsInfo() { Property = $"Value{i + 1}", Title = columns[i].Title, Type = columns[i].Type, Width = columns[i].Width, TextAlign = columns[i].TextAlign });
            }
            List<ValueTextInfo>? dataDeliveries = await ComService!.GetValueTextInfo("VW_DROPDOWNGRID_倉庫配送先コード");
            comp倉庫配送先 = new CompItemInfo
            {
                CompType = typeof(CompDropDownDataGridSingle),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_倉庫配送先 },
                    { "Required", true },
                    { "InitialValue", string.Empty },
                    { "Columns", columnsDeliveries },
                    { "Data", dataDeliveries }
                },
                TitleLabel = PROPKEY_倉庫配送先,
                DispTitleLabel = true,
                DispRequiredSuffix = true,
            };

            // 加工区分
            List<ValueTextInfo>? dataProcessing = await ComService!.GetValueTextInfo("VW_DROPDOWN_加工区分");
            comp加工区分 = new CompItemInfo
            {
                CompType = typeof(CompDropDown),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_加工区分 },
                    { "Required", true },
                    { "InitialValue", string.Empty },
                    { "Data", dataProcessing }
                },
                TitleLabel = PROPKEY_加工区分,
                DispTitleLabel = true,
                DispRequiredSuffix = true,
            };

            // 加工場
            List<ValueTextInfo>? dataProcessingPlants = await ComService!.GetValueTextInfo("VW_DROPDOWN_加工場");
            comp加工場 = new CompItemInfo
            {
                CompType = typeof(CompDropDown),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_加工場 },
                    { "Required", false },
                    { "InitialValue", string.Empty },
                    { "Data", dataProcessingPlants }
                },
                TitleLabel = PROPKEY_加工場,
                DispTitleLabel = true,
                DispRequiredSuffix = false,
            };

            // 緊急出荷区分
            List<ValueTextInfo>? dataUrgent = await ComService!.GetValueTextInfo("VW_DROPDOWN_緊急出荷区分");
            comp緊急出荷区分 = new CompItemInfo
            {
                CompType = typeof(CompDropDown),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_緊急出荷区分 },
                    { "Required", true },
                    { "InitialValue", string.Empty },
                    { "Data", dataUrgent }
                },
                TitleLabel = PROPKEY_緊急出荷区分,
                DispTitleLabel = true,
                DispRequiredSuffix = true,
            };

            // 摘要
            comp摘要 = new CompItemInfo
            {
                CompType = typeof(CompTextBox),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_摘要 },
                    { "Required", false },
                    { "InitialValue", string.Empty },
                    { "MaxLength", (long)DialogContentRemarksRows },
                },
                TitleLabel = PROPKEY_摘要,
                DispTitleLabel = true,
                DispRequiredSuffix = false,
            };

            // 備考
            comp備考 = new CompItemInfo
            {
                CompType = typeof(CompTextArea),
                CompParam = new Dictionary<string, object>
                {
                    { "Title", PROPKEY_備考 },
                    { "Required", false },
                    { "InitialValue", string.Empty },
                    { "MaxLength", (long)DialogContentMaxlength },
                    { "Rows", DialogContentCols },
                    { "Cols", DialogContentRows },
                    //{ "RegexPattern" , SharedConst.STR_VARIDATE_FULL_WIDTH_CHAR_ONLY} //TODO :半角交じりでも上位連携はエラーになっていないようなので、一旦コメント化。必要な場合はコメントを戻す。
                },
                TitleLabel = PROPKEY_備考,
                DispTitleLabel = true,
                DispRequiredSuffix = false,
            };

            // コンポーネント情報追加
            _searchCompItems.Add(new List<CompItemInfo> { comp納品日 });
            _searchCompItems.Add(new List<CompItemInfo> { comp出庫理由, comp倉庫配送先, comp緊急出荷区分 });
            _searchCompItems.Add(new List<CompItemInfo> { comp加工区分, comp加工場 });
            _searchCompItems.Add(new List<CompItemInfo> { comp摘要 });
            _searchCompItems.Add(new List<CompItemInfo> { comp備考 });
        }

        /// <summary>
        /// 選択行チェック処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 選択行チェック(ComponentProgramInfo info)
        {
            // チェック
            if (_gridData == null || _gridData?.Count() == 0)
            {
                // メッセージ取得
                string msg = "データがありません。";
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
        /// 確定前チェック処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            // 加工区分が1の場合、加工場が選択されているかチェック
            string strProcessingType = string.Empty;
            string strProcessingTypeName = string.Empty;
            string strProcessingPlants = string.Empty;
            {
                if (comp加工区分?.CompObj?.Instance is CompDropDown compDropDown)
                {
                    strProcessingType = compDropDown.InputValue;
                    strProcessingTypeName = compDropDown.GetInputText();
                }
            }
            {
                if (comp加工場?.CompObj?.Instance is CompDropDown compDropDown)
                {
                    strProcessingPlants = compDropDown.InputValue;
                }
            }
            if (strProcessingType == "1" && string.IsNullOrEmpty(strProcessingPlants))
            {
                await ComService.DialogShowOK($"加工区分が[{strProcessingTypeName}]の場合、加工場を選択してください。", pageName);
                return false;
            }

            return true;
        }

        /// <summary>
        /// ストアドデータ設定_引数データ作成
        /// </summary>
        /// <returns></returns>
        public override async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            try
            {
                _storedData = new Dictionary<string, object>();
                Dictionary<string, (object, WhereParam)> items = ComService.GetCompItemInfoValues(_searchCompItems);
                if (items is not null)
                {
                    // 納品日
                    {
                        _storedData[PROPKEY_納品日] = items.TryGetValue(PROPKEY_納品日, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                    // 出庫理由
                    {
                        _storedData[PROPKEY_出庫理由] = items.TryGetValue(PROPKEY_出庫理由, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                    // 倉庫配送先
                    {
                        _storedData[PROPKEY_倉庫配送先] = items.TryGetValue(PROPKEY_倉庫配送先, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                    // 加工区分
                    {
                        _storedData[PROPKEY_加工区分] = items.TryGetValue(PROPKEY_加工区分, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                    // 加工場
                    {
                        _storedData[PROPKEY_加工場] = items.TryGetValue(PROPKEY_加工場, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                    // 緊急出荷区分
                    {
                        _storedData[PROPKEY_緊急出荷区分] = items.TryGetValue(PROPKEY_緊急出荷区分, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                    // 摘要
                    {
                        _storedData[PROPKEY_摘要] = items.TryGetValue(PROPKEY_摘要, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                    // 備考
                    {
                        _storedData[PROPKEY_備考] = items.TryGetValue(PROPKEY_備考, out (object, WhereParam) data) ? data.Item1 : string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// ストアドデータ設定_テーブルデータ作成
        /// </summary>
        /// <returns></returns>
        public override async Task ストアドデータ設定_テーブルデータ作成(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            try
            {
                _storedTableData = new List<Dictionary<string, object>>();
                if (_gridData is not null)
                {
                    foreach (IDictionary<string, object> rows in _gridData)
                    {
                        Dictionary<string, object> rowdata = new();
                        foreach (KeyValuePair<string, object> data in rows)
                        {
                            rowdata[data.Key] = data.Value;
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

        #endregion
    }
}

using System.Collections;
using System.Reflection;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 入荷進捗照会 品名別
    /// </summary>
    public partial class TabItemArrivalsProgressProducts : TabItemBase
    {
        public const string STR_ATTRIBUTE_GRID_ALL = "AttributesGridAll";
        public const string STR_GRID_VIEW_NAME = "VW_入荷進捗_品名別";
        public const string STR_GRID_ALL_VIEW_NAME = "VW_入荷進捗_品名別_全体進捗";

        public const string STR_GRID_COL_入荷予定数 = "入荷\n予定数";
        public const string STR_GRID_COL_受付数 = "受付数";
        public const string STR_GRID_COL_受付進捗率 = "受付\n進捗率";
        public const string STR_GRID_COL_PRG_受付進捗率 = DataGridProgress.KEY_PROGRESS + "受付進捗率";
        public const string STR_GRID_COL_入荷検品数 = "入荷\n検品数";
        public const string STR_GRID_COL_検品進捗率 = "検品\n進捗率";
        public const string STR_GRID_COL_PRG_検品進捗率 = DataGridProgress.KEY_PROGRESS + "検品進捗率";
        public const string STR_GRID_COL_パレット紐づけ数 = "パレット\n紐づけ数";
        public const string STR_GRID_COL_紐づけ進捗率 = "紐づけ\n進捗率";
        public const string STR_GRID_COL_PRG_紐づけ進捗率 = DataGridProgress.KEY_PROGRESS + "紐づけ進捗率";

        /// <summary>
        /// 全体グリッドカラム定義
        /// </summary>
        protected IList<ComponentColumnsInfo> _gridAllColumns { get; set; } = new List<ComponentColumnsInfo>();

        /// <summary>
        /// 全体グリッドデータ
        /// </summary>
        protected List<IDictionary<string, object>> _gridAllData { get; set; } = new List<IDictionary<string, object>>();

        #region override

        /// <summary>
        /// グリッド初期化
        /// </summary>
        /// <returns></returns>
        protected override async Task InitDataGridAsync()
        {
            // カラム設定情報取得
            _componentColumns = await ComService!.GetGridColumnsData(GetType().Name);

            _gridColumns = _componentColumns.Where(_ => _.ComponentName == STR_ATTRIBUTE_GRID).ToList();
            _gridAllColumns = _componentColumns.Where(_ => _.ComponentName == STR_ATTRIBUTE_GRID_ALL).ToList();
        }

        /// <summary>
        /// attributes情報初期化
        /// </summary>
        /// <returns></returns>
        protected override async Task InitAttributesAsync()
        {
            await Task.Delay(0);

            // Attributesクリア
            Attributes.Clear();

            // コンポーネントの種類を追加
            _componentsInfo.GroupBy(_ => _.ComponentName).ToList().ForEach(group =>
            {
                Attributes.Add(group.Key, new Dictionary<string, object>());
            });

            // 各コンポーネントのAttributesを設定
            for (int i = 0; _componentsInfo.Count > i; i++)
            {
                try
                {
                    IDictionary attribute = (IDictionary)Attributes[_componentsInfo[i].ComponentName];
                    if (attribute != null)
                    {
                        object? value = null;
                        switch (_componentsInfo[i].ValueObjectType)
                        {
                            case (int)ComponentsInfo.EnumValueObjectType.ValueIndicator:
                                // 値をデータ型より変換
                                value = Convert.ChangeType(_componentsInfo[i].Value, _componentsInfo[i].Type);
                                break;
                            case (int)ComponentsInfo.EnumValueObjectType.VariableIndicator:
                                // 変数文字列から変数を取得
                                Type type = _componentsInfo[i].Type;
                                if (null == type)
                                {
                                    type = typeof(TabItemArrivalsProgressProducts);
                                }
                                PropertyInfo? pi = type.GetProperty(_componentsInfo[i].Value, BindingFlags.NonPublic | BindingFlags.Instance);
                                if (pi != null)
                                {
                                    value = pi.GetValue(this, null);
                                }
                                break;
                            case (int)ComponentsInfo.EnumValueObjectType.EnumStringIndicator:
                                // Enumを文字列から値に変換
                                string strEnumStr = _componentsInfo[i].Value;
                                string strEnumStrPos = strEnumStr[(strEnumStr.LastIndexOf('.') + 1)..];
                                value = typeof(CommonService).GetMethod("GetEnumValue")!.MakeGenericMethod(_componentsInfo[i].Type).Invoke(null, new object[] { strEnumStrPos });
                                break;
                            case (int)ComponentsInfo.EnumValueObjectType.ClassNameIndicator:
                                value = _componentsInfo[i].Type;
                                break;
                        }
                        if (value != null)
                        {
                            attribute.Add(_componentsInfo[i].AttributesName, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _ = ComService.PostLogAsync(ex.Message);
                }
            }
        }

        /// <summary>
        /// グリッド更新
        /// </summary>
        public override async Task グリッド更新(ComponentProgramInfo info)
        {
            try
            {
                ClassNameSelect custom = new();
                Dictionary<string, (object, WhereParam)> items = ComService.GetCompItemInfoValues(_searchCompItems);
                foreach (KeyValuePair<string, (object, WhereParam)> item in items)
                {
                    custom.whereParam.Add(item.Key, item.Value.Item2);
                }
                await RefreshGridData(custom.whereParam, strViewName: STR_GRID_VIEW_NAME, attributeName: info.ComponentName);
                await RefreshGridAllData();
            }
            catch (Exception ex)
            {
                _ = ComService.PostLogAsync(ex.Message);
            }
        }

        /// <summary>
        /// 画面クリア処理
        /// </summary>
        public override void 画面クリア(ComponentProgramInfo info)
        {
            // 検索条件クリア
            ClearSearchCondition();

            // グリッドクリア
            Attributes[STR_ATTRIBUTE_GRID_ALL]["Data"] = _gridAllData = new List<IDictionary<string, object>>();
            Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = new List<IDictionary<string, object>>();

            // 選択データクリア
            _gridSelectedData = null;
        }

        #endregion

        #region private

        /// <summary>
        /// 全体グリッドに表示するデータを取得しグリッドにセットする
        /// </summary>
        /// <returns></returns>
        private async Task RefreshGridAllData()
        {
            try
            {
                // グリッドクリア
                _ = Attributes[STR_ATTRIBUTE_GRID_ALL]["Data"] = _gridAllData = new List<IDictionary<string, object>>();

                // 明細データがある場合のみ全体グリッド更新
                if (_gridData != null && _gridData.Count() > 0)
                {
                    // VIEWから全体グリッドの枠を取得
                    ClassNameSelect select = new()
                    {
                        viewName = STR_GRID_ALL_VIEW_NAME,
                    };
                    _gridAllData = await ComService!.GetSelectGridData(_gridAllColumns, select);

                    if (_gridAllData != null && _gridAllData.Count() > 0)
                    {
                        // 明細データを集計
                        decimal dec入荷予定数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_入荷予定数) ? Convert.ToString(_[STR_GRID_COL_入荷予定数]) : "0"));
                        decimal dec受付数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_受付数) ? Convert.ToString(_[STR_GRID_COL_受付数]) : "0"));
                        decimal dec入荷検品数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_入荷検品数) ? Convert.ToString(_[STR_GRID_COL_入荷検品数]) : "0"));
                        decimal decパレット紐づけ数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_パレット紐づけ数) ? Convert.ToString(_[STR_GRID_COL_パレット紐づけ数]) : "0"));

                        // グリッドにセット
                        _gridAllData[0][STR_GRID_COL_入荷予定数] = dec入荷予定数;
                        _gridAllData[0][STR_GRID_COL_受付数] = dec受付数;
                        _gridAllData[0][STR_GRID_COL_PRG_受付進捗率] = _gridAllData[0][STR_GRID_COL_受付進捗率] = ComService!.GetPercent(dec受付数, dec入荷予定数, 1);
                        _gridAllData[0][STR_GRID_COL_入荷検品数] = dec入荷検品数;
                        _gridAllData[0][STR_GRID_COL_PRG_検品進捗率] = _gridAllData[0][STR_GRID_COL_検品進捗率] = ComService!.GetPercent(dec入荷検品数, dec入荷予定数, 1);
                        _gridAllData[0][STR_GRID_COL_パレット紐づけ数] = decパレット紐づけ数;
                        _gridAllData[0][STR_GRID_COL_PRG_紐づけ進捗率] = _gridAllData[0][STR_GRID_COL_紐づけ進捗率] = ComService!.GetPercent(decパレット紐づけ数, dec入荷予定数, 1);
                    }
                }

                // グリッドデータ更新
                _ = Attributes[STR_ATTRIBUTE_GRID_ALL]["Data"] = _gridAllData;

                // Blazor へ状態変化を通知
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
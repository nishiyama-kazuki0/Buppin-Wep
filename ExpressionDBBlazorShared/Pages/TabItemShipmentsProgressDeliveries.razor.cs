using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Shared;
using ExpressionDBBlazorShared.Util;
using SharedModels;
using System.Collections;
using System.Reflection;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// 出荷進捗照会 倉庫配送先別
/// </summary>
public partial class TabItemShipmentsProgressDeliveries : TabItemBase
{
    public const string STR_ATTRIBUTE_GRID_ALL = "AttributesGridAll";
    public const string STR_GRID_VIEW_NAME = "VW_出荷進捗_倉庫配送先別";
    public const string STR_GRID_ALL_VIEW_NAME = "VW_出荷進捗_倉庫配送先別_全体進捗";

    public const string STR_GRID_COL_出荷予定数 = "出荷\n予定数";
    public const string STR_GRID_COL_欠品数 = "欠品数";
    public const string STR_GRID_COL_引当済数 = "引当済数";
    public const string STR_GRID_COL_引当進捗率 = "引当\n進捗率";
    public const string STR_GRID_COL_PRG_引当進捗率 = DataGridProgress.KEY_PROGRESS + "引当進捗率";
    public const string STR_GRID_COL_ピック済数 = "ピック済数";
    public const string STR_GRID_COL_ピック進捗率 = "ピック\n進捗率";
    public const string STR_GRID_COL_PRG_ピック進捗率 = DataGridProgress.KEY_PROGRESS + "ピック進捗率";
    public const string STR_GRID_COL_コーナー搬送済数 = "コーナー\n搬送済数";
    public const string STR_GRID_COL_搬送進捗率 = "搬送\n進捗率";
    public const string STR_GRID_COL_PRG_搬送進捗率 = DataGridProgress.KEY_PROGRESS + "搬送進捗率";

    /// <summary>
    /// 全体グリッドカラム定義
    /// </summary>
    protected IList<ComponentColumnsInfo> _gridAllColumns { get; set; } = [];

    /// <summary>
    /// 全体グリッドデータ
    /// </summary>
    protected List<IDictionary<string, object>> _gridAllData { get; set; } = [];

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
                                type = typeof(TabItemShipmentsProgressDeliveries);
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
                            value = typeof(ConvertUtil).GetMethod("GetEnumValue")!.MakeGenericMethod(_componentsInfo[i].Type).Invoke(null, new object[] { strEnumStrPos });
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
            catch (Exception ex) { _ = WebComService.PostLogAsync(ex.Message); }
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
            _ = WebComService.PostLogAsync(ex.Message);
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
        Attributes[STR_ATTRIBUTE_GRID_ALL]["Data"] = _gridAllData = [];
        Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = [];

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
            _ = Attributes[STR_ATTRIBUTE_GRID_ALL]["Data"] = _gridAllData = [];

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
                    decimal dec出荷予定数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_出荷予定数) ? Convert.ToString(_[STR_GRID_COL_出荷予定数]) : "0"));
                    decimal dec引当済数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_引当済数) ? Convert.ToString(_[STR_GRID_COL_引当済数]) : "0"));
                    decimal dec欠品数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_欠品数) ? Convert.ToString(_[STR_GRID_COL_欠品数]) : "0"));
                    decimal decピック済数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_ピック済数) ? Convert.ToString(_[STR_GRID_COL_ピック済数]) : "0"));
                    decimal decコーナー搬送済数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_コーナー搬送済数) ? Convert.ToString(_[STR_GRID_COL_コーナー搬送済数]) : "0"));

                    // グリッドにセット
                    _gridAllData[0][STR_GRID_COL_出荷予定数] = dec出荷予定数;
                    _gridAllData[0][STR_GRID_COL_引当済数] = dec引当済数;
                    _gridAllData[0][STR_GRID_COL_PRG_引当進捗率] = _gridAllData[0][STR_GRID_COL_引当進捗率] = CalcUtil.GetPercent(dec引当済数, dec出荷予定数, 1);
                    _gridAllData[0][STR_GRID_COL_欠品数] = dec欠品数;
                    _gridAllData[0][STR_GRID_COL_ピック済数] = decピック済数;
                    _gridAllData[0][STR_GRID_COL_PRG_ピック進捗率] = _gridAllData[0][STR_GRID_COL_ピック進捗率] = CalcUtil.GetPercent(decピック済数, dec出荷予定数, 1);
                    _gridAllData[0][STR_GRID_COL_コーナー搬送済数] = decコーナー搬送済数;
                    _gridAllData[0][STR_GRID_COL_PRG_搬送進捗率] = _gridAllData[0][STR_GRID_COL_搬送進捗率] = CalcUtil.GetPercent(decコーナー搬送済数, dec出荷予定数, 1);
                }
            }

            // グリッドデータ更新
            _ = Attributes[STR_ATTRIBUTE_GRID_ALL]["Data"] = _gridAllData;

            // Blazor へ状態変化を通知
            StateHasChanged();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    /// <summary>
    /// OnCellRenderのCallBack
    /// </summary>
    /// <param name="args"></param>
    private new void CellRender(DataGridCellRenderEventArgs<IDictionary<string, object>> args)
    {
        try
        {
            if ("作業開始\\n時間" == args.Column.Title)
            {
                // 作業開始時間の背景色変更
                if (args.Data.TryGetValue("作業開始_残分", out object? value))
                {
                    ComService.AddAttrWarnExcessTime(value?.ToString(), _sysParams.ShipmentsStartWarnTime, args.Attributes);
                }
            }
            else if ("出荷締切\\n時間" == args.Column.Title)
            {
                // 出荷締切時間の背景色変更
                if (args.Data.TryGetValue("出荷締切_残分", out object? value))
                {
                    ComService.AddAttrWarnExcessTime(value?.ToString(), _sysParams.ShipmentsDeadlineWarnTime, args.Attributes);
                }
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    #endregion
}
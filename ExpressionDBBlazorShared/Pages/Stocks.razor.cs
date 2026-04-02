using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Shared;
using SharedModels;

namespace ExpressionDBBlazorShared.Pages;

public partial class Stocks : ChildPageBasePC
{
    #region override

    /// <summary>
    /// グリッドに表示するデータを取得しグリッドにセットする
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// </summary>
    /// <returns></returns>
    public override async Task RefreshGridData(Dictionary<string, WhereParam> whereParam, string strViewName = "", string attributeName = STR_ATTRIBUTE_GRID, bool bInitSelect = false)
    {
        try
        {
            await base.RefreshGridData(whereParam, strViewName, attributeName, bInitSelect);

            // パレット数合計をセット
            int intPaletteCnt = _gridData.Select(_ => Convert.ToString(_["パレットNo"])).Where(_ => !string.IsNullOrEmpty(_.Trim())).Distinct().Count();
            Attributes[attributeName]["HeaderTextValue"] = intPaletteCnt.ToString("N0");

            StateHasChanged();
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
        base.画面クリア(info);

        // パレット数合計をクリア
        Attributes[STR_ATTRIBUTE_GRID]["HeaderTextValue"] = string.Empty;
    }

    /// <summary>
    /// ストアドデータ設定_引数データ作成
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
    {
        await Task.Delay(0);
        try
        {
            switch (info.ComponentName)
            {
                case "F4StoredData":
                    SetStoredParam_在庫リスト発行();
                    break;
                case "F7StoredData":
                    SetStoredParam_在庫メンテナンス削除();
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }

    #endregion

    #region private

    /// <summary>
    /// 在庫リスト発行用のストアドパラメータセット
    /// </summary>
    private void SetStoredParam_在庫リスト発行()
    {
        _storedData = [];
        Dictionary<string, (object, WhereParam)> items = ComService.GetCompItemInfoValues(_searchCompItems);
        if (items is not null)
        {
            string strName = "入庫日From";
            _storedData[strName] = items.TryGetValue(strName, out (object, WhereParam) data) ? data.Item1 : string.Empty;
            strName = "入庫日To";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "入荷No";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "明細No";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "品名コード";
            _storedData[strName] = items.TryGetValue(strName, out data) ? string.Join(',', (List<string>)data.Item1) : string.Empty;
            strName = "課コード";
            _storedData[strName] = items.TryGetValue(strName, out data) ? string.Join(',', (List<string>)data.Item1) : string.Empty;
            strName = "出荷者コード";
            _storedData[strName] = items.TryGetValue(strName, out data) ? string.Join(',', (List<string>)data.Item1) : string.Empty;
            strName = "産地コード";
            _storedData[strName] = items.TryGetValue(strName, out data) ? string.Join(',', (List<string>)data.Item1) : string.Empty;
            strName = "パレットNo";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "賞味期限From";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "賞味期限To";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "倉庫コード";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "ゾーンコード";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "ロケーションNo";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "在庫有無";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "資材管理";
            _storedData[strName] = items.TryGetValue(strName, out data) ? data.Item1 : string.Empty;
            strName = "滞留期間(日)";
            if (items.TryGetValue(strName, out data))
            {
                _storedData["滞留期間"] = data.Item1;
                _storedData["滞留期間whereType"] = (int)data.Item2.whereType;
            }
            else
            {
                _storedData["滞留期間"] = string.Empty;
                _storedData["滞留期間whereType"] = (int)enumWhereType.Equal;
            }
        }
    }

    /// <summary>
    /// 在庫メンテナンス削除用のストアドパラメータセット
    /// </summary>
    private void SetStoredParam_在庫メンテナンス削除()
    {
        _storedData = [];
        if (_gridSelectedData is not null)
        {
            foreach (IDictionary<string, object> rows in _gridSelectedData)
            {
                foreach (KeyValuePair<string, object> data in rows)
                {
                    if (null != _storedData)
                    {
                        _storedData[data.Key] = data.Value;
                    }
                }
                break;
            }
        }
    }

    #endregion
}
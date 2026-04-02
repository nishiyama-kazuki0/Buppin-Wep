using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Shared;
using SharedModels;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// AGV搬送情報メンテナンス
/// </summary>
public partial class AGVOrderInformationMaintenance : ChildPageBasePC
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

        // 画面クリア
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
                case "F7StoredData":
                    SetStoredParam_AGV移動完了();
                    break;
                case "F8StoredData":
                    SetStoredParam_AGV搬送情報削除();
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
    /// AGV移動完了用のストアドパラメータセット
    /// </summary>
    private void SetStoredParam_AGV移動完了()
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
            //if (null != _storedData)
            //{
            //    _storedData["操作端末"] = 0;
            //}

        }
    }
    /// <summary>
    /// AGV搬送情報削除用のストアドパラメータセット
    /// </summary>
    private void SetStoredParam_AGV搬送情報削除()
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
            //if (null != _storedData)
            //{
            //    _storedData["操作端末"] = 0;
            //}

        }
    }

    #endregion
}
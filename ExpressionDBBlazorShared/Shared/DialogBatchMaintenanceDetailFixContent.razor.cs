using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Pages;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Util;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SharedModels;
using Sotsera.Blazor.Toaster.Core.Models;
using System.Collections;
using System.Reflection;
using System.Runtime;
using System.Text;
using static SharedModels.SharedConst;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// バッチメンテナンス明細ダイアログ
/// </summary>
public partial class DialogBatchMaintenanceDetailFixContent : DialogCommonGridInputContent
{
    public string ProductionPlan = "";//製造区分
    public string BatchManagementId = "";//バッチ管理ID
    public string MaterialCode = "";//原料品目コード
    public const string STR_ATTRIBUTE_GRID_ALL = "AttributesGridAll";
    public string STR_GRID_ALL_VIEW_NAME = "FC_GET_PRODUCTION_PLAN_ALLOCATE('{バッチ管理ID}','{原料品目コード}')";

    /// <summary>
    /// 全体グリッドカラム定義
    /// </summary>
    protected IList<ComponentColumnsInfo> _gridAllColumns { get; set; } = [];
    /// <summary>
    /// グリッド初期化
    /// </summary>
    /// <returns></returns>
    protected override async Task InitDataGridAsync()
    {
        //取得した各コンポーネント情報を元にVIEW_NAMEの決定
        STR_GRID_ALL_VIEW_NAME = $"FC_GET_PRODUCTION_PLAN_ALLOCATE('{BatchManagementId}','{MaterialCode}')";
        // カラム設定情報取得※仮のVWから枠組のみ作成し、後にgridDataを作成、格納予定、要検討
        _componentColumns = await ComService!.GetGridColumnsData(GetType().Name);
        foreach (var gridList in _componentColumns)
        {
            gridList.ViewName = STR_GRID_ALL_VIEW_NAME;
        }
        _gridColumns = _componentColumns.Where(_ => _.ComponentName == STR_ATTRIBUTE_GRID).ToList();
        _gridAllColumns = _componentColumns.Where(_ => _.ComponentName == STR_ATTRIBUTE_GRID_ALL).ToList();
    }
    /// <summary>
    /// グリッドに表示するデータを取得しグリッドにセットする
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// </summary>
    /// <returns></returns>
    public override async Task RefreshGridData(Dictionary<string, WhereParam>? whereParam, string strViewName = "", string attributeName = STR_ATTRIBUTE_GRID, bool bInitSelect = false)
    {
        try
        {
            await base.RefreshGridData(whereParam, strViewName, attributeName, bInitSelect);

            //// パレット数合計をセット
            //int intPaletteCnt = _gridData.Select(_ => Convert.ToString(_["パレットNo"])).Where(_ => !string.IsNullOrEmpty(_.Trim())).Distinct().Count();
            //Attributes[attributeName]["HeaderTextValue"] = intPaletteCnt.ToString("N0");

            StateHasChanged();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
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
                                type = typeof(TabItemShipmentsProgressAreas);
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
        await RefreshGridAllData();
        // 選択データを初期化
        _gridSelectedData = _gridData
            .Where(row => (bool.TryParse(row["選択状態"] as string, out var 選択状態) && 選択状態))
            .ToList();
    }
    /// <summary>
    /// グリッド更新
    /// </summary>
    // 必要か不明なため、一旦コメント
    //public override async Task グリッド更新(ComponentProgramInfo info)
    //{
    //    try
    //    {
    //        ClassNameSelect custom = new();
    //        Dictionary<string, (object, WhereParam)> items = ComService.GetCompItemInfoValues(_searchCompItems);
    //        foreach (KeyValuePair<string, (object, WhereParam)> item in items)
    //        {
    //            custom.whereParam.Add(item.Key, item.Value.Item2);
    //        }
    //        STR_GRID_ALL_VIEW_NAME = $"FC_GET_PRODUCTION_PLAN_ALLOCATE('{BatchManagementId}','{MaterialCode}')";
    //        await RefreshGridData(custom.whereParam, strViewName: STR_GRID_ALL_VIEW_NAME, attributeName: info.ComponentName);
    //        await RefreshGridAllData();
    //    }
    //    catch (Exception ex)
    //    {
    //        _ = WebComService.PostLogAsync(ex.Message);
    //    }
    //}
    /// <summary>
    /// 全体グリッドに表示するデータを取得しグリッドにセットする
    /// </summary>
    /// <returns></returns>
    private async Task RefreshGridAllData()
    {
        try
        {
            // グリッドクリア
            _ = Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData = [];
            STR_GRID_ALL_VIEW_NAME = $"FC_GET_PRODUCTION_PLAN_ALLOCATE('{BatchManagementId}','{MaterialCode}')";
            // 明細データがある場合のみ全体グリッド更新
            //if (_gridData != null && _gridData.Count() > 0)
            //{
                // VIEWから全体グリッドの枠を取得
                ClassNameSelect select = new()
                {
                    viewName = STR_GRID_ALL_VIEW_NAME,
                    columnsDefineName = "FC_GET_PRODUCTION_PLAN_ALLOCATE"
                };
                _gridData = await ComService!.GetSelectGridData(_gridColumns, select);
            //カラムの設定とviewNameに関数名の代入は完了、griddataに値は取得できていない


                //if (_gridData != null && _gridData.Count() > 0)
                //{
                    // 明細データを集計
                    //decimal dec引当済数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_引当済数) ? Convert.ToString(_[STR_GRID_COL_引当済数]) : "0"));
                    //decimal dec欠品数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_欠品数) ? Convert.ToString(_[STR_GRID_COL_欠品数]) : "0"));
                    //decimal decピック済数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_ピック済数) ? Convert.ToString(_[STR_GRID_COL_ピック済数]) : "0"));
                    //decimal decコーナー別仕分済数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_コーナー別仕分済数) ? Convert.ToString(_[STR_GRID_COL_コーナー別仕分済数]) : "0"));
                    //decimal decコーナー搬送済数 = _gridData.Sum(_ => decimal.Parse(_.ContainsKey(STR_GRID_COL_コーナー搬送済数) ? Convert.ToString(_[STR_GRID_COL_コーナー搬送済数]) : "0"));

                    // グリッドにセット
                    //_gridData[0]["原料品目コード"] = "テスト";
                    //_gridData[0][STR_GRID_COL_引当済数] = dec引当済数;
                    //_gridData[0][STR_GRID_COL_欠品数] = dec欠品数;
                    //_gridData[0][STR_GRID_COL_ピック済数] = decピック済数;
                    //_gridData[0][STR_GRID_COL_PRG_ピック進捗率] = _gridAllData[0][STR_GRID_COL_ピック進捗率] = CalcUtil.GetPercent(decピック済数, dec引当済数, 1);
                    //_gridData[0][STR_GRID_COL_コーナー別仕分済数] = decコーナー別仕分済数;
                    //_gridData[0][STR_GRID_COL_PRG_コーナー別仕分進捗率] = _gridAllData[0][STR_GRID_COL_コーナー別仕分進捗率] = CalcUtil.GetPercent(decコーナー別仕分済数, dec引当済数, 1);
                    //_gridData[0][STR_GRID_COL_コーナー搬送済数] = decコーナー搬送済数;
                    //_gridData[0][STR_GRID_COL_PRG_搬送進捗率] =   _gridAllData[0][STR_GRID_COL_搬送進捗率] = CalcUtil.GetPercent(decコーナー搬送済数, dec引当済数, 1);
                //}
            //}

            // グリッドデータ更新
            _ = Attributes[STR_ATTRIBUTE_GRID]["Data"] = _gridData;

            // Blazor へ状態変化を通知
            StateHasChanged();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message);
        }
    }


    /// <summary>
    /// 各コンポーネントの値取得
    /// </summary>

    protected override async Task InitInputItemsAsync()
    {
        foreach (List<CompItemInfo> listItem in _infoItems)
        {
            foreach (CompItemInfo item in listItem)
            {
                //手動投入のチェック
                if (item.CompParam.TryGetValue("InitialValue", out object value))
                {
                    if (value.ToString() == "手動投入")
                    {
                        ProductionPlan = "手動投入";
                    }
                }
                //各引数取得
                if (item.CompParam.TryGetValue("Title", out object valueTitle))
                {
                    switch (valueTitle.ToString()) 
                    {

                        case "バッチ管理ID":
                            if (item.CompParam.TryGetValue("InitialValue", out object valueBatch))
                            {
                                BatchManagementId = valueBatch.ToString();
                            }
                            break;
                        case "原料品目コード":
                            if (item.CompParam.TryGetValue("InitialValue", out object valueMaterial))
                            {
                                MaterialCode = valueMaterial.ToString();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    protected override async Task OnClickResultF1(object? sender)
    {
        // チェック
        if (false == バリデートチェック())
        {
            return;
        }

        bool retb = false;
        try
        {
            // 入力エリアロック
            ComService.SetCompItemListDisabled(_inputItems, true);

            // 結果変数
            List<ExecResult> lstResult = [];
            int notifyDuration = _sysParams is null ? SharedConst.DEFAULT_NOTIFY_DURATION : _sysParams.NotifyPopupDuration;
            string strSummary = DialogTitle.Replace("\\n", "");
            string strResultMessage = "登録しました。";

            // 確認
            bool? retConfirm;
            if (PasswordCheck) retConfirm = await ComService.DialogShowPassword("設定を確定しますか？", strSummary);
            else retConfirm = await ComService.DialogShowYesNo("設定を確定しますか？", strSummary);

            retb = retConfirm is not null && (bool)retConfirm;
            if (!retb)
            {
                return;
            }

            // 登録
            retb = false;
            string programName = ProgramName;
            if (ProductionPlan == "手動投入")
            {
                programName = "バッチメンテナンス明細編集_手動投入";
            }    
            if (!string.IsNullOrEmpty(programName))
            {
                // RequestValueにデータを作成する
                RequestValue rv = RequestValue.CreateRequestProgram(programName);

                // 情報エリアからWebAPIに渡す値を作成する
                Dictionary<string, (object, WhereParam)> itemInfos = ComService.GetCompItemInfoValues(_infoItems);
                foreach (List<CompItemInfo> listItem in _infoItems)
                {
                    foreach (CompItemInfo item in listItem)
                    {
                        _ = itemInfos.TryGetValue(item.TitleLabel, out (object, WhereParam) value)
                            ? rv.SetArgumentValue(item.TitleLabel, value.Item1, "")
                            : rv.SetArgumentValue(item.TitleLabel, string.Empty, "");
                    }
                }

                // 入力エリアからWebAPIに渡す値を作成する
                Dictionary<string, (object, WhereParam)> itemInputs = ComService.GetCompItemInfoValues(_inputItems);
                foreach (List<CompItemInfo> listItem in _inputItems)
                {
                    foreach (CompItemInfo item in listItem)
                    {
                        _ = itemInputs.TryGetValue(item.TitleLabel, out (object, WhereParam) value)
                            ? rv.SetArgumentValue(item.TitleLabel, value.Item1, "")
                            : rv.SetArgumentValue(item.TitleLabel, string.Empty, "");
                    }
                }

                // グリッドデータからWebAPIに渡す値を作成する
                if (_gridSelectedData is not null)
                {
                    List<List<ArgumentValue>> argumentValues = [];
                    foreach (IDictionary<string, object> rows in _gridSelectedData)
                    {
                        List<ArgumentValue> argumentValue = [];
                        foreach (KeyValuePair<string, object> data in rows)
                        {
                            argumentValue.Add(ArgumentValue.CreateArgumentValue(data.Key, data.Value, ""));
                        }
                        argumentValues.Add(argumentValue);

                    }
                    _ = rv.SetArgumentDataset("パレットNOArray", argumentValues);
                }

                // 追加情報を登録する
                foreach (KeyValuePair<string, object> item in _Arguments)
                {
                    _ = rv.SetArgumentValue(item.Key, item.Value, "");
                }

                // WebAPIへアクセス
                retb = true;
                ExecResult[]? results = await WebComService.SetRequestValue(this.GetType().Name, rv);
                if (results == null)
                {
                    // 実行結果がnullの場合は異常
                    ComService!.ShowNotifyMessege(ToastType.Error, $"{strSummary}", "WebAPIへのアクセスが異常終了しました。ログを確認して下さい。");
                    return;
                }
                else
                {
                    // 実行結果が返った場合
                    lstResult = new List<ExecResult>(results);
                }
            }

            // 実行結果を異常・正常・確認に分ける
            List<ExecResult> lstError = lstResult.Where(_ => _.RetCode < 0).OrderBy(_ => _.ExecOrderRank).ToList();
            List<ExecResult> lstSuccess = lstResult.Where(_ => _.RetCode == 0).OrderBy(_ => _.ExecOrderRank).ToList();
            List<ExecResult> lstConfirm = lstResult.Where(_ => _.RetCode > 0).OrderBy(_ => _.ExecOrderRank).ToList();

            if (lstError.Count() > 0)
            {
                // 異常結果がある場合
                retb = false;

                // 異常メッセージを全て通知
                foreach (ExecResult result in lstError)
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, $"{strSummary}", result.Message);
                }
            }
            else
            {
                // 異常結果が無い場合
                retb = true;

                // 正常結果のメッセージがある場合、全て通知
                foreach (ExecResult result in lstSuccess)
                {
                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        ComService!.ShowNotifyMessege(ToastType.Success, $"{strSummary}", result.Message);
                    }
                }

                // 確認結果がある場合、全ての確認ダイアログ表示
                foreach (ExecResult result in lstConfirm)
                {
                    bool? ret = await ComService.DialogShowYesNo(result.Message);
                    retb = ret is not null && (bool)ret;
                    if (!retb)
                    {
                        break;
                    }
                }
            }

            // 正常終了で結果メッセージがある場合は通知
            if (retb)
            {
                if (!string.IsNullOrEmpty(strResultMessage))
                {
                    ComService!.ShowNotifyMessege(ToastType.Success, $"{strSummary}", strResultMessage);
                }
            }
        }
        finally
        {
            // 入力エリアロック解除
            ComService.SetCompItemListDisabled(_inputItems, false);
        }

        // 正常終了ならダイアログ閉じる
        if (retb)
        {
            DialogService.CloseSide(retb);
        }
    }
}
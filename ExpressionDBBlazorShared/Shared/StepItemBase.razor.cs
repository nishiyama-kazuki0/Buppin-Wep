using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Util;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedModels;
using Sotsera.Blazor.Toaster.Core.Models;
using System.Reflection;
using static SharedModels.SharedConst;

namespace ExpressionDBBlazorShared.Shared;

public partial class StepItemBase : ChildPageBaseMobile
{
    protected const string STR_INIT_FOCUS_MARK = "InitFocusMark";

    [Inject]
    protected CustomForSuntoryService CstService { get; set; } = null!;

    /// <summary>
    /// ステップ管理UIコンポーネント
    /// </summary>
    [Parameter]
    public StepsExtend? StepsExtend { get; set; }

    /// <summary>
    /// 画面のViewModel
    /// </summary>
    [CascadingParameter(Name = "ViewModel")]
    protected BaseViewModel? PageVm { get; set; }

    /// <summary>
    /// DataGridコンポーネント
    /// </summary>
    protected DataGridExtend2? dataGrid;

    /// <summary>
    /// DataCardListコンポーネント
    /// </summary>
    protected DataCardList? cardLst;

    /// <summary>
    /// 倉庫ドロップダウン
    /// </summary>
    protected IList<ValueTextInfo> dropdownArea { get; set; } = [];

    /// <summary>
    /// ゾーンドロップダウン
    /// </summary>
    protected IList<ValueTextInfo> dropdownZone { get; set; } = [];

    /// <summary>
    /// ロケーションドロップダウン
    /// </summary>
    protected IList<ValueTextInfo> dropdownLocation { get; set; } = [];

    /// <summary>
    /// 倉庫ドロップダウン
    /// </summary>
    protected IList<ValueTextInfo> dropdownShelf { get; set; } = [];


    /// <summary>
    /// 倉庫マスタ情報
    /// </summary>
    protected List<MstAreaData> _lstMstArea = [];

    /// </summary>
    protected List<MstShelf> _lstMstShelf = [];
    /// <summary>
    /// ゾーンマスタ情報
    /// </summary>
    protected List<MstZoneData> _lstMstZone = [];

    /// <summary>
    /// ロケーションマスタ情報
    /// 西山19
    /// </summary>
    protected List<MstLocationData> _lstMstLocation = [];

   


    /// <summary>
    /// ロケーションIDからロケーション名を取得する
    /// </summary>
    /// <param name="locationId"></param>
    /// <returns></returns>
    protected string GetLocationName(string locationId)
    {
        return _lstMstShelf.SingleOrDefault(_ => _.ID == locationId)?.棚ID ?? string.Empty;
    }
    /// <summary>
    /// カードデータ
    /// </summary>
    protected List<IDictionary<string, DataCardListInfo>>? _cardValuesList { get; set; } = [];

    /// <summary>
    /// カード選択データ
    /// </summary>
    protected IList<IDictionary<string, DataCardListInfo>>? _cardSelectedData { get; set; }

    /// <summary>
    /// 初回フォーカスID
    /// 共通処理、バリデータ後にフォーカスを合わせるElementId
    /// </summary>
    protected string FirstFocusId = "";

    private StepItemMovePalletViewModel? model;

    public new async ValueTask DisposeAsync()
    {
        await JS.InvokeVoidAsync("removeHtStepKeyListener");
        await base.DisposeAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender)
        {
            return;
        }

        await JS.InvokeVoidAsync("removeHtStepKeyListener");
        DotNetObjectReference<StepItemBase> dotNetReference = DotNetObjectReference.Create(this);
        await JS!.InvokeVoidAsync("initializeHtStepKeyListener", dotNetReference);

        // 初期フォーカスに合わせる
        SetElementIdFocus(FirstFocusId);
    }

    [JSInvokable("OnKeyDownDataSelect")]
    public async void OnKeyDown(int keyCode)
    {
        //if (AssemblyState.Debug)
        //{
        //    NotificationService.Notify(new NotificationMessage()
        //    {
        //        Style = "position: absolute; bottom: -1000px;",
        //        Severity = ToastType.Error,
        //        Summary = "test",
        //        Detail = $"onkey : {keyCode}",
        //        Duration = 3000
        //    });
        //}

        await Task.Delay(0); //警告の抑制

        if (keyCode is 37 or 39)
        {
            // ← →
            if (cardLst is not null)
            {
                _ = keyCode is 37 ? cardLst.BackPage() : cardLst.NextPage();
            }
        }
        else if (keyCode is 38 or 40)
        {
            // ↑ ↓
            if (dataGrid is not null)
            {
                if (keyCode is 38)
                {
                    dataGrid.SelectUp();
                }
                else
                {
                    dataGrid.SelectDown();
                }
            }
        }
    }

    /// <summary>
    /// 倉庫、ゾーン、ロケーション情報を取得
    /// </summary>
    protected async Task InitComboAreaZoneLocation(bool area = true, bool zone = true, bool location = true, bool shelf = true)
    {
        List<Task> tl = [];
        if (area)
        {
            tl.Add(InvokeAsync(async () =>
            {
                _lstMstArea = await ComService.GetMstAreaInfoAll();
                SetDropdownArea();
                return;
            }));
        }
        if (zone)
        {
            tl.Add(InvokeAsync(async () =>
            {
                _lstMstZone = await ComService!.GetMstZoneInfoAll();
                SetDropdownZone();
                return;
            }));
        }
        if (location)
        {
            tl.Add(InvokeAsync(async () =>
            {
                _lstMstLocation = await ComService!.GetMstLocationInfoAll();
                SetDropdownLocation();
                return;
            }));
        }
        if (shelf)
        {
            tl.Add(InvokeAsync(async () =>
            {
                _lstMstShelf = await ComService!.GetMstShelfAll();
                SetDropdownShelf();
                return;
            }));
        }
        await Task.WhenAll(tl);
    }

    /// <summary>
    /// 倉庫コンボボックスの初期化
    /// </summary>
    protected void SetDropdownArea()
    {
        dropdownArea.Clear();
        foreach (MstAreaData item in _lstMstArea)
        {
            ValueTextInfo info = new()
            {
                Value = item.AreaId,
                Text = item.AreaName,
            };
            dropdownArea.Add(info);
        }
    }

    /// <summary>
    /// 棚コンボボックスの初期化
    /// </summary>
    protected void SetDropdownShelf(string shelfCd = "")
    {
        dropdownShelf.Clear();
        foreach (MstShelf item in _lstMstShelf)
        {
            ValueTextInfo info = new()
            {
                Value = item.ID,
                Text = item.棚ID,
            };
            dropdownShelf.Add(info);
        }
    }


    /// <summary>
    /// ゾーンコンボボックスの初期化
    /// </summary>
    /// <param name="areaCd"></param>
    protected void SetDropdownZone(string areaCd = "")
    {
        dropdownZone.Clear();
        List<MstZoneData> lstZone = [];
        if (!string.IsNullOrEmpty(areaCd))
        {
            lstZone = _lstMstZone.Where(_ => _.AreaId == areaCd).ToList();
        }
        foreach (MstZoneData item in lstZone)
        {
            ValueTextInfo info = new()
            {
                Value = item.棚ID,
                Text = item.ZoneName,
            };
            dropdownZone.Add(info);
        }
    }

    /// <summary>
    /// ロケーションコンボボックスの初期化
    /// </summary>
    protected void SetDropdownLocation(string areaCd = "", string zoneCd = "")
    {
        //dropdownLocation.Clear();
        //List<MstLocationData> lstLocation = [];
        //if (!string.IsNullOrEmpty(areaCd) && !string.IsNullOrEmpty(zoneCd))
        //{
        //    lstLocation = _lstMstLocation.Where(_ => _.AreaId == areaCd && _.ZoneId == zoneCd).ToList();
        //}
        //if (lstLocation.Count > _sysParams!.HT_LocComBoxMaxCount && DeviceInfo.IsHandy())
        //{
        //    //ロケーション数が多い場合は全てのロケを表示しない。かつHTの場合。
        //    return;
        //}
        //foreach (MstLocationData item in lstLocation)
        //{
        //    ValueTextInfo info = new()
        //    {
        //        Value = item.LocationId,
        //        Text = item.LocationName,
        //    };
        //    dropdownLocation.Add(info);
        //}

       
        List<MstShelf> lstLocation = [];
        if (!string.IsNullOrEmpty(areaCd) && !string.IsNullOrEmpty(zoneCd))
        {
            lstLocation = _lstMstShelf.Where(_ => _.ID == areaCd).ToList();
        }
        if (lstLocation.Count > _sysParams!.HT_LocComBoxMaxCount && DeviceInfo.IsHandy())
        {
            //ロケーション数が多い場合は全てのロケを表示しない。かつHTの場合。
            return;
        }
        foreach (MstShelf item in lstLocation)
        {
            ValueTextInfo info = new()
            {
                Value = item.ID,
                Text = item.棚ID,
            };
            dropdownLocation.Add(info);
        }
    }

    /// <summary>
    /// ゾーンマスタに存在しない場合のエラーメッセージ
    /// </summary>
    /// <param name="zoneCd"></param>
    protected async Task ShowNotExistZone(string zoneCd)
    {
        // DialogShowOKでは、F1でのOK押下時にMainLayoutのF1の処理も実行されてしまうため、Notifyで表示
        //await ComService.DialogShowOK($"ｿﾞｰﾝﾏｽﾀに存在しないｿﾞｰﾝCDです。", pageName);
        ComService!.ShowNotifyMessege(ToastType.Error, pageName, "ｿﾞｰﾝﾏｽﾀに存在しないｿﾞｰﾝCDです。");
        await Task.Delay(0);
    }

    /// <summary>
    /// ロケーションマスタに存在しない場合のエラーメッセージ
    /// 
    /// 西山
    /// 存在しない棚番の場合のエラーメッセージ変更
    /// </summary>
    /// <param name="zoneCd"></param>
    protected async Task ShowNotExistLocation(string locationCd)
    {
        // DialogShowOKでは、F1でのOK押下時にMainLayoutのF1の処理も実行されてしまうため、Notifyで表示
        //await ComService.DialogShowOK($"ﾛｹｰｼｮﾝﾏｽﾀに存在しないﾛｹｰｼｮﾝCDです。", pageName);
        ComService!.ShowNotifyMessege(ToastType.Error, pageName, "存在しない棚番です");
        await Task.Delay(0);
    }

    /// <summary>
    /// 混載情報取得
    /// </summary>
    /// <param name="strPalletNo"></param>
    /// <returns></returns>
    protected async Task<PalletInfo> GetPalletInfo(string strPalletNo)
    {
        try
        {
            PalletInfo ret = new();
            List<IDictionary<string, object>> datas = [];
            // ViewNameが指定されている場合はView名で値を取得
            ClassNameSelect select = new()
            {
                viewName = "VW_HT_パレット混載情報",
            };
            select.whereParam.Add("パレットNo", new WhereParam { val = strPalletNo, whereType = enumWhereType.Equal });
            datas = await ComService!.GetSelectData(select);
            if (null != datas && datas.Count > 0)
            {
                IDictionary<string, object> dic = datas.First();
                string str = ConvertUtil.GetValueString(dic, "混載");
                if (str == "1")
                {
                    ret.IsMixed = true;
                }
                str = ConvertUtil.GetValueString(dic, "引当済在庫");
                if (str == "1")
                {
                    ret.IsReserved = true;
                }
                ret.ReservedAlertTitle = ConvertUtil.GetValueString(dic, "引当済アラートタイトル");
                ret.ReservedAlertText = ConvertUtil.GetValueString(dic, "引当済アラートテキスト");
                ret.MixedAlertTitle = ConvertUtil.GetValueString(dic, "混載アラートタイトル");
                ret.MixedAlertText = ConvertUtil.GetValueString(dic, "混載アラートテキスト");
            }

            return ret;
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
            return new PalletInfo();
        }
    }

    /// <summary>
    /// 摘取ピック引当済み在庫チェック
    /// </summary>
    /// <returns></returns>
    protected async Task<(int, string)> GetHikiateZumiZaikoDelivery(string strPalletNo, string strDelivery)
    {
        try
        {
            int ret = 0;
            string id = string.Empty;
            List<IDictionary<string, object>> datas = [];
            ClassNameSelect select = new()
            {
                viewName = "VW_HT_摘取ピック_配送先_引当済在庫チェック",
            };
            select.whereParam.Add("パレットNo", new WhereParam { val = strPalletNo, whereType = enumWhereType.Equal });
            select.whereParam.Add("倉庫配送先コード", new WhereParam { val = strDelivery, whereType = enumWhereType.Equal });
            datas = await ComService!.GetSelectData(select);
            if (null != datas && datas.Count > 0)
            {
                IDictionary<string, object> dic = datas.First();
                ret = ConvertUtil.GetValueInt(dic, "引当済在庫数");//記載は引当済在庫数だが実際は引当て済みの在庫が存在する指示のカウントが入る（COUNT(出荷予定集約ID)の値）
                id = ConvertUtil.GetValueString(dic, "出荷予定集約ID");
            }
            return (ret, id);
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
            return (0, "");
        }
    }

    /// <summary>
    /// 摘取ピック引当済み在庫チェック
    /// </summary>
    /// <returns></returns>
    protected async Task<(int, string)> GetHikiateZumiZaiko(string strPalletNo)
    {
        try
        {
            int ret = 0;
            string id = string.Empty;
            List<IDictionary<string, object>> datas = [];
            ClassNameSelect select = new()
            {
                viewName = "VW_HT_摘取ピック_引当済在庫チェック",
            };
            select.whereParam.Add("パレットNo", new WhereParam { val = strPalletNo, whereType = enumWhereType.Equal });
            datas = await ComService!.GetSelectData(select);
            if (null != datas && datas.Count > 0)
            {
                IDictionary<string, object> dic = datas.First();
                ret = ConvertUtil.GetValueInt(dic, "引当済在庫数");
                id = ConvertUtil.GetValueString(dic, "出荷予定集約ID");
            }
            return (ret, id);
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
            return (0, "");
        }
    }

    /// <summary>
    /// コーナー別仕分引当済み在庫チェック
    /// </summary>
    /// <returns></returns>
    protected async Task<(int, string)> GetHikiateZumiZaikoCorner(string strPalletNo)
    {
        try
        {
            int ret = 0;
            string id = string.Empty;
            List<IDictionary<string, object>> datas = [];
            ClassNameSelect select = new()
            {
                viewName = "VW_HT_コーナー別仕分_引当チェック",
            };
            select.whereParam.Add("パレットNo", new WhereParam { val = strPalletNo, whereType = enumWhereType.Equal });
            datas = await ComService!.GetSelectData(select);
            if (null != datas && datas.Count > 0)
            {
                IDictionary<string, object> dic = datas.First();
                ret = ConvertUtil.GetValueInt(dic, "引当済在庫数");
                id = ConvertUtil.GetValueString(dic, "出荷予定集約ID");
            }
            return (ret, id);
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
            return (0, "");
        }
    }

    /// <summary>
    /// グリッドに表示するデータを取得しグリッドにセットする
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// </summary>
    /// <returns></returns>
    protected async Task<int> GetDataCount(string strViewName = "")
    {
        return await GetDataCount(WhereParamGet(), strViewName: strViewName);
    }
    /// <summary>
    /// ロケーションの数を取得。件数が多いとHTのコンボに値をセットするパフォーマンスが大変悪くなるため、絞る用途に使用。
    /// そもそも件数が多い場合はコンボで選ぶのか、という観点から実装。
    /// </summary>
    /// <param name="areaCd"></param>
    /// <param name="zoneCd"></param>
    /// <returns></returns>
    protected int GetCountLocationList(string areaCd = "", string zoneCd = "")
    {
        return _lstMstLocation.Count > 0 ? _lstMstLocation.Count(_ => _.AreaId == areaCd && _.ZoneId == zoneCd) : 0;
    }

    /// <summary>
    /// グリッドに表示するデータを取得しグリッドにセットする
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// </summary>
    /// <returns></returns>
    protected async Task LoadGridData(string strViewName = "")
    {
        await RefreshGridData(WhereParamGet(), strViewName: strViewName, bInitSelect: true);
    }

    /// <summary>
    /// グリッドに表示するデータを取得しグリッドにセットする
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// 
    /// 指定したキー、値と一致する行を初期選択させる
    /// </summary>
    /// <returns></returns>
    protected async Task LoadGridDataInitSel(string strViewName = "", string strInitSelectKey = "", string strInitSelectVal = "")
    {
        await RefreshGridDataInitSel(WhereParamGet(), strViewName: strViewName, bInitSelect: true, strInitSelectKey: strInitSelectKey, strInitSelectVal: strInitSelectVal);
    }

    /// <summary>
    /// カードリストに表示するデータを取得しカードリストにセットする
    /// DEFINE_COMPONENTSテーブルのAttributeNameにListCardHiddenで設定されている項目は表示対象外としています
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// </summary>
    /// <returns></returns>
    protected async Task LoadCardListData(string strViewName = "")
    {
        try
        {
            await LoadCardListData(WhereParamGet(), strViewName);
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
        }
    }

    /// <summary>
    /// カードリストに表示するデータを取得しカードリストにセットする
    /// DEFINE_COMPONENTSテーブルのAttributeNameにListCardHiddenで設定されている項目は表示対象外としています
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// </summary>
    /// <returns></returns>
    protected async Task LoadCardListData(Dictionary<string, WhereParam>? where, string strViewName = "")
    {
        await LoadCardListDataInitSel(where, strViewName);
    }

    /// <summary>
    /// カードリストに表示するデータを取得しカードリストにセットする
    /// DEFINE_COMPONENTSテーブルのAttributeNameにListCardHiddenで設定されている項目は表示対象外としています
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// 
    /// 初期選択データ指定用
    /// </summary>
    /// <returns></returns>
    protected async Task LoadCardListDataInitSel(string strViewName = "", string strInitSelectKey = "", string strInitSelectVal = "")
    {
        try
        {
            await LoadCardListDataInitSel(WhereParamGet(), strViewName, strInitSelectKey, strInitSelectVal);
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
        }
    }

    /// <summary>
    /// カードリストに表示するデータを取得しカードリストにセットする
    /// DEFINE_COMPONENTSテーブルのAttributeNameにListCardHiddenで設定されている項目は表示対象外としています
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// </summary>
    /// <returns></returns>
    protected async Task LoadCardListDataInitSel(Dictionary<string, WhereParam>? where, string strViewName = "", string strInitSelectKey = "", string strInitSelectVal = "")
    {
        try
        {
            _cardSelectedData = null;
            Dictionary<string, object> listcardHidden = (Dictionary<string, object>)GetAttributes(STR_LISTCARD_HIDDEN);
            ClassNameSelect select = new()
            {
                className = string.IsNullOrEmpty(strViewName) ? GetType().Name : string.Empty,
                viewName = string.IsNullOrEmpty(strViewName) ? string.Empty : strViewName,
                whereParam = where is null ? WhereParamGet() : where,
                orderByParam = OrderByParamGet()
            };
            List<IDictionary<string, object>> datas = await ComService!.GetSelectData(select);
            _cardValuesList = [];
            if (null != datas && datas.Count > 0)
            {
                foreach (IDictionary<string, object> rows in datas)
                {
                    Dictionary<string, DataCardListInfo> newCard = [];
                    foreach (KeyValuePair<string, object> row in rows)
                    {
                        bool hidden = false;
                        if (listcardHidden.ContainsKey(row.Key))
                        {
                            _ = listcardHidden.TryGetValue(row.Key, out object? obj);
                            if (obj is string)
                            {
                                string[]? vals = obj?.ToString().Split(":");
                                if (vals.Length > 1)
                                {
                                    _ = rows.TryGetValue(vals[0], out object? rowval);
                                    if (rowval.ToString() == vals[1])
                                    {
                                        hidden = true;
                                    }
                                }
                            }
                            else
                            {
                                hidden = ConvertUtil.GetValueBool(obj);
                            }
                        }
                        newCard[row.Key] = new DataCardListInfo { Value = ConvertUtil.GetValueString(row.Value), Visible = !hidden };
                    }
                    _cardValuesList.Add(newCard);
                }
            }
            if (null != cardLst)
            {
                int nInitPageIndex = 0;
                if (!string.IsNullOrEmpty(strInitSelectKey))
                {
                    IEnumerable<int> data = _cardValuesList.Select((item, index) => new { Index = index, Value = item }).Where(dict => dict.Value.ContainsKey(strInitSelectKey) && dict.Value[strInitSelectKey].Value == strInitSelectVal).Select(_ => _.Index);
                    if (null != data && data.Any())
                    {
                        nInitPageIndex = data.First();
                    }
                }
                cardLst.InitPageIndex = nInitPageIndex;
            }
            StateHasChanged();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
        }
    }

    /// <summary>
    /// カードリストに表示するデータを取得しカードリストにセットする
    /// DEFINE_COMPONENTSテーブルのAttributeNameにListCardHiddenで設定されている項目は表示対象外としています
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// </summary>
    /// <returns></returns>
    protected async Task<List<IDictionary<string, DataCardListInfo>>> GetCardListData(string strViewName = "", Dictionary<string, WhereParam>? where = null)
    {
        List<IDictionary<string, DataCardListInfo>> result = [];
        try
        {
            Dictionary<string, object> listcardHidden = (Dictionary<string, object>)GetAttributes(STR_LISTCARD_HIDDEN);
            ClassNameSelect select = new()
            {
                className = string.IsNullOrEmpty(strViewName) ? GetType().Name : string.Empty,
                viewName = string.IsNullOrEmpty(strViewName) ? string.Empty : strViewName,
                whereParam = where is null ? WhereParamGet() : where
            };
            List<IDictionary<string, object>> datas = await ComService!.GetSelectData(select);
            if (null != datas && datas.Count > 0)
            {
                foreach (IDictionary<string, object> rows in datas)
                {
                    Dictionary<string, DataCardListInfo> newCard = [];
                    foreach (KeyValuePair<string, object> row in rows)
                    {
                        bool hidden = false;
                        if (listcardHidden.ContainsKey(row.Key))
                        {
                            _ = listcardHidden.TryGetValue(row.Key, out object? obj);
                            if (obj is string)
                            {
                                string[]? vals = obj?.ToString().Split(":");
                                if (vals.Length > 1)
                                {
                                    _ = rows.TryGetValue(vals[0], out object? rowval);
                                    if (rowval.ToString() == vals[1])
                                    {
                                        hidden = true;
                                    }
                                }
                            }
                            else
                            {
                                hidden = ConvertUtil.GetValueBool(obj);
                            }
                        }
                        newCard[row.Key] = new DataCardListInfo { Value = ConvertUtil.GetValueString(row.Value), Visible = !hidden };
                    }
                    result.Add(newCard);
                }
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
        }
        return result;
    }

    /// <summary>
    /// DEFIME_COMPONENTSのCOMPONENT_NAMEが[ViewModelBind]に設定されている項目名から
    /// ViewModelに定義されている変数に値をセットする
    /// Viewで取得出来るデータは一件のみ取得できるものとする（複数件取れた場合は１件目をバインドさせる）
    /// View名を指定しない場合は、DEFINE_PAGE_VALUESにデータをセットしておく必要があります
    /// 
    /// 【DEFIME_COMPONENTSテーブルの決めごと】
    /// ATTRIBUTES_NAMEはViewで取得される名称とする
    /// VALUEはViewModelの変数名とする
    /// VALUE_OBJECT_TYPEは0:文字列とする
    /// </summary>
    /// <returns></returns>
    protected async Task<int> LoadViewModelBind(string strViewName = "")
    {
        try
        {
            // バインド用変数をクリアする
            ClearData();

            List<IDictionary<string, object>> datas = [];
            // ViewNameが指定されている場合はView名で値を取得
            ClassNameSelect select = new()
            {
                className = string.IsNullOrEmpty(strViewName) ? GetType().Name : string.Empty,
                viewName = string.IsNullOrEmpty(strViewName) ? string.Empty : strViewName,
                whereParam = WhereParamGet()
            };
            datas = await ComService!.GetSelectData(select);
            if (null != datas && datas.Count > 0)
            {
                Dictionary<string, object> attributes = (Dictionary<string, object>)GetAttributes(STR_VIEW_MODEL_BIND);
                Type? type = PageVm?.GetType();
                // 一件目のデータをViewModelのバインド変数をセットする
                IDictionary<string, object> dic = datas.First();
                foreach (KeyValuePair<string, object> att in attributes)
                {
                    if (dic.ContainsKey(att.Key))
                    {
                        PropertyInfo? p = type?.GetProperty(att.Value.ToString()!, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        p?.SetValue(PageVm, dic[att.Key].ToString());
                    }
                }
            }

            return datas is null ? 0 : datas.Count;
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
            return 0;
        }
    }

    /// <summary>
    /// グリッドで選択しているデータをViewModelの変数にバインドさせる
    /// 
    /// 【DEFIME_COMPONENTSテーブルの決めごと】
    /// ATTRIBUTES_NAMEはViewで取得される名称とする(グリッドの列名)
    /// VALUEはViewModelの変数名とする
    /// VALUE_OBJECT_TYPEは0:文字列とする
    /// </summary>
    /// <returns></returns>
    protected async Task GridSelectViewModelBind()
    {
        try
        {
            await Task.Delay(0);//警告の抑制
            if (null != _gridSelectedData && _gridSelectedData.Count > 0)
            {
                Dictionary<string, object> attributes = (Dictionary<string, object>)GetAttributes(STR_GRIDSELECT_VIEW_MODEL_BIND);
                Type? type = PageVm?.GetType();
                // 一件目のデータをViewModelのバインド変数をセットする
                IDictionary<string, object> grid = _gridSelectedData.First();
                foreach (KeyValuePair<string, object> att in attributes)
                {
                    if (grid.TryGetValue(att.Key, out object? sel))
                    {
                        PropertyInfo? p = type?.GetProperty(att.Value.ToString()!, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        p?.SetValue(PageVm, sel);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
        }
    }

    /// <summary>
    /// CardListで選択しているデータをViewModelの変数にバインドさせる
    /// 
    /// 【DEFIME_COMPONENTSテーブルの決めごと】
    /// ATTRIBUTES_NAMEはViewで取得される名称とする
    /// VALUEはViewModelの変数名とする
    /// VALUE_OBJECT_TYPEは0:文字列とする
    /// </summary>
    /// <returns></returns>
    protected async Task ListCardViewModelBind(string strViewName = "")
    {
        try
        {
            await Task.Delay(0);//警告の抑制
            if (null != _cardSelectedData && _cardSelectedData.Count > 0)
            {
                Dictionary<string, object> attributes = (Dictionary<string, object>)GetAttributes(STR_LISTCARD_VIEW_MODEL_BIND);
                Type? type = PageVm?.GetType();
                // 一件目のデータをViewModelのバインド変数をセットする
                IDictionary<string, DataCardListInfo> card = _cardSelectedData.First();
                foreach (KeyValuePair<string, object> att in attributes)
                {
                    if (card.TryGetValue(att.Key, out DataCardListInfo? sel))
                    {
                        PropertyInfo? p = type?.GetProperty(att.Value.ToString()!, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        p?.SetValue(PageVm, sel.Value);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
        }
    }

    /// <summary>
    /// 表示データクリア
    /// </summary>
    protected void ClearData()
    {
        Dictionary<string, object> attributes = (Dictionary<string, object>)GetAttributes(STR_VIEW_MODEL_BIND);
        Type? type = PageVm?.GetType();
        foreach (KeyValuePair<string, object> att in attributes)
        {
            PropertyInfo? p = type?.GetProperty(att.Value.ToString()!, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            p?.SetValue(PageVm, string.Empty);
        }
    }

    /// <summary>
    /// DEFIME_COMPONENTSのCOMPONENT_NAMEが[WhereParam][WhereParamCondition]に設定されている項目名から
    /// WhereParamを作成する
    /// 
    /// 【DEFIME_COMPONENTSテーブルの決めごと】
    /// ATTRIBUTES_NAMEはViewで取得される名称とする
    /// VALUEはViewModelの変数名とする
    /// VALUE_OBJECT_TYPEは0:文字列とする
    /// </summary>
    /// <param name="componentName"></param>
    /// <param name="componentNameCondition"></param>
    /// <returns></returns>
    protected Dictionary<string, WhereParam> WhereParamGet(string componentName = STR_WHERE_PARAM, string componentNameCondition = STR_WHERE_PARAM_CONDITION)
    {
        Dictionary<string, WhereParam> whereParam = [];
        Dictionary<string, object> attributes = (Dictionary<string, object>)GetAttributes(componentName);
        Dictionary<string, object> attributesCondition = (Dictionary<string, object>)GetAttributes(componentNameCondition);
        Type? type = PageVm?.GetType();
        foreach (KeyValuePair<string, object> att in attributes)
        {
            PropertyInfo? p = type?.GetProperty(att.Value.ToString()!, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (p != null)
            {
                object? value = p.GetValue(PageVm);
                if (!string.IsNullOrEmpty(value?.ToString()))
                {
                    WhereParam param = new() { val = value?.ToString()!, whereType = enumWhereType.Equal };
                    if (attributesCondition.TryGetValue(att.Key, out object? whereType) && whereType is enumWhereType)
                    {
                        param.whereType = (enumWhereType)whereType;
                    }
                    whereParam.Add(att.Key, param);
                }
            }
        }
        return whereParam;
    }

    /// <summary>
    /// DEFIME_COMPONENTSのCOMPONENT_NAMEが[LocalStrageGet]に設定されている項目名から
    /// LocalStorageから値を取得しViewModelに値をセットする
    /// 
    /// 【DEFIME_COMPONENTSテーブルの決めごと】
    /// ATTRIBUTES_NAMEはViewで取得される名称とする
    /// VALUEはViewModelの変数名とする
    /// VALUE_OBJECT_TYPEは0:文字列とする
    /// </summary>
    /// <param name="componentName"></param>
    /// <returns></returns>
    protected async Task LocalStreageGet(string componentName = STR_LOCAL_STORAGE_GET)
    {
        try
        {
            Dictionary<string, object> attributes = (Dictionary<string, object>)GetAttributes(componentName);
            Type? type = PageVm?.GetType();
            foreach (KeyValuePair<string, object> att in attributes)
            {
                string param = await LocalStorage.GetItemAsync<string>(att.Key.ToString());
                if (param != null)
                {
                    PropertyInfo? p = type?.GetProperty(att.Value.ToString()!, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    p?.SetValue(PageVm, param);
                }

                // 取得したキーはクリアする
                await LocalStorage.RemoveItemAsync(att.Key);
            }
            // ViewModelに展開した後
            //await LocalStorage.ClearAsync();
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
        }
    }

    protected void SetElementIdFocus(string id)
    {
        dynamic window = _js!.GetWindow();
        dynamic element = window.document.getElementById(id);
        element?.focus(); // カーソルを合わせる
    }

    protected void SetElementIdReFocus(string id)
    {
        dynamic window = _js!.GetWindow();
        dynamic element = window.document.getElementById(id);
        element?.blur();
        element?.focus(); // カーソルを合わせる
    }
    #region DEFINE_COMPONENT_PROGRAM呼び出しメソッド

    /// <summary>
    /// 確認ダイアログ表示
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task<bool> DialogShowYesNo(ComponentProgramInfo info)
    {
        bool ret = await base.DialogShowYesNo(info);
        if (!ret)
        {
            SetElementIdFocus(FirstFocusId);
        }
        return ret;
    }

    /// <summary>
    /// ログアウト
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task<bool> ログアウト(ComponentProgramInfo info)
    {
        bool ret = await ログアウト(info);
        if (!ret)
        {
            SetElementIdFocus(FirstFocusId);
        }
        return ret;
    }

    /// <summary>
    /// バリデート
    /// </summary>
    /// <returns></returns>
    public override bool バリデートチェック(ComponentProgramInfo info)
    {
        bool ret = base.バリデートチェック(info);
        if (!ret)
        {
            SetElementIdFocus(FirstFocusId);
        }
        return ret;
    }

    /// <summary>
    /// ストアドデータ設定_選択データ
    /// </summary>
    /// <returns></returns>
    public override async Task ストアドデータ設定_引数データ作成(ComponentProgramInfo info)
    {
        try
        {
            await base.ストアドデータ設定_引数データ作成(info);

            Dictionary<string, object> dlgParam = new(GetAttributes(info.ComponentName));
            if (PageVm is not null && dlgParam is not null)
            {
                foreach (KeyValuePair<string, object> param in dlgParam)
                {
                    object? value = null;
                    // 変数文字列から変数を取得
                    PropertyInfo? pi = PageVm.GetType().GetProperty(param.Value.ToString()!, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (pi != null)
                    {
                        value = pi.GetValue(PageVm, null);
                    }
                    else
                    {
                        FieldInfo? fi = PageVm.GetType().GetField(param.Value.ToString()!, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (fi != null)
                        {
                            value = fi.GetValue(PageVm);
                        }
                    }
                    if (value != null)
                    {
                        _storedData![param.Key] = value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
        }
    }

    /// <summary>
    /// ストアド呼び出し
    /// </summary>
    /// <returns></returns>
    public override async Task<bool> ストアド呼び出し(ComponentProgramInfo info)
    {
        bool ret = await base.ストアド呼び出し(info);
        if (!ret)
        {
            SetElementIdFocus(FirstFocusId);
        }


        
        return ret;
    }



    /// <summary>
    /// ローカルストレージにデータをセットする
    /// 
    /// DEFIME_COMPONENTSのCOMPONENT_NAMEが[LocalStrageSet]に設定されている項目名から
    /// LocalStorageに値をセットする
    /// 
    /// 【DEFIME_COMPONENTSテーブルの決めごと】
    /// ATTRIBUTES_NAMEはViewで取得される名称とする
    /// VALUEはViewModelの変数名とする
    /// VALUE_OBJECT_TYPEは0:文字列とする
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task 連携データ設定(ComponentProgramInfo info)
    {
        if (Attributes.ContainsKey(info.ComponentName))
        {
            try
            {
                Dictionary<string, object> attributes = (Dictionary<string, object>)GetAttributes(info.ComponentName);
                Type? type = PageVm?.GetType();
                foreach (KeyValuePair<string, object> att in attributes)
                {
                    PropertyInfo? p = type?.GetProperty(att.Value.ToString()!, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (p != null)
                    {
                        object? value = p.GetValue(PageVm);
                        await LocalStorage.SetItemAsync(att.Key, value?.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
            }
        }
    }

    /// <summary>
    /// LocalStreageからデータ取得
    /// 
    /// DEFIME_COMPONENTSのCOMPONENT_NAMEが[LocalStrageGet]に設定されている項目名から
    /// LocalStorageから値を取得し_streageDataに値をセットする
    /// 取得したキーは取得した時点で削除する
    /// 
    /// 【DEFIME_COMPONENTSテーブルの決めごと】
    /// ATTRIBUTES_NAMEは連携データ設定で作成した名称とする
    /// VALUEは使用しない
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task 連携データ取得(ComponentProgramInfo info)
    {
        if (Attributes.ContainsKey(info.ComponentName))
        {
            try
            {
                Dictionary<string, object> attributes = (Dictionary<string, object>)GetAttributes(info.ComponentName);
                Type? type = PageVm?.GetType();
                foreach (KeyValuePair<string, object> att in attributes)
                {
                    string param = await LocalStorage.GetItemAsync<string>(att.Key.ToString());
                    if (param != null)
                    {
                        PropertyInfo? p = type?.GetProperty(att.Value.ToString()!, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                        p?.SetValue(PageVm, param);
                    }

                    // 取得したキーはクリアする
                    await LocalStorage.RemoveItemAsync(att.Key);
                }
            }
            catch (Exception ex)
            {
                _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
            }
        }
    }

    /// <summary>
    /// 前ステップへ
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task 前ステップへ(ComponentProgramInfo info)
    {
        if (StepsExtend is not null)
        {
            await StepsExtend!.PrevStep();
        }
    }

    /// <summary>
    /// 次ステップへ
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public override async Task 次ステップへ(ComponentProgramInfo info)
    {
        if (StepsExtend is not null)
        {
            await StepsExtend!.NextStep();
        }
    }

    #endregion


    /// <summary>
    /// 出庫用データの設定
    /// 
    /// パレットピック、摘取ピックに戻る際のストレージ情報削除も本処理で行うように変更
    /// bFirstRirekiがtrueの場合は本処理を呼び出し後、FirstRirekiのurlに遷移するものとします
    /// 現在、残格納遷移時に呼び出している箇所はbFirstRirekiにfalseを指定して、処理しないようにしています
    /// </summary>
    /// <returns></returns>
    protected async Task ShipInfoLocalStorage(bool bFirstRireki = true)
    {
        string val = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_SHIP_DELIVERY_ID);
        if (!string.IsNullOrEmpty(val))
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID, val);
        }
        val = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_SHIP_AREA_ID);
        if (!string.IsNullOrEmpty(val))
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID, val);
        }
        val = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_SHIP_ZONE_ID);
        if (!string.IsNullOrEmpty(val))
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID, val);
        }

        if (bFirstRireki && PageVm is not null)
        {
            _ = PageVm.FirstRireki;
            string className = PageVm.FirstRireki;
            //if (className.Equals(typeof(StepItemPickingTargetSelectZone).Name))
            //{
            //    // パレットピック【倉庫別】－ ゾーン選択
            //    PageVm.PickPlanFlag = await CheckPickPlans();
            //    if (PageVm.PickPlanFlag.Item1)
            //    {
            //        // ピック予定が存在するので、ストレージに保持している情報はそのままとする
            //    }
            //    else
            //    {
            //        if (PageVm.PickPlanFlag.Item2)
            //        {
            //            // ゾーンに予定がある場合は、ゾーン選択画面に遷移させるためストレージに保持しているゾーンを削除して再度選択させる
            //            _ = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);
            //        }
            //        else
            //        {
            //            // 倉庫選択画面に遷移させるためストレージに保持している倉庫、ゾーンを削除して再度選択させる
            //            _ = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID);
            //            _ = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);
            //        }
            //    }
            //}
            //else if (className.Equals(typeof(StepItemPickingTargetSelectByDeliveryZone).Name) ||
            //    className.Equals(typeof(StepItemPickingTargetSelectItemByDeliveryZone).Name))
            //{
            //    // パレットピック【倉庫配送先別】－ ゾーン選択
            //    // 摘取ピック【倉庫配送先別】－ ゾーン選択

            //    PageVm.PickPlanFlag = await CheckPickPlans(true);
            //    if (PageVm.PickPlanFlag.Item1)
            //    {
            //        // ピック予定が存在するので、ストレージに保持している情報はそのままとする
            //    }
            //    else
            //    {
            //        if (PageVm.PickPlanFlag.Item2)
            //        {
            //            // ゾーンに予定がある場合は、ゾーン選択画面に遷移させるためストレージに保持しているゾーンを削除して再度選択させる
            //            _ = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);
            //        }
            //        else if (PageVm.PickPlanFlag.Item3)
            //        {
            //            // 倉庫選択画面に遷移させるためストレージに保持している倉庫、ゾーンを削除して再度選択させる
            //            _ = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID);
            //            _ = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);
            //        }
            //        else
            //        {
            //            // 倉庫配送先選択画面に遷移させるためストレージに保持している倉庫配送先、倉庫、ゾーンを削除して再度選択させる
            //            _ = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID);
            //            _ = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID);
            //            _ = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);
            //        }
            //    }
            //}
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>戻り値:ピック予定存在フラグ、ゾーン予定存在フラグ、倉庫予定存在フラグ、倉庫配送先予定存在フラグ</returns>
    public async Task<(bool, bool, bool, bool)> CheckPickPlans(bool bDelivery = false)
    {
        bool bPickPlan = false;
        bool bZonePlan = false;
        bool bAreaPlan = false;

        string? DeliveryCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID);
        string? AreaCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID);
        string? ZoneCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);

        //List<PickPlansInfo>? lst = await ComService!.GetPickPlansInfoAsync();
        List<PickPlansInfo>? lst = await CstService!.GetPickPlansInfoAsync();
        if (null != lst)
        {
            if (!bDelivery)
            {
                if (lst.Any(_ => _.AreaCd == AreaCd && _.ZoneCd == ZoneCd))
                {
                    // 倉庫コード、ゾーンコードが一致する予定が存在する場合は、予定一覧へ遷移
                    bPickPlan = true;
                }
                else if (lst.Any(_ => _.AreaCd == AreaCd))
                {
                    // 倉庫コードが一致する予定が存在する場合は、ゾーン選択画面へ遷移
                    bZonePlan = true;
                }
            }
            else
            {
                if (lst.Any(_ => _.DeliveryCd == DeliveryCd && _.AreaCd == AreaCd && _.ZoneCd == ZoneCd))
                {
                    // 倉庫配送先、倉庫コード、ゾーンコードが一致する予定が存在する場合は、予定一覧へ遷移
                    bPickPlan = true;
                }
                else if (lst.Any(_ => _.DeliveryCd == DeliveryCd && _.AreaCd == AreaCd))
                {
                    // 倉庫配送先、倉庫コードが一致する予定が存在する場合は、ゾーン選択画面へ遷移
                    bZonePlan = true;
                }
                else if (lst.Any(_ => _.DeliveryCd == DeliveryCd))
                {
                    // 倉庫配送先が一致する予定が存在する場合は、倉庫選択画面へ遷移
                    bAreaPlan = true;
                }
            }
        }

        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID, DeliveryCd);
        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID, AreaCd);
        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID, ZoneCd);

        return (bPickPlan, bZonePlan, bAreaPlan, false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>戻り値:ピック予定存在フラグ、ゾーン予定存在フラグ、倉庫予定存在フラグ、倉庫配送先予定存在フラグ</returns>
    public async Task<(bool, bool, bool, bool)> CheckPickPlansDelivery()
    {
        bool bPickPlan = false;
        bool bZonePlan = false;
        bool bAreaPlan = false;

        string? DeliveryCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID);
        string? AreaCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID);
        string? ZoneCd = await ComService.GetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID);

        //List<PickPlansInfo>? lst = await ComService!.GetPickPlansInfoAsync();
        List<PickPlansInfo>? lst = await CstService!.GetPickPlansInfoAsync();
        if (null != lst)
        {
            if (lst.Any(_ => _.DeliveryCd == DeliveryCd && _.AreaCd == AreaCd && _.ZoneCd == ZoneCd))
            {
                // 倉庫配送先、倉庫コード、ゾーンコードが一致する予定が存在する場合は、予定一覧へ遷移
                bPickPlan = true;
            }
            else if (lst.Any(_ => _.DeliveryCd == DeliveryCd && _.AreaCd == AreaCd))
            {
                // 倉庫配送先、倉庫コードが一致する予定が存在する場合は、ゾーン選択画面へ遷移
                bZonePlan = true;
            }
            else if (lst.Any(_ => _.DeliveryCd == DeliveryCd))
            {
                // 倉庫配送先が一致する予定が存在する場合は、倉庫選択画面へ遷移
                bAreaPlan = true;
            }
        }

        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID, DeliveryCd);
        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_AREA_ID, AreaCd);
        await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_ZONE_ID, ZoneCd);

        return (bPickPlan, bZonePlan, bAreaPlan, false);
    }

    /// <summary>
    /// パレット残在庫チェック
    /// </summary>
    /// <returns></returns>
    protected async Task<List<PickPlansInfo>?> GetPickPlans()
    {
        try
        {
            List<PickPlansInfo> datas = [];
            //datas = await ComService!.GetPickPlansInfoAsync();
            datas = await CstService!.GetPickPlansInfoAsync();
            return datas;
        }
        catch (Exception ex)
        {
            _ = WebComService.PostLogAsync(ex.Message, TYPE_LOGGER.FATAL);
            ComService!.ShowNotifyMessege(ToastType.Error, pageName, ex.Message);
            return null;
        }
    }
}

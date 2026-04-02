using System.Data;
using Blazored.SessionStorage;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Pages;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Shared;
using ExpressionDBBlazorShared.Util;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen.Blazor;
using SharedModels;
using Sotsera.Blazor.Toaster.Core.Models;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.Bibliography;
using ZXing.Common;
using ZXing;
using System.Text.RegularExpressions;
using static SharedModels.SharedConst;
using System;
using static ExpressionDBBlazorShared.Shared.MainLayout;




namespace ExpressionDBBlazorShared.Pages
{
    /// <summary>
    /// パレット移動/パレット№読取
    /// </summary>
    public partial class StepInventoryWork : StepItemBase
    {
        private StepItemMovePalletViewModel? model;
        LoginInfo[]? allUser = null;


     

        #region override

        private CompItemInfo? compReceptionNo;


        // 列名（DB の実カラム名に合わせて修正可）
        private const string COL_MNG_NO = "管理番号";
        private const string COL_SHELF = "棚ID";
        private const string COL_SHIPMENT = "状態";
        private const string COL_DATATIME = "日時";

        // 同画面内でのキャッシュ（全件取得結果を保持）
        private List<IDictionary<string, object>>? _itemStorageCache;

        // 同画面内でのキャッシュ（全件取得結果を保持）
        private List<IDictionary<string, object>>? _itemStorageCache2;

      

        
        /// <summary>
        /// null/DBNull/空白/大小文字差を吸収して比較するヘルパー
        /// </summary>
        private static bool EqualStr(object? o, string target)
        {
            var s = (o == null || o is DBNull) ? "" : o.ToString() ?? "";
            return string.Equals(s.Trim(), (target ?? "").Trim(), StringComparison.OrdinalIgnoreCase);
        }


        protected override Task OnAfterRenderAsync(bool firstRender)
        {


            model = (StepItemMovePalletViewModel?)PageVm;

            //出庫済の物品は表示したくないのでWhereで「入庫済」のものしか表示しないようにする
            model!.状態 = "入庫済";


            

            FirstFocusId = "PalletNo";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// OnCellRenderのCallBack
        /// </summary>
        /// <param name="args"></param>
        private new void CellRender(DataGridCellRenderEventArgs<IDictionary<string, object>> args)
        {
            try
            {

                
                // NOTIFY_CATEGORYの値によって区分の背景色変更
                if (args.Data.TryGetValue("棚卸日時", out object? value))
                {
                    ComService.AddAttrResultCatgory2(value?.ToString(), args.Attributes);
                }
                


            }
            catch (Exception)
            {
            }
        }


        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {

            //model = (StepItemMovePalletViewModel?)PageVm;

            //if (model != null)
            //{
            //    // 当日の日付をセット（形式は必要に応じて変更）
            //    model.保管開始日 = DateTime.Today.ToString("yyyy/MM/dd");
            //}

            //await base.OnInitializedAsync();


            ClassNameSelect select = new()
            {
                viewName = "VW_ログイン情報"
            };



            // 初期処理呼び出し
            await InitProcAsync();

        }

       
        // 読取った管理番号の一致する行データを返す
        private async Task<IDictionary<string, object>?> ITEM_STORAGE_ROW(string managementNo, bool forceReload = false)
        {
            managementNo = (managementNo ?? "").Trim();

            if (_itemStorageCache == null || forceReload)
            {
                ClassNameSelect select = new()
                {
                    viewName = "ITEM_STORAGE",
                };
                _itemStorageCache = await ComService!.GetSelectData(select);
            }

            var datas = _itemStorageCache ?? new();

            var row = datas
                .Where(r => r.ContainsKey(COL_MNG_NO))
                .FirstOrDefault(r => EqualStr(r[COL_MNG_NO], managementNo));

            return row; // 見つからなければ null、見つかれば行をそのまま返す
        }



        private async Task<IDictionary<string, object>?> MST_SHELF_ROW(string managementNo, bool forceReload = false)
        {
            managementNo = (managementNo ?? "").Trim();

            if (_itemStorageCache == null || forceReload)
            {
                ClassNameSelect select = new()
                {
                    viewName = "MST_SHELF",
                };
                _itemStorageCache = await ComService!.GetSelectData(select);
            }

            var datas = _itemStorageCache ?? new();

            var row = datas
                .Where(r => r.ContainsKey(COL_SHELF))
                .FirstOrDefault(r => EqualStr(r[COL_SHELF], managementNo));

            return row; // 見つからなければ null、見つかれば行をそのまま返す
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

                

                string manegementNo =  (value ?? string.Empty).Trim();
                string Shelf = (model!.棚番 ?? string.Empty).Trim();

                model!.管理番号 = manegementNo;


                var row = await ITEM_STORAGE_ROW(manegementNo);
                var row2 = await MST_SHELF_ROW(Shelf);

                if(row2 != null)
                {
                     model!.場所  = row2.TryGetValue("場所", out var loc)
                       ? (loc?.ToString() ?? "").Trim()
                       : "";

                     model!.階  = row2.TryGetValue("階", out var flo)
                      ? (flo?.ToString() ?? "").Trim()
                      : "";

                    
                }





                if (row != null)
                {

                    //ITEM_STORAGEテーブルで保存されているデータ
                    // 例：管理番号、棚、品名、数量、社員コードなどをモデルに詰める
                    string manegementNo2 = row.TryGetValue("管理番号", out var mng)
                        ? (mng?.ToString() ?? "").Trim()
                        : "";


                    string ShelfID = row.TryGetValue("棚", out var shelfVal)
                        ? (shelfVal?.ToString() ?? "").Trim()
                        : "";

                    string SHIPMENT = row.TryGetValue("状態", out var emp)
                        ? (emp?.ToString() ?? "").Trim()
                        : "";

                    model!.仕掛番号 = row.TryGetValue("仕掛番号", out var si)
                     ? (si?.ToString() ?? "").Trim()
                     : "";


                    if (ShelfID == model!.棚番 && SHIPMENT == "入庫済")
                    {


                        //F5のボタン機能がほしいのでこの処理を行うときだけ表示して
                        //処理が終われば処理を非表示に戻す
                        //今はこれで行く
                        Attributes[STR_ATTRIBUTE_FUNC]["button5text"] = "";
                        Attributes[STR_ATTRIBUTE_FUNC]["IconName5"] = "eraser_size1";
                        UpdateFuncButton(Attributes[STR_ATTRIBUTE_FUNC]);
                        //管理番号と入庫している棚が一致＋入庫されている状態なら
                        await ContainerMainLayout!.ButtonClickF5();
                        Attributes[STR_ATTRIBUTE_FUNC]["button5text"] = "";
                        Attributes[STR_ATTRIBUTE_FUNC]["IconName5"] = "";
                        UpdateFuncButton(Attributes[STR_ATTRIBUTE_FUNC]);
                        ComService!.ShowNotifyMessege(ToastType.Info, "完了", $" 管理番号「{model!.管理番号}」の物品は棚卸完了しました");
                        _ = LoadGridData();

                    }
                    else if (SHIPMENT == "入庫済" && model!.管理番号 == manegementNo && ShelfID != model!.棚番)
                    {
                        //管理番号は存在して入庫されている場合＋棚番が一致してない場合
                        //移動するかの選択
                        bool ret = await ComService!.DialogShowYesNo($"{ShelfID}の棚に存在しますが\n{model!.棚番}に移動しますか？");
                        //「はい」を選択した場合
                        if (ret)
                        {
                            //現在棚卸している棚を移動先棚の変数として使う
                            model!.元棚 = ShelfID;
                            model!.移動先棚 = model!.棚番;
                         
                            //この処理も上記と一緒
                            Attributes[STR_ATTRIBUTE_FUNC]["button6text"] = "";
                            Attributes[STR_ATTRIBUTE_FUNC]["IconName6"] = "eraser_size1";
                            UpdateFuncButton(Attributes[STR_ATTRIBUTE_FUNC]);
                            await ContainerMainLayout!.ButtonClickF6();
                            Attributes[STR_ATTRIBUTE_FUNC]["button6text"] = "";
                            Attributes[STR_ATTRIBUTE_FUNC]["IconName6"] = "";
                            UpdateFuncButton(Attributes[STR_ATTRIBUTE_FUNC]);
                            ShelfID = "";
                            _ = LoadGridData();

                            //Clearにすると再取得されず前回のデータが残るので
                            //データがからのまま再取得されない
                            //一度nullにして再取得をしてもらう
                            _itemStorageCache = null;

                        }
                        //「いいえ」を選択した場合
                        if (true != ret)
                        {
                            return;
                        }


                    }
                    else if (ShelfID == "" && model!.管理番号 == manegementNo2)
                    {
                        //管理番号が存在しても物品がまだ入庫されていない場合
                        ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $" この物品はまだ入庫が完了していません");
                    }
                    else if (SHIPMENT == "出庫済" && model!.管理番号 == manegementNo2)
                    {
                        //管理番号に対してその物品が出庫済の場合
                        ComService!.ShowNotifyMessege(ToastType.Error, "エラー", $"この物品は出庫済みです");
                    }
                    else
                    {
                        // 
                        ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $" {SHIPMENT}{manegementNo2}{ShelfID}");//ココはもうちょい分岐を考える


                    }

                }
                else
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, "未検出", "存在しない管理番号です");
                }


            }
            else if (IsLocationBarcode(value))
            {
                // 見つからない場合
                ComService!.ShowNotifyMessege(ToastType.Error, "エラー", $" ラベルのバーコードを読み込んでください");

            }
        }


        /// <summary>
        /// 確定
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F1画面遷移(ComponentProgramInfo info)
        {


            await 次ステップへ(info);


        }

        public override async Task F2画面遷移(ComponentProgramInfo info)//F2クリック時イベント
        {
            await Task.Delay(0);
        }
        /// <summary>
        /// 戻る
        /// 一個前のページに戻る
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
                    string uri = model!.GetFirstRirekiUrl();

                    if (string.IsNullOrWhiteSpace(uri)
                        || (uri == "mobile_inventory_work" && ClassName.Equals(typeof(StepShelfSelection).Name))
                        )//ﾋﾟｯｸボタンを押下して、さらに自分自身に戻ってくる場合は画面遷移が効かないので修正
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
                    await 前ステップへ(info);
                }
            }
            catch (Exception ex)
            {
                //何かエラーが発生した場合は強制的に前ステップとする
                _ = ComService.DialogShowOK(ex.Message);
                await 前ステップへ(info);
            }
        }


        //public override async Task<bool> 確定前チェック(ComponentProgramInfo info)////return falsにすると確定後の処理が行われない
        //{
            

        public override async Task<bool> 確定後処理(ComponentProgramInfo info)
        {
            
            
            await Task.Delay(0);
            
          
            return false;


        }


        public override async Task<bool> 確定後処理2(ComponentProgramInfo info)
        {


            await Task.Delay(0);

            await F4画面遷移(info);
            return false;


        }

        //管理番号の引数データを作成
        public override async Task 管理番号引数データ作成(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            _storedData = new Dictionary<string, object>();
            //
            {
                string stName = "管理番号";
                _storedData[stName] = model!.管理番号;
                stName = "元棚";
                _storedData[stName] = model!.元棚;
                stName = "移動先棚";
                _storedData[stName] = model!.移動先棚;

            

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



        #endregion


        #region 棚卸の管理番号の引数設定


        #endregion

        #region Event

        /// <summary>
        /// パレットNoの入力イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangePalletNo(object value)
        {
            model!.PalletNo = (string)value;




            await Task.Delay(0);
            StateHasChanged();
        }

        /// <summary>
        /// 倉庫の選択イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task 棚(object value)
        {
            await Task.Delay(0);

            model!.棚番 = (string)value;
           
            _ = LoadGridData();
        }

        private async Task 社員コード(object value)//ログインしている社員情報を入力する
        {


            await Task.Delay(0);
            StateHasChanged();
        }

        private async Task 管理責任者N(object value)//ログインしている社員情報を入力する
        {


            await Task.Delay(0);
            StateHasChanged();
        }



        private async Task 備考(object value)
        {


            await Task.Delay(0);
            StateHasChanged();
        }
        /// <summary>
        /// ゾーンの選択イベント
        /// </summary>
        /// <param name="value"></param>
        private async void OnChangeZone(object value)
        {
            await Task.Delay(0);
            model!.LocationCd = string.Empty;
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);

            _ = LoadGridData();
        }

        #endregion

        #region private

        /// <summary>
        /// 初期処理
        /// </summary>
        private async Task InitProcAsync()
        {
            InitParam();

            // コンボボックス初期化
            await InitComboAreaZoneLocation();

            // コンボボックス更新
            SetDropdownZone(model!.AreaCd);
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);

            // 空きデータ一覧の読込
            //ここでデータ一覧を取得している
            _ = LoadGridData();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //他画面から移動してきた場合、他のボタンが残ったままになるでココで非表示処理する
            //他にやり方があると思うけど一旦この処理でいく
            //Attributes[STR_ATTRIBUTE_FUNC]["button5text"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["IconName5"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["button6text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName6"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["button7text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName7"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["button8text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName8"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["button9text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName9"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["button10text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName10"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["button11text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName11"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["button12text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName12"] = "";
            UpdateFuncButton(Attributes[STR_ATTRIBUTE_FUNC]);
        }

        /// <summary>
        /// ゾーンコードスキャン処理
        /// </summary>
        /// <param name="zoneCd"></param>
        private async Task ScanZoneCd(string zoneCd)
        {
            MstZoneData? infoZone = _lstMstZone.FirstOrDefault(_ => _.棚ID == zoneCd);
            if (infoZone != null)
            {
                model!.AreaCd = infoZone.AreaId;
                SetDropdownZone(model!.AreaCd);
                model!.ZoneCd = zoneCd;
                SetDropdownLocation();
                model!.LocationCd = string.Empty;
            }
            else
            {
                // エラーメッセージ
                await ShowNotExistZone(zoneCd);

                model!.ZoneCd = string.Empty;
                model!.LocationCd = string.Empty;
                SetDropdownLocation();
            }
        }

        /// <summary>
        /// ロケーションコードスキャン処理
        /// </summary>
        /// <param name="shelfCd"></param>
        private async Task ScanLocationCd(string shelfCd)
        {
            MstShelf? infoLocation = _lstMstShelf.SingleOrDefault(_ => _.ID == shelfCd);
            if (infoLocation != null)
            {
                model!.棚 = infoLocation.ID;
                
               
            }
            else
            {
                // エラーメッセージ
                await ShowNotExistLocation(shelfCd);

                model!.LocationCd = string.Empty;
            }
        }

      

        private string GetLocationName()
        {
            StepItemMovePalletViewModel? model = (StepItemMovePalletViewModel?)PageVm;
            return GetLocationName(model!.LocationCd);
        }
        #endregion
    }
}
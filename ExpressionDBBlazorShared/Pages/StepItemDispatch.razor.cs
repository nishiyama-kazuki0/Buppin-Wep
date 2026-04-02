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




namespace ExpressionDBBlazorShared.Pages
{
    /// <summary>
    /// パレット移動/パレット№読取
    /// </summary>
    public partial class StepItemDispatch : StepItemBase
    {
        private StepItemMovePalletViewModel? model;
        LoginInfo[]? allUser = null;

        Workbook Workbook = new Workbook();//Excel作成
        Worksheet sheet = new Worksheet();

        #region override

        private const string PROPKEY_ARRIVAL_NO = "PalletNo";
        private CompItemInfo? compReceptionNo;

        // 列名（DB の実カラム名に合わせて修正可）
        private const string COL_MNG_NO = "管理番号";
        private const string COL_CONDITION = "状態";

        // 同画面内でのキャッシュ（全件取得結果を保持）
        private List<IDictionary<string, object>>? _itemStorageCache;

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


            var user = MainLayout.LoginUserName;
            model!.管理責任者 = user;

            FirstFocusId = "PalletNo";
            return base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// 初期化後処理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task AfterInitializedAsync(ComponentProgramInfo info)
        {

            
            if (model != null)
            {
                // 当日の日付をセット（形式は必要に応じて変更）
                model.保管開始日 = DateTime.Today.ToString("yyyy/MM/dd");
            }

            


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


        /// <summary>
        /// HTスキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

        
            if (IsPalletBarcode(value))
            {

                string managementNo = (value ?? string.Empty).Trim();

                // 全件取得済みキャッシュから検索して棚を取得
                var row = await ITEM_STORAGE_ROW(managementNo);

                //データがあれば
                if (row != null)
                {

                    //ITEM_STORAGEテーブルで保存されているデータ
                    // 例：管理番号、棚、品名、数量、社員コードなどをモデルに詰める
                    string 管理番号 = row.TryGetValue("管理番号", out var mng)
                        ? (mng?.ToString() ?? "").Trim()
                        : "";


                    model!.棚 = row.TryGetValue("棚", out var shelfVal)
                        ? (shelfVal?.ToString() ?? "").Trim()
                        : "";

                    string 状態 = row.TryGetValue("状態", out var emp)
                        ? (emp?.ToString() ?? "").Trim()
                        : "";

                   

                    model!.仕掛番号 = row.TryGetValue("仕掛番号", out var si)
                        ? (si?.ToString() ?? "").Trim()
                        : "";



                    if (状態 == "入庫済")
                    {
                        await OnChangePalletNo(管理番号);
                    }
                    else if(状態 == "登録")
                    {
                        ComService!.ShowNotifyMessege(ToastType.Error, "出庫済", $"既に入庫済みです");
                        
                    }
                    else if (状態 == "出庫済")
                    {
                        ComService!.ShowNotifyMessege(ToastType.Error, "未入庫", $"まだ入庫が完了していません");
                       
                    }
                    

                }
                else
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $"存在しない管理番号です");
                    model!.PalletNo = "";
                }

            }
            else if (IsLocationBarcode(value))
            {
                // ロケーションID
                
                await ScanLocationCd(value);
                model!.棚 = value;
                _ = LoadGridData();
                SetElementIdFocus("棚ID");
            }
        }

        //Excelのセルに値を書き込むために引数データ作成
        public override async Task Excel発行(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            _storedData = new Dictionary<string, object>();
            //
            {
                string stName = "PalletNo";
                _storedData[stName] = model!.PalletNo;
                


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
            
        }
        /// <summary>
        /// 戻る
        /// 一個前のページに戻る
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override async Task F4画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);

            // パレット照会画面に遷移
            //下記のURLに移動
            NavigationManager.NavigateTo("mobile_menu");
        }


        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)////return falsにすると確定後の処理が行われない
        {
            await Task.Delay(0);
            if (string.IsNullOrEmpty(model!.PalletNo))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "管理番号は必須です");
                return false;
            }


            return !string.IsNullOrEmpty(model!.PalletNo);
                  
        }

        public override async Task<bool> 確定後処理(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            //出庫が完了したらクリアにする
            model!.PalletNo = "";
            return false;


        }

        #endregion


        #region Excel出力（OpenXML）追加


        #endregion

        #region Event

        /// <summary>
        /// パレットNoの入力イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangePalletNo(object value)
        {
            model!.PalletNo = (string)value;


            string managementNo = (model!.PalletNo ?? string.Empty).Trim();

            // 全件取得済みキャッシュから検索して棚を取得
            var row = await ITEM_STORAGE_ROW(managementNo);

            //データがあれば
            if (row != null)
            {

                //ITEM_STORAGEテーブルで保存されているデータ
                // 例：管理番号、棚、品名、数量、社員コードなどをモデルに詰める
                string 管理番号 = row.TryGetValue("管理番号", out var mng)
                    ? (mng?.ToString() ?? "").Trim()
                    : "";


                model!.棚 = row.TryGetValue("棚", out var shelfVal)
                    ? (shelfVal?.ToString() ?? "").Trim()
                    : "";

                string 状態 = row.TryGetValue("状態", out var emp)
                    ? (emp?.ToString() ?? "").Trim()
                    : "";

           
                model!.仕掛番号 = row.TryGetValue("仕掛番号", out var si)
                    ? (si?.ToString() ?? "").Trim()
                    : "";



                if (状態 == "入庫済")
                {
                    model!.PalletNo = 管理番号;
                }
                else if(状態 == "登録")
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, "未入庫", $"まだ入庫が完了していません");
                   
                }
                else if (状態 == "出庫済")
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, "出庫済", $"既に入庫済みです");
                    
                }
            }
            else
            {
                
                ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $"存在しない管理番号です");
                model!.PalletNo = "";
            }

            await Task.Delay(0);
            StateHasChanged();
        }
        



        private async Task 棚(object value)
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

            //// コンボボックス更新
            //SetDropdownZone(model!.AreaCd);
            //SetDropdownLocation(model!.AreaCd, model!.ZoneCd);

            // 空きデータ一覧の読込
            _ = LoadGridData();
        }

        /// <summary>
        /// パラメータ関連初期化
        /// </summary>
        private void InitParam()
        {
            //他画面から移動してきた場合、他のボタンが残ったままになるでココで非表示処理する
            //他にやり方があると思うけど一旦この処理でいく
            Attributes[STR_ATTRIBUTE_FUNC]["button5text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName5"] = "";
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
        /// <param name="locationCd"></param>
        private async Task ScanLocationCd(string locationCd)
        {
            MstShelf? infoLocation = _lstMstShelf.SingleOrDefault(_ => _.ID == locationCd);
            if (infoLocation != null)
            {
                model!.AreaCd = infoLocation.ID;
                SetDropdownZone(model!.AreaCd);
               
            }
            else
            {
                // エラーメッセージ
                await ShowNotExistLocation(locationCd);

                model!.LocationCd = string.Empty;
            }
        }


        #endregion
    }
}
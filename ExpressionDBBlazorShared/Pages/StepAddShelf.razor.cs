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




namespace ExpressionDBBlazorShared.Pages
{
    /// <summary>
    /// パレット移動/パレット№読取
    /// </summary>
    public partial class StepAddShelf : StepItemBase
    {
        private StepItemMovePalletViewModel? model;
        LoginInfo[]? allUser = null;

     

        #region override

        private CompItemInfo? compReceptionNo;


        // 列名（DB の実カラム名に合わせて修正可）
        private const string COL_MNG_NO = "管理番号";
        private const string COL_SHELF = "棚ID";

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

        //MST_SHELFテーブルの棚IDをチェックする
        /// </summary>
        private async Task<string?> MST_SHELf(string shelfNo, bool forceReload = false)
        {
            shelfNo = (shelfNo ?? "").Trim();

            // 1) キャッシュがなければ全件取得（または強制再取得）
            if (_itemStorageCache2 == null || forceReload)
            {
                ClassNameSelect select = new()
                {
                    viewName = "MST_SHELF",
                };

                _itemStorageCache2 = await ComService!.GetSelectData(select);
            }

             var datas = _itemStorageCache2 ?? new();

            //棚番が一致する行を1件取る
            //棚番も重複しないので応用する
            var row = datas
                .Where(r => r.ContainsKey(COL_SHELF))
                .FirstOrDefault(r => EqualStr(r[COL_SHELF], shelfNo));

            if (row == null) return null;


            // 3) 棚の値を返す（なければ null）
            return row.ContainsKey(COL_SHELF) ? row[COL_SHELF]?.ToString()?.Trim() : null;

            
        }

        //ITEM_STORAGEテーブルの管理番号をチェックする
        /// </summary>
        private async Task<string?> ITEM_STORAGE(string managementNo, bool forceReload = false)
        {
            managementNo = (managementNo ?? "").Trim();

            // 1) キャッシュがなければ全件取得（または強制再取得）
            if (_itemStorageCache == null || forceReload)
            {
                ClassNameSelect select = new()
                {
                    viewName = "ITEM_STORAGE",
                };

                _itemStorageCache = await ComService!.GetSelectData(select);
            }

            var datas = _itemStorageCache ?? new();

            //　管理番号は重複しない値なので
            //  管理番号が一致する行を1件取る
            //　　     
            var row = datas
                .Where(r => r.ContainsKey(COL_MNG_NO))
                .FirstOrDefault(r => EqualStr(r[COL_MNG_NO], managementNo));

            if (row == null) return null;

            // 3) 棚の値を返す（なければ null）
            return row.ContainsKey(COL_MNG_NO) ? row[COL_MNG_NO]?.ToString()?.Trim() : null;
        }


        /// <summary>
        /// HTスキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            string value = scanData.strStringData;

            bool HasAsciiAlpha(string s) => Regex.IsMatch(s ?? "", "[A-Za-z]");

           
            if (IsPalletBarcode(value))
            {

                string managementNo = (value ?? string.Empty).Trim();

                // 全件取得済みキャッシュから検索して棚を取得
                string? 管理番号 = await ITEM_STORAGE(managementNo);

                if (!string.IsNullOrEmpty(管理番号))
                {


                    model!.PalletNo = 管理番号;

                    _ = LoadGridData();
                    
                }
                else
                {
                    // 見つからない場合
                    ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $" 存在しない管理番号です");

                    // 必要に応じて棚値をクリア
                    model!.PalletNo = "";
                }

            }
            else if (IsLocationBarcode(value))
            {

                string shelfNo = (value ?? string.Empty).Trim();

                // 全件取得済みキャッシュから検索して棚を取得
                string? shelf = await MST_SHELf(shelfNo);

                if (!string.IsNullOrEmpty(shelf))
                {
                  

                    model!.棚 = shelf;

                    _ = LoadGridData();
                   
                }
                else
                {
                    // 見つからない場合
                    ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $" {shelfNo} 存在しない棚番です");

                    // 必要に応じて棚値をクリア
                     model!.棚 = "";
                }

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
                stName = "社員コード";
                _storedData[stName] = model!.社員コード;
                stName = "管理責任者";
                _storedData[stName] = model!.管理責任者;
                stName = "部署";
                _storedData[stName] = model!.部署;
                stName = "プロジェクト名";
                _storedData[stName] = model!.プロジェクト名;
                stName = "保管開始日";
                _storedData[stName] = model!.保管開始日;
                stName = "保管終了日";
                _storedData[stName] = model!.保管終了日;
                stName = "仕掛番号";
                _storedData[stName] = model!.仕掛番号;


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
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);

            // パレット照会画面に遷移
            //下記のURLに移動
            NavigationManager.NavigateTo("mobile_menu");
        }


        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)////return falsにすると確定後の処理が行われない
        {
            await Task.Delay(0); 
            if (string.IsNullOrEmpty(model!.棚番))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "棚番は必須です");
                
                return false;
            }
            if (string.IsNullOrEmpty(model!.場所))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "場所は必須です");
                return false;

            }

            if (string.IsNullOrEmpty(model!.階)) 
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "階は必須です");
                return false;

            }

            return !string.IsNullOrEmpty(model!.棚番) &&
                　 !string.IsNullOrEmpty(model!.場所) &&
                   !string.IsNullOrEmpty(model!.階);
        }

        public override async Task<bool> 確定後処理(ComponentProgramInfo info)
        {
            //string excelPath = "\"\\\\166.26.30.2\\users\\G2507\\5-3 保管_物品管理ラベル.xlsx\"";
            //WriteValueToExcel(excelPath);
            //XLWorkbook workbook = new XLWorkbook(@"""\\166.26.30.2\users\G2507\5-3 保管_物品管理ラベル.xlsx""");
            //XLWorkbook workbook = new XLWorkbook(WebAPIService.AppDataPath);


            await Task.Delay(0);
            
            return false;


        }


        public override async Task<bool> 確定後処理2(ComponentProgramInfo info)
        {
            //string excelPath = "\"\\\\166.26.30.2\\users\\G2507\\5-3 保管_物品管理ラベル.xlsx\"";
            //WriteValueToExcel(excelPath);
            //XLWorkbook workbook = new XLWorkbook(@"""\\166.26.30.2\users\G2507\5-3 保管_物品管理ラベル.xlsx""");
            //XLWorkbook workbook = new XLWorkbook(WebAPIService.AppDataPath);


            await Task.Delay(0);

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




            await Task.Delay(0);
            StateHasChanged();
        }


        /// <summary>
        /// 倉庫の選択イベント
        /// </summary>
        /// <param name="value"></param>
        private async void 部署N(object value)//ログインしている社員情報を入力する
        {


            await Task.Delay(0);
            model!.LocationCd = string.Empty;
            model!.ZoneCd = string.Empty;
            SetDropdownZone(model!.AreaCd);
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);

            _ = LoadGridData();
        }
        private async Task 場所(object value)
        {


            await Task.Delay(0);
            StateHasChanged();
        }

        private async Task 階(object value)
        {


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

            // コンボボックス更新
            SetDropdownZone(model!.AreaCd);
            SetDropdownLocation(model!.AreaCd, model!.ZoneCd);

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
            //Attributes[STR_ATTRIBUTE_FUNC]["button5text"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["IconName5"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["button6text"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["IconName6"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["button7text"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["IconName7"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["button8text"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["IconName8"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["button9text"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["IconName9"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["button10text"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["IconName10"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["button11text"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["IconName11"] = "";
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
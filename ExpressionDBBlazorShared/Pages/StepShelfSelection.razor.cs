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
    public partial class StepShelfSelection : StepItemBase
    {
        private StepItemMovePalletViewModel? model;
        LoginInfo[]? allUser = null;

     

        #region override

        private CompItemInfo? compReceptionNo;


        // 列名（DB の実カラム名に合わせて修正可）
        private const string COL_MNG_NO = "管理番号";
        private const string COL_SHELF = "棚ID";
        private const string COL_CONDITION = "状態";


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



            ClassNameSelect select = new()
            {
                viewName = "VW_ログイン情報"
            };

            //棚卸終了際　棚番の入力クリア
            model!.棚番 = "";
            // 初期処理呼び出し
            await InitProcAsync(); 

        }

        /// <summary>
        /// OnCellRenderのCallBack
        /// セルの情報を見てその
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


       

        //MST_SHELFテーブルの棚IDをチェックする
        /// </summary>
        private async Task<string?> MST_SHELf(string shelfNo, bool forceReload = false)
        {
            shelfNo = (shelfNo ?? "").Trim();

            // 1) キャッシュがなければ全件取得（または強制再取得）
            if (_itemStorageCache == null || forceReload)
            {
                ClassNameSelect select = new()
                {
                    viewName = "MST_SHELF",
                };

                _itemStorageCache = await ComService!.GetSelectData(select);
            }

            var datas = _itemStorageCache ?? new();

            //棚番が一致する行を1件取る
            //棚番も重複しないので応用する
            var row = datas
                .Where(r => r.ContainsKey(COL_SHELF))
                .FirstOrDefault(r => EqualStr(r[COL_SHELF], shelfNo));

            if (row == null) return null;

            // 3) 棚の値を返す（なければ null）
            return row.ContainsKey(COL_SHELF) ? row[COL_SHELF]?.ToString()?.Trim() : null;
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

               

            }
            else if (IsLocationBarcode(value))
            {

                string shelfNo = (value ?? string.Empty).Trim();

                // 全件取得済みキャッシュから検索して棚を取得
                string? shelf = await MST_SHELf(shelfNo);

                if (!string.IsNullOrEmpty(shelf))
                {


                    await 棚(shelf);

                    //棚卸をしたい棚番をスキャンしたらF1の処理が走るようにする
                    await ContainerMainLayout!.ButtonClickF1();
                    _ = LoadGridData();


                   
                }
                else
                {
                    // 見つからない場合
                    ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $" {shelfNo} 存在しない棚番です");
                    model!.棚番 = "";
                }

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
        public override async Task F3画面遷移(ComponentProgramInfo info)
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);

            // パレット照会画面に遷移
            //下記のURLに移動
            NavigationManager.NavigateTo("mobile_menu");
        }


        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            await Task.Delay(0); 
            if (string.IsNullOrEmpty(model!.棚番))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "棚を選択してください");
                
                return false;
            }

           



            return !string.IsNullOrEmpty(model!.棚番);
        }

        public override async Task<bool> 確定後処理(ComponentProgramInfo info)
        {
         
            await Task.Delay(0);
            
            return false;

        }


        /// <summary>
        /// システム状態
        /// </summary>
        private string SystemStatus { get; set; } = string.Empty;
        /// <summary>
        /// 作業日
        /// </summary>
        private string WorkDate { get; set; } = string.Empty;
        /// <summary>
        /// 自動日次処理
        /// </summary>
        private string AutoProcStatus { get; set; } = string.Empty;
        /// <summary>
        /// 次回自動処理日時
        /// </summary>
        private string NextProcTime { get; set; } = string.Empty;
        /// <summary>
        /// 前回処理日時
        /// </summary>
        private string LastProcTime { get; set; } = string.Empty;

      
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
        private async Task 棚(object value)
        {
            await Task.Delay(0);

            model!.棚番 = (string)value;

            string shelfNo = (model!.棚番 ?? string.Empty).Trim();

            // 全件取得済みキャッシュから検索して棚を取得
            string? shelf = await MST_SHELf(shelfNo);

            if (!string.IsNullOrEmpty(shelf))
            {


                 model!.棚番 = shelf;

             
                _ = LoadGridData();



            }
            else
            {
                // 見つからない場合
                ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $" {shelfNo} 存在しない棚番です");
                model!.棚番 = "";
            }


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
            //Attributes[STR_ATTRIBUTE_FUNC]["button4text"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["IconName4"] = "";
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
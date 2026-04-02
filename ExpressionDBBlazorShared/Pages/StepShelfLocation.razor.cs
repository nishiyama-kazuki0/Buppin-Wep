using System.Data;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Spreadsheet;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Shared;
using Microsoft.AspNetCore.Components;
using SharedModels;
using Sotsera.Blazor.Toaster.Core.Models;




namespace ExpressionDBBlazorShared.Pages
{
    /// <summary>
    /// パレット移動/パレット№読取
    /// </summary>
    public partial class StepShelfLocation : StepItemBase
    {
        private StepItemMovePalletViewModel? model;
        LoginInfo[]? allUser = null;



        #region override


        // 列名（DB の実カラム名に合わせて修正可）
        private const string COL_MNG_NO = "管理番号";
        private const string COL_SHELFID = "棚ID";

        // 同画面内でのキャッシュ（全件取得結果を保持）
        //ITEM_STORAGEテーブル用
        private List<IDictionary<string, object>>? _itemStorageCache;
        // 同画面内でのキャッシュ（全件取得結果を保持）
        //MST_SHELF用
        private List<IDictionary<string, object>>? _itemStorageCache2;

        /// <summary>
        /// null/DBNull/空白/大小文字差を吸収して比較するヘルパー
        /// </summary>
        private static bool EqualStr(object? o, string target)
        {
            var s = (o == null || o is DBNull) ? "" : o.ToString() ?? "";
            return string.Equals(s.Trim(), (target ?? "").Trim(), StringComparison.OrdinalIgnoreCase);
        }

        // 読取った管理番号の一致する行データを返す
        //見つかったら管理番号の値（string）を返す。なければ null を返す
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
        /// ITEM_STORAGE を全件取得して、管理番号=managementNo の行を1件検索し、
        /// 見つかったら 棚 の値（string）を返す。なければ null を返す。
        /// </summary>
        private async Task<string?> MST_SHELF(string ShelfId, bool forceReload = false)
        {
            ShelfId = (ShelfId ?? "").Trim();

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

            //　管理番号は重複しない値なので
            //  管理番号が一致する行を1件取る
            //　　     
            var row = datas
                .Where(r => r.ContainsKey(COL_SHELFID))
                .FirstOrDefault(r => EqualStr(r[COL_SHELFID], ShelfId));

            if (row == null) return null;


            // 3) 棚の値を返す（なければ null）
            return row.ContainsKey(COL_SHELFID) ? row[COL_SHELFID]?.ToString()?.Trim() : null;
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



            // 初期処理呼び出し
            await InitProcAsync();

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

                //もしそのデータがなければスルーする
                if (row != null)
                {

                    // 例：管理番号、棚、品名、数量、社員コードなどをモデルに詰める
                    string 管理番号 = row.TryGetValue("管理番号", out var mng)
                    ? (mng?.ToString() ?? "").Trim()
                    : "";


                    string 棚 = row.TryGetValue("棚", out var shelfVal)
                        ? (shelfVal?.ToString() ?? "").Trim()
                        : "";

                    string 状態 = row.TryGetValue("状態", out var ship)
                        ? (ship?.ToString() ?? "").Trim()
                        : "";

                  

                    model!.仕掛番号 = row.TryGetValue("仕掛番号", out var shi)
                        ? (shi?.ToString() ?? "").Trim()
                        : "";

                    if (状態 == "出庫済")
                    {
                        ComService!.ShowNotifyMessege(ToastType.Error, "エラー", "この物品は出庫済みです");
                    }
                    else if (!string.IsNullOrEmpty(棚))
                    {
                        //一致するなら管理番号をTextにいれる
                        await OnChangePalletNo(管理番号);

                        model!.元棚 = 棚;

                        _ = LoadGridData();
                        SetElementIdFocus("棚ID");
                    }
                    else if (状態 == "出庫済")
                    {
                        ComService!.ShowNotifyMessege(ToastType.Error, "エラー", "この物品は出庫済みです");
                    }
                    else
                    {
                        // 見つからない場合
                        ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $" {managementNo} まだ入庫が完了していません");

                        // 必要に応じて棚値をクリア
                        // model!.移動先棚 = null;
                    }
                }
                else
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $"存在しない管理番号です");
                    //存在しない管理番号なら値をクリアする
                    model!.PalletNo = "";
                }





            }
            else if (IsLocationBarcode(value))
            {
                string ShelfId = (value ?? string.Empty).Trim();

                string? Shelf2 = await MST_SHELF(ShelfId);

                if (!string.IsNullOrEmpty(Shelf2))
                {
                    model!.移動先棚 = Shelf2;

                    _ = LoadCardListData();
                }
                else
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $" {Shelf2} 存在しない棚番です");


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


        public override async Task<bool> 確定前チェック(ComponentProgramInfo info)////return falsにすると確定後の処理が行われない
        {
            await Task.Delay(0);
            if (string.IsNullOrEmpty(model!.PalletNo))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "管理番号は必須です");
                return false;
            }

            if (string.IsNullOrEmpty(model!.移動先棚))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "移動先棚は必須です");
                return false;

            }

            return !string.IsNullOrEmpty(model!.PalletNo) &&
                   !string.IsNullOrEmpty(model!.移動先棚);
        }

        public override async Task<bool> 確定後処理(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            //移動が完了したらTextを空にする
            model!.PalletNo = "";
            model!.元棚 = "";
            model!.移動先棚 = "";
            //一応もう一度再取得してもらう
            _itemStorageCache = null;

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

            //もしそのデータがなければスルーする
            if (row != null)
            {

                // 例：管理番号、棚、品名、数量、社員コードなどをモデルに詰める
                string 管理番号 = row.TryGetValue("管理番号", out var mng)
                ? (mng?.ToString() ?? "").Trim()
                : "";


                string 棚 = row.TryGetValue("棚", out var shelfVal)
                    ? (shelfVal?.ToString() ?? "").Trim()
                    : "";

                string 状態 = row.TryGetValue("状態", out var ship)
                    ? (ship?.ToString() ?? "").Trim()
                    : "";

               

                model!.仕掛番号 = row.TryGetValue("仕掛番号", out var shi)
                    ? (shi?.ToString() ?? "").Trim()
                    : "";

                if (状態 == "出庫済")
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, "エラー", "この物品は出庫済みです");
                }
                else if (!string.IsNullOrEmpty(棚))
                {
                    //一致するなら管理番号をTextにいれる
                    model!.PalletNo = 管理番号;

                    model!.元棚 = 棚;

                    _ = LoadGridData();
                    SetElementIdFocus("棚ID");
                }
                else if (状態 == "出庫済")
                {
                    ComService!.ShowNotifyMessege(ToastType.Error, "エラー", "この物品は出庫済みです");
                }
                else
                {
                    // 見つからない場合
                    ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $" {managementNo} まだ入庫が完了していません");

                    model!.元棚 = "";
                    // 必要に応じて棚値をクリア
                    // model!.移動先棚 = null;
                }

            }
            else
            {
                ComService!.ShowNotifyMessege(ToastType.Error, "未検出", $"存在しない管理番号です");
                //存在しない管理番号なら値をクリア
                model!.PalletNo = "";
            }

                await Task.Delay(0);
            StateHasChanged();
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



        //private async Task 部署(object value)
        //{


        //    await Task.Delay(0);
        //    StateHasChanged();
        //}

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
        private async Task 仕掛番号(object value)
        {


            await Task.Delay(0);
            StateHasChanged();
        }

        private async Task プロジェクト名(object value)
        {


            await Task.Delay(0);
            StateHasChanged();
        }

        private async Task 内容(object value)
        {


            await Task.Delay(0);
            StateHasChanged();
        }

        public DateTime? 保管開始日2 { get; set; }

        private async Task 保管開始日(object value)
        {

            //今日の日付を入力するように



            await Task.Delay(0);
            StateHasChanged();

        }

        private async Task 保管終了日(object value)
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
            Attributes[STR_ATTRIBUTE_FUNC]["button4text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName4"] = "";
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
            MstShelf? infoLocation = _lstMstShelf.SingleOrDefault(_ => _.棚ID == locationCd);
            if (infoLocation != null)
            {
                model!.棚 = infoLocation.棚ID;


            }
            else
            {
                // エラーメッセージ
                await ShowNotExistLocation(locationCd);

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
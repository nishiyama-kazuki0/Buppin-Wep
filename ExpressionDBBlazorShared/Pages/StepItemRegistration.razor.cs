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
using DocumentFormat.OpenXml.EMMA;
using static SharedModels.SharedConst;
using System.Globalization;
using System.Net.Http.Json;
using static ExpressionDBBlazorShared.Shared.MainLayout;




namespace ExpressionDBBlazorShared.Pages
{
    /// <summary>
    /// パレット移動/パレット№読取
    /// </summary>
    public partial class StepItemRegistration : StepItemBase
    {
        private StepItemMovePalletViewModel? model;

        private LoginInfo? allUser;



        #region override

        private const string PROPKEY_ARRIVAL_NO = "PalletNo";
        private CompItemInfo? compReceptionNo;


        private static bool CanUse = true;


        [Inject] private HttpClient Http { get; set; } = default!;
       
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!; // ← 別名にする
        private IJSObjectReference? _downloadModule;


        // 追加フィールド（ダウンロードを1回だけにするためのフラグ等）
        private bool _downloadStarted = false;
       


        private bool _started;
        


        // 列名（DB の実カラム名に合わせて修正可）
        private const string EMPLOYEECODE = "USER_ID";
        private const string DEPAREMENT= "ID";


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

 
        // 行（IDictionary<string, object>）を返す版
        private async Task<IDictionary<string, object>?> MST_USERS_ROW(string managementNo, bool forceReload = false)
        {
            managementNo = (managementNo ?? "").Trim();

            if (_itemStorageCache == null || forceReload)
            {
                ClassNameSelect select = new()
                {
                    viewName = "MST_USERS",
                };
                _itemStorageCache = await ComService!.GetSelectData(select);
            }

            var datas = _itemStorageCache ?? new();

            var row = datas
                .Where(r => r.ContainsKey(EMPLOYEECODE))
                .FirstOrDefault(r => EqualStr(r[EMPLOYEECODE], managementNo));

            return row; // 見つからなければ null、見つかれば行をそのまま返す
        }

        // 行（IDictionary<string, object>）を返す版
        private async Task<IDictionary<string, object>?> MST_DEPAREMENT_ROW(string managementNo, bool forceReload = false)
        {
            managementNo = (managementNo ?? "").Trim();

            if (_itemStorageCache2 == null || forceReload)
            {
                ClassNameSelect select = new()
                {
                    viewName = "MST_Department",
                };
                _itemStorageCache2 = await ComService!.GetSelectData(select);
            }

            var datas = _itemStorageCache2 ?? new();

            var row = datas
                .Where(r => r.ContainsKey(DEPAREMENT))
                .FirstOrDefault(r => EqualStr(r[DEPAREMENT], managementNo));


            return row; // 見つからなければ null、見つかれば行をそのまま返す
        }
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            

            if(firstRender)
            {
                CanUse = true;
            }
            model = (StepItemMovePalletViewModel?)PageVm;

            if (model != null)
            {
                // 当日の日付をセット（形式は必要に応じて変更）
                //model.保管開始日 = DateTime.Today.ToString("yyyy/MM/dd");
                model.保管開始日2 = DateTime.Now;
            }

            //var user= MainLayout.LoginUserName;
            //model!.管理責任者 = user;



            FirstFocusId = "社員コード";
            return base.OnAfterRenderAsync(firstRender);
        }


      
        /// <summary>
        /// サーバーがメモリでつくったExcelを受け取ってダウンロードする
        /// </summary>
        /// <returns></returns>
        private async Task ダウンロード実行Async()
        {
            var req = new { model!.PalletNo, model!.管理責任者, model!.部署, model!.プロジェクト名, model!.保管開始日, model!.保管終了日, model!.仕掛番号, model!.内容 ,model!.部,model!.課};

            using var res = await Http.PostAsJsonAsync("api/label/excel", req);
            res.EnsureSuccessStatusCode();

            var bytes = await res.Content.ReadAsByteArrayAsync();
            var base64 = Convert.ToBase64String(bytes);
            var mime = res.Content.Headers.ContentType?.ToString()
                       ?? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = res.Content.Headers.ContentDisposition?.FileNameStar
                           ?? res.Content.Headers.ContentDisposition?.FileName?.Trim('"')
                           ?? $"物品管理ラベル_{DateTime.Now:yyyyMMdd}.xlsx";

            var module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "/download.js"); // wwwroot直下
            await module.InvokeVoidAsync("saveFileFromBytes", fileName, base64, mime);
            await module.DisposeAsync();
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

            if (value.Length == SharedConst.LEN_ZONE_ID)
            {
                // ゾーンID
                model!.ZoneCd = value;
                await ScanZoneCd(value);
                _ = LoadGridData();
                SetElementIdFocus("PalletNo");
            }
            else if (IsPalletBarcode(value))
            {
                // 棚に値を入力
                //await 棚(value);

                await OnChangePalletNo(value);
            }
            else if (IsLocationBarcode(value))
            {
                // ロケーションID
                model!.LocationCd = value;
                await ScanLocationCd(value);
                _ = LoadGridData();
                SetElementIdFocus("AreaCd");
            }
        }

        //Excelのセルに値を書き込むために引数データ作成
        public override async Task Excel発行(ComponentProgramInfo info)
        {
            await Task.Delay(0);

            string Deparement = (model!.部署 ?? string.Empty).Trim();

            var row = await MST_DEPAREMENT_ROW(Deparement);

            model!.部署 = Deparement;
            if (row != null)
            {

                // 例：管理番号、棚、品名、数量、社員コードなどをモデルに詰める
                model!.部 = row.TryGetValue("部", out var mng)
                ? (mng?.ToString() ?? "").Trim()
                : "";

                // 例：管理番号、棚、品名、数量、社員コードなどをモデルに詰める
                model!.課 = row.TryGetValue("課", out var AfId)
                    ? (AfId?.ToString() ?? "").Trim()
                    : "";

            }

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
                stName = "部";
                _storedData[stName] = model!.部;
                stName = "課";
                _storedData[stName] = model!.課;
                stName = "プロジェクト名";
                _storedData[stName] = model!.プロジェクト名;
                stName = "保管開始日";
                _storedData[stName] = model!.保管開始日;
                stName = "保管終了日";
                _storedData[stName] = model!.保管終了日;
                stName = "仕掛番号";
                _storedData[stName] = model!.仕掛番号;
                stName = "内容";
                _storedData[stName] = model!.内容;

                
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

        public override async Task 管理番号採番(ComponentProgramInfo info)//F2クリック時イベント
        {
            //model = (StepItemMovePalletViewModel?)PageVm;


            

                ClassNameSelect select = new()
                {
                    viewName = "ITEM_STORAGE",
                };

                // 全件取得
                List<IDictionary<string, object>> datas = await ComService!.GetSelectData(select);

                const string NoCol = "管理番号"; // ここをモデルに入れる

                // 最新行（管理番号の最大値）を 1 件取得
                var value = datas?
                    .Where(r => r.ContainsKey(NoCol))
                    .Select(r => new
                    {
                        Row = r,
                        No = Convert.ToInt32(r[NoCol])
                    })
                    .OrderByDescending(x => x.No)
                    .FirstOrDefault();

                // 値をモデルへ
                if (value != null)
                {
                
                     await OnChangePalletNo(value.No.ToString()); // 直接代入
                     
                     
                }
                else
                {
                    model!.PalletNo = null;
                }

                CanUse = false;// 採番を連続で押せないよう対策
                               // 初期処理
            await InvokeAsync(StateHasChanged);


            _ = LoadGridData();

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




        public override async  Task<bool> 確定前チェック(ComponentProgramInfo info)
        {
            await Task.Delay(0);
            //DataTime型を文字列型に変換
            model!.保管開始日 = model!.保管開始日2?.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) ?? string.Empty;
            model!.保管終了日 = model!.保管終了日2?.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) ?? string.Empty;
            if (string.IsNullOrEmpty(model!.PalletNo))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "管理番号は必須です");
                return false;
            }
            if (string.IsNullOrEmpty(model!.社員コード))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "社員コードは必須です");
                return false;
            }
            if (string.IsNullOrEmpty(model!.管理責任者))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "管理責任者は必須です");
                return false;
            }
            if (string.IsNullOrEmpty(model!.部署))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "部署は必須です");
                return false;
            }
            if (string.IsNullOrEmpty(model!.プロジェクト名))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "プロジェクト名は必須です");
                return false;
            }
            if (string.IsNullOrEmpty(model!.保管開始日))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "保管開始日は必須です");
                return false;
            }
            if (string.IsNullOrEmpty(model!.保管終了日))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "保管終了日は必須です");
                return false;
            }
           
            return !string.IsNullOrEmpty(model!.PalletNo) &&
                   !string.IsNullOrEmpty(model!.社員コード) &&
                   !string.IsNullOrEmpty(model!.管理責任者) &&
                   !string.IsNullOrEmpty(model!.部署) &&
                   !string.IsNullOrEmpty(model!.プロジェクト名) &&
                   !string.IsNullOrEmpty(model!.保管開始日) &&
                   !string.IsNullOrEmpty(model!.保管終了日)
                   ;
            
        }


        public override async Task<bool> 確定前チェック2(ComponentProgramInfo info)////return falsにすると確定後の処理が行われない
        {
            await Task.Delay(0);
            if (!string.IsNullOrEmpty(model!.PalletNo))
            {
                ComService!.ShowNotifyMessege(ToastType.Error, pageName, "一度登録まで行ってください");
                return false;
            }

            return string.IsNullOrEmpty(model!.PalletNo);


        }


        public override async Task<bool> 確定後処理(ComponentProgramInfo info)
        {

            //UI コンテキスト上で fire-and-forget
            _ = InvokeAsync(ダウンロード実行Async);

            await Task.Delay(0);
            ComService!.ShowNotifyMessege(ToastType.Success, "完了", "物品を登録しました！");

            model!.PalletNo = "";//連続で同じ管理番号が登録できないようにする
            CanUse = true;// もう一度採番できるようにする
            return false;

            
        }

        //セルをダブりクリックした場合
        private async Task OnDbClick(string code)
        {


            if (_gridSelectedData is { Count: > 0 })
            {
                var row = _gridSelectedData[0];

                // 列名「棚番」を model.棚番 にセット
                if (row.TryGetValue("社員コード", out var value) && value is not null && value is not DBNull)
                {
                    await 社員コード(value.ToString());
                }
                // if (row.TryGetValue("社員名", out var value2) && value2 is not null && value2 is not DBNull)
                // {
                //     model!.管理責任者= value2.ToString();
                // }
                // if (row.TryGetValue("部署名", out var value3) && value3 is not null && value3 is not DBNull)
                // {
                //     model!.部署 = value3.ToString();
                // }
            }
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
            await InvokeAsync(StateHasChanged);
            _ = LoadGridData();

        }



        private async Task 社員コード(object value)//ログインしている社員情報を入力する
        {

            model!.社員コード = (string)value;

            string EmpID = (model!.社員コード ?? string.Empty).Trim();
           

            var row = await MST_USERS_ROW(EmpID);
            if (row != null)
            {

                // 例：管理番号、棚、品名、数量、社員コードなどをモデルに詰める
                string? 管理責任者 = row.TryGetValue("USER_NAME", out var mng)
                ? (mng?.ToString() ?? "").Trim()
                : "";

                // 例：管理番号、棚、品名、数量、社員コードなどをモデルに詰める
                string? 部署 = row.TryGetValue("AFFILIATION_ID", out var AfId)
                    ? (AfId?.ToString() ?? "").Trim()
                    : "";

                await 管理責任者N(管理責任者);

                await 部署N(部署);

            }

            await Task.Delay(0);
            StateHasChanged();
        }

        private async Task 管理責任者N(object value)//ログインしている社員情報を入力する
        {

            model!.管理責任者 = (string)value;
            await Task.Delay(0);
          
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
        private async Task 部署N(object value)//ログインしている社員情報を入力する
        {


            await Task.Delay(0);
            model!.部署 = (string)value;
            model!.LocationCd = string.Empty;
            model!.ZoneCd = string.Empty;
        ;
        }
        private async Task 仕掛番号 (object value)
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

        private  Task 保管開始日2(DateTime? value)
        {
            
            StateHasChanged();
            return Task.CompletedTask;
        }


        /// <summary>
        /// ゾーンの選択イベント
        /// </summary>
        /// <param name="value"></param>
        private async Task OnChangeZone(object value)
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

        ///// <summary>
        ///// パラメータ関連初期化
        ///// </summary>
        private void InitParam()
        {

            //他画面から移動してきた場合、他のボタンが残ったままになるでココで非表示処理する
            //他にやり方があると思うけど一旦この処理でいく
            Attributes[STR_ATTRIBUTE_FUNC]["button3text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName3"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["button4text"] = "";
            Attributes[STR_ATTRIBUTE_FUNC]["IconName4"] = "";         
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
            //Attributes[STR_ATTRIBUTE_FUNC]["button12text"] = "";
            //Attributes[STR_ATTRIBUTE_FUNC]["IconName12"] = "";
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
            MstLocationData? infoLocation = _lstMstLocation.SingleOrDefault(_ => _.LocationId == locationCd);
            if (infoLocation != null)
            {
                model!.AreaCd = infoLocation.ZoneId;
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
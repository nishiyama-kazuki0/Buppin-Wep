using SharedModels;
using ZennohBlazorShared.Data;
using ZennohBlazorShared.Services;
using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 出庫メニュー
    /// </summary>
    public partial class MobileShipMenu : ChildPageBaseMobile
    {
        /// <summary>
        /// 作業件数
        /// </summary>
        private string WorkCnt { get; set; } = string.Empty;
        /// <summary>
        /// コーナー別仕分済件数
        /// </summary>
        private string SortingByCornerCnt { get; set; } = string.Empty;
        /// <summary>
        /// 倉庫配送先数
        /// </summary>
        private string DeliveryCnt { get; set; } = string.Empty;

        /// <summary>
        /// 初期処理
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            WorkCnt = string.Empty;
            DeliveryCnt = string.Empty;
            SortingByCornerCnt = string.Empty;

            //出庫作業の件数を表示する
            _ = InvokeAsync(async () =>
            {
                ClassNameSelect select = new()
                {
                    viewName = "VW_HT_出庫メニュー",
                    tsqlHints = EnumTSQLhints.NOLOCK
                };
                List<IDictionary<string, object>> datas = await ComService!.GetSelectData(select);
                if (null != datas && datas.Count > 0)
                {
                    IDictionary<string, object> dic = datas.First();
                    WorkCnt = ComService.GetValueString(dic, "作業件数");
                    DeliveryCnt = ComService.GetValueString(dic, "倉庫配送先数");
                    SortingByCornerCnt = ComService.GetValueString(dic, "コーナー別仕分残件数");
                }
                StateHasChanged();
            });

            // 出庫で選択した倉庫配送先、倉庫コード、ゾーンコードをクリアする
            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_DELIVERY_ID);
            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_AREA_ID);
            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_ZONE_ID);
            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_SHIP_DELIVERY_ID);
            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_SHIP_AREA_ID);
            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_SHIP_ZONE_ID);

            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_DELIVERY_NM);
            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_AREA_NM);
            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_ZONE_NM);

            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_AREA_ID);
            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_ZONE_ID);
            await LocalStorage.RemoveItemAsync(SharedConst.STR_LOCALSTORAGE_CUT_CONVEY_LOCATION_ID);

            await base.OnInitializedAsync();
        }

        /// <summary>
        /// HTスキャン処理
        /// </summary>
        /// <param name="scanData"></param>
        protected override async Task HtService_HtScanEvent(ScanData scanData)
        {
            await Task.Delay(0);

            // パレットNoの読み取り。履歴は残さず、読み取ったパレットNowo セットしてパレット照会画面に遷移。戻りは在庫の在庫メニューの在庫とする
            string value = scanData.strStringData;

            if (IsPalletBarcode(value))
            {
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
                // await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
                await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, value);//読み取りはパレットNoのみ
                // パレット照会画面に遷移
                NavigationManager.NavigateTo("pallet_inventory_inquiry");
            }
        }
    }
}
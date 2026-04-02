using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using ExpressionDBBlazorShared.Shared;
using SharedModels;

namespace ExpressionDBBlazorShared.Pages;

/// <summary>
/// 出庫指図画面
/// </summary>
public partial class WorkHistory : ChildPageBaseMobile
{
    /// <summary>
    /// HTスキャン処理
    /// </summary>　　
    /// <param name="scanData"></param>
    protected override async Task HtService_HtScanEvent(ScanData scanData)
    {
        await Task.Delay(0);

        // パレットNoの読み取り。履歴は残さず、読み取ったパレットNowo セットしてパレット照会画面に遷移。戻りは在庫の在庫メニューの在庫とする
        string value = scanData.strStringData;

        

        if (IsPalletBarcode(value))//パレット用バーコードか判定
        {
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移画面, ClassName);
            // await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_遷移履歴, model!.StrAddRireki(ClassName));
            await ComService.SetLocalStorage(SharedConst.STR_LOCALSTORAGE_PALLETE_NO, value);//情報を保存して次のページに持っていく

            
            // パレット照会画面に遷移
            //下記のURLに移動
            //NavigationManager.NavigateTo("worker_registration2");
        }
    }
}
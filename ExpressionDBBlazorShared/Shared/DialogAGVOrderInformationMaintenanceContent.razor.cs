using ExpressionDBBlazorShared.Data;
namespace ExpressionDBBlazorShared.Shared;


/// <summary>
/// AGV搬送情報メンテナンスダイアログ
/// </summary>
public partial class DialogAGVOrderInformationMaintenanceContent : DialogCommonInputContent
{
    #region override
    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        
        ComponentColumnsInfo? comp;
        string[] propertyArray = { "AGVタスクID", "FROM_ステーションコード", "TO_ステーションコード", "指定ゾーン区分", "パレットNO", "専用容器QR","指定AGV_ID", "生産タスクID", "到着時反転", "プロセスカテゴリ" };        //編集可否　1:可能,2:不可
        int[] editArray = { 2, 2, 2, 2, 2, 2, 2, 2 ,2,1};

        for (int i = 0; i < propertyArray.Length; i++)
        {
            comp = Components.Where(_ => _.Property == propertyArray[i]).FirstOrDefault();
            if (comp != null)
            {
                comp.EditType = editArray[i]; //編集可能
            }
        }
        await base.OnInitializedAsync();
    }
    #endregion



    //空きロケーションを保管区分によって絞り込む

    //正袋の場合のみ正袋数、正袋梱包量に基づき在庫量を自動計算(在庫量はさわれない)

}
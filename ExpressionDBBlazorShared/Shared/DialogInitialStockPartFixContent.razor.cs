using ExpressionDBBlazorShared.Data;
using System;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 専用パレット：初期在庫登録ダイアログ
/// </summary>
public partial class DialogInitialStockPartFixContent : DialogCommonInputContent
{
    #region override
    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        //Componentsをベースにして項目を初期化する前にComponentsのステータスを変更する
        //マスタの編集画面用にComponentsは設定されているため、本画面用のパラメータ変更を行う
        #region データ修正
        var precmp専用容器QR = new ComponentColumnsInfo();
        var cmp専用容器QR = Components.Where(_ => _.Property == "専用容器QR").FirstOrDefault();
        if (cmp専用容器QR != null)
        {
            precmp専用容器QR.MaxLength = cmp専用容器QR.MaxLength;
            precmp専用容器QR.IsEdit = cmp専用容器QR.IsEdit;
            cmp専用容器QR.MaxLength = 12;
            cmp専用容器QR.IsEdit = true;
        }
        var precmp原料品目コード = new ComponentColumnsInfo();
        var cmp原料品目コード = Components.Where(_ => _.Property == "原料品目コード").FirstOrDefault();
        if (cmp原料品目コード != null)
        {
            precmp原料品目コード.EditType = cmp原料品目コード.EditType;
            cmp原料品目コード.EditType = 2; //編集不可
        }
        var precmp用途 = new ComponentColumnsInfo();
        var cmp用途 = Components.Where(_ => _.Property == "用途").FirstOrDefault();
        if (cmp用途 != null)
        {
            precmp用途.EditType = cmp用途.EditType;
            cmp用途.EditType = 2; //編集不可
        }
        #endregion

        await base.OnInitializedAsync();

        #region 初期化後データを戻す
        if (cmp専用容器QR != null)
        {
            cmp専用容器QR.MaxLength = precmp専用容器QR.MaxLength;
            cmp専用容器QR.IsEdit = precmp専用容器QR.IsEdit;
        }
        if (cmp原料品目コード != null)
        {
            cmp原料品目コード.EditType = precmp原料品目コード.EditType;
        }
        if (cmp用途 != null)
        {
            cmp用途.EditType = precmp用途.EditType;
        }
        #endregion
    }
    #endregion


}
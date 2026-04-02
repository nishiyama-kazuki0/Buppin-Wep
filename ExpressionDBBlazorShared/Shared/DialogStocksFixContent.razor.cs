using ExpressionDBBlazorShared.Data;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 在庫メンテナンスダイアログ
/// </summary>
public partial class DialogStocksFixContent : DialogCommonInputContent
{
    #region override
    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitializedAsync()
    {
        //正袋在庫と端数在庫の時で編集できる項目を変更する
        //正袋：正袋数*正袋梱包量=在庫量
        //端数：在庫量のみ

        //Componentsをベースにして項目を初期化する前にComponentsのステータスを変更する
        //なお共通のストアドを呼び出す関係でパラメータはすべて必要
        //編集不要なパラメータでも情報のほうには表示するようにする
        bool isStocks = true;
        if (InitialData.TryGetValue("専用容器QR", out object? value))
        {
            //専用容器QRがある場合は端数在庫とみなす
            string? tmp = value?.ToString();
            if (!string.IsNullOrEmpty(tmp)) isStocks = false;
        }

        var precmp正袋数 = new ComponentColumnsInfo();
        var precmp正袋梱包量 = new ComponentColumnsInfo();
        var precmp在庫量 = new ComponentColumnsInfo();
        var precmp内袋情報 = new ComponentColumnsInfo();
        var precmp外装区分 = new ComponentColumnsInfo();
        var precmp保管区分 = new ComponentColumnsInfo();
        var precmp在庫種類 = new ComponentColumnsInfo();
        ComponentColumnsInfo? cmp正袋数;
        ComponentColumnsInfo? cmp正袋梱包量;
        ComponentColumnsInfo? cmp在庫量;
        ComponentColumnsInfo? cmp内袋情報;
        ComponentColumnsInfo? cmp外装区分;
        ComponentColumnsInfo? cmp保管区分;
        ComponentColumnsInfo? cmp在庫種類;

        if (isStocks)
        {
            cmp正袋数 = Components.Where(_ => _.Property == "正袋数").FirstOrDefault();
            if (cmp正袋数 != null) 
            {
                precmp正袋数.EditType = cmp正袋数.EditType;
                cmp正袋数.EditType = 1; //編集可能
            }
            cmp正袋梱包量 = Components.Where(_ => _.Property == "正袋梱包量").FirstOrDefault();
            if (cmp正袋梱包量 != null)
            {
                precmp正袋梱包量.EditType = cmp正袋梱包量.EditType;
                cmp正袋梱包量.EditType = 1; //編集可能
            }
            cmp在庫量 = Components.Where(_ => _.Property == "在庫量").FirstOrDefault();
            if (cmp在庫量 != null)
            {
                precmp在庫量.EditType = cmp在庫量.EditType;
                cmp在庫量.EditType = 2; //編集不可
            }

            await base.OnInitializedAsync();

            #region 初期化後データを戻す
            if (cmp正袋数 != null)
            {
                cmp正袋数.EditType = precmp正袋数.EditType;
            }
            if (cmp正袋梱包量 != null)
            {
                cmp正袋梱包量.EditType = precmp正袋梱包量.EditType;
            }
            if (cmp在庫量 != null)
            {
                cmp在庫量.EditType = precmp在庫量.EditType;
            }
            #endregion
        }
        else
        {
            cmp正袋数 = Components.Where(_ => _.Property == "正袋数").FirstOrDefault();
            if (cmp正袋数 != null)
            {
                precmp正袋数.EditType = precmp正袋数.EditType;
                cmp正袋数.EditType = 2; //編集不可
            }
            cmp正袋梱包量 = Components.Where(_ => _.Property == "正袋梱包量").FirstOrDefault();
            if (cmp正袋梱包量 != null)
            {
                precmp正袋梱包量.EditType = cmp正袋梱包量.EditType;
                cmp正袋梱包量.EditType = 2; //編集不可
            }
            cmp在庫量 = Components.Where(_ => _.Property == "在庫量").FirstOrDefault();
            if (cmp在庫量 != null)
            {
                precmp在庫量.EditType = cmp在庫量.EditType;
                cmp在庫量.EditType = 1; //編集可能
            }
            cmp内袋情報 = Components.Where(_ => _.Property == "内袋情報").FirstOrDefault();
            if (cmp内袋情報 != null)
            {
                precmp内袋情報.EditType = cmp内袋情報.EditType;
                cmp内袋情報.EditType = 2; //編集不可
            }
            cmp外装区分 = Components.Where(_ => _.Property == "外装区分").FirstOrDefault();
            if (cmp外装区分 != null)
            {
                precmp外装区分.EditType = cmp外装区分.EditType;
                cmp外装区分.EditType = 2; //編集不可
            }
            cmp保管区分 = Components.Where(_ => _.Property == "保管区分").FirstOrDefault();
            if (cmp保管区分 != null)
            {
                precmp保管区分.EditType = cmp保管区分.EditType;
                cmp保管区分.EditType = 2; //編集不可
            }
            cmp在庫種類 = Components.Where(_ => _.Property == "在庫種類").FirstOrDefault();
            if (cmp在庫種類 != null)
            {
                precmp在庫種類.EditType = cmp在庫種類.EditType;
                cmp在庫種類.EditType = 2; //編集不可
            }

            await base.OnInitializedAsync();

            #region 初期化後データを戻す
            if (cmp正袋数 != null)
            {
                cmp正袋数.EditType = precmp正袋数.EditType;
            }
            if (cmp正袋梱包量 != null)
            {
                cmp正袋梱包量.EditType = precmp正袋梱包量.EditType;
            }
            if (cmp在庫量 != null)
            {
                cmp在庫量.EditType = precmp在庫量.EditType;
            }
            if (cmp内袋情報 != null)
            {
                cmp内袋情報.EditType = precmp内袋情報.EditType;
            }
            if (cmp外装区分 != null)
            {
                cmp外装区分.EditType = precmp外装区分.EditType;
            }
            if (cmp保管区分 != null)
            {
                cmp保管区分.EditType = precmp保管区分.EditType;
            }
            if (cmp在庫種類 != null)
            {
                cmp在庫種類.EditType = precmp在庫種類.EditType;
            }
            #endregion
        }


        //TODO:保管区分による選択肢の絞り込みを行うため、コンボボックスのイベントを定義する

    }
    #endregion

    //TODO:空きロケーションを保管区分によって絞り込む

    //TODO:正袋の場合のみ正袋数、正袋梱包量に基づき在庫量を自動計算(在庫量はさわれない)
}
namespace ExpressionDBBlazorShared.Data;

//TODO 20240223 現在、DEFINE_COMPONENTで定義されている値をVMにセットしているが、これは設計意図と異なる。
//TODO ＶＭを作るなら、ＶＭをＴ型でコントローラーに渡して、コントローラーでＶＭに値をセットするようにするべき。そういう機能のコントローラーを作るべき。そのほうが画面側でリフレクションを使用する必要が内ので、効率よくなるはず。
/// <summary>
/// 基底ViewModel
/// </summary>
public class BaseViewModel
{
    // 呼出元
    public string Caller { get; set; } = string.Empty;
    // ピック予定存在フラグ、ゾーン予定存在フラグ、倉庫予定存在フラグ、倉庫配送先予定存在フラグ
    public (bool, bool, bool, bool) PickPlanFlag;

    #region 画面遷移制御関連
    // 画面遷移制御関連
    public List<string> Rireki { get; set; } = new List<string>();

    public bool IsRireki => Rireki.Count > 0;

    public string FirstRireki
    {
        get
        {
            string className = string.Empty;
            if (Rireki.Count > 0)
            {
                className = Rireki[0];
            }
            return className;
        }
    }

    public string LastRireki
    {
        get
        {
            string className = string.Empty;
            if (Rireki.Count > 0)
            {
                className = Rireki[^1];
            }
            return className;
        }
    }

    /// <summary>
    /// 画面遷移履歴を取得
    /// 基本GetLastRirekiUrlと対でしようします
    /// </summary>
    /// <returns></returns>
    public string StrBackRireki()
    {
        return string.Join(",", Rireki);
    }

    /// <summary>
    /// 画面遷移履歴最初の画面を取得
    /// 基本GetFirstRirekiUrlと対でしようします
    /// </summary>
    /// <returns></returns>
    public string StrFirstRireki()
    {
        if (Rireki.Count > 0)
        {
            Rireki = [Rireki[0]];
        }
        return string.Join(",", Rireki);
    }

    /// <summary>
    /// データを引継ぎ画面を遷移する時に使用しています
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    public string StrAddRireki(string val)
    {
        if (!string.IsNullOrEmpty(val))
        {
            Rireki.Add(val);
        }
        return string.Join(",", Rireki);
    }

    /// <summary>
    /// パレット照会から戻った時に、戻ってきた画面の履歴をクリアする時に使用しています
    /// </summary>
    /// <param name="val"></param>
    public void RemoveRireki(string val)
    {
        _ = Rireki.Remove(val);
    }

    /// <summary>
    /// 画面遷移履歴最初の画面URLを取得
    /// 画面遷移先の確定終了後、機能の一番初めに戻るときに使用しています
    /// 確定処理の後、別画面にデータを引き継ぎ遷移し、遷移先で戻る場合に機能の一番初めに戻る処理で使用しています
    /// </summary>
    /// <returns></returns>
    public string GetFirstRirekiUrl()
    {
        string url = string.Empty;
        _ = FirstRireki;
        //if (className.Equals(typeof(StepItemArrivalsInspectsInput).Name))
        //{
        //    // 入荷検品

        //    // 入荷検品へ
        //    url = "arrivals_inspects";
        //}
        //else if (className.Equals(typeof(ストアド呼び出し3).Name) ||
        //    className.Equals(typeof(StepItemMovePalletInput).Name))
        //{
        //    // パレット移動

        //    // パレット移動へ
        //    url = "move_pallet";
        //}
        //else if (className.Equals(typeof(StepItemPickingTargetSelectZone).Name))
        //{
        //    // パレットピック【倉庫別】－ ゾーン選択

        //    // パレットピック(倉庫別)へ
        //    //url = "picking_pallet";
        //    // ピック予定が存在する場合はパレットピック(倉庫別)へ
        //    // 存在しない場合は、パレットピック(倉庫別)の倉庫、ゾーン選択へ
        //    url = PickPlanFlag.Item1 ? "picking_pallet" : "picking_target_select";
        //}
        //else if (className.Equals(typeof(StepItemPickingTargetSelectByDeliveryZone).Name))
        //{
        //    // パレットピック【倉庫配送先別】－ ゾーン選択

        //    // パレットピック(倉庫配送先別)へ
        //    //url = "picking_pallet_by_delivery";
        //    // ピック予定が存在する場合はパレットピック(倉庫配送先別)へ
        //    // 存在しない場合は、パレットピック(倉庫配送先別)の倉庫配送先、倉庫、ゾーン選択へ
        //    url = PickPlanFlag.Item1 ? "picking_pallet_by_delivery" : "picking_target_select_by_delivery";
        //}
        //else if (className.Equals(typeof(StepItemPickingTargetSelectItemByDeliveryZone).Name))
        //{
        //    // 摘取ピック【倉庫配送先別】－ ゾーン選択

        //    // 摘取ピック(倉庫配送先別)へ
        //    //url = "picking_item_by_delivery";
        //    // ピック予定が存在する場合は摘取ピック(倉庫配送先別)へ
        //    // 存在しない場合は、摘取ピック(倉庫配送先別)の倉庫配送先、倉庫、ゾーン選択へ
        //    url = PickPlanFlag.Item1 ? "picking_item_by_delivery" : "picking_target_select_item_by_delivery";
        //}
        //else if (className.Equals(typeof(StepItemMoveCompleteSearch).Name) ||
        //    className.Equals(typeof(StepItemMoveCompleteSave).Name))
        //{
        //    // 切出し搬送

        //    // 切出し搬送へ
        //    url = "move_complete";
        //}
        //else if (className.Equals(typeof(StepItemSortingByCornersSearch).Name) ||
        //    className.Equals(typeof(StepItemSortingByCornersInput).Name) ||
        //    className.Equals(typeof(StepItemSortingByCornersSave).Name))
        //{
        //    // コーナー別仕分

        //    // コーナー別仕分へ
        //    url = "sorting_by_corners";
        //}
        return url;
    }

    /// <summary>
    /// 画面遷移履歴最後の画面URLを取得
    /// パレット照会で戻るときに使用しています
    /// </summary>
    /// <returns></returns>
    public string GetLastRirekiUrl()
    {
        string url = string.Empty;
        _ = LastRireki;

        //if (className.Equals(typeof(StepItemArrivalsInspectsInput).Name))
        //{
        //    // 入荷検品へ
        //    url = "arrivals_inspects";
        //}
        //else if (className.Equals(typeof(StepItemMovePalletInput).Name))
        //{
        //    // パレット移動へ
        //    url = "move_pallet";
        //}
        //else if (className.Equals(typeof(StepItemPickingPalletPick).Name))
        //{
        //    // パレットピック(倉庫別)へ
        //    url = "picking_pallet";
        //}
        //else if (className.Equals(typeof(StepItemPickingPalletByDeliveryPick).Name))
        //{
        //    // パレットピック(倉庫配送先別)へ
        //    url = "picking_pallet_by_delivery";
        //}
        //else if (className.Equals(typeof(StepItemMoveCompleteSave).Name))
        //{
        //    // 切出搬送へ
        //    url = "move_complete";
        //}
        //else if (className.Equals(typeof(StepItemSortingByCornersInput).Name) || className.Equals(typeof(StepItemSortingByCornersSave).Name))
        //{
        //    // コーナー別仕分へ
        //    url = "sorting_by_corners";
        //}
        //else if (className.Equals(typeof(StepItemMoveCompleteCornerSave).Name))
        //{
        //    // コーナー搬送へ
        //    url = "move_complete_corner";
        //}
        //else if (className.Equals(typeof(StepItemPalletDivisionOrgInput).Name) || className.Equals(typeof(StepItemPalletDivisionDestInput).Name))
        //{
        //    // パレット分割へ
        //    url = "pallet_division";
        //}
        //else if (className.Equals(typeof(StepItemPalletAssortParentInput).Name) || className.Equals(typeof(StepItemPalletAssortChildInput).Name))
        //{
        //    // パレット詰合せへ
        //    url = "pallet_assort";
        //}

        return url;
    }

    public static List<string> GetRireki(string val)
    {
        return val is null ? [] : val.Split(",").ToList();
    }
    #endregion
}

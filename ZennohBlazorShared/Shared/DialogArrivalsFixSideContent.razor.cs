using ZennohBlazorShared.Data;
using ZennohBlazorShared.Pages;

namespace ZennohBlazorShared.Shared
{
    /// <summary>
    /// 入荷情報追加・修正
    /// </summary>
    public partial class DialogArrivalsFixSideContent : DialogCommonInputContent
    {
        #region override

        /// <summary>
        /// F1ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        protected override async Task OnClickResultF1(object? sender)
        {
            // バリデートチェック
            if (!base.バリデートチェック())
            {
                return;
            }

            Dictionary<string, object> retVal = new();
            try
            {
                // 入力エリアロック
                ComService.SetCompItemListDisabled(_inputItems, true);

                // 確認
                bool? ret = await ComService.DialogShowYesNo("設定を確定しますか？", DialogTitle);
                if (true != ret)
                {
                    return;
                }
                int inputCase = 0;
                int inputBara = 0;
                // 入力値を取得
                foreach (List<CompItemInfo> listItem in _inputItems)
                {
                    foreach (CompItemInfo item in listItem)
                    {
                        switch (item.TitleLabel)
                        {
                            case ArrivalsDetailsAddMainte.PROPKEY_等級_CODE:
                                if (item?.CompObj?.Instance is CompDropDown cmbGrade)
                                {
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_等級_CODE] = cmbGrade.InputValue;
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_等級_NAME] = cmbGrade.GetInputValue1();
                                }
                                break;
                            case ArrivalsDetailsAddMainte.PROPKEY_階級_CODE:
                                if (item?.CompObj?.Instance is CompDropDown cmbRanks)
                                {
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_階級_CODE] = cmbRanks.InputValue;
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_階級_NAME] = cmbRanks.GetInputValue1();
                                }
                                break;
                            case ArrivalsDetailsAddMainte.PROPKEY_荷姿_CODE:
                                if (item?.CompObj?.Instance is CompDropDown cmbPacking)
                                {
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_荷姿_CODE] = cmbPacking.InputValue;
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_荷姿_NAME] = cmbPacking.GetInputValue1();
                                }
                                break;
                            case ArrivalsDetailsAddMainte.PROPKEY_入数:
                                if (item?.CompObj?.Instance is CompNumeric numQuantity)
                                {
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_入数] = numQuantity.InputValue;
                                }
                                break;
                            case ArrivalsDetailsAddMainte.PROPKEY_予定ケース数:
                                if (item?.CompObj?.Instance is CompNumeric numCase)
                                {
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_予定ケース数] = numCase.InputValue;
                                    inputCase = Convert.ToInt32(numCase.InputValue);
                                }
                                break;
                            case ArrivalsDetailsAddMainte.PROPKEY_予定バラ数:
                                if (item?.CompObj?.Instance is CompNumeric numBara)
                                {
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_予定バラ数] = numBara.InputValue;
                                    inputBara = Convert.ToInt32(numBara.InputValue);
                                }
                                break;
                            case ArrivalsDetailsAddMainte.PROPKEY_賞味期限_FRM:
                                if (item?.CompObj?.Instance is CompDatePicker dteExpiration)
                                {
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_賞味期限] = dteExpiration.InputValue;
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_賞味期限_FRM] = dteExpiration.GetDispInputValue();
                                }
                                break;
                            case ArrivalsDetailsAddMainte.PROPKEY_特別管理品_CODE:
                                if (item?.CompObj?.Instance is CompDropDown cmbSpecialItem)
                                {
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_特別管理品_CODE] = cmbSpecialItem.InputValue;
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_特別管理品_NAME] = cmbSpecialItem.GetInputValue1();
                                }
                                break;
                            case ArrivalsDetailsAddMainte.PROPKEY_作業者説明_CODE:
                                if (item?.CompObj?.Instance is CompDropDown cmbOperator)
                                {
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_作業者説明_CODE] = cmbOperator.InputValue;
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_作業者説明_NAME] = cmbOperator.GetInputValue1();
                                }
                                break;
                            case ArrivalsDetailsAddMainte.PROPKEY_荷印:
                                if (item?.CompObj?.Instance is CompTextBox txtMark)
                                {
                                    retVal[ArrivalsDetailsAddMainte.PROPKEY_荷印] = txtMark.InputValue;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (inputCase + inputBara <= 0)
                {
                    await ComService.DialogShowOK("予定ケース数+予定バラ数は1以上を入力してください。", DialogTitle);
                    return;
                }
            }
            finally
            {
                // 入力エリアロック解除
                ComService.SetCompItemListDisabled(_inputItems, false);
            }

            // 入力値を返却
            DialogService.CloseSide(retVal);
        }

        /// <summary>
        /// 情報エリア初期化
        /// </summary>
        protected override async Task InitInfoItemsAsync()
        {
            // クリア
            _infoItems.Clear();

            // カラム設定データからヘッダ項目のみを抽出し、並び変える
            List<ComponentColumnsInfo> listInfo = Components
                .Where(_ => _.IsEdit == true && _.EditType == 2)
                .OrderBy(_ => _.EditDialogLayoutGroup)
                .ThenBy(_ => _.EditDialogLayoutDispOrder)
                .ToList();

            // コンポーネント情報を作成
            _infoItems = await ComService.GetCompItemInfo(listInfo, InitialData, new List<ComponentColumnsInfo>(), ComponentsInfos, false, true);

            // 情報エリアAttributes設定
            AttributesInfo.Add("AllowCollapse", InfoAllowCollapse);
            AttributesInfo.Add("GroupTitle", InfoTitle);
            AttributesInfo.Add("IconName", InfoIconName);
            AttributesInfo.Add("CopmItems", _infoItems);
            AttributesInfo.Add("LabelWidth", DialogLabelWidth);
        }

        /// <summary>
        /// 明細項目初期化
        /// </summary>
        protected override async Task InitInputItemsAsync()
        {
            // クリア
            _inputItems.Clear();

            // 追加モードの場合、DEFINE_COMPONENTSの初期値を設定
            if (Mode == enumDialogMode.Add)
            {
                List<ComponentsInfo> lstInitInfo = ComponentsInfos.Where(_ => _.ComponentName == STR_ATTRIBUTE_ADD_DIALOG_INITIAL_VALUE).ToList();
                foreach (ComponentsInfo initInfo in lstInitInfo)
                {
                    if (!string.IsNullOrEmpty(initInfo.Value))
                    {
                        // 【注意】複数の初期値を設定できるコンポーネントは、カンマ(,)区切りで初期値が登録されている前提です
                        string[] values = initInfo.Value.Split(',');
                        InitialData[initInfo.AttributesName] = values.Length > 1 ? new List<string>(values) : values[0];
                    }
                }
            }

            // カラム設定データから明細項目のみを抽出し、並び変える
            List<ComponentColumnsInfo> listInfo = Components
                .Where(_ => _.IsEdit == true && _.EditType == 1)
                .OrderBy(_ => _.EditDialogLayoutGroup)
                .ThenBy(_ => _.EditDialogLayoutDispOrder)
                .ToList();

            // コンポーネント情報を作成
            _inputItems = await ComService.GetCompItemInfo(listInfo, InitialData, new List<ComponentColumnsInfo>(), ComponentsInfos, false, false);

            // 入力エリアAttributes設定
            AttributesInput.Add("AllowCollapse", InputAllowCollapse);
            AttributesInput.Add("GroupTitle", InputTitle);
            AttributesInput.Add("IconName", InputIconName);
            AttributesInput.Add("CopmItems", _inputItems);
            AttributesInput.Add("LabelWidth", DialogLabelWidth);
        }

        #endregion
    }
}
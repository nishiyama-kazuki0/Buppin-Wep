using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using SharedModels;
using Sotsera.Blazor.Toaster.Core.Models;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// 作業者マスタ/自動倉庫棚メンテナンスダイアログ
/// 西山
/// </summary>
/// 

public partial class DialogPersonFixContent2 : DialogCommonInputContent
{
    protected override async Task OnInitializedAsync()
    {
        if (Mode == enumDialogMode.Edit)
        {
            // 編集の時はパスワードを非表示にする
            Components = Components.Where(_ => _.Property != "パスワード").ToList();
        }

        await base.OnInitializedAsync();
    }

    protected override async Task<bool> ShowConfirmationAsync(string message,string summary)//物品追加
    {
        
        bool? result;
        message = "棚を登録しますか？";

        if (PasswordCheck)
            result = await ComService.DialogShowPassword(message, summary,540,240);
        else
            result = await ComService.DialogShowYesNo(message, summary,540,240);

        return result is not null && (bool)result;
    }

    #region override

    
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
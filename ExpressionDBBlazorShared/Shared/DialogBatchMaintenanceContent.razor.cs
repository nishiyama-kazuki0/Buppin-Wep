using ExpressionDBBlazorShared.Data;
using ExpressionDBBlazorShared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SharedModels;
using Sotsera.Blazor.Toaster.Core.Models;
using System.Text;

namespace ExpressionDBBlazorShared.Shared;

/// <summary>
/// バッチメンテナンス：追加
/// 追加用画面のため、編集は考慮していない
/// </summary>
public partial class DialogBatchMaintenanceContent : DialogCommonGridInputContent
{
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
        // override:必須項目のみに表示を限定(バッチヘッダのデータに必須な項目のみ)
        List<ComponentColumnsInfo> listInfo = Mode == enumDialogMode.Edit
            ? Components
                .Where(_ => _.IsEdit == true && _.EditType == 1 && _.EditInputRequired)
                .OrderBy(_ => _.EditDialogLayoutGroup)
                .ThenBy(_ => _.EditDialogLayoutDispOrder)
                .ToList()
            : Components
                .Where(_ => _.IsEdit == true && (_.EditType == 1 || _.EditType == 2) && _.EditInputRequired)
                .OrderBy(_ => _.EditDialogLayoutGroup)
                .ThenBy(_ => _.EditDialogLayoutDispOrder)
                .ToList();

        // コンポーネント情報を作成
        _inputItems = await ComService.GetCompItemInfo(listInfo, InitialData, Components, ComponentsInfos, false, false);

        // 入力エリアAttributes設定
        AttributesInput.Add("AllowCollapse", InputAllowCollapse);
        AttributesInput.Add("GroupTitle", InputTitle);
        AttributesInput.Add("IconName", InputIconName);
        AttributesInput.Add("CopmItems", _inputItems);
        AttributesInput.Add("LabelWidth", DialogLabelWidth);
    }
}

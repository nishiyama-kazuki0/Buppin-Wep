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

public partial class DialogPersonFixContent3 : DialogCommonInputContent
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
        message = "編集内容を確定しますか？";

        if (PasswordCheck)
            result = await ComService.DialogShowPassword(message, summary,540,240);
        else
            result = await ComService.DialogShowYesNo(message, summary,540,240);

        return result is not null && (bool)result;
    }

    
}
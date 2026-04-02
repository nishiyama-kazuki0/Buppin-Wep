using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace ExpressionDBBlazorShared.Validator;

public class RegexValidator : FieldValidatorBase<string?>
{
    public override string? ErrorMessage => $"{ItemName}の入力値が不正です。";

    [Parameter]
    public string PatternString { get; set; } = string.Empty;

    protected override bool ValidateValue(string? value)
    {
        //PatternStringが空の場合は必須チェックのみ行う?
        return string.IsNullOrEmpty(PatternString) || new RegularExpressionAttribute(PatternString).IsValid(value);
    }
}

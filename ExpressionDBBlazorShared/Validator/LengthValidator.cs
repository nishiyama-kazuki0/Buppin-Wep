using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Validator;

public class LengthValidator : FieldValidatorBase<string?>
{
    public override string? ErrorMessage => $"{ItemName} は最大{MaxLength}文字です。";

    [Parameter]
    public int MinLength { get; set; } = int.MinValue;

    [Parameter]
    public int MaxLength { get; set; } = int.MaxValue;

    protected override bool ValidateValue(string? value)
    {
        return value != null && value.Length >= MinLength && value.Length <= MaxLength;
    }
}

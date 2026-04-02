using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace ExpressionDBBlazorShared.Validator;

public class ByteLengthValidator : FieldValidatorBase<string?>
{
    public override string? ErrorMessage => $"{ItemName}に入力可能な文字数(全角{MaxZen}、半角{MaxLength})を超えています。";

    [Parameter]
    public long? MaxLength { get; set; } = 0;

    public long MaxZen => MaxLength == null ? 0 : (long)MaxLength / 2;

    protected override bool ValidateValue(string? value)
    {
        // SQLServerの照合順序がJapanese_CI_ASであるため
        // Shift_JISでのバイト数を確認する
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding enc = Encoding.GetEncoding("Shift_JIS");
        int cnt = enc.GetByteCount(value ?? string.Empty);
        return cnt <= MaxLength;
    }
}

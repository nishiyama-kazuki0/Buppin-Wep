using Microsoft.AspNetCore.Components;

namespace ExpressionDBBlazorShared.Validator;

public class NumericRangeValidator : FieldValidatorBase<object?>
{
    public override string? ErrorMessage => decimal.TryParse(Min.ToString(), out decimal valMin) && decimal.TryParse(Max.ToString(), out decimal valMax)
                ? $"{ItemName} は[{string.Format(FormatMinMax, valMin)}]～[{string.Format(FormatMinMax, valMax)}]で入力してください。"
                : $"{ItemName} は[{Min}]～[{Max}]で入力してください。";

    /// <summary>
    /// Specifies the minimum value. The component value should be greater than the minimum in order to be valid.
    /// </summary>
    [Parameter]
    public required IComparable Min { get; set; }

    /// <summary>
    /// Specifies the maximum value. The component value should be less than the maximum in order to be valid.
    /// </summary>
    [Parameter]
    public required IComparable Max { get; set; }

    /// <summary>
    /// 最大最小の表示フォーマット
    /// </summary>
    [Parameter]
    public string FormatMinMax { get; set; } = "{0:F0}";

    protected override bool ValidateValue(object? value)
    {
        if (Min == null && Max == null)
        {
            throw new ArgumentException("Min and Max cannot be both null");
        }

        if (value == null)
        {
            throw new ArgumentException("value is null");
        }

        if (string.IsNullOrWhiteSpace(value.ToString()))
        {
            return true;
        }

        if (!decimal.TryParse(value.ToString(), out _))
        {
            ErrorMessage2 = $"{ItemName} は数値で入力してください。";
            return false;
        }

        if (Min != null)
        {
            if (!TryConvertToType(value, Min.GetType(), out object? convertedValue) || Min.CompareTo(convertedValue) > 0)
            {
                return false;
            }
        }

        if (Max != null)
        {
            if (!TryConvertToType(value, Max.GetType(), out object? convertedValue) || Max.CompareTo(convertedValue) < 0)
            {
                return false;
            }
        }

        return true;
    }

    private bool TryConvertToType(object value, Type type, out object? convertedValue)
    {
        try
        {
            convertedValue = Convert.ChangeType(value, type);
            return true;
        }
        catch (Exception ex) when (ex is InvalidCastException or OverflowException)
        {
            convertedValue = null;
            return false;
        }
    }
}

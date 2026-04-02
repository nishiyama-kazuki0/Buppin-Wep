namespace ExpressionDBBlazorShared.Validator;

public class RequiredValidator : FieldValidatorBase<object?>
{
    public override string? ErrorMessage => $"{ItemName} は必須です。";

    protected override bool ValidateValue(object? value)
    {
        if (value is null)
        {
            return false;
        }

        bool ret = true;
        if (value!.GetType() == typeof(string))
        {
            ret = !string.IsNullOrWhiteSpace((string)value);
        }
        else if (value!.GetType().GetGenericTypeDefinition() == typeof(List<>))
        {
            ret = ((List<string>)value).Count() > 0;
        }
        else if (value!.GetType() == typeof(decimal))
        {
        }
        else if (value!.GetType() == typeof(DateTime))
        {
        }

        return ret;
    }
}

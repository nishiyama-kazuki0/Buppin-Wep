using SharedModels;

namespace ExpressionDBCycleProcessApp.Services;

internal static class ConvertUtil
{
    #region データ変換

    /// <summary>
    /// DB取得データから文字列を取得
    /// </summary>
    /// <param name="resp"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public static string GetValueString(object? obj)
    {
        return obj is not null ? obj!.ToString()! : "";
    }
    public static string GetValueString(IDictionary<string, object> resp, string column)
    {
        _ = resp.TryGetValue(column, out object? obj);
        return GetValueString(obj);
    }
    public static string GetValueString(ResponseValue resp, string column)
    {
        return GetValueString(resp.Values, column);
    }

    /// <summary>
    /// DB取得データから文字列を取得
    /// </summary>
    /// <param name="resp"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public static bool GetValueBool(object? obj)
    {
        return obj is not null && ConvertBool(obj!.ToString()!);
    }
    public static bool GetValueBool(IDictionary<string, object> resp, string column)
    {
        _ = resp.TryGetValue(column, out object? obj);
        return GetValueBool(obj);
    }
    public static bool GetValueBool(ResponseValue resp, string column)
    {
        return GetValueBool(resp.Values, column);
    }
    public static bool ConvertBool(string value)
    {
        _ = bool.TryParse(value, out bool result);
        return result;
    }

    /// <summary>
    /// DB取得データからByteを取得
    /// </summary>
    /// <param name="resp"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public static byte GetValueByte(object? obj)
    {
        return obj is not null ? ConvertByte(obj!.ToString()!) : (byte)0;
    }
    public static byte GetValueByte(IDictionary<string, object> resp, string column)
    {
        _ = resp.TryGetValue(column, out object? obj);
        return GetValueByte(obj);
    }
    public static byte GetValueByte(ResponseValue resp, string column)
    {
        return GetValueByte(resp.Values, column);
    }
    public static byte ConvertByte(string value)
    {
        _ = byte.TryParse(value, out byte result);
        return result;
    }

    /// <summary>
    /// DB取得データから数値を取得
    /// </summary>
    /// <param name="resp"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public static int GetValueInt(object? obj)
    {
        return obj is not null ? ConvertInt(obj!.ToString()!) : 0;
    }
    public static int GetValueInt(IDictionary<string, object> resp, string column)
    {
        _ = resp.TryGetValue(column, out object? obj);
        return GetValueInt(obj);
    }
    public static int GetValueInt(ResponseValue resp, string column)
    {
        return GetValueInt(resp.Values, column);
    }
    public static int ConvertInt(string value)
    {
        _ = int.TryParse(value, out int result);
        return result;
    }

    /// <summary>
    /// DB取得データから数値を取得
    /// </summary>
    /// <param name="resp"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public static decimal? GetValueDecimalNull(object? obj)
    {
        return obj is not null && !string.IsNullOrEmpty(obj.ToString()) ? ConvertDecimal(obj!.ToString()!) : null;
    }
    public static decimal? GetValueDecimalNull(IDictionary<string, object> resp, string column)
    {
        _ = resp.TryGetValue(column, out object? obj);
        return GetValueDecimalNull(obj);
    }
    public static decimal? GetValueDecimalNull(ResponseValue resp, string column)
    {
        return GetValueDecimalNull(resp.Values, column);
    }
    public static decimal ConvertDecimal(string value)
    {
        _ = decimal.TryParse(value, out decimal result);
        return result;
    }

    /// <summary>
    /// DB取得データから数値を取得
    /// </summary>
    /// <param name="resp"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public static Type? GetValueType(object? obj)
    {
        return obj is not null ? ConvertType(obj!.ToString()!) : typeof(string);
    }
    public static Type? GetValueType(IDictionary<string, object> resp, string column)
    {
        _ = resp.TryGetValue(column, out object? obj);
        return GetValueType(obj);
    }
    public static Type? GetValueType(ResponseValue resp, string column)
    {
        return GetValueType(resp.Values, column);
    }
    public static Type? ConvertType(string value)
    {
        Type? result = null;
        try
        {
            result = Type.GetType(value);
        }
        catch (Exception)
        {
            //_ = WebComService.PostLogAsync(ex.Message);
        }
        return result;
    }

    /// <summary>
    /// DB取得データからEnumを取得
    /// </summary>
    /// <param name="resp"></param>
    /// <param name="column"></param>
    /// <returns></returns>
    public static T GetValueEnum<T>(object? obj) where T : new()
    {
        T ret = new();
        if (obj is not null)
        {
            string strEnumStr = obj.ToString()!;
            string strEnumStrPos = strEnumStr[(strEnumStr.LastIndexOf('.') + 1)..];
            ret = GetEnumValue<T>(strEnumStrPos);
        }
        return ret;
    }
    public static T GetValueEnum<T>(IDictionary<string, object> resp, string column) where T : new()
    {
        _ = resp.TryGetValue(column, out object? obj);
        return GetValueEnum<T>(obj);
    }
    public static T GetValueEnum<T>(ResponseValue resp, string column) where T : new()
    {
        return GetValueEnum<T>(resp.Values, column);
    }

    public static T GetEnumValue<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value);
    }
    #endregion

}

namespace SharedModels;

public class ArgumentValue
{

    /// <summary>
    /// 要求ファンクション名
    /// ※クライアント側からはセットできないかもしれないが、一旦変数作成。
    /// </summary>
    public string RequestFunctionName { get; set; } = string.Empty;

    /// <summary>
    /// 変数名
    /// </summary>
    public string ArgumentName { get; set; } = null!;
    /// <summary>
    /// 引数値
    /// </summary>
    public object Value { get; set; } = null!;
    /// <summary>
    /// 引数データタイプ
    /// </summary>
    public string DataTypeString { get; set; } = string.Empty;

    //JSONの関係でメンバーはすべてpublicで宣言必要とのこと。
    //TODO publicの場合はコンストラクタやCreateメソッドを定義する意味が薄れるので、削除するか検討。

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="requestFunctionName"></param>
    /// <param name="argumentName"></param>
    /// <param name="value"></param>
    /// <param name="dataTypeString"></param>
    public ArgumentValue(
        string requestFunctionName
        , string argumentName
        , object value
        , string dataTypeString
        )
    {
        RequestFunctionName = requestFunctionName;
        ArgumentName = argumentName;
        Value = value;
        DataTypeString = dataTypeString;
    }

    /// <summary>
    /// ArgumentValueオブジェクトを作成して返します。
    /// </summary>
    /// <param name="argumentName"></param>
    /// <param name="value"></param>
    /// <param name="dataTypeString"></param>
    /// <returns></returns>
    public static ArgumentValue CreateArgumentValue(string argumentName, object value, string dataTypeString)
    {
        ArgumentValue argumentValue = new(
            string.Empty //TODO 必要か検討
            , argumentName
            , value ?? string.Empty
            , dataTypeString
            );
        return argumentValue;
    }
    /// <summary>
    /// 自身をクローンして返す
    /// </summary>
    /// <returns></returns>
    public ArgumentValue Clone()
    {
        ArgumentValue clone = new(
            RequestFunctionName,
            ArgumentName,
            Value,
            DataTypeString
        );

        return clone;
    }
}

namespace SharedModels;

public class RequestValue
{
    /// <summary>
    /// 要求プログラム名
    /// </summary>
    public string RequestProgramName { get; set; } = null!;
    /// <summary>
    /// 引数
    /// キーは変数名と同じ内容が格納されるとする
    /// </summary>
    public Dictionary<string, ArgumentValue> ArgumentValues { get; set; } = [];

    /// <summary>
    /// グリッドなど選択したデータセット
    /// intはrowcoutを想定。Listの入れ子でも良いかもしれない。
    /// 未使用時はカウント0(Any()==falseで判断する)
    /// </summary>
    public Dictionary<string, SortedList<int, List<ArgumentValue>>> ArgumentDataSet { get; set; } = [];
    /// <summary>
    /// ArgumentDataSetをInsert(BulkCopy)するテーブル名
    /// 未使用時はString.empty
    /// WebAPI側で結合したときに取得するようにすれば不要かもしれない。というのも、クライアント側でわからない可能性が高い。
    /// もしくはBulkCopyをせずとも、WebAPI側でdotNetメソッド呼び出し、Linqで必要データのみ処理していく形の実装の方がパフォーマンスが良いかもしれない
    /// </summary>
    public HashSet<string> TagetTableNamesForDataSet { get; set;  } = [];
    /// <summary>
    /// ExecutionControllerのPostにてキャンセルトークンの判定を無視させるフラグ
    /// true:無視する false:無視しない
    /// </summary>
    public bool IsCancelTokenIgnore { get; set; } = false;

    //TODO 要求元のクライアント名、ユーザー名が必要か検討する

    //JSONの関係でメンバーはすべてpublicで宣言必要とのこと。
    //TODO publicの場合はコンストラクタやCreate,Setメソッドを定義する意味が薄れるので、削除するか検討。

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public RequestValue()
    {
    }

    /// <summary>
    /// RequestValueオブジェクトを作成して返します。
    /// </summary>
    /// <param name="requestProgramName"></param>
    /// <returns></returns>
    public static RequestValue CreateRequestProgram(string requestProgramName)
    {
        RequestValue requestValue = new()
        {
            RequestProgramName = requestProgramName
        };
        return requestValue;
    }

    /// <summary>
    /// 自身のRequestValueにArgumentValueを作成してセット追加します。すでにキーが存在する場合は上書きする
    /// </summary>
    /// <param name="argumentName"></param>
    /// <param name="value"></param>
    /// <param name="dataTypeString"></param>
    public RequestValue SetArgumentValue(string argumentName, object value, string dataTypeString)
    {
        ArgumentValue argumentValue = ArgumentValue.CreateArgumentValue(argumentName, value, dataTypeString);
        if (ArgumentValues.ContainsKey(argumentName))
        {
            ArgumentValues[argumentName] = argumentValue;
        }
        else
        {
            ArgumentValues.Add(argumentName, argumentValue);
        }
        return this;
    }
    /// <summary>
    /// 自身のRequestValueにArgumentValueを作成してセット追加します。すでにキーが存在する場合は上書きする
    /// </summary>
    /// <param name="argumentValue"></param>
    /// <returns></returns>
    public RequestValue SetArgumentValue(ArgumentValue argumentValue)
    {
        if (ArgumentValues.ContainsKey(argumentValue.ArgumentName))
        {
            ArgumentValues[argumentValue.ArgumentName] = argumentValue;
        }
        else
        {
            ArgumentValues.Add(argumentValue.ArgumentName, argumentValue);
        }
        return this;
    }

    /// <summary>
    /// 自身のRequestValueにArgumentDataSetを作成してセット追加します。
    /// </summary>
    /// <param name="tableNameForDataSet"></param>
    /// <param name="argumentValues"></param>
    public RequestValue SetArgumentDataset(string tableNameForDataSet, List<List<ArgumentValue>> argumentValues)
    {
        TagetTableNamesForDataSet.Add(tableNameForDataSet);
        //argumentValuesをArgumentDataSetにセットする。List<ArgumentValue>分ループし、キーは連番値でセットする。
        var argumentDataSet = new SortedList<int, List<ArgumentValue>>();
        for (int i = 0; i < argumentValues.Count; i++)
        {
            argumentDataSet.Add(i, argumentValues[i]);
        }
        ArgumentDataSet.Add(tableNameForDataSet, argumentDataSet);
        return this;
    }

    /// <summary>
    /// 自身のクローンオブジェクトを作成して返す
    /// </summary>
    /// <returns></returns>
    public RequestValue Clone()
    {
        RequestValue clone = new()
        {
            RequestProgramName = RequestProgramName,
            //データセットは負荷が高いので一旦コメント化。操作ログ、通知ログ登録のためにクローンを作成のため。
            //ArgumentDataSet = new SortedList<int, List<ArgumentValue>>(this.ArgumentDataSet) 
        };
        clone.TagetTableNamesForDataSet = new(this.TagetTableNamesForDataSet);

        //ArgumentValuesをクローンする
        foreach (KeyValuePair<string, ArgumentValue> a in ArgumentValues)
        {
            _ = clone.SetArgumentValue(a.Value.Clone());
        }

        return clone;
    }
}

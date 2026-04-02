namespace SharedModels;

public class ResponseValue
{
    public IList<string> Columns { get; set; }
    public IDictionary<string, object> Values { get; set; }
    public ResponseValue()
    {
        Columns = [];
        Values = new Dictionary<string, object>();
    }
}

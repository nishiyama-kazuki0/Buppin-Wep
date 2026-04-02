namespace ZennohBlazorShared.Data
{
    public class ResponseValue
    {
        public IList<string> Columns { get; set; }
        public IDictionary<string, object> Values { get; set; }
        public ResponseValue()
        {
            Columns = new List<string>();
            Values = new Dictionary<string, object>();
        }
    }
}

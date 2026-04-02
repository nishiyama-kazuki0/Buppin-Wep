using System.Reflection;
using System.Xml.Linq;

namespace ExpressionDBCycleProcessApp.Maintenance;

/// <summary>
/// XMLドキュメントの解析を行うクラス(本案件コード特有)
/// </summary>
internal class DocumentCommentAnalizer : DocumentCommentAnalizerBase
{
    internal DocumentCommentAnalizer(string xmlFilePath) : base(xmlFilePath) {}

    public string GetSeqIDSummary(string seqID) 
        => GetFieldSummaryStringByName($"F:ExpressionDBCycleProcessApp.MELSECComm.SequenceProcess.SequenceID.{seqID}");

    /// <summary>
    /// 通信Areaのフィールドのコメント取得
    /// </summary>
    /// <param name="areaName"></param>
    /// <param name="fieldClassName"></param>
    /// <returns></returns>
    public string GetDeviceSummary(string areaName , string fieldClassName)
        => GetFieldSummaryStringByName($"F:ExpressionDBCycleProcessApp.MELSECComm.DeviceArea.{areaName}.{fieldClassName}");
}
/// <summary>
/// XMLドキュメントの解析を行うクラス(汎用コードのみ)
/// </summary>
internal class DocumentCommentAnalizerBase
{
    //private string _document;
    XElement _xmlDocument;

    internal DocumentCommentAnalizerBase(string xmlFilePath)
    {
        //_document = System.IO.File.ReadAllText();
        _xmlDocument = XElement.Load(xmlFilePath);
    }

    public string GetFieldSummaryStringByName(string memberName)
    {
        var member = _xmlDocument.Descendants("member")
                        .FirstOrDefault(m => (string?)m.Attribute("name") == memberName);

        var summary = member?.Element("summary");
        return summary?.Value.Trim();
    }
    public FieldSummary GetFieldSummary(FieldInfo fi)
    {
        var memberName = $"F:{fi.DeclaringType.FullName}.{fi.Name}";
        var member = _xmlDocument.Descendants("member")
                        .FirstOrDefault(m => (string?)m.Attribute("name") == memberName);

        var summary = member?.Element("summary");
        var result = new FieldSummary();
        if (summary != null)
        {
            var strSummary = summary?.Value.Trim();
            result.Para = summary.Elements("para")
                    .Select(p => p.Value.Trim())
                    .ToArray();
            foreach (var r in result.Para)
            {
                if (string.IsNullOrWhiteSpace(r))
                {
                    continue;
                }
                strSummary = strSummary.Replace(r, "");
            }
            result.Summary = strSummary;
        }
        return result;
    }

    public string GetClassSummary(Type type)
    {
        var memberName = $"T:{type.FullName}";
        var member = _xmlDocument.Descendants("member")
                        .FirstOrDefault(m => (string?)m.Attribute("name") == memberName);
        var summary = member?.Element("summary")?.Value.Trim();
        return summary;
    }
}


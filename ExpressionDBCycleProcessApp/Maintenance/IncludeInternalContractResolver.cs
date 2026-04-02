using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ExpressionDBCycleProcessApp.Maintenance;

// internal プロパティも対象にする ContractResolver
public class IncludeInternalContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);

        // internal もシリアライズ可能に
        prop.Readable = true;
        prop.Writable = true;

        return prop;
    }

    protected override List<MemberInfo> GetSerializableMembers(Type objectType)
    {
        // internal, private プロパティも含める
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        return objectType.GetProperties(flags).Cast<MemberInfo>().ToList();
    }
}

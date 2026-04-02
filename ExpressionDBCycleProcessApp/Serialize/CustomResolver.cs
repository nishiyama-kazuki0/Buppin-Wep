using SharedModels;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ExpressionDBCycleProcessApp.Serialize;

// カスタム TypeInfoResolver を定義
public class CustomResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var typeInfo = base.GetTypeInfo(type, options);

        if (type == typeof(ClassNameSelect))
        {
            // "GetModelType" プロパティを除外
            typeInfo.Properties.Remove(
                typeInfo.Properties.FirstOrDefault(p => p.Name == "GetModelType")
            );
        }

        return typeInfo;
    }
}


//GetModelType
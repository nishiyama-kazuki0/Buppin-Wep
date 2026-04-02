using Anotar.Serilog;
using ExpressionDBCycleProcessApp.MELSECComm.SequenceProcess;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace ExpressionDBCycleProcessApp.Serialize;
internal static  class SequenceActionDeserialize
{
    static JsonSerializerOptions options = new JsonSerializerOptions
    {
        TypeInfoResolver = new CustomResolver()
    };

    //
    internal static void Exec(string LoadDataFolder, SequenceAction action)
    {
        //SequenceAction毎の処理
        var fileName = $"{action.GetType().Name}_{action.InstanceId}.json";
        var path = Path.Combine(LoadDataFolder, fileName);
        if (File.Exists(path) == false)
        {
            LogTo.Error($"ファイル「{path}」が見つかりませんでした。");
            return;
        }
        var json = File.ReadAllText(path);

        using JsonDocument document = JsonDocument.Parse(json);
        TraverseJson(document.RootElement, null, action, null, 0, "");
        //currentStepの復元
        {
            var tmp = JsonSerializer.Deserialize<SequenceAction2>(json, options);
            var array = tmp.CurrentStepString.Split('@');
            var name = array[0];
            var stepName = array[1];
            //GetSequenceStep
            if (Enum.TryParse(name, out SequenceID result))
            {
                //currentStepの設定
                var step = SequenceStep.GetSequenceStep(result, stepName);
                action.currentStep = step;
            }
            else
            {
                LogTo.Error($"SequenceID : {name} は無効なSequenceID名です");
            }
        }

        //JSONから属性を取得し、同じ名称のプロパティを探す。
        //なければ、警告ログを出力

        //名称がCurrentStepStringならスキップ

        //あれば、型を調べる
        //型がSequenceActionならスキップ
        //

        //
    }

    static void TraverseJson(JsonElement element, object? parent, object target, PropertyInfo? pi, int level, string path)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    var type = target.GetType();
                    if (level > 0 && target is SequenceAction)
                    {
                        Console.WriteLine($"SubSequenceはスキップ");
                    }
                    else if (target is DBAccessGetReponceItemsSequence)
                    {
                        Console.WriteLine($"DBAccessGetReponceItemsSequenceはスキップ");
                    }
                    else if (target is DBAccessRequestProgramSequence)
                    {
                        Console.WriteLine($"DBAccessGetReponceItemsSequenceはスキップ");
                    }
                    else
                    {
                        var prop = type.GetProperty(property.Name);
                        if (prop == null)
                        {
                            LogTo.Error($"{property.Name}の取得に失敗");
                        } else
                        {
                            var value = prop.GetValue(target);
                            TraverseJson(property.Value, target, value, prop, level + 1, AppendPath(path, property.Name));
                        }
                    }
                }
                break;

            case JsonValueKind.Array:
                {
                    if (level == 1 && path == "GroupKeys")
                    {
                        //無視
                    } else
                    {
                        int index = 0;
                        foreach (var item in element.EnumerateArray())
                        {
                            TraverseJson(item, parent, target, pi, level, $"{path}[{index}]");
                            index++;
                        }
                    }
                }
                break;

            default:
                {
                    if (level == 1 && path == "CurrentStepString")
                    {

                    }
                    else if (level == 1 && path == "InstanceId")
                    {
                        //無視
                    }
                    else
                    {
                        Console.WriteLine($"{path}: {element}");
                        if (pi.CanWrite)
                        {
                            if (pi.PropertyType == typeof(int))
                            {
                                pi.SetValue(parent, element.GetInt32());
                            }
                            else if (pi.PropertyType == typeof(short))
                            {
                                pi.SetValue(parent, element.GetInt16());
                            }
                            else if (pi.PropertyType == typeof(string))
                            {
                                pi.SetValue(parent, element.GetString());
                            }
                        }
                    }
                }
                break;
        }
    }

    static string AppendPath(string path, string name)
    {
        return string.IsNullOrEmpty(path) ? name : $"{path}.{name}";
    }
}


class Program2
{
    static void Main()
    {
        string json = @"
        {
            ""name"": ""John"",
            ""age"": 30,
            ""address"": {
                ""city"": ""New York"",
                ""zip"": ""10001""
            },
            ""phones"": [""123-4567"", ""987-6543""],
            ""active"": true
        }";

    }
}

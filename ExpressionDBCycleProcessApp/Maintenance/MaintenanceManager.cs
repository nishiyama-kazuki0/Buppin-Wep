using ExpressionDBCycleProcessApp.MELSECComm;
using ExpressionDBCycleProcessApp.MELSECComm.SequenceProcess;
using ExpressionDBCycleProcessApp.ViewModels;
using McProtocolCore;
using McProtocolCore.Device;
using R3;
using Serilog;
using System.Collections.Concurrent;
using System.Data;
using System.Reflection;

namespace ExpressionDBCycleProcessApp.Maintenance;

/// <summary>
/// メンテナンス表示の管理用クラス
/// </summary>
internal class MaintenanceManager
{
    /// <summary>ソースコードのフォルダ</summary>
    public static string SourceFolder { get; set; } = string.Empty;

    /// <summary>デバイス通信Areaのリスト</summary>
    internal static List<CommunicationAreaViewModel> 通信Area { get; } = new();
    /// <summary>フィールド情報のDictionary</summary>
    internal Dictionary<string, FieldInfoViewModel> Devices { get; } = new();
    /// <summary>SequenceStepのDictionary</summary>
    internal static Dictionary<SequenceStep, SequenceAction> Steps { get; } = new();
    /// <summary>ViewModelは１度だけ定義するための処理</summary>
    internal static ConcurrentDictionary<SequenceAction, SequenceActionViewModel> SequenceActionViewModels { get; } = new();
    internal static SequenceActionViewModel Root;

    MaterialHandlingManager _materialHandlingManager;
    MaterialHandlingData _materialHandlingData;

    static IServiceProvider _sp;

    internal readonly DocumentCommentAnalizer documentCommentAnalizer;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="sp"></param>
    public MaintenanceManager(IServiceProvider sp)
    {
        _sp = sp;
        _materialHandlingManager = sp.GetRequiredService<MaterialHandlingManager>();
        _materialHandlingData = sp.GetRequiredService<MaterialHandlingManager>().MaterialHandlingData;

        documentCommentAnalizer = new DocumentCommentAnalizer("ExpressionDBCycleProcessApp.xml");
    }

    /// <summary>
    /// CommunicationAreaViewModelの初期化
    /// </summary>
    /// <param name="sp"></param>
    public void InitCommunicationAreaViewModels(IServiceProvider sp)
    {
        var t = _materialHandlingData.GetType();
        var fieldInfos = t.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var fieldInfo in fieldInfos)
        {
            var fieldobj = fieldInfo.GetValue(_materialHandlingData) as AbstractCommunicationArea;
            通信Area.Add(new(sp, fieldInfo, fieldobj));
        }
    }

    /// <summary>
    /// Fieldのデバイスの種別に応じたViewModelを生成して返す
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="fi"></param>
    /// <returns></returns>
    public CDeviceViewModel CreateDeviceViewModel(IServiceProvider sp, FieldInfoViewModel fi)
    {
        if (fi == null)
        {
            return null;
        }
        //Selected通信Area.Value;
        var (area, field, summary, fieldValue) = AnalizeDeviceArea(
                fi.Area,
                fi.Name);
        if (area == null || field == null || fieldValue == null)
        {
            return null;
        }
        CDeviceViewModel resultVm;
        var type = field.GetType();
        if (type == typeof(CWord))
        {
            var vm = new CWordViewModel(sp);
            resultVm = vm;

            vm.Area.Value = area;
            vm.Field.Value = field;
            var result = area.GetInt16(field);
            vm.Value.Value = result;
        }
        else if (type == typeof(CDword))
        {
            var vm = new CDwordViewModel(sp);
            resultVm = vm;

            vm.Area.Value = area;
            vm.Field.Value = field;
            var result = area.GetInt32(field);
            vm.Value.Value = result;
        }
        else if (type == typeof(CMultiBits))
        {
            var vm = new CMultiBitsViewModel(sp);
            resultVm = vm;
        }
        else if (type == typeof(CBit))
        {
            var vm = new CBitViewModel(sp);
            resultVm = vm;

            vm.Area.Value = area;
            vm.Field.Value = field;
            var result = area.GetBit(field);
            vm.IsChecked.Value = result;
        }
        else
        {
            return null;
        }

        resultVm.Summary.Value = summary;
        resultVm.FieldValue.Value = fieldValue;

        return resultVm;
    }

    private static readonly Serilog.ILogger _logger = Log.ForContext<MaintenanceManager>();
    /// <summary>
    /// SequenceActionのStepが変更されたときに呼び出される
    /// ここではSTEPの遷移をCSV形式でログに出力する
    /// </summary>
    /// <param name="action">対象のSequenceAction</param>
    /// <param name="oldStep">遷移元</param>
    /// <param name="newStep">遷移先</param>
    internal static void OnStepChanged(SequenceAction action, SequenceStep oldStep, SequenceStep newStep)
    {
        var vm = SequenceActionViewModel.GetOrCreateSequenceActionViewModel(_sp, action, false);
        var oldFieldName = vm.Steps.FirstOrDefault(m => m.sequenceStep == oldStep)?.fi?.Name;
        var newFieldName = vm.Steps.FirstOrDefault(m => m.sequenceStep == newStep)?.fi?.Name;

        _logger.Verbose($"{action.InstanceId},{action.GetType().Name},{action.ID},{oldStep.Name},{oldFieldName},{newStep.Name},{newFieldName}");
    }

    /// <summary>
    /// デバイス通信Areaの解析処理
    /// </summary>
    /// <param name="areaName">対象のデバイスエリア名</param>
    /// <param name="fieldClassName">対象フィールドのクラス名</param>
    /// <returns></returns>
    internal (AbstractCommunicationArea area, object field, string summary, string a) AnalizeDeviceArea(string areaName, string fieldClassName)
    {
        var summary = documentCommentAnalizer.GetDeviceSummary(areaName, fieldClassName);

        // プロパティの名前を指定して取得
        var fieldName1 = $"{areaName}Buf";
        // 通信エリアのインスタンスを取得
        var area = GetFieldValue(_materialHandlingManager.MaterialHandlingData, fieldName1) as AbstractCommunicationArea;
        // 通信エリア内のフィールドのインスタンスを取得
        var field = GetFieldValue(area, $"{fieldClassName}");
        // フィールドの型を取得
        var type = field.GetType();
        if (type == typeof(CMultiDword))
        {
        }
        else if (type == typeof(CWord))
        {
            var result = area.GetInt16(field);
            return (area, field, summary, result.ToString());
        }
        else if (type == typeof(CDword))
        {
            var result = area.GetInt32(field);
            return (area, field, summary, result.ToString());
        }
        else if (type == typeof(CMultiBits))
        {
        }
        else if (type == typeof(CBit))
        {
            var result = area.GetBit(field);
            return (area, field, summary, result.ToString());
        }

        return (null, null, summary, "");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj">対象クラスのインスタンス</param>
    /// <param name="name">対象フィールド名</param>
    /// <returns></returns>
    static object? GetFieldValue(object obj, string name)
    {
        if (obj == null || string.IsNullOrEmpty(name))
            return null;

        var type = obj.GetType();
        var fi = obj.GetType().GetField(name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        return fi?.GetValue(obj);
    }

    public string GetClassSummary(Type type) => documentCommentAnalizer.GetClassSummary(type);

    public FieldSummary GetFieldSummary(FieldInfo fi) => documentCommentAnalizer.GetFieldSummary(fi);

    public string GetFieldSummaryStringByName(string memberName) => documentCommentAnalizer.GetFieldSummaryStringByName(memberName);

    /// <summary>
    /// SequenceActionのメンバのStepフィールドのリストを取得して保持する。
    /// </summary>
    public List<(SequenceStep ss, FieldInfo fi)> GetSequenceStepList(SequenceAction action)
    {
        var type = action.GetType();
        return action.GetType().GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(m => m.FieldType.IsAssignableFrom(typeof(SequenceStep))).Select(m => (m.GetValue(action) as SequenceStep, m)).ToList();
    }
}

/// <summary>
/// MaintenanceManagerの拡張メソッド
/// </summary>
public static class MaintenanceManagerExtensions
{
    public static bool GetBit(this AbstractCommunicationArea area, object field) => area.GetValue<bool>(field, "getBit", new Type[] { typeof(CBit) });
    public static short GetInt16(this AbstractCommunicationArea area, object field) => area.GetValue<short>(field, "getInt16", new Type[] { typeof(CWord) });
    public static short[] GetInt16(this AbstractCommunicationArea area, object field, int count)
    {
        var list = new List<short>();
        for (var i = 0; i < count; i++)
        {
            list.Add(area.GetValue<short>(field, i, "getInt16", new Type[] { typeof(CMultiWord), typeof(int) }));
        }
        return list.ToArray();
    }
    public static int GetInt32(this AbstractCommunicationArea area, object field) => area.GetValue<int>(field, "getInt32", new Type[] { typeof(CDword) });
    public static int[] GetInt32(this AbstractCommunicationArea area, object field, int count)
    {
        var  list = new List<int>();
        for (var i = 0; i < count; i++)
        {
            list.Add(area.GetValue<int>(field, i, "getInt32", new Type[] { typeof(CMultiDword), typeof(int) }));
        }
        return list.ToArray();
    }

    public static void SetBit(this AbstractCommunicationArea area, object field, bool value) 
        => area.SetValue<bool>(field, "commSetBit", value, new Type[] { typeof(bool), typeof(CBit) });

    public static void SetInt16(this AbstractCommunicationArea area, object field, short value)
        => area.SetValue<short>(field, "commSetInt16", value, new Type[] { typeof(short), typeof(CWord) });

    public static void SetInt32(this AbstractCommunicationArea area, object field, int value)
        => area.SetValue<int>(field, "commSetInt32", value, new Type[] { typeof(int), typeof(CDword) });

    public static void SetValue<T>(this AbstractCommunicationArea area, object field, string methodName, T value, Type[]? types = null)
    {
        if (types == null)
        {
            types = Type.EmptyTypes;
        }
        //Reflectionでメソッド"getBit"を呼出し、結果をaに代入する。
        var methodInfo = area.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, types);
        if (methodInfo != null)
        {
            _ = methodInfo.Invoke(area, new object[] { value, field });
        }
    }

    public static T GetValue<T>(this AbstractCommunicationArea area, object field, int index, string methodName, Type[]? types = null)
    {
        if (types == null)
        {
            types = Type.EmptyTypes;
        }
        //Reflectionでメソッド"getBit"を呼出し、結果をaに代入する。
        var methodInfo = area.GetType().GetMethod(methodName,
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
            types);
        if (methodInfo != null)
        {
            var result = methodInfo.Invoke(area, new object[] { field, index });
            return (T)result;
        }
        throw new Exception("Method not found");
    }

    public static T GetValue<T>(this AbstractCommunicationArea area, object field, string methodName, Type[]? types = null)
    {
        if ( types == null)
        {
            types = Type.EmptyTypes;
        }
        //Reflectionでメソッド"getBit"を呼出し、結果をaに代入する。
        var methodInfo = area.GetType().GetMethod(methodName, 
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
            types);
        if (methodInfo != null)
        {
            var result = methodInfo.Invoke(area, new object[] { field });
            return (T)result;
        }
        throw new Exception("Method not found");
    }

}
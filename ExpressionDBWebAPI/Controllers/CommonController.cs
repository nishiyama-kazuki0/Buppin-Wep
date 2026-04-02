using Anotar.Serilog;
using ExpressionDBWebAPI.Common;
using ExpressionDBWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Data;
using System.Text;

namespace ExpressionDBWebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommonController : ControllerBase
{
    //コンストラクタ
    public CommonController()
    {
    }

    /// <summary>
    /// DB共通取得処理
    /// contentを引数で受信したいので、処理的にはGetだがPostとする
    /// </summary>
    /// <param name="selectInfo"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<IEnumerable<ResponseValue>> Post([FromBody] ClassNameSelect selectInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            LogTo.Information("{@SelectInfo}", selectInfo);
            cancellationToken.ThrowIfCancellationRequested();
            return GetResponseValue(selectInfo);

        }
        catch (Exception ex)
        {
            LogTo.Fatal(ex.Message);
        }

        return Ok(null);
    }

    /// <summary>
    /// DB共通取得処理
    /// </summary>
    /// <param name="selectInfo"></param>
    /// <returns></returns>
    [NonAction]
    public static List<ResponseValue> GetResponseValue([FromBody] ClassNameSelect selectInfo)
    {
        (string sqlString, IDictionary<string, WhereParam>? param) = GetSQLStringAndParam(selectInfo);
        List<ResponseValue> res = DataSource.ExecuteQuery(sqlString, param, selectInfo.tableFuncFlg, commandTimeout: selectInfo.CommandTimeout);
        return res;
    }

    /// <summary>
    /// DB共通取得処理
    /// </summary>
    /// <param name="selectInfo"></param>
    /// <returns></returns>
    [NonAction]
    public static IEnumerable<T> GetGenericResponseValue<T>([FromBody] ClassNameSelect selectInfo) where T : class, new()
    {
        (string sqlString, IDictionary<string, WhereParam>? param) = GetSQLStringAndParam(selectInfo);

        List<IDbDataParameter>? lstParam = DataSource.CreateDbDataParamList(param);
        IEnumerable<T> res = DataSource.GetEntityCollection<T>(sqlString, lstParam);
        return res;
    }

    [NonAction]
    private static (string sqlString, IDictionary<string, WhereParam>? param) GetSQLStringAndParam([FromBody] ClassNameSelect selectInfo)
    {
        // VIEW名またはテーブル名が指定されていない場合は、クラス名からテーブルまたはVIEW名を取得
        if (string.IsNullOrEmpty(selectInfo.viewName))
        {
            string className = selectInfo.className;
            selectInfo.viewName = GetViewName(className);
        }

        string strColumnsDefineName = string.IsNullOrEmpty(selectInfo.columnsDefineName) ? selectInfo.viewName : selectInfo.columnsDefineName;

        // 列情報を取得
        IDictionary<string, ColumnsDefine> columsDefine = GetColumnsDefine(strColumnsDefineName);

        // Wasmから支所場コード、所場区分、荷主IDは必ず受け取り列情報に存在する項目のみWhere句に含めるようにする
        // 支所場コードはフィールド名がBASE_IDまたは、支所場コードと複数の場合があるため、WasmからはBASE_IDという名前で受け取り、列情報によって名称を変える
        // 所場区分はフィールド名がBASE_TYPEまたは、所場区分と複数の場合があるため、WasmからはBASE_IDという名前で受け取り、列情報によって名称を変える
        //WhereParam whereParam;
        //Dictionary<string, List<string>> keys = new()
        //{
        //    { SharedConst.KEY_BASE_ID, new List<string>{ "支所場コード", "BASE_ID" } },
        //    { SharedConst.KEY_BASE_TYPE, new List<string>{ "所場区分", "BASE_TYPE" } },
        //    { SharedConst.KEY_CONSIGNOR_ID, new List<string>{ "CONSIGNOR_ID" } },
        //};
        //foreach (KeyValuePair<string, List<string>> key in keys)
        //{
        //    IEnumerable<KeyValuePair<string, WhereParam>> value = selectInfo.whereParam.Where(_ => _.Key == key.Key);
        //    if (value.Any())
        //    {
        //        whereParam = value.First().Value;
        //        _ = selectInfo.whereParam.Remove(key.Key);
        //        foreach (string itemkey in key.Value)
        //        {
        //            if (columsDefine.TryGetValue(itemkey, out ColumnsDefine? c))
        //            {
        //                selectInfo.whereParam[key.Key] = new WhereParam { field = itemkey, val = whereParam.val, whereType = enumWhereType.Equal, size = c!.MaxLength, dbTypeString = c!.DataTypeStr };
        //            }
        //        }
        //    }
        //}

        int i = 0;
        StringBuilder sb = new();

        // 取得件数が指定されている場合はTOPを指定
        _ = selectInfo.selectTopCnt > 0 ? sb.AppendLine($"SELECT TOP {selectInfo.selectTopCnt}") : sb.AppendLine("SELECT");

        if (selectInfo.selectParam.Count > 0)
        {
            // 列指定がある場合は指定された列名を追加する(ただし列名が存在する場合のみ)
            for (int j = 0; selectInfo.selectParam.Count > j; j++)
            {
                if (columsDefine.ContainsKey(selectInfo.selectParam[j]))
                {
                    _ = sb.AppendLine($"{(i == 0 ? " " : ",")}{DataSource.AddQuotationMarks(selectInfo.selectParam[j])}");
                    i++;
                }
            }
        }
        // 列指定がないまたは、列指定があったが存在する列が１件もなかった場合
        if (selectInfo.selectParam.Count == 0 || i == 0)
        {
            // テーブルまたはVIEWの列名を全て追加する
            foreach (KeyValuePair<string, ColumnsDefine> col in columsDefine)
            {
                _ = sb.AppendLine($"{(i == 0 ? " " : ",")}{DataSource.AddQuotationMarks(col.Key)}");
                i++;
            }
        }
        _ = sb.AppendLine("FROM");
        _ = sb.AppendLine(" " + selectInfo.viewName); //TODO 引用符はつけたほうが良いか検討。TableFuncじゃなければ付与したほうが良いかもしれない。
        _ = sb.Append(DataSource.GetStringQueryHints(selectInfo));//TODO Oracleの場合、HintはSQLの先頭に書くはずなので考える必要がある

        IDictionary<string, WhereParam>? param = selectInfo.whereParam;
        if (param!.Any())
        {
            if (!selectInfo.tableFuncFlg)
            {
                _ = sb.AppendLine("WHERE");
                i = 0;
                foreach (KeyValuePair<string, WhereParam> pam in param)
                {
                    string fieldName = string.IsNullOrEmpty(pam.Value.field) ? pam.Key : pam.Value.field;
                    if (pam.Value.orLinking)
                    {
                        // OR連結
                        _ = sb.AppendLine($"  {(i == 0 ? "   " : "AND")} (");
                        for (int j = 0; j < pam.Value.linkingVals.Count; j++)
                        {
                            _ = pam.Value.whereType is enumWhereType.EqualZeroSuppress or
                                enumWhereType.NotEqualZeroSuppress or
                                enumWhereType.AboveZeroSuppress or
                                enumWhereType.BelowZeroSuppress or
                                enumWhereType.BigZeroSuppress or
                                enumWhereType.SmallZeroSuppress or
                                enumWhereType.LikeStartZeroSuppress or
                                enumWhereType.LikeEndZeroSuppress or
                                enumWhereType.LikePartialZeroSuppress
                                ? sb.AppendLine(DataSource.CreateWhereSQLString(fieldName, pam.Value.GetWhereType(), i, j, "OR", "FC_GET_ZERO_SUPPRESS_CODE"))
                                : sb.AppendLine(DataSource.CreateWhereSQLString(fieldName, pam.Value.GetWhereType(), i, j, "OR"));
                        }
                        _ = sb.AppendLine(")");

                    }
                    else
                    {
                        _ = pam.Value.whereType is enumWhereType.EqualZeroSuppress or
                            enumWhereType.NotEqualZeroSuppress or
                            enumWhereType.AboveZeroSuppress or
                            enumWhereType.BelowZeroSuppress or
                            enumWhereType.BigZeroSuppress or
                            enumWhereType.SmallZeroSuppress or
                            enumWhereType.LikeStartZeroSuppress or
                            enumWhereType.LikeEndZeroSuppress or
                            enumWhereType.LikePartialZeroSuppress
                            ? sb.AppendLine(DataSource.CreateWhereSQLString(fieldName, pam.Value.GetWhereType(), i, "AND", "FC_GET_ZERO_SUPPRESS_CODE"))
                            : sb.AppendLine(DataSource.CreateWhereSQLString(fieldName, pam.Value.GetWhereType(), i, "AND"));
                        pam.Value.val = pam.Value.ProcessValueWhereType(pam.Value.val);
                    }
                    // タイプ指定がされていない場合は、テーブルから取得したタイプをセットする
                    if (pam.Value.type == null)
                    {
                        if (columsDefine.TryGetValue(fieldName, out ColumnsDefine? columns))
                        {
                            pam.Value.type = columns.DataType;
                            pam.Value.dbTypeString = columns.DataTypeStr;
                        }
                    }
                    i++;
                }
            }
            else
            {
                i = 0;
                bool addWhere = false;
                foreach (KeyValuePair<string, WhereParam> pam in param)
                {
                    string fieldName = string.IsNullOrEmpty(pam.Value.field) ? pam.Key : pam.Value.field;
                    // TableFunctionでもWhere句を指定する
                    if (pam.Value.tableFuncWithWhere)
                    {
                        if (!addWhere)
                        {
                            _ = sb.AppendLine("WHERE");
                        }
                        if (pam.Value.orLinking)
                        {
                            // OR連結
                            _ = sb.AppendLine($"  {(!addWhere ? "   " : "AND")} (");
                            for (int j = 0; j < pam.Value.linkingVals.Count; j++)
                            {
                                _ = pam.Value.whereType is enumWhereType.EqualZeroSuppress or
                                    enumWhereType.NotEqualZeroSuppress or
                                    enumWhereType.AboveZeroSuppress or
                                    enumWhereType.BelowZeroSuppress or
                                    enumWhereType.BigZeroSuppress or
                                    enumWhereType.SmallZeroSuppress or
                                    enumWhereType.LikeStartZeroSuppress or
                                    enumWhereType.LikeEndZeroSuppress or
                                    enumWhereType.LikePartialZeroSuppress
                                    ? sb.AppendLine(DataSource.CreateWhereSQLString(fieldName, pam.Value.GetWhereType(), i, j, "OR", "FC_GET_ZERO_SUPPRESS_CODE"))
                                    : sb.AppendLine(DataSource.CreateWhereSQLString(fieldName, pam.Value.GetWhereType(), i, j, "OR"));
                            }
                            _ = sb.AppendLine(")");

                        }
                        else
                        {
                            _ = pam.Value.whereType is enumWhereType.EqualZeroSuppress or
                                enumWhereType.NotEqualZeroSuppress or
                                enumWhereType.AboveZeroSuppress or
                                enumWhereType.BelowZeroSuppress or
                                enumWhereType.BigZeroSuppress or
                                enumWhereType.SmallZeroSuppress or
                                enumWhereType.LikeStartZeroSuppress or
                                enumWhereType.LikeEndZeroSuppress or
                                enumWhereType.LikePartialZeroSuppress
                                ? sb.AppendLine(DataSource.CreateWhereSQLString(fieldName, pam.Value.GetWhereType(), addWhere ? 0 : 1, "AND", "FC_GET_ZERO_SUPPRESS_CODE"))
                                : sb.AppendLine(DataSource.CreateWhereSQLString(fieldName, pam.Value.GetWhereType(), addWhere ? 0 : 1, "AND"));
                            pam.Value.val = pam.Value.ProcessValueWhereType(pam.Value.val);
                        }
                        addWhere = true;
                    }
                    // タイプ指定がされていない場合は、テーブルから取得したタイプをセットする
                    if (pam.Value.type == null)
                    {
                        if (columsDefine.TryGetValue(fieldName, out ColumnsDefine? columns))
                        {
                            pam.Value.type = columns.DataType;
                            pam.Value.dbTypeString = columns.DataTypeStr;
                        }
                    }
                    i++;
                }
            }
        }

        // OrderBy句を設定
        if (selectInfo.orderByParam.Count > 0)
        {
            _ = sb.AppendLine("ORDER BY");
            i = 0;
            foreach (OrderByParam prm in selectInfo.orderByParam)
            {
                _ = sb.AppendLine($"{(i == 0 ? " " : ",")}{DataSource.AddQuotationMarks(prm.field)}{(prm.desc == true ? " DESC" : "")}");
                i++;
            }
        }
        LogTo.Information($"SQL:{sb.ToString().Replace("\r\n", " ")}");
        return (sb.ToString(), param);
    }

    /// <summary>
    /// クラス名から取得するテーブルまたはVIEW名称を取得する
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    [NonAction]
    private static string GetViewName(string className)
    {
        // クラス名からSELECTするテーブルまたはVIEW情報を取得する
        string sql = DataSource.CreateSimpleSelectQuery("DEFINE_PAGE_VALUES", ["CLASS_NAME"]);
        DEFINE_PAGE_VALUES_Model? ret
            = DataSource.GetEntityCollection<DEFINE_PAGE_VALUES_Model>(sql,
            DataSource.CreateParamTable(new Dictionary<string, (object? value, int? size, Type? type, ParameterDirection? parameterDirection)>
                    {
                        { "CLASS_NAME", (className , 128 , typeof(string),null) }
                    }))
                .FirstOrDefault();//1件のみに絞られる想定。
        return ret is null ? string.Empty : ret.VIEW_NAME;
    }

    /// <summary>
    /// テーブルまたはVIEWまたはテーブル値関数の列情報を取得する
    /// </summary>
    /// <param name="objectName">テーブルまたはVIEWまたはテーブル値関数の名前</param>
    /// <returns></returns>
    [NonAction]
    private static IDictionary<string, ColumnsDefine> GetColumnsDefine(string objectName)
    {
        // VIEWのフィールド情報を取得する
        StringBuilder sb = new();
        string[] paramName = ["param0"];
        IDictionary<string, WhereParam> nameParam = new Dictionary<string, WhereParam>
            {
                { "name", new WhereParam() { val = objectName} } //TODO 良くないが、中途半端に指定できないので、とりあえずこれで対応
            };
        _ = sb.AppendLine(DataSource.CreateTableFunctionQuery("GetObjectColumnsDefine", paramName));
        List<ResponseValue> nameRes = DataSource.ExecuteQuery(sb.ToString(), nameParam);
        //TODO わざわざDicにする必要はあるのか。モデルのリストではパフォーマンスが悪かったのか。検討。
        IDictionary<string, ColumnsDefine> columsDefine = new Dictionary<string, ColumnsDefine>();
        foreach (ResponseValue r in nameRes)
        {
            ColumnsDefine col = new();
            col.SetDictonary(r.Values);
            if (!columsDefine.ContainsKey(col.ColumnName))
            {
                columsDefine.Add(col.ColumnName, col);
            }
        }
        return columsDefine;
    }
}

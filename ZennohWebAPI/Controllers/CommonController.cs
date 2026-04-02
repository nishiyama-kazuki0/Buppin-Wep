using Anotar.Serilog;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Data;
using System.Text;
using ZennohBlazorShared.Data;
using ZennohWebAPI.Common;
using ZennohWebAPI.Data;

namespace ZennohWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommonController : ControllerBase
    {
        /// <summary>
        /// DB共通取得処理
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
                //// VIEW名またはテーブル名が指定されていない場合は、クラス名からテーブルまたはVIEW名を取得
                //if (string.IsNullOrEmpty(selectInfo.viewName))
                //{
                //    string className = selectInfo.className;
                //    selectInfo.viewName = GetViewName(className);
                //}

                //string strColumnsDefineName = string.IsNullOrEmpty(selectInfo.columnsDefineName) ? selectInfo.viewName : selectInfo.columnsDefineName;

                //// 列情報を取得
                //IDictionary<string, ColumnsDefine> columsDefine = GetColumnsDefine(strColumnsDefineName);

                //// Wasmから支所場コード、所場区分、荷主IDは必ず受け取り列情報に存在する項目のみWhere句に含めるようにする
                //// 支所場コードはフィールド名がBASE_IDまたは、支所場コードと複数の場合があるため、WasmからはBASE_IDという名前で受け取り、列情報によって名称を変える
                //// 所場区分はフィールド名がBASE_TYPEまたは、所場区分と複数の場合があるため、WasmからはBASE_IDという名前で受け取り、列情報によって名称を変える
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
                //            if (columsDefine.Select(_ => _.Key).Contains(itemkey))
                //            {
                //                selectInfo.whereParam[key.Key] = new WhereParam { field = itemkey, val = whereParam.val, whereType = enumWhereType.Equal };
                //            }
                //        }
                //    }
                //}

                //int i = 0;
                //StringBuilder sb = new();
                //_ = sb.AppendLine("SELECT");
                //if (selectInfo.selectParam.Count > 0)
                //{
                //    // 列指定がある場合は指定された列名を追加する(ただし列名が存在する場合のみ)
                //    for (int j = 0; selectInfo.selectParam.Count > j; j++)
                //    {
                //        if (columsDefine.ContainsKey(selectInfo.selectParam[j]))
                //        {
                //            _ = sb.AppendLine($"{(i == 0 ? " " : ",")}[{selectInfo.selectParam[j]}]");
                //            i++;
                //        }
                //    }
                //}
                //// 列指定がないまたは、列指定があったが存在する列が１件もなかった場合
                //if (selectInfo.selectParam.Count == 0 || i == 0)
                //{
                //    // テーブルまたはVIEWの列名を全て追加する
                //    foreach (KeyValuePair<string, ColumnsDefine> col in columsDefine)
                //    {
                //        _ = sb.AppendLine($"{(i == 0 ? " " : ",")}[{col.Key}]");
                //        i++;
                //    }
                //}
                //_ = sb.AppendLine("FROM");
                //_ = sb.AppendLine(" " + selectInfo.viewName);

                //IDictionary<string, WhereParam>? param = selectInfo.whereParam;
                //if (param!.Any())
                //{
                //    _ = sb.AppendLine("WHERE");
                //    i = 0;
                //    foreach (KeyValuePair<string, WhereParam> pam in param)
                //    {
                //        string fieldName = string.IsNullOrEmpty(pam.Value.field) ? pam.Key : pam.Value.field;
                //        if (pam.Value.orLinking)
                //        {
                //            // OR連結
                //            _ = sb.AppendLine($"  {(i == 0 ? "   " : "AND")} (");
                //            for (int j = 0; j < pam.Value.linkingVals.Count; j++)
                //            {
                //                _ = sb.AppendLine($"  {(j == 0 ? "  " : "OR")} [{fieldName}] {pam.Value.GetWhereType()} @param{i}_{j}");
                //            }
                //            _ = sb.AppendLine(")");

                //        }
                //        else
                //        {
                //            _ = sb.AppendLine($"  {(i == 0 ? "   " : "AND")} [{fieldName}] {pam.Value.GetWhereType()} @param{i}");
                //            pam.Value.val = pam.Value.ProcessValueWhereType(pam.Value.val);
                //        }
                //        // タイプ指定がされていない場合は、テーブルから取得したタイプをセットする
                //        if (pam.Value.type == null)
                //        {
                //            if (columsDefine.TryGetValue(fieldName, out ColumnsDefine? columns))
                //            {
                //                pam.Value.type = columns.DataType;
                //            }
                //        }
                //        i++;
                //    }
                //}

                //// OrderBy句を設定
                //if (selectInfo.orderByParam.Count > 0)
                //{
                //    _ = sb.AppendLine("ORDER BY");
                //    i = 0;
                //    foreach (OrderByParam prm in selectInfo.orderByParam)
                //    {
                //        _ = sb.AppendLine($"{(i == 0 ? " " : ",")}[{prm.field}]{(prm.desc == true ? " DESC" : "")}");
                //        i++;
                //    }
                //}
                //LogTo.Information($"SQL:{sb.ToString().Replace("\r\n", " ")}");
                //List<ResponseValue> res = DataSource.ExecuteQuery(sb.ToString(), param);
                //return res;

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
            WhereParam whereParam;
            Dictionary<string, List<string>> keys = new()
            {
                { SharedConst.KEY_BASE_ID, new List<string>{ "支所場コード", "BASE_ID" } },
                { SharedConst.KEY_BASE_TYPE, new List<string>{ "所場区分", "BASE_TYPE" } },
                { SharedConst.KEY_CONSIGNOR_ID, new List<string>{ "CONSIGNOR_ID" } },
            };
            foreach (KeyValuePair<string, List<string>> key in keys)
            {
                IEnumerable<KeyValuePair<string, WhereParam>> value = selectInfo.whereParam.Where(_ => _.Key == key.Key);
                if (value.Any())
                {
                    whereParam = value.First().Value;
                    _ = selectInfo.whereParam.Remove(key.Key);
                    foreach (string itemkey in key.Value)
                    {
                        if (columsDefine.TryGetValue(itemkey, out ColumnsDefine? c))
                        {
                            selectInfo.whereParam[key.Key] = new WhereParam { field = itemkey, val = whereParam.val, whereType = enumWhereType.Equal, size = c!.MaxLength };
                        }
                    }
                }
            }

            int i = 0;
            StringBuilder sb = new();

            // 取得件数が指定されている場合はTOPを指定
            if (selectInfo.selectTopCnt > 0)
            {
                _ = sb.AppendLine($"SELECT TOP {selectInfo.selectTopCnt}");
            }
            else
            {
                _ = sb.AppendLine("SELECT");
            }

            if (selectInfo.selectParam.Count > 0)
            {
                // 列指定がある場合は指定された列名を追加する(ただし列名が存在する場合のみ)
                for (int j = 0; selectInfo.selectParam.Count > j; j++)
                {
                    if (columsDefine.ContainsKey(selectInfo.selectParam[j]))
                    {
                        _ = sb.AppendLine($"{(i == 0 ? " " : ",")}[{selectInfo.selectParam[j]}]");
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
                    _ = sb.AppendLine($"{(i == 0 ? " " : ",")}[{col.Key}]");
                    i++;
                }
            }
            _ = sb.AppendLine("FROM");
            _ = sb.AppendLine(" " + selectInfo.viewName);
            if (selectInfo.tsqlHints != EnumTSQLhints.NONE && !selectInfo.tableFuncFlg)
            {
                _ = sb.Append($" WITH ({selectInfo.GetHintStr}) "); //テーブルヒント句を設定    
            }

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
                                    ? sb.AppendLine($"  {(j == 0 ? "  " : "OR")} dbo.FC_GET_ZERO_SUPPRESS_CODE([{fieldName}]) {pam.Value.GetWhereType()} @param{i}_{j}")
                                    : sb.AppendLine($"  {(j == 0 ? "  " : "OR")} [{fieldName}] {pam.Value.GetWhereType()} @param{i}_{j}");
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
                                ? sb.AppendLine($"  {(i == 0 ? "   " : "AND")} dbo.FC_GET_ZERO_SUPPRESS_CODE([{fieldName}]) {pam.Value.GetWhereType()} @param{i}")
                                : sb.AppendLine($"  {(i == 0 ? "   " : "AND")} [{fieldName}] {pam.Value.GetWhereType()} @param{i}");
                            pam.Value.val = pam.Value.ProcessValueWhereType(pam.Value.val);
                        }
                        // タイプ指定がされていない場合は、テーブルから取得したタイプをセットする
                        if (pam.Value.type == null)
                        {
                            if (columsDefine.TryGetValue(fieldName, out ColumnsDefine? columns))
                            {
                                pam.Value.type = columns.DataType;
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
                                        ? sb.AppendLine($"  {(j == 0 ? "  " : "OR")} dbo.FC_GET_ZERO_SUPPRESS_CODE([{fieldName}]) {pam.Value.GetWhereType()} @param{i}_{j}")
                                        : sb.AppendLine($"  {(j == 0 ? "  " : "OR")} [{fieldName}] {pam.Value.GetWhereType()} @param{i}_{j}");
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
                                    ? sb.AppendLine($"  {(addWhere ? "   " : "AND")} dbo.FC_GET_ZERO_SUPPRESS_CODE([{fieldName}]) {pam.Value.GetWhereType()} @param{i}")
                                    : sb.AppendLine($"  {(addWhere ? "   " : "AND")} [{fieldName}] {pam.Value.GetWhereType()} @param{i}");
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
                    _ = sb.AppendLine($"{(i == 0 ? " " : ",")}[{prm.field}]{(prm.desc == true ? " DESC" : "")}");
                    i++;
                }
            }
            LogTo.Information($"SQL:{sb.ToString().Replace("\r\n", " ")}");
            List<ResponseValue> res = DataSource.ExecuteQuery(sb.ToString(), param, selectInfo.tableFuncFlg);
            return res;
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
            string formName = string.Empty;
            IDictionary<string, WhereParam> viewNameParam = new Dictionary<string, WhereParam>
                {
                    { "CLASS_NAME", new WhereParam() { val = className, type = SqlDbType.NVarChar } }
                };

            StringBuilder sb = new();
            _ = sb.AppendLine("SELECT");
            _ = sb.AppendLine(" VIEW_NAME");
            _ = sb.AppendLine("FROM");
            _ = sb.AppendLine(" DEFINE_PAGE_VALUES");
            _ = sb.AppendLine("WHERE");
            int i = 0;
            foreach (KeyValuePair<string, WhereParam> pam in viewNameParam)
            {
                string fieldName = string.IsNullOrEmpty(pam.Value.field) ? pam.Key : pam.Value.field;
                //_ = sb.AppendLine($"  {(i == 0 ? "   " : "AND")} {fieldName} {pam.Value.GetWhereType()} @{pam.Key}");
                _ = sb.AppendLine($"  {(i == 0 ? "   " : "AND")} {fieldName} {pam.Value.GetWhereType()} @param{i}");
                i++;
            }
            List<ResponseValue> viewNameRes = DataSource.ExecuteQuery(sb.ToString(), viewNameParam);
            foreach (ResponseValue r in viewNameRes)
            {
                if (null != r.Values && null != r.Columns && r.Columns.Count > 0 && null != r.Values[r.Columns[0]])
                {
                    formName = r.Values[r.Columns[0]].ToString()!.ToString();
                }
            }
            return formName;
        }

        /// <summary>
        /// テーブルまたはVIEWの列情報を取得する
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        [NonAction]
        private static IDictionary<string, ColumnsDefine> GetColumnsDefine(string viewName)
        {
            // VIEWのフィールド情報を取得する
            StringBuilder sb = new();
            IDictionary<string, WhereParam> nameParam = new Dictionary<string, WhereParam>
                {
                    { "name", new WhereParam() { val = viewName, type = SqlDbType.NVarChar ,size = 128} }
                };

            _ = sb.AppendLine("SELECT");
            _ = sb.AppendLine(" *");
            _ = sb.AppendLine("FROM");
            _ = sb.AppendLine(" dbo.GetObjectColumnsDefine(@param0)");
            List<ResponseValue> nameRes = DataSource.ExecuteQuery(sb.ToString(), nameParam);
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

        // GET: Common/GetExceProcess
        [HttpGet("GetExceProcess")]
        public ActionResult GetExceProcess([FromQuery] int? seq)
        {
            //LogTo.Information("groupId:{groupId}, menuId:{menuId}", groupId, menuId);
            try
            {
                StringBuilder sb = new();
                _ = sb.Append("SELECT ");
                _ = sb.Append("  REQUEST_PROGRAM ");
                _ = sb.Append(" ,USE_VARIABLE_COUNT ");
                _ = sb.Append(" ,COLUMN_NAME1 ");
                _ = sb.Append(" ,DATA_TYPE_NAME1 ");
                _ = sb.Append(" ,PARAM_VALUE1 ");
                _ = sb.Append(" ,COLUMN_NAME2 ");
                _ = sb.Append(" ,DATA_TYPE_NAME2 ");
                _ = sb.Append(" ,PARAM_VALUE2 ");
                _ = sb.Append("FROM ");
                _ = sb.Append("  PROCESS_ORDER_DETAILS ");
                _ = sb.Append(string.Format("WHERE SEQ = {0} ", seq));
                _ = sb.Append("ORDER BY ");
                _ = sb.Append("  SEQ_SUB ");

                List<ResponseValue> res = DataSource.ExecuteQuery(sb.ToString());
                foreach (ResponseValue r in res)
                {
                    ColumnsDefine col = new();
                    Type t = typeof(ColumnsDefine);
                    int intVal;
                    foreach (System.Reflection.FieldInfo f in t.GetFields())
                    {
                        if (r.Values.TryGetValue(f.Name, out object? val))
                        {
                            switch (f.Name)
                            {
                                case nameof(col.ColumnName):
                                    col.ColumnName = val.ToString() ?? "";
                                    break;
                                case nameof(col.DataType):
                                    col.DataType = DataSource.GetDbType(val.ToString() ?? "");
                                    break;
                                case nameof(col.MaxLength):
                                    _ = int.TryParse(val.ToString(), out intVal);
                                    col.MaxLength = intVal;
                                    break;
                                case nameof(col.Precision):
                                    _ = int.TryParse(val.ToString(), out intVal);
                                    col.Precision = intVal;
                                    break;
                                case nameof(col.Scale):
                                    _ = int.TryParse(val.ToString(), out intVal);
                                    col.Scale = intVal;
                                    break;
                                case nameof(col.IsNullable):
                                    _ = bool.TryParse(val.ToString(), out bool boolVal);
                                    col.IsNullable = boolVal;
                                    break;
                            }
                        }
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                LogTo.Fatal(e.Message);
            }

            return Ok(null);
        }
    }
}

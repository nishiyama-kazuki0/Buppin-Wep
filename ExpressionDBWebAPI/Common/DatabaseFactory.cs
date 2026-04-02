using static SharedModels.SharedConst;
namespace ExpressionDBWebAPI.Common;

internal class DatabaseFactory
{
    public static IDataSource CreateDatabaseAccess(TYPE_DB_TYPE dbtype)
    {
        return dbtype switch
        {
            TYPE_DB_TYPE.SQL => new SqlServerSource(),//TODO コンフィグサービスをinjectするべきか
            TYPE_DB_TYPE.ORACLE => new OracleDataSource(),//TODO コンフィグサービスをinjectするべきか
            _ => new SqlServerSource(),
        };
    }
}
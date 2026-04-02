namespace SharedModels;

public static class ClassNameSelectExtensions
{
    public static ClassNameSelect From(this ClassNameSelect cns,  string viewName)
    {
        cns.viewName = viewName;
        return cns;
    }

    public static ClassNameSelect Where(this ClassNameSelect cns, string key, string value)
    {
        cns.whereParam.Add(key, new() { val = value});
        return cns;
    }

    public static ClassNameSelect OrderBy(this ClassNameSelect cns, params string[] fields)
    {
        foreach (var f in fields)
        {
            cns.orderByParam.Add(new() { field = f });
        }
        return cns;
    }

    public static ClassNameSelect OrderByDesc(this ClassNameSelect cns, params string[] fields)
    {
        foreach (var f in fields)
        {
            cns.orderByParam.Add(new() { field = f, desc = true });
        }
        return cns;
    }
}
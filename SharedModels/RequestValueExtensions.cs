using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedModels;

public static class RequestValueExtensions
{
    public static RequestValue Set<T>(this RequestValue rv, string key, T value) =>
        value switch
        {
            string s => rv.SetArgumentValue(key, value, "System.String"),
            bool b => rv.SetArgumentValue(key, value, "System.Boolean"),
            _ => throw new NotSupportedException($"Type {typeof(T)} is not supported.")
        };
}

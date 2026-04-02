using System.Reflection;

namespace SharedModels;

/// <summary>
/// VIEWモデル基底クラスの拡張
/// </summary>
public abstract class AbstractViewModelBaseExtension : AbstractViewModelBase
{
    #region 拡張処理
    /// <summary>
    /// プロパティ値を型に沿ってリセットする
    /// 文字型：string.Empty／bool型：false／列挙型：0／数値型：0
    /// ※この型に含まれない、または、初期値が異なる場合、派生クラスにて実装する
    /// </summary>
    /// <param name="bindingFlags">リフレクションによるメンバと型の検索方法</param>
    public virtual void ResetPropertys(BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public)
    {
        //プロパティをリセットする
        foreach (PropertyInfo prop in GetType().GetProperties(bindingFlags))
        {
            //書込可否
            if (prop.CanWrite == false)
            {
                continue;
            }
            //リセット処理
            if (prop.PropertyType == typeof(string))    //文字型
            {
                prop.SetValue(this, string.Empty);
            }
            else if (prop.PropertyType == typeof(bool)) //bool型
            {
                prop.SetValue(this, false);
            }
            else if (prop.PropertyType.IsEnum == true)  //列挙型
            {
                prop.SetValue(this, 0);
            }
            else if (IsNumericType(prop.PropertyType) == true)  //数値型
            {
                prop.SetValue(this, Convert.ChangeType(0, prop.PropertyType));
            }
        }
    }
    /// <summary>
    /// 数値型の判定
    /// </summary>
    /// <param name="type">対象タイプ</param>
    /// <returns>true：数値型／false：以外</returns>
    private bool IsNumericType(Type type)
    {
        return type == typeof(byte)
            || type == typeof(sbyte)
            || type == typeof(short)
            || type == typeof(ushort)
            || type == typeof(int)
            || type == typeof(uint)
            || type == typeof(long)
            || type == typeof(ulong)
            || type == typeof(float)
            || type == typeof(double)
            || type == typeof(decimal);
    }

    /// <summary>
    /// オブジェクトの複製
    /// </summary>
    /// <returns>複製した新インスタンス</returns>
    public T CloneProperty<T>()
    {
        return (T)MemberwiseClone();
    }
    #endregion
}

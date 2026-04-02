namespace ExpressionDBBlazorShared.Util;

internal static class CalcUtil
{
    #region 計算

    /// <summary>
    /// パーセントを計算
    /// </summary>
    /// <param name="decNum"></param>
    /// <param name="decDen"></param>
    /// <param name="intDecPoint"></param>
    /// <returns></returns>
    internal static decimal GetPercent(decimal decNum, decimal decDen, int intDecPoint = 0)
    {
        decimal decRet = 0.0m;
        if (decDen == 0.0m)
        {
            return decRet;
        }
        if (intDecPoint < 0)
        {
            intDecPoint = 0;
        }
        double pow = Math.Pow(10, intDecPoint);
        decRet = decNum / decDen * 100;
        return Math.Floor(decRet * (int)pow) / (int)pow;
    }

    #endregion

}


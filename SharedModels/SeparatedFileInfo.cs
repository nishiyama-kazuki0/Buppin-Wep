using System.Text;

namespace SharedModels;

/// <summary>
/// セパレートファイルの種類情報
/// </summary>
public class SeparatedFileInfo
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 拡張子(ピリオド(.)なし)
    /// </summary>
    public string Extension { get; set; }
    /// <summary>
    /// セパレート文字
    /// </summary>
    public string Delimiter { get; set; }
    /// <summary>
    /// ファイルエンコーディング
    /// </summary>
    public Encoding FileEncoding { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public SeparatedFileInfo()
    {
        //RegisterProviderの以下1文がないとshift_jis指定で例外が発生する点に注意
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        Name = string.Empty;
        Extension = string.Empty;
        Delimiter = string.Empty;
        FileEncoding = Encoding.GetEncoding("shift_jis");
    }
}

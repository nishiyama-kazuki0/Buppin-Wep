namespace ExpressionDBBlazorShared.Data;

/// <summary>
/// DEFINE_COMPONENT_COLUMNSテーブル情報
/// </summary>
public class ComponentColumnsInfo
{
    /// <summary>
    /// コンポーネント名
    /// </summary>
    public string ComponentName { get; set; }
    /// <summary>
    /// View名
    /// </summary>
    public string ViewName { get; set; }
    /// <summary>
    /// プロパティ－（キー）
    /// </summary>
    public string Property { get; set; }
    /// <summary>
    /// 値最小値
    /// </summary>
    public decimal? ValueMin { get; set; }
    /// <summary>
    /// 値最大値
    /// </summary>
    public decimal? ValueMax { get; set; }
    /// <summary>
    /// カラム幅 //非表示は0とする
    /// </summary>
    public int Width { get; set; }
    /// <summary>
    /// データタイプ
    /// </summary>
    public Type Type { get; set; }
    /// <summary>
    /// データタイプ文字(DEFINE_COMPONENT_COLUMNS.COMPONET_DATA_TYPE)
    /// </summary>
    public string TypeText { get; set; }
    /// <summary>
    /// 文字位置
    /// </summary>
    public TextAlign TextAlign { get; set; }
    /// <summary>
    /// カラム幅変更有無
    /// </summary>
    public bool Resizable { get; set; }
    /// <summary>
    /// カラムのドラッグドロップ並び替え移動有無
    /// </summary>
    public bool Reorderable { get; set; }
    /// <summary>
    /// ソート有無
    /// </summary>
    public bool Sortable { get; set; }
    /// <summary>
    /// フィルター有無
    /// </summary>
    public bool Filterable { get; set; }
    /// <summary>
    /// 文字列フォーマット
    /// </summary>
    public string FormatString { get; set; }
    /// <summary>
    /// 編集可能
    /// </summary>
    public bool IsEdit { get; set; }
    /// <summary>
    /// データ出力有無
    /// </summary>
    public bool IsDataExport { get; set; }
    /// <summary>
    /// 検索条件有無
    /// </summary>
    public bool IsSearchCondition { get; set; }
    /// <summary>
    /// 集計タイプ（0:該当なし,1:合計,2:平均）
    /// </summary>
    public int SummaryType { get; set; }
    /// <summary>
    /// 検索条件値取得VIEW名
    /// </summary>
    public string SearchValuesViewName { get; set; }
    /// <summary>
    /// 検索条件データ型
    /// </summary>
    public string SearchDataTypeKey { get; set; }
    /// <summary>
    /// 検索条件入力必須有無
    /// </summary>
    public bool InputRequired { get; set; }
    /// <summary>
    /// 並び替え初期
    /// </summary>
    public int OrderbyRank { get; set; }
    /// <summary>
    /// 検索条件 表示グループ
    /// </summary>
    public int SearchLayoutGroup { get; set; }
    /// <summary>
    /// 検索条件 表示順
    /// </summary>
    public int SearchLayoutDispOrder { get; set; }
    /// <summary>
    /// 入力項目 必須有無
    /// </summary>
    public bool EditInputRequired { get; set; }
    /// <summary>
    /// 入力項目 正規表現
    /// </summary>
    public string RegularExpressionString { get; set; }
    /// <summary>
    /// 入力項目 表示グループ
    /// </summary>
    public int EditDialogLayoutGroup { get; set; }
    /// <summary>
    /// 入力項目 表示順
    /// </summary>
    public int EditDialogLayoutDispOrder { get; set; }
    /// <summary>
    /// 入力項目 データ型
    /// </summary>
    public int EditType { get; set; }
    /// <summary>
    /// 入力項目 
    /// </summary>
    public string EditValuesViewName { get; set; }
    /// <summary>
    /// 入力項目 
    /// </summary>
    public string EditDataTypeKey { get; set; }
    /// <summary>
    /// 列 編集可不可
    /// </summary>
    public bool InlineEdit { get; set; }

    // テーブル定義
    /// <summary>
    /// SQLServerデータ型
    /// </summary>
    public string DataType { get; set; }
    /// <summary>
    /// 最大文字数
    /// </summary>
    public long? MaxLength { get; set; }
    /// <summary>
    /// 整数桁(NUMERIC型)
    /// </summary>
    public int Precision { get; set; }
    /// <summary>
    /// 少数桁(NUMERIC型)
    /// </summary>
    public int Scale { get; set; }
    /// <summary>
    /// Null許可
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// タイトル
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// ユーザ設定によるカラムの非表示
    /// </summary>
    public bool UserSetHidden { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ComponentColumnsInfo()
    {
        ComponentName = string.Empty;
        ViewName = string.Empty;
        Property = string.Empty;
        ValueMin = null;
        ValueMax = null;
        Width = 100;
        Type = typeof(string);
        TypeText = string.Empty;
        TextAlign = TextAlign.Left;
        Resizable = true;
        Reorderable = true;
        Sortable = true;
        Filterable = true;
        FormatString = string.Empty;
        IsEdit = false;
        IsDataExport = false;
        IsSearchCondition = false;
        SummaryType = 0;
        SearchValuesViewName = string.Empty;
        SearchDataTypeKey = string.Empty;
        InputRequired = false;
        OrderbyRank = 0;
        SearchLayoutGroup = 0;
        SearchLayoutDispOrder = 0;
        EditInputRequired = false;
        RegularExpressionString = string.Empty;
        EditDialogLayoutGroup = 0;
        EditDialogLayoutDispOrder = 0;
        EditType = 0;
        EditValuesViewName = string.Empty;
        EditDataTypeKey = string.Empty;
        InlineEdit = false;
        DataType = string.Empty;
        MaxLength = 0;
        Precision = 0;
        Scale = 0;
        IsNullable = false;

        Title = string.Empty;

        UserSetHidden = false;
    }
}

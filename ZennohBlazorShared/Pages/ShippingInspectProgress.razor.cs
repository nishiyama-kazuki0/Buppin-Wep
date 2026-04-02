using ZennohBlazorShared.Shared;

namespace ZennohBlazorShared.Pages
{
    /// <summary>
    /// 出荷検品進捗グリッド表示用データクラス
    /// </summary>
    public class ShippingInspectProgressData
    {
        public string Title { get; set; }
        public string SokoName { get; set; }
        public string ZoneName { get; set; }
        public string CornerName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime LimitTime { get; set; }
        public IList<string> Prosess { get; set; }
        public int YoteiNum { get; set; }
        public int ShippingNum { get; set; }
        public double ShippingProcess { get; set; }
        public ShippingInspectProgressData()
        {
            Title = "";
            SokoName = "";
            ZoneName = "";
            CornerName = "";
            //Prosess = Enumerable.Repeat<string>("", 100 / (10 / SortingProgress.Bunkatu)).ToArray();
            Prosess = Enumerable.Repeat<string>("", 100 / (10 / 10)).ToArray();
        }
    }

    public partial class ShippingInspectProgress : ChildPageBasePC
    {



        //protected List<IDictionary<string, object>> _dataParent { get; set; } = new List<IDictionary<string, object>>();
        ///// <summary>
        ///// カラム定義
        ///// key:カラム名
        ///// tuple(型,カラム幅,)
        ///// </summary>
        //protected IDictionary<string, (Type, int, TextAlign)> _columnsParent { get; set; } = new Dictionary<string, (Type, int, TextAlign)>();
        //private IDictionary<string, object> AttributesGrid { get; set; } = new Dictionary<string, object>();
        private IDictionary<string, object> AttributesFuncButton { get; set; } = new Dictionary<string, object>();

        private readonly IEnumerable<ShippingInspectProgressData> progressDataTotal = new List<ShippingInspectProgressData>();
        private readonly IEnumerable<ShippingInspectProgressData> progressData = new List<ShippingInspectProgressData>();
        private IEnumerable<ShippingInspectProgressData> progressDataTotalCorner = new List<ShippingInspectProgressData>();
        private IEnumerable<ShippingInspectProgressData> progressDataCorner = new List<ShippingInspectProgressData>();
        public static readonly int Bunkatu = 10;


        protected override async Task OnInitializedAsync()
        {
            progressDataCorner = new List<ShippingInspectProgressData>()
            {
                new ShippingInspectProgressData() { SokoName = "基幹棟", CornerName = "万代コーナー", StartTime = new DateTime(2023, 1, 1, 23, 0, 0), LimitTime = new DateTime(2023, 1, 1, 1, 0, 0), YoteiNum = 1000, ShippingNum = 500 },
                new ShippingInspectProgressData() { SokoName = "基幹棟", CornerName = "イズミヤコーナー", StartTime = new DateTime(2023, 1, 1, 2, 0, 0), LimitTime = new DateTime(2023, 1, 1, 3, 0, 0), YoteiNum = 1000, ShippingNum = 1000 },
                new ShippingInspectProgressData() { SokoName = "基幹棟", CornerName = "ライフ店舗別コーナー", StartTime = new DateTime(2023, 1, 1, 2, 0, 0), LimitTime = new DateTime(2023, 1, 1, 3, 0, 0), YoteiNum = 1000, ShippingNum = 0 },
                new ShippingInspectProgressData() { SokoName = "基幹棟", CornerName = "ライフセンターコーナー", StartTime = new DateTime(2023, 1, 1, 2, 0, 0), LimitTime = new DateTime(2023, 1, 1, 3, 0, 0), YoteiNum = 1000, ShippingNum = 500 },
                new ShippingInspectProgressData() { SokoName = "基幹棟", CornerName = "原体コーナー", StartTime = new DateTime(2023, 1, 1, 2, 0, 0), LimitTime = new DateTime(2023, 1, 1, 3, 0, 0), YoteiNum = 1000, ShippingNum = 600 },
                new ShippingInspectProgressData() { SokoName = "基幹棟", CornerName = "イオンコーナー", StartTime = new DateTime(2023, 1, 1, 3, 0, 0), LimitTime = new DateTime(2023, 1, 1, 4, 0, 0), YoteiNum = 1000, ShippingNum = 0 },
                new ShippingInspectProgressData() { SokoName = "基幹棟", CornerName = "光洋コーナー", StartTime = new DateTime(2023, 1, 1, 3, 0, 0), LimitTime = new DateTime(2023, 1, 1, 4, 0, 0), YoteiNum = 100, ShippingNum = 0 },
                new ShippingInspectProgressData() { SokoName = "基幹棟", CornerName = "フレスココーナー", StartTime = new DateTime(2023, 1, 1, 3, 0, 0), LimitTime = new DateTime(2023, 1, 1, 4, 0, 0), YoteiNum = 1000, ShippingNum = 0 },
                new ShippingInspectProgressData() { SokoName = "基幹棟", CornerName = "Ａコープコーナー", StartTime = new DateTime(2023, 1, 1, 4, 0, 0), LimitTime = new DateTime(2023, 1, 1, 5, 0, 0), YoteiNum = 1000, ShippingNum = 0 },
                new ShippingInspectProgressData() { SokoName = "基幹棟", CornerName = "コープ近畿コーナー", StartTime = new DateTime(2023, 1, 1, 4, 0, 0), LimitTime = new DateTime(2023, 1, 1, 5, 0, 0), YoteiNum = 1000, ShippingNum = 0 },
            };
            foreach (ShippingInspectProgressData data in progressDataCorner)
            {
                data.ShippingProcess = data.YoteiNum > 0 ? data.ShippingNum / (double)data.YoteiNum * 100.0 : 0;
            }

            // トータルデータの作成
            progressDataTotalCorner = new List<ShippingInspectProgressData>()
            {
                new ShippingInspectProgressData() { Title = "全体進捗", YoteiNum = progressDataCorner.Sum(x => x.YoteiNum), ShippingNum = progressDataCorner.Sum(x => x.ShippingNum) },
            };
            foreach (ShippingInspectProgressData data in progressDataTotalCorner)
            {
                data.ShippingProcess = data.YoteiNum > 0 ? data.ShippingNum / (double)data.YoteiNum * 100.0 : 0;
            }
            ////TODO DBの」テーブル名、またはVIEW名から列の情報を取得する
            //_columnsParent = new Dictionary<string, (Type, int, TextAlign)>(){
            //    { "出庫予定日", (typeof(string),100,TextAlign.Left) },
            //{ "棚番", (typeof(string),100,TextAlign.Left) },
            //{ "案件名", (typeof(string),150,TextAlign.Left) },
            //{ "メーカー名", (typeof(string),150,TextAlign.Left) },
            //    { "品名", (typeof(string),150,TextAlign.Left) },
            //    { "品番", (typeof(string),150,TextAlign.Left) },
            //    { "数量", (typeof(string),80,TextAlign.Right) },
            //};
            //AttributesGrid.Add("Columns", _columnsParent);
            ////TODO DBのテーブル名またはVIEW名からデータを取得する
            //_dataParent = new List<IDictionary<string, object>>();

            //for (int i = 0; i <= 100; i++)
            //{
            //    Dictionary<string, object> newRow = new()
            //    {
            //        { "出庫予定日", $"2022/12/21" },
            //        { "棚番", $"A1-01-01-{i:00}" },
            //        { "案件名", $"TEST" },
            //        { "メーカー名", $"JJJJJJJJJ" },
            //        { "品名", $"JJJJJJJJJ" },
            //        { "品番", $"XXXXXX" },
            //        { "数量", $"{1:0}" }
            //    };

            //    _dataParent.Add(newRow);
            //}
            //AttributesGrid.Add("Data", _dataParent);
            //AttributesGrid.Add("SelectionMode", DataGridSelectionMode.Multiple);
            //AttributesGrid.Add("FilterMode", FilterMode.Advanced);
            AttributesFuncButton = new Dictionary<string, object>(){
                { "button1text", string.Empty },
                { "button2text", "検索[F2]" },
                { "button3text", "明細[F3]" },
                { "button4text", string.Empty },
                { "button5text", string.Empty },
                { "button6text", string.Empty },
                { "button7text", string.Empty },
                { "button8text", string.Empty },
                { "button9text", string.Empty },
                { "button10text", string.Empty },
                { "button11text", $"クリア[F11]" },
                { "button12text", $"戻る[F12]" },
            };
            //TODO DBからページタイトルを取得する
            pageName = "出荷検品進捗";
            base.OnUpdateParentPageTitle(pageName);
            await base.OnInitializedAsync();
        }


        private async Task OnClickResultF2(string result)
        {
            //TODO
            if (AssemblyState.Debug)
            {
                bool? ret = await DialogService.Confirm("Are you sure?", "MyTitle", new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });
                bool retb = ret is not null && (bool)ret;

                NotificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Info,
                    Summary = "test",
                    Detail = retb ? "OK" : "NO",
                    Duration = 3000
                });
            }


        }
        private void OnClickResultF3(string result)
        {
            //TODO
            if (AssemblyState.Debug)
            {
                NotificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "test",
                    Detail = "TEST",
                    Duration = 3000
                });
            }
        }

        protected void OnChange()
        {
        }

        private void HeaderCellRender(DataGridCellRenderEventArgs<ShippingInspectProgressData> args)
        {
            // 進捗率のヘッダセルを結合する
            if (0 == args.Column.Property.IndexOf("Prosess"))
            {
                if (int.TryParse(args.Column.Property.Replace("Prosess", ""), out int no))
                {
                    if (no % Bunkatu == 0)
                    {
                        args.Attributes.Add("colspan", Bunkatu);
                    }
                }
            }
        }

        private void CellRender(DataGridCellRenderEventArgs<ShippingInspectProgressData> args)
        {
            // 進捗率セルの背景色を設定する
            if (0 == args.Column.Property.IndexOf("Prosess"))
            {
                if (int.TryParse(args.Column.Property.Replace("Prosess", ""), out int no))
                {
                    args.Attributes.Add("style", $"background-color: {(args.Data.ShippingProcess >= (no + 1) * (10 / Bunkatu) ? "#FFD966" : "var(--rz-base-background-color)")};");
                }
            }
        }
    }
}
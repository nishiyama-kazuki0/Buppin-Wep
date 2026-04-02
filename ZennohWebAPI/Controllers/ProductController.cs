using Anotar.Serilog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RepoDb;
using System.Reflection;
using ZennohBlazorShared.Data;

namespace ZennohWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [NonAction]
        public static T GetEnumValue<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        [HttpGet(Name = "GetProduct")]
        public ActionResult<IEnumerable<ResponseValue>> Get()
        {
            try
            {
                // Enumを文字列から値に変換
                string strTextAlign = "TextAlign.Center";
                string strTextAlignPos = strTextAlign[(strTextAlign.LastIndexOf('.') + 1)..];
                //Type type = Type.GetType($"AdvanceSoftware.VBReport.TextAlign, {typeof(TextAlign).Assembly}");
                Type type = Type.GetType($"AdvanceSoftware.VBReport.TextAlign, AdvanceSoftware.VBReport.Shared");
                //Type type = typeof(TextAlign);// Type.GetType("AdvanceSoftware.VBReport.TextAlign, AdvanceSoftware.VBReport");

                object enumValue = typeof(ProductController).GetMethod("GetEnumValue").MakeGenericMethod(type).Invoke(null, new object[] { strTextAlignPos });


                //TextAlign align = GetEnumValue<TextAlign>(strTextAlignPos);
                //TextAlign align1 = TextAlign.Left;

                // テーブルより取得する値、データタイプ
                string strValue = "1";
                string typeName = "System.Int32";
                // 文字列からデータタイプの値に変換
                object? value1 = Convert.ChangeType(strValue, Type.GetType(typeName));

                // 呼び出すメソッド名（テーブルのプログラム名から取得）
                //string strPgName = "ZennohWebAPI.MyClass.MyMethod";
                //string strNsName = strPgName[..strPgName.LastIndexOf('.')];
                //string strMdName = strPgName[(strPgName.LastIndexOf('.') + 1)..];
                //↑確認に不要だったため、一旦コメント化

                // ExpressionCompilerを使用しない呼び出し
                //
                // 例１
                //
                // 呼び出すメソッド名（テーブルのプログラム名から取得）
                //Type? nsType = Type.GetType(strNsName);
                //object? obj = Activator.CreateInstance(nsType);
                //MethodInfo? methodInfo = nsType.GetMethod(strMdName);
                //object[] parameters = new object[] { value1 };
                //// メソッドに変数を渡して戻り値を受ける
                //object? r4 = methodInfo.Invoke(obj, parameters);
                //Console.WriteLine(r4); // 101
                //↑確認に不要だったため、一旦コメント化

                //
                // 例２（Expressionを使用した方法）
                // MyClassをnewするため汎用的ではないと思われる
                //MyClass myClass = new();
                //ParameterExpression x = Expression.Parameter(typeof(int), "x");
                //MethodCallExpression call = Expression.Call(Expression.Constant(myClass), typeof(MyClass).GetMethod(strMdName), x);
                //Expression<Func<int, int>> lambda = Expression.Lambda<Func<int, int>>(call, x);
                //Func<int, int> func = lambda.Compile();
                //int r3 = func.Invoke((int)value1);
                //Console.WriteLine(r3); // 101
                //↑確認に不要だったため、一旦コメント化


                //Type? nsType = Type.GetType(strNsName);
                //object? obj = Activator.CreateInstance(nsType);

                string methodName = "MyMet";
                object[] parameters = new object[] { value1 };
                //MethodInfo method = typeof(MyClass).GetMethod(methodName);

                // MyClassが持っているメソッドの属性を取得し、属性が一致するメソッドを実行する
                Type? clsType = Type.GetType("ZennohWebAPI.MyClass");
                MethodInfo method = null;
                foreach (MethodInfo info in clsType.GetMethods())
                {
                    Attribute[] authors = Attribute.GetCustomAttributes(info, typeof(MyAttribute));
                    if (authors.Where(_ => _ is MyAttribute && ((MyAttribute)_).Name == methodName).Any())
                    {
                        // 属性が一致するメソッドを保持する
                        method = info;
                        break;
                    }
                }
                if (null != method)
                {
                    // メソッドに変数を渡して戻り値を受ける
                    object? r4 = method.Invoke(null, parameters);
                    Console.WriteLine(r4); // 101
                }

                // 
                // 以下、ExpressionCompilerプロジェクトを使用して呼び出す
                // githubのサンプルソースを実行
                //Func<int> calculator = new ExpressionCompiler(
                // "return (int)Math.Round(Math.PI);")
                //.WithUsing("System")
                //.Returns(typeof(int))
                //.Compile<Func<int>>();
                //int r = calculator.Invoke();
                //Console.WriteLine(r); // Prints 3

                // 自作クラスのメソッドを実行させる
                try
                {
                    string className = "ZennohWebAPI.MyClass";
                    System.Reflection.Assembly ass = Type.GetType(className).Assembly;
                    //Func<int> calculator2 = new ExpressionCompiler(
                    // $"return MyClass.MyMethod({value1});"
                    // )
                    ////.WithReference(Type.GetType(className).Assembly)
                    //.WithReference(ass)
                    //.WithUsing("System")
                    //.WithUsing("ZennohWebAPI")
                    //.Returns(typeof(int))
                    //.Compile<Func<int>>();                 // ←ここで例外
                    // 【例外内容】Generated code以下が、ExpressionCompilerが自動で作成したラムダ式のクラス
                    //Compilation failed:
                    //CS0246: The type or namespace name 'ZennohWebAPI' could not be found (are you missing a using directive or an assembly reference?)
                    //CS0103: The name 'MyClass' does not exist in the current context
                    //Generated code:
                    //using System;
                    //using ZennohWebAPI;
                    //
                    //namespace ExpressionCompilerNamespace
                    //{
                    //    class ExpressionCompilerClass
                    //    {
                    //        public static System.Int32 ExpressionMethod()
                    //        {
                    //            return MyClass.MyMethod(1);
                    //        }
                    //    }
                    //}
                    //int r2 = calculator2.Invoke();
                    //Console.WriteLine(r2); // 2
                }
                catch (Exception ex)
                {
                    LogTo.Fatal(ex.Message);
                }

                // ↑↑ここまで調査用コード


                IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string? connString = config.GetConnectionString("DefaultConnection");

                using SqlConnection connection = new(connString);
                connection.Open();
                //                IEnumerable<DEFINE_DEVICE_TYPES> lst = connection.QueryAll<DEFINE_DEVICE_TYPES>();
                string strSQL = @"
                                    SELECT 
                                        DEVICE_TYPE,
                                        DEVICE_TYPE_NAME,
                                        JUDGE_STRING,
                                        DEVICE_TYPE_GROUP_ID 
                                    FROM
                                        DEFINE_DEVICE_TYPES
                                    ;";
                IEnumerable<IDictionary<string, object>> lst = connection.ExecuteQuery<IDictionary<string, object>>(strSQL);
                List<ResponseValue> res = new();
                int i = 0;
                foreach (IDictionary<string, object> item in lst)
                {
                    ResponseValue todoItem = new()
                    {
                        Columns = new List<string>(),
                        Values = new Dictionary<string, object>()
                    };
                    foreach (string key in item.Keys)
                    {
                        todoItem.Columns.Add(key);
                        object val = item[key];
                        todoItem.Values.Add(key, val);
                    }
                    todoItem.Values.Add("intval", i);
                    todoItem.Values.Add("doubleval", i * 1.1d);
                    todoItem.Values.Add("stringval", "value" + i.ToString());
                    todoItem.Values.Add("timeval", DateTime.Now.AddHours(i));
                    (int x, int n, string y) tuple = (1, 2, "tupletest");
                    todoItem.Values.Add("tupleval", tuple);
                    i++;
                    res.Add(todoItem);
                }

                //foreach (DEFINE_DEVICE_TYPES item in lst)
                //{
                //    //var val = new
                //    //{
                //    //    left = item.COLOR_CD,
                //    //    right = item.COLOR_NAME
                //    //};
                //    //(string id, string name) = (item.COLOR_CD, item.COLOR_NAME);
                //    //ValueTuple<string?, string?> val = new(item.COLOR_CD, item.COLOR_NAME);
                //    ResponseValue todoItem = new ResponseValue();
                //    todoItem.Values = new Dictionary<string, object>();
                //    todoItem.Values.Add(nameof(item.DEVICE_TYPE), item.DEVICE_TYPE);
                //    todoItem.Values.Add(nameof(item.DEVICE_TYPE_NAME), item.DEVICE_TYPE_NAME);
                //    todoItem.Values.Add(nameof(item.JUDGE_STRING), item.JUDGE_STRING);
                //    todoItem.Values.Add(nameof(item.DEVICE_TYPE_GROUP_ID), item.DEVICE_TYPE_GROUP_ID);
                //    todoItem.Values.Add("intval", i);
                //    todoItem.Values.Add("doubleval", i * 1.1d);
                //    todoItem.Values.Add("stringval", "value" + i.ToString());
                //    todoItem.Values.Add("timeval", DateTime.Now.AddHours(i));
                //    (int x, int n, string y) tuple = (1, 2, "tupletest");
                //    todoItem.Values.Add("tupleval", tuple);
                //    i++;
                //    res.Add(todoItem);
                //}
                return res;
                //IEnumerable<T> people = connection.QueryAll<T>();

            }
            catch (Exception e)
            {
                LogTo.Fatal(e.Message);
            }

            return Ok();
            //return await connection.QueryAllAsync<WeatherForecast>();

            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            //    TemperatureC = Random.Shared.Next(-20, 55),
            //    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            //})
            //.ToArray();
        }

        //[HttpGet(Name = "GetWeatherForecast2")]
        //public IEnumerable<WeatherForecast>? GetWeatherForecast2()
        //{
        //    try
        //    {
        //        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //        string connString = config.GetConnectionString("DefaultConnection");

        //        using SqlConnection connection = new(connString);
        //        connection.Open();

        //        return connection.QueryAll<WeatherForecast>();
        //        //IEnumerable<T> people = connection.QueryAll<T>();

        //    }
        //    catch (Exception e)
        //    {
        //        System.Console.WriteLine(e.Message);
        //    }

        //    return null;
        //    //return await connection.QueryAllAsync<WeatherForecast>();

        //    //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    //{
        //    //    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //    //    TemperatureC = Random.Shared.Next(-20, 55),
        //    //    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    //})
        //    //.ToArray();
        //}

        //// GET: Products/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return StatusCode(StatusCodes.Status404NotFound);
        //    }
        //    Product product = productRepository.FindByID(id.Value);
        //    return product == null ? StatusCode(StatusCodes.Status404NotFound) : View(product);
        //}
    }

    //[ApiController]
    //[Route("[controller]")]
    //public class ProductController : Controller
    //{
    //    private readonly IRepository<Product> productRepository;

    //    private readonly ILogger<ProductController> _logger;

    //    public ProductController(ILogger<ProductController> logger)
    //    {
    //        _logger = logger;
    //    }

    //    public ProductController(IRepository<Product> repository)
    //    {
    //        productRepository = repository;
    //    }

    //    [HttpGet(Name = "GetProduct")]
    //    public IEnumerable<DC_COLOR>? Get()
    //    {
    //        try
    //        {
    //            IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
    //            string connString = config.GetConnectionString("DefaultConnection");

    //            using SqlConnection connection = new(connString);
    //            connection.Open();

    //            return connection.QueryAll<DC_COLOR>();
    //            //IEnumerable<T> people = connection.QueryAll<T>();

    //        }
    //        catch (Exception e)
    //        {
    //            System.Console.WriteLine(e.Message);
    //        }

    //        return null;
    //        //return await connection.QueryAllAsync<WeatherForecast>();

    //        //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //        //{
    //        //    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
    //        //    TemperatureC = Random.Shared.Next(-20, 55),
    //        //    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //        //})
    //        //.ToArray();
    //    }

    //    // GET: Products
    //    public ActionResult Index()
    //    {
    //        return View(productRepository.FindAll().ToList());
    //    }

    //    // GET: Products/Details/5
    //    public ActionResult Details(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return StatusCode(StatusCodes.Status404NotFound);
    //        }
    //        Product product = productRepository.FindByID(id.Value);
    //        return product == null ? StatusCode(StatusCodes.Status404NotFound) : View(product);
    //    }

    //    // GET: Products/Create
    //    public ActionResult Create()
    //    {
    //        return View();
    //    }

    //    // POST: Products/Create
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Create([Bind("Id,Name,Quantity,Price")] Product product)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            productRepository.Add(product);
    //            return RedirectToAction("Index");
    //        }

    //        return View(product);
    //    }

    //    // GET: Products/Edit/5
    //    public ActionResult Edit(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return StatusCode(StatusCodes.Status400BadRequest);
    //        }
    //        Product product = productRepository.FindByID(id.Value);
    //        return product == null ? StatusCode(StatusCodes.Status404NotFound) : View(product);
    //    }

    //    // POST: Products/Edit/5
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Edit([Bind("Id,Name,Quantity,Price")] Product product)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            productRepository.Update(product);
    //            return RedirectToAction("Index");
    //        }
    //        return View(product);
    //    }

    //    // GET: Products/Delete/5
    //    public ActionResult Delete(int? id)
    //    {
    //        if (id == null)
    //        {
    //            return StatusCode(StatusCodes.Status400BadRequest);
    //        }
    //        Product product = productRepository.FindByID(id.Value);
    //        return product == null ? StatusCode(StatusCodes.Status404NotFound) : View(product);
    //    }

    //    // POST: Products/Delete/5
    //    [HttpPost, ActionName("Delete")]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult DeleteConfirmed(int id)
    //    {
    //        productRepository.Remove(id);
    //        return RedirectToAction("Index");
    //    }
    //}
}
using Anotar.Serilog;

namespace ZennohWebAPI
{
    public static class MyClass
    {
        [MyAttribute("MyMet")]
        public static int MyMethod(int val)
        {
            //try
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append(string.Format("UPDATE MST_DEVICES SET UPDATE_DATETIME = '{0}' WHERE DEVICE_ID = '{1}'", DateTime.Now.ToString("yyyyMMddHHmmdd"), "localhost"));

            //    int res = DataSource.ExecuteNonQuery(sb.ToString(), null);

            //    return res;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            return val + 100;
        }

        [MyAttribute("MyMet")]
        public static (int retCode, string retMsg) TestFunction1(int TestArg1)
        {
            LogTo.Information($"TestFunction1_In:{TestArg1}");
            //try
            //{
            //    StringBuilder sb = new StringBuilder();
            //    sb.Append(string.Format("UPDATE MST_DEVICES SET UPDATE_DATETIME = '{0}' WHERE DEVICE_ID = '{1}'", DateTime.Now.ToString("yyyyMMddHHmmdd"), "localhost"));

            //    int res = DataSource.ExecuteNonQuery(sb.ToString(), null);

            //    return res;
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            return (TestArg1 + 99, "TestDotNetFunc");
        }
        //[MyAttribute("MyMet")]
        public static void TestAction1()
        {
            LogTo.Information($"TestAction1_In_NOArg:");
        }
        [MyAttribute("MyMet")]
        public static void TestAction1(int TestArg1)
        {
            LogTo.Information($"TestAction1_In:{TestArg1}");
        }

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class MyAttribute : Attribute
    {
        public string Name { get; set; }

        public MyAttribute(string name)
        {
            Name = name;
        }
    }
}

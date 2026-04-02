using Moq;
using Moq.Protected;
using RepoDb;
using SharedModels;
using ZennohWebAPI.Controllers;

namespace WebAPIxUnitTest
{
    public class ExecutionControllerTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();

        /// <summary>
        /// 単純な引数を渡すテスト
        /// </summary>
        [Fact]
        public void GivenValidRequestValue_ReturnsExpectedResult()
        {
            // Arrange

            int testArg1 = 1;
            //テスト用のパラメータを作成する
            RequestValue requestValue =
                RequestValue.CreateRequestProgram("TestProgram")
                .SetArgumentValue("TestArg1", testArg1, testArg1.GetType().Name)
                .SetArgumentValue("SEQ", testArg1, testArg1.GetType().Name);

            //結果を定義する
            List<ExecResult> expected = new()
            {
                new ExecResult("TestProgram", "upc_TestStoredProcedure1", 1, 0, "")
                ,new ExecResult("TestProgram", "TestAction1", 2, 0, "")
                ,new ExecResult("TestProgram", "TestAction1", 3, 0, "")
                ,new ExecResult("TestProgram", "TestFunction1", 4, 100, "TestDotNetFunc")
            };

            //コントローラーとMoqテストを実行する
            TestExecute(requestValue, expected);
        }

        /// <summary>
        /// ストアドにユーザー定義テーブル型を渡すテスト
        /// </summary>
        [Fact]
        public void TestProcedureUseArgmentDataSet()
        {
            // Arrange

            //テスト用のパラメータを作成する

            List<List<ArgumentValue>> argSet = new();

            //1行目
            List<ArgumentValue> argList = new()
            {
                ArgumentValue.CreateArgumentValue("COL_INT",1,"int"),
                ArgumentValue.CreateArgumentValue("COL_VARCHAR", "TEST1", "varchar"),
                ArgumentValue.CreateArgumentValue("COL_BIT", true, "bit")
            };
            argSet.Add(argList);

            //2行目
            argList.Add(ArgumentValue.CreateArgumentValue("COL_INT", 2, "int"));
            argList.Add(ArgumentValue.CreateArgumentValue("COL_VARCHAR", "TEST2", "TEST2"));
            argList.Add(ArgumentValue.CreateArgumentValue("COL_BIT", false, "bit"));

            argSet.Add(argList);

            RequestValue requestValue =
                RequestValue.CreateRequestProgram("TestProgram2")
                .SetArgumentDataset("I_TEST_UT", argSet);

            //結果を定義する
            List<ExecResult> expected = new()
            {
                new ExecResult("TestProgram2", "upc_TestStoredProcedure2", 1, 0, "")
            };

            //コントローラーとMoqテストを実行する
            TestExecute(requestValue, expected);
        }

        /// <summary>
        /// ストアドでOUTPUT型の戻り値を受けるテスト
        /// </summary>
        [Fact]
        public void TestProcedureReturnOutPutParam()
        {
            // Arrange

            //テスト用のパラメータを作成する
            RequestValue requestValue =
                RequestValue.CreateRequestProgram("TestProgram3");

            //結果を定義する
            List<ExecResult> expected = new()
            {
                new ExecResult("TestProgram3", "upc_TestStoredProcedure3", 1, 99, "TEST")
            };

            //コントローラーとMoqテストを実行する
            TestExecute(requestValue, expected);
        }

        /// <summary>
        /// 単純な.netメソッドを実行するテスト
        /// </summary>
        [Fact]
        public void TestDotNetFunctionNormal()
        {
            // Arrange

            //TODO .netをメソッドを実行する単純なテストを作成する
            //テスト用のパラメータを作成する
            RequestValue requestValue = RequestValue.CreateRequestProgram("TestProgram");
            int testArg1 = 1;
            _ = requestValue.SetArgumentValue("TestArg1", testArg1, testArg1.GetType().Name);
            _ = requestValue.SetArgumentValue("SEQ", testArg1, testArg1.GetType().Name);


            //結果を定義する
            List<ExecResult> expected = new()
            {
                new ExecResult("TestProgram", "TestFunction", 1, 0, "Test Result")
            };

            //コントローラーとMoqテストを実行する
            TestExecute(requestValue, expected);
        }

        /// <summary>
        /// バッチファイルを実行する単純なテスト
        /// </summary>
        [Fact]
        public void TestBatchFileExecNormal()
        {
            // Arrange

            //TODO バッチファイルを実行する単純なテストを作成する
            //テスト用のパラメータを作成する
            RequestValue requestValue = RequestValue.CreateRequestProgram("TestProgram");
            int testArg1 = 1;
            _ = requestValue.SetArgumentValue("TestArg1", testArg1, testArg1.GetType().Name);
            _ = requestValue.SetArgumentValue("SEQ", testArg1, testArg1.GetType().Name);

            //結果を定義する
            List<ExecResult> expected = new()
            {
                new ExecResult("TestProgram", "TestFunction", 1, 0, "Test Result")
            };

            //コントローラーとMoqテストを実行する
            TestExecute(requestValue, expected);
        }


        /// <summary>
        /// コントローラーをMoq実行する
        /// </summary>
        /// <param name="requestMoqArg"></param>
        /// <param name="expected"></param>
        private void TestExecute(RequestValue requestMoqArg, List<ExecResult> expected)
        {
            //ExecutionController controller = new(_httpClientFactoryMock.Object);
            ExecutionController controller = new();

            Mock<HttpMessageHandler> mockHttpMessageHandler = new();
            _ = mockHttpMessageHandler.Protected()
                                  .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
            //.ReturnsAsync(new HttpResponseMessage
            //{
            //    StatusCode = HttpStatusCode.OK,
            //    Content = new StringContent(expected)
            //});
            HttpClient httpClient = new(mockHttpMessageHandler.Object);
            _ = _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
                                  .Returns(httpClient);

            //RepoDBのセットアップ(SQL Server)
            _ = GlobalConfiguration
                .Setup()
                .UseSqlServer();

            // Act
            Microsoft.AspNetCore.Mvc.ActionResult<IEnumerable<ExecResult>> result = controller.Post(requestMoqArg);

            // Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.NotNull(result.Value);
            List<ExecResult> execResults = result.Value.ToList();
            Xunit.Assert.Equal(expected.Count(), execResults.Count());
            //exexResultsとexpectedの比較
            for (int i = 0; i < execResults.Count(); i++)
            {
                Xunit.Assert.Equal(expected[i].ProgramName, execResults[i].ProgramName);
                Xunit.Assert.Equal(expected[i].FunctionName, execResults[i].FunctionName);
                Xunit.Assert.Equal(expected[i].ExecOrderRank, execResults[i].ExecOrderRank);
                Xunit.Assert.Equal(expected[i].RetCode, execResults[i].RetCode);
                Xunit.Assert.Equal(expected[i].Message, execResults[i].Message);
            }
            //CollectionAssert.AreEqual((System.Collections.ICollection?)expected, (System.Collections.ICollection?)result.Value);
        }
    }
}

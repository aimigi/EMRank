using CustomerRankAPI.Service;
using System.Diagnostics;
using Xunit.Abstractions;

namespace CustomerRankTest
{
    public class RankServiceTest
    {

        private List<(long CustomerId, int Socre)> TestData = new List<(long CustomerId, int Socre)>
        {
           (15514665, 124),
           (81546541,113),
           (1745431, 100),
           (76786448, 100),
           (254814111, 96),
           (53274324,95),
           (6144320,93),
           (8009471,93),
           (11028481, 93),
           (38819,92),
           (38819,-2),
           (388192,-1),
           (388193,98),
           (15514665, 121),
           (388192,98),
        };

        private readonly IRankService _service;
        private readonly ITestOutputHelper _output;
        public RankServiceTest(ITestOutputHelper testOutputHelper)
        {
            _service = new CustomerRankOffsetAlg();

            _output = testOutputHelper;
        }



        private List<(long CustomerId, int Score)> GenerateTestData(int n)
        {
            var ret = new List<(long CustomerId, int Score)>();
            for (var i = 0; i < n; i++)
            {
                var customerid = new Random().NextInt64(100000, long.MaxValue);
                var score = new Random().Next(-100, 10000);
                ret.Add((customerid, score));
            }
            return ret;
        }
        [Fact]
        public void UpdateScorePerformanceTest()
        {
            var n = 100000;
            var testData = GenerateTestData(n);

            var sw = new Stopwatch();
            sw.Start();
            testData.AsParallel().ForAll(item =>
            {
                _service.UpdateCustomerScore(item.CustomerId, item.Score);
            });
            sw.Stop();
            _output.WriteLine($"ori update  {n} elapsed time:{sw.ElapsedMilliseconds}  ms");

            Assert.True(true);
        }



        [Fact]
        public void UpdateScoreTest()
        {
            var n = 100000;
            var testData = GenerateTestData(n);
            var sw = new Stopwatch();
            sw.Start();
            testData.ForEach(item =>
            {
                _service.UpdateCustomerScore(item.CustomerId, item.Score);
            });
            sw.Stop();
            _output.WriteLine($"ori update  {n} elapsed time:{sw.ElapsedMilliseconds}  ms");
            Assert.True(true);
        }

        [Fact]
        public void GetCustomersByRankTest()
        {
            TestData.ForEach(item =>
            {
                _service.UpdateCustomerScore(item.CustomerId, item.Socre);
                _output.WriteLine($"inserted -----");
                for (long t = 0; t < _service.Length; t++)
                {
                    var itemT = _service.SortedArray[t];
                    _output.WriteLine($"customer id : {itemT.Customerid},score:{itemT.Score},rank:{_service.CustomerRank[itemT.Customerid]}");

                }
            });
            _output.WriteLine($"-----result -----");
            var ret = _service.GetCustomersByRank(1, 20);

            foreach (var item in ret)
            {
                _output.WriteLine($"customer id:{item.Customerid},score:{item.Score},rank:{item.Rank}");
            }
            Assert.NotNull(ret);
        }

        [Fact]
        public void GetCustomerNearRankTest()
        {
            TestData.ForEach(item =>
            {
                _service.UpdateCustomerScore(item.CustomerId, item.Socre);
            });


            var ret = _service.GetCustomerNearRank(15514665, 1, 3);
            foreach (var item in ret)
            {
                _output.WriteLine($"customer id:{item.Customerid},score:{item.Score},rank:{item.Rank}");
            }

            Assert.NotNull(ret);
        }
    }
}
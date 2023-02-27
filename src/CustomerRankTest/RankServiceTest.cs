using CustomerRankAPI;
using CustomerRankAPI.Service;
using System.Text.Json;
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
        };

        private readonly IRankService _service;
        private readonly ITestOutputHelper _output;
        public RankServiceTest(ITestOutputHelper testOutputHelper)
        {
            _service = new CustomerRankSortedArray();

            _output = testOutputHelper;
        }
        [Fact]
        public void UpdateScoreTest()
        {
            TestData.ForEach(item =>
            {
                _service.UpdateCustomerScore(item.CustomerId, item.Socre);
                _output.WriteLine($"inserted -----");
                for (long t = 0; t < _service.Length; t++)
                {
                    _output.WriteLine($"customer id : {_service.SortedArray[t].Customerid},score:{_service.SortedArray[t].Score}");

                }
            });
            Assert.True(true);
        }

        [Fact]
        public void GetCustomersByRankTest()
        {
            TestData.ForEach(item =>
            {
                _service.UpdateCustomerScore(item.CustomerId, item.Socre);
            });


            var ret = _service.GetCustomersByRank(1, 10);

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
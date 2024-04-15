
namespace CustomerRankAPI.Service
{
    public class SkipListRankService : IRankService
    {
        private volatile int _isSafe = 1;
        public readonly SkipList<CustomerScoreRankModel> _scoreList;

        public SkipListRankService()
        {
            // The maximum number of layers and random probability for table skipping can be set based on the amount of data
            // default level=16  Suitable for maximum data  2^16
            _scoreList = new SkipList<CustomerScoreRankModel>();
        }

        public IEnumerable<CustomerScoreRankModel> GetCustomerNearRank(long customerid, int high, int low)
        {
            // O(n) Loop
            var node = _scoreList.FindNodeByExpression(x => x.Customerid == customerid);
            if (node != null)
            {
                var start = node.Value.Rank - high;
                if (start < 1)
                {
                    start = 1;
                }
                var end = node.Value.Rank + low;
                if (end > _scoreList.Length)
                {
                    end = _scoreList.Length;
                }

                var list = _scoreList.GetRangeValues(start, end);
                return list;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<CustomerScoreRankModel> GetCustomersByRank(int start, int end)
        {
            if (start > end)
            {
                throw new ArgumentException("start cannot greater than end.");
            }
            if (start > _scoreList.Length)
            {
                throw new ArgumentException("start over limit.");
            }
            if (end > _scoreList.Length)
            {
                end = _scoreList.Length;
            }
            var ret = _scoreList.GetRangeValues(start, end);
            return ret;

        }

        public bool UpdateCustomerScore(long customerid, int score)
        {

            // performce lock
            if (Interlocked.Exchange(ref _isSafe, 0) == 1)
            {
                // O(n) Loop
                var node = _scoreList.FindNodeByExpression(x => x.Customerid == customerid);
                if (node != null)
                {
                    _scoreList.Delete(node.Value);
                    _scoreList.Insert(new CustomerScoreRankModel
                    {
                        Customerid = customerid,
                        Score = node.Value.Score + score
                    });
                }
                else
                {
                    _scoreList.Insert(new CustomerScoreRankModel
                    {
                        Customerid = customerid,
                        Score = score
                    });

                }
                Interlocked.Exchange(ref _isSafe, 1);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

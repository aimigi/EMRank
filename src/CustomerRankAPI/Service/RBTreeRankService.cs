using CustomerRankAPI.RBTree;

namespace CustomerRankAPI.Service
{
    public class RBTreeRankService : IRankService
    {
        private volatile int _isSafe = 1;

        private readonly RBTreeSimple<CustomerScoreRankModel> _tree;

        public RBTreeRankService()
        {
            _tree = new RBTreeSimple<CustomerScoreRankModel>();
        }

        public IEnumerable<CustomerScoreRankModel> GetCustomerNearRank(long customerid, int high, int low)
        {
            var node = _tree.FindNodeByExpression(x => x.Customerid == customerid);
            if (node == null)
            {
                return null;
            }
            else
            {
                var start = node.Value.Rank - high;
                if (start < 1)
                {
                    start = 1;
                }
                var end = node.Value.Rank + low;
                if (end > _tree.Total)
                {
                    end = _tree.Total;
                }

                return _tree.GetRangeValues(start, end);
            }

        }

        public IEnumerable<CustomerScoreRankModel> GetCustomersByRank(int start, int end)
        {
            if (start > end)
            {
                throw new ArgumentException("start cannot greater than end.");
            }
            if (start > _tree.Total)
            {
                return null;
            }
            if (end > _tree.Total)
            {
                end = _tree.Total;
            }
            

            var ret = _tree.GetRangeValues(start, end);
            return ret;
        }

        public bool UpdateCustomerScore(long customerid, int score)
        {
            // performce lock
            if (Interlocked.Exchange(ref _isSafe, 0) == 1)
            {
                var node = _tree.FindNodeByExpression(x => x.Customerid == customerid);
                if (node != null)
                {
                    _tree.DeleteNode(node);
                    _tree.Add(new CustomerScoreRankModel
                    {
                        Customerid = customerid,
                        Score = node.Value.Score + score
                    });
                }
                else
                {
                    _tree.Add(new CustomerScoreRankModel
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

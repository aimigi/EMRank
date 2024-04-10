using CustomerRankAPI.RBTree;

namespace CustomerRankAPI.Service
{
    public class CustomerRankOffsetAlg : IRankService
    {
        private volatile int _isSafe = 1;

        private readonly Tree<CustomerScoreRankModel> _tree;

        public CustomerRankOffsetAlg()
        {
            _tree = new Tree<CustomerScoreRankModel>();
        }

        public IEnumerable<CustomerScoreRankModel> GetCustomerNearRank(long customerid, int high, int low)
        {
            var result = new List<CustomerScoreRankModel>();

            return result;
        }

        public IEnumerable<CustomerScoreRankModel> GetCustomersByRank(long start, long end)
        {
            var result = new List<CustomerScoreRankModel>();

            if (start > end)
            {
                throw new ArgumentException("start cannot greater than end.");
            }
            if (start > _tree.Total)
            {
                throw new ArgumentException("start over limit.");
            }
            if (end > _tree.Total)
            {
                end = _tree.Total;
            }
            for (long index = start - 1; index < end; index++)
            {

            }
            return result;
        }

        public bool UpdateCustomerScore(long customerid, int score)
        {
            // performce lock
            if (Interlocked.Exchange(ref _isSafe, 0) == 1)
            {
                _tree.Insert(new CustomerScoreRankModel
                {
                    Customerid = customerid,
                    Score = score
                }, InsertHandle);

                Interlocked.Exchange(ref _isSafe, 1);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void InsertHandle(RBTreeNode<CustomerScoreRankModel> node, string arg2)
        {
            if (arg2 == "root")
            {
                node.Value.Rank = 1;
            }
            if (arg2 == "left")
            {
                node.Value.Rank = node.Left.Value.Rank + 1;
                ParentTraversal(node.Parent);
                InOrderRightTraversal(node.Right);
            }
            if (arg2 == "right")
            {
                InOrderRightTraversal(node.Right);
            }
        }

        private void InOrderRightTraversal(RBTreeNode<CustomerScoreRankModel> node)
        {

            if (node != null)
            {
                node.Value.Rank += 1;
            }
            if (node.Right != null)
            {
                InOrderRightTraversal(node.Right);               
            }
        }

        private void ParentTraversal(RBTreeNode<CustomerScoreRankModel> node)
        {
            if (node != null)
            {
                node.Value.Rank += 1;
            }
            if (node.Parent != null)
            {
                ParentTraversal(node.Parent);
            }
        }
    }
}

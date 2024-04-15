using CustomerRankAPI;
using CustomerRankAPI.RBTree;
using CustomerRankAPI.Service;

namespace CustomerRankConsoleTest
{
    internal class Program
    {

        private static SkipList<CustomerScoreRankModel> _scoreList;

        private static RBTreeSimple<CustomerScoreRankModel> _scoreTree;

        private static List<(long CustomerId, int Socre)> TestData = new List<(long CustomerId, int Socre)>
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
           (15514665, -10),
           (388192,98),
        };

        #region RBTree 测试


        public static IEnumerable<CustomerScoreRankModel> GetCustomerNearRank(long customerid, int high, int low)
        {
            var node = _scoreTree.FindNodeByExpression(x => x.Customerid == customerid);
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
                if (end > _scoreTree.Total)
                {
                    end = _scoreTree.Total;
                }

                return _scoreTree.GetRangeValues(start, end);
            }

        }
        public static IEnumerable<CustomerScoreRankModel> GetCustomersByRank(int start, int end)
        {
            if (start > end)
            {
                throw new ArgumentException("start cannot greater than end.");
            }
            if (start > _scoreTree.Total)
            {
                return null;
            }
            if (end > _scoreTree.Total)
            {
                end = _scoreTree.Total;
            }


            var ret = _scoreTree.GetRangeValues(start, end);
            return ret;
        }


     

        static void RBTreeTest()
        {

            _scoreTree = new RBTreeSimple<CustomerScoreRankModel>();
            foreach (var item in TestData)
            {
                var node = _scoreTree.FindNodeByExpression(x => x.Customerid == item.CustomerId);
                if (node != null)
                {
                    _scoreTree.DeleteNode(node);
                    _scoreTree.Add(new CustomerScoreRankModel
                    {
                        Customerid = item.CustomerId,
                        Score = node.Value.Score + item.Socre
                    });
                }
                else
                {
                    _scoreTree.Add(new CustomerScoreRankModel
                    {
                        Customerid = item.CustomerId,
                        Score = item.Socre
                    });

                }

            }

            Console.WriteLine("---------------tree nodes---------------");
            _scoreTree.InOrderPrint();
            Console.WriteLine("---------------GetCustomerNearRank 8009471 high 3,low 4---------------");
            var list = GetCustomerNearRank(8009471, 3, 4);
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("---------------GetCustomersByRank from 3-5---------------");
            var list2 = GetCustomersByRank(3, 5);
            foreach (var item in list2)
            {
                Console.WriteLine(item);
            }

        }

        #endregion

        #region SkipList Test

        static void SkipListTest()
        {
            var service = new SkipListRankService();
            foreach (var item in TestData)
            {
                service.UpdateCustomerScore(item.CustomerId, item.Socre);                
            }
            Console.WriteLine($"-------current list total {service._scoreList.Length}-------");
            service._scoreList.Print();

            Console.WriteLine("---------------GetCustomerNearRank 8009471 high 3,low 4---------------");
            var list = service.GetCustomerNearRank(8009471, 3, 4);
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("---------------GetCustomersByRank from 3-5---------------");
            var list2 = service.GetCustomersByRank(3, 5);
            foreach (var item in list2)
            {
                Console.WriteLine(item);
            }


        } 
        #endregion

        static void Main(string[] args)
        {

            #region RBTree test
            RBTreeTest();
            #endregion

            SkipListTest();

        }



        static void RBTreeRotateLeftTest()
        {
            var testData = new List<CustomerScoreRankModel>
            {

                 new CustomerScoreRankModel{ Customerid = 1,Score=100},
                 new CustomerScoreRankModel{ Customerid = 2,Score=99},
                 new CustomerScoreRankModel{ Customerid = 3,Score=98},
                 new CustomerScoreRankModel{ Customerid = 5,Score=97},
                 new CustomerScoreRankModel{ Customerid = 6,Score=96}
            };
            var tree = new RBTreeSimple<CustomerScoreRankModel>();

            testData.ForEach(item => tree.Add(item));

            tree.Print();

            tree.LeftRotate(tree.Root);

            tree.Print();

            tree.RightRotate(tree.Root);

            tree.Print();



        }

        private static void InsertSkipListNode(long customerid, int score)
        {
            var node = _scoreList.FindNodeByExpression(x => x.Customerid == customerid);
            if (node != null)
            {
                _scoreList.Delete(node.Value);
                Console.WriteLine($"--delete {node.Value}-----current list total {_scoreList.Length}-------");
                _scoreList.Print();
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
        }

    }
}

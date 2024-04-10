using CustomerRankAPI.RBTree;
using CustomerRankAPI;

namespace CustomerRankConsoleTest
{
    internal class Program
    {
        private static List<(long CustomerId, int Socre)> TestData = new List<(long CustomerId, int Socre)>
        {
           (15514665, 124),
           (81546541,113),
           (1745431, 100),
           (76786448, 100),
           //(254814111, 96),
           //(53274324,95),
          // (6144320,93),
           //(8009471,93),
           //(11028481, 93),
           //(38819,92),
           //(38819,-2),
           //(388192,-1),
           //(388193,98),
           //(15514665, 121),
           //(388192,98),
        };


        static void Main(string[] args)
        {
            var tree = new Tree<CustomerScoreRankModel>();
            TestData.ForEach(x =>
            {
                tree.Insert(new CustomerScoreRankModel
                {
                    Customerid = x.CustomerId,
                    Score = x.Socre
                }, InsertHandle);
            });

            tree.PrintNode();

            Console.WriteLine($"-------total {tree.Total}-------");

            tree.InOrderPrint();
             
        }


        private static void InsertHandle(RBTreeNode<CustomerScoreRankModel> node, string arg2)
        {
            if (arg2 == "root")
            {
                node.Value.Rank = 1;
            }
            if (arg2 == "left")
            {
                if (node.Left != null)
                {
                    node.Value.Rank = node.Left.Value.Rank + 1;
                }
                else
                {
                    node.Value.Rank = 1;
                }

                ParentAndRightTraversal(node.Parent);
                //InOrderRightTraversal(node.Right);
            }
            if (arg2 == "right")
            {
                node.Value.Rank = node.Parent.Value.Rank + 1;
                InOrderRightTraversal(node.Right);
            }
        }

        private static void InOrderRightTraversal(RBTreeNode<CustomerScoreRankModel> node)
        {

            if (node != null)
            {
                node.Value.Rank += 1;
                if (node.Right != null)
                {
                    InOrderRightTraversal(node.Right);
                }
            }

        }

        private static void ParentAndRightTraversal(RBTreeNode<CustomerScoreRankModel> node)
        {
            if (node != null)
            {
                node.Value.Rank += 1;
                if (node.Right != null)
                {
                    node.Right.Value.Rank += 1;
                }
                if (node.Parent != null)
                {
                    ParentAndRightTraversal(node.Parent);
                }
                
            }
            
        }
    }
}

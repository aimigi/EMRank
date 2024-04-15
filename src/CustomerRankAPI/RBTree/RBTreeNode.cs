namespace CustomerRankAPI
{
    public interface IRankIndexNode
    {
        public int Rank { get; set; }
    }

    public class RBTreeNode<T> where T : IComparable<T>, IRankIndexNode
    {
        public T Value { get; set; }
        public RBTreeNode<T> Left { get; set; }
        public RBTreeNode<T> Right { get; set; }
        public RBTreeNode<T> Parent { get; set; }

        public bool IsRed { get; set; }

        public RBTreeNode(T value)
        {
            Value = value;
            IsRed = true;
        }

        /// <summary>
        /// 为当前node 添加子节点
        /// </summary>
        /// <param name="newNode"></param>
        public void AddNode(RBTreeNode<T> newNode)
        {
            if (newNode == null || newNode.Value.CompareTo(Value) == 0)
            {
                return;
            }

            if (newNode.Value.CompareTo(Value) < 0)
            {
                if (this.Left == null)
                {
                    this.Left = newNode;
                    newNode.Parent = this;
                }
                else
                {
                    this.Left.AddNode(newNode);
                }
            }
            else
            {
                if (this.Right == null)
                {
                    this.Right = newNode;
                    newNode.Parent = this;
                }
                else
                {
                    this.Right.AddNode(newNode);
                }
            }

        }


        public override string ToString()
        {
            return $"Node value:{Value},Red:{IsRed}";
        }
    }
}

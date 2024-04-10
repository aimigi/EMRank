namespace CustomerRankAPI.RBTree
{
    public class RBTreeNode<T> where T:IComparable<T>
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
    }
}

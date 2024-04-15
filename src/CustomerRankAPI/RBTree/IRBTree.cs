namespace CustomerRankAPI.RBTree
{
    public interface IRBTree<T> where T : IComparable<T>, IRankIndexNode
    {
        RBTreeNode<T> Root { get; }

        void AddNode(RBTreeNode<T> node);

        /// <summary>
        /// 是否为左子树
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        bool IsLeft(RBTreeNode<T> node);

        /// <summary>
        /// 以node 为支点左旋
        /// </summary>
        /// <param name="node"></param>
        void LeftRotate(RBTreeNode<T> node);
        /// <summary>
        /// 以node 为支点右旋
        /// </summary>
        /// <param name="node"></param>
        void RightRotate(RBTreeNode<T> node);

        RBRotateType GetRBRotateType(RBTreeNode<T> node);

        void Print();

        /// <summary>
        /// 查询节点
        /// </summary>
        /// <returns></returns>
        RBTreeNode<T> FindNode(RBTreeNode<T> node);

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="node"></param>
        void DeleteNode(RBTreeNode<T> node);

        /// <summary>
        /// 获取后继节点（右子树最小值）
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        RBTreeNode<T> GetSubNode(RBTreeNode<T> node);
    }
}



using System;
using System.Xml.Linq;

namespace CustomerRankAPI.RBTree
{
    public enum RBRotateType
    {
        None = 0,
        LL = 1,
        LR = 2,
        RR = 3,
        RL = 4,

    }

    public class RBTreeSimple<T> : IRBTree<T> where T : IComparable<T>, IRankIndexNode
    {

        public int Total { get; private set; }

        public RBTreeNode<T> Root { get; private set; }

        public RBTreeSimple()
        {
            Root = null;
        }

        #region 查找节点


        public RBTreeNode<T> FindNode(RBTreeNode<T> node)
        {
            var temp = Root;
            while (temp != null)
            {
                if (temp.Value.CompareTo(node.Value) == 0)
                {
                    return temp;
                }
                if (temp.Value.CompareTo(node.Value) > 0)
                {
                    temp = temp.Left;
                }
                else
                {
                    temp = temp.Right;
                }
            }
            return null;
        }

        public RBTreeNode<T> FindNodeByExpression(Func<T, bool> func)
        {
            return TraversalFindNode(Root, func);
        }

        private RBTreeNode<T> TraversalFindNode(RBTreeNode<T> node, Func<T, bool> func)
        {
            RBTreeNode<T> ret;
            if (node != null)
            {
                if (func(node.Value))
                {
                    return node;
                }
                ret = TraversalFindNode(node.Left, func);
                if (ret == null)
                {
                    ret = TraversalFindNode(node.Right, func);
                }
                return ret;


            }
            return null;
        }

        #endregion

        #region 新增节点

        public void Add(T value)
        {
            var newNode = new RBTreeNode<T>(value);
            AddNode(newNode);
        }

        /// <summary>
        /// 
        /// 新增节点没有叔叔节点的情况如下
        ///  
        /// 1.LL 型 ---> 右旋   着色
        ///     黑3     
        ///    /
        ///   红2          黑2
        ///  /            /  \
        /// 红1          红1   红3
        /// 
        /// 2. RR 型  ----> 左旋
        ///     黑3
        ///      \
        ///      红2          黑2   
        ///       \          / \
        ///       红1       红1  红3
        ///       
        /// 3. LR 型和 RL 型    先父节点左、右旋，再爷节点右、左旋
        ///    黑4        黑2
        ///    /           \
        ///   红2          红4       --->        黑3
        ///    \           /                    / \
        ///    红3        红3                  红2  红4 
        /// 
        /// 4. 左中右型不需要调整
        ///    黑   
        ///    / \  
        ///  红  红 
        /// 
        /// 5. 新增节点+祖父节点黑，，父+叔 节点为红 只需要着色调整--->   祖父节点变红，父节点和叔叔节点变黑色，如果爷爷节点为root，则调整为黑色
        /// 
        ///      黑3                        红3             
        ///     /  \                       / \
        ///    红2  红4   + 红1  ---->    黑2  黑4
        ///                              /
        ///                             红1
        ///                             
        /// 新增节点有叔叔节点 且叔叔节点为黑色
        /// 1. 父 、叔节点变黑色，爷爷节点变红色
        /// 2. 如果爷爷节点的的父节点也是红色，则向上回溯 继续旋转、变色
        /// 
        /// 
        /// **************************************************************************************
        /// 整体思路
        /// 1. 如果添加的是根节点，直接变成黑色返回
        /// 2. 如果父节点是黑色，无需要调整
        /// 3. 如果父节点红色
        ///    3.1 叔叔节点为空或为黑色---> 根据LL/LR/RR/RL 分别调整
        ///    3.2 叔叔节点为红色
        ///       3.2.1  父 、叔节点变黑色，爷爷节点变红色，如果爷爷节点的的父节点也是红色，则向上回溯 继续旋转、变色
        /// </summary>
        /// <param name="newNode"></param>
        public void AddNode(RBTreeNode<T> newNode)
        {
            if (Root == null)
            {
                Root = newNode;
                // 根节点为黑色
                Root.IsRed = false;
                Root.Value.Rank = 1;
                Total++;
            }
            else
            {
                Root.AddNode(newNode);
                Total++;
                InsertAdjust(newNode);
                FixValueRank();
            }
        }

        /// **************************************************************************************
        /// 整体思路
        /// 1. 如果添加的是根节点，直接变成黑色返回
        /// 2. 如果父节点是黑色，无需要调整
        /// 3. 如果父节点红色
        ///    3.1 叔叔节点为空或为黑色---> 根据LL/LR/RR/RL 分别调整
        ///    3.2 叔叔节点为红色
        ///       3.2.1  父 、叔节点变黑色，爷爷节点变红色，如果爷爷节点的的父节点也是红色，则向上回溯 继续旋转、变色 <summary>
        /// **************************************************************************************
        /// </summary>
        /// <param name="newNode"></param>
        public void InsertAdjust(RBTreeNode<T> newNode)
        {
            var parent = newNode.Parent;
            if (parent == null)
            {
                // 根节点，直接变黑
                newNode.IsRed = false;
                return;
            }
            // 父节点黑色，不需要调整
            if (!parent.IsRed)
            {
                return;
            }
            //3. 父节点红色，根据情况进行调整
            // 2-3 树的 3节点添加 
            var grand = parent.Parent;
            // 如果父节点是左节点，则叔叔节点为右节点
            var uncle = IsLeft(parent) ? grand.Right : grand.Left;
            // 3.1 叔叔节点为空或为黑色---> 根据LL/LR/RR/RL 分别调整
            if (uncle == null || (uncle != null && !uncle.IsRed))
            {
                // 获取旋转类型
                var rotateType = GetRBRotateType(newNode);
                switch (rotateType)
                {
                    case RBRotateType.LL:
                        grand.IsRed = true; // 设置红色
                        parent.IsRed = false; // 设置黑色
                        RightRotate(grand);
                        break;
                    case RBRotateType.RR:
                        grand.IsRed = true;
                        parent.IsRed = false;
                        LeftRotate(grand);
                        break;
                    case RBRotateType.LR:
                        grand.IsRed = true;
                        parent.IsRed = false;
                        // parent 先左旋
                        LeftRotate(parent);
                        // grand 再右旋
                        RightRotate(grand);
                        break;
                    case RBRotateType.RL:
                        grand.IsRed = true;
                        parent.IsRed = false;
                        // parent 先右旋
                        RightRotate(parent);
                        // grand 再左旋
                        LeftRotate(grand);
                        break;
                }

            }
            else  // 3.2 父 叔节点都是红色
            {
                // 爷爷节点变红色
                grand.IsRed = true;
                // 父、叔节点都变黑
                parent.IsRed = false;
                uncle.IsRed = false;
                // 递归往上检查调整
                InsertAdjust(grand);
            }
            // 让root 节点变黑
            Root.IsRed = false;
        }



        #endregion

        #region 删除节点

        /// <summary>
        /// 删除节点node
        /// 有三种情况
        /// 1. 删除叶子节点（非NIL 节点）
        ///    1.1 如果是红色节点，直接删除，不影响黑高
        ///    1.2 如果是黑色节点，黑高失衡，需要平衡调整
        ///     
        /// 2. 删除只有1 个叶子节点（非NIL 节点）的节点
        ///    2.1 此时删除的节点只能是黑色，子节点是红色，否则是无法满足红黑树性质的
        ///    2.2 将红色节点的值 copy 到父节点，将删除节点转换为删除红色叶子节点
        ///    
        /// 3. 删除有2个子节点的节点
        ///    3.1 用前驱或后继节点（>删除节点的最小值）作为删除节点的替代节点，转换1 或2 的情况
        ///    
        /// 辅助方法
        /// FindNode   查找删除节点
        /// GetPreNode   获得前驱节点
        /// GetSubNode   获得后继节点
        /// GetReplaceNode  获得替代节点
        ///        
        /// 处理逻辑
        /// 1. 没找到直接返回
        /// 2. 唯一根节点直接置空然后返回
        /// 3. 如果删除有两个子节点的节点
        ///    3.1 用前驱或后继节点（>删除节点的最小值）作为删除节点的替代节点
        ///    3.2 将删除的节点 指向替代节点 ，再转换成如下4，5处理
        /// 4. 删除只有1个子节点的节点时，删除节点只能时黑色，子节点是红色
        ///    4.1 将红色子节点copy 到父节点，将删除节点转换为删除红色叶子节点 -->5.2
        /// 5. 删除叶子节点
        ///    5.1 对要删除的黑色叶子节点进行调整
        ///    5.2 删除红黑两种叶子节点
        /// </summary>
        /// <param name="node"></param>
        public void DeleteNode(RBTreeNode<T> deleteNode)
        {
            if (deleteNode == null)
            {
                return;
            }
            // 用来返回删除的节点
            // var returnNode = deleteNode;

            if (Root == deleteNode && Root.Left == null && Root.Right == null)
            {
                Root = null;
                Total--;
                return;
            }


            RBTreeNode<T> replaceNode = null;
            // 3. 个叶子节点删除的情况
            if (deleteNode.Left != null && deleteNode.Right != null)
            {
                replaceNode = GetReplaceNode(deleteNode);
                // 置换节点的值
                deleteNode.Value = replaceNode.Value;
                deleteNode = replaceNode;
            }
            // 4. 有一个红色子节点的情况
            if ((deleteNode.Left != null && deleteNode.Right == null) || (deleteNode.Left == null && deleteNode.Right != null))
            {
                replaceNode = deleteNode.Left == null ? deleteNode.Right : deleteNode.Left;
                deleteNode.Value = replaceNode.Value;
                deleteNode = replaceNode;
            }

            // 5.1 删除黑色叶子节点,需要重调平衡
            if (!deleteNode.IsRed)
            {
                DeleteFixup(deleteNode);
            }
            var parent = deleteNode.Parent;
            // 删除节点如果是左孩子
            if (parent.Left == deleteNode)
            {
                parent.Left = null;
            }
            else
            {
                parent.Right = null;
            }
            // 引用删除
            deleteNode.Parent = null;
            deleteNode = null;
            Total--;
            // 重置所有rank
            FixValueRank();

        }

        /// <summary>
        /// 删除节点调整 整体情况分3种
        /// 一、根节点 ---> 直接变黑
        /// 二、兄弟黑色只有下面2种情况
        ///     1.1 兄弟有子节点
        ///         LL/RR/LR/RL
        ///     1.2 兄弟无子节点
        ///         父节点 红 / 黑
        ///         父节点黑色时作为新删除（调整）节点向上递归调整     
        /// 三、兄弟是红
        ///     2.1 兄弟是左子树，右旋-->右侄变红
        ///     2.2 兄弟时右子树，左旋-->左侄变红
        /// 
        /// 
        /// 兄弟是黑，右红色子节点的情况分析如下：
        /// 1. 删除的黑色节点是右子叶节点的情况
        ///   1.1 兄弟(左侧)为黑色
        ///       1.1.1 LL  兄弟有红色左子节点 LL 右旋，再变色，颜色变成原位置颜色  爷孙变黑，兄变父色
        ///             P                
        ///            / \ 
        ///          B黑  D黑      右旋---->  B黑          着色--->     B？
        ///          /                       / \                     / \
        ///        左侄红                 左侄红  P                  NL黑 P黑
        ///       
        ///      1.1.2  LR  兄弟节点只有红色右子节点 先左旋，再右旋，再恢复原位置颜色， 侄变父色，父变黑色
        ///      
        ///            P                        P
        ///           / \                      /
        ///         B黑  D   --> NR 左旋-->   NR红      NR 右旋 -->      NR红  --->变色    NR
        ///          \                       /                         / \              / \
        ///          NR红                   B黑                      B黑  P             B黑 P黑
        /// 
        ///     1.1.3  兄弟无红色子节点
        ///         1.1.3.1 父节点是红色 ---> 父变黑，兄变红
        ///             G Grand 节点 /  U  uncle 节点
        /// 
        ///              G黑                                              G黑
        ///              / \                                             / \
        ///             U黑 P红  ---> 兄弟黑，无子节点，父节点是红色  --->   U黑 P黑
        ///                 / \                                             /
        ///                B黑 D                                           B红
        ///            
        ///         1.1.3.2  父节点也是黑色
        ///              以父节点 为基准（新删除节点）向上递归调整
        ///                --->兄弟节点变红，黑高失衡， 再把P父亲节点当作新的被删除节点，这就是上一种1.1.3.1中父节点是红色的情况，直至遇到红色父节点或遇到根节点，平衡结束
        ///                G黑         
        ///                / \     
        ///               U黑 P黑  
        ///                   / \  
        ///                  B黑 D 
        ///     1.2 兄弟(左侧)节点是红色
        ///         删除右子节点需要右旋，兄弟和右侄NR颜色互换
        /// 
        ///        P黑                P黑                     B黑
        ///       / \                / \                     / \
        ///      B红 D   --->着色    B黑 D    --右旋-->     NL黑 P黑   
        ///      / \                / \                        /
        ///    NL黑 NR黑          NL黑 NR红                   NR红
        /// 
        /// 
        /// 
        /// 
        /// </summary>
        /// <param name="deleteNode"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void DeleteFixup(RBTreeNode<T> deleteNode)
        {
            if (Root == deleteNode)
            {
                Root.IsRed = false;
                return;
            }
            var parent = deleteNode.Parent;
            var brother = IsLeft(deleteNode) ? parent.Right : parent.Left;


            // 兄弟黑
            if (!brother.IsRed)
            {
                var rotateType = GetDeleteRBRotateType(brother);

                switch (rotateType)
                {
                    case RBRotateType.LL:
                        brother.IsRed = parent.IsRed; // 兄弟节点取代父节点颜色
                        // 父节点和侄子节点染黑
                        parent.IsRed = false;
                        brother.Left.IsRed = false;
                        // 右旋
                        RightRotate(parent);
                        break;
                    case RBRotateType.RR:
                        // 兄弟节点取代父节点颜色
                        brother.IsRed = parent.IsRed;
                        parent.IsRed = false;
                        brother.Right.IsRed = false;
                        LeftRotate(parent);
                        break;
                    case RBRotateType.LR:
                        // NR 染父色
                        brother.Right.IsRed = parent.IsRed;
                        // 父变黑
                        parent.IsRed = false;
                        // 以兄弟节点左旋
                        LeftRotate(brother);
                        // 以父节点右旋
                        RightRotate(parent);

                        break;
                    case RBRotateType.RL:
                        brother.Left.IsRed = parent.IsRed;
                        // 父变黑
                        parent.IsRed = false;
                        // 以兄弟节点右旋
                        RightRotate(brother);
                        // 以父节点左旋
                        LeftRotate(parent);
                        break;
                    default:
                        // 父节点是红色，直接交换颜色
                        if (parent.IsRed)
                        {
                            parent.IsRed = false;
                            brother.IsRed = true;
                        }
                        else
                        {
                            // 父节点是黑色，想上递归处理
                            brother.IsRed = true;
                            DeleteFixup(parent);
                        }
                        // 父节点是黑色，递归处理
                        break;
                }

            }
            // 兄弟红色
            else
            {
                // 删除的是左子树，兄弟变黑，随父侄，黑变红
                if (IsLeft(deleteNode))
                {
                    brother.IsRed = false;
                    brother.Left.IsRed = true;
                    // 以parent 左旋
                    LeftRotate(deleteNode.Parent);
                }
                else
                {
                    brother.IsRed = false;
                    brother.Right.IsRed = true;
                    RightRotate(deleteNode.Parent);
                }
            }
        }



        /// <summary>
        /// 获得删除节点的替代节点，该方法目的是减少调整次数
        /// 1. 先找前驱，如果找到以下两种情况则返回前驱
        ///    1.1 红色叶子节点 
        ///    1.2 有一个红色叶子节点的黑节点
        /// 2. 找不到则返回后继
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private RBTreeNode<T> GetReplaceNode(RBTreeNode<T> node)
        {
            var preNode = GetPreNode(node);
            if (preNode.IsRed)
            {
                return preNode;
            }
            else if (preNode.Left != null)
            {
                return preNode;
            }
            return GetSubNode(node);
        }

        /// <summary>
        ///  获取前驱节点 比前节点值小的最大值
        ///  即获取左子树中的最大值，沿着左子树一直向右 到尽头
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private RBTreeNode<T> GetPreNode(RBTreeNode<T> node)
        {
            var temp = node.Left;
            while (temp != null)
            {
                temp = temp.Right;
            }
            return temp;
        }

        /// <summary>
        /// 获得后继节点
        /// 从右子树中 取得最小值
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public RBTreeNode<T> GetSubNode(RBTreeNode<T> node)
        {
            var temp = node.Right;
            while (temp != null)
            {
                temp = temp.Left;
            }
            return temp;
        }

        #endregion

        #region 旋转辅助方法
        /// <summary>
        /// 左旋操作 
        /// 1. oldNode 右子树  = x 左子树
        /// 2. x 左子树 = node  
        ///    
        /// 
        ///   oldNode                    x
        ///    / \      左旋            /   \
        ///  T1  x     ------>       oldNode T3
        ///     / \                  / \
        ///    T2 T3                T1 T2
        /// </summary>
        /// <param name="node"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public void LeftRotate(RBTreeNode<T> oldNode)
        {

            var parent = oldNode.Parent;
            var x = oldNode.Right;

            #region  处理顶点的父节点
            x.Parent = parent;
            if (parent != null)
            {
                // 如果老顶点是左子树
                if (IsLeft(oldNode))
                {
                    parent.Left = x;
                }
                else
                {
                    parent.Right = x;
                }
            }
            else
            {
                Root = x;
            }
            #endregion

            // 左旋转
            oldNode.Right = x.Left;
            if (x.Left != null)
            {
                // 新顶点左子节点的父节点指向老顶点
                x.Left.Parent = oldNode;
            }
            x.Left = oldNode;
            // 老顶点的父节点指向新顶点
            oldNode.Parent = x;

        }

        /// <summary>
        /// 右旋转操作
        /// 1. oldNode 右子树  = x 左子树
        /// 2. x 左子树 = node  
        ///    
        /// 
        ///                  oldNode            x 
        ///    右旋            /   \            / \    
        ///   ------>        x     T3         T1  oldNode    
        ///                 / \                   / \   
        ///                T1 T2                 T2 T3  
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public void RightRotate(RBTreeNode<T> oldNode)
        {
            var parent = oldNode.Parent;
            var x = oldNode.Left;

            #region  处理顶点的父节点
            x.Parent = parent;
            if (parent != null)
            {
                // 如果老顶点是左子树
                if (IsLeft(oldNode))
                {
                    parent.Left = x;
                }
                else
                {
                    parent.Right = x;
                }
            }
            else
            {
                Root = x;
            }
            #endregion

            // 右旋转 新顶点的右子树 变成老顶点的左子树
            oldNode.Left = x.Right;
            if (x.Right != null)
            {
                // 新顶点右子节点的父节点指向老顶点
                x.Right.Parent = oldNode;
            }
            x.Right = oldNode;
            // 老顶点的父节点指向新顶点
            oldNode.Parent = x;
        }



        /// <summary>
        /// 节点是否为左子树
        /// </summary>
        /// <param name="brotherNode"></param>
        /// <returns></returns>
        public bool IsLeft(RBTreeNode<T> node)
        {
            var parent = node.Parent;
            return parent != null && parent.Left == node;
        }

        public RBRotateType GetRBRotateType(RBTreeNode<T> node)
        {
            var parent = node.Parent;

            if (node == null || parent == null)
            {
                return RBRotateType.None;
            }
            // 本节点和父节点都是红色
            if (node.IsRed && parent.IsRed)
            {
                // 父节点是左子树
                if (IsLeft(parent))
                {
                    if (IsLeft(node))
                    {
                        return RBRotateType.LL;
                    }
                    else
                    {
                        return RBRotateType.LR;
                    }
                }
                else
                {
                    if (IsLeft(node))
                    {
                        return RBRotateType.RL;
                    }
                    else
                    {
                        return RBRotateType.RR;
                    }
                }
            }

            return RBRotateType.None;
        }

        /// <summary>
        /// 删除节点时由于兄弟时黑色节点，根据兄弟节点获取旋转类型
        /// </summary>
        /// <param name="brotherNode"></param>
        /// <returns></returns>
        public RBRotateType GetDeleteRBRotateType(RBTreeNode<T> brotherNode)
        {
            if (IsLeft(brotherNode))
            {
                if (brotherNode.Left != null && brotherNode.Left.IsRed)
                    return RBRotateType.LL;
                if (brotherNode.Right != null && brotherNode.Right.IsRed)
                    return RBRotateType.LR;
            }


            else
            {
                if (brotherNode.Right != null && brotherNode.Right.IsRed)
                    return RBRotateType.RR;
                if (brotherNode.Left != null && brotherNode.Left.IsRed)
                    return RBRotateType.RL;
            }

            return RBRotateType.None;
        }

        // 重置Rank
        private void FixValueRank()
        {
            var root = Root;

            Stack<RBTreeNode<T>> stack = new Stack<RBTreeNode<T>>();
            var rank = 1;
            while (root != null || stack.Count > 0)
            {
                if (root != null)
                {
                    stack.Push(root);
                    root = root.Left;
                }
                else
                {
                    root = stack.Pop();
                    root.Value.Rank = rank;
                    rank++;
                    root = root.Right;
                }
            }

        }
        #endregion


        public void Print()
        {
            InOrderDisplayNode(Root);
        }

        /// <summary>
        /// 前序遍历输出
        /// </summary>
        /// <param name="node"></param>
        private void InOrderDisplayNode(RBTreeNode<T> node)
        {
            if (node == null)
            {
                return;
            }

            InOrderDisplayNode(node.Left);
            Console.Write($"{node}--->");
            InOrderDisplayNode(node.Right);
            Console.WriteLine();
        }

        private void TraversalNode(RBTreeNode<T> node, Action<T> func)
        {
            if (node == null)
            {
                return;
            }
            TraversalNode(node.Left, func);
            func(node.Value);
            TraversalNode(node.Right, func);
        }

        /// <summary>
        /// 当前节点的范围遍历
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public IEnumerable<T> GetRangeValues(int start, int end)
        {
            var ret = new List<T>();
            Stack<RBTreeNode<T>> stack = new Stack<RBTreeNode<T>>();
            var root = Root;
            var _end = end;
            while ((root != null || stack.Count > 0) && _end >= 0)
            {
                if (root != null)
                {
                    stack.Push(root);
                    root = root.Left;
                }
                else
                {
                    root = stack.Pop();
                    if (root.Value.Rank >= start && root.Value.Rank <= end)
                    {
                        ret.Add(root.Value);
                    }
                    _end--;

                    root = root.Right;
                }
            }
            return ret.ToList();
        }

        internal RBTreeNode<T> FindStartNodeByRank(int startRank)
        {
            var currentNode = FindNodeByExpression(x => x.Rank == startRank);

            return currentNode;
        }

        public void InOrderPrint()
        {
            InOrderTraversal(Root);
        }

        private void InOrderTraversal(RBTreeNode<T> node)
        {
            if (node != null)
            {
                InOrderTraversal(node.Left);
                Console.WriteLine(node.Value);
                InOrderTraversal(node.Right);
            }
        }
    }
}

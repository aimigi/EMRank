namespace CustomerRankAPI.RBTree
{
    public class Tree<T> where T : IComparable<T>
    {
        private RBTreeNode<T> _root;
        public long Total { get; private set; }

        public Tree()
        {
            _root = null;
        }

        public void Insert(T value, Action<RBTreeNode<T>, string> action)
        {
            var newNode = new RBTreeNode<T>(value);
            var isleftInsert = false;
            if (_root == null)
            {
                _root = newNode;
                _root.IsRed = false;
                action(newNode, "root");
                Total++;
            }
            else
            {
                var node = _root;
                RBTreeNode<T> parent = null;

                while (node != null)
                {
                    parent = node;
                    int compare = value.CompareTo(node.Value);
                    if (compare < 0)
                    {
                        node = node.Left;                        
                    }
                    else if (compare > 0)
                    {
                        node = node.Right;                        
                    }
                    else
                    {
                        // do nothing.
                        return;
                    }
                }
                if (value.CompareTo(parent.Value) < 0)
                {
                    parent.Left = newNode;
                    // left insert
                    isleftInsert = true;

                }
                else
                {
                    isleftInsert = false;
                    parent.Right = newNode;
                   
                }
                newNode.Parent = parent;
                // Insert fixup
                InsertFixup(newNode);
                Total++;
                action(newNode, isleftInsert ? "left" : "right");
            }
        }

        private void InsertFixup(RBTreeNode<T> node)
        {
            while (node != _root && node.Parent.IsRed)
            {
                // father node is grandfather node's left child node
                if (node.Parent == node.Parent.Parent.Left)
                {
                    var uncle = node.Parent.Parent.Right;
                    // 1. uncle node is red
                    if (uncle != null && uncle.IsRed)
                    {
                        node.Parent.IsRed = false;
                        uncle.IsRed = false;
                        node.Parent.Parent.IsRed = true;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        // 2. node is parent node's right child node  --> rotate left node
                        if (node == node.Parent.Right)
                        {
                            node = node.Parent;
                            RotateLeft(node);
                        }

                        // 3. node is parent node's left child node --> rotate right node
                        node.Parent.IsRed = false;
                        node.Parent.Parent.IsRed = true;
                        RotateRight(node.Parent.Parent);
                    }
                }
                //  father node is the grandfather 's right child node,
                else
                {
                    var uncle = node.Parent.Parent.Left;
                    if (uncle != null && uncle.IsRed)
                    {
                        node.Parent.IsRed = false;
                        node.IsRed = false;
                        node.Parent.Parent.IsRed = true;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        if (node == node.Parent.Left)
                        {
                            node = node.Parent;
                            RotateRight(node);
                        }
                        node.Parent.IsRed = false;
                        node.Parent.Parent.IsRed = true;
                        RotateLeft(node.Parent.Parent);
                    }
                }
            }
            _root.IsRed = false;
        }

        public RBTreeNode<T> Find(T value)
        {
            var node = _root;
            while (node != null)
            {
                int compare = value.CompareTo(node.Value);
                if (compare < 0)
                {
                    node = node.Left;
                }
                else if (compare > 0)
                {
                    node = node.Right;
                }
                else
                {
                    return node;
                }
            }
            return null;
        }


        public void Remove(T value)
        {
            var node = Find(value);
            if (node != null)
            {

            }
        }

        // left rotate
        private void RotateLeft(RBTreeNode<T> node)
        {
            var right = node.Right;
            node.Right = right.Left;
            if (right.Left != null)
            {
                right.Left.Parent = node;

            }
            right.Parent = node.Parent;
            if (node.Parent == null)
            {
                _root = right;
            }
            else if (node == node.Parent.Left)
            {
                node.Parent.Left = right;
            }
            else
            {
                node.Parent.Right = right;
            }
            right.Left = node;
            node.Parent = right;
        }

        private void RotateRight(RBTreeNode<T> node)
        {
            var left = node.Left;
            node.Left = left.Right;
            if (left.Right != null)
            {
                left.Right.Parent = node;
            }
            left.Parent = node.Parent;
            if (node.Parent == null)
            {
                _root = left;
            }
            else if (node == node.Parent.Right)
            {
                node.Parent.Right = left;
            }
            else
            {
                node.Parent.Left = left;
            }
            left.Right = node;
            node.Parent = left;
            node.Parent = left;
        }

        public void InOrderPrint()
        {
            InOrderTraversal(_root);
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

        public void PrintNode()
        {
            if (_root == null)
            {
                Console.WriteLine("Tree is empty");
            }

            Queue<RBTreeNode<T>> queue = new Queue<RBTreeNode<T>>();
            queue.Enqueue(_root);

            while (queue.Count > 0)
            {
                var levelSize = queue.Count;
                while (levelSize > 0)
                {
                    var node = queue.Dequeue();
                    Console.Write($"{node.Value}({(node.IsRed ? "R" : "B")}) ");
                    if (node.Left != null)
                    {
                        queue.Enqueue(node.Left);
                    }
                    if (node.Right != null)
                    {
                        queue.Enqueue(node.Right);
                    }

                    levelSize--;
                }
                Console.WriteLine();
            }
        }
    }
}

namespace CustomerRankAPI
{
    public class SkipList<T>
    {
        // 最大层数
        private int maxLevel;
        // 当前层次
        private int level;
        // 起始节点
        private SkipListNode<T> header;
        // 当前链层的几率
        private float probability;
        // 跳表末尾的特殊值
        private const int NIL = Int32.MaxValue;
        private const float PROB = 0.5f;





    }
}

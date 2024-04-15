namespace CustomerRankAPI
{
    public class SkipListNode<T> where T : IRankIndexNode
    {
        public T Value { get; set; }
        public int Index { get; set; } = 0;
        /// <summary>
        /// store the next node reference
        /// </summary>
        public SkipListNode<T>[] Forward { get; set; }
        public SkipListNode(T value, int level)
        {
            Value = value;
            Forward = new SkipListNode<T>[level + 1];
        }
    }
}

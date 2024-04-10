namespace CustomerRankAPI
{
    public class SkipListNode<T>
    {
        private T _value;
        public int _key;
        private SkipListNode<T>[] link;
        public SkipListNode(int level, int key, T value)
        {
            _value = value;
            _key = key;
            link = new SkipListNode<T>[level];
        }
    }
}

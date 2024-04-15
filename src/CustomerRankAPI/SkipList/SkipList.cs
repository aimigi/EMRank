namespace CustomerRankAPI
{
    public class SkipList<T> where T : IComparable<T>, IRankIndexNode
    {
        private readonly SkipListNode<T> head;
        private readonly int MaxLevel;
        // random generate level
        private readonly float Probability;
        // store the skip current level count
        private int level;
        // total length
        public int Length { get; private set; }
        public SkipList(int maxLevel = 16, float probobility = 0.5f)
        {
            MaxLevel = maxLevel;
            Probability = probobility;
            head = new SkipListNode<T>(default(T), MaxLevel);
            level = 0;

        }

        private int RandomLevel()
        {
            int lvl = 0;
            while (level < MaxLevel && (new Random().NextDouble() < Probability))
            {
                lvl++;
            }
            return lvl;
        }

        /// <summary>
        /// insert node
        /// </summary>
        /// <param name="value"></param>
        public void Insert(T value)
        {

            var update = new SkipListNode<T>[MaxLevel + 1];

            var current = head;

            for (int i = level; i >= 0; i--)
            {
                while (current.Forward[i] != null && current.Forward[i].Value.CompareTo(value) < 0)
                {
                    current = current.Forward[i];
                }
                update[i] = current;
            }

            // 
            current = current.Forward[0];

            if (current == null || current.Value?.CompareTo(value) != 0)
            {
                // 0.5 机率 增加索引层级
                int lvl = RandomLevel();
                if (lvl > level)
                {
                    for (int i = level + 1; i <= lvl; i++)
                    {
                        update[i] = head;
                    }
                    level = lvl;
                }

                var newNode = new SkipListNode<T>(value, lvl);
                for (int i = 0; i <= lvl; i++)
                {
                    newNode.Forward[i] = update[i].Forward[i];
                    update[i].Forward[i] = newNode;
                }
                Length++;
                FixValueRank();

            }
        }
        // 重置Rank
        private void FixValueRank()
        {
            var current=head.Forward[0];
            var rank = 1;
            while(current != null)
            {
                current.Value.Rank=rank;
                rank++;
                current = current.Forward[0];
            }

        }
                
        public bool Delete(T value)
        {
            var update = new SkipListNode<T>[MaxLevel + 1];
            var current = head;

            // 1.from the top level ,find every level pre node
            for (int i = level; i >= 0; i--)
            {
                while (current.Forward[i] != null && current.Forward[i].Value.CompareTo(value) < 0)
                {
                    current = current.Forward[i];
                }
                update[i] = current;
            }
            // 2. mode to current node
            current = current.Forward[0];

            if (current != null && current.Value.CompareTo(value) == 0)
            {
                Length--;
                
                // 3.1 loop delete level reference
                for (int i = 0; i <= level; i++)
                {
                    if (update[i].Forward[i] != current)
                    {
                        break;
                    }
                    update[i].Forward[i] = current.Forward[i];
                }

                // 3.2 delete no data node

                while (level > 0 && head.Forward[level] == null)
                {
                    level--;
                }
                FixValueRank();
                // 
                return true;
            }
            return true;
        }

        /// <summary>
        /// O(Log(n)) Find
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public SkipListNode<T> Search(T value)
        {
            var current = head;
            for (int i = level; i >= 0; i--)
            {
                while (current.Forward[i] != null && current.Forward[i].Value.CompareTo(value) < 0)
                {
                    current = current.Forward[i];
                }
            }

            current = current.Forward[0];
            if (current != null && current.Value.CompareTo(value) == 0)
            {
                return current;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// O(n) find
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public SkipListNode<T> FindNodeByExpression(Func<T, bool> func)
        {
            var current = head.Forward[0];
            while (current != null)
            {
                if (func(current.Value))
                {
                    return current;
                }
                current = current.Forward[0];
            }
            return null;
        }

        public void Print()
        {
            var current = head.Forward[0];
            while (current != null)
            {
                Console.WriteLine(current.Value);
                current = current.Forward[0];
            }
        }

        public List<T> GetRangeValues(int start, int end)
        {
            var ret = new List<T>();
            var index = 1;
            var current = head.Forward[0];
            while (current != null)
            {
                current = current.Forward[0];
                index++;
                if (index >= start && index <= end)
                {
                    T t = current.Value;
                    ret.Add(t);
                }
            }
            return ret;
        }
    }
}

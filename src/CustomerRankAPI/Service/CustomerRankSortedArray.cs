namespace CustomerRankAPI.Service
{
    public class CustomerRankSortedArray : IRankService
    {
        private readonly object _lock = new object();
        // data repo
        public (long Customerid, int Score)[] SortedArray { get; private set; }

        // array capacity
        private long _capacity = 10;
        // array size
        private long _length = 0;
        public long Length { get { return _length; } }

        public CustomerRankSortedArray()
        {
            SortedArray = new (long Customerid, int Score)[_capacity];
        }

        public IEnumerable<CustomerScoreRankModel> GetCustomerNearRank(long customerid, int high, int low)
        {
            var result = new List<CustomerScoreRankModel>();
            var index = FindIndex(customerid);
            if (index < 0)
            {
                throw new ArgumentException("can not find customerid");
            }
            long start = index - high > 0 ? index - high : 0;
            long end = index + low > _length ? _length : index + low;
            for (long i = start; i <= end; i++)
            {
                result.Add(new CustomerScoreRankModel()
                {
                    Customerid = SortedArray[i].Customerid,
                    Rank = i + 1,
                    Score = SortedArray[i].Score,
                });
            }
            return result;
        }

        private long FindIndex(long customerid)
        {
            for (var i = 0; i < _length; i++)
            {
                if (SortedArray[i].Customerid == customerid)
                {
                    return i;
                }
            }
            return -1;
        }

        // O(1)
        public IEnumerable<CustomerScoreRankModel> GetCustomersByRank(int start, int end)
        {
            var result = new List<CustomerScoreRankModel>();

            if (start > end)
            {
                throw new ArgumentException("start cannot greater than end.");
            }
            if (end > _length)
            {
                end = (int)_length;
            }
            for (long index = start - 1; index < end; index++)
            {
                result.Add(new CustomerScoreRankModel()
                {
                    Customerid = SortedArray[index].Customerid,
                    Rank = index + 1,
                    Score = SortedArray[index].Score,
                });
            }
            return result;
        }

        public bool UpdateCustomerScore(long customerid, int score)
        {
            lock (_lock)
            {
                var index = FindIndex(customerid);
                if (index >= 0)
                {
                    var oldScore = SortedArray[index].Score;
                    var newScore = oldScore + score;
                    SortedArray[index] = (customerid, newScore);
                    if (score > 0)
                    {
                        QuickInsertLeft(index);
                    }
                    else
                    {
                        QuickInsertRight(index);
                    }
                }
                else
                {
                    if (_length + 1 == _capacity)
                    {
                        Resize();
                    }
                    QuickInsert((customerid, score));
                }
                return true;
            }
        }

        private void QuickInsertLeft(long index)
        {
            for (long i = index; i > 0; i--)
            {
                if (SortedArray[index].Score < SortedArray[i - 1].Score)
                {
                    break;
                }
                else if (SortedArray[index].Score == SortedArray[i].Score)
                {
                    if (SortedArray[index].Customerid > SortedArray[i].Customerid)
                    {
                        break;
                    }
                }
                else
                {
                    SortedArray[i] = SortedArray[i - 1];
                }
            }
        }
        // O(n)
        private void QuickInsertRight(long index)
        {
            for (long i = index + 1; i < _length; i++)
            {
                if (SortedArray[index].Score > SortedArray[i].Score)
                {
                    break;
                }
                else if (SortedArray[index].Score == SortedArray[i].Score)
                {
                    if (SortedArray[index].Customerid < SortedArray[i].Customerid)
                    {
                        break;
                    }
                }
                else
                {
                    SortedArray[i] = SortedArray[i + 1];
                }
            }
        }

        //O(n)
        private void QuickInsert((long customerid, int score) item)
        {
            long index = 0;
            for (long i = 0; i <= _length; i++)
            {
                if (item.score > SortedArray[i].Score)
                {
                    index = i;
                    break;
                }
                else if (item.score == SortedArray[i].Score)
                {
                    if (item.customerid < SortedArray[i].Customerid)
                    {
                        index = i;
                        break;
                    }
                }
                index = i;
            }
            for (long j = _length; j > index; j--)
            {
                SortedArray[j] = SortedArray[j - 1];
            }
            SortedArray[index] = item;
            _length++;
        }

        private void Resize()
        {
            var newArray = new (long Customerid, int Score)[(long)(_capacity * 1.5)];

            for (long i = 0; i < _length; i++)
            {
                newArray[i] = SortedArray[i];
            }
            SortedArray = newArray;
        }
    }
}

namespace CustomerRankAPI.Service
{
    public class CustomerRankSortedArray : IRankService
    {
        private readonly object _lock = new object();
        private volatile int _isSafe = 1;
        // data repo
        public (long Customerid, int Score)[] SortedArray { get; private set; }

        // array capacity
        private long _capacity = 10;
        // array size
        private long _length = 0;
        public long Length { get { return _length; } }

        public Dictionary<long, long> CustomerRank => throw new NotImplementedException();

        private Dictionary<long, int> _customerRank;

        public CustomerRankSortedArray()
        {
            SortedArray = new (long Customerid, int Score)[_capacity];
            _customerRank = new Dictionary<long, int>();
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
            if (start > _length)
            {
                throw new ArgumentException("start over limit.");
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

        // https://duongnt.com/interlocked-synchronization/
        // https://www.cnblogs.com/5iedu/p/4719625.html
        // https://learn.microsoft.com/zh-cn/dotnet/api/system.threading.interlocked.exchange?redirectedfrom=MSDN&view=net-7.0#System_Threading_Interlocked_Exchange_System_Int32__System_Int32_
        public bool UpdateCustomerScoreEx(long customerid, int score)
        {
            if (Interlocked.Exchange(ref _isSafe, 0) == 1)
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
                Interlocked.Exchange(ref _isSafe, 1);
                return true;
            }
            return false;
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
            _length++;
            for (long j = _length - 1; j > index; j--)
            {
                SortedArray[j] = SortedArray[j - 1];
            }
            SortedArray[index] = item;

        }

        private void Resize()
        {
            _capacity = (long)(_capacity * 1.5);
            var newArray = new (long Customerid, int Score)[_capacity];

            for (long i = 0; i < _length; i++)
            {
                newArray[i] = SortedArray[i];
            }
            SortedArray = newArray;
        }
    }
}

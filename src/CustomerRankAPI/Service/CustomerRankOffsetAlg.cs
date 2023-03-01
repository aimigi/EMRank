using static System.Formats.Asn1.AsnWriter;

namespace CustomerRankAPI.Service
{
    public class CustomerRankOffsetAlg : IRankService
    {
        private volatile int _isSafe = 1;
        private long _length;
        public long Length { get { return _length; } }

        private long _capacity;

        // data store
        public (long Customerid, int Score)[] SortedArray { get; private set; }
        // cached rank data
        private Dictionary<long, long> _customerRank;
        // just for test
        public Dictionary<long, long> CustomerRank { get { return _customerRank; } }

        public CustomerRankOffsetAlg()
        {
            _length = 0;
            _capacity = 10;
            SortedArray = new (long Customerid, int Score)[_capacity];
            _customerRank = new Dictionary<long, long>();
        }

        public IEnumerable<CustomerScoreRankModel> GetCustomerNearRank(long customerid, int high, int low)
        {
            var result = new List<CustomerScoreRankModel>();
            if (!_customerRank.TryGetValue(customerid, out long rank))
            {
                throw new ArgumentException("can not find customerid");
            }
            long index = rank - 1;

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
            // performce lock
            if (Interlocked.Exchange(ref _isSafe, 0) == 1)
            {
                if (_customerRank.TryGetValue(customerid, out long rank))
                {
                    var index = rank - 1;
                    // customer exists, find score range
                    var oldScore = SortedArray[index].Score;
                    var newScore = oldScore + score;
                    SortedArray[index] = (customerid, newScore);
                   
                    // remove customer
                    if (newScore == 0)
                    {
                        _customerRank.Remove(customerid);
                        ChangeLeftOffset(index);
                        _length--;
                    }
                    else
                    {
                        if (score > 0)
                        {
                            QuickChangeOffset(index, 0, index);
                        }
                        else
                        {
                            QuickChangeOffset(index, index, _length - 1);
                        }
                    }

                }
                else
                {
                    if (_length + 1 >= _capacity)
                    {
                        Resize();
                    }
                    InsertNewCustomer(customerid, score);
                }

                Interlocked.Exchange(ref _isSafe, 1);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ChangeLeftOffset(long index)
        {
            for (long i = index; i < _length; i++)
            {
                SortedArray[i - 1] = SortedArray[i];
            }
        }

        // Capacity increased by 1.5 times
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

        private void InsertNewCustomer(long customerid, int score)
        {
            if (_length == 0)
            {
                _customerRank.Add(customerid, 1);
                SortedArray[0] = (customerid, score);
                _length++;
                return;
            }
            // scan from high(score) to low, once find the index , break loop
            long index = _length;
            for (long i = 0; i < _length; i++)
            {
                if (score > SortedArray[i].Score)
                {
                    index = i;
                    break;
                }
                else if (score == SortedArray[i].Score && customerid < SortedArray[i].Customerid)
                {
                    index = i;
                    break;
                }
            }

            // find the insert position
            // move right after index
            for (long j = _length; j > index; j--)
            {
                SortedArray[j] = SortedArray[j - 1];
                // change rank value
                _customerRank[SortedArray[j].Customerid] = j + 1;
            }

            SortedArray[index] = (customerid, score);
            _customerRank.Add(customerid, index + 1);

            _length++;

        }

        private void QuickChangeOffset(long index, long start, long end)
        {
            if (start == end)
            {
                _customerRank[SortedArray[index].Customerid] = index + 1;
                return;
            }

            // scan from left to right, if found the score position break.
            long flag = end;
            var temp = SortedArray[index];
            for (long i = start; i < end; i++)
            {
                if (SortedArray[index].Score > SortedArray[i].Score)
                {
                    flag = i;
                    break;
                }
                else if (SortedArray[index].Score == SortedArray[i].Score && SortedArray[index].Customerid < SortedArray[i].Customerid)
                {
                    flag = i;
                    break;
                }
            }
            for (long j = end; j > flag; j--)
            {
                _customerRank[SortedArray[j].Customerid] = j + 1;
                SortedArray[j] = SortedArray[j - 1];
            }
            SortedArray[flag] = temp;
            
        }
    }
}

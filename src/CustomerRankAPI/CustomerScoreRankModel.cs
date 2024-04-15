namespace CustomerRankAPI
{
    public class CustomerScoreRankModel : IComparable<CustomerScoreRankModel>, IRankIndexNode
    {
        public long Customerid { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }

        public int CompareTo(CustomerScoreRankModel? other)
        {
            if (Score > other.Score) return -1;
            else if (Score < other.Score) return 1;
            else
            {
                if (Customerid > other.Customerid) return 1;
                else if (Customerid < other.Customerid) return -1;
                else return 0;
            }
        }

        public override string ToString()
        {
            return $"({Rank},{Customerid},{Score})";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public static bool operator ==(CustomerScoreRankModel left, CustomerScoreRankModel right)
        {
            if (ReferenceEquals(left, right)) return true;

            if (left is null)
            {
                return false;
            }
            return left.Customerid == right.Customerid && left.Score == right.Score;
        }

        public static bool operator !=(CustomerScoreRankModel left, CustomerScoreRankModel right)
        {
            return !(left == right);
        }

    }
}

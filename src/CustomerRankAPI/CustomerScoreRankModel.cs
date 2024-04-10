namespace CustomerRankAPI
{
    public class CustomerScoreRankModel : IComparable<CustomerScoreRankModel>
    {
        public long Customerid { get; set; }
        public int Score { get; set; }
        public long Rank { get; set; }

        public int CompareTo(CustomerScoreRankModel? other)
        {
            if (Score > other.Score) return 1;
            else if (Score < other.Score) return -1;
            else
            {
                if (Customerid > other.Customerid) return 1;
                else if (Customerid < other.Customerid) return -1;
                else return 0;
            }
        }

        public override string ToString()
        {
            return $"{Rank}-[{Customerid},{Score}]";
        }
    }
}

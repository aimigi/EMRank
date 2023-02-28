namespace CustomerRankAPI.Service
{
    public interface IRankService
    {
        long Length { get; }
        (long Customerid, int Score)[] SortedArray { get; }
        Dictionary<long, long> CustomerRank { get; }
        IEnumerable<CustomerScoreRankModel> GetCustomerNearRank(long customerid, int high, int low);
        IEnumerable<CustomerScoreRankModel> GetCustomersByRank(int start, int end);
        bool UpdateCustomerScore(long customerid, int score);
    }
}

namespace CustomerRankAPI.Service
{
    public interface IRankService
    {       
        IEnumerable<CustomerScoreRankModel> GetCustomerNearRank(long customerid, int high, int low);
        IEnumerable<CustomerScoreRankModel> GetCustomersByRank(int start, int end);
        bool UpdateCustomerScore(long customerid, int score);
    }
}

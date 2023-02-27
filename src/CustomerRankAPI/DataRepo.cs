using System.Collections.Concurrent;

namespace CustomerRankAPI
{

    public static class DataRepo
    {
        /// <summary>
        /// customer-score dictionary
        /// </summary>
        public static ConcurrentDictionary<long, int> CustomerSocre;

        public static CustomerScoreRankModel[] SortedModels;

        static DataRepo()
        {
            CustomerSocre = new ConcurrentDictionary<long, int>();
        }
        /// <summary>
        /// Ignore the customerid check, and add if there is no customerid
        /// </summary>
        /// <param name="id">customerid</param>
        /// <param name="score">score</param>
        public static bool UpdateCustomerScore(long id, int score)
        {
            if (CustomerSocre.TryGetValue(id, out int oldScore))
            {
                CustomerSocre[id] = oldScore + score;
                return true;
            }
            else
            {
                return CustomerSocre.TryAdd(id, score);
            }
        }


    }
}

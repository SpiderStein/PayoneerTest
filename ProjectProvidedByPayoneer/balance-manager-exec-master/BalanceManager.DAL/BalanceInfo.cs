using System;

namespace BalanceManager.DAL
{
    public class BalanceInfo
    {
        public long BalanceId { get; set; }
        public float Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}

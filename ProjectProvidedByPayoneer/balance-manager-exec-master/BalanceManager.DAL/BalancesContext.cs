using System;
using System.Collections.Generic;

namespace BalanceManager.DAL
{
    public class BalancesContext : IBalancesContext
    {
        #region Do not change

        public IList<BalanceInfo> Balances => _balances;
        private IList<BalanceInfo> _balances;

        public BalancesContext()
        {
            // Initial Data
            _balances = new List<BalanceInfo>
            {
                new BalanceInfo{BalanceId = 3,Amount = 40, CreateDate=new DateTime(1991,11,11), UpdateDate = new DateTime(2019,11,11) },
                new BalanceInfo{BalanceId = 4,Amount = 40, CreateDate=new DateTime(1987,9,23), UpdateDate = DateTime.Now },
                new BalanceInfo{BalanceId = 5,Amount = 40, CreateDate=new DateTime(2017,6,23), UpdateDate = new DateTime(2019,7,3) },
            };
        }

        #endregion
    }
}

using System.Collections.Generic;

namespace BalanceManager.DAL
{
    public interface IBalancesContext
    {
        IList<BalanceInfo> Balances { get; }
    }
}

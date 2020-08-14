using System.Threading.Tasks;
namespace BalanceManager.DAL
{
    internal interface IPayoneerDBDAL
    {
        /// <returns>Balance ID</returns>
        Task<long> CreateBalance();
        /// <returns>Boolean that indicates if there's an existing balance for the given balanceID, and if so, the balance itself.!--
        /// If it doesn't, then balance is null</returns>
        Task<(BalanceInfo, bool)> GetBalance(long ID);
        /// <returns>If there's no existing balance for the given ID, returns false</returns>
        Task<bool> Charge(long balanceID, float amount);
        /// <returns>If there's no existing balance for the given ID, returns false</returns>
        Task<bool> Load(long balanceID, float amount);
        Task Transfer(long senderBalanceID, long recipientBalanceID);
    }
}
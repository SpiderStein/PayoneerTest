using System.Threading.Tasks;
namespace BalanceManager.DAL
{
    public interface IPayoneerDBDAL
    {
        /// <returns>Balance ID</returns>
        Task<long> CreateBalance();
        /// <returns>Boolean that indicates if there's an existing balance for the given balanceID, and if so, the balance itself.
        /// If it doesn't, then balance is null and the boolean is false</returns>
        Task<(BalanceInfo, bool)> GetBalance(long ID);
        /// <returns>If there's no existing balance for the given ID, returns (false, false)
        /// If the balance doesn't have sufficient amount, returns (true, false)</returns>
        Task<(bool, bool)> Charge(long balanceID, float amount);
        /// <returns>If there's no existing balance for the given ID, returns (false, false)
        Task<bool> Load(long balanceID, float amount);
        /// <returns>If there's no existing balance for the one of the given IDs or for both, returns (false, false)
        /// If the sender's balance doesn't have sufficient amount, returns (true, false)</returns>
        Task<(bool, bool)> Transfer(long senderBalanceID, long recipientBalanceID, float amount);
    }
}
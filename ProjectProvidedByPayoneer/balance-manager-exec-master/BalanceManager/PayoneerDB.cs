using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using BalanceManager.DAL;
using System.Threading;

namespace BalanceManager
{
    // I'm aware that implementing the dal is incorrect. Doing so because creating
    // a thin DAL to this entity seems to be redundant and duck typing doesn't exist in the type system.
    internal class PayoneerDB : IPayoneerDBDAL
    {
        private long newestBalanceID; // Maybe it's better to type "BalanceInfo.ID" as ulong.
        private ConcurrentDictionary<long, BalanceInfo> balanceIDToBalance { get; set; }
        private object dicAccessSyncObj { get; set; }
        public PayoneerDB()
        {
            // this.newestBalanceID = long.MaxValue;                        PROVIDE the value behind in the iocC.
        }

        public Task<long> CreateBalance()
        {
            return Task.Run<long>(() =>
            {
                var IDCopyInThreadContext = Interlocked.Increment(ref this.newestBalanceID);
                this.balanceIDToBalance.TryAdd(IDCopyInThreadContext,
                new BalanceInfo
                {
                    BalanceId = IDCopyInThreadContext,
                    Amount = 0,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
                return IDCopyInThreadContext;
            });
        }

        public Task<(BalanceInfo, bool)> GetBalance(long ID)
        {
            return Task.Run<(BalanceInfo, bool)>(() =>
            {
                BalanceInfo balance;
                lock (this.dicAccessSyncObj)
                {
                    var doesBalanceExist = this.balanceIDToBalance.TryGetValue(ID, out balance);
                    if (doesBalanceExist)
                    {
                        return (balance, true);
                    }
                    else
                    {
                        return (null, false);
                    }
                }
            });
        }

        public Task<bool> Charge(long balanceID, float amount)
        {
            return Task.Run<bool>(() =>
            {
                BalanceInfo balance;
                lock (this.dicAccessSyncObj)
                {
                    var doesBalanceExist = this.balanceIDToBalance.TryGetValue(balanceID, out balance);
                    if (!doesBalanceExist)
                    {
                        return false;
                    }
                }
            });
        }

        public Task<bool> Load(long balanceID, float amount)
        {
            return Task.Run<bool>(() =>
           {
               BalanceInfo balance;
               lock (this.dicAccessSyncObj)
               {
                   var doesBalanceExist = this.balanceIDToBalance.TryGetValue(balanceID, out balance);
                   if (!doesBalanceExist)
                   {
                       return false;
                   }
                   while (!this.balanceIDToBalance.TryUpdate(balanceID, //  Arbitrarily chose to use optimistic concurrency.
                       new BalanceInfo
                       {
                           BalanceId = balance.BalanceId,
                           CreateDate = balance.CreateDate,
                           UpdateDate = balance.UpdateDate,
                           Amount = balance.Amount + amount
                       }, balance)) { }

                   return true;
               }
           });
        }

        public Task Transfer(long senderBalanceID, long recipientBalanceID)
        {
            throw new System.NotImplementedException();
        }
    }
}
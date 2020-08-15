using System.Linq;
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

        public PayoneerDB(
            long IncrementalIDBase,
            ConcurrentDictionary<long, BalanceInfo> balanceIDToBalance,
            object dicAccessSyncObj)
        {
            this.newestBalanceID = IncrementalIDBase - 1;
            this.balanceIDToBalance = balanceIDToBalance;
            this.dicAccessSyncObj = dicAccessSyncObj;
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

        public Task<(bool, bool)> Charge(long balanceID, float amount)
        {
            return Task.Run<(bool, bool)>(() =>
            {
                BalanceInfo balance;
                lock (this.dicAccessSyncObj)
                {
                    var doesBalanceExist = this.balanceIDToBalance.TryGetValue(balanceID, out balance);
                    if (!doesBalanceExist)
                    {
                        return (false, false);
                    }
                    if (balance.Amount < amount)
                    {
                        return (true, false);
                    }
                    //  Arbitrarily chose to use optimistic concurrency.
                    //  According to the msft docs, even if I lock, I can't be sure that the attempt to
                    //  update will be successful.
                    while (!this.balanceIDToBalance.TryUpdate(balanceID, //  Arbitrarily chose to use optimistic concurrency.
                       new BalanceInfo
                       {
                           BalanceId = balance.BalanceId,
                           CreateDate = balance.CreateDate,
                           UpdateDate = balance.UpdateDate,
                           Amount = balance.Amount - amount
                       }, balance))
                    { }

                    return (true, true);
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
                   //  Arbitrarily chose to use optimistic concurrency.
                   //  According to the msft docs, even if I lock, I can't be sure that the attempt to
                   //  update will be successful.
                   while (!this.balanceIDToBalance.TryUpdate(balanceID,
                       new BalanceInfo
                       {
                           BalanceId = balance.BalanceId,
                           CreateDate = balance.CreateDate,
                           UpdateDate = balance.UpdateDate,
                           Amount = balance.Amount + amount
                       }, balance))
                   { }

                   return true;
               }
           });
        }

        public Task<(bool, bool)> Transfer(long senderBalanceID, long recipientBalanceID, float amount)
        {
            return Task.Run<(bool, bool)>(() =>
            {
                BalanceInfo senderBalance, recipientBalance;
                lock (this.dicAccessSyncObj)
                {
                    var doesBalanceExist = this.balanceIDToBalance.TryGetValue(senderBalanceID, out senderBalance);
                    if (!doesBalanceExist)
                    {
                        return (false, false);
                    }
                    doesBalanceExist = this.balanceIDToBalance.TryGetValue(recipientBalanceID, out recipientBalance);
                    if (!doesBalanceExist)
                    {
                        return (false, false);
                    }
                    while (!this.balanceIDToBalance.TryUpdate(senderBalanceID,
                       new BalanceInfo
                       {
                           BalanceId = senderBalance.BalanceId,
                           CreateDate = senderBalance.CreateDate,
                           UpdateDate = senderBalance.UpdateDate,
                           Amount = senderBalance.Amount - amount
                       }, senderBalance))
                    { }
                    while (!this.balanceIDToBalance.TryUpdate(recipientBalanceID,
                       new BalanceInfo
                       {
                           BalanceId = recipientBalance.BalanceId,
                           CreateDate = recipientBalance.CreateDate,
                           UpdateDate = recipientBalance.UpdateDate,
                           Amount = recipientBalance.Amount + amount
                       }, recipientBalance))
                    { }

                    return (true, true);
                }
            });
        }
    }
}
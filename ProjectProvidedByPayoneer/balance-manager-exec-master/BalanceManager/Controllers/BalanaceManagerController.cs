using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BalanceManager.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BalanceManager
{
    [ApiController]
    [Route("api/balances/v1/[controller]")]
    public class BalanaceManagerController : ControllerBase
    {
        private IPayoneerDBDAL payoneerDBDAL { get; set; }
        public BalanaceManagerController(IPayoneerDBDAL payoneerDBDAL)
        {
            this.payoneerDBDAL = payoneerDBDAL;
            System.Console.WriteLine("asdasdasd");
        }


        [HttpGet("CreateBalance")]
        public async Task<ActionResult<long>> CreateBalance()
        {
            var balanceID = await this.payoneerDBDAL.CreateBalance().ConfigureAwait(false);
            return Ok(balanceID);
        }

        [HttpGet("GetBalanceInfo/{id}")]
        public async Task<ActionResult<BalanceInfo>> GetBalanceInfo(long id)
        {
            var (balance, doesBalanceExist) = await this.payoneerDBDAL.GetBalance(id).ConfigureAwait(false);
            if (!doesBalanceExist)
            {
                return NotFound();
            }
            return Ok(balance);
        }


    }
}

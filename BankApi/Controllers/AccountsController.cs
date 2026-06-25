using BankApi.Data;
using BankApi.Data.Enums;
using BankApi.Data.Models;
using BankApi.Data.Models.DTOs.RequestDTOs;
using BankApi.Data.Models.DTOs.ResponseDTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountServices _accountServices;

        public AccountsController(IAccountServices accountService)
        {
            _accountServices = accountService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAll()
        {
            var result = await _accountServices.GetAllAccountsAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponseDto>> GetById(int id)
        {
            var result = await _accountServices.GetAccountByIdAsync(id);

            if (result == null) return NotFound("Account not found");

            return Ok(result);
        }

        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetAccountHistory(int id)
        {
            var accountExists = await _accountServices.GetAccountByIdAsync(id);

            if (accountExists == null) return NotFound("Account not found");

            var result = await _accountServices.GetAccountHistoryAsync(id);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{accountId}/deposit")]
        public async Task<IActionResult> Deposit(int accountId, [FromBody] decimal amount)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
            {
                return Unauthorized("Sorry, you must login first.");
            }

            int currentCustomerId = int.Parse(userIdString);

            var account = await _accountServices.GetAccountByIdAsync(accountId);

            if (account == null)
            {
                return NotFound("Account is not exist.");
            }
            
            if (account.CustomerId != currentCustomerId)
            {
                return StatusCode(403, "You're not authorized to use this account");
            }

            var result = await _accountServices.DepositAsync(accountId, amount);

            if (!result) return BadRequest("Deposit failed.");

            return Ok(new { message = "Deposit successful" });
        }

        [Authorize]
        [HttpPost("{accountId}/withdraw")]
        public async Task<IActionResult> Withdraw(int accountId, [FromBody] decimal amount)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
            {
                return Unauthorized("Sorry, you must login first.");
            }

            int currentCustomerId = int.Parse(userIdString);

            var account = await _accountServices.GetAccountByIdAsync(accountId);

            if (account == null)
            {
                return NotFound("Account is not exist.");
            }

            if (account.CustomerId != currentCustomerId)
            {
                return StatusCode(403, "You're not authorized to use this account");
            }

            var result = await _accountServices.WithdrawAsync(accountId, amount);

            if (!result) return BadRequest("Withdrawal failed, please check your account balance or withdrawal limit");
            
            return Ok(new { message = "Withdrawal successful" });
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _accountServices.DeleteAccountAsync(id);

            if (!result) return BadRequest("Account deletion failed");

            return Ok(new { message = "Account deleted successfully" });
        }

        [HttpPost("saving")]
        public async Task<ActionResult<AccountResponseDto>> CreateSavingAccount([FromBody] SavingAccountDto dto)
        {
            var result = await _accountServices.CreateSavingAccountAsync(dto);
            if (result == null) return BadRequest("Failed to create saving account");

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("current")]
        public async Task<ActionResult<AccountResponseDto>> CreateCurrentAccount([FromBody] CurrentAccountDto dto)
        {
            var result = await _accountServices.CreateCurrentAccountAsync(dto);
            if (result == null) return BadRequest("Failed to create current account");

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("{id}/update-limit")]
        public async Task<IActionResult> UpdateWithdrawalLimit(int id, [FromBody] UpdateWithdrawalLimitDto dto)
        {
            var result = await _accountServices.UpdateAccountAsync(id, dto.NewWithdrawalLimit);
            if (!result) return BadRequest("Failed to update limit. Account might not exist or it is a Saving Account.");
            
            return Ok(new { message = "Withdrawal limit updated successfully." });
        }




        // _____________________________________________________________________________________________________
        
        
     
        //[HttpPost("saving")]
        //public async Task<ActionResult<AccountResponseDto>> CreateSavingAccount(SavingAccountDto dto)
        //{
        //    var customer = await _context.Customers.FindAsync(dto.CustomerId);
        //    if (customer == null)
        //    {
        //        return BadRequest(new { message = "Customer does not exist. " });
        //    }
        //    string generatedAccountNumber = $"SAV-" + new Random().Next(10000000, 99999999).ToString();

        //    while (await _context.Accounts.AnyAsync(a => a.AccountNumber == generatedAccountNumber))
        //    {
        //        generatedAccountNumber = $"SAV-" + new Random().Next(10000000, 99999999).ToString();
        //    }

        //    var savingAccount = new SavingAccount
        //    {
        //        AccountNumber = generatedAccountNumber,
        //        Balance = dto.Balance,
        //        CustomerId = dto.CustomerId,
        //        InterestRate = dto.InterestRate
        //    };
        //    _context.Accounts.Add(savingAccount);
        //    await _context.SaveChangesAsync();

        //    var response = new AccountResponseDto
        //    {
        //        Id = savingAccount.Id,
        //        AccountNumber = generatedAccountNumber,
        //        Balance = savingAccount.Balance,
        //        CreatedAt = savingAccount.CreatedAt,
        //        AccountType = "SavingAccount",
        //        CustomerId = savingAccount.CustomerId,
        //        CustomerName = customer.FullName
        //    };
        //    return CreatedAtAction(nameof(GetById), new { id = savingAccount.Id }, response);
        //}
        //[HttpPost("current")]
        //public async Task<ActionResult<AccountResponseDto>> CreateCurrentAccount(CurrentAccountDto dto)
        //{
        //    var customer = await _context.Customers.FindAsync(dto.CustomerId);
        //    if (customer == null)
        //    {
        //        return BadRequest(new { message = "Customer does not exist. " });
        //    }

        //    string generatedAccountNumber = $"CUR-" + new Random().Next(10000000, 99999999).ToString();

        //    while (await _context.Accounts.AnyAsync(a => a.AccountNumber == generatedAccountNumber))
        //    {
        //        generatedAccountNumber = $"CUR-" + new Random().Next(10000000, 99999999).ToString();
        //    }

        //    var currentAccount = new CurrentAccount
        //    {
        //        AccountNumber = generatedAccountNumber,
        //        Balance = dto.Balance,
        //        CustomerId = dto.CustomerId,
        //        WithdrawalLimit = dto.WithdrawLimit
        //    };  

        //    _context.Accounts.Add(currentAccount);
        //    await _context.SaveChangesAsync();

        //    var response = new AccountResponseDto
        //    {
        //        Id = currentAccount.Id,
        //        AccountNumber = generatedAccountNumber,
        //        Balance = currentAccount.Balance,
        //        CreatedAt = currentAccount.CreatedAt,
        //        AccountType = "CurrentAccount",
        //        CustomerId = currentAccount.CustomerId,
        //        CustomerName = customer.FullName
        //    };

        //    return CreatedAtAction(nameof(GetById), new { id = currentAccount.Id }, response);
    }
}


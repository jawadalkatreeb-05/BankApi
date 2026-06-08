using BankApi.Data;
using BankApi.Data.Models;
using BankApi.Data.Models.DTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountServices _accountService;

        public AccountsController(IAccountServices accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAll()
        {
            var result = await _accountService.GetAllAccountsAsync();

            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponseDto>> GetById(int id)
        {
            var result = await _accountService.GetAccountByIdAsync(id);

            if (result == null) return NotFound("Account not found");

            return Ok(result);
        }
        [HttpGet("{id}/history")]
        public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetAccountHistory(int id)
        {
            var accountExists = await _accountService.GetAccountByIdAsync(id);

            if (accountExists == null) return NotFound("Account not found");

            var result = await _accountService.GetAccountHistoryAsync(id);

            return Ok(result);
        }

        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(int id, [FromBody] decimal amount)
        {
            var result = await _accountService.DepositAsync(id, amount);

            if (!result) return BadRequest("Deposit failed");

            return Ok("Deposit successful");
        }
        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(int id, [FromBody] decimal amount)
        {
            var result = await _accountService.WithdrawAsync(id, amount);

            if (!result) return BadRequest("Withdrawal failed");
            
            return Ok("Withdrawal successful");
        }
        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _accountService.DeleteAccountAsync(id);

            if (!result) return BadRequest("Account deletion failed");

            return Ok("Account deleted successfully");
        }

        // _____________________________________________________________________________________________________
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAll()
        //{
        //    var accounts = await _context.Accounts.Include(a => a.Customer).ToListAsync();
        //    var response = accounts.Select(a => new AccountResponseDto
        //    {

        //        Id = a.Id,
        //        AccountNumber = a.AccountNumber,
        //        Balance = a.Balance,
        //        CreatedAt = a.CreatedAt,
        //        AccountType = a switch
        //        {
        //            SavingAccount => "Saving Account",
        //            CurrentAccount => "Current Account",
        //            _ => "BaseAccount"
        //        },
        //        CustomerId = a.CustomerId,
        //        CustomerName = a.Customer != null ? $"{a.Customer.FirstName} {a.Customer.LastName}" : "Unknown"
        //    }).ToList();

        //    return Ok(response);
        //}
        //[HttpGet("{id}")]
        //public async Task<ActionResult<AccountResponseDto>> GetById(int id)
        //{
        //    var account = await _context.Accounts
        //        .Include(a => a.Customer)
        //        .FirstOrDefaultAsync(a => a.Id == id);

        //    if (account == null)
        //    {
        //        return NotFound(new { message = $"Account with ID {id} not found." });
        //    }

        //    var response = new AccountResponseDto
        //    {
        //        Id = account.Id,
        //        AccountNumber = account.AccountNumber,
        //        Balance = account.Balance,
        //        CreatedAt = account.CreatedAt,
        //        AccountType = account.GetType().Name, 
        //        CustomerId = account.CustomerId,
        //        CustomerName = account.Customer != null ? account.Customer.FullName : "Unknown"
        //    };

        //    return Ok(response);
        //}
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


using BankApi.Data;
using BankApi.Data.Models;
using BankApi.Data.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AccountsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAll()
        {
            var response = await _context.Accounts.Select(a => new AccountResponseDto
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                Balance = a.Balance,
                CreatedAt = a.CreatedAt,
                AccountType = a is SavingAccount ? "Saving Account" : "CurrentAccount",
                CustomerId = a.CustomerId,
                CustomerName = a.Customer != null ? $"{a.Customer.FirstName} {a.Customer.LastName}" : "Unknown"
            }).ToListAsync();
            
            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponseDto>> GetById(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (account == null)
            {
                return NotFound(new { message = $"Account with ID {id} not found." });
            }

            var response = new AccountResponseDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt,
                AccountType = account.GetType().Name, 
                CustomerId = account.CustomerId,
                CustomerName = account.Customer != null ? account.Customer.FullName : "Unknown"
            };

            return Ok(response);
        }
        [HttpPost("saving")]
        public async Task<ActionResult<AccountResponseDto>> CreateSavingAccount(SavingAccountDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
            {
                return BadRequest(new { message = "Customer does not exist. " });
            }
            string generatedAccountNumber = $"SAV-" + new Random().Next(10000000, 99999999).ToString();

            while (await _context.Accounts.AnyAsync(a => a.AccountNumber == generatedAccountNumber))
            {
                generatedAccountNumber = $"SAV-" + new Random().Next(10000000, 99999999).ToString();
            }

            var savingAccount = new SavingAccount
            {
                AccountNumber = generatedAccountNumber,
                Balance = dto.Balance,
                CustomerId = dto.CustomerId,
                InterestRate = dto.InterestRate
            };
            _context.Accounts.Add(savingAccount);
            await _context.SaveChangesAsync();

            var response = new AccountResponseDto
            {
                Id = savingAccount.Id,
                AccountNumber = generatedAccountNumber,
                Balance = savingAccount.Balance,
                CreatedAt = savingAccount.CreatedAt,
                AccountType = "SavingAccount",
                CustomerId = savingAccount.CustomerId,
                CustomerName = customer.FullName
            };
            return CreatedAtAction(nameof(GetById), new { id = savingAccount.Id }, response);
        }
        [HttpPost("current")]
        public async Task<ActionResult<AccountResponseDto>> CreateCurrentAccount(CurrentAccountDto dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
            {
                return BadRequest(new { message = "Customer does not exist. " });
            }

            string generatedAccountNumber = $"CUR-" + new Random().Next(10000000, 99999999).ToString();

            while (await _context.Accounts.AnyAsync(a => a.AccountNumber == generatedAccountNumber))
            {
                generatedAccountNumber = $"CUR-" + new Random().Next(10000000, 99999999).ToString();
            }

            var currentAccount = new CurrentAccount
            {
                AccountNumber = generatedAccountNumber,
                Balance = dto.Balance,
                CustomerId = dto.CustomerId,
                WithdrawalLimit = dto.WithdrawLimit
            };  

            _context.Accounts.Add(currentAccount);
            await _context.SaveChangesAsync();

            var response = new AccountResponseDto
            {
                Id = currentAccount.Id,
                AccountNumber = generatedAccountNumber,
                Balance = currentAccount.Balance,
                CreatedAt = currentAccount.CreatedAt,
                AccountType = "CurrentAccount",
                CustomerId = currentAccount.CustomerId,
                CustomerName = customer.FullName
            };

            return CreatedAtAction(nameof(GetById), new { id = currentAccount.Id }, response);
        }
    }
}

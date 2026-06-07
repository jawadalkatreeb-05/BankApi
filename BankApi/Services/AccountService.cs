using BankApi.Data;
using BankApi.Data.Enums;
using BankApi.Data.Models;
using BankApi.Data.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services
{
    public class AccountService : IAccountSevices
    {
        private readonly AppDbContext _context;
        public AccountService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync()
        {
            var response = await _context.Accounts
                .Select(a => new AccountResponseDto
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    Balance = a.Balance,
                    CreatedAt = a.CreatedAt,
                    AccountType = a is SavingAccount ? "SavingAccount" : "CurrentAccount",
                    CustomerId = a.CustomerId,
                    CustomerName = a.Customer != null ? a.Customer.FirstName + " " + a.Customer.LastName : "Unknown"
                }).ToListAsync();

            return response;
        }

        public async Task<AccountResponseDto> GetAccountByIdAsync(int id)
        {
            var account = await _context.Accounts
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (account == null) return null!;
            return new AccountResponseDto  // Review 
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance,
                CreatedAt = account.CreatedAt,
                AccountType = account is SavingAccount ? "SavingAccount" : "CurrentAccount",
                CustomerId = account.CustomerId,
                CustomerName = account.Customer != null ? account.Customer.FirstName + " " + account.Customer.LastName : "Unknown"
            };
        }
        public async Task<bool> DeleteAccountAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return false;
            }
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DepositAsync(int accountId, decimal amount)
        {
            if (amount <= 0) return false;

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return false;

            account.Balance += amount; // about DTOs ??

            var transaction = new Transaction
            {
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                Type = TransactionType.Deposit,
                AccountId = accountId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> WithdrawAsync(int accountId, decimal amount)
        {
            return true;
        }
    }
}

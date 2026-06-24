using BankApi.Data;
using BankApi.Data.Enums;
using BankApi.Data.Models;
using BankApi.Data.Models.DTOs.RequestDTOs;
using BankApi.Data.Models.DTOs.ResponseDTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly AppDbContext _context;
        public AccountServices(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync()
        {
            var response = await _context.Accounts
                .AsNoTracking()
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
            var response = await _context.Accounts
                .AsNoTracking()
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (response == null) return null!;
            return new AccountResponseDto  // Review 
            {
                Id = response.Id,
                AccountNumber = response.AccountNumber,
                Balance = response.Balance,
                CreatedAt = response.CreatedAt,
                AccountType = response is SavingAccount ? "SavingAccount" : "CurrentAccount",
                CustomerId = response.CustomerId,
                CustomerName = response.Customer != null ? response.Customer.FirstName + " " + response.Customer.LastName : "Unknown"
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

            account.Balance += amount; // about DTOs

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
            if (amount <= 0) return false;

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return false;

            if (account.Balance < amount) return false;

            account.Balance -= amount;

            var transaction = new Transaction
            {
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                Type = TransactionType.Withdrawal,
                AccountId = accountId
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<TransactionResponseDto>> GetAccountHistoryAsync(int accountId)
        {
            var response = await _context.Transactions
                .AsNoTracking()
                .Where(r => r.AccountId == accountId)
                .OrderByDescending(r => r.TransactionDate)
                .Select(r => new TransactionResponseDto
                {
                    Id = r.Id,
                    Amount = r.Amount,
                    TransactionDate = r.TransactionDate,
                    TransactionType = r.Type.ToString(),
                    AccountId = r.AccountId,
                    AccountNumber = r.Account != null ? r.Account.AccountNumber : "Unknown"
                }).ToListAsync();
                

            return response;
        }
        public async Task<bool> UpdateAccountAsync(int accountId, decimal newLimit)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return false;

            if (account is not CurrentAccount currentAccount)
            {
                return false;
            }

            currentAccount.WithdrawalLimit = newLimit;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AccountResponseDto> CreateSavingAccountAsync(SavingAccountDto dto)
        {
            var customerExists = await _context.Customers.AsNoTracking().AnyAsync(c => c.Id == dto.CustomerId);
            if (!customerExists) return null!;

            var savingAccount = new SavingAccount
            {
                AccountNumber = await GenerateUniqueAccountNumberAsync("SAV"),
                Balance = dto.Balance,
                CreatedAt = DateTime.UtcNow,
                CustomerId = dto.CustomerId,
                InterestRate = dto.InterestRate
            };
            await _context.Accounts.AddAsync(savingAccount);
            await _context.SaveChangesAsync();

            return await GetAccountByIdAsync(savingAccount.Id);
        }

        public async Task<AccountResponseDto> CreateCurrentAccountAsync(CurrentAccountDto dto)
        {
            var customerExists = await _context.Customers.AsNoTracking().AnyAsync(c => c.Id == dto.CustomerId);
            if (!customerExists) return null!;

            var currentAccount = new CurrentAccount
            {
                AccountNumber = await GenerateUniqueAccountNumberAsync("CUR"),
                Balance = dto.Balance,
                CreatedAt = DateTime.UtcNow,
                CustomerId = dto.CustomerId,
                WithdrawalLimit= dto.WithdrawalLimit
            };

            await _context.Accounts.AddAsync(currentAccount);
            await _context.SaveChangesAsync();

            return await GetAccountByIdAsync(currentAccount.Id);
        }
        private async Task<string> GenerateUniqueAccountNumberAsync(string prefix)
        {
            var random = new Random();
            string accountNumber = string.Empty;
            bool isUnique = false;
            while (!isUnique)
            {
                int randomNumber = random.Next(10000000, 99999999);
                accountNumber = $"{prefix}-{randomNumber})";

                var exists = await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber);
                if (!exists)
                {
                    isUnique = true;
                }   
            }
            return  accountNumber;
        }
    }
}

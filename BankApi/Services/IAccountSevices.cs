using BankApi.Data;
using BankApi.Data.Models.DTOs;

namespace BankApi.Services
{
    public interface IAccountSevices
    {
        Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync();
        Task<AccountResponseDto> GetAccountByIdAsync(int id);
        Task<bool> DeleteAccountAsync(int id);
        Task<bool> WithdrawAsync(int accountId, decimal amount);
        Task<bool> DepositAsync(int accountId, decimal amount);
    }
}

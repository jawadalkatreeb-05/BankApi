using BankApi.Data;
using BankApi.Data.Models.DTOs.RequestDTOs;
using BankApi.Data.Models.DTOs.ResponseDTOs;

namespace BankApi.Services
{
    public interface IAccountServices
    {
        Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync();
        Task<AccountResponseDto> GetAccountByIdAsync(int id);
        Task<bool> DeleteAccountAsync(int id);
        Task<bool> WithdrawAsync(int accountId, decimal amount);
        Task<bool> DepositAsync(int accountId, decimal amount);
        Task<IEnumerable<TransactionResponseDto>> GetAccountHistoryAsync(int accountId);
        Task<bool> UpdateAccountAsync(int accountId, decimal newLimit);

        // Creating Acccount
        Task<AccountResponseDto> CreateSavingAccountAsync(SavingAccountDto dto);
        Task<AccountResponseDto> CreateCurrentAccountAsync(CurrentAccountDto dto);


    }
}

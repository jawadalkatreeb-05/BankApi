using BankApi.Data.Models.DTOs;

namespace BankApi.Services
{
    public interface ICustomerServices
    {
        Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync();
        Task<CustomerResponseDto> GetCustomerByIdAsync(int id);
        Task<CustomerResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto);
        Task<CustomerResponseDto> LoginAsync(LoginDto dto);
        Task<bool> UpdateCustomerAsync(int id, UpdateCustomerDto dto);
    }
}

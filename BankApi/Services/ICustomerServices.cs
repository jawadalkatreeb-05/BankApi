using BankApi.Data.Models.DTOs.RequestDTOs;
using BankApi.Data.Models.DTOs.ResponseDTOs;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BankApi.Services
{
    public interface ICustomerServices
    {
        Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync(string? searchTerm = null);
        Task<CustomerResponseDto> GetCustomerByIdAsync(int id);
        Task<CustomerResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task<bool> UpdateCustomerAsync(int id, UpdateCustomerDto dto);
        Task<bool> ChangePasswordAsync(int id, ChangePasswordDto dto);

        Task<bool> ToggleCustomerStatusAsync(int id, bool isActive);
    }
}

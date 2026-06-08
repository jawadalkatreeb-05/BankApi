using BankApi.Data;
using BankApi.Data.Models;
using BankApi.Data.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BankApi.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly AppDbContext _context;

        public CustomerServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers.AsNoTracking()
                .Select(c => new CustomerResponseDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Phone = c.Phone,
                    Email = c.Email,
                    FullName = $"{c.FirstName} {c.LastName}"
                }).ToListAsync();
            return customers;
        }

        public async Task<CustomerResponseDto> GetCustomerByIdAsync(int id)
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return null!;
            return new CustomerResponseDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Email = customer.Email,
                FullName = $"{customer.FirstName} {customer.LastName}"
            };
        }

        public async Task<CustomerResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto)
        {
            var emailExists = await _context.Customers.AnyAsync(c => c.Email.ToLower() == dto.Email.ToLower());
            if (emailExists) return null!;

            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var customer = new Customer
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                Email = dto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            
            return new CustomerResponseDto
            {
                Id = customer.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Phone = dto.Phone,
                Email = dto.Email,
                FullName = $"{dto.FirstName} {dto.LastName}"
            }; 
        }
        public async Task<CustomerResponseDto> LoginAsync(LoginDto dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email.ToLower() == dto.Email.ToLower());
            if(customer == null) return null!;

            if (!VerifyPasswordHash(dto.Password, customer.PasswordHash, customer.PasswordSalt)) return null!;

            return new CustomerResponseDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Email = customer.Email,
                FullName = customer.FullName
            };
        }

        public Task<bool> UpdateCustomerAsync(int id, UpdateCustomerDto dto)
        {
            throw new NotImplementedException();
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computingHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computingHash.Length; i++)
                {
                    if (computingHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        
    }
}

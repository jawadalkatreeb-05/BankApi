using BankApi.Data;
using BankApi.Data.Models;
using BankApi.Data.Models.DTOs.RequestDTOs;
using BankApi.Data.Models.DTOs.ResponseDTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BankApi.Services
{
    public class CustomerServices : ICustomerServices
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public CustomerServices(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IEnumerable<CustomerResponseDto>> GetAllCustomersAsync(string? searchTerm)  // -------------- Done
        {
            var customers = _context.Customers.AsNoTracking();
                
            if (!string.IsNullOrEmpty(searchTerm))
            {
                customers = customers.Where(c => c.FirstName.ToLower().Contains(searchTerm.ToLower()) || c.LastName.ToLower().Contains(searchTerm.ToLower()));
            }

            return await customers
                .Select(c => new CustomerResponseDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Phone = c.Phone,
                    Email = c.Email,
                    IsActive = c.IsActive,
                    FullName = $"{c.FirstName} {c.LastName}",
                    Accounts = null
                })
                .ToListAsync();
        }

        public async Task<CustomerResponseDto> GetCustomerByIdAsync(int id) // --------------- Done
        {
            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return null!;

            var accountTypes = await _context.Accounts
                .AsNoTracking()
                .Where(a => a.CustomerId == customer.Id)
                .Select(a => a is SavingAccount ? "SavingAccount": "CurrentAccount")
                .Distinct()
                .ToListAsync();

            return new CustomerResponseDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Email = customer.Email,
                IsActive = customer.IsActive,
                FullName = $"{customer.FirstName} {customer.LastName}",
                Accounts = accountTypes.Any() ? accountTypes : null
            };
        }

        public async Task<CustomerResponseDto> RegisterCustomerAsync(RegisterCustomerDto dto) // --------------- Done
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
                PasswordSalt = passwordSalt,
                IsActive = true
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
                IsActive = customer.IsActive,
                FullName = $"{dto.FirstName} {dto.LastName}"
            }; 
        }
        public async Task<LoginResponseDto> LoginAsync(LoginDto dto) // --------------- Done
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email.ToLower() == dto.Email.ToLower());
            if(customer == null) return null!;

            if (!customer.IsActive) return null!;

            if (!VerifyPasswordHash(dto.Password, customer.PasswordHash, customer.PasswordSalt)) return null!;
                
            var customerDto = new CustomerResponseDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Email = customer.Email,
                IsActive = customer.IsActive,
                FullName = customer.FullName
            };
            var generatedToken = CreateJwtToken(customer);

            return new LoginResponseDto
            {
               Token = generatedToken,
               Expiration = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]!)),
               Customer = customerDto
            };

        }

        public async Task<bool> UpdateCustomerAsync(int id, UpdateCustomerDto dto) // --------------- Done
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id ==id);
            if(customer == null) return false;

            customer.FirstName = dto.FirstName;
            customer.LastName = dto.LastName;
            customer.Phone = dto.Phone;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> ChangePasswordAsync(int id, ChangePasswordDto dto) // -------------- Done
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            if (customer == null) return false;

            if (!VerifyPasswordHash(dto.CurrentPassword, customer.PasswordHash, customer.PasswordSalt))
                return false;

            if (dto.NewPassword != dto.ConfirmPassword)
                return false;

            CreatePasswordHash(dto.NewPassword, out byte[] newHash, out byte[] newSalt);

            customer.PasswordHash = newHash;
            customer.PasswordSalt = newSalt;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleCustomerStatusAsync(int id, bool isActive)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null) return false;

            customer.IsActive = isActive;
            
            await _context.SaveChangesAsync();
            return true;
        }
// _____________________________________________________________________________________________________________________ //

        private string CreateJwtToken(Customer customer)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Name, customer.FullName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]!)),
                Issuer = _configuration["Jwt:Issure"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
               
            return tokenHandler.WriteToken(token);
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

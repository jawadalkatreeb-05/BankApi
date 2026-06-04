using BankApi.Data;
using BankApi.Data.Models;
using BankApi.Data.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetAll()
        {
            var customers = await _context.Customers.ToListAsync();

            var response = customers.Select(c => new CustomerResponseDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                FullName = c.FullName,
                Phone = c.Phone,
                Email = c.Email,
            });

            return Ok(response);
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<CustomerResponseDto>> Register(RegisterCustomerDto dto)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == dto.Email))
            {
                return BadRequest(new { message = "Email is already registered." });
            }

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
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var response = new CustomerResponseDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Email = customer.Email,
                FullName = customer.FullName
            };
            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, response);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponseDto>> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }
            var response = new CustomerResponseDto 
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.Phone,
                Email = customer.Email,
                FullName = customer.FullName,
            };
            return Ok(response);
        }
    }
}

using BankApi.Data;
using BankApi.Data.Models;
using BankApi.Data.Models.DTOs.RequestDTOs;
using BankApi.Data.Models.DTOs.ResponseDTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerServices _customerServices;

        public CustomersController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerResponseDto>>> GetAll([FromQuery] string? searchTerm = null)
        {
            var customers = await _customerServices.GetAllCustomersAsync(searchTerm);

            return Ok(customers);   
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponseDto>> GetById(int id)
        {
            var customer = await _customerServices.GetCustomerByIdAsync(id);

            if (customer == null) return NotFound("Customer not found" );

            return Ok(customer);
        }

        [HttpPost("register")]
        public async Task<ActionResult<CustomerResponseDto>> Register([FromBody] RegisterCustomerDto dto)
        {
            var result = await _customerServices.RegisterCustomerAsync(dto);
            if (result == null) return BadRequest("Email is already registered.");

            //return GetById(result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto dto)
        {
            var result = await _customerServices.LoginAsync(dto);
            if (result == null) return BadRequest("Invalid email or password.");

            return Ok(new { Message = "Login successful", Customer = result });
        }
        [HttpPost("update")]
        public async Task<IActionResult> Update(int id,[FromBody] UpdateCustomerDto dto)
        {
            var result = await _customerServices.UpdateCustomerAsync(id, dto);
            if (!result) return BadRequest("Customer Not Found");

            var updatedCustomer = await _customerServices.GetCustomerByIdAsync(id);
            return Ok(updatedCustomer);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(int id, ChangePasswordDto dto)
        {
            var result = await _customerServices.ChangePasswordAsync(id, dto);
            if (!result) return BadRequest("Mission Failed");

            return Ok(new { message = "Password Changed Successfully" });
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> Toggle(int id, [FromQuery] bool isActive)
        {
            var result = await _customerServices.ToggleCustomerStatusAsync(id, isActive);
            if (!result) return BadRequest("Customer Not Found");
            var updatedCustomer = await _customerServices.GetCustomerByIdAsync(id);
            return Ok(updatedCustomer);
        }







        //[HttpPost("register")]
        //public async Task<ActionResult<CustomerResponseDto>> Register(RegisterCustomerDto dto)
        //{
        //    if (await _context.Customers.AnyAsync(c => c.Email == dto.Email))
        //    {
        //        return BadRequest(new { message = "Email is already registered." });
        //    }

        //    CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

        //    var customer = new Customer
        //    {
        //        FirstName = dto.FirstName,
        //        LastName = dto.LastName,
        //        Phone = dto.Phone,
        //        Email = dto.Email,
        //        PasswordHash = passwordHash,
        //        PasswordSalt = passwordSalt
        //    };
        //    _context.Customers.Add(customer);
        //    await _context.SaveChangesAsync();

        //    var response = new CustomerResponseDto
        //    {
        //        Id = customer.Id,
        //        FirstName = customer.FirstName,
        //        LastName = customer.LastName,
        //        Phone = customer.Phone,
        //        Email = customer.Email,
        //        FullName = customer.FullName
        //    };
        //    return CreatedAtAction(nameof(GetById), new { id = customer.Id }, response);
        //}



    }
}

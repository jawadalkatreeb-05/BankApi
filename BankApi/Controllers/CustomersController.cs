using BankApi.Data;
using BankApi.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> GetAll()
        {
            var customers = await _context.Customers.ToListAsync();
            return Ok(customers);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound(new { message = "Customer not found" });
            }
            return Ok(customer);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
        }
    }
}

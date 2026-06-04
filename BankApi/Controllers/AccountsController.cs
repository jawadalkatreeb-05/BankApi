using BankApi.Data;
using BankApi.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AccountsController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return Ok(accounts);
        }
        [HttpPost("{saving}")]
        public async Task<IActionResult> CreateSavingAccount(SavingAccount account)
        {
            var customerEsists = await _context.Customers.AnyAsync(c => c.Id == account.CustomerId);
            if (!customerEsists)
            {
                return BadRequest(new { message = "Customer does not exist. " });
            }
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = account.Id }, account);
        }
        [HttpPost("{current}")]
        public async Task<IActionResult> CreateCurrentAccount(CurrentAccount account)
        {
            var customerEsists = await _context.Customers.AnyAsync(c => c.Id == account.CustomerId);
            if (!customerEsists)
            {
                return BadRequest(new { message = "Customer does not exist. " });
            }
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAll), new { id = account.Id }, account);
        }
    }
}

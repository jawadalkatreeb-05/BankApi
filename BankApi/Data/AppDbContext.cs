using Microsoft.EntityFrameworkCore;
using BankApi.Data.Models;
namespace BankApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<SavingAccount> SavingAccounts { get; set; }
        public DbSet<CurrentAccount> CurrentAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}

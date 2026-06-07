using BankApi.Data.Enums;

namespace BankApi.Data.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public TransactionType Type { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; } = null!;
    }
}

using BankApi.Data.Enums;

namespace BankApi.Data.Models.DTOs.ResponseDTOs
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } 
        public string TransactionType { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public string AccountNumber{ get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace BankApi.Data.Models.DTOs
{
    public class SavingAccountDto
    {
        [Required]
        [StringLength(20)]
        public string AccountNumber { get; set; } = string.Empty;

        [Required]
        public decimal Balance { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public decimal InterestRate { get; set; }
    }
}

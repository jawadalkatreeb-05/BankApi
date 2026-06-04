using System.ComponentModel.DataAnnotations;

namespace BankApi.Data.Models
{
    public class SavingAccount : Account
    {
        [Required]
        public decimal InterestRate { get; set; }
        public decimal CalculateInterest()
        {
            return Balance * InterestRate;
        }
    }
}

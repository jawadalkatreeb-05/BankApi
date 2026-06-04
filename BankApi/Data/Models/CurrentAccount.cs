using System.ComponentModel.DataAnnotations;

namespace BankApi.Data.Models
{
    public class CurrentAccount : Account
    {
        [Required]
        public decimal WithdrawalLimit { get; set; }
        public bool IsWithdrawalAllowed(decimal amount)
        {
            return amount <= WithdrawalLimit && amount <= Balance;
        }
    }
}

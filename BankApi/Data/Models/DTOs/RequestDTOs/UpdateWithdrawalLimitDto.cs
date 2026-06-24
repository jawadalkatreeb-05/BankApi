using System.ComponentModel.DataAnnotations;

namespace BankApi.Data.Models.DTOs.RequestDTOs
{
    public class UpdateWithdrawalLimitDto
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Withdrawal limit must be a non-negative value.")]
        public decimal NewWithdrawalLimit { get; set; }
    }
}

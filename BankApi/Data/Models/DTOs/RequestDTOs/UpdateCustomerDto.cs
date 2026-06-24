using System.ComponentModel.DataAnnotations;

namespace BankApi.Data.Models.DTOs.RequestDTOs
{
    public class UpdateCustomerDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
    }
}

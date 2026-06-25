using System.ComponentModel.DataAnnotations;

namespace BankApi.Data.Models.DTOs.RequestDTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]

        public string Password { get; set; } = string.Empty;
    }
}

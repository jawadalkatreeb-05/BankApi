namespace BankApi.Data.Models.DTOs.ResponseDTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public CustomerResponseDto Customer { get; set; } = null!;

    }
}

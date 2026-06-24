using Azure;
using System.Text.Json.Serialization;

namespace BankApi.Data.Models.DTOs.ResponseDTOs
{
    public class CustomerResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Accounts { get; set; } // GetAllCust will ignore it
    }
}

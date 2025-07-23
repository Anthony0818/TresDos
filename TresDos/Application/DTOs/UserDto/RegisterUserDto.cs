namespace TresDos.Application.DTOs.UserDto
{
    public class RegisterUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int CommissionPercentage { get; set; }
        public int ParentId { get; set; }
        public bool Status { get; set; }
    }
}

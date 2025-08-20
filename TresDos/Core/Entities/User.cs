using Microsoft.AspNetCore.Mvc.Rendering;
namespace TresDos.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "User";
        public int? CommissionPercentage { get; set; }
        public int? ParentId { get; set; }
        public bool? Status { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TresDos.Models
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
    public class AgentService
    {
        public List<User> GetDummyAgents()
        {
            return new List<User>
        {
            new User { Id = 1, FirstName = "Alice Smith", CommissionPercentage = 10, ParentId = null, Status = true },
            new User { Id = 2, FirstName = "Bob Johnson", CommissionPercentage = 8, ParentId = 1, Status = true },
            new User { Id = 3, FirstName = "Charlie Brown", CommissionPercentage = 5, ParentId = 2, Status = false },
            new User { Id = 4, FirstName = "Diana Prince", CommissionPercentage = null, ParentId = 1, Status = null }
        };
        }

        public List<SelectListItem> GetAgentSelectList()
        {
            return GetDummyAgents()
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.FirstName
                })
                .ToList();
        }
    }
}

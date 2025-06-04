using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Core.Entities
{
    public class User
    {
        public int? Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string PasswordHash { get; set; }
        public bool IsTeacher { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Role { get; set; }
        public ICollection<FolderUser> FolderUsers { get; set; }
    }
}

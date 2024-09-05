using Microsoft.AspNetCore.Identity;

namespace SassProject.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
        public string UpdatedAt { get; set; } = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

    }
}

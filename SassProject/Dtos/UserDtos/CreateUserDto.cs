using System.ComponentModel.DataAnnotations;

namespace SassProject.Dtos.UserDtos
{
    public enum UserType
    {
        ADMIN,
        VIEWER
    }
    public class CreateUserDto
    {
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s.\-]{2,}$", ErrorMessage = "Please Enter Only Letters")]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s.\-]{2,}$", ErrorMessage = "Please Enter Only Letters")]
        public string LastName { get; set; } = string.Empty;
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        
        public UserType UserType { get; set; } = UserType.VIEWER;
    }
}

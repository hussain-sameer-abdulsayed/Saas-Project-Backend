using System.ComponentModel.DataAnnotations;

namespace SassProject.Dtos.UserDtos
{
    public class ForgotPasswordConfirmation
    {
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

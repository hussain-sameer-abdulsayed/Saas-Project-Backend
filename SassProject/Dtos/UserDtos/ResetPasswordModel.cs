using System.ComponentModel.DataAnnotations;

namespace SassProject.Dtos.UserDtos
{
    public class ResetPasswordModel
    {
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

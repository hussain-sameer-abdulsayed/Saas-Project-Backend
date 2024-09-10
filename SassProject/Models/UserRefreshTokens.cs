namespace SassProject.Models
{
    public class UserRefreshTokens
    {
        public int Id { get; set; } // change it to string
        public string Message { get; set; } = "Success";
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

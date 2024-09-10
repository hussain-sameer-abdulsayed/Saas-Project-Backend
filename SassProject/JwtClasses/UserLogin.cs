namespace SassProject.JwtClasses
{
    public class UserLogin
    {
        public string email { get; set; }
        public string password { get; set; }
        public string? Message { get; set; }
        public bool? IsAuthenticated { get; set; }
    }
}

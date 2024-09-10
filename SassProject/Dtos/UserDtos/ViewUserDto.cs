namespace SassProject.Dtos.UserDtos
{
    public class ViewUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;
        public string UpdatedAt { get; set; } = string.Empty; 
        public bool EmailConfirmed { get; set; } = false;
        public List<string> Role { get; set; } = new List<string>();

        
    }

}

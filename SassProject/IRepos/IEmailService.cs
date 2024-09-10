using SassProject.Dtos.UserDtos;

namespace SassProject.IRepos
{
    public interface IEmailService
    {
        Task SendValidationEmailAsync(EmailDto request);
    }
}

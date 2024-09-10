using Microsoft.AspNetCore.Identity;
using SassProject.Dtos.UserDtos;
using SassProject.JwtClasses;
using SassProject.Models;

namespace SassProject.IRepos
{
    public interface IUserRepo
    {
        Task<IEnumerable<ViewUserDto>> GetAdminsAsync();
        Task<ViewUserDto> GetUserByIdAsync(string userId);
        Task<ViewUserDto> GetUserByEmailAsync(string Email);
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);
        Task<IEnumerable<ViewUserDto>> GetUsersAsync();
        Task<UserRefreshTokens> CreateUserAsync(CreateUserDto userDto);
        Task<CheckFunc> SetAdminAsync(string userId);
        Task<CheckFunc> RemoveRoleAdminAsync(string userId);
        Task<CheckFunc> DeleteUserAsync(string userId);
        Task<UserLogin> LoginAsync(UserLogin users);
        Task<UserRefreshTokens> AddUserRefreshTokensAsync(UserRefreshTokens user);
        Task<UserRefreshTokens> GetSavedRefreshTokensAsync(string username, string refreshToken);
        void DeleteUserRefreshTokens(string username, string refreshToken);
        Task<CheckFunc> UpdateUserAsync(string userId, UpdateUserDto userDto);
        Task<CheckFunc> ForgotPasswordConfirmationAsync(ForgotPasswordConfirmation model, string requestScheme, string requestHost);
        Task<CheckFunc> ForgotPasswordAsync(ForgotPasswordModel request);
        Task<CheckFunc> ResetPassword(string userId, ResetPasswordModel request);
        Task<User> VerifiyEmailAsync(string verificationToken);
    }
}

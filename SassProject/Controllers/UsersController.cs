using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NuGet.Common;
using SassProject.Dtos.UserDtos;
using SassProject.IRepos;
using SassProject.JwtClasses;
using SassProject.Models;
using SassProject.Repos;

namespace SassProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IEmailService _emailService;
        private readonly IJWTMangerRepo _jWTMangerRepo;
        private readonly IMapper _mapper;
        private readonly ITransactionRepo _transactionRepo;

        public UsersController(IUserRepo userRepo, IEmailService emailService, IJWTMangerRepo jWTMangerRepo, IMapper mapper, ITransactionRepo transactionRepo)
        {
            _userRepo = userRepo;
            _emailService = emailService;
            _jWTMangerRepo = jWTMangerRepo;
            _mapper = mapper;
            _transactionRepo = transactionRepo;
        }


        //[Authorize()]
        [HttpGet()]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userRepo.GetUsersAsync();
                if (users.Count() == 0)
                {
                    return NotFound("not found");
                }
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize()]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var user = await _userRepo.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("this user does not exist");
                }
                return Ok(user);
            }
            catch
            {
                return BadRequest();
            }
        }


        //[Authorize(Roles = "SUPERADMIN, ADMIN")]
        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {
            try
            {
                var admins = await _userRepo.GetAdminsAsync();
                if (admins.Count() == 0)
                {
                    return NotFound("There is no admin found.");
                }
                return Ok(admins);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //[Authorize(Roles = "SUPERADMIN, ADMIN")]
        [HttpGet("userRole/{userId}")]
        public async Task<IActionResult> GetUserRole(string userId)
        {
            try
            {
                var Roles = await _userRepo.GetUserRolesAsync(userId);
                if (Roles.Count() == 0)
                {
                    return NotFound("This user does not has a roles, There is no user is this id");
                }
                return Ok(Roles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // check this *return type*
        [HttpGet("verify")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string verificationToken)
        {
            try
            {
                var user = await _userRepo.VerifiyEmailAsync(verificationToken);
                if (user == null)
                {
                    return BadRequest("Invalid email or token.");
                }
                return Ok("Thank you for validating your email.");
            }
            catch
            {
                return BadRequest();
            }
        }


        [AllowAnonymous()]
        [HttpPost("signup")]
        public async Task<IActionResult> Register([FromForm] CreateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _transactionRepo.BeginTransactionAsync();
                // if string = admin create admin
                // if string = viwer create viwer
                // change it to enum
                var result = await _userRepo.CreateUserAsync(userDto);
                if (!result.IsAuthenticated)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(result.Message);
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok("Registration successful. Please check your email to verify your account.");
            }
            catch
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest();
            }
        }

        [AllowAnonymous()]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _userRepo.LoginAsync(userLogin);
                if (result.IsAuthenticated is false)
                {
                    return BadRequest(result.Message);
                }
                
                var user = await _userRepo.GetUserByEmailAsync(userLogin.email);
                var token = await _jWTMangerRepo.GenerateToken(user.UserName);
                if (token == null)
                {
                    return Unauthorized("Invalid attempt...");
                }

                UserRefreshTokens obj = new UserRefreshTokens
                {
                    UserName = user.UserName,
                    RefreshToken = token.RefreshToken,
                    Email = userLogin.email
                };

                await _userRepo.AddUserRefreshTokensAsync(obj);


                return Ok(token);
                /*
                var TestObj = new TestModel
                {
                    Id = 1,
                    token = token.AccessToken,
                    isFreelancer = true,
                    userId = user.Id
                };
                return Ok(TestObj);
                */
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken(MyToken token)
        {
            var principal = _jWTMangerRepo.GetPrincipalFromExpiredToken(token.AccessToken);
            var username = principal.Identity?.Name;

            var savedRefreshToken = await _userRepo.GetSavedRefreshTokensAsync(username, token.RefreshToken);

            if (savedRefreshToken.RefreshToken != token.RefreshToken)
            {
                return Unauthorized("Invalid attempt!");
            }

            var newJwtToken = await _jWTMangerRepo.GenerateRefreshToken(username);

            if (newJwtToken == null)
            {
                return Unauthorized("Invalid attempt!");
            }

            UserRefreshTokens obj = new UserRefreshTokens
            {
                RefreshToken = newJwtToken.RefreshToken,
                UserName = username
            };

            _userRepo.DeleteUserRefreshTokens(username, token.RefreshToken);
            await _userRepo.AddUserRefreshTokensAsync(obj);

            return Ok(newJwtToken);
        }


        [Authorize(Roles = "SUPERADMIN, ADMIN")]
        [HttpPost("setUserToAdmin/{userId}")]
        public async Task<IActionResult> SetUserToAdmin(string userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _transactionRepo.BeginTransactionAsync();
                var result = await _userRepo.SetAdminAsync(userId);
                if (!result.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(result.Message);
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(result.Message);

            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "SUPERADMIN, ADMIN")]
        [HttpDelete("removeAdmin/{userId}")]
        public async Task<IActionResult> DeleteAdmin(string userId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _transactionRepo.BeginTransactionAsync();
                var result = await _userRepo.RemoveRoleAdminAsync(userId);
                if (!result.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(result.Message);
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(result.Message);
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }


        [Authorize()]
        [HttpPut()]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                string accessToken = Request.Headers[HeaderNames.Authorization];
                var token = accessToken.Substring(7);
                var userId = _jWTMangerRepo.GetUserId(token);

                await _transactionRepo.BeginTransactionAsync();
                var result = await _userRepo.UpdateUserAsync(userId, userDto);
                if (!result.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest();
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(result.Message);
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }


        // this api for verifiy his email then the second(under) api for reset
        [HttpPost("forgotPasswordConfirmation")]
        public async Task<IActionResult> ForgotPasswordConfirmation([FromBody] ForgotPasswordConfirmation request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var check = await _userRepo.ForgotPasswordConfirmationAsync(request, Request.Scheme, Request.Host.Value);

                if (!check.IsSucceeded)
                {
                    return BadRequest(check.Message);
                }

                return Ok(check.Message);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        // when user verifiy his eamil will come to this api
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var check = await _userRepo.ForgotPasswordAsync(request);

                if (!check.IsSucceeded)
                {
                    return BadRequest(check.Message);
                }

                return Ok(check.Message);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [Authorize()]
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string accessToken = Request.Headers[HeaderNames.Authorization];
                var token = accessToken.Substring(7);
                var userId = _jWTMangerRepo.GetUserId(token);

                await _transactionRepo.BeginTransactionAsync();

                var check = await _userRepo.ResetPassword(userId, request);
                if (!check.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(check.Message);
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(check.Message);
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }
        

        [Authorize(Roles = "SUPERADMIN, ADMIN")]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _transactionRepo.BeginTransactionAsync();
                /*
                var imageDeleted = await _userRepo.DeleteUserImage(userId);
                if (!imageDeleted)
                {
                    return BadRequest("image was not Deleted");
                }
                */
                var result = await _userRepo.DeleteUserAsync(userId);
                if (!result.IsSucceeded)
                {
                    await _transactionRepo.RollBackTransactionAsync();
                    return BadRequest(result.Message);
                }
                await _transactionRepo.CommitTransactionAsync();
                return Ok(result.Message);
            }
            catch (Exception ex)
            {
                await _transactionRepo.RollBackTransactionAsync();
                return BadRequest($"User was not Deleted:{ex.Message}");
            }
        }




    }
}

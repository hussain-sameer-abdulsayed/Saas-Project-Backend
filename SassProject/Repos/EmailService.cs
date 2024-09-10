using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using SassProject.Dtos.UserDtos;
using SassProject.IRepos;


/*
using System.Net;
*/
namespace SassProject.Repos
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;

        public EmailService(IConfiguration configuration, UserManager<IdentityUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task SendValidationEmailAsync(EmailDto request)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_configuration.GetSection("EmailSettings:SenderName").Value!, _configuration.GetSection("EmailSettings:FromEmail").Value!));
            message.To.Add(MailboxAddress.Parse(request.To));
            message.Subject = request.Subject;
            var bodyBuilder = new BodyBuilder { HtmlBody = request.Body };
            message.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_configuration.GetSection("EmailSettings:MailServer").Value!, int.Parse(_configuration.GetSection("EmailSettings:MailPort").Value!), SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration.GetSection("EmailSettings:FromEmail").Value!, _configuration.GetSection("EmailSettings:Password").Value!);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

    }
}

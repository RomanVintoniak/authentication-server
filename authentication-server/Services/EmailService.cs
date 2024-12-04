using authentication_server.Configurations;
using authentication_server.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Security.Claims;
using System.Text;

namespace authentication_server.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISmtpClient _smtpClient;
        private readonly EmailServiceConfig _emailServiceConfig;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        public EmailService(
            ISmtpClient smtpClient, 
            IOptions<EmailServiceConfig> emailServiceConfig,
            IJwtService jwtService,
            IConfiguration configuration
        )
        {
            _smtpClient = smtpClient;
            _emailServiceConfig = emailServiceConfig.Value;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        public async Task SendEmailVerificationAsync(string toEmail)
        {
            string token =  _jwtService.GetToken([new Claim("email", toEmail)], DateTime.Now.AddHours(24));
            string verificationUrl = $"{_configuration["ClientBaseUrl"]}/email-verification/{token}";

            MimeMessage message = new();
            message.From.Add(MailboxAddress.Parse(_emailServiceConfig.From));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Email Verification";

            StringBuilder sb = new();
            sb.AppendFormat("<b>Hi!</b>");
            sb.AppendFormat($"<p>Please click <a href='{verificationUrl}'>here</a> or on link below to verify your email</p>");
            sb.AppendFormat($"<p><a href='{verificationUrl}'>{verificationUrl}</a></p>");

            message.Body = new TextPart(TextFormat.Html) { Text = sb.ToString() };

            _smtpClient.Connect(_emailServiceConfig.Host, _emailServiceConfig.Port, false);
            _smtpClient.Authenticate(_emailServiceConfig.From, _emailServiceConfig.AuthCode);
            await _smtpClient.SendAsync(message);
            _smtpClient.Disconnect(true);
        }
    }
}

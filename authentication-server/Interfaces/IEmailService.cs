namespace authentication_server.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailVerificationAsync(string toEmail);
    }
}

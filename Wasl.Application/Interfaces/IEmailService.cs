namespace Wasl.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string email, string confirmationLink);
        Task SendPasswordResetAsync(string email, string resetLink);
        Task SendTwoFactorCodeAsync(string email, string code);
        Task SendEmailAsync(string to, string subject, string body);
    }
}

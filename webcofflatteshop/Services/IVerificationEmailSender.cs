namespace webcofflatteshop.Services;

public interface IVerificationEmailSender
{
    Task SendEmailVerificationCodeAsync(string toEmail, string code);

    Task SendPasswordResetLinkAsync(string toEmail, string resetLink);
}

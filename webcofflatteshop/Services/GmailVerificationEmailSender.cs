using System.Net;
using System.Net.Mail;

namespace webcofflatteshop.Services;

public class GmailVerificationEmailSender : IVerificationEmailSender
{
    private readonly IConfiguration _configuration;

    public GmailVerificationEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailVerificationCodeAsync(string toEmail, string code)
    {
        await SendMailAsync(
            toEmail,
            "Mã xác thực Gmail - Coffe Latte Kawaii",
            $"""
            Xin chào,

            Mã xác thực Gmail của bạn là: {code}

            Mã này có hiệu lực trong 10 phút. Nếu bạn không yêu cầu đổi Gmail, vui lòng bỏ qua email này.

            Coffe Latte Kawaii
            """);
    }

    public async Task SendPasswordResetLinkAsync(string toEmail, string resetLink)
    {
        await SendMailAsync(
            toEmail,
            "Đặt lại mật khẩu - Coffe Latte Kawaii",
            $"""
            Xin chào,

            Bạn vừa yêu cầu đặt lại mật khẩu cho tài khoản Coffe Latte Kawaii.

            Hãy mở liên kết này để tạo mật khẩu mới:
            {resetLink}

            Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.

            Coffe Latte Kawaii
            """);
    }

    private async Task SendMailAsync(string toEmail, string subject, string body)
    {
        var section = _configuration.GetSection("GmailVerification");
        var senderEmail = section["SenderEmail"];
        var appPassword = section["AppPassword"]?.Replace(" ", string.Empty);
        var displayName = section["DisplayName"] ?? "Coffe Latte Kawaii";
        var host = section["Host"] ?? "smtp.gmail.com";
        var port = int.TryParse(section["Port"], out var configuredPort) ? configuredPort : 587;

        if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(appPassword)
            || senderEmail.Contains("your-gmail", StringComparison.OrdinalIgnoreCase)
            || appPassword.Contains("your-gmail", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Chưa cấu hình GmailVerification:SenderEmail và GmailVerification:AppPassword trong appsettings.json.");
        }

        using var message = new MailMessage
        {
            From = new MailAddress(senderEmail, displayName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(toEmail);

        using var smtp = new SmtpClient(host, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(senderEmail, appPassword)
        };

        try
        {
            await smtp.SendMailAsync(message);
        }
        catch (SmtpException ex) when (ex.Message.Contains("5.7.0", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("Authentication", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Gmail chưa xác thực được SMTP. Hãy kiểm tra SenderEmail đúng Gmail gửi, AppPassword là mật khẩu ứng dụng 16 ký tự, tài khoản Gmail đã bật xác minh 2 bước và không dùng mật khẩu đăng nhập Gmail thường.", ex);
        }
    }
}

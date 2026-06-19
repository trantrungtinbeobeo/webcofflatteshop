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
        var normalizedCode = new string((code ?? string.Empty).Where(char.IsDigit).ToArray()).PadLeft(6, '0')[^6..];
        var codeHtml = $"""
            <span style="color:#3e2723">{WebUtility.HtmlEncode(normalizedCode[..2])}</span><span style="color:#5d9f68">{WebUtility.HtmlEncode(normalizedCode.Substring(2, 2))}</span><span style="color:#ec82ac">{WebUtility.HtmlEncode(normalizedCode.Substring(4, 2))}</span>
            """;

        await SendMailAsync(
            toEmail,
            "Mã xác thực Gmail - Coffe Latte Kawaii",
            BuildBrandedEmail(
                "Xác thực Gmail của bạn",
                "Dùng mã bên dưới để xác thực Gmail trong trang cá nhân.",
                $"""
                <div style="margin:26px 0;padding:20px;border:1px solid #eadbd7;border-radius:12px;background:#fffaf9;text-align:center">
                    <div style="font-size:13px;font-weight:700;color:#74635f;letter-spacing:1px;text-transform:uppercase">Mã xác thực</div>
                    <div style="margin-top:8px;font-size:42px;font-weight:800;letter-spacing:8px">{codeHtml}</div>
                </div>
                <p style="margin:0;color:#625f4d;font-size:14px;line-height:22px">Mã có hiệu lực trong 10 phút. Nếu bạn không yêu cầu thay đổi Gmail, hãy bỏ qua email này.</p>
                """));
    }

    public async Task SendPasswordResetLinkAsync(string toEmail, string resetLink)
    {
        var safeLink = WebUtility.HtmlEncode(resetLink);
        await SendMailAsync(
            toEmail,
            "Đặt lại mật khẩu - Coffe Latte Kawaii",
            BuildBrandedEmail(
                "Tạo mật khẩu mới",
                "Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.",
                $"""
                <div style="margin:28px 0;text-align:center">
                    <a href="{safeLink}" style="display:inline-block;padding:13px 24px;border-radius:8px;background:#3e2723;color:#ffffff;font-size:15px;font-weight:700;text-decoration:none">Đặt lại mật khẩu</a>
                </div>
                <p style="margin:0;color:#625f4d;font-size:14px;line-height:22px">Nếu nút không hoạt động, mở liên kết sau:</p>
                <p style="margin:8px 0 0;word-break:break-all;color:#5d9f68;font-size:12px;line-height:19px">{safeLink}</p>
                <p style="margin:22px 0 0;color:#625f4d;font-size:14px;line-height:22px">Nếu bạn không yêu cầu đặt lại mật khẩu, hãy bỏ qua email này.</p>
                """));
    }

    private static string BuildBrandedEmail(string title, string introduction, string content)
    {
        return $"""
        <!doctype html>
        <html lang="vi">
        <body style="margin:0;padding:0;background:#f4f4ef;font-family:Arial,sans-serif;color:#3e2723">
            <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background:#f4f4ef;padding:32px 12px">
                <tr>
                    <td align="center">
                        <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:560px;overflow:hidden;border:1px solid #eadbd7;border-radius:14px;background:#ffffff;box-shadow:0 12px 35px rgba(62,39,35,.08)">
                            <tr>
                                <td style="padding:26px 30px;background:#fffaf9;border-bottom:1px solid #eadbd7;text-align:center">
                                    <div style="font-size:34px;line-height:36px;color:#ec82ac">✿</div>
                                    <div style="margin-top:6px;font-family:Georgia,serif;font-size:25px;font-weight:700">
                                        <span style="color:#3e2723">Coffe</span><span style="color:#5d9f68">Latte</span><span style="color:#ec82ac">Kawaii</span>
                                    </div>
                                    <div style="margin-top:5px;color:#827472;font-size:11px;font-weight:700;letter-spacing:1.4px;text-transform:uppercase">Saigon warmth · Kyoto precision</div>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding:32px 30px">
                                    <h1 style="margin:0;font-family:Georgia,serif;font-size:25px;line-height:32px;color:#3e2723">{WebUtility.HtmlEncode(title)}</h1>
                                    <p style="margin:12px 0 0;color:#625f4d;font-size:15px;line-height:24px">{WebUtility.HtmlEncode(introduction)}</p>
                                    {content}
                                </td>
                            </tr>
                            <tr>
                                <td style="padding:18px 30px;background:#3e2723;color:#ffdad4;text-align:center;font-size:12px;line-height:18px">
                                    © 2026 Coffe Latte Kawaii · Cảm ơn bạn đã đồng hành cùng tiệm.
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </body>
        </html>
        """;
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
            IsBodyHtml = true
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

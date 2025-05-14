// LikeMovie.Business/Services/EmailService.cs
using LikeMovie.Business.Interfaces;
using Microsoft.Extensions.Configuration; // Để đọc appsettings.json
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Text; // Cho Encoding
using System.Threading.Tasks;

namespace LikeMovie.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration; // Để đọc cấu hình email từ appsettings

        // Lấy thông tin cấu hình SMTP từ appsettings.json
        private string SmtpServer => _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
        private int SmtpPort => int.TryParse(_configuration["EmailSettings:SmtpPort"], out int port) ? port : 587;
        private string SmtpUsername => _configuration["EmailSettings:SmtpUsername"] ?? "your-email@gmail.com";
        private string SmtpPassword => _configuration["EmailSettings:SmtpPassword"] ?? "your-app-password"; // App Password cho Gmail
        private bool EnableSsl => bool.TryParse(_configuration["EmailSettings:EnableSsl"], out bool ssl) ? ssl : true;
        private string FromEmailAddress => _configuration["EmailSettings:FromEmailAddress"] ?? SmtpUsername;
        private string FromEmailDisplayName => _configuration["EmailSettings:FromEmailDisplayName"] ?? "LikeMovie Team";


        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                using (var client = new SmtpClient(SmtpServer, SmtpPort))
                {
                    client.EnableSsl = EnableSsl;
                    client.Credentials = new NetworkCredential(SmtpUsername, SmtpPassword);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(FromEmailAddress, FromEmailDisplayName),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8,
                        SubjectEncoding = Encoding.UTF8
                    };
                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully to {ToEmail} with subject {Subject}.", toEmail, subject);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail} with subject {Subject}.", toEmail, subject);
                return false;
            }
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string userName, string otp)
        {
            string subject = "Mã xác nhận OTP - LikeMovie";
            // TODO: Tạo template HTML đẹp hơn cho email OTP
            string body = $@"
                <html><body>
                <p>Xin chào {userName},</p>
                <p>Mã OTP của bạn để thực hiện thao tác trên LikeMovie là: <strong>{otp}</strong></p>
                <p>Mã này có hiệu lực trong vòng vài phút. Vui lòng không chia sẻ mã này cho bất kỳ ai.</p>
                <p>Trân trọng,<br/>Đội ngũ LikeMovie</p>
                </body></html>";
            return await SendEmailAsync(toEmail, subject, body);
        }

        public async Task<bool> SendRegistrationConfirmationEmailAsync(string toEmail, string userName)
        {
            string subject = "Chào mừng bạn đến với LikeMovie!";
            // TODO: Tạo template HTML đẹp hơn
            string body = $@"
                <html><body>
                <p>Xin chào {userName},</p>
                <p>Cảm ơn bạn đã đăng ký tài khoản thành công tại LikeMovie. Chúc bạn có những trải nghiệm xem phim vui vẻ!</p>
                <p>Trân trọng,<br/>Đội ngũ LikeMovie</p>
                </body></html>";
            return await SendEmailAsync(toEmail, subject, body);
        }
    }
}
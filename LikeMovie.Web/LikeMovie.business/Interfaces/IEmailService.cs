// LikeMovie.Business/Interfaces/IEmailService.cs
using System.Threading.Tasks;

namespace LikeMovie.Business.Interfaces
{
    public interface IEmailService
    {
        /// <summary>
        /// Gửi email.
        /// </summary>
        /// <param name="toEmail">Địa chỉ email người nhận.</param>
        /// <param name="subject">Chủ đề email.</param>
        /// <param name="htmlMessage">Nội dung email dưới dạng HTML.</param>
        /// <returns>True nếu gửi thành công, False nếu thất bại.</returns>
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage);

        /// <summary>
        /// Gửi email chứa mã OTP.
        /// </summary>
        Task<bool> SendOtpEmailAsync(string toEmail, string userName, string otp);

        /// <summary>
        /// Gửi email xác nhận đăng ký thành công.
        /// </summary>
        Task<bool> SendRegistrationConfirmationEmailAsync(string toEmail, string userName);
    }
}
using AppGroupCar.Exceptions;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AppGroupCar.Services.Auth.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            // 1. Tạo đối tượng email
            var email = new MimeMessage();

            // Lấy config từ appsettings.json
            var senderName = _config["EmailSettings:SenderName"];
            var senderEmail = _config["EmailSettings:SenderEmail"];

            email.From.Add(new MailboxAddress(senderName, senderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            // 2. Sử dụng BodyBuilder để tạo nội dung
            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody
            };

            // SỬA LỖI TẠI ĐÂY: Gọi phương thức ToMessageBody()
            email.Body = builder.ToMessageBody();

            // 3. Gửi qua SmtpClient của MailKit
            using var smtp = new SmtpClient();
            try
            {
                var host = _config["EmailSettings:Host"];
                var port = int.Parse(_config["EmailSettings:Port"]);
                var pass = _config["EmailSettings:Password"];

                // Kết nối an toàn (STARTTLS)
                await smtp.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(senderEmail, pass);

                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                // Ghi log lỗi ra Console để Hangfire bắt được và Retry
                Console.WriteLine($"[Lỗi gửi email]: {ex.Message}");

                throw new AppException(500, ex.Message);
            }
            finally
            {
                // Luôn đảm bảo ngắt kết nối sạch sẽ
                await smtp.DisconnectAsync(true);
            }
        }
    }
}

using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ProductINV.Services
{
    public class EmailService
    {
        private readonly string _fromEmail = "johnleelugnasin30@gmail.com";
        private readonly string _appPassword = "przo aaig mpxx ktvc"; // your app password

        public async Task SendVerificationCodeAsync(string toEmail, string verificationCode)
        {
            string subject = "Your Login Verification Code";
            string body = $"Your verification code is: {verificationCode}";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential(_fromEmail, _appPassword);
                smtpClient.EnableSsl = true;

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to send verification email: " + ex.Message);
                }
            }
        }
    }
}

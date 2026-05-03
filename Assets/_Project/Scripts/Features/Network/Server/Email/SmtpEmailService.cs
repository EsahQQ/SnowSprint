using System.Net;
using System.Net.Mail;
using UnityEngine;

namespace _Project.Scripts.Features.Network.Server.Email
{
    public class SmtpEmailService : IEmailService
    {
        private const string SmtpHost = "sandbox.smtp.mailtrap.io";
        private const int SmtpPort = 2525;
        private const string SmtpUser = "497d30f5b17405";
        private const string SmtpPass = "7681e0fce9996b";

        public void SendVerificationEmail(string toEmail, string code)
        {
            try
            {
                using var client = new SmtpClient(SmtpHost, SmtpPort)
                {
                    Credentials = new NetworkCredential(SmtpUser, SmtpPass),
                    EnableSsl = true
                };

                using var mail = new MailMessage("auth@mygame.com", toEmail)
                {
                    Subject = "Verification Code",
                    Body = $"Ваш код: {code}\nВведите его в игре."
                };

                client.Send(mail);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[EmailService] Ошибка отправки: {ex.Message}");
            }
        }
    }
}
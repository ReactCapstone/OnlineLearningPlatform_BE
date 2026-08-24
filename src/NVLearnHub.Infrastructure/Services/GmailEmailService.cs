using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using NVLearnHub.Application.Interfaces;

namespace NVLearnHub.Infrastructure.Services
{
    public class GmailEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public GmailEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpEmailAsync(string toEmail, string otpCode)
        {
            var host = _config["Email:Host"]!;
            var port = int.Parse(_config["Email:Port"]!);
            var senderEmail = _config["Email:SenderEmail"]!;
            var senderName = _config["Email:SenderName"]!;
            var appPassword = _config["Email:AppPassword"]!;

            var smtpClient = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(senderEmail, appPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Your NVLearnHub Verification Code",
                IsBodyHtml = true,
                Body = $@"
                    <div style='font-family:Arial,sans-serif;max-width:480px;margin:auto;padding:24px;border:1px solid #e5e7eb;border-radius:12px;'>
                        <h2 style='color:#4f46e5;margin-bottom:8px;'>NV LearnHub</h2>
                        <p style='color:#374151;'>Your verification code is:</p>
                        <div style='font-size:36px;font-weight:bold;letter-spacing:12px;color:#4f46e5;padding:16px 0;'>
                            {otpCode}
                        </div>
                        <p style='color:#6b7280;font-size:14px;'>This code expires in <b>10 minutes</b>.</p>
                        <p style='color:#6b7280;font-size:14px;'>If you didn't request this, please ignore this email.</p>
                    </div>"
            };

            mailMessage.To.Add(toEmail);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
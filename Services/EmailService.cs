using MailKit.Net.Smtp;
using MimeKit;

namespace NageshaJewellers.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendEmail(string toEmail,
                              string subject,
                              string body)
        {
            var email = new MimeMessage();

            email.From.Add(
                MailboxAddress.Parse(
                    _config["EmailSettings:Email"]
                )
            );

            email.To.Add(
                MailboxAddress.Parse(toEmail)
            );

            email.Subject = subject;

            email.Body =
                new TextPart("html")
                {
                    Text = body
                };

            using var smtp =
                new SmtpClient();

            smtp.Connect(
                _config["EmailSettings:Host"],
                int.Parse(
                    _config["EmailSettings:Port"]
                ),
                MailKit.Security.SecureSocketOptions.StartTls
            );

            smtp.Authenticate(
                _config["EmailSettings:Email"],
                _config["EmailSettings:Password"]
            );

            smtp.Send(email);

            smtp.Disconnect(true);
        }
    }
}
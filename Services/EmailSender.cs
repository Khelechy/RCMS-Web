using Microsoft.Extensions.Configuration;
using MimeKit;

namespace RCMS_web.Services
{
    public class EmailSender : IEmailSender
    {
        private string _smtpServer;
        private int _smtpPort;
        private string _fromAddress;
        private string _fromAddressTitle;
        private string _username;
        private string _password;
        private bool _enableSsl;
        private bool _useDefaultCredentials;


        public EmailSender(IConfiguration configuration) // configuration is automatically added to DI in ASP.NET Core 3.0
        {
            _smtpServer = configuration["Email:SmtpServer"];
            _smtpPort = 465;
            _fromAddress = configuration["Email:FromAddress"];
            _fromAddressTitle = configuration["FromAddressTitle"];
            _username = configuration["Email:SmtpUsername"];
            _password = configuration["Email:SmtpPassword"];
            _enableSsl = true;
            _useDefaultCredentials = false;
        }

        public async void Send(string toAddress, string firstname, string subject, string body, bool sendAsync = true)
        {
            var mimeMessage = new MimeMessage(); // MIME : Multipurpose Internet Mail Extension
            MailboxAddress from = new MailboxAddress(_fromAddressTitle, 
            _fromAddress);
            mimeMessage.From.Add(from);
            MailboxAddress to = new MailboxAddress(firstname, toAddress);
            mimeMessage.To.Add(to);
            mimeMessage.Subject = subject;
            var bodyBuilder = new MimeKit.BodyBuilder
            {
                HtmlBody = body
            };
            mimeMessage.Body = bodyBuilder.ToMessageBody();
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(_smtpServer, _smtpPort, _enableSsl);
                client.Authenticate(_username, _password); // If using GMail this requires turning on LessSecureApps : https://myaccount.google.com/lesssecureapps
                if (sendAsync)
                {
                    await client.SendAsync(mimeMessage);
                }
                else
                {
                    client.Send(mimeMessage);
                }
                client.Disconnect(true);
            }

        }
        
    }
}
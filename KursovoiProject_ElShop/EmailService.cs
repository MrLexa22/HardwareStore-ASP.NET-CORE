using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using KursovoiProject_ElShop.Controllers.Validation;
using System.Net;

namespace KursovoiProject_ElShop
{
    public class EmailService
    {
        public static async Task SendEmailRegistrationAsync(string email, string password)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Администрация сайта", "no-reply@mrlexao.ru"));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = "Успешная регистрация на сайте";

                string FilePath = BaseAddresse.Address + "TemplatesMessage/TemplateRegistration.html";
                WebClient clientss = new WebClient();
                string htmlCode = clientss.DownloadString(FilePath);

                htmlCode = htmlCode.Replace("{0}", email);
                htmlCode = htmlCode.Replace("{1}", password);
                emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = htmlCode
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.mail.ru", 465, true);
                    await client.AuthenticateAsync("no-reply@mrlexao.ru", "XMMndD9m3MNmPthYHSwU");
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
            }
            catch { }
        }
    }
}

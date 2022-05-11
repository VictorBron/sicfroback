using BackendOperacionesFroward.Entities.Models;
using BackendOperacionesFroward.Logger;
using BackendOperacionesFroward.Settings.Objects;
using BackendOperacionesFroward.Shared.Utilities;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BackendOperacionesFroward.Shared.Messages
{
    public class Sender
    {
        private static ILogger _logger = LoggerServiceConsole.CreateInstanceLoger("Utils.Sender");

        private static SmtpClient GetSmtpClient() {

            EmailConfiguration emailConfiguration = AppSettings.GetConfigurationOptions(true).Email;

            return new SmtpClient()
            {
                Host = emailConfiguration.SMTP_HOST,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailConfiguration.USERNAME, emailConfiguration.PASSWORD),
                EnableSsl = true,
                Port = emailConfiguration.PORT,
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };
        }

        public static async Task SendMessageRecoverPassword(User user, Token token)
        {

            EmailConfiguration emailConfiguration = AppSettings.GetConfigurationOptions(true).Email;
            MailAddress fromAddress = new(emailConfiguration.USERNAME);
            MailAddress toAddress = new(user.Email);

            SmtpClient client = GetSmtpClient();

            MailMessage message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Portuaria Cabo Froward cambio de contraseña",
                Body = GetBody(user, token),
                IsBodyHtml = true,
                Priority = MailPriority.High,
                SubjectEncoding = Encoding.UTF8
            };

            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex);
            }
        }

        public static async Task SendMessageRegistration(User user, Token token)
        {
            EmailConfiguration emailConfiguration = AppSettings.GetConfigurationOptions(true).Email;

            MailAddress fromAddress = new(emailConfiguration.USERNAME);
            MailAddress toAddress = new(user.Email);

            SmtpClient client = GetSmtpClient();

            MailMessage message = new MailMessage(fromAddress, toAddress)
            {
                IsBodyHtml = true,
                Subject = $"Bienvenido {user.Name} a la Portuaria Cabo Froward",
                Priority = MailPriority.High,
                SubjectEncoding = Encoding.UTF8,
                Body = GetBody(user, token),
            };

            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.WriteLineException(ex);
            }
        }


        private static string GetBody(User user, Token token) {
            FrontInformation FrontInformation = AppSettings.GetConfigurationOptions().FrontInformation;

            string body = $"Bienvenido {user.Login} a la Portuaria Cabo Froward<br><br>";
            body += $"Accede a " +
                $"<a href='{FrontInformation.APPLICATION_URL}{FrontInformation.LOGIN_PAGE}?tokenId={token.TokenCode}'>este enlace</a> " +
                $"para realizar el cambio de contraseña<br>";
            
            body += "<br><br>";
            body += "<label>Teléfonos de contacto: </label>";
            body += "<ul>";
            body += "   <li>Coronel: + (56) 41 218 2201</li>";
            body += "   <li>Calbuco: + (63) 2 211180";
            body += "</ul>";

            body += "<label>Fax: </label>";
            body += "<ul>";
            body += "   <li>(63) 2 211080";
            body += "</ul>";

            body += "<label>Email: </label>";
            body += "<ul>";
            body += "   <li>contacto@froward.cl";
            body += "</ul>";
            return body;
        }
    }
}

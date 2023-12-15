using System.Net.Mail;
using System.Net;

namespace BookHub_Group5.Services
{
    public class EmailService
    {
        public void SendContactEmail(string name, string fromEmail, string subject, string message)
        {
            using (var mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(fromEmail);
                mailMessage.To.Add("bookhubofficialcontact@gmail.com");
                mailMessage.Subject = subject;
                mailMessage.Body = $"Name: {name}\nEmail: {fromEmail}\nSubject: {subject}\nMessage: {message}";

                using (var client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential("bookhubofficialcontact@gmail.com", "onzfuuhzstvorksa");
                    client.EnableSsl = true;

                    client.Send(mailMessage);
                }
            }
        }
    }
}

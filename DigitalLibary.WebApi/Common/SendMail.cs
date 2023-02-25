using System.Net.Mail;

namespace DigitalLibary.WebApi.Common
{
    public class SendMail
    {
        #region FUNCTION
        //FUNCTION SEND MAIL AUTOMATION COMMON
        public void SendMailAuto(string fromMail, string toMail, string password, string body, string Subject)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress(fromMail);
            mail.To.Add(toMail);
            mail.IsBodyHtml = true;
            mail.Subject = Subject;
            mail.Body = body;

            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential(fromMail, password);
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }
        #endregion
    }
}

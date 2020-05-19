using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace ChineseSchool.Utilities
{
    public class MailSender
    {
        protected string _sender = "";
        protected string _password = "";
        public MailSender(string sender, string password)
        {
            _sender = sender;
            _password = password;
        }

        public void SendMail(string recipient, string subject, string message)
        {
            SmtpClient client = new SmtpClient("smtp.sendgrid.net");
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("aquilapharmatech", _password);
            client.EnableSsl = false;
            client.Credentials = credentials;
            var mail = new MailMessage(_sender.Trim(), recipient.Trim());
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            try
            {

                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public void SendMail(string recipient, string subject, string message)
        //{
        //    SmtpClient client = new SmtpClient("mail.chinesecenteroftoledo.org");
        //    client.Port = 587;
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    client.UseDefaultCredentials = false;
        //    System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("CCT", _password);
        //    client.EnableSsl = false;
        //    client.Credentials = credentials;
        //    var mail = new MailMessage(_sender.Trim(), recipient.Trim());
        //    mail.Subject = subject;
        //    mail.Body = message;
        //    mail.IsBodyHtml = true;
        //    try
        //    {

        //        client.Send(mail);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}
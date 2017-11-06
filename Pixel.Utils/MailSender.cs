using System.IO;
using System.Net.Mail;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Pixel.Utils
{
    public class MailSender
    {
        public static void SendMail(string[] to, string subject, XmlDocument bodyXml, string xslPath)
        {
            string innerText;
            try
            {
                XslCompiledTransform compiledTransform = new XslCompiledTransform();
                compiledTransform.Load(xslPath);
                StringBuilder sb = new StringBuilder();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(sb));
                compiledTransform.Transform(bodyXml.CreateNavigator(), null, xmlTextWriter);
                innerText = sb.ToString();
            }
            catch
            {
                innerText = bodyXml.InnerText;
            }
            SendMail(to, subject, innerText);
        }
        public static void SendMail(string[] to, string subject, XmlDocument bodyXml, string xslPath, SmtpClient client)
        {
            string innerText;
            try
            {
                XslCompiledTransform compiledTransform = new XslCompiledTransform();
                compiledTransform.Load(xslPath);
                StringBuilder sb = new StringBuilder();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(new StringWriter(sb));
                compiledTransform.Transform(bodyXml.CreateNavigator(), null, xmlTextWriter);
                innerText = sb.ToString();
            }
            catch
            {
                innerText = bodyXml.InnerText;
            }
            SendMail(to, subject, innerText, client);
        }

        public static void SendMail(string[] to, string subject, string body)
        {
            SmtpClient smtpClient = new SmtpClient();
            using (MailMessage message = new MailMessage())
            {
                message.IsBodyHtml = true;
                message.Body = body;
                foreach (string address in to)
                    message.To.Add(new MailAddress(address));
                message.Subject = subject;
                smtpClient.Send(message);
            }
        }
        public static void SendMail(string[] to, string subject, string body, SmtpClient client)
        {
            SmtpClient smtpClient = client;
            using (MailMessage message = new MailMessage())
            {
                message.IsBodyHtml = true;
                message.Body = body;
                foreach (string address in to)
                    message.To.Add(new MailAddress(address));
                message.Subject = subject;
                smtpClient.Send(message);
            }
        }

        public static void SendMail(string to, string subject, XmlDocument bodyXml, string xslPath)
        {
            SendMail(new string[] { to }, subject, bodyXml, xslPath);
        }
        public static void SendMail(string to, string subject, XmlDocument bodyXml, string xslPath, SmtpClient client)
        {
            SendMail(new string[] { to }, subject, bodyXml, xslPath, client);
        }

        public static void SendMail(string to, string subject, string body)
        {
            SendMail(new string[1] { to }, subject, body);
        }
        public static void SendMail(string to, string subject, string body, SmtpClient client)
        {
            SendMail(new string[1] { to }, subject, body, client);
        }
    }
}

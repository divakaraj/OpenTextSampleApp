using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using System.Data;
using System.Net;

namespace OpenTextSampleApp
{
    class EMailer
    {
        /// <summary>
        /// Logic to send mail with the contents
        /// </summary>
        /// <param name="fromName"></param>
        /// <param name="fromAddress"></param>
        /// <param name="toAddresses"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="serverName"></param>
        /// <param name="attachmentFilePath"></param>
        /// <returns></returns>
        public static bool SendMail(string fromName, string fromAddress, string toAddresses, string subject, string body, string serverName, string attachmentFilePath)
        {
            try
            {
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["Mail.Port"]);

                SmtpClient Client = new SmtpClient(serverName, Port);
                MailAddress From = new MailAddress(fromAddress, fromName);

                MailMessage Mail = new MailMessage(fromAddress, toAddresses);

                Mail.From = From;
                Mail.Subject = subject;
                Mail.Body = body;

                if (attachmentFilePath.Length > 0)
                {
                    Attachment data = new Attachment(attachmentFilePath);
                    Mail.Attachments.Add(data);
                }

                string IsAuthenticationRequired = ConfigurationManager.AppSettings["Mail.IsAuthenticationRequired"];
                string UserName = ConfigurationManager.AppSettings["Mail.UserName"];
                string Password = ConfigurationManager.AppSettings["Mail.Password"];
                if (IsAuthenticationRequired == "Y")
                {
                    if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                    {
                        throw new ArgumentException("Provide UserName/Password credentials for mail server in config file or set IsAuthenticationRequired = 'N' ");
                    }
                    Client.Credentials = new NetworkCredential(UserName, Password);
                }
                else
                {
                    Client.UseDefaultCredentials = true;
                }
                Client.Send(Mail);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

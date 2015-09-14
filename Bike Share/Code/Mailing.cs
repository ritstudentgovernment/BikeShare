using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace BikeShare.Code
{
    public class MailItem
    {
        public MailItem()
        {
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
        }
        public List<String> To { get; set; }
        public List<String> Cc { get; set; }
        public List<String> Bcc { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public bool isHtml { get; set; }
        public string attachmentPath { get; set; }
    }
    public static class Mailing
    {

        static Mailing()
        {
            mailQueue = new Queue<MailItem>();
        }
        private static Queue<MailItem> mailQueue {get; set;}
        public static void queueMail(MailItem mail)
        {
            Mailing.mailQueue.Enqueue(mail);
        }
        public static void queueCheckoutNotice(string emailTo, DateTime dueDate)
        {
            var mail = new MailItem();

            mail.To.Add(emailTo);
            mail.Subject = "Bike Checked Out";
            mail.Body = "Thank you for checking out a bike! You have the bike for 24 hours. Enjoy your ride and be safe!";
            if (DateTime.Now.ToString("ddd") == "Fri")
            {
                mail.Body = "Thank you for checking out a bike! You have the bike until Monday morning. Enjoy your ride and be safe!";
            }
            mail.isHtml = false;
            queueMail(mail);
        }

        public static void queueRegistrationNotice(string emailTo, string programNotice, string legalNotice, string legalPDFPath, int registrationDays, string phone, string firstName, string lastName, string appName)
        {
            var mail = new MailItem();
            mail.To.Add(emailTo);
            mail.Subject = appName + " Registration Confirmation";
            mail.isHtml = true;
            mail.Body += "<div style=\"text-align: center; font-size: 24pt\">" + appName + " Registration Confirmation</div>";
            mail.Body += "<div style=\"font-size: 14pt\">This email serves as confirmation of your registration with this bike sharing program and your agreement to the terms and conditions of the service.</div>";
            mail.Body += "<div style=\"text-align: center; font-size: 24pt\">Registration Details</div>";
            mail.Body += "<div style=\"font-size: 14pt\"><strong>Registration Date:</strong> " + DateTime.Now.ToShortDateString() + " </div>";
            mail.Body += "<div style=\"font-size: 14pt\"><strong>First Name:</strong> " + firstName + " </div>";
            mail.Body += "<div style=\"font-size: 14pt\"><strong>Last Name:</strong> " + lastName + " </div>";
            mail.Body += "<div style=\"font-size: 14pt\"><strong>Phone Number:</strong> " + phone + " </div>";
            mail.Body += "<div style=\"font-size: 14pt\"><strong>Registration Expiration:</strong> " + DateTime.Now.AddDays(registrationDays).ToShortDateString() + " </div>";
            mail.Body += "<div style=\"text-align: center; font-size: 24pt\">Program Details</div>";
            mail.Body += "<div style=\"font-size: 14pt\">" + programNotice + " </div>";
            mail.Body += "<div style=\"text-align: center; font-size: 24pt\">Legal/Liability Requirements</div>";
            mail.Body += "<div style=\"font-size: 14pt\">" + legalNotice + " </div>"; 
            mail.Body += "<hr />";
            mail.Body += "<p>Thanks, and have many safe and enjoyable rides!</p>";
            mail.attachmentPath = legalPDFPath;
            queueMail(mail);
        }

        public static void queueRegistrationTerminationNotice(string emailTo, string appName)
        {
            var mail = new MailItem();
            mail.To.Add(emailTo);
            mail.Subject = appName + " Registration Expiration";
            mail.isHtml = true;
            mail.Body += "<div style=\"text-align: center; font-size: 24pt\">" + appName + " Registration Expiration Notice</div>";
            mail.Body += "<div style=\"font-size: 14pt\">Your registration has expired. Please use the website to renew your registration. You will not be able to borrow bikes until you renew your registration.</div>";
            mail.Body += "<hr />";
            mail.Body += "<p>Thanks, and have many safe and enjoyable rides!</p>";
            queueMail(mail);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void sendMail()
        {
            
            using(var server = new SmtpClient())
            {
                while(mailQueue.Count > 0)
                {
                    var mail = mailQueue.Dequeue();
                    MailMessage message = new MailMessage();
                    mail.To.ForEach(m => message.To.Add(m));
                    message.Subject = mail.Subject;
                    message.Body = mail.Body;
                    message.IsBodyHtml = mail.isHtml;
                    mail.Cc.ForEach(m => message.CC.Add(m));
                    mail.Bcc.ForEach(m => message.Bcc.Add(m));
                    message.From = new MailAddress("sgnoreply@rit.edu");
                    if (!String.IsNullOrWhiteSpace(mail.attachmentPath))
                    {
                        Attachment attach = new Attachment(mail.attachmentPath);
                        message.Attachments.Add(attach);
                    }
                    server.Send(message);
                }
            }
            
        }
    }
}

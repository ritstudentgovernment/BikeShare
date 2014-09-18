using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Repositories;
using BikeShare.Models;
using FluentScheduler;
using System.Configuration;
using System.Net.Mail;

namespace BikeShare.Code.Mailers
{
    public class DueSoonBikes : ITask
    {
        public void Execute()
        {
            string emailFrom = ConfigurationSettings.AppSettings["emailFrom"];
            string pass = ConfigurationSettings.AppSettings["emailPass"];
            string user = ConfigurationSettings.AppSettings["emailUser"];
            string server = ConfigurationSettings.AppSettings["emailServer"];
            int port = Convert.ToInt32(ConfigurationSettings.AppSettings["emailPort"]);

            SmtpClient smtpServer = new SmtpClient(server);
            smtpServer.Credentials = new System.Net.NetworkCredential(user, pass);
            smtpServer.Port = port; 
            AdminDbRepository aRepo = new AdminDbRepository();
            SettingsDbRepository sRepo = new SettingsDbRepository();
            string appName = sRepo.getappName();
            int rentDays = sRepo.getmaxRentDays();
            foreach(var checkout in aRepo.getAllCurrentCheckOuts())
            {
                if (checkout.timeOut.AddHours(2).AddDays(rentDays) > DateTime.Now && checkout.timeOut.AddDays(rentDays) < DateTime.Now)
                {
                    MailMessage mail = new MailMessage();


                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(checkout.user.email);
                    mail.Subject = "Bike Due Soon - " + appName;
                    mail.Body = "Thank you for using the " + appName + ". You rented a bike on " + checkout.timeOut.ToShortDateString() + " at " + checkout.timeOut.ToShortTimeString() + 
                        ". Per our policy, your bike will be due within the next two hours. Please return your bike as soon as possible.";
                    if (DateTime.Now.ToString("ddd") == "Fri" && DateTime.Now.Hour > 17)
                    {
                        mail.Body = "Thank you for using the " + appName + ". You rented a bike on " + checkout.timeOut.ToShortDateString() + " at " + checkout.timeOut.ToShortTimeString() +
                        ". Per our policy, your bike will be due next business day. Please return your bike as soon as possible.";
                    }
                    if (DateTime.Now.ToString("ddd") == "Sat" || DateTime.Now.ToString("ddd") == "Sun")
                    {
                        mail.Body = "Thank you for using the " + appName + ". You rented a bike on " + checkout.timeOut.ToShortDateString() + " at " + checkout.timeOut.ToShortTimeString() +
                        ". Per our policy, your bike will be due next business day. Please return your bike as soon as possible.";
                    }
                    smtpServer.Send(mail);
                }
            }
        }
    }

    public class overDueBikes : ITask
    {

        public void Execute()
        {
            string emailFrom = ConfigurationSettings.AppSettings["emailFrom"];
            string pass = ConfigurationSettings.AppSettings["emailPass"];
            string user = ConfigurationSettings.AppSettings["emailUser"];
            string server = ConfigurationSettings.AppSettings["emailServer"];
            int port = Convert.ToInt32(ConfigurationSettings.AppSettings["emailPort"]);

            SmtpClient smtpServer = new SmtpClient(server);
            smtpServer.Credentials = new System.Net.NetworkCredential(user, pass);
            smtpServer.Port = port;
            AdminDbRepository aRepo = new AdminDbRepository();
            SettingsDbRepository sRepo = new SettingsDbRepository();
            string appName = sRepo.getappName();
            int rentDays = sRepo.getmaxRentDays();
            foreach (var checkout in aRepo.getAllCurrentCheckOuts())
            {
                if (checkout.timeOut.AddDays(rentDays) < DateTime.Now)
                {
                    MailMessage mail = new MailMessage();


                    mail.From = new MailAddress(emailFrom);
                    mail.To.Add(checkout.user.email);
                    mail.Subject = "Bike Now Overdue - " + appName;
                    mail.Body = "Thank you for using the " + appName + ". You rented a bike on " + checkout.timeOut.ToShortDateString() + " at " + checkout.timeOut.ToShortTimeString() +
                        ". Per our policy, your bike is now overdue, and failure to return it in a timely manner may result in charges to your account. Please return your bike as soon as possible.";
                    if (DateTime.Now.ToString("ddd") == "Fri" && DateTime.Now.Hour > 17)
                    {
                        mail.Body = "Thank you for using the " + appName + ". You rented a bike on " + checkout.timeOut.ToShortDateString() + " at " + checkout.timeOut.ToShortTimeString() +
                        ". Per our policy, your bike will be due at the start of business on the next business day. Failure to return  your bike in a timely manner may result in charges to your account. Please return your bike as soon as possible.";
                    }
                    if (DateTime.Now.ToString("ddd") == "Sat" || DateTime.Now.ToString("ddd") == "Sun")
                    {
                        mail.Body = "Thank you for using the " + appName + ". You rented a bike on " + checkout.timeOut.ToShortDateString() + " at " + checkout.timeOut.ToShortTimeString() +
                        ". Per our policy, your bike will be due at the start of business on the next business day. Failure to return  your bike in a timely manner may result in charges to your account. Please return your bike as soon as possible.";
                    }
                    smtpServer.Send(mail);
                }
            }
        }
    }

    public class chargeReminder : ITask
    {

        public void Execute()
        {
            string emailFrom = ConfigurationSettings.AppSettings["emailFrom"];
            string pass = ConfigurationSettings.AppSettings["emailPass"];
            string user = ConfigurationSettings.AppSettings["emailUser"];
            string server = ConfigurationSettings.AppSettings["emailServer"];
            int port = Convert.ToInt32(ConfigurationSettings.AppSettings["emailPort"]);

            SmtpClient smtpServer = new SmtpClient(server);
            smtpServer.Credentials = new System.Net.NetworkCredential(user, pass);
            smtpServer.Port = port;
            SettingsDbRepository sRepo = new SettingsDbRepository();
            FinanceDbRepository fRepo = new FinanceDbRepository();
            string appName = sRepo.getappName();
            foreach(var charge in fRepo.getUnresolvedCharges())
            {
                MailMessage mail = new MailMessage();


                mail.From = new MailAddress(emailFrom);
                mail.To.Add(charge.user.email);
                mail.Subject = "Account Balance Reminder - " + appName;
                mail.Body = "Thank you for using the " + appName + ". There is currently a pending charge on your account. Title: " + charge.title + " Description: " + charge.description  +
                    ". The amount of the charge is $" + charge.amountCharged + ". Please review your account on our website for more details.";
                smtpServer.Send(mail);
            }
        }
    }
}
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
            SmtpClient smtpServer = new SmtpClient();
            AdminDbRepository aRepo = new AdminDbRepository();
            SettingsDbRepository sRepo = new SettingsDbRepository();
            string appName = sRepo.getappName();
            int rentDays = sRepo.getmaxRentDays();
            foreach(var checkout in aRepo.getAllCurrentCheckOuts())
            {
                if (checkout.timeOut.AddHours(2).AddDays(rentDays) > DateTime.Now && checkout.timeOut.AddDays(rentDays) < DateTime.Now)
                {
                    MailMessage mail = new MailMessage();
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
            if((DateTime.Now.ToString("ddd") == "Fri" && DateTime.Now.Hour > 17) || DateTime.Now.ToString("ddd") == "Sat" || DateTime.Now.ToString("ddd") == "Sun")
            {
                return;
            }

            SmtpClient smtpServer = new SmtpClient();
            AdminDbRepository aRepo = new AdminDbRepository();
            SettingsDbRepository sRepo = new SettingsDbRepository();
            string appName = sRepo.getappName();
            int rentDays = sRepo.getmaxRentDays();
            foreach (var checkout in aRepo.getAllCurrentCheckOuts())
            {
                if (checkout.timeOut.AddDays(rentDays) < DateTime.Now)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(checkout.user.email);
                    mail.Subject = "Bike Now Overdue - " + appName;
                    mail.Body = "Thank you for using the " + appName + ". You rented a bike on " + checkout.timeOut.ToShortDateString() + " at " + checkout.timeOut.ToShortTimeString() +
                        ". Per our policy, your bike is now overdue, and failure to return it in a timely manner may result in charges to your account. Please return your bike as soon as possible.";
                    smtpServer.Send(mail);
                }
            }
        }
    }

    public class chargeReminder : ITask
    {

        public void Execute()
        {
            SmtpClient smtpServer = new SmtpClient();
            SettingsDbRepository sRepo = new SettingsDbRepository();
            FinanceDbRepository fRepo = new FinanceDbRepository();
            string appName = sRepo.getappName();
            foreach(var charge in fRepo.getUnresolvedCharges())
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(charge.user.email);
                mail.Subject = "Account Balance Reminder - " + appName;
                mail.Body = "Thank you for using the " + appName + ". There is currently a pending charge on your account. Title: " + charge.title + " Description: " + charge.description  +
                    ". The amount of the charge is $" + charge.amountCharged + ". Please review your account on our website for more details.";
                smtpServer.Send(mail);
            }
        }
    }
}
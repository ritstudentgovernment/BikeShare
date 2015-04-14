using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;
using FluentScheduler;
using System.Configuration;
using System.Net.Mail;

namespace BikeShare.Code.Mailers
{
    public class AdminMailing : ITask
    {
        public void Execute()
        {
            using(var context = new BikesContext())
            {
                var set = context.settings.First();
                SmtpClient smtpServer = new SmtpClient();
                string appName = set.appName;
                int rentDays = set.maxRentDays;
                List<CheckOut> checks = context.CheckOut.Where(i => !i.isResolved).Where(m => m.timeOut.AddDays(rentDays) < DateTime.Now).ToList();

                MailMessage mail = new MailMessage();
                foreach (var s in set.adminEmailList.Split(','))
                {
                    mail.To.Add(s);
                }
                mail.Subject = "Admin Bike Report - " + appName;
                mail.IsBodyHtml = true;
                mail.Body += "<div style=\"text-align: center; font-size: 24pt\">" + appName + " Admin Mailing</div>";
                mail.Body += "\n<div style=\"text-align: center; font-size: 20pt; color: gray;\">" + checks.Count().ToString() + " Overdue Bikes</div>";
                mail.Body += "<table><tr><th>Bike Number</th><th>Rental Date</th><th>User Name</th><th>Real Name</th><th>Phone Number</th></tr>";
                foreach (var check in checks)
                {
                    mail.Body += "<tr><td>" + check.bike.bikeNumber.ToString() + "</td><td>" + check.timeOut.ToString() + "</td><td>" + check.user.userName + "</td><td>" + check.user.firstName + " " + check.user.lastName + "</td><td>" + check.user.phoneNumber + "</td></tr>";
                }
                mail.Body += "</table>";
                smtpServer.Send(mail);
            }
        }
    }
    public class DueSoonBikes : ITask
    {
        public void Execute()
        {
            using(var context = new BikesContext())
            {
                SmtpClient smtpServer = new SmtpClient();
                var set = context.settings.First();
                string appName = set.appName;
                int rentDays = set.maxRentDays;
                foreach (var checkout in context.CheckOut.Where(i => !i.isResolved))
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
    }

    public class overDueBikes : ITask
    {

        public void Execute()
        {
            if ((DateTime.Now.ToString("ddd") == "Fri" && DateTime.Now.Hour > 17) || DateTime.Now.ToString("ddd") == "Sat" || DateTime.Now.ToString("ddd") == "Sun" || (DateTime.Now.ToString("ddd") == "Mon" && DateTime.Now.Hour < 9))
            {
                return;
            }

            SmtpClient smtpServer = new SmtpClient();
            
            using(var context = new BikesContext())
            {
                var set = context.settings.First();
                string appName = set.appName;
                int rentDays = set.maxRentDays;
                foreach (var checkout in context.CheckOut.Where(i => !i.isResolved))
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
    }

    public class chargeReminder : ITask
    {
        public void Execute()
        {

            SmtpClient smtpServer = new SmtpClient();
            using (var context = new BikesContext())
            {
                string appName = context.settings.First().appName;
                foreach (var charge in context.Charge.Where(i => !i.isResolved))
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(charge.user.email);
                    mail.Subject = "Account Balance Reminder - " + appName;
                    mail.Body = "Thank you for using the " + appName + ". There is currently a pending charge on your account. Title: " + charge.title + " Description: " + charge.description +
                        ". The amount of the charge is $" + charge.amountCharged + ". Please review your account on our website for more details.";
                    smtpServer.Send(mail);
                    charge.notificationsCounter += 1;
                }
                context.SaveChanges();
            }
            
        }
    }
}
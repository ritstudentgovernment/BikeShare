using BikeShare.Models;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BikeShare.Code.ScheduledTasks
{
    public class AdminMailing : ITask
    {
        public void Execute()
        {
            using (var context = new BikesContext())
            {
                var set = context.settings.First();
                string appName = set.appName;
                int rentDays = set.maxRentDays;
                DateTime threshold = DateTime.Now.Subtract(new TimeSpan(rentDays, 0, 0, 0));
                List<CheckOut> checks = context.CheckOut.Where(i => !i.isResolved).Where(m => m.timeOut < threshold).ToList();

                var mail = new MailItem();
                foreach (var s in set.adminEmailList.Split(','))
                {
                    mail.To.Add(s);
                }
                mail.Subject = "Admin Bike Report - " + appName;
                mail.isHtml = true;
                mail.Body += "<div style=\"text-align: center; font-size: 24pt\">" + appName + " Admin Mailing</div>";
                mail.Body += "\n<div style=\"text-align: center; font-size: 20pt; color: gray;\">" + checks.Count().ToString() + " Overdue Bikes</div>";
                mail.Body += "<table><tr><th>Bike Number</th><th>Rental Date</th><th>User Name</th><th>Real Name</th><th>Phone Number</th></tr>";
                foreach (var check in checks)
                {
                    var user = context.BikeUser.Find(check.rider);
                    mail.Body += "<tr><td>" + context.Bike.Find(check.bike).bikeNumber.ToString() + "</td><td>" + check.timeOut.ToString() + "</td><td>" + user.userName + "</td><td>" + user.firstName + " " + user.lastName + "</td><td>" + user.phoneNumber + "</td></tr>";
                }
                mail.Body += "</table>";
                Mailing.queueMail(mail);
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

            using (var context = new BikesContext())
            {
                var set = context.settings.First();
                string appName = set.appName;
                int rentDays = set.maxRentDays;
                foreach (var checkout in context.CheckOut.Where(i => !i.isResolved).ToList())
                {
                    if (checkout.timeOut.AddDays(rentDays) < DateTime.Now)
                    {
                        MailItem mail = new MailItem();
                        mail.To.Add(context.BikeUser.Find(checkout.rider).email);
                        mail.Subject = "Bike Now Overdue - " + appName;
                        mail.Body = "Thank you for using the " + appName + ". You rented a bike on " + checkout.timeOut.ToShortDateString() + " at " + checkout.timeOut.ToShortTimeString() +
                            ". Per our policy, your bike is now overdue, and failure to return it in a timely manner may result in charges to your account. Please return your bike as soon as possible.";
                        Mailing.queueMail(mail);
                    }
                }
            }
        }
    }

    public class chargeReminder : ITask
    {
        public void Execute()
        {
            using (var context = new BikesContext())
            {
                string appName = context.settings.First().appName;
                foreach (var charge in context.Charge.Where(i => !i.isResolved))
                {
                    var mail = new MailItem();
                    mail.To.Add(charge.user.email);
                    mail.Subject = "Account Balance Reminder - " + appName;
                    mail.Body = "Thank you for using the " + appName + ". There is currently a pending charge on your account. Title: " + charge.title + " Description: " + charge.description +
                        ". The amount of the charge is $" + charge.amountCharged + ". Please review your account on our website for more details.";
                    mail.isHtml = false;
                    Mailing.queueMail(mail);
                    charge.notificationsCounter += 1;
                }
                context.SaveChanges();
            }
        }
    }

    public class flushMail : ITask
    {
        public void Execute() { Mailing.sendMail(); }
    }
}
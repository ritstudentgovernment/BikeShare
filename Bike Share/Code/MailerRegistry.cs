using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentScheduler;

namespace BikeShare.Code
{
    public class MailerRegistry : Registry
    {
        public MailerRegistry()
        {
            Schedule<BikeShare.Code.Mailers.DueSoonBikes>().ToRunEvery(15).Minutes();
            Schedule<BikeShare.Code.Mailers.chargeReminder>().ToRunEvery(1).Days().At(6, 45);
            Schedule<BikeShare.Code.Mailers.overDueBikes>().ToRunEvery(3).Hours().At(0);
        }
    }
}
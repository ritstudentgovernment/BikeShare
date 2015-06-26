using FluentScheduler;

namespace BikeShare.Code
{
    public class MailerRegistry : Registry
    {
        public MailerRegistry()
        {
            Schedule<BikeShare.Code.Mailers.chargeReminder>().ToRunEvery(1).Days().At(6, 45);
            Schedule<BikeShare.Code.Mailers.overDueBikes>().ToRunEvery(1).Days().At(16, 30);
            Schedule<BikeShare.Code.Mailers.AdminMailing>().ToRunEvery(1).Days().At(16, 30);
            Schedule<BikeShare.Code.Mailers.InspectionSchedule>().ToRunNow().AndEvery(15).Minutes();
        }
    }
}
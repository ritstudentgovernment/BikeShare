using FluentScheduler;

namespace BikeShare.Code
{
    public class SceduleRegistry : Registry
    {
        public SceduleRegistry()
        {
            Schedule<BikeShare.Code.ScheduledTasks.chargeReminder>().ToRunEvery(1).Days().At(6, 45);
            Schedule<BikeShare.Code.ScheduledTasks.overDueBikes>().ToRunEvery(1).Days().At(16, 30);
            Schedule<BikeShare.Code.ScheduledTasks.AdminMailing>().ToRunEvery(1).Days().At(16, 30);
            Schedule<BikeShare.Code.ScheduledTasks.InspectionSchedule>().ToRunNow().AndEvery(15).Minutes();
            Schedule<BikeShare.Code.ScheduledTasks.flushMail>().ToRunNow().AndEvery(10).Seconds();
        }
    }
}
using System;
using FluentScheduler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BikeShare.Models;

namespace BikeShare.Code.Mailers
{
    public class InspectionSchedule : ITask
    {
        public void Execute()
        {
            using (var context = new BikesContext())
            {
                foreach(var schedule in context.schedules.Where(d => d.start < DateTime.Now).Where(d => d.end > DateTime.Now).ToList())
                {
                    if(schedule.lastRun.DayOfYear == DateTime.Now.DayOfYear)
                        continue;
                    if (schedule.hour != DateTime.Now.Hour)
                        continue;
                    foreach(string bikeNumber in schedule.affectedBikes.Split(','))
                    {
                        int number;
                        if (!int.TryParse(bikeNumber, out number))
                            continue;
                        var bikes = context.Bike.Where(b => b.bikeNumber == number);
                        if (context.Bike.Where(b => b.bikeNumber == number).Count() != 1)
                            continue;
                        else
                            bikes.First().onInspectionHold = true;
                    }

                    schedule.lastRun = DateTime.Now;
                    
                }
                context.SaveChanges();
            }
        }
    }
}

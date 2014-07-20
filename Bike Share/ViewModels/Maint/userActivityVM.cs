using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;

namespace BikeShare.ViewModels.Maint
{
    public class userActivityVM
    {
        public PageInfo activtyPage { get; set; }
        public PageInfo hoursPage { get; set; }
        public List<WorkHour> hours { get; set; }
        public List<ActivityCard> cards { get; set; }
    }
}
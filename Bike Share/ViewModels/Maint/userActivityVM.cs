using BikeShare.Models;
using System.Collections.Generic;

namespace BikeShare.ViewModels.Maint
{
    public class userActivityVM
    {
        public PageInfo activtyPage { get; set; }

        public PageInfo hoursPage { get; set; }

        public List<ActivityCard> cards { get; set; }
    }
}
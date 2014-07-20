using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BikeShare.ViewModels
{
    public class appSettingsViewModel
    {
        public string appName { get; set; }
        public int rentPeriodMaxDays { get; set; }
        public int maxBalance { get; set; }
    }
}
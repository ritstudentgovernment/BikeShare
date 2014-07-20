using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;

namespace BikeShare.ViewModels
{
    public class FilteredBikeViewModel : PaginatedViewModel<Bike>
    {
        public bool includeMissing { get; set; }
        public bool includeOverdue { get; set; }
        public bool includeCheckedOut { get; set; }
        public bool includeCheckedIn { get; set; }
        public string filterMaintainer { get; set; }
        public string filterRenter { get; set; }
        public string filterCheckOutPerson { get; set; }
        public bool includeCurrent { get; set; }
        public bool includeArchived { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BikeShare.Models;

namespace BikeShare.ViewModels
{
    public class inspectionListViewModel
    {
        public List<Inspection> inspections { get; set; }
        public PageInfo pagingInfo { get; set; }
    }
}
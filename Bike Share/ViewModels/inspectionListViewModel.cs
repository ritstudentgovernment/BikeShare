using BikeShare.Models;
using System.Collections.Generic;

namespace BikeShare.ViewModels
{
    public class inspectionListViewModel
    {
        public List<Inspection> inspections { get; set; }

        public PageInfo pagingInfo { get; set; }
    }
}
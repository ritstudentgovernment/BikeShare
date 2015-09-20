using System.Collections.Generic;

namespace BikeShare.ViewModels
{
    public class PaginatedViewModel<T> where T : class
    {
        public List<T> modelList { get; set; }

        public PageInfo pagingInfo { get; set; }

        public PaginatedViewModel()
        {
            this.modelList = new List<T>();
        }

        public PaginatedViewModel(int totalItems, int itemsPerPage)
        {
            this.modelList = new List<T>();
            this.pagingInfo = new PageInfo { TotalItems = totalItems, ItemsPerPage = itemsPerPage };
        }
    }
}
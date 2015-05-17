using System;

namespace BikeShare.ViewModels
{
    public class PageInfo
    {
        public PageInfo(int totalItems, int itemsPerPage, int currentPage)
        {
            this.TotalItems = totalItems;
            this.ItemsPerPage = itemsPerPage;
            this.CurrentPage = currentPage;
        }

        public PageInfo()
        {
        }

        public int TotalItems { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }
    }
}
using System;

namespace BikeShare.ViewModels
{
    public class ActivityCard : IComparable
    {
        public cardStatus status { get; set; }

        public string title { get; set; }

        public DateTime date { get; set; }

        public int id { get; set; }

        public activityType type;

        public string userName { get; set; }

        public int userId { get; set; }

        public int CompareTo(object obj)
        {
            ActivityCard card = (ActivityCard)obj;
            if (this.date > card.date)
            {
                return 1;
            }
            if (this.date < card.date)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public string getStatusStyle()
        {
            switch (this.status)
            {
                case cardStatus.defaults:
                    return "default";

                case cardStatus.disabled:
                    return "disabled";

                case cardStatus.danger:
                    return "danger";

                case cardStatus.primary:
                    return "primary";

                case cardStatus.success:
                    return "success";

                default:
                    return "default";
            }
        }
    }

    public class bikeCard
    {
        public cardStatus status {get; set;}

        public string bikeName { get; set; }

        public int bikeId { get; set; }

        public int bikeNumber { get; set; }

        public int rackId { get; set; }

        public string rackName { get; set; }

        public int totalCheckouts { get; set; }

        public int totalInspections { get; set; }

        public int totalMaintenance { get; set; }

        public string dateLastInspected { get; set; }

        public string dateLastMaintenance { get; set; }

        public string getStatusStyle()
        {
            switch (this.status)
            {
                case cardStatus.defaults:
                    return "default";

                case cardStatus.disabled:
                    return "disabled";

                case cardStatus.danger:
                    return "danger";

                case cardStatus.primary:
                    return "primary";

                case cardStatus.success:
                    return "success";

                default:
                    return "default";
            }
        }
    }

    public enum cardStatus { success, danger, primary, defaults, disabled }

    public enum activityType { maintenance, inspection, comment, work, checkout, charge, admin }
}
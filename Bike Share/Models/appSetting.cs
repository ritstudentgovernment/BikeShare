using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace BikeShare.Models
{
    public class appSetting
    {
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int settingId { get; set; }

        public string appName { get; set; }

        public string expectedEmail { get; set; }

        public string adminEmailList { get; set; }

        public int maxRentDays { get; set; }

        public int DaysBetweenInspections { get; set; }

        public int daysBetweenRegistrations { get; set; }

        public int overdueBikeMailingIntervalHours { get; set; }

        [AllowHtml]
        public string footerHTML { get; set; }

        [AllowHtml]
        public string homeHTML { get; set; }

        [AllowHtml]
        public string announcementHTML { get; set; }

        [AllowHtml]
        public string FAQHTML { get; set; }

        [AllowHtml]
        public string contactHTML { get; set; }

        [AllowHtml]
        public string aboutHTML { get; set; }

        [AllowHtml]
        public string safetyHTML { get; set; }

        [AllowHtml]
        public string registerHTML { get; set; }

        [AllowHtml]
        public string legalHTML { get; set; }

        [AllowHtml]
        public string programHTML { get; set; }

        [AllowHtml]
        public int? latestPDFNumber { get; set; }
    }
}
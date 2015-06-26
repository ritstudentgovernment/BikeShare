using System;
using System.Data.Entity;

namespace BikeShare.Models
{
    public class BikesContext : DbContext
    {
        public DbSet<Bike> Bike { get; set; }

        public DbSet<BikeRack> BikeRack { get; set; }

        public DbSet<bikeUser> BikeUser { get; set; }

        public DbSet<CheckOut> CheckOut { get; set; }

        public DbSet<MaintenanceEvent> MaintenanceEvent { get; set; }

        public DbSet<MaintenanceUpdate> MaintenanceUpdate { get; set; }

        public DbSet<Inspection> Inspection { get; set; }

        public DbSet<Charge> Charge { get; set; }

        public DbSet<WorkHour> workHours { get; set; }

        public DbSet<appSetting> settings { get; set; }
        public DbSet<ScheduledInspection> schedules { get; set; }

        public BikesContext()
            : base("name=BikesDatabase")
        {
            Database.SetInitializer<BikesContext>(new BaseDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public class BaseDbInitializer : CreateDatabaseIfNotExists<BikesContext>
        {
            protected override void Seed(BikesContext context)
            {
                bikeUser defaultAdmin = new bikeUser() { userName = "sgsvcs", lastRegistered = DateTime.Now, email = "sgsvcs@rit.edu", isArchived = false, canAdministerSite = true, canMaintainBikes = true, canCheckOutBikes = true, canBorrowBikes = true };
                appSetting defaultSetting = new appSetting()
                {
                    adminEmailList = "sgsvcs@rit.edu",
                    appName = "RIT Bikeshare",
                    DaysBetweenInspections = 5,
                    daysBetweenRegistrations = 180,
                    expectedEmail = "@rit.edu",
                    maxRentDays = 1,
                    overdueBikeMailingIntervalHours = 4,
                    footerHTML = "<footer>Footer goes here.</footer>",
                    homeHTML = "<h3>Welcome to the Bikeshare. Customize home</h3>",
                    announcementHTML = ""
                };

                context.settings.Add(defaultSetting);
                context.BikeUser.Add(defaultAdmin);

                base.Seed(context);
            }
        }
    }
}
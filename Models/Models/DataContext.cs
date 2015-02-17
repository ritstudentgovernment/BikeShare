using System.Data.Entity;
using System.Linq;
using System;

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
        public DbSet<MailSub> MailSub { get; set; }
        public DbSet<Workshop> WorkShop { get; set; }
        public DbSet<Inspection> Inspection { get; set; }
        public DbSet<Charge> Charge { get; set; }
        public DbSet<Tracer> tracer { get; set; }
        public DbSet<WorkHour> workHours { get; set; }
        public DbSet<Hour> hour { get; set; }
        public DbSet<appSetting> settings { get; set; }
        public BikesContext() : base("name=BikesDatabase")
        {
            Database.SetInitializer<BikesContext>(new BaseDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bike>().HasRequired(p => p.bikeRack).WithMany(b => b.bikes).WillCascadeOnDelete(false);
            modelBuilder.Entity<MaintenanceEvent>().HasRequired(p => p.staffPerson).WithMany(m => m.maintenanceEvents).WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }
        public class BaseDbInitializer : CreateDatabaseIfNotExists<BikesContext>
        {
            protected override void Seed(BikesContext context)
            {
                Workshop homeWorkshop = new Workshop() { name = "Default", isArchived = false, GPSCoordX = 0, GPSCoordY = 0 };
                bikeUser defaultAdmin = new bikeUser() { userName = "sgsvcs", lastRegistered = DateTime.Now, email = "sgsvcs@rit.edu", isArchived = false, hasBike = false, canAdministerSite = true, canMaintainBikes = true, canCheckOutBikes = true, canBorrowBikes = true };
                appSetting defaultSetting = new appSetting() { adminEmailList = "sgsvcs@rit.edu", appName = "RIT Bikeshare", DaysBetweenInspections = 5, daysBetweenRegistrations = 180, expectedEmail = "@rit.edu", maxRentDays = 1, overdueBikeMailingIntervalHours = 4 };

                context.settings.Add(defaultSetting);
                context.BikeUser.Add(defaultAdmin);
                context.WorkShop.Add(homeWorkshop);

                base.Seed(context);
            }
        }

    }
}
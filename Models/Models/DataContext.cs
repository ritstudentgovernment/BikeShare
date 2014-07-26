using System.Data.Entity;
using System.Linq;

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
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bike>().HasRequired(p => p.bikeRack).WithMany(b => b.bikes).WillCascadeOnDelete(false);
            modelBuilder.Entity<MaintenanceEvent>().HasRequired(p => p.staffPerson).WithMany(m => m.maintenanceEvents).WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);

        }
                //base.Seed(context);
                //var setting = new appSetting();
                //setting.appName = "Bike Share";
                //setting.DaysBetweenInspections = 7;
                //setting.expectedEmail = "@rit.edu";
                //setting.maxRentDays = 1;
                //context.settings.Add(setting);
                //context.SaveChanges();

    }
}
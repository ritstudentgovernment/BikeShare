namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedSchedules : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ScheduledInspections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        day = c.Int(nullable: false),
                        start = c.DateTime(nullable: false),
                        end = c.DateTime(nullable: false),
                        affectedBikes = c.String(),
                        hour = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ScheduledInspections");
        }
    }
}

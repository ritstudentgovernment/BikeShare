namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeworkshop : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Inspections", "placeInspectedId");
            DropColumn("dbo.MaintenanceEvents", "workshopId");
            DropTable("dbo.Workshops");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Workshops",
                c => new
                    {
                        workshopId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        GPSCoordX = c.Single(nullable: false),
                        GPSCoordY = c.Single(nullable: false),
                        isArchived = c.Boolean(nullable: false),
                        hours = c.String(),
                    })
                .PrimaryKey(t => t.workshopId);
            
            AddColumn("dbo.MaintenanceEvents", "workshopId", c => c.Int(nullable: false));
            AddColumn("dbo.Inspections", "placeInspectedId", c => c.Int(nullable: false));
        }
    }
}

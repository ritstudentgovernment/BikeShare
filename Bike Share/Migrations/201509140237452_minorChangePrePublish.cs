namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorChangePrePublish : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Bikes", "bikeRackId");
            AddForeignKey("dbo.Bikes", "bikeRackId", "dbo.BikeRacks", "bikeRackId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bikes", "bikeRackId", "dbo.BikeRacks");
            DropIndex("dbo.Bikes", new[] { "bikeRackId" });
        }
    }
}

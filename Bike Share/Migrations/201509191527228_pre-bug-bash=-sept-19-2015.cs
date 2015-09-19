namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prebugbashsept192015 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bikes", "bikeRackId", "dbo.BikeRacks");
            DropIndex("dbo.Bikes", new[] { "bikeRackId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.Bikes", "bikeRackId");
            AddForeignKey("dbo.Bikes", "bikeRackId", "dbo.BikeRacks", "bikeRackId");
        }
    }
}

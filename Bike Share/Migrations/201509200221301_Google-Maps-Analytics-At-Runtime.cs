namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoogleMapsAnalyticsAtRuntime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.appSettings", "GoogleAnalyticsId", c => c.String());
            AddColumn("dbo.appSettings", "GoogleMapsKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.appSettings", "GoogleMapsKey");
            DropColumn("dbo.appSettings", "GoogleAnalyticsId");
        }
    }
}

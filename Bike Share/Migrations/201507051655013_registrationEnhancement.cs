namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class registrationEnhancement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.bikeUsers", "registrationPDFNumber", c => c.Int(nullable: false));
            AddColumn("dbo.appSettings", "legalHTML", c => c.String());
            AddColumn("dbo.appSettings", "programHTML", c => c.String());
            AddColumn("dbo.appSettings", "latestPDFNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.appSettings", "latestPDFNumber");
            DropColumn("dbo.appSettings", "programHTML");
            DropColumn("dbo.appSettings", "legalHTML");
            DropColumn("dbo.bikeUsers", "registrationPDFNumber");
        }
    }
}

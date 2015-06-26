namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_lastRun_to_inspection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScheduledInspections", "lastRun", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ScheduledInspections", "lastRun");
        }
    }
}

namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserNotes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.bikeUsers", "Notes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.bikeUsers", "Notes");
        }
    }
}

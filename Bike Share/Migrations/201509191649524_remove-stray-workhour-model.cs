namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removestrayworkhourmodel : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.WorkHours");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.WorkHours",
                c => new
                    {
                        workHourId = c.Int(nullable: false, identity: true),
                        comment = c.String(),
                        timeStart = c.DateTime(nullable: false),
                        timeEnd = c.DateTime(nullable: false),
                        userid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.workHourId);
            
        }
    }
}

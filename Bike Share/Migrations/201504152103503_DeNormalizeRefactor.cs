namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeNormalizeRefactor : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CheckOuts", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.bikeUsers", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.Tracers", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.Tracers", "charge_chargeId", "dbo.Charges");
            DropForeignKey("dbo.Tracers", "checkOut_checkOutId", "dbo.CheckOuts");
            DropForeignKey("dbo.MaintenanceEvents", "bikeAffected_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.Tracers", "maint_MaintenanceEventId", "dbo.MaintenanceEvents");
            DropForeignKey("dbo.MaintenanceEvents", "staffPerson_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.MaintenanceUpdates", "associatedEvent_MaintenanceEventId", "dbo.MaintenanceEvents");
            DropForeignKey("dbo.MaintenanceUpdates", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.Tracers", "update_MaintenanceUpdateId", "dbo.MaintenanceUpdates");
            DropForeignKey("dbo.MaintenanceUpdates", "postedBy_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.Tracers", "shop_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.Hours", "rack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.Hours", "shop_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.MaintenanceEvents", "workshop_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.MaintenanceEvents", "Inspection_inspectionId", "dbo.Inspections");
            DropForeignKey("dbo.Inspections", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.Tracers", "inspection_inspectionId", "dbo.Inspections");
            DropForeignKey("dbo.Inspections", "inspector_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.Inspections", "placeInspected_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.Tracers", "rack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.Tracers", "user_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.WorkHours", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.Tracers", "workHour_workHourId", "dbo.WorkHours");
            DropForeignKey("dbo.WorkHours", "maint_MaintenanceEventId", "dbo.MaintenanceEvents");
            DropForeignKey("dbo.WorkHours", "rack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.WorkHours", "shop_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.WorkHours", "user_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.CheckOuts", "bikeUser_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.CheckOuts", "checkOutPerson_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.CheckOuts", "rackCheckedIn_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.CheckOuts", "rackCheckedOut_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.CheckOuts", "user_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.CheckOuts", "BikeRack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.Bikes", "bikeRack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.MailSubs", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.MailSubs", "rack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.MailSubs", "subscriber_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.MailSubs", "workshop_workshopId", "dbo.Workshops");
            DropIndex("dbo.Bikes", new[] { "bikeRack_bikeRackId" });
            DropIndex("dbo.CheckOuts", new[] { "bike_bikeId" });
            DropIndex("dbo.CheckOuts", new[] { "bikeUser_bikeUserId" });
            DropIndex("dbo.CheckOuts", new[] { "checkOutPerson_bikeUserId" });
            DropIndex("dbo.CheckOuts", new[] { "rackCheckedIn_bikeRackId" });
            DropIndex("dbo.CheckOuts", new[] { "rackCheckedOut_bikeRackId" });
            DropIndex("dbo.CheckOuts", new[] { "user_bikeUserId" });
            DropIndex("dbo.CheckOuts", new[] { "BikeRack_bikeRackId" });
            DropIndex("dbo.bikeUsers", new[] { "bike_bikeId" });
            DropIndex("dbo.Tracers", new[] { "bike_bikeId" });
            DropIndex("dbo.Tracers", new[] { "charge_chargeId" });
            DropIndex("dbo.Tracers", new[] { "checkOut_checkOutId" });
            DropIndex("dbo.Tracers", new[] { "maint_MaintenanceEventId" });
            DropIndex("dbo.Tracers", new[] { "update_MaintenanceUpdateId" });
            DropIndex("dbo.Tracers", new[] { "shop_workshopId" });
            DropIndex("dbo.Tracers", new[] { "inspection_inspectionId" });
            DropIndex("dbo.Tracers", new[] { "rack_bikeRackId" });
            DropIndex("dbo.Tracers", new[] { "user_bikeUserId" });
            DropIndex("dbo.Tracers", new[] { "workHour_workHourId" });
            DropIndex("dbo.Inspections", new[] { "bike_bikeId" });
            DropIndex("dbo.Inspections", new[] { "inspector_bikeUserId" });
            DropIndex("dbo.Inspections", new[] { "placeInspected_workshopId" });
            DropIndex("dbo.MaintenanceEvents", new[] { "bikeAffected_bikeId" });
            DropIndex("dbo.MaintenanceEvents", new[] { "staffPerson_bikeUserId" });
            DropIndex("dbo.MaintenanceEvents", new[] { "workshop_workshopId" });
            DropIndex("dbo.MaintenanceEvents", new[] { "Inspection_inspectionId" });
            DropIndex("dbo.MaintenanceUpdates", new[] { "associatedEvent_MaintenanceEventId" });
            DropIndex("dbo.MaintenanceUpdates", new[] { "bike_bikeId" });
            DropIndex("dbo.MaintenanceUpdates", new[] { "postedBy_bikeUserId" });
            DropIndex("dbo.Hours", new[] { "rack_bikeRackId" });
            DropIndex("dbo.Hours", new[] { "shop_workshopId" });
            DropIndex("dbo.WorkHours", new[] { "bike_bikeId" });
            DropIndex("dbo.WorkHours", new[] { "maint_MaintenanceEventId" });
            DropIndex("dbo.WorkHours", new[] { "rack_bikeRackId" });
            DropIndex("dbo.WorkHours", new[] { "shop_workshopId" });
            DropIndex("dbo.WorkHours", new[] { "user_bikeUserId" });
            DropIndex("dbo.MailSubs", new[] { "bike_bikeId" });
            DropIndex("dbo.MailSubs", new[] { "rack_bikeRackId" });
            DropIndex("dbo.MailSubs", new[] { "subscriber_bikeUserId" });
            DropIndex("dbo.MailSubs", new[] { "workshop_workshopId" });
            RenameColumn(table: "dbo.Charges", name: "user_bikeUserId", newName: "bikeUserId");
            RenameIndex(table: "dbo.Charges", name: "IX_user_bikeUserId", newName: "IX_bikeUserId");
            AddColumn("dbo.Bikes", "bikeRackId", c => c.Int());
            AddColumn("dbo.Bikes", "onInspectionHold", c => c.Boolean(nullable: false));
            AddColumn("dbo.Bikes", "onMaintenanceHold", c => c.Boolean(nullable: false));
            AddColumn("dbo.BikeRacks", "hours", c => c.String());
            AddColumn("dbo.CheckOuts", "checkOutPerson", c => c.Int(nullable: false));
            AddColumn("dbo.CheckOuts", "rider", c => c.Int(nullable: false));
            AddColumn("dbo.CheckOuts", "bike", c => c.Int(nullable: false));
            AddColumn("dbo.CheckOuts", "rackCheckedOut", c => c.Int(nullable: false));
            AddColumn("dbo.CheckOuts", "rackCheckedIn", c => c.Int(nullable: false));
            AddColumn("dbo.Inspections", "inspectorId", c => c.Int(nullable: false));
            AddColumn("dbo.Inspections", "bikeId", c => c.Int(nullable: false));
            AddColumn("dbo.Inspections", "placeInspectedId", c => c.Int(nullable: false));
            AddColumn("dbo.MaintenanceEvents", "bikeId", c => c.Int(nullable: false));
            AddColumn("dbo.MaintenanceEvents", "submittedById", c => c.Int(nullable: false));
            AddColumn("dbo.MaintenanceEvents", "maintainedById", c => c.Int());
            AddColumn("dbo.MaintenanceEvents", "workshopId", c => c.Int(nullable: false));
            AddColumn("dbo.MaintenanceUpdates", "postedById", c => c.Int(nullable: false));
            AddColumn("dbo.MaintenanceUpdates", "associatedEventId", c => c.Int(nullable: false));
            AddColumn("dbo.MaintenanceUpdates", "bikeId", c => c.Int(nullable: false));
            AddColumn("dbo.Workshops", "hours", c => c.String());
            AddColumn("dbo.WorkHours", "userid", c => c.Int(nullable: false));
            AlterColumn("dbo.MaintenanceEvents", "timeResolved", c => c.DateTime());
            DropColumn("dbo.Bikes", "lastPassedInspection");
            DropColumn("dbo.Bikes", "bikeRack_bikeRackId");
            DropColumn("dbo.CheckOuts", "bike_bikeId");
            DropColumn("dbo.CheckOuts", "bikeUser_bikeUserId");
            DropColumn("dbo.CheckOuts", "checkOutPerson_bikeUserId");
            DropColumn("dbo.CheckOuts", "rackCheckedIn_bikeRackId");
            DropColumn("dbo.CheckOuts", "rackCheckedOut_bikeRackId");
            DropColumn("dbo.CheckOuts", "user_bikeUserId");
            DropColumn("dbo.CheckOuts", "BikeRack_bikeRackId");
            DropColumn("dbo.bikeUsers", "hasBike");
            DropColumn("dbo.bikeUsers", "bike_bikeId");
            DropColumn("dbo.Inspections", "bike_bikeId");
            DropColumn("dbo.Inspections", "inspector_bikeUserId");
            DropColumn("dbo.Inspections", "placeInspected_workshopId");
            DropColumn("dbo.MaintenanceEvents", "resolved");
            DropColumn("dbo.MaintenanceEvents", "bikeAffected_bikeId");
            DropColumn("dbo.MaintenanceEvents", "staffPerson_bikeUserId");
            DropColumn("dbo.MaintenanceEvents", "workshop_workshopId");
            DropColumn("dbo.MaintenanceEvents", "Inspection_inspectionId");
            DropColumn("dbo.MaintenanceUpdates", "associatedEvent_MaintenanceEventId");
            DropColumn("dbo.MaintenanceUpdates", "bike_bikeId");
            DropColumn("dbo.MaintenanceUpdates", "postedBy_bikeUserId");
            DropColumn("dbo.WorkHours", "bike_bikeId");
            DropColumn("dbo.WorkHours", "maint_MaintenanceEventId");
            DropColumn("dbo.WorkHours", "rack_bikeRackId");
            DropColumn("dbo.WorkHours", "shop_workshopId");
            DropColumn("dbo.WorkHours", "user_bikeUserId");
            DropTable("dbo.Tracers");
            DropTable("dbo.Hours");
            DropTable("dbo.MailSubs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.MailSubs",
                c => new
                    {
                        subId = c.Int(nullable: false, identity: true),
                        email = c.String(),
                        bike_bikeId = c.Int(),
                        rack_bikeRackId = c.Int(),
                        subscriber_bikeUserId = c.Int(nullable: false),
                        workshop_workshopId = c.Int(),
                    })
                .PrimaryKey(t => t.subId);
            
            CreateTable(
                "dbo.Hours",
                c => new
                    {
                        hourId = c.Int(nullable: false, identity: true),
                        dayStart = c.String(),
                        dayEnd = c.String(),
                        hourStart = c.Int(nullable: false),
                        hourEnd = c.Int(nullable: false),
                        minuteStart = c.Int(nullable: false),
                        minuteEnd = c.Int(nullable: false),
                        isOpen = c.Boolean(nullable: false),
                        comment = c.String(),
                        rack_bikeRackId = c.Int(),
                        shop_workshopId = c.Int(),
                    })
                .PrimaryKey(t => t.hourId);
            
            CreateTable(
                "dbo.Tracers",
                c => new
                    {
                        tracerId = c.Int(nullable: false, identity: true),
                        comment = c.String(),
                        time = c.DateTime(nullable: false),
                        bike_bikeId = c.Int(),
                        charge_chargeId = c.Int(),
                        checkOut_checkOutId = c.Int(),
                        maint_MaintenanceEventId = c.Int(),
                        update_MaintenanceUpdateId = c.Int(),
                        shop_workshopId = c.Int(),
                        inspection_inspectionId = c.Int(),
                        rack_bikeRackId = c.Int(),
                        user_bikeUserId = c.Int(),
                        workHour_workHourId = c.Int(),
                    })
                .PrimaryKey(t => t.tracerId);
            
            AddColumn("dbo.WorkHours", "user_bikeUserId", c => c.Int());
            AddColumn("dbo.WorkHours", "shop_workshopId", c => c.Int());
            AddColumn("dbo.WorkHours", "rack_bikeRackId", c => c.Int());
            AddColumn("dbo.WorkHours", "maint_MaintenanceEventId", c => c.Int());
            AddColumn("dbo.WorkHours", "bike_bikeId", c => c.Int());
            AddColumn("dbo.MaintenanceUpdates", "postedBy_bikeUserId", c => c.Int(nullable: false));
            AddColumn("dbo.MaintenanceUpdates", "bike_bikeId", c => c.Int());
            AddColumn("dbo.MaintenanceUpdates", "associatedEvent_MaintenanceEventId", c => c.Int());
            AddColumn("dbo.MaintenanceEvents", "Inspection_inspectionId", c => c.Int());
            AddColumn("dbo.MaintenanceEvents", "workshop_workshopId", c => c.Int(nullable: false));
            AddColumn("dbo.MaintenanceEvents", "staffPerson_bikeUserId", c => c.Int(nullable: false));
            AddColumn("dbo.MaintenanceEvents", "bikeAffected_bikeId", c => c.Int(nullable: false));
            AddColumn("dbo.MaintenanceEvents", "resolved", c => c.Boolean(nullable: false));
            AddColumn("dbo.Inspections", "placeInspected_workshopId", c => c.Int(nullable: false));
            AddColumn("dbo.Inspections", "inspector_bikeUserId", c => c.Int(nullable: false));
            AddColumn("dbo.Inspections", "bike_bikeId", c => c.Int(nullable: false));
            AddColumn("dbo.bikeUsers", "bike_bikeId", c => c.Int());
            AddColumn("dbo.bikeUsers", "hasBike", c => c.Boolean(nullable: false));
            AddColumn("dbo.CheckOuts", "BikeRack_bikeRackId", c => c.Int());
            AddColumn("dbo.CheckOuts", "user_bikeUserId", c => c.Int());
            AddColumn("dbo.CheckOuts", "rackCheckedOut_bikeRackId", c => c.Int());
            AddColumn("dbo.CheckOuts", "rackCheckedIn_bikeRackId", c => c.Int());
            AddColumn("dbo.CheckOuts", "checkOutPerson_bikeUserId", c => c.Int());
            AddColumn("dbo.CheckOuts", "bikeUser_bikeUserId", c => c.Int());
            AddColumn("dbo.CheckOuts", "bike_bikeId", c => c.Int());
            AddColumn("dbo.Bikes", "bikeRack_bikeRackId", c => c.Int(nullable: false));
            AddColumn("dbo.Bikes", "lastPassedInspection", c => c.DateTime(nullable: false));
            AlterColumn("dbo.MaintenanceEvents", "timeResolved", c => c.DateTime(nullable: false));
            DropColumn("dbo.WorkHours", "userid");
            DropColumn("dbo.Workshops", "hours");
            DropColumn("dbo.MaintenanceUpdates", "bikeId");
            DropColumn("dbo.MaintenanceUpdates", "associatedEventId");
            DropColumn("dbo.MaintenanceUpdates", "postedById");
            DropColumn("dbo.MaintenanceEvents", "workshopId");
            DropColumn("dbo.MaintenanceEvents", "maintainedById");
            DropColumn("dbo.MaintenanceEvents", "submittedById");
            DropColumn("dbo.MaintenanceEvents", "bikeId");
            DropColumn("dbo.Inspections", "placeInspectedId");
            DropColumn("dbo.Inspections", "bikeId");
            DropColumn("dbo.Inspections", "inspectorId");
            DropColumn("dbo.CheckOuts", "rackCheckedIn");
            DropColumn("dbo.CheckOuts", "rackCheckedOut");
            DropColumn("dbo.CheckOuts", "bike");
            DropColumn("dbo.CheckOuts", "rider");
            DropColumn("dbo.CheckOuts", "checkOutPerson");
            DropColumn("dbo.BikeRacks", "hours");
            DropColumn("dbo.Bikes", "onMaintenanceHold");
            DropColumn("dbo.Bikes", "onInspectionHold");
            DropColumn("dbo.Bikes", "bikeRackId");
            RenameIndex(table: "dbo.Charges", name: "IX_bikeUserId", newName: "IX_user_bikeUserId");
            RenameColumn(table: "dbo.Charges", name: "bikeUserId", newName: "user_bikeUserId");
            CreateIndex("dbo.MailSubs", "workshop_workshopId");
            CreateIndex("dbo.MailSubs", "subscriber_bikeUserId");
            CreateIndex("dbo.MailSubs", "rack_bikeRackId");
            CreateIndex("dbo.MailSubs", "bike_bikeId");
            CreateIndex("dbo.WorkHours", "user_bikeUserId");
            CreateIndex("dbo.WorkHours", "shop_workshopId");
            CreateIndex("dbo.WorkHours", "rack_bikeRackId");
            CreateIndex("dbo.WorkHours", "maint_MaintenanceEventId");
            CreateIndex("dbo.WorkHours", "bike_bikeId");
            CreateIndex("dbo.Hours", "shop_workshopId");
            CreateIndex("dbo.Hours", "rack_bikeRackId");
            CreateIndex("dbo.MaintenanceUpdates", "postedBy_bikeUserId");
            CreateIndex("dbo.MaintenanceUpdates", "bike_bikeId");
            CreateIndex("dbo.MaintenanceUpdates", "associatedEvent_MaintenanceEventId");
            CreateIndex("dbo.MaintenanceEvents", "Inspection_inspectionId");
            CreateIndex("dbo.MaintenanceEvents", "workshop_workshopId");
            CreateIndex("dbo.MaintenanceEvents", "staffPerson_bikeUserId");
            CreateIndex("dbo.MaintenanceEvents", "bikeAffected_bikeId");
            CreateIndex("dbo.Inspections", "placeInspected_workshopId");
            CreateIndex("dbo.Inspections", "inspector_bikeUserId");
            CreateIndex("dbo.Inspections", "bike_bikeId");
            CreateIndex("dbo.Tracers", "workHour_workHourId");
            CreateIndex("dbo.Tracers", "user_bikeUserId");
            CreateIndex("dbo.Tracers", "rack_bikeRackId");
            CreateIndex("dbo.Tracers", "inspection_inspectionId");
            CreateIndex("dbo.Tracers", "shop_workshopId");
            CreateIndex("dbo.Tracers", "update_MaintenanceUpdateId");
            CreateIndex("dbo.Tracers", "maint_MaintenanceEventId");
            CreateIndex("dbo.Tracers", "checkOut_checkOutId");
            CreateIndex("dbo.Tracers", "charge_chargeId");
            CreateIndex("dbo.Tracers", "bike_bikeId");
            CreateIndex("dbo.bikeUsers", "bike_bikeId");
            CreateIndex("dbo.CheckOuts", "BikeRack_bikeRackId");
            CreateIndex("dbo.CheckOuts", "user_bikeUserId");
            CreateIndex("dbo.CheckOuts", "rackCheckedOut_bikeRackId");
            CreateIndex("dbo.CheckOuts", "rackCheckedIn_bikeRackId");
            CreateIndex("dbo.CheckOuts", "checkOutPerson_bikeUserId");
            CreateIndex("dbo.CheckOuts", "bikeUser_bikeUserId");
            CreateIndex("dbo.CheckOuts", "bike_bikeId");
            CreateIndex("dbo.Bikes", "bikeRack_bikeRackId");
            AddForeignKey("dbo.MailSubs", "workshop_workshopId", "dbo.Workshops", "workshopId");
            AddForeignKey("dbo.MailSubs", "subscriber_bikeUserId", "dbo.bikeUsers", "bikeUserId", cascadeDelete: true);
            AddForeignKey("dbo.MailSubs", "rack_bikeRackId", "dbo.BikeRacks", "bikeRackId");
            AddForeignKey("dbo.MailSubs", "bike_bikeId", "dbo.Bikes", "bikeId");
            AddForeignKey("dbo.Bikes", "bikeRack_bikeRackId", "dbo.BikeRacks", "bikeRackId");
            AddForeignKey("dbo.CheckOuts", "BikeRack_bikeRackId", "dbo.BikeRacks", "bikeRackId");
            AddForeignKey("dbo.CheckOuts", "user_bikeUserId", "dbo.bikeUsers", "bikeUserId");
            AddForeignKey("dbo.CheckOuts", "rackCheckedOut_bikeRackId", "dbo.BikeRacks", "bikeRackId");
            AddForeignKey("dbo.CheckOuts", "rackCheckedIn_bikeRackId", "dbo.BikeRacks", "bikeRackId");
            AddForeignKey("dbo.CheckOuts", "checkOutPerson_bikeUserId", "dbo.bikeUsers", "bikeUserId");
            AddForeignKey("dbo.CheckOuts", "bikeUser_bikeUserId", "dbo.bikeUsers", "bikeUserId");
            AddForeignKey("dbo.WorkHours", "user_bikeUserId", "dbo.bikeUsers", "bikeUserId");
            AddForeignKey("dbo.WorkHours", "shop_workshopId", "dbo.Workshops", "workshopId");
            AddForeignKey("dbo.WorkHours", "rack_bikeRackId", "dbo.BikeRacks", "bikeRackId");
            AddForeignKey("dbo.WorkHours", "maint_MaintenanceEventId", "dbo.MaintenanceEvents", "MaintenanceEventId");
            AddForeignKey("dbo.Tracers", "workHour_workHourId", "dbo.WorkHours", "workHourId");
            AddForeignKey("dbo.WorkHours", "bike_bikeId", "dbo.Bikes", "bikeId");
            AddForeignKey("dbo.Tracers", "user_bikeUserId", "dbo.bikeUsers", "bikeUserId");
            AddForeignKey("dbo.Tracers", "rack_bikeRackId", "dbo.BikeRacks", "bikeRackId");
            AddForeignKey("dbo.Inspections", "placeInspected_workshopId", "dbo.Workshops", "workshopId", cascadeDelete: true);
            AddForeignKey("dbo.Inspections", "inspector_bikeUserId", "dbo.bikeUsers", "bikeUserId", cascadeDelete: true);
            AddForeignKey("dbo.Tracers", "inspection_inspectionId", "dbo.Inspections", "inspectionId");
            AddForeignKey("dbo.Inspections", "bike_bikeId", "dbo.Bikes", "bikeId", cascadeDelete: true);
            AddForeignKey("dbo.MaintenanceEvents", "Inspection_inspectionId", "dbo.Inspections", "inspectionId");
            AddForeignKey("dbo.MaintenanceEvents", "workshop_workshopId", "dbo.Workshops", "workshopId", cascadeDelete: true);
            AddForeignKey("dbo.Hours", "shop_workshopId", "dbo.Workshops", "workshopId");
            AddForeignKey("dbo.Hours", "rack_bikeRackId", "dbo.BikeRacks", "bikeRackId");
            AddForeignKey("dbo.Tracers", "shop_workshopId", "dbo.Workshops", "workshopId");
            AddForeignKey("dbo.MaintenanceUpdates", "postedBy_bikeUserId", "dbo.bikeUsers", "bikeUserId", cascadeDelete: true);
            AddForeignKey("dbo.Tracers", "update_MaintenanceUpdateId", "dbo.MaintenanceUpdates", "MaintenanceUpdateId");
            AddForeignKey("dbo.MaintenanceUpdates", "bike_bikeId", "dbo.Bikes", "bikeId");
            AddForeignKey("dbo.MaintenanceUpdates", "associatedEvent_MaintenanceEventId", "dbo.MaintenanceEvents", "MaintenanceEventId");
            AddForeignKey("dbo.MaintenanceEvents", "staffPerson_bikeUserId", "dbo.bikeUsers", "bikeUserId");
            AddForeignKey("dbo.Tracers", "maint_MaintenanceEventId", "dbo.MaintenanceEvents", "MaintenanceEventId");
            AddForeignKey("dbo.MaintenanceEvents", "bikeAffected_bikeId", "dbo.Bikes", "bikeId", cascadeDelete: true);
            AddForeignKey("dbo.Tracers", "checkOut_checkOutId", "dbo.CheckOuts", "checkOutId");
            AddForeignKey("dbo.Tracers", "charge_chargeId", "dbo.Charges", "chargeId");
            AddForeignKey("dbo.Tracers", "bike_bikeId", "dbo.Bikes", "bikeId");
            AddForeignKey("dbo.bikeUsers", "bike_bikeId", "dbo.Bikes", "bikeId");
            AddForeignKey("dbo.CheckOuts", "bike_bikeId", "dbo.Bikes", "bikeId");
        }
    }
}

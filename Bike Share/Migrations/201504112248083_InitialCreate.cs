namespace Bike_Share.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bikes",
                c => new
                    {
                        bikeId = c.Int(nullable: false, identity: true),
                        bikeNumber = c.Int(nullable: false),
                        bikeName = c.String(nullable: false),
                        lastCheckedOut = c.DateTime(nullable: false),
                        isArchived = c.Boolean(nullable: false),
                        lastPassedInspection = c.DateTime(nullable: false),
                        bikeRack_bikeRackId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.bikeId)
                .ForeignKey("dbo.BikeRacks", t => t.bikeRack_bikeRackId)
                .Index(t => t.bikeRack_bikeRackId);
            
            CreateTable(
                "dbo.BikeRacks",
                c => new
                    {
                        bikeRackId = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        GPSCoordX = c.Single(nullable: false),
                        GPSCoordY = c.Single(nullable: false),
                        description = c.String(nullable: false),
                        isArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.bikeRackId);
            
            CreateTable(
                "dbo.CheckOuts",
                c => new
                    {
                        checkOutId = c.Int(nullable: false, identity: true),
                        timeOut = c.DateTime(nullable: false),
                        timeIn = c.DateTime(),
                        isResolved = c.Boolean(nullable: false),
                        bike_bikeId = c.Int(),
                        bikeUser_bikeUserId = c.Int(),
                        checkOutPerson_bikeUserId = c.Int(),
                        rackCheckedIn_bikeRackId = c.Int(),
                        rackCheckedOut_bikeRackId = c.Int(),
                        user_bikeUserId = c.Int(),
                        BikeRack_bikeRackId = c.Int(),
                    })
                .PrimaryKey(t => t.checkOutId)
                .ForeignKey("dbo.Bikes", t => t.bike_bikeId)
                .ForeignKey("dbo.bikeUsers", t => t.bikeUser_bikeUserId)
                .ForeignKey("dbo.bikeUsers", t => t.checkOutPerson_bikeUserId)
                .ForeignKey("dbo.BikeRacks", t => t.rackCheckedIn_bikeRackId)
                .ForeignKey("dbo.BikeRacks", t => t.rackCheckedOut_bikeRackId)
                .ForeignKey("dbo.bikeUsers", t => t.user_bikeUserId)
                .ForeignKey("dbo.BikeRacks", t => t.BikeRack_bikeRackId)
                .Index(t => t.bike_bikeId)
                .Index(t => t.bikeUser_bikeUserId)
                .Index(t => t.checkOutPerson_bikeUserId)
                .Index(t => t.rackCheckedIn_bikeRackId)
                .Index(t => t.rackCheckedOut_bikeRackId)
                .Index(t => t.user_bikeUserId)
                .Index(t => t.BikeRack_bikeRackId);
            
            CreateTable(
                "dbo.bikeUsers",
                c => new
                    {
                        bikeUserId = c.Int(nullable: false, identity: true),
                        lastRegistered = c.DateTime(nullable: false),
                        userName = c.String(nullable: false),
                        firstName = c.String(),
                        lastName = c.String(),
                        hasBike = c.Boolean(nullable: false),
                        email = c.String(nullable: false),
                        phoneNumber = c.String(),
                        isArchived = c.Boolean(nullable: false),
                        canMaintainBikes = c.Boolean(nullable: false),
                        canBorrowBikes = c.Boolean(nullable: false),
                        canAdministerSite = c.Boolean(nullable: false),
                        canCheckOutBikes = c.Boolean(nullable: false),
                        bike_bikeId = c.Int(),
                    })
                .PrimaryKey(t => t.bikeUserId)
                .ForeignKey("dbo.Bikes", t => t.bike_bikeId)
                .Index(t => t.bike_bikeId);
            
            CreateTable(
                "dbo.Charges",
                c => new
                    {
                        chargeId = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false),
                        description = c.String(nullable: false),
                        dateAssesed = c.DateTime(nullable: false),
                        dateResolved = c.DateTime(nullable: false),
                        isResolved = c.Boolean(nullable: false),
                        notificationsCounter = c.Int(nullable: false),
                        amountCharged = c.Decimal(nullable: false, precision: 18, scale: 2),
                        amountPaid = c.Decimal(nullable: false, precision: 18, scale: 2),
                        user_bikeUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.chargeId)
                .ForeignKey("dbo.bikeUsers", t => t.user_bikeUserId, cascadeDelete: true)
                .Index(t => t.user_bikeUserId);
            
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
                .PrimaryKey(t => t.tracerId)
                .ForeignKey("dbo.Bikes", t => t.bike_bikeId)
                .ForeignKey("dbo.Charges", t => t.charge_chargeId)
                .ForeignKey("dbo.CheckOuts", t => t.checkOut_checkOutId)
                .ForeignKey("dbo.MaintenanceEvents", t => t.maint_MaintenanceEventId)
                .ForeignKey("dbo.MaintenanceUpdates", t => t.update_MaintenanceUpdateId)
                .ForeignKey("dbo.Workshops", t => t.shop_workshopId)
                .ForeignKey("dbo.Inspections", t => t.inspection_inspectionId)
                .ForeignKey("dbo.BikeRacks", t => t.rack_bikeRackId)
                .ForeignKey("dbo.bikeUsers", t => t.user_bikeUserId)
                .ForeignKey("dbo.WorkHours", t => t.workHour_workHourId)
                .Index(t => t.bike_bikeId)
                .Index(t => t.charge_chargeId)
                .Index(t => t.checkOut_checkOutId)
                .Index(t => t.maint_MaintenanceEventId)
                .Index(t => t.update_MaintenanceUpdateId)
                .Index(t => t.shop_workshopId)
                .Index(t => t.inspection_inspectionId)
                .Index(t => t.rack_bikeRackId)
                .Index(t => t.user_bikeUserId)
                .Index(t => t.workHour_workHourId);
            
            CreateTable(
                "dbo.Inspections",
                c => new
                    {
                        inspectionId = c.Int(nullable: false, identity: true),
                        datePerformed = c.DateTime(nullable: false),
                        isPassed = c.Boolean(nullable: false),
                        comment = c.String(),
                        bike_bikeId = c.Int(nullable: false),
                        inspector_bikeUserId = c.Int(nullable: false),
                        placeInspected_workshopId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.inspectionId)
                .ForeignKey("dbo.Bikes", t => t.bike_bikeId, cascadeDelete: true)
                .ForeignKey("dbo.bikeUsers", t => t.inspector_bikeUserId, cascadeDelete: true)
                .ForeignKey("dbo.Workshops", t => t.placeInspected_workshopId, cascadeDelete: true)
                .Index(t => t.bike_bikeId)
                .Index(t => t.inspector_bikeUserId)
                .Index(t => t.placeInspected_workshopId);
            
            CreateTable(
                "dbo.MaintenanceEvents",
                c => new
                    {
                        MaintenanceEventId = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false),
                        details = c.String(),
                        timeAdded = c.DateTime(nullable: false),
                        resolved = c.Boolean(nullable: false),
                        timeResolved = c.DateTime(nullable: false),
                        disableBike = c.Boolean(nullable: false),
                        isArchived = c.Boolean(nullable: false),
                        bikeAffected_bikeId = c.Int(nullable: false),
                        staffPerson_bikeUserId = c.Int(nullable: false),
                        workshop_workshopId = c.Int(nullable: false),
                        Inspection_inspectionId = c.Int(),
                    })
                .PrimaryKey(t => t.MaintenanceEventId)
                .ForeignKey("dbo.Bikes", t => t.bikeAffected_bikeId, cascadeDelete: true)
                .ForeignKey("dbo.bikeUsers", t => t.staffPerson_bikeUserId)
                .ForeignKey("dbo.Workshops", t => t.workshop_workshopId, cascadeDelete: true)
                .ForeignKey("dbo.Inspections", t => t.Inspection_inspectionId)
                .Index(t => t.bikeAffected_bikeId)
                .Index(t => t.staffPerson_bikeUserId)
                .Index(t => t.workshop_workshopId)
                .Index(t => t.Inspection_inspectionId);
            
            CreateTable(
                "dbo.MaintenanceUpdates",
                c => new
                    {
                        MaintenanceUpdateId = c.Int(nullable: false, identity: true),
                        title = c.String(nullable: false),
                        body = c.String(),
                        timePosted = c.DateTime(nullable: false),
                        isCommentOnBike = c.Boolean(nullable: false),
                        associatedEvent_MaintenanceEventId = c.Int(),
                        bike_bikeId = c.Int(),
                        postedBy_bikeUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaintenanceUpdateId)
                .ForeignKey("dbo.MaintenanceEvents", t => t.associatedEvent_MaintenanceEventId)
                .ForeignKey("dbo.Bikes", t => t.bike_bikeId)
                .ForeignKey("dbo.bikeUsers", t => t.postedBy_bikeUserId, cascadeDelete: true)
                .Index(t => t.associatedEvent_MaintenanceEventId)
                .Index(t => t.bike_bikeId)
                .Index(t => t.postedBy_bikeUserId);
            
            CreateTable(
                "dbo.Workshops",
                c => new
                    {
                        workshopId = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        GPSCoordX = c.Single(nullable: false),
                        GPSCoordY = c.Single(nullable: false),
                        isArchived = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.workshopId);
            
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
                .PrimaryKey(t => t.hourId)
                .ForeignKey("dbo.BikeRacks", t => t.rack_bikeRackId)
                .ForeignKey("dbo.Workshops", t => t.shop_workshopId)
                .Index(t => t.rack_bikeRackId)
                .Index(t => t.shop_workshopId);
            
            CreateTable(
                "dbo.WorkHours",
                c => new
                    {
                        workHourId = c.Int(nullable: false, identity: true),
                        comment = c.String(),
                        timeStart = c.DateTime(nullable: false),
                        timeEnd = c.DateTime(nullable: false),
                        bike_bikeId = c.Int(),
                        maint_MaintenanceEventId = c.Int(),
                        rack_bikeRackId = c.Int(),
                        shop_workshopId = c.Int(),
                        user_bikeUserId = c.Int(),
                    })
                .PrimaryKey(t => t.workHourId)
                .ForeignKey("dbo.Bikes", t => t.bike_bikeId)
                .ForeignKey("dbo.MaintenanceEvents", t => t.maint_MaintenanceEventId)
                .ForeignKey("dbo.BikeRacks", t => t.rack_bikeRackId)
                .ForeignKey("dbo.Workshops", t => t.shop_workshopId)
                .ForeignKey("dbo.bikeUsers", t => t.user_bikeUserId)
                .Index(t => t.bike_bikeId)
                .Index(t => t.maint_MaintenanceEventId)
                .Index(t => t.rack_bikeRackId)
                .Index(t => t.shop_workshopId)
                .Index(t => t.user_bikeUserId);
            
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
                .PrimaryKey(t => t.subId)
                .ForeignKey("dbo.Bikes", t => t.bike_bikeId)
                .ForeignKey("dbo.BikeRacks", t => t.rack_bikeRackId)
                .ForeignKey("dbo.bikeUsers", t => t.subscriber_bikeUserId, cascadeDelete: true)
                .ForeignKey("dbo.Workshops", t => t.workshop_workshopId)
                .Index(t => t.bike_bikeId)
                .Index(t => t.rack_bikeRackId)
                .Index(t => t.subscriber_bikeUserId)
                .Index(t => t.workshop_workshopId);
            
            CreateTable(
                "dbo.appSettings",
                c => new
                    {
                        settingId = c.Int(nullable: false, identity: true),
                        appName = c.String(),
                        expectedEmail = c.String(),
                        adminEmailList = c.String(),
                        maxRentDays = c.Int(nullable: false),
                        DaysBetweenInspections = c.Int(nullable: false),
                        daysBetweenRegistrations = c.Int(nullable: false),
                        overdueBikeMailingIntervalHours = c.Int(nullable: false),
                        footerHTML = c.String(),
                        homeHTML = c.String(),
                        announcementHTML = c.String(),
                        FAQHTML = c.String(),
                        contactHTML = c.String(),
                        aboutHTML = c.String(),
                        safetyHTML = c.String(),
                        registerHTML = c.String(),
                    })
                .PrimaryKey(t => t.settingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MailSubs", "workshop_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.MailSubs", "subscriber_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.MailSubs", "rack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.MailSubs", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.Bikes", "bikeRack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.CheckOuts", "BikeRack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.CheckOuts", "user_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.CheckOuts", "rackCheckedOut_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.CheckOuts", "rackCheckedIn_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.CheckOuts", "checkOutPerson_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.CheckOuts", "bikeUser_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.Charges", "user_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.WorkHours", "user_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.WorkHours", "shop_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.WorkHours", "rack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.WorkHours", "maint_MaintenanceEventId", "dbo.MaintenanceEvents");
            DropForeignKey("dbo.Tracers", "workHour_workHourId", "dbo.WorkHours");
            DropForeignKey("dbo.WorkHours", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.Tracers", "user_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.Tracers", "rack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.Inspections", "placeInspected_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.Inspections", "inspector_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.Tracers", "inspection_inspectionId", "dbo.Inspections");
            DropForeignKey("dbo.Inspections", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.MaintenanceEvents", "Inspection_inspectionId", "dbo.Inspections");
            DropForeignKey("dbo.MaintenanceEvents", "workshop_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.Hours", "shop_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.Hours", "rack_bikeRackId", "dbo.BikeRacks");
            DropForeignKey("dbo.Tracers", "shop_workshopId", "dbo.Workshops");
            DropForeignKey("dbo.MaintenanceUpdates", "postedBy_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.Tracers", "update_MaintenanceUpdateId", "dbo.MaintenanceUpdates");
            DropForeignKey("dbo.MaintenanceUpdates", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.MaintenanceUpdates", "associatedEvent_MaintenanceEventId", "dbo.MaintenanceEvents");
            DropForeignKey("dbo.MaintenanceEvents", "staffPerson_bikeUserId", "dbo.bikeUsers");
            DropForeignKey("dbo.Tracers", "maint_MaintenanceEventId", "dbo.MaintenanceEvents");
            DropForeignKey("dbo.MaintenanceEvents", "bikeAffected_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.Tracers", "checkOut_checkOutId", "dbo.CheckOuts");
            DropForeignKey("dbo.Tracers", "charge_chargeId", "dbo.Charges");
            DropForeignKey("dbo.Tracers", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.bikeUsers", "bike_bikeId", "dbo.Bikes");
            DropForeignKey("dbo.CheckOuts", "bike_bikeId", "dbo.Bikes");
            DropIndex("dbo.MailSubs", new[] { "workshop_workshopId" });
            DropIndex("dbo.MailSubs", new[] { "subscriber_bikeUserId" });
            DropIndex("dbo.MailSubs", new[] { "rack_bikeRackId" });
            DropIndex("dbo.MailSubs", new[] { "bike_bikeId" });
            DropIndex("dbo.WorkHours", new[] { "user_bikeUserId" });
            DropIndex("dbo.WorkHours", new[] { "shop_workshopId" });
            DropIndex("dbo.WorkHours", new[] { "rack_bikeRackId" });
            DropIndex("dbo.WorkHours", new[] { "maint_MaintenanceEventId" });
            DropIndex("dbo.WorkHours", new[] { "bike_bikeId" });
            DropIndex("dbo.Hours", new[] { "shop_workshopId" });
            DropIndex("dbo.Hours", new[] { "rack_bikeRackId" });
            DropIndex("dbo.MaintenanceUpdates", new[] { "postedBy_bikeUserId" });
            DropIndex("dbo.MaintenanceUpdates", new[] { "bike_bikeId" });
            DropIndex("dbo.MaintenanceUpdates", new[] { "associatedEvent_MaintenanceEventId" });
            DropIndex("dbo.MaintenanceEvents", new[] { "Inspection_inspectionId" });
            DropIndex("dbo.MaintenanceEvents", new[] { "workshop_workshopId" });
            DropIndex("dbo.MaintenanceEvents", new[] { "staffPerson_bikeUserId" });
            DropIndex("dbo.MaintenanceEvents", new[] { "bikeAffected_bikeId" });
            DropIndex("dbo.Inspections", new[] { "placeInspected_workshopId" });
            DropIndex("dbo.Inspections", new[] { "inspector_bikeUserId" });
            DropIndex("dbo.Inspections", new[] { "bike_bikeId" });
            DropIndex("dbo.Tracers", new[] { "workHour_workHourId" });
            DropIndex("dbo.Tracers", new[] { "user_bikeUserId" });
            DropIndex("dbo.Tracers", new[] { "rack_bikeRackId" });
            DropIndex("dbo.Tracers", new[] { "inspection_inspectionId" });
            DropIndex("dbo.Tracers", new[] { "shop_workshopId" });
            DropIndex("dbo.Tracers", new[] { "update_MaintenanceUpdateId" });
            DropIndex("dbo.Tracers", new[] { "maint_MaintenanceEventId" });
            DropIndex("dbo.Tracers", new[] { "checkOut_checkOutId" });
            DropIndex("dbo.Tracers", new[] { "charge_chargeId" });
            DropIndex("dbo.Tracers", new[] { "bike_bikeId" });
            DropIndex("dbo.Charges", new[] { "user_bikeUserId" });
            DropIndex("dbo.bikeUsers", new[] { "bike_bikeId" });
            DropIndex("dbo.CheckOuts", new[] { "BikeRack_bikeRackId" });
            DropIndex("dbo.CheckOuts", new[] { "user_bikeUserId" });
            DropIndex("dbo.CheckOuts", new[] { "rackCheckedOut_bikeRackId" });
            DropIndex("dbo.CheckOuts", new[] { "rackCheckedIn_bikeRackId" });
            DropIndex("dbo.CheckOuts", new[] { "checkOutPerson_bikeUserId" });
            DropIndex("dbo.CheckOuts", new[] { "bikeUser_bikeUserId" });
            DropIndex("dbo.CheckOuts", new[] { "bike_bikeId" });
            DropIndex("dbo.Bikes", new[] { "bikeRack_bikeRackId" });
            DropTable("dbo.appSettings");
            DropTable("dbo.MailSubs");
            DropTable("dbo.WorkHours");
            DropTable("dbo.Hours");
            DropTable("dbo.Workshops");
            DropTable("dbo.MaintenanceUpdates");
            DropTable("dbo.MaintenanceEvents");
            DropTable("dbo.Inspections");
            DropTable("dbo.Tracers");
            DropTable("dbo.Charges");
            DropTable("dbo.bikeUsers");
            DropTable("dbo.CheckOuts");
            DropTable("dbo.BikeRacks");
            DropTable("dbo.Bikes");
        }
    }
}

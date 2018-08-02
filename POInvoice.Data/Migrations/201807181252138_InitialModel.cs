namespace POInvoice.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PoFormId = c.Int(nullable: false),
                        TypeOfDocument = c.Int(nullable: false),
                        FilePath = c.String(),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PoForms", t => t.PoFormId, cascadeDelete: true)
                .Index(t => t.PoFormId);
            
            CreateTable(
                "dbo.PoForms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Overview = c.String(),
                        PoNumber = c.String(),
                        Status = c.Int(nullable: false),
                        VendorId = c.Int(nullable: false),
                        ResponsibleParty = c.String(),
                        DateRequired = c.DateTime(nullable: false),
                        RequisitioningDepartment = c.String(),
                        ShipViaTerms = c.String(),
                        Attn = c.String(),
                        SubmittedDate = c.DateTime(nullable: false),
                        ExpirationDate = c.DateTime(nullable: false),
                        FinalizedDate = c.DateTime(nullable: false),
                        GrandTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Vendors", t => t.VendorId, cascadeDelete: true)
                .Index(t => t.VendorId);
            
            CreateTable(
                "dbo.LineItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PoFormId = c.Int(nullable: false),
                        AccountNumber = c.String(),
                        Description = c.String(),
                        Quantity = c.String(),
                        UnitCost = c.String(),
                        Total = c.String(),
                        Created = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        Modified = c.DateTime(nullable: false),
                        ModifiedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PoForms", t => t.PoFormId, cascadeDelete: true)
                .Index(t => t.PoFormId);
            
            CreateTable(
                "dbo.Vendors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Zip = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "PoFormId", "dbo.PoForms");
            DropForeignKey("dbo.PoForms", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.LineItems", "PoFormId", "dbo.PoForms");
            DropIndex("dbo.LineItems", new[] { "PoFormId" });
            DropIndex("dbo.PoForms", new[] { "VendorId" });
            DropIndex("dbo.Documents", new[] { "PoFormId" });
            DropTable("dbo.Vendors");
            DropTable("dbo.LineItems");
            DropTable("dbo.PoForms");
            DropTable("dbo.Documents");
        }
    }
}

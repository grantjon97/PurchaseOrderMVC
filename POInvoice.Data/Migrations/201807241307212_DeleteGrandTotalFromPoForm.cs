namespace POInvoice.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteGrandTotalFromPoForm : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PoForms", "GrandTotal");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PoForms", "GrandTotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

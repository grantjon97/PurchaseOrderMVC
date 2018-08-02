namespace POInvoice.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeLineItemTypes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LineItems", "Quantity", c => c.Int(nullable: false));
            AlterColumn("dbo.LineItems", "UnitCost", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LineItems", "UnitCost", c => c.String());
            AlterColumn("dbo.LineItems", "Quantity", c => c.String());
        }
    }
}

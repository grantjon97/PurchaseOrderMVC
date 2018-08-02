namespace POInvoice.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteLineItemTotal : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.LineItems", "Total");
        }
        
        public override void Down()
        {
            AddColumn("dbo.LineItems", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

namespace POInvoice.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTotalFromStringToDecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LineItems", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.LineItems", "Total", c => c.String());
        }
    }
}

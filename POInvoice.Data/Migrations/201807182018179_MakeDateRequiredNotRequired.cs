namespace POInvoice.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeDateRequiredNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PoForms", "DateRequired", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PoForms", "DateRequired", c => c.DateTime(nullable: false));
        }
    }
}

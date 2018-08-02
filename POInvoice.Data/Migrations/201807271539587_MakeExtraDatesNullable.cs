namespace POInvoice.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeExtraDatesNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PoForms", "SubmittedDate", c => c.DateTime());
            AlterColumn("dbo.PoForms", "ExpirationDate", c => c.DateTime());
            AlterColumn("dbo.PoForms", "FinalizedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PoForms", "FinalizedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PoForms", "ExpirationDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PoForms", "SubmittedDate", c => c.DateTime(nullable: false));
        }
    }
}

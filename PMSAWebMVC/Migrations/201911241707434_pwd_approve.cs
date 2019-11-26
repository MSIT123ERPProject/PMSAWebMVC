namespace PMSAWebMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pwd_approve : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AppUsers", "LastPasswordChangedDate", c => c.DateTime());
            AlterColumn("dbo.AppUsers", "IsApproved", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AppUsers", "IsApproved", c => c.Boolean(nullable: false));
            AlterColumn("dbo.AppUsers", "LastPasswordChangedDate", c => c.DateTime(nullable: false));
        }
    }
}

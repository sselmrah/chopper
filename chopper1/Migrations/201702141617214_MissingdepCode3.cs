namespace chopper1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissingdepCode3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "departmentCode");
        }
        
        public override void Down()
        {
        }
    }
}

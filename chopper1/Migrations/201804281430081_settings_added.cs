namespace chopper1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class settings_added : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Red1 = c.String(),
                        Red2 = c.String(),
                        Red3 = c.String(),
                        Green1 = c.String(),
                        Green2 = c.String(),
                        Green3 = c.String(),
                        BaseShare = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StepShare = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Settings");
        }
    }
}

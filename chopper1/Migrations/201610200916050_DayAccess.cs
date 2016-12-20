namespace chopper1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DayAccess : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DayAccesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TvDayRef = c.String(),
                        Permitted = c.String(),
                        SemiPermitted = c.String(),
                        Forbidden = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DayAccesses");
        }
    }
}

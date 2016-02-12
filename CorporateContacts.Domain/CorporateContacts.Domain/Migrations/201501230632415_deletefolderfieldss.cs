namespace CorporateContacts.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletefolderfieldss : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tblFolders", "Name3", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tblFolders", "Name3", c => c.String(maxLength: 200));
        }
    }
}

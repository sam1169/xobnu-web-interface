namespace CorporateContacts.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deletefolderfields : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tblFolders", "Name2");
            DropColumn("dbo.tblFolders", "Name3");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tblFolders", "Name3", c => c.String());
            AddColumn("dbo.tblFolders", "Name2", c => c.String());
        }
    }
}

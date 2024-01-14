using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManagementWithIdentity.Data.Migrations
{
    public partial class assignUserAdminIntoRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "INSERT INTO [security].[UserRoles] (UserId,RoleId) SELECT '7436c172-fba2-4c6d-b9ef-ab65e5fa0512',Id from [security].[Roles]"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE from [security].[UserRoles] WHERE UserId='7436c172-fba2-4c6d-b9ef-ab65e5fa0512' ");
        }
    }
}

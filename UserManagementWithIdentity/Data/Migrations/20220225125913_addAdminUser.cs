using Microsoft.EntityFrameworkCore.Migrations;

namespace UserManagementWithIdentity.Data.Migrations
{
    public partial class addAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [security].[Users] ([Id], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount], [FirstName], [LastName], [ProfilePicture]) VALUES (N'7436c172-fba2-4c6d-b9ef-ab65e5fa0512', N'admin', N'ADMIN', N'admin@test.com', N'ADMIN@TEST.COM', 0, N'AQAAAAEAACcQAAAAEE4x7l0BwlpAcsxueAqi0BWQgG7vf4UddxQqwriAzzb/XrUe+0WnETFi00BQ+fbpWg==', N'6SGSRHFT3EJFOMVLOBOGUUAD4F6SF4PI', N'd6b2b3fb-666b-4ccf-b88a-051872c57b4f', NULL, 0, 0, NULL, 1, 0, N'mohamed', N'mohamed', null) ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELET FROM [security].[Users] WHERE Id='7436c172-fba2-4c6d-b9ef-ab65e5fa0512' ");
        }
    }
}

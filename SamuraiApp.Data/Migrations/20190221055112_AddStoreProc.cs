using Microsoft.EntityFrameworkCore.Migrations;

namespace SamuraiApp.Data.Migrations
{
    public partial class AddStoreProc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"CREATE PROCEDURE FilterSamuraiByNamePart
                    @namepart varchar(50)
                    AS
                        Select * From Samurais Where name Like '%'+@namepart+'%'"
                );
            migrationBuilder.Sql(
                @"CREATE PROCEDURE FindLongestName
                    @procResult varchar(50) OUTPUT
                    AS
                    BEGIN
                        Select top 1 @procResult = name from Samurais Order By len(name) desc
                    END"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

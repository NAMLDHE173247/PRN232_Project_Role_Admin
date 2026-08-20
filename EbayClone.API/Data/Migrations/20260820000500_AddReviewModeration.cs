using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbayClone.API.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260820000500_AddReviewModeration")]
public partial class AddReviewModeration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF COL_LENGTH('Review', 'status') IS NULL
BEGIN
    ALTER TABLE [Review]
    ADD [status] nvarchar(20) NOT NULL
        CONSTRAINT [DF_Review_AdminStatus] DEFAULT N'Visible' WITH VALUES;
END");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF COL_LENGTH('Review', 'status') IS NOT NULL
BEGIN
    ALTER TABLE [Review] DROP CONSTRAINT [DF_Review_AdminStatus];
    ALTER TABLE [Review] DROP COLUMN [status];
END");
    }
}

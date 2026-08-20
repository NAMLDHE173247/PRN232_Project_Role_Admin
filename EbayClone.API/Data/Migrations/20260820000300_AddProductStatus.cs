using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EbayClone.API.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260820000300_AddProductStatus")]
public partial class AddProductStatus : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF COL_LENGTH('Product', 'status') IS NULL
BEGIN
    ALTER TABLE [Product]
    ADD [status] nvarchar(20) NOT NULL
        CONSTRAINT [DF_Product_AdminStatus] DEFAULT N'Active' WITH VALUES;
END");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF COL_LENGTH('Product', 'status') IS NOT NULL
BEGIN
    ALTER TABLE [Product] DROP CONSTRAINT [DF_Product_AdminStatus];
    ALTER TABLE [Product] DROP COLUMN [status];
END");
    }
}

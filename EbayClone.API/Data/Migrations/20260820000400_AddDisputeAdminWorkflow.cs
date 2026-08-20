using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EbayClone.API.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260820000400_AddDisputeAdminWorkflow")]
public partial class AddDisputeAdminWorkflow : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF COL_LENGTH('Dispute', 'assignedTo') IS NULL
    ALTER TABLE [Dispute] ADD [assignedTo] int NULL;
IF COL_LENGTH('Dispute', 'assignedAt') IS NULL
    ALTER TABLE [Dispute] ADD [assignedAt] datetime2 NULL;
IF COL_LENGTH('Dispute', 'resolvedBy') IS NULL
    ALTER TABLE [Dispute] ADD [resolvedBy] int NULL;
IF COL_LENGTH('Dispute', 'resolvedAt') IS NULL
    ALTER TABLE [Dispute] ADD [resolvedAt] datetime2 NULL;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
IF COL_LENGTH('Dispute', 'resolvedAt') IS NOT NULL
    ALTER TABLE [Dispute] DROP COLUMN [resolvedAt];
IF COL_LENGTH('Dispute', 'resolvedBy') IS NOT NULL
    ALTER TABLE [Dispute] DROP COLUMN [resolvedBy];
IF COL_LENGTH('Dispute', 'assignedAt') IS NOT NULL
    ALTER TABLE [Dispute] DROP COLUMN [assignedAt];
IF COL_LENGTH('Dispute', 'assignedTo') IS NOT NULL
    ALTER TABLE [Dispute] DROP COLUMN [assignedTo];");
    }
}

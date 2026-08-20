using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace EbayClone.API.Data.Migrations;

[Migration("20260820000100_AddAdminUserManagementFields")]
[DbContext(typeof(AppDbContext))]
public partial class AddAdminUserManagementFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'Status') IS NULL ALTER TABLE [User] ADD [Status] nvarchar(20) NOT NULL CONSTRAINT [DF_User_Status] DEFAULT 'Active';");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'ApprovalStatus') IS NULL ALTER TABLE [User] ADD [ApprovalStatus] nvarchar(30) NOT NULL CONSTRAINT [DF_User_ApprovalStatus] DEFAULT 'Approved';");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'ApprovedBy') IS NULL ALTER TABLE [User] ADD [ApprovedBy] int NULL;");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'ApprovedAt') IS NULL ALTER TABLE [User] ADD [ApprovedAt] datetime2 NULL;");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'BannedReason') IS NULL ALTER TABLE [User] ADD [BannedReason] nvarchar(max) NULL;");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'BannedBy') IS NULL ALTER TABLE [User] ADD [BannedBy] int NULL;");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'BannedAt') IS NULL ALTER TABLE [User] ADD [BannedAt] datetime2 NULL;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'Status') IS NOT NULL BEGIN IF OBJECT_ID('[DF_User_Status]', 'D') IS NOT NULL ALTER TABLE [User] DROP CONSTRAINT [DF_User_Status]; ALTER TABLE [User] DROP COLUMN [Status]; END;");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'ApprovalStatus') IS NOT NULL BEGIN IF OBJECT_ID('[DF_User_ApprovalStatus]', 'D') IS NOT NULL ALTER TABLE [User] DROP CONSTRAINT [DF_User_ApprovalStatus]; ALTER TABLE [User] DROP COLUMN [ApprovalStatus]; END;");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'ApprovedBy') IS NOT NULL ALTER TABLE [User] DROP COLUMN [ApprovedBy];");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'ApprovedAt') IS NOT NULL ALTER TABLE [User] DROP COLUMN [ApprovedAt];");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'BannedReason') IS NOT NULL ALTER TABLE [User] DROP COLUMN [BannedReason];");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'BannedBy') IS NOT NULL ALTER TABLE [User] DROP COLUMN [BannedBy];");
        migrationBuilder.Sql("IF COL_LENGTH('[User]', 'BannedAt') IS NOT NULL ALTER TABLE [User] DROP COLUMN [BannedAt];");
    }
}

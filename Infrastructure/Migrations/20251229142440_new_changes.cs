using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class new_changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CustomerAddresses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "CustomerAddresses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CustomerAddresses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "CustomerAddresses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CustomerAddresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CustomerAddresses",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "CustomerAddresses",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "CustomerAddresses",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "CustomerAddresses");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "CustomerAddresses");
        }
    }
}

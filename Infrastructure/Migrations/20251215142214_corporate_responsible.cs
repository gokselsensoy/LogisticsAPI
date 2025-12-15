using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class corporate_responsible : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateResponsibles_CorporateCustomers_CorporateCustomerId",
                table: "CorporateResponsibles");

            migrationBuilder.DropIndex(
                name: "IX_CorporateResponsibles_CorporateCustomerId",
                table: "CorporateResponsibles");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "CorporateResponsibles");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CorporateResponsibles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "CorporateResponsibles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CorporateResponsibles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "CorporateResponsibles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CorporateResponsibles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CorporateResponsibles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "CorporateResponsibles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "CorporateResponsibles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int[]>(
                name: "Roles",
                table: "CorporateResponsibles",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CorporateResponsibles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CorporateResponsibles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CorporateResponsibles");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "CorporateResponsibles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CorporateResponsibles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CorporateResponsibles");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "CorporateResponsibles");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "CorporateResponsibles");

            migrationBuilder.DropColumn(
                name: "Roles",
                table: "CorporateResponsibles");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "CorporateResponsibles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateResponsibles_CorporateCustomerId",
                table: "CorporateResponsibles",
                column: "CorporateCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateResponsibles_CorporateCustomers_CorporateCustomerId",
                table: "CorporateResponsibles",
                column: "CorporateCustomerId",
                principalTable: "CorporateCustomers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

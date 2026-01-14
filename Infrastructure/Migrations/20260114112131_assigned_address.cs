using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class assigned_address : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateAddressResponsibleMap_CorporateResponsibles_Corpor~",
                table: "CorporateAddressResponsibleMap");

            migrationBuilder.DropIndex(
                name: "IX_CorporateAddressResponsibleMap_CorporateResponsibleId",
                table: "CorporateAddressResponsibleMap");

            migrationBuilder.DropColumn(
                name: "CorporateResponsibleId",
                table: "CorporateAddressResponsibleMap");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateAddressResponsibleMap_CorporateResponsibles_Respon~",
                table: "CorporateAddressResponsibleMap",
                column: "ResponsibleId",
                principalTable: "CorporateResponsibles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateAddressResponsibleMap_CorporateResponsibles_Respon~",
                table: "CorporateAddressResponsibleMap");

            migrationBuilder.AddColumn<Guid>(
                name: "CorporateResponsibleId",
                table: "CorporateAddressResponsibleMap",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateAddressResponsibleMap_CorporateResponsibleId",
                table: "CorporateAddressResponsibleMap",
                column: "CorporateResponsibleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateAddressResponsibleMap_CorporateResponsibles_Corpor~",
                table: "CorporateAddressResponsibleMap",
                column: "CorporateResponsibleId",
                principalTable: "CorporateResponsibles",
                principalColumn: "Id");
        }
    }
}

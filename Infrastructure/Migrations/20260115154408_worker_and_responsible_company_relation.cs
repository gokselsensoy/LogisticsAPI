using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class worker_and_responsible_company_relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Companies",
                type: "text",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateResponsibles_CorporateCustomers_CorporateCustomerId",
                table: "CorporateResponsibles");

            migrationBuilder.DropIndex(
                name: "IX_CorporateResponsibles_CorporateCustomerId",
                table: "CorporateResponsibles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Companies");
        }
    }
}

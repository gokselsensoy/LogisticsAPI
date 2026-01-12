using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class terminal_service_radius : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CorporateAddressResponsibleMap",
                table: "CorporateAddressResponsibleMap");

            migrationBuilder.AddColumn<double>(
                name: "ServiceRadiusKm",
                table: "Terminals",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CorporateAddressResponsibleMap",
                table: "CorporateAddressResponsibleMap",
                columns: new[] { "ResponsibleId", "AddressId" });

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_DepartmentId",
                table: "Terminals",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_PackageId",
                table: "Stocks",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateAddressResponsibleMap_AddressId",
                table: "CorporateAddressResponsibleMap",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_CorporateAddressResponsibleMap_CustomerAddresses_AddressId",
                table: "CorporateAddressResponsibleMap",
                column: "AddressId",
                principalTable: "CustomerAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Suppliers_SupplierId",
                table: "Products",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Packages_PackageId",
                table: "Stocks",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Terminals_Departments_DepartmentId",
                table: "Terminals",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CorporateAddressResponsibleMap_CustomerAddresses_AddressId",
                table: "CorporateAddressResponsibleMap");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Suppliers_SupplierId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Packages_PackageId",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Terminals_Departments_DepartmentId",
                table: "Terminals");

            migrationBuilder.DropIndex(
                name: "IX_Terminals_DepartmentId",
                table: "Terminals");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_PackageId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Products_SupplierId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CorporateAddressResponsibleMap",
                table: "CorporateAddressResponsibleMap");

            migrationBuilder.DropIndex(
                name: "IX_CorporateAddressResponsibleMap_AddressId",
                table: "CorporateAddressResponsibleMap");

            migrationBuilder.DropColumn(
                name: "ServiceRadiusKm",
                table: "Terminals");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Products");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CorporateAddressResponsibleMap",
                table: "CorporateAddressResponsibleMap",
                column: "Id");
        }
    }
}

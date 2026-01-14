using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class turn_geography : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "LastKnownLocation",
                table: "Vehicles",
                type: "geography (point, 4326)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 4326)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Point>(
                name: "Address_Location",
                table: "Terminals",
                type: "geography (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Pic_Location",
                table: "Shipments",
                type: "geography (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Del_Location",
                table: "Shipments",
                type: "geography (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Target_Location",
                table: "RouteTasks",
                type: "geography (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Pickup_Location",
                table: "ReturnRequests",
                type: "geography (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Del_Location",
                table: "Orders",
                type: "geography (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Address_Location",
                table: "Departments",
                type: "geography (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Address_Location",
                table: "CustomerAddresses",
                type: "geography (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry (point, 4326)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "LastKnownLocation",
                table: "Vehicles",
                type: "geometry (point, 4326)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geography (point, 4326)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Point>(
                name: "Address_Location",
                table: "Terminals",
                type: "geometry (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Pic_Location",
                table: "Shipments",
                type: "geometry (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Del_Location",
                table: "Shipments",
                type: "geometry (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Target_Location",
                table: "RouteTasks",
                type: "geometry (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Pickup_Location",
                table: "ReturnRequests",
                type: "geometry (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Del_Location",
                table: "Orders",
                type: "geometry (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Address_Location",
                table: "Departments",
                type: "geometry (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (point, 4326)");

            migrationBuilder.AlterColumn<Point>(
                name: "Address_Location",
                table: "CustomerAddresses",
                type: "geometry (point, 4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography (point, 4326)");
        }
    }
}

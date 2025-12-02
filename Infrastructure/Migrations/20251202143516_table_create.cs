using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class table_create : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "ShippingZipCode",
                table: "Orders",
                newName: "Del_ZipCode");

            migrationBuilder.RenameColumn(
                name: "ShippingStreet",
                table: "Orders",
                newName: "Del_Street");

            migrationBuilder.RenameColumn(
                name: "ShippingCity",
                table: "Orders",
                newName: "Del_City");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Orders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Del_ZipCode",
                table: "Orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Del_Street",
                table: "Orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Del_City",
                table: "Orders",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Contact_Email",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contact_Name",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Contact_Phone",
                table: "Orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Orders",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerAddressId",
                table: "Orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Del_BuildingNo",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Del_Country",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Del_Door",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Del_FloorLabel",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Del_FloorNumber",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Del_FormattedAddress",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Point>(
                name: "Del_Location",
                table: "Orders",
                type: "geometry (point, 4326)",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ExternalReferenceCode",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Origin",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Payment_Channel",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Payment_LogSettlement",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Payment_ProdSettlement",
                table: "Orders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "NameSnapshot",
                table: "OrderItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "PackageId",
                table: "OrderItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price_Amount",
                table: "OrderItems",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Price_Currency",
                table: "OrderItems",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Spec_Desc",
                table: "OrderItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Spec_Volume",
                table: "OrderItems",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Spec_Weight",
                table: "OrderItems",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdentityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CvrNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyWorkSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ShiftStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShiftEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyWorkSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryPlans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransporterId = table.Column<Guid>(type: "uuid", nullable: false),
                    FreelancerId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlanDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Freelancers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CvrNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Freelancers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationCode = table.Column<string>(type: "text", nullable: false),
                    IsVirtual = table.Column<bool>(type: "boolean", nullable: false),
                    Area = table.Column<string>(type: "text", nullable: false),
                    Corridor = table.Column<string>(type: "text", nullable: true),
                    Place = table.Column<string>(type: "text", nullable: true),
                    Shelf = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityChange = table.Column<int>(type: "integer", nullable: false),
                    QuantityAfter = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReturnRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Contact_Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Contact_Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Contact_Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OriginalOrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Pickup_Location = table.Column<Point>(type: "geometry (point, 4326)", nullable: false),
                    Pickup_Street = table.Column<string>(type: "text", nullable: false),
                    Pickup_BuildingNo = table.Column<string>(type: "text", nullable: false),
                    Pickup_ZipCode = table.Column<string>(type: "text", nullable: false),
                    Pickup_City = table.Column<string>(type: "text", nullable: false),
                    Pickup_Country = table.Column<string>(type: "text", nullable: false),
                    Pickup_FloorNumber = table.Column<int>(type: "integer", nullable: true),
                    Pickup_FloorLabel = table.Column<string>(type: "text", nullable: true),
                    Pickup_Door = table.Column<string>(type: "text", nullable: true),
                    Pickup_Address_Full = table.Column<string>(type: "text", nullable: false),
                    TargetTerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedShipmentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReturnRequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalShipmentRef = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    SourceType = table.Column<int>(type: "integer", nullable: false),
                    Pic_Location = table.Column<Point>(type: "geometry (point, 4326)", nullable: false),
                    Pic_Street = table.Column<string>(type: "text", nullable: false),
                    Pic_BuildingNo = table.Column<string>(type: "text", nullable: false),
                    Pic_ZipCode = table.Column<string>(type: "text", nullable: false),
                    Pic_City = table.Column<string>(type: "text", nullable: false),
                    Pic_Country = table.Column<string>(type: "text", nullable: false),
                    Pic_FloorNumber = table.Column<int>(type: "integer", nullable: true),
                    Pic_FloorLabel = table.Column<string>(type: "text", nullable: true),
                    Pic_Door = table.Column<string>(type: "text", nullable: true),
                    Pic_FormattedAddress = table.Column<string>(type: "text", nullable: false),
                    Del_Location = table.Column<Point>(type: "geometry (point, 4326)", nullable: false),
                    Del_Street = table.Column<string>(type: "text", nullable: false),
                    Del_BuildingNo = table.Column<string>(type: "text", nullable: false),
                    Del_ZipCode = table.Column<string>(type: "text", nullable: false),
                    Del_City = table.Column<string>(type: "text", nullable: false),
                    Del_Country = table.Column<string>(type: "text", nullable: false),
                    Del_FloorNumber = table.Column<int>(type: "integer", nullable: true),
                    Del_FloorLabel = table.Column<string>(type: "text", nullable: true),
                    Del_Door = table.Column<string>(type: "text", nullable: true),
                    Del_FormattedAddress = table.Column<string>(type: "text", nullable: false),
                    AssignedTransporterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeeklyShiftPatterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    ShiftStart = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ShiftEnd = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DefaultVehicleId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyShiftPatterns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Address_Location = table.Column<Point>(type: "geometry (point, 4326)", nullable: false),
                    Address_Street = table.Column<string>(type: "text", nullable: false),
                    Address_BuildingNo = table.Column<string>(type: "text", nullable: false),
                    Address_ZipCode = table.Column<string>(type: "text", nullable: false),
                    Address_City = table.Column<string>(type: "text", nullable: false),
                    Address_Country = table.Column<string>(type: "text", nullable: false),
                    Address_FloorNumber = table.Column<int>(type: "integer", nullable: true),
                    Address_FloorLabel = table.Column<string>(type: "text", nullable: true),
                    Address_Door = table.Column<string>(type: "text", nullable: true),
                    Address_Full = table.Column<string>(type: "text", nullable: false),
                    ContactPhone = table.Column<string>(type: "text", nullable: true),
                    ContactEmail = table.Column<string>(type: "text", nullable: true),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Suppliers_Companies_Id",
                        column: x => x.Id,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Terminals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address_Location = table.Column<Point>(type: "geometry (point, 4326)", nullable: false),
                    Address_Street = table.Column<string>(type: "text", nullable: false),
                    Address_BuildingNo = table.Column<string>(type: "text", nullable: false),
                    Address_ZipCode = table.Column<string>(type: "text", nullable: false),
                    Address_City = table.Column<string>(type: "text", nullable: false),
                    Address_Country = table.Column<string>(type: "text", nullable: false),
                    Address_FloorNumber = table.Column<int>(type: "integer", nullable: true),
                    Address_FloorLabel = table.Column<string>(type: "text", nullable: true),
                    Address_Door = table.Column<string>(type: "text", nullable: true),
                    Address_FormattedAddress = table.Column<string>(type: "text", nullable: false),
                    ContactPhone = table.Column<string>(type: "text", nullable: true),
                    ContactEmail = table.Column<string>(type: "text", nullable: true),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terminals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terminals_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transporters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transporters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transporters_Companies_Id",
                        column: x => x.Id,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Roles = table.Column<int[]>(type: "integer[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorporateCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CvrNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateCustomers_Customers_Id",
                        column: x => x.Id,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Address_Location = table.Column<Point>(type: "geometry (point, 4326)", nullable: false),
                    Address_Street = table.Column<string>(type: "text", nullable: false),
                    Address_BuildingNo = table.Column<string>(type: "text", nullable: false),
                    Address_ZipCode = table.Column<string>(type: "text", nullable: false),
                    Address_City = table.Column<string>(type: "text", nullable: false),
                    Address_Country = table.Column<string>(type: "text", nullable: false),
                    Address_FloorNumber = table.Column<int>(type: "integer", nullable: true),
                    Address_FloorLabel = table.Column<string>(type: "text", nullable: true),
                    Address_Door = table.Column<string>(type: "text", nullable: true),
                    Address_Full = table.Column<string>(type: "text", nullable: false),
                    AddressType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IndividualCustomers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndividualCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndividualCustomers_Customers_Id",
                        column: x => x.Id,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyWorkScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleAllocations_DailyWorkSchedules_DailyWorkScheduleId",
                        column: x => x.DailyWorkScheduleId,
                        principalTable: "DailyWorkSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: true),
                    FreelancerId = table.Column<Guid>(type: "uuid", nullable: true),
                    RouteDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeliveryPlanId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Routes_DeliveryPlans_DeliveryPlanId",
                        column: x => x.DeliveryPlanId,
                        principalTable: "DeliveryPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlateNumber = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    MaxWeightKg = table.Column<double>(type: "double precision", nullable: false),
                    MaxVolumeM3 = table.Column<double>(type: "double precision", nullable: false),
                    LastKnownLocation = table.Column<Point>(type: "geometry (point, 4326)", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: true),
                    FreelancerId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicles_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Vehicles_Freelancers_FreelancerId",
                        column: x => x.FreelancerId,
                        principalTable: "Freelancers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryLocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_InventoryLocations_InventoryLocationId",
                        column: x => x.InventoryLocationId,
                        principalTable: "InventoryLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReturnItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<int>(type: "integer", nullable: false),
                    ReturnRequestId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnItems_ReturnRequests_ReturnRequestId",
                        column: x => x.ReturnRequestId,
                        principalTable: "ReturnRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShipmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: true),
                    Spec_Desc = table.Column<string>(type: "text", nullable: false),
                    Spec_Weight = table.Column<double>(type: "double precision", nullable: false),
                    Spec_Volume = table.Column<double>(type: "double precision", nullable: false),
                    PlannedQuantity = table.Column<int>(type: "integer", nullable: false),
                    LoadedQuantity = table.Column<int>(type: "integer", nullable: false),
                    DeliveredQuantity = table.Column<int>(type: "integer", nullable: false),
                    RejectedQuantity = table.Column<int>(type: "integer", nullable: false),
                    RejectionReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentItems_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShiftPatternItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WeeklyShiftPatternId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DefaultVehicleId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftPatternItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShiftPatternItems_WeeklyShiftPatterns_WeeklyShiftPatternId",
                        column: x => x.WeeklyShiftPatternId,
                        principalTable: "WeeklyShiftPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorporateResponsibles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorporateCustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateResponsibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateResponsibles_CorporateCustomers_CorporateCustomerId",
                        column: x => x.CorporateCustomerId,
                        principalTable: "CorporateCustomers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Target_Location = table.Column<Point>(type: "geometry (point, 4326)", nullable: false),
                    Target_Street = table.Column<string>(type: "text", nullable: false),
                    Target_BuildingNo = table.Column<string>(type: "text", nullable: false),
                    Target_ZipCode = table.Column<string>(type: "text", nullable: false),
                    Target_City = table.Column<string>(type: "text", nullable: false),
                    Target_Country = table.Column<string>(type: "text", nullable: false),
                    Target_FloorNumber = table.Column<int>(type: "integer", nullable: true),
                    Target_FloorLabel = table.Column<string>(type: "text", nullable: true),
                    Target_Door = table.Column<string>(type: "text", nullable: true),
                    Target_FormattedAddress = table.Column<string>(type: "text", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReturnRequestId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteTasks_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Dim_Width = table.Column<double>(type: "double precision", nullable: false),
                    Dim_Height = table.Column<double>(type: "double precision", nullable: false),
                    Dim_Depth = table.Column<double>(type: "double precision", nullable: false),
                    AtomicQuantity = table.Column<int>(type: "integer", nullable: false),
                    BoxQuantity = table.Column<int>(type: "integer", nullable: false),
                    IsReturnable = table.Column<bool>(type: "boolean", nullable: false),
                    DepositPrice_Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DepositPrice_Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorporateAddressResponsibleMap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponsibleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    CorporateResponsibleId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateAddressResponsibleMap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorporateAddressResponsibleMap_CorporateResponsibles_Corpor~",
                        column: x => x.CorporateResponsibleId,
                        principalTable: "CorporateResponsibles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_IdentityId",
                table: "AppUsers",
                column: "IdentityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CorporateAddressResponsibleMap_CorporateResponsibleId",
                table: "CorporateAddressResponsibleMap",
                column: "CorporateResponsibleId");

            migrationBuilder.CreateIndex(
                name: "IX_CorporateResponsibles_CorporateCustomerId",
                table: "CorporateResponsibles",
                column: "CorporateCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_CustomerId",
                table: "CustomerAddresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CompanyId",
                table: "Departments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryLocations_TerminalId_LocationCode",
                table: "InventoryLocations",
                columns: new[] { "TerminalId", "LocationCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Packages_ProductId",
                table: "Packages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SupplierId",
                table: "Products",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnItems_ReturnRequestId",
                table: "ReturnItems",
                column: "ReturnRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DeliveryPlanId",
                table: "Routes",
                column: "DeliveryPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteTasks_RouteId",
                table: "RouteTasks",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleAllocations_DailyWorkScheduleId",
                table: "ScheduleAllocations",
                column: "DailyWorkScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ShiftPatternItems_WeeklyShiftPatternId",
                table: "ShiftPatternItems",
                column: "WeeklyShiftPatternId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentItems_ShipmentId",
                table: "ShipmentItems",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_InventoryLocationId",
                table: "Stocks",
                column: "InventoryLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Terminals_CompanyId",
                table: "Terminals",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_CompanyId",
                table: "Vehicles",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_FreelancerId",
                table: "Vehicles",
                column: "FreelancerId");

            migrationBuilder.CreateIndex(
                name: "IX_Workers_CompanyId",
                table: "Workers",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "CorporateAddressResponsibleMap");

            migrationBuilder.DropTable(
                name: "CustomerAddresses");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "IndividualCustomers");

            migrationBuilder.DropTable(
                name: "InventoryTransactions");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "ReturnItems");

            migrationBuilder.DropTable(
                name: "RouteTasks");

            migrationBuilder.DropTable(
                name: "ScheduleAllocations");

            migrationBuilder.DropTable(
                name: "ShiftPatternItems");

            migrationBuilder.DropTable(
                name: "ShipmentItems");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Terminals");

            migrationBuilder.DropTable(
                name: "Transporters");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "Workers");

            migrationBuilder.DropTable(
                name: "CorporateResponsibles");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ReturnRequests");

            migrationBuilder.DropTable(
                name: "Routes");

            migrationBuilder.DropTable(
                name: "DailyWorkSchedules");

            migrationBuilder.DropTable(
                name: "WeeklyShiftPatterns");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "InventoryLocations");

            migrationBuilder.DropTable(
                name: "Freelancers");

            migrationBuilder.DropTable(
                name: "CorporateCustomers");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "DeliveryPlans");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropColumn(
                name: "Contact_Email",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Contact_Name",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Contact_Phone",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CustomerAddressId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Del_BuildingNo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Del_Country",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Del_Door",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Del_FloorLabel",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Del_FloorNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Del_FormattedAddress",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Del_Location",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExternalReferenceCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_Channel",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_LogSettlement",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Payment_ProdSettlement",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "NameSnapshot",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "PackageId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Price_Amount",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Price_Currency",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Spec_Desc",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Spec_Volume",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Spec_Weight",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "Del_ZipCode",
                table: "Orders",
                newName: "ShippingZipCode");

            migrationBuilder.RenameColumn(
                name: "Del_Street",
                table: "Orders",
                newName: "ShippingStreet");

            migrationBuilder.RenameColumn(
                name: "Del_City",
                table: "Orders",
                newName: "ShippingCity");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShippingZipCode",
                table: "Orders",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingStreet",
                table: "Orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ShippingCity",
                table: "Orders",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Orders",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "OrderItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "OrderItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    IdentityId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_IdentityId",
                table: "Users",
                column: "IdentityId",
                unique: true);
        }
    }
}

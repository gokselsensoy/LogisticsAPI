using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class full_audited_worker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Workers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Workers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Workers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Workers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Workers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Workers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Workers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Workers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "WeeklyShiftPatterns",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "WeeklyShiftPatterns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WeeklyShiftPatterns",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "WeeklyShiftPatterns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "WeeklyShiftPatterns",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WeeklyShiftPatterns",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "WeeklyShiftPatterns",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "WeeklyShiftPatterns",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Vehicles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Vehicles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Vehicles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Vehicles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "Vehicles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ScheduleAllocations",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ScheduleAllocations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ScheduleAllocations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ScheduleAllocations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ScheduleAllocations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ScheduleAllocations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "ScheduleAllocations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "ScheduleAllocations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DailyWorkSchedules",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "DailyWorkSchedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DailyWorkSchedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "DailyWorkSchedules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "DailyWorkSchedules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DailyWorkSchedules",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedAt",
                table: "DailyWorkSchedules",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "DailyWorkSchedules",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "WeeklyShiftPatterns");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "WeeklyShiftPatterns");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WeeklyShiftPatterns");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "WeeklyShiftPatterns");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "WeeklyShiftPatterns");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WeeklyShiftPatterns");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "WeeklyShiftPatterns");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "WeeklyShiftPatterns");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ScheduleAllocations");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ScheduleAllocations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ScheduleAllocations");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ScheduleAllocations");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ScheduleAllocations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ScheduleAllocations");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "ScheduleAllocations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "ScheduleAllocations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DailyWorkSchedules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DailyWorkSchedules");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DailyWorkSchedules");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "DailyWorkSchedules");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "DailyWorkSchedules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DailyWorkSchedules");

            migrationBuilder.DropColumn(
                name: "LastModifiedAt",
                table: "DailyWorkSchedules");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "DailyWorkSchedules");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicSystem.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingChangeHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_BookingChanges_Action",
                table: "booking_changes");

            migrationBuilder.AddColumn<DateOnly>(
                name: "new_slot_date",
                table: "booking_changes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "new_slot_time",
                table: "booking_changes",
                type: "time(0)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "old_slot_date",
                table: "booking_changes",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "old_slot_time",
                table: "booking_changes",
                type: "time(0)",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_BookingChanges_Action",
                table: "booking_changes",
                sql: "action IN ('create', 'change_type', 'cancel','reschedule')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_BookingChanges_Action",
                table: "booking_changes");

            migrationBuilder.DropColumn(
                name: "new_slot_date",
                table: "booking_changes");

            migrationBuilder.DropColumn(
                name: "new_slot_time",
                table: "booking_changes");

            migrationBuilder.DropColumn(
                name: "old_slot_date",
                table: "booking_changes");

            migrationBuilder.DropColumn(
                name: "old_slot_time",
                table: "booking_changes");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BookingChanges_Action",
                table: "booking_changes",
                sql: "action IN ('create', 'change_type', 'cancel')");
        }
    }
}

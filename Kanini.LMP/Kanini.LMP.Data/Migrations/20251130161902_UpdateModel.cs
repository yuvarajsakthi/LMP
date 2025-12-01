using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kanini.LMP.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisbursementTransactionId",
                table: "LoanAccounts");

            migrationBuilder.AddColumn<int>(
                name: "Channel",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSent",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationId",
                keyValue: 1,
                columns: new[] { "Channel", "ExternalId", "IsSent", "Priority", "SentAt", "Type" },
                values: new object[] { 4, null, false, 1, null, 8 });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationId",
                keyValue: 2,
                columns: new[] { "Channel", "ExternalId", "IsSent", "Priority", "SentAt", "Type" },
                values: new object[] { 4, null, false, 1, null, 8 });

            migrationBuilder.UpdateData(
                table: "Notifications",
                keyColumn: "NotificationId",
                keyValue: 3,
                columns: new[] { "Channel", "ExternalId", "IsSent", "Priority", "SentAt", "Type" },
                values: new object[] { 4, null, false, 1, null, 8 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsSent",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "DisbursementTransactionId",
                table: "LoanAccounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "LoanAccounts",
                keyColumn: "LoanAccountId",
                keyValue: 1,
                column: "DisbursementTransactionId",
                value: null);

            migrationBuilder.UpdateData(
                table: "LoanAccounts",
                keyColumn: "LoanAccountId",
                keyValue: 2,
                column: "DisbursementTransactionId",
                value: null);
        }
    }
}

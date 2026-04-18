using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPGOInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddItemGSTFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HSNCode",
                table: "Items",
                type: "varchar(8)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGSTApplicable",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TaxType",
                table: "Items",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 7, 14, 33, 17, 293, DateTimeKind.Utc).AddTicks(6374));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HSNCode",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsGSTApplicable",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "TaxType",
                table: "Items");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 7, 14, 17, 29, 621, DateTimeKind.Utc).AddTicks(4599));
        }
    }
}

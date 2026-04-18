using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPGOInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 7, 12, 55, 31, 417, DateTimeKind.Utc).AddTicks(9196), "$2a$11$nNfRid0iG4ZITKxcMOQsaODRQEekR4GIbQBsxciEChA8cHwliJVJy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 7, 11, 10, 25, 632, DateTimeKind.Utc).AddTicks(6131), "$2a$11$0z1oV7v2zWlBqG2WwI//0.B1HkL3Jg00H.gP6K9V0/wA2J5t1rNFW" });
        }
    }
}

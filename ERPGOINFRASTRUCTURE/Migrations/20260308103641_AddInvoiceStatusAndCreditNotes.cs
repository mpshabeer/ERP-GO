using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERPGOInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceStatusAndCreditNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "GstSalesInvoices",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "PostedAt",
                table: "GstSalesInvoices",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostedBy",
                table: "GstSalesInvoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "GstSalesInvoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SupplyType",
                table: "GstSalesInvoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HSNCode",
                table: "GstSalesInvoiceItems",
                type: "varchar(8)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GstCreditNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditNoteNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OriginalInvoiceId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalGstAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GstCreditNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GstCreditNotes_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GstCreditNotes_GstSalesInvoices_OriginalInvoiceId",
                        column: x => x.OriginalInvoiceId,
                        principalTable: "GstSalesInvoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GstCreditNoteItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GstCreditNoteId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    HSNCode = table.Column<string>(type: "varchar(8)", nullable: true),
                    OriginalInvoiceItemId = table.Column<int>(type: "int", nullable: true),
                    Qty = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GstCreditNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GstCreditNoteItems_GstCreditNotes_GstCreditNoteId",
                        column: x => x.GstCreditNoteId,
                        principalTable: "GstCreditNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GstCreditNoteItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GstCreditNoteItems_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.CreateIndex(
                name: "IX_GstCreditNoteItems_GstCreditNoteId",
                table: "GstCreditNoteItems",
                column: "GstCreditNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_GstCreditNoteItems_ItemId",
                table: "GstCreditNoteItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GstCreditNoteItems_UnitId",
                table: "GstCreditNoteItems",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_GstCreditNotes_CustomerId",
                table: "GstCreditNotes",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_GstCreditNotes_OriginalInvoiceId",
                table: "GstCreditNotes",
                column: "OriginalInvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GstCreditNoteItems");

            migrationBuilder.DropTable(
                name: "GstCreditNotes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "GstSalesInvoices");

            migrationBuilder.DropColumn(
                name: "PostedAt",
                table: "GstSalesInvoices");

            migrationBuilder.DropColumn(
                name: "PostedBy",
                table: "GstSalesInvoices");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "GstSalesInvoices");

            migrationBuilder.DropColumn(
                name: "SupplyType",
                table: "GstSalesInvoices");

            migrationBuilder.DropColumn(
                name: "HSNCode",
                table: "GstSalesInvoiceItems");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 7, 14, 33, 17, 293, DateTimeKind.Utc).AddTicks(6374));
        }
    }
}

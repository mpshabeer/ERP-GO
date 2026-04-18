using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ERPGODomain.Entities;

namespace ERPGOAPI.Reports;

public class SalesInvoiceDocument : IDocument
{
    private readonly SalesInvoice _invoice;
    private readonly string _companyName;
    private readonly string _companyAddress;

    // Colour constants (hex strings used directly)
    private const string PrimaryColor = "#1565C0";
    private const string AccentColorHex = "#E3F2FD";
    private const string TextLight = "#78909C";
    private const string LineColorHex = "#E0E0E0";

    public SalesInvoiceDocument(SalesInvoice invoice,
        string companyName = "ERP Go Edition",
        string companyAddress = "Your Company Address, City, Country")
    {
        _invoice = invoice;
        _companyName = companyName;
        _companyAddress = companyAddress;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title   = $"Invoice {_invoice.InvoiceNo}",
        Author  = _companyName,
        Subject = "Sales Invoice"
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.MarginTop(30);
            page.MarginBottom(30);
            page.MarginHorizontal(36);
            page.DefaultTextStyle(ts => ts.FontSize(9).FontFamily("Arial"));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    // ─────────────── HEADER ───────────────
    void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(_companyName).FontSize(20).Bold().FontColor(PrimaryColor);
                    c.Item().Text(_companyAddress).FontSize(8).FontColor(TextLight);
                });

                row.ConstantItem(140).AlignRight().Column(c =>
                {
                    c.Item().Text("SALES INVOICE").FontSize(18).Bold().FontColor(PrimaryColor);
                    c.Item().Text($"# {_invoice.InvoiceNo}").FontSize(11).Bold().FontColor("#263238");
                });
            });

            col.Item().PaddingTop(8).LineHorizontal(2).LineColor(PrimaryColor);

            col.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("BILL TO").FontSize(7).Bold().FontColor(TextLight).LetterSpacing(0.08f);
                    c.Item().PaddingTop(2).Text(_invoice.Customer?.Name ?? "—").FontSize(10).Bold();
                    if (!string.IsNullOrWhiteSpace(_invoice.Customer?.Address))
                        c.Item().Text(_invoice.Customer.Address).FontSize(8).FontColor(TextLight);
                    if (!string.IsNullOrWhiteSpace(_invoice.Customer?.Mobile))
                        c.Item().Text($"📞 {_invoice.Customer.Mobile}").FontSize(8).FontColor(TextLight);
                });

                row.ConstantItem(190).Table(t =>
                {
                    t.ColumnsDefinition(cd =>
                    {
                        cd.RelativeColumn();
                        cd.RelativeColumn();
                    });

                    void AddRow(string label, string value)
                    {
                        t.Cell().Text(label).FontSize(8).FontColor(TextLight);
                        t.Cell().AlignRight().Text(value).FontSize(8);
                    }

                    AddRow("Invoice Date:", _invoice.Date.ToString("dd MMM yyyy"));
                    AddRow("Due Date:", _invoice.DueDate?.ToString("dd MMM yyyy") ?? "—");
                    AddRow("Payment Terms:", string.IsNullOrWhiteSpace(_invoice.PaymentTerms) ? "—" : _invoice.PaymentTerms);
                });
            });

            col.Item().PaddingTop(8).LineHorizontal(1).LineColor(LineColorHex);
        });
    }

    // ─────────────── CONTENT ───────────────
    void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().PaddingTop(12).Table(table =>
            {
                table.ColumnsDefinition(cd =>
                {
                    cd.ConstantColumn(24);
                    cd.RelativeColumn(4);
                    cd.RelativeColumn(1.5f);
                    cd.ConstantColumn(52);
                    cd.ConstantColumn(52);
                    cd.ConstantColumn(72);
                    cd.ConstantColumn(56);
                    cd.ConstantColumn(80);
                });

                // Header
                table.Header(h =>
                {
                    IContainer HeaderCell(IContainer c) =>
                        c.Background(PrimaryColor).PaddingVertical(5).PaddingHorizontal(4);

                    h.Cell().Element(HeaderCell).Text("#").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HeaderCell).Text("Item Description").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HeaderCell).Text("Unit").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HeaderCell).AlignRight().Text("Qty").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HeaderCell).AlignRight().Text("Qty/Base").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HeaderCell).AlignRight().Text("Rate").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HeaderCell).AlignRight().Text("Disc %").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HeaderCell).AlignRight().Text("Amount").FontSize(8).Bold().FontColor(Colors.White);
                });

                // Rows
                var items = _invoice.SalesInvoiceItems?.ToList() ?? new();
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var bgColor = i % 2 == 0 ? "#FFFFFF" : AccentColorHex;

                    IContainer DataCell(IContainer c) =>
                        c.Background(bgColor)
                         .BorderBottom(1).BorderColor(LineColorHex)
                         .PaddingVertical(4).PaddingHorizontal(4);

                    table.Cell().Element(DataCell).Text($"{i + 1}").FontSize(8).FontColor(TextLight);

                    table.Cell().Element(DataCell).Column(cc =>
                    {
                        cc.Item().Text(item.Item?.Name ?? "—").FontSize(9).Bold();
                        if (!string.IsNullOrWhiteSpace(item.Item?.ItemCode))
                            cc.Item().Text(item.Item.ItemCode).FontSize(7).FontColor(TextLight);
                    });

                    table.Cell().Element(DataCell)
                        .Text(item.Unit?.Name ?? item.ItemUnit?.Unit?.Name ?? "—").FontSize(8);
                    table.Cell().Element(DataCell).AlignRight()
                        .Text(item.Qty.ToString("N3")).FontSize(8);
                    table.Cell().Element(DataCell).AlignRight()
                        .Text(item.QtyPerBaseUnit.ToString("N3")).FontSize(8);
                    table.Cell().Element(DataCell).AlignRight()
                        .Text(item.Rate.ToString("N2")).FontSize(8);
                    table.Cell().Element(DataCell).AlignRight()
                        .Text(item.DiscountPercent > 0 ? item.DiscountPercent.ToString("N1") + "%" : "—").FontSize(8);
                    table.Cell().Element(DataCell).AlignRight()
                        .Text(item.Amount.ToString("N2")).FontSize(9).Bold();
                }
            });

            // Totals
            col.Item().PaddingTop(12).AlignRight().Width(230).Table(t =>
            {
                t.ColumnsDefinition(cd =>
                {
                    cd.RelativeColumn();
                    cd.ConstantColumn(90);
                });

                void TotalRow(string label, string value)
                {
                    t.Cell().PaddingVertical(3).PaddingLeft(4).Text(label).FontSize(8).FontColor(TextLight);
                    t.Cell().AlignRight().PaddingVertical(3).PaddingRight(4).Text(value).FontSize(8).FontColor(TextLight);
                }

                TotalRow("Sub Total:", $"{_invoice.SubTotal:N2}");

                if (_invoice.DiscountAmount > 0)
                    TotalRow($"Discount ({_invoice.DiscountPercent:N1}%):", $"- {_invoice.DiscountAmount:N2}");

                if (_invoice.TaxAmount > 0)
                    TotalRow($"Tax ({_invoice.TaxPercent:N1}%):", $"+ {_invoice.TaxAmount:N2}");

                t.Cell().ColumnSpan(2).LineHorizontal(1.5f).LineColor(PrimaryColor);

                // Grand Total row (blue bar)
                t.Cell().Background(PrimaryColor).PaddingVertical(5).PaddingLeft(6)
                    .Text("TOTAL AMOUNT").FontSize(10).Bold().FontColor(Colors.White);
                t.Cell().Background(PrimaryColor).AlignRight().PaddingVertical(5).PaddingRight(6)
                    .Text($"{_invoice.TotalAmount:N2}").FontSize(11).Bold().FontColor(Colors.White);
            });

            // Notes
            if (!string.IsNullOrWhiteSpace(_invoice.Notes))
            {
                col.Item().PaddingTop(16).Column(c =>
                {
                    c.Item().Text("Notes").FontSize(8).Bold().FontColor(TextLight);
                    c.Item().PaddingTop(2).Text(_invoice.Notes).FontSize(8);
                });
            }

            // Signature area
            col.Item().PaddingTop(36).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().LineHorizontal(1).LineColor("#B0BEC5");
                    c.Item().PaddingTop(4).Text("Authorised Signature").FontSize(8).FontColor(TextLight).AlignCenter();
                });
                row.ConstantItem(60);
                row.RelativeItem().Column(c =>
                {
                    c.Item().LineHorizontal(1).LineColor("#B0BEC5");
                    c.Item().PaddingTop(4).Text("Customer Signature").FontSize(8).FontColor(TextLight).AlignCenter();
                });
            });
        });
    }

    // ─────────────── FOOTER ───────────────
    void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(1).LineColor(LineColorHex);
            col.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text($"Thank you for your business! — {_companyName}")
                    .FontSize(7).FontColor(TextLight).Italic();

                row.ConstantItem(100).AlignRight().Text(text =>
                {
                    text.Span("Page ").FontSize(7).FontColor(TextLight);
                    text.CurrentPageNumber().FontSize(7).FontColor(TextLight);
                    text.Span(" / ").FontSize(7).FontColor(TextLight);
                    text.TotalPages().FontSize(7).FontColor(TextLight);
                });
            });
        });
    }
}

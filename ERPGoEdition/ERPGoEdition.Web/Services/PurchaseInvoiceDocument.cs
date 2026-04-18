using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ERPGODomain.Entities;
using ERPGOAPPLICATION.Interfaces;

namespace ERPGoEdition.Web.Services;

public class PurchaseInvoiceDocument : IDocument
{
    private readonly Purchase _purchase;
    private readonly AppSettingsModel _settings;
    
    private string CompanyName => !string.IsNullOrWhiteSpace(_settings.CompanyName) ? _settings.CompanyName : "ERP Go Edition";
    private string CompanyAddress => _settings.CompanyAddress ?? "";

    private const string PrimaryColor = "#2E7D32"; // Green theme for purchases
    private const string AccentColorHex = "#E8F5E9";
    private const string TextLight = "#78909C";
    private const string LineColorHex = "#E0E0E0";

    public PurchaseInvoiceDocument(Purchase purchase, AppSettingsModel settings)
    {
        _purchase = purchase;
        _settings = settings;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title   = $"Purchase {_purchase.PurchaseNo}",
        Author  = CompanyName,
        Subject = "Purchase Invoice"
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

    void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(CompanyName).FontSize(20).Bold().FontColor(PrimaryColor);
                    
                    if (!string.IsNullOrWhiteSpace(CompanyAddress))
                        c.Item().Text(CompanyAddress).FontSize(8).FontColor(TextLight);
                    
                    if (!string.IsNullOrWhiteSpace(_settings.CompanyTaxNumber))
                        c.Item().Text($"Tax # {_settings.CompanyTaxNumber}").FontSize(8).FontColor(TextLight);
                        
                    var contactLine = new List<string>();
                    if (!string.IsNullOrWhiteSpace(_settings.CompanyPhone)) contactLine.Add($"Phone: {_settings.CompanyPhone}");
                    if (!string.IsNullOrWhiteSpace(_settings.CompanyEmail)) contactLine.Add($"Email: {_settings.CompanyEmail}");
                    if (contactLine.Any())
                        c.Item().Text(string.Join(" | ", contactLine)).FontSize(8).FontColor(TextLight);
                });
                row.ConstantItem(150).AlignRight().Column(c =>
                {
                    c.Item().Text("PURCHASE INVOICE").FontSize(16).Bold().FontColor(PrimaryColor);
                    c.Item().Text($"# {_purchase.PurchaseNo}").FontSize(11).Bold().FontColor("#263238");
                });
            });

            col.Item().PaddingTop(8).LineHorizontal(2).LineColor(PrimaryColor);

            col.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("VENDOR").FontSize(7).Bold().FontColor(TextLight).LetterSpacing(0.08f);
                    c.Item().PaddingTop(2).Text(_purchase.Supplier?.Name ?? "—").FontSize(10).Bold();
                    if (!string.IsNullOrWhiteSpace(_purchase.Supplier?.Address))
                        c.Item().Text(_purchase.Supplier.Address).FontSize(8).FontColor(TextLight);
                    if (!string.IsNullOrWhiteSpace(_purchase.Supplier?.Mobile))
                        c.Item().Text($"📞 {_purchase.Supplier.Mobile}").FontSize(8).FontColor(TextLight);
                });

                row.ConstantItem(190).Table(t =>
                {
                    t.ColumnsDefinition(cd => { cd.RelativeColumn(); cd.RelativeColumn(); });

                    void AddRow(string label, string value)
                    {
                        t.Cell().Text(label).FontSize(8).FontColor(TextLight);
                        t.Cell().AlignRight().Text(value).FontSize(8);
                    }

                    AddRow("Purchase Date:", _purchase.Date.ToString("dd MMM yyyy"));
                    AddRow("Supplier Inv #:", string.IsNullOrWhiteSpace(_purchase.InvoiceNo) ? "—" : _purchase.InvoiceNo);
                    AddRow("Due Date:", _purchase.DueDate?.ToString("dd MMM yyyy") ?? "—");
                    AddRow("Payment Terms:", string.IsNullOrWhiteSpace(_purchase.PaymentTerms) ? "—" : _purchase.PaymentTerms);
                });
            });

            col.Item().PaddingTop(8).LineHorizontal(1).LineColor(LineColorHex);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().PaddingTop(12).Table(table =>
            {
                table.ColumnsDefinition(cd =>
                {
                    cd.ConstantColumn(24); cd.RelativeColumn(4); cd.RelativeColumn(1.5f);
                    cd.ConstantColumn(52); cd.ConstantColumn(52); cd.ConstantColumn(72); cd.ConstantColumn(56); cd.ConstantColumn(80);
                });

                table.Header(h =>
                {
                    IContainer HC(IContainer c) =>
                        c.Background(PrimaryColor).PaddingVertical(5).PaddingHorizontal(4);

                    h.Cell().Element(HC).Text("#").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Item Description").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Unit").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Qty").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Qty/Base").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Rate").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Disc %").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Amount").FontSize(8).Bold().FontColor(Colors.White);
                });

                var items = _purchase.PurchaseItems?.ToList() ?? new();
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var bg = i % 2 == 0 ? "#FFFFFF" : AccentColorHex;

                    IContainer DC(IContainer c) =>
                        c.Background(bg).BorderBottom(1).BorderColor(LineColorHex)
                         .PaddingVertical(4).PaddingHorizontal(4);

                    table.Cell().Element(DC).Text($"{i + 1}").FontSize(8).FontColor(TextLight);
                    table.Cell().Element(DC).Column(cc =>
                    {
                        cc.Item().Text(item.Item?.Name ?? "—").FontSize(9).Bold();
                        if (!string.IsNullOrWhiteSpace(item.Item?.ItemCode))
                            cc.Item().Text(item.Item.ItemCode).FontSize(7).FontColor(TextLight);
                    });
                    table.Cell().Element(DC).Text(item.Unit?.Name ?? item.ItemUnit?.Unit?.Name ?? "—").FontSize(8);
                    table.Cell().Element(DC).AlignRight().Text(item.Qty.ToString("N3")).FontSize(8);
                    table.Cell().Element(DC).AlignRight().Text(item.QtyPerBaseUnit.ToString("N3")).FontSize(8);
                    table.Cell().Element(DC).AlignRight().Text(item.Rate.ToString("N2")).FontSize(8);
                    table.Cell().Element(DC).AlignRight()
                        .Text(item.DiscountPercent > 0 ? item.DiscountPercent.ToString("N1") + "%" : "—").FontSize(8);
                    table.Cell().Element(DC).AlignRight().Text(item.Amount.ToString("N2")).FontSize(9).Bold();
                }
            });

            // Totals
            col.Item().PaddingTop(12).AlignRight().Width(230).Table(t =>
            {
                t.ColumnsDefinition(cd => { cd.RelativeColumn(); cd.ConstantColumn(90); });

                void TR(string lbl, string val)
                {
                    t.Cell().PaddingVertical(3).PaddingLeft(4).Text(lbl).FontSize(8).FontColor(TextLight);
                    t.Cell().AlignRight().PaddingVertical(3).PaddingRight(4).Text(val).FontSize(8).FontColor(TextLight);
                }

                TR("Sub Total:", $"{_purchase.SubTotal:N2}");
                if (_purchase.DiscountAmount > 0)
                    TR($"Discount ({_purchase.DiscountPercent:N1}%):", $"- {_purchase.DiscountAmount:N2}");
                if (_purchase.TaxAmount > 0)
                    TR($"Tax ({_purchase.TaxPercent:N1}%):", $"+ {_purchase.TaxAmount:N2}");

                t.Cell().ColumnSpan(2).LineHorizontal(1.5f).LineColor(PrimaryColor);
                t.Cell().Background(PrimaryColor).PaddingVertical(5).PaddingLeft(6)
                    .Text("TOTAL AMOUNT").FontSize(10).Bold().FontColor(Colors.White);
                t.Cell().Background(PrimaryColor).AlignRight().PaddingVertical(5).PaddingRight(6)
                    .Text($"{_purchase.TotalAmount:N2}").FontSize(11).Bold().FontColor(Colors.White);
            });

            if (!string.IsNullOrWhiteSpace(_purchase.Notes))
            {
                col.Item().PaddingTop(16).Column(c =>
                {
                    c.Item().Text("Notes").FontSize(8).Bold().FontColor(TextLight);
                    c.Item().PaddingTop(2).Text(_purchase.Notes).FontSize(8);
                });
            }

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
                    c.Item().PaddingTop(4).Text("Prepared By").FontSize(8).FontColor(TextLight).AlignCenter();
                });
            });
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(1).LineColor(LineColorHex);
            col.Item().PaddingTop(4).Row(row =>
            {
                var footerMessage = !string.IsNullOrWhiteSpace(_settings.InvoiceFooterMessage) 
                    ? _settings.InvoiceFooterMessage 
                    : $"Thank you for your business! — {CompanyName}";
                    
                row.RelativeItem().Text(footerMessage).FontSize(7).FontColor(TextLight).Italic();
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

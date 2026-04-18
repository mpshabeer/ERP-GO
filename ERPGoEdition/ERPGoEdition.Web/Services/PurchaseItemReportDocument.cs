using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ERPGODomain.DTOs;
using ERPGOAPPLICATION.Interfaces;

namespace ERPGoEdition.Web.Services;

public class PurchaseItemReportDocument : IDocument
{
    private readonly PurchaseReportRequest _request;
    private readonly List<ItemWiseReportLine> _lines;
    private readonly AppSettingsModel _settings;
    
    private string CompanyName => !string.IsNullOrWhiteSpace(_settings.CompanyName) ? _settings.CompanyName : "ERP Go Edition";
    private string CompanyAddress => _settings.CompanyAddress ?? "";

    private const string PrimaryColor = "#F57C00"; // Orange for Purchase
    private const string AccentColorHex = "#FFF3E0"; // Light Orange
    private const string TextLight = "#78909C";
    private const string LineColorHex = "#E0E0E0";

    public PurchaseItemReportDocument(PurchaseReportRequest request, List<ItemWiseReportLine> lines, AppSettingsModel settings)
    {
        _request = request;
        _lines = lines;
        _settings = settings;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title   = "Item-Wise Purchase Report",
        Author  = CompanyName,
        Subject = "Purchase"
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
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
                        
                    var contactLine = new List<string>();
                    if (!string.IsNullOrWhiteSpace(_settings.CompanyPhone)) contactLine.Add($"Phone: {_settings.CompanyPhone}");
                    if (!string.IsNullOrWhiteSpace(_settings.CompanyEmail)) contactLine.Add($"Email: {_settings.CompanyEmail}");
                    if (contactLine.Any())
                        c.Item().Text(string.Join(" | ", contactLine)).FontSize(8).FontColor(TextLight);
                });
                row.ConstantItem(250).AlignRight().Column(c =>
                {
                    c.Item().Text("ITEM-WISE PURCHASE REPORT").FontSize(16).Bold().FontColor(PrimaryColor);
                    c.Item().Text($"From: {_request.FromDate:dd MMM yyyy}   To: {_request.ToDate:dd MMM yyyy}")
                            .FontSize(10).SemiBold().FontColor("#263238");
                });
            });

            col.Item().PaddingTop(8).LineHorizontal(2).LineColor(PrimaryColor);
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
                    cd.ConstantColumn(20);         // #
                    cd.ConstantColumn(60);         // Date
                    cd.ConstantColumn(70);         // Pur No
                    cd.RelativeColumn(1.5f);       // Supplier
                    cd.RelativeColumn(1.5f);       // Item Name
                    cd.ConstantColumn(60);         // Category
                    cd.RelativeColumn(0.6f);       // Qty
                    cd.RelativeColumn(0.6f);       // Qty/Base
                    cd.ConstantColumn(40);         // Unit
                    cd.RelativeColumn(0.8f);       // Rate
                    cd.RelativeColumn(1f);         // Amount
                });

                table.Header(h =>
                {
                    IContainer HC(IContainer c) =>
                        c.Background(PrimaryColor).PaddingVertical(5).PaddingHorizontal(4);

                    h.Cell().Element(HC).Text("#").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Date").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Pur No").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Supplier").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Item Name").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Category").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Qty").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Qty/Base").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Unit").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Rate").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Amount").FontSize(8).Bold().FontColor(Colors.White);
                });

                for (int i = 0; i < _lines.Count; i++)
                {
                    var item = _lines[i];
                    var bg = i % 2 == 0 ? "#FFFFFF" : AccentColorHex;

                    IContainer DC(IContainer c) =>
                        c.Background(bg).BorderBottom(1).BorderColor(LineColorHex)
                         .PaddingVertical(4).PaddingHorizontal(4);

                    table.Cell().Element(DC).Text($"{i + 1}").FontSize(8).FontColor(TextLight);
                    table.Cell().Element(DC).Text(item.Date.ToString("dd/MM/yyyy")).FontSize(8);
                    table.Cell().Element(DC).Text(item.DocumentNo).FontSize(8).SemiBold();
                    table.Cell().Element(DC).Text(item.PartyName).FontSize(8);
                    table.Cell().Element(DC).Text(item.ItemName).FontSize(8);
                    table.Cell().Element(DC).Text(item.CategoryName).FontSize(8).FontColor(TextLight);
                    
                    table.Cell().Element(DC).AlignRight().Text(item.Qty.ToString("N2")).FontSize(8).Bold();
                    table.Cell().Element(DC).AlignRight().Text(item.QtyPerBaseUnit.ToString("N2")).FontSize(8).Bold();
                    table.Cell().Element(DC).Text(item.UnitName).FontSize(8).FontColor(TextLight);
                    table.Cell().Element(DC).AlignRight().Text(item.Rate.ToString("N2")).FontSize(8);
                    table.Cell().Element(DC).AlignRight().Text(item.Amount.ToString("N2")).FontSize(8).Bold().FontColor(PrimaryColor);
                }

                if (_lines.Any())
                {
                    table.Cell().ColumnSpan(10).Background(AccentColorHex).BorderTop(1).BorderColor(PrimaryColor).PaddingVertical(6).PaddingHorizontal(4).AlignRight().Text("TOTAL PURCHASES:").FontSize(9).Bold().FontColor(PrimaryColor);
                    table.Cell().Background(AccentColorHex).BorderTop(1).BorderColor(PrimaryColor).PaddingVertical(6).PaddingHorizontal(4).AlignRight().Text(_lines.Sum(x => x.Amount).ToString("N2")).FontSize(9).Bold().FontColor(PrimaryColor);
                }
            });

            if (!_lines.Any())
            {
                col.Item().PaddingTop(20).AlignCenter().Text("No purchase transactions found for the selected criteria.").FontSize(10).Italic().FontColor(TextLight);
            }
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            var footerMsg = _settings.InvoiceFooterMessage ?? "Generated by ERP Go Edition";
            
            col.Item().AlignCenter().Text(footerMsg)
                .FontSize(8).Italic().FontColor(TextLight);

            col.Item().PaddingTop(4).AlignCenter().Text(text =>
            {
                text.Span("Page ").FontSize(8).FontColor(TextLight);
                text.CurrentPageNumber().FontSize(8).FontColor(TextLight);
                text.Span(" of ").FontSize(8).FontColor(TextLight);
                text.TotalPages().FontSize(8).FontColor(TextLight);
            });
        });
    }
}

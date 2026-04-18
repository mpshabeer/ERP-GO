using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ERPGODomain.DTOs;
using ERPGOAPPLICATION.Interfaces;

namespace ERPGoEdition.Web.Services;

public class AccountLedgerDocument : IDocument
{
    private readonly LedgerReportResult _result;
    private readonly AppSettingsModel _settings;
    
    private string CompanyName => !string.IsNullOrWhiteSpace(_settings.CompanyName) ? _settings.CompanyName : "ERP Go Edition";
    private string CompanyAddress => _settings.CompanyAddress ?? "";

    private const string PrimaryColor = "#1565C0"; 
    private const string AccentColorHex = "#E3F2FD"; 
    private const string TextLight = "#78909C";
    private const string LineColorHex = "#E0E0E0";

    public AccountLedgerDocument(LedgerReportResult result, AppSettingsModel settings)
    {
        _result = result;
        _settings = settings;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title   = $"Account Ledger - {_result.AccountName}",
        Author  = CompanyName,
        Subject = "Accounting"
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Portrait());
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
                    c.Item().Text("ACCOUNT LEDGER").FontSize(16).Bold().FontColor(PrimaryColor);
                    c.Item().Text($"Account: {_result.AccountName}").FontSize(11).Bold().FontColor("#263238");
                    
                    var dateStr = "All Dates";
                    if (_result.FromDate.HasValue && _result.ToDate.HasValue)
                        dateStr = $"From: {_result.FromDate:dd MMM yyyy}  To: {_result.ToDate:dd MMM yyyy}";
                    else if (_result.FromDate.HasValue)
                        dateStr = $"From: {_result.FromDate:dd MMM yyyy}";
                    else if (_result.ToDate.HasValue)
                        dateStr = $"To: {_result.ToDate:dd MMM yyyy}";

                    c.Item().Text(dateStr).FontSize(10).SemiBold().FontColor("#263238");
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
                    cd.ConstantColumn(60);         // Date
                    cd.ConstantColumn(60);         // Voucher No
                    cd.RelativeColumn();           // Description
                    cd.ConstantColumn(65);         // Debit
                    cd.ConstantColumn(65);         // Credit
                    cd.ConstantColumn(75);         // Balance
                });

                table.Header(h =>
                {
                    IContainer HC(IContainer c) => c.Background(PrimaryColor).PaddingVertical(5).PaddingHorizontal(4);

                    h.Cell().Element(HC).Text("Date").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Voucher No").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Particulars").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Debit").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Credit").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Balance").FontSize(8).Bold().FontColor(Colors.White);
                });

                // Opening Balance Row
                IContainer OBC(IContainer c) => c.Background("#F5F5F5").BorderBottom(1).BorderColor(LineColorHex).PaddingVertical(4).PaddingHorizontal(4);
                
                table.Cell().ColumnSpan(3).Element(OBC).AlignRight().Text("Opening Balance").FontSize(8).Bold();
                table.Cell().Element(OBC).AlignRight().Text("").FontSize(8);
                table.Cell().Element(OBC).AlignRight().Text("").FontSize(8);
                table.Cell().Element(OBC).AlignRight().Text(_result.OpeningBalance.ToString("N2")).FontSize(8).Bold();

                for (int i = 0; i < _result.Lines.Count; i++)
                {
                    var item = _result.Lines[i];
                    var bg = i % 2 == 0 ? "#FFFFFF" : AccentColorHex;

                    IContainer DC(IContainer c) => c.Background(bg).BorderBottom(1).BorderColor(LineColorHex).PaddingVertical(4).PaddingHorizontal(4);

                    table.Cell().Element(DC).Text(item.Date.ToString("dd/MM/yyyy")).FontSize(8);
                    table.Cell().Element(DC).Text(item.VoucherNo).FontSize(8).SemiBold();
                    table.Cell().Element(DC).Text(item.Description).FontSize(8);
                    
                    table.Cell().Element(DC).AlignRight().Text(item.Debit > 0 ? item.Debit.ToString("N2") : "-").FontSize(8).FontColor("#D32F2F");
                    table.Cell().Element(DC).AlignRight().Text(item.Credit > 0 ? item.Credit.ToString("N2") : "-").FontSize(8).FontColor("#388E3C");
                    table.Cell().Element(DC).AlignRight().Text(item.Balance.ToString("N2")).FontSize(8).Bold();
                }

                // Totals Row
                IContainer TC(IContainer c) => c.Background(AccentColorHex).BorderTop(1).BorderColor(PrimaryColor).PaddingVertical(6).PaddingHorizontal(4);
                
                table.Cell().ColumnSpan(3).Element(TC).AlignRight().Text("TOTALS:").FontSize(9).Bold().FontColor(PrimaryColor);
                table.Cell().Element(TC).AlignRight().Text(_result.TotalDebit.ToString("N2")).FontSize(9).Bold().FontColor(PrimaryColor);
                table.Cell().Element(TC).AlignRight().Text(_result.TotalCredit.ToString("N2")).FontSize(9).Bold().FontColor(PrimaryColor);
                table.Cell().Element(TC).AlignRight().Text(_result.ClosingBalance.ToString("N2")).FontSize(9).Bold().FontColor(PrimaryColor);
            });

            if (!_result.Lines.Any())
            {
                col.Item().PaddingTop(20).AlignCenter().Text("No transactions found for the selected period.").FontSize(10).Italic().FontColor(TextLight);
            }
        });
    }

    void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            var footerMsg = _settings.InvoiceFooterMessage ?? "Generated by ERP Go Edition";
            col.Item().AlignCenter().Text(footerMsg).FontSize(8).Italic().FontColor(TextLight);
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

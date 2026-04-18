using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ERPGODomain.DTOs;
using ERPGOAPPLICATION.Interfaces;

namespace ERPGoEdition.Web.Services;

public class TrialBalanceDocument : IDocument
{
    private readonly TrialBalanceResult _result;
    private readonly AppSettingsModel _settings;
    
    private string CompanyName => !string.IsNullOrWhiteSpace(_settings.CompanyName) ? _settings.CompanyName : "ERP Go Edition";
    private string CompanyAddress => _settings.CompanyAddress ?? "";

    private const string PrimaryColor = "#4527A0"; // Deep Purple for TB
    private const string AccentColorHex = "#EDE7F6"; 
    private const string TextLight = "#78909C";
    private const string LineColorHex = "#E0E0E0";

    public TrialBalanceDocument(TrialBalanceResult result, AppSettingsModel settings)
    {
        _result = result;
        _settings = settings;
    }

    public DocumentMetadata GetMetadata() => new DocumentMetadata
    {
        Title   = "Trial Balance",
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
                    c.Item().Text("TRIAL BALANCE").FontSize(16).Bold().FontColor(PrimaryColor);
                    c.Item().Text($"As of Date: {_result.AsOfDate:dd MMM yyyy}").FontSize(10).SemiBold().FontColor("#263238");
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
                    cd.ConstantColumn(100);        // Account Group
                    cd.RelativeColumn();           // Account Name
                    cd.ConstantColumn(80);         // Debit
                    cd.ConstantColumn(80);         // Credit
                });

                table.Header(h =>
                {
                    IContainer HC(IContainer c) => c.Background(PrimaryColor).PaddingVertical(5).PaddingHorizontal(4);

                    h.Cell().Element(HC).Text("Account Group").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).Text("Account Details").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Debit (Dr)").FontSize(8).Bold().FontColor(Colors.White);
                    h.Cell().Element(HC).AlignRight().Text("Credit (Cr)").FontSize(8).Bold().FontColor(Colors.White);
                });

                string currentGroup = "";

                for (int i = 0; i < _result.Lines.Count; i++)
                {
                    var item = _result.Lines[i];
                    var bg = i % 2 == 0 ? "#FFFFFF" : AccentColorHex;

                    IContainer DC(IContainer c) => c.Background(bg).BorderBottom(1).BorderColor(LineColorHex).PaddingVertical(4).PaddingHorizontal(4);

                    if (item.AccountGroupName != currentGroup)
                    {
                        currentGroup = item.AccountGroupName;
                        table.Cell().Element(DC).Text(currentGroup).FontSize(8).Bold().FontColor(PrimaryColor);
                    }
                    else
                    {
                        table.Cell().Element(DC).Text("");
                    }
                    
                    table.Cell().Element(DC).Column(c => 
                    {
                        c.Item().Text(item.AccountName).FontSize(8).SemiBold();
                        c.Item().Text(item.AccountHeadName).FontSize(7).FontColor(TextLight);
                    });
                    
                    table.Cell().Element(DC).AlignRight().Text(item.DebitBalance > 0 ? item.DebitBalance.ToString("N2") : "-").FontSize(8).FontColor("#D32F2F");
                    table.Cell().Element(DC).AlignRight().Text(item.CreditBalance > 0 ? item.CreditBalance.ToString("N2") : "-").FontSize(8).FontColor("#388E3C");
                }

                // Totals Row
                IContainer TC(IContainer c) => c.Background(AccentColorHex).BorderTop(1).BorderColor(PrimaryColor).PaddingVertical(6).PaddingHorizontal(4);
                
                table.Cell().ColumnSpan(2).Element(TC).AlignRight().Text("TOTALS:").FontSize(9).Bold().FontColor(PrimaryColor);
                table.Cell().Element(TC).AlignRight().Text(_result.TotalDebit.ToString("N2")).FontSize(9).Bold().FontColor(PrimaryColor);
                table.Cell().Element(TC).AlignRight().Text(_result.TotalCredit.ToString("N2")).FontSize(9).Bold().FontColor(PrimaryColor);
            });
            
            if (_result.TotalDebit != _result.TotalCredit)
            {
                col.Item().PaddingTop(10).AlignCenter().Text("TRIAL BALANCE DOES NOT MATCH!").FontSize(10).Bold().FontColor(Colors.Red.Medium);
            }

            if (!_result.Lines.Any())
            {
                col.Item().PaddingTop(20).AlignCenter().Text("No active accounts with balances found.").FontSize(10).Italic().FontColor(TextLight);
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

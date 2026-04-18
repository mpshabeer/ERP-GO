using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ERPGODomain.Entities;
using System.Linq;
using System;

namespace ERPGoEdition.Shared.Reports;

public class GstSalesInvoiceDocument : IDocument
{
    private readonly GstSalesInvoice _invoice;
    private readonly string _companyName;
    private readonly string _companyAddress;
    private readonly string _companyGstin;
    private readonly string _companyPhone;
    private readonly string _companyEmail;

    // Standard B&W/Gray theme for realistic invoices
    private const string BorderColor = "#000000";
    private const string TextColor   = "#000000";
    private const string RedColor    = "#000000"; // Pure B&W for standard ERP style

    public GstSalesInvoiceDocument(
        GstSalesInvoice invoice,
        string companyName    = "ERP GO EDITION PVT. LTD.",
        string companyAddress = "No. 1, Business Park, Industrial Area, City – 600001",
        string companyGstin   = "33AAAAB1234C1Z5",
        string companyPhone   = "+91-9876543210",
        string companyEmail   = "accounts@erpgo.com")
    {
        _invoice        = invoice;
        _companyName    = string.IsNullOrWhiteSpace(companyName) ? "COMPANY NAME" : companyName;
        _companyAddress = companyAddress;
        _companyGstin   = companyGstin;
        _companyPhone   = companyPhone;
        _companyEmail   = companyEmail;
    }

    public DocumentMetadata GetMetadata() => new()
    {
        Title   = $"GST Invoice {_invoice.InvoiceNo}",
        Author  = _companyName,
        Subject = "GST Tax Invoice"
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.MarginTop(30);
            page.MarginBottom(30);
            page.MarginHorizontal(36);
            page.DefaultTextStyle(ts => ts.FontSize(8).FontFamily(Fonts.Arial).FontColor(TextColor));

            page.Content().Element(ComposeInvoice);
            page.Footer().Element(ComposeFooter);
        });
    }

    // ══════════════════════════════════════════════════════════════
    // MAIN INVOICE CONTAINER
    // ══════════════════════════════════════════════════════════════
    void ComposeInvoice(IContainer container)
    {
        // Wrap everything in a main outer bordered box
        container.Border(1).BorderColor(BorderColor).Column(column =>
        {
            // 1. Company Header
            column.Item().BorderBottom(1).BorderColor(BorderColor).PaddingVertical(8).AlignCenter().Column(hc =>
            {
                hc.Item().AlignCenter().Text(_companyName).FontSize(16).Bold().FontColor(RedColor).LetterSpacing(0.05f);
                if (!string.IsNullOrWhiteSpace(_companyAddress))
                    hc.Item().AlignCenter().Text(_companyAddress).FontSize(8);
                if (!string.IsNullOrWhiteSpace(_companyGstin))
                    hc.Item().AlignCenter().Text($"GSTIN/UIN: {_companyGstin}").FontSize(8).Bold();
            });

            // 2. Tax Invoice Title Box
            column.Item().BorderBottom(1).BorderColor(BorderColor).Background("#F5F5F5").Padding(4).AlignCenter()
                .Text("TAX INVOICE").FontSize(9).Bold();

            // 3. Bill To & Invoice Meta Split
            column.Item().BorderBottom(1).BorderColor(BorderColor).MinHeight(60).Row(row =>
            {
                // Consignee (Ship to)
                row.RelativeItem(1).Padding(5).Column(leftCol =>
                {
                    leftCol.Item().Text("Consignee (Ship to)").FontSize(7);
                    leftCol.Item().PaddingTop(2).Text(_invoice.Customer?.Name ?? "—").FontSize(9).Bold();
                    if (!string.IsNullOrWhiteSpace(_invoice.Customer?.Address))
                        leftCol.Item().Text(_invoice.Customer.Address).FontSize(8);
                    
                    if (!string.IsNullOrWhiteSpace(_invoice.Customer?.Mobile))
                        leftCol.Item().PaddingTop(1).Text($"Mobile: {_invoice.Customer.Mobile}").FontSize(8);

                    leftCol.Item().PaddingTop(6).Text("State Name       : —").FontSize(8);
                });

                // Vertical Divider
                row.AutoItem().LineVertical(1).LineColor(BorderColor);

                // Meta Info
                row.RelativeItem(1).Padding(5).Column(rightCol =>
                {
                    void MetaRow(string key, string val)
                    {
                        rightCol.Item().PaddingBottom(2).Row(mr =>
                        {
                            mr.RelativeItem(1).Text(key).FontSize(8);
                            mr.AutoItem().Text(" : ").FontSize(8);
                            mr.RelativeItem(2).Text(val).FontSize(8).Bold();
                        });
                    }

                    MetaRow("Invoice No", _invoice.InvoiceNo);
                    MetaRow("Date", _invoice.Date.ToString("dd-MMM-yyyy"));
                    MetaRow("Due Date", _invoice.DueDate?.ToString("dd-MMM-yyyy") ?? "—");
                    MetaRow("Payment Terms", _invoice.PaymentTerms ?? "—");
                });
            });

            // 4. Main Item Table (occupies available vertical space if needed, but we keep it sequential)
            column.Item().Element(ComposeItemTable);

            long totalAmountRound = (long)Math.Round(_invoice.TotalAmount);

            // 5. Amount in words summary
            column.Item().BorderBottom(1).BorderColor(BorderColor).PaddingHorizontal(5).PaddingVertical(3).Row(row =>
            {
                row.RelativeItem().Column(mc =>
                {
                    mc.Item().Text("Amount Chargeable (in words)").FontSize(7);
                    mc.Item().Text($"INR {NumberToWords(totalAmountRound).Replace("  ", " ")} Only")
                        .FontSize(8).Bold();
                });
                row.AutoItem().AlignBottom().Text("E. & O.E").FontSize(7).Italic();
            });

            // 6. HSN Summary
            column.Item().BorderBottom(1).BorderColor(BorderColor).Element(ComposeHsnSummary);

            // 7. Footer details (Tax in words, Bank, Signature)
            column.Item().MinHeight(80).Row(row =>
            {
                // Left side: tax in words + declaration
                row.RelativeItem(5.5f).Padding(5).Column(lc =>
                {
                    long taxAmountRound = (long)Math.Round(_invoice.TotalGstAmount);
                    lc.Item().Text("Tax Amount (in words) :").FontSize(7);
                    lc.Item().Text($"INR {NumberToWords(taxAmountRound).Replace("  ", " ")} Only")
                        .FontSize(8).Bold();
                    
                    lc.Item().PaddingTop(15).Text("Declaration").FontSize(7).Underline();
                    lc.Item().Text("We declare that this invoice shows the actual price of the goods described and that all particulars are true and correct.")
                        .FontSize(7);
                });

                // Vertical divider
                row.AutoItem().LineVertical(1).LineColor(BorderColor);

                // Right side: Signature
                row.RelativeItem(4.5f).Column(rc =>
                {
                    // Signature Block
                    rc.Item().Padding(5).Column(sc =>
                    {
                        sc.Item().AlignRight().Text($"for {_companyName}").FontSize(8).Bold();
                        sc.Item().Height(30); // space for signature
                        sc.Item().AlignRight().Text("Authorised Signatory").FontSize(8);
                    });
                });
            });

        });
    }

    // ══════════════════════════════════════════════════════════════
    // ITEM TABLE
    // ══════════════════════════════════════════════════════════════
    void ComposeItemTable(IContainer container)
    {
        var items = _invoice.GstSalesInvoiceItems?.ToList() ?? new();
        bool isInterState = _invoice.SupplyType == "Inter-State";

        container.Table(table =>
        {
            table.ColumnsDefinition(cd =>
            {
                cd.ConstantColumn(22);  // Sl No.
                cd.RelativeColumn(3);   // Description
                cd.ConstantColumn(50);  // HSN/SAC
                cd.ConstantColumn(45);  // Quantity
                cd.ConstantColumn(50);  // Rate
                cd.ConstantColumn(30);  // per
                cd.ConstantColumn(60);  // Amount
            });

            IContainer HeaderCell(IContainer c) => c.BorderBottom(1).BorderColor(BorderColor).BorderRight(1).BorderColor(BorderColor).PaddingVertical(3).AlignCenter();
            IContainer LastHeaderCell(IContainer c) => c.BorderBottom(1).BorderColor(BorderColor).PaddingVertical(3).AlignCenter();

            // Header
            table.Header(h =>
            {
                h.Cell().Element(HeaderCell).Text("Sl No.").FontSize(7);
                h.Cell().Element(HeaderCell).Text("Description of Goods").FontSize(7);
                h.Cell().Element(HeaderCell).Text("HSN/SAC").FontSize(7);
                h.Cell().Element(HeaderCell).Text("Quantity").FontSize(7);
                h.Cell().Element(HeaderCell).Text("Rate").FontSize(7);
                h.Cell().Element(HeaderCell).Text("per").FontSize(7);
                h.Cell().Element(LastHeaderCell).Text("Amount").FontSize(7);
            });

            IContainer CellBlock(IContainer c) => c.BorderRight(1).BorderColor(BorderColor).PaddingHorizontal(4).PaddingVertical(3);
            IContainer LastCellBlock(IContainer c) => c.PaddingHorizontal(4).PaddingVertical(3);

            // Data Rows
            decimal totalQty = 0;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                totalQty += item.Qty;

                table.Cell().Element(CellBlock).AlignCenter().Text($"{i + 1}").FontSize(8);
                table.Cell().Element(CellBlock).Text(item.Item?.Name ?? "—").FontSize(8).Bold();
                table.Cell().Element(CellBlock).AlignCenter().Text(item.HSNCode ?? "—").FontSize(8);
                table.Cell().Element(CellBlock).AlignRight().Text($"{item.Qty:G.##}").FontSize(8).Bold();
                table.Cell().Element(CellBlock).AlignRight().Text($"{item.Rate:N2}").FontSize(8);
                table.Cell().Element(CellBlock).AlignCenter().Text(item.ItemUnit?.Unit?.Name ?? item.Unit?.Name ?? "—").FontSize(8);
                table.Cell().Element(LastCellBlock).AlignRight().Text($"{item.Amount:N2}").FontSize(8).Bold();
            }

            // Discount Row
            if (_invoice.DiscountAmount > 0)
            {
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock).AlignRight().Text("Less : DISCOUNT ALLOWED").FontSize(8).Italic();
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(LastCellBlock).AlignRight().Text($"(-){_invoice.DiscountAmount:N2}").FontSize(8);
            }

            // Tax Rows Added within the Item Table block 
            if (isInterState)
            {
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock).AlignRight().Text("IGST").FontSize(8).Bold();
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(LastCellBlock).AlignRight().Text($"{_invoice.TotalGstAmount:N2}").FontSize(8).Bold();
            }
            else
            {
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock).AlignRight().Text("CGST").FontSize(8).Bold();
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(LastCellBlock).AlignRight().Text($"{(_invoice.TotalGstAmount / 2):N2}").FontSize(8).Bold();

                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock).AlignRight().Text("SGST").FontSize(8).Bold();
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(CellBlock);
                table.Cell().Element(LastCellBlock).AlignRight().Text($"{(_invoice.TotalGstAmount / 2):N2}").FontSize(8).Bold();
            }

            // Spacer row to ensure the border draws cleanly down a bit if there are few items (without forcing new pages)
            table.Cell().Element(CellBlock).MinHeight(20);
            table.Cell().Element(CellBlock);
            table.Cell().Element(CellBlock);
            table.Cell().Element(CellBlock);
            table.Cell().Element(CellBlock);
            table.Cell().Element(CellBlock);
            table.Cell().Element(LastCellBlock);

            // Total Footer Row of Item Table
            IContainer TotCellBlock(IContainer c) => c.BorderTop(1).BorderColor(BorderColor).BorderRight(1).BorderColor(BorderColor).PaddingHorizontal(4).PaddingVertical(3);
            IContainer LastTotCellBlock(IContainer c) => c.BorderTop(1).BorderColor(BorderColor).PaddingHorizontal(4).PaddingVertical(3);

            table.Cell().Element(TotCellBlock);
            table.Cell().Element(TotCellBlock).AlignRight().Text("Total").FontSize(8).Bold();
            table.Cell().Element(TotCellBlock);
            table.Cell().Element(TotCellBlock).AlignRight().Text($"{totalQty:G.##}").FontSize(8).Bold();
            table.Cell().Element(TotCellBlock);
            table.Cell().Element(TotCellBlock);
            table.Cell().Element(LastTotCellBlock).AlignRight().Text($"₹ {_invoice.TotalAmount:N2}").FontSize(9).Bold();
        });
    }

    // ══════════════════════════════════════════════════════════════
    // HSN SUMMARY TABLE
    // ══════════════════════════════════════════════════════════════
    void ComposeHsnSummary(IContainer container)
    {
        var items = _invoice.GstSalesInvoiceItems?.ToList() ?? new();
        bool isInterState = _invoice.SupplyType == "Inter-State";

        var hsnGroups = items.GroupBy(x => new { x.HSNCode, x.GstPercent })
            .Select(g => new {
                HSNCode = string.IsNullOrWhiteSpace(g.Key.HSNCode) ? "Other" : g.Key.HSNCode,
                GstPercent = g.Key.GstPercent,
                TaxableValue = g.Sum(x => x.Amount),
                TaxAmount = g.Sum(x => x.GstAmount)
            }).OrderBy(x => x.HSNCode).ToList();

        container.Table(table =>
        {
            table.ColumnsDefinition(cd =>
            {
                cd.RelativeColumn();    // HSN/SAC
                cd.ConstantColumn(80);  // Taxable Value
                
                if (isInterState)
                {
                    cd.ConstantColumn(40); // IGST Rate
                    cd.ConstantColumn(60); // IGST Amount
                }
                else
                {
                    cd.ConstantColumn(40); // CGST Rate
                    cd.ConstantColumn(60); // CGST Amount
                    cd.ConstantColumn(40); // SGST Rate
                    cd.ConstantColumn(60); // SGST Amount
                }
                
                cd.ConstantColumn(70);  // Total Tax Amount
            });

            // Headers
            table.Header(h =>
            {
                IContainer HC(IContainer c) => c.BorderBottom(1).BorderColor(BorderColor).BorderRight(1).BorderColor(BorderColor).PaddingVertical(2).AlignCenter();
                IContainer LastHC(IContainer c) => c.BorderBottom(1).BorderColor(BorderColor).PaddingVertical(2).AlignCenter();

                // Top Header Row (spans for tax categories)
                h.Cell().RowSpan(2).Element(HC).AlignMiddle().Text("HSN/SAC").FontSize(7);
                h.Cell().RowSpan(2).Element(HC).AlignMiddle().Text("Taxable Value").FontSize(7);
                
                if (isInterState)
                {
                    h.Cell().ColumnSpan(2).Element(HC).Text("Integrated Tax").FontSize(7);
                }
                else
                {
                    h.Cell().ColumnSpan(2).Element(HC).Text("Central Tax").FontSize(7);
                    h.Cell().ColumnSpan(2).Element(HC).Text("State Tax").FontSize(7);
                }
                h.Cell().RowSpan(2).DefaultTextStyle(x => x.FontSize(7)).Element(LastHC).AlignMiddle().Text("Total\nTax Amount");

                // Bottom Header Row (Rate / Amount) - RowSpan pushes these under the tax groups automatically
                IContainer SubHC(IContainer c) => c.BorderBottom(1).BorderColor(BorderColor).BorderRight(1).BorderColor(BorderColor).PaddingVertical(2).AlignCenter();

                if (isInterState)
                {
                    h.Cell().Element(SubHC).Text("Rate").FontSize(7);
                    h.Cell().Element(SubHC).Text("Amount").FontSize(7);
                }
                else
                {
                    h.Cell().Element(SubHC).Text("Rate").FontSize(7);
                    h.Cell().Element(SubHC).Text("Amount").FontSize(7);
                    h.Cell().Element(SubHC).Text("Rate").FontSize(7);
                    h.Cell().Element(SubHC).Text("Amount").FontSize(7);
                }
            });

            IContainer CellBlock(IContainer c) => c.BorderRight(1).BorderColor(BorderColor).PaddingHorizontal(3).PaddingVertical(2);
            IContainer LastCellBlock(IContainer c) => c.PaddingHorizontal(3).PaddingVertical(2);
            
            decimal totTaxable = 0;
            decimal totTax = 0;

            foreach (var hsn in hsnGroups)
            {
                totTaxable += hsn.TaxableValue;
                totTax += hsn.TaxAmount;

                table.Cell().Element(CellBlock).Text(hsn.HSNCode).FontSize(7);
                table.Cell().Element(CellBlock).AlignRight().Text($"{hsn.TaxableValue:N2}").FontSize(7);
                
                if (isInterState)
                {
                    table.Cell().Element(CellBlock).AlignRight().Text($"{hsn.GstPercent:G.##}%").FontSize(7);
                    table.Cell().Element(CellBlock).AlignRight().Text($"{hsn.TaxAmount:N2}").FontSize(7);
                }
                else
                {
                    table.Cell().Element(CellBlock).AlignRight().Text($"{(hsn.GstPercent / 2):G.##}%").FontSize(7);
                    table.Cell().Element(CellBlock).AlignRight().Text($"{(hsn.TaxAmount / 2):N2}").FontSize(7);
                    table.Cell().Element(CellBlock).AlignRight().Text($"{(hsn.GstPercent / 2):G.##}%").FontSize(7);
                    table.Cell().Element(CellBlock).AlignRight().Text($"{(hsn.TaxAmount / 2):N2}").FontSize(7);
                }
                
                table.Cell().Element(LastCellBlock).AlignRight().Text($"{hsn.TaxAmount:N2}").FontSize(7);
            }

            // Total footer row for HSN summary
            IContainer TotCellBlock(IContainer c) => c.BorderTop(1).BorderColor(BorderColor).BorderRight(1).BorderColor(BorderColor).PaddingHorizontal(3).PaddingVertical(2);
            IContainer LastTotCellBlock(IContainer c) => c.BorderTop(1).BorderColor(BorderColor).PaddingHorizontal(3).PaddingVertical(2);

            table.Cell().Element(TotCellBlock).AlignRight().Text("Total").FontSize(7).Bold();
            table.Cell().Element(TotCellBlock).AlignRight().Text($"{totTaxable:N2}").FontSize(7).Bold();
            
            if (isInterState)
            {
                table.Cell().Element(TotCellBlock);
                table.Cell().Element(TotCellBlock).AlignRight().Text($"{totTax:N2}").FontSize(7).Bold();
            }
            else
            {
                table.Cell().Element(TotCellBlock);
                table.Cell().Element(TotCellBlock).AlignRight().Text($"{(totTax / 2):N2}").FontSize(7).Bold();
                table.Cell().Element(TotCellBlock);
                table.Cell().Element(TotCellBlock).AlignRight().Text($"{(totTax / 2):N2}").FontSize(7).Bold();
            }
            
            table.Cell().Element(LastTotCellBlock).AlignRight().Text($"{totTax:N2}").FontSize(7).Bold();
        });
    }

    // ══════════════════════════════════════════════════════════════
    // FOOTER (Outside the box)
    // ══════════════════════════════════════════════════════════════
    void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text("This is a Computer Generated Invoice").FontSize(7).AlignCenter();
            row.RelativeItem().AlignRight().Text(text =>
            {
                text.Span("Page ").FontSize(7);
                text.CurrentPageNumber().FontSize(7);
                text.Span(" of ").FontSize(7);
                text.TotalPages().FontSize(7);
            });
        });
    }

    // ══════════════════════════════════════════════════════════════
    // HELPER — Number to words (Indian system)
    // ══════════════════════════════════════════════════════════════
    private static string NumberToWords(long number)
    {
        if (number == 0) return "Zero";

        string[] ones = { "", "One", "Two", "Three", "Four", "Five", "Six",
                          "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve",
                          "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
                          "Eighteen", "Nineteen" };
        string[] tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty",
                          "Sixty", "Seventy", "Eighty", "Ninety" };

        string Words(long n)
        {
            if (n == 0)          return "";
            if (n < 20)          return ones[n] + " ";
            if (n < 100)         return tens[n / 10] + " " + Words(n % 10);
            if (n < 1_000)       return ones[n / 100] + " Hundred " + Words(n % 100);
            if (n < 1_00_000)    return Words(n / 1_000) + " Thousand " + Words(n % 1_000);
            if (n < 1_00_00_000) return Words(n / 1_00_000) + " Lakh " + Words(n % 1_00_000);
            return Words(n / 1_00_00_000) + " Crore " + Words(n % 1_00_00_000);
        }

        return Words(number).Trim();
    }
}

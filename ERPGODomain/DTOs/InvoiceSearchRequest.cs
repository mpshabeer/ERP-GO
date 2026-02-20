namespace ERPGODomain.DTOs;

/// <summary>Filter parameters for searching/paginating sales invoices.</summary>
public class InvoiceSearchRequest
{
    public string? InvoiceNo { get; set; }
    public string? CustomerName { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}

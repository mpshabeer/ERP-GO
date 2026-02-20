namespace ERPGODomain.DTOs;

public class PurchaseSearchRequest
{
    public string? PurchaseNo { get; set; }
    public string? InvoiceNo { get; set; } // Supplier Inv No
    public string? SupplierName { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}

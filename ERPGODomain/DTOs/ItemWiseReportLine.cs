namespace ERPGODomain.DTOs;

public class ItemWiseReportLine
{
    public DateTime Date { get; set; }
    public string DocumentNo { get; set; } = string.Empty; // Invoice No / Purchase No
    public string PartyName { get; set; } = string.Empty; // Customer Name / Supplier Name
    
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    
    public string UnitName { get; set; } = string.Empty;
    public decimal Qty { get; set; }
    public decimal QtyPerBaseUnit { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; } // Custom logic can be Qty * Rate or actual Line Item Amount (after discounts)
}

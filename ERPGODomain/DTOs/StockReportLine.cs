namespace ERPGODomain.DTOs;

public class StockReportLine
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    
    // Aggregated Quantities
    public decimal OpeningQty { get; set; }
    public decimal InwardQty { get; set; }
    public decimal OutwardQty { get; set; }
    
    // Calculated standard stock
    public decimal ClosingQty { get; set; } // Opening + In - Out
    
    // Valuation based on Item.PurchaseRate
    public decimal ItemRate { get; set; }
    public decimal StockValue { get; set; } // ClosingQty * ItemRate
}

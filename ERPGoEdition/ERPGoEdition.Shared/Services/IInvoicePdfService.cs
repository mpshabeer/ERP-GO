namespace ERPGoEdition.Shared.Services;

/// <summary>Generates PDF documents for ERP entities.</summary>
public interface IInvoicePdfService
{
    /// <summary>Generate a PDF for the given sales invoice ID and return the raw bytes.</summary>
    Task<byte[]> GenerateSalesInvoicePdfAsync(int invoiceId);

    /// <summary>Generate a PDF for the given GST sales invoice ID and return the raw bytes.</summary>
    Task<byte[]> GenerateGstSalesInvoicePdfAsync(int invoiceId);
}

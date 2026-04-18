using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface IGstCreditNoteService
{
    Task<GstCreditNote> CreateCreditNoteAsync(GstCreditNote note);
    Task<List<GstCreditNote>> GetAllAsync();
    Task<GstCreditNote?> GetByIdAsync(int id);
    Task<List<GstCreditNote>> GetByInvoiceIdAsync(int originalInvoiceId);
    Task<string> GetNextCreditNoteNumber();
}

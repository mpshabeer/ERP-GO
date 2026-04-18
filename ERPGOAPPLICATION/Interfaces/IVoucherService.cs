using ERPGODomain.DTOs;
using ERPGODomain.Entities;

namespace ERPGOAPPLICATION.Interfaces;

public interface IVoucherService
{
    // Receipts
    Task<Receipt> CreateReceiptAsync(ReceiptRequest request);
    Task<List<Receipt>> GetReceiptsAsync();
    
    // Payments
    Task<Payment> CreatePaymentAsync(PaymentRequest request);
    Task<List<Payment>> GetPaymentsAsync();
    
    // Expenses
    Task<Expense> CreateExpenseAsync(ExpenseRequest request);
    Task<List<Expense>> GetExpensesAsync();
}

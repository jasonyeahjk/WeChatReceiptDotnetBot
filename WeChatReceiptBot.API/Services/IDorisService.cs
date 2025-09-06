using WeChatReceiptBot.API.Models;

namespace WeChatReceiptBot.API.Services
{
    public interface IDorisService
    {
        // Transaction operations
        Task<bool> InsertAccountTransactionAsync(Transaction transaction, List<string> beneficiaryUserIds);
        Task<bool> UpdateAccountTransactionAsync(Transaction transaction);
        Task<Transaction?> GetAccountTransactionAsync(string transactionId);
        Task<List<Transaction>> GetAccountTransactionsByBillAsync(string billId);
        Task<List<Transaction>> GetAccountTransactionsByGroupAsync(string groupId);
        Task<List<Transaction>> GetAccountTransactionsByUserAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        
        // Payment record operations
        Task<bool> InsertPaymentRecordAsync(PaymentRecord paymentRecord);
        Task<bool> UpdatePaymentRecordAsync(PaymentRecord paymentRecord);
        Task<PaymentRecord?> GetPaymentRecordAsync(string paymentId);
        Task<List<PaymentRecord>> GetPaymentRecordsByUserAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<PaymentRecord>> GetPaymentRecordsByTransactionAsync(string transactionId);
        
        // User activity operations
        Task<bool> InsertUserActivityAsync(UserActivity activity);
        Task<List<UserActivity>> GetUserActivitiesAsync(string userId, int limit = 100);
        Task<List<UserActivity>> GetUserActivitiesByTypeAsync(string userId, string activityType, DateTime? startDate = null, DateTime? endDate = null);
        
        // Analytics and reporting
        Task<decimal> GetUserTotalExpenseAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetUserTotalIncomeAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetGroupTotalExpenseAsync(string groupId, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetBillTotalAmountAsync(string billId);
        Task<decimal> GetBillSettledAmountAsync(string billId);
        
        // Monthly/Daily summaries
        Task<List<(DateTime Date, decimal TotalExpense, decimal TotalIncome)>> GetUserMonthlySummaryAsync(string userId, int months = 12);
        Task<List<(DateTime Date, decimal TotalExpense, decimal TotalIncome)>> GetGroupMonthlySummaryAsync(string groupId, int months = 12);
        Task<List<(string Currency, decimal TotalAmount)>> GetUserExpenseByCurrencyAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        
        // Transaction statistics
        Task<int> GetUserTransactionCountAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetGroupTransactionCountAsync(string groupId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<(string PaymentMethod, int Count, decimal TotalAmount)>> GetPaymentMethodStatsAsync(string userId, DateTime? startDate = null, DateTime? endDate = null);
        
        // Unsettled transactions
        Task<List<Transaction>> GetUnsettledTransactionsByUserAsync(string userId);
        Task<List<Transaction>> GetUnsettledTransactionsByGroupAsync(string groupId);
        Task<List<Transaction>> GetUnsettledTransactionsByBillAsync(string billId);
        
        // Search and filtering
        Task<List<Transaction>> SearchTransactionsAsync(string searchTerm, string? userId = null, string? groupId = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<PaymentRecord>> SearchPaymentRecordsAsync(string searchTerm, string? userId = null, DateTime? startDate = null, DateTime? endDate = null);
        
        // Bulk operations
        Task<bool> BulkInsertAccountTransactionsAsync(List<(Transaction transaction, List<string> beneficiaryUserIds)> transactions);
        Task<bool> BulkInsertPaymentRecordsAsync(List<PaymentRecord> paymentRecords);
        Task<bool> BulkInsertUserActivitiesAsync(List<UserActivity> activities);
        
        // Data cleanup
        Task<bool> DeleteOldUserActivitiesAsync(DateTime beforeDate);
        Task<bool> ArchiveOldTransactionsAsync(DateTime beforeDate);
    }
}


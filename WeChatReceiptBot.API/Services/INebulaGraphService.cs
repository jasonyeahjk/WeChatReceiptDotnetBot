using WeChatReceiptBot.API.Models;

namespace WeChatReceiptBot.API.Services
{
    public interface INebulaGraphService
    {
        // User operations
        Task<bool> CreateUserVertexAsync(User user);
        Task<bool> UpdateUserVertexAsync(User user);
        Task<User?> GetUserVertexAsync(string userId);
        
        // Group operations
        Task<bool> CreateGroupVertexAsync(Group group);
        Task<bool> UpdateGroupVertexAsync(Group group);
        Task<Group?> GetGroupVertexAsync(string groupId);
        
        // Bill operations
        Task<bool> CreateBillVertexAsync(Bill bill);
        Task<bool> UpdateBillVertexAsync(Bill bill);
        Task<Bill?> GetBillVertexAsync(string billId);
        
        // Transaction operations
        Task<bool> CreateTransactionVertexAsync(Transaction transaction);
        Task<bool> UpdateTransactionVertexAsync(Transaction transaction);
        Task<Transaction?> GetTransactionVertexAsync(string transactionId);
        
        // Relationship operations
        Task<bool> CreateBelongsToEdgeAsync(string userId, string groupId, string role, DateTime joinedAt);
        Task<bool> CreateOwesEdgeAsync(string fromUserId, string toUserId, decimal amount, string currency, string status = "Unpaid");
        Task<bool> CreateParticipatesInEdgeAsync(string userId, string transactionId, decimal amountPaid);
        Task<bool> CreateIncursEdgeAsync(string transactionId, string userId, decimal amountOwed);
        Task<bool> CreateRecordedInEdgeAsync(string transactionId, string billId);
        Task<bool> CreateManagesEdgeAsync(string userId, string billId);
        Task<bool> CreateSettlesEdgeAsync(string userId, string transactionId, DateTime settledAt);
        
        // Query operations
        Task<List<Group>> GetUserGroupsAsync(string userId);
        Task<List<User>> GetGroupMembersAsync(string groupId);
        Task<List<Transaction>> GetBillTransactionsAsync(string billId);
        Task<List<(string UserId, decimal Amount, string Currency, string Status)>> GetUserOwesAsync(string userId);
        Task<List<(string UserId, decimal Amount, string Currency, string Status)>> GetUserOwedByAsync(string userId);
        Task<List<User>> GetTransactionParticipantsAsync(string transactionId);
        Task<List<User>> GetTransactionBeneficiariesAsync(string transactionId);
        
        // Analytics operations
        Task<decimal> GetUserTotalPaidInGroupAsync(string userId, string groupId);
        Task<decimal> GetUserTotalOwedInGroupAsync(string userId, string groupId);
        Task<List<(string UserId, decimal NetBalance)>> GetGroupBalancesAsync(string groupId);
        Task<bool> IsUserInGroupAsync(string userId, string groupId);
        Task<string?> GetUserRoleInGroupAsync(string userId, string groupId);
        
        // Cleanup operations
        Task<bool> DeleteUserVertexAsync(string userId);
        Task<bool> DeleteGroupVertexAsync(string groupId);
        Task<bool> DeleteBillVertexAsync(string billId);
        Task<bool> DeleteTransactionVertexAsync(string transactionId);
        Task<bool> RemoveUserFromGroupAsync(string userId, string groupId);
        Task<bool> UpdateOwesEdgeStatusAsync(string fromUserId, string toUserId, string status);
    }
}


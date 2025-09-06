using System.ComponentModel.DataAnnotations;
using WeChatReceiptBot.API.DTOs;


namespace WeChatReceiptBot.API.DTOs
{
    public class CreateBillRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string BillName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public Guid GroupId { get; set; }
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
        
        public bool IsAutoSettle { get; set; } = false;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
    
    public class BillResponse
    {
        public Guid Id { get; set; }
        public string BillName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public Guid CreatorId { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal SettledAmount { get; set; }
        public decimal PendingAmount { get; set; }
        public bool IsAutoSettle { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<TransactionSummaryResponse> Transactions { get; set; } = new();
        public List<BillMemberSummaryResponse> MemberSummaries { get; set; } = new();
    }
    
    public class BillSummaryResponse
    {
        public Guid Id { get; set; }
        public string BillName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal SettledAmount { get; set; }
        public int TransactionCount { get; set; }
    }
    
    public class BillMemberSummaryResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public decimal TotalPaid { get; set; }
        public decimal TotalOwed { get; set; }
        public decimal NetBalance { get; set; } // Positive means they are owed money, negative means they owe money
        public int TransactionCount { get; set; }
    }
    
    public class UpdateBillRequest
    {
        [StringLength(100)]
        public string BillName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
        
        public decimal TotalAmount { get; set; }
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
        
        public bool IsAutoSettle { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
}



    public class BillStatisticsResponse
    {
        public int TotalBills { get; set; }
        public int ActiveBills { get; set; }
        public int SettledBills { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal PendingAmount { get; set; }
    }

    public class BillDetailResponse
    {
        public Guid Id { get; set; }
        public string BillName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public Guid CreatorId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SettledAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<TransactionResponse> Transactions { get; set; } = new();
        public List<BillMemberShareResponse> MemberShares { get; set; } = new();
    }

    public class BillMemberShareResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public decimal ShareAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Balance { get; set; }
    }



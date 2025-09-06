using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.DTOs
{
    public class CreateTransactionRequest
    {
        [Required]
        public Guid BillId { get; set; }
        
        [Required]
        public Guid PayerId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
        
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string TransactionType { get; set; } = "Expense";
        
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        // Base64 encoded image for Donut recognition
        public string? ImageFile { get; set; }
        
        // List of beneficiary user IDs and their amounts
        [Required]
        public List<TransactionBeneficiary> Beneficiaries { get; set; } = new();
        
        // Whether to create a smart contract for this transaction
        public bool CreateSmartContract { get; set; } = false;
    }
    
    public class BeneficiaryRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }
    
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public string BillName { get; set; } = string.Empty;
        public Guid PayerId { get; set; }
        public string PayerUsername { get; set; } = string.Empty;
        public string PayerNickname { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<TransactionBeneficiary> Beneficiaries { get; set; } = new();
        public string? BlockchainTxHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
        public string? ContractAddress { get; set; }
        public string? TransactionHash { get; set; }
        public bool IsOnChain { get; set; }
        public bool HasImageRecognition { get; set; }
        public DonutRecognitionResponse? RecognitionData { get; set; }
    }
    
    public class BeneficiaryResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
    
    public class TransactionSummaryResponse
    {
        public Guid Id { get; set; }
        public string PayerUsername { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public bool IsSettled { get; set; }
        public string TransactionType { get; set; } = string.Empty;
    }
    
    public class SettleTransactionRequest
    {
        [Required]
        public string SettledByUserId { get; set; } = string.Empty;
        
        // Base64 encoded payment image for Donut recognition
        public string? PaymentImageFile { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        // Whether to create a smart contract for this payment
        public bool CreateSmartContract { get; set; } = false;
    }
    
    public class DonutRecognitionResponse
    {
        public decimal? ExtractedAmount { get; set; }
        public DateTime? ExtractedDate { get; set; }
        public string? ExtractedDescription { get; set; }
        public List<string> ExtractedItems { get; set; } = new();
        public string? Merchant { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Payer { get; set; }
        public string? Receiver { get; set; }
        public decimal Confidence { get; set; }
        public string RawData { get; set; } = string.Empty;
    }
}



    public class TransactionDetailResponse
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public Guid PayerId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<TransactionBeneficiary> Beneficiaries { get; set; } = new();
        public string? BlockchainTxHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TransactionBeneficiary
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class TransactionStatistics
    {
        public int TotalTransactions { get; set; }
        public int PendingTransactions { get; set; }
        public int ConfirmedTransactions { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal AverageTransactionAmount { get; set; }
    }




    public class UpdateTransactionRequest
    {
        [Required]
        public Guid BillId { get; set; }
        
        [Required]
        public Guid PayerId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
        
        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string TransactionType { get; set; } = "Expense";
        
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        // Base64 encoded image for Donut recognition
        public string? ImageFile { get; set; }
        
        // List of beneficiary user IDs and their amounts
        [Required]
        public List<TransactionBeneficiary> Beneficiaries { get; set; } = new();
        
        // Whether to create a smart contract for this transaction
        public bool CreateSmartContract { get; set; } = false;

        public string Status { get; set; } = string.Empty;
}



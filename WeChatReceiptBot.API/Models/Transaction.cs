using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.Models
{
    public class Transaction
    {
        [Key]
        public string TransactionId { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string BillId { get; set; } = string.Empty;
        
        [Required]
        public string PayerUserId { get; set; } = string.Empty;
        
        [Required]
        public decimal Amount { get; set; }
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
        
        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string TransactionType { get; set; } = "Expense"; // Expense, Income
        
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        public bool IsSettled { get; set; } = false;
        
        public string? SettledByUserId { get; set; }
        
        public DateTime? SettledAt { get; set; }
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        // JSON string containing beneficiary user IDs and their amounts
        [StringLength(2000)]
        public string BeneficiaryData { get; set; } = string.Empty;
        
        // Smart contract related fields
        [StringLength(42)]
        public string? ContractAddress { get; set; }
        
        [StringLength(66)]
        public string? TransactionHash { get; set; }
        
        public bool IsOnChain { get; set; } = false;
        
        // Donut recognition related fields
        public bool HasImageRecognition { get; set; } = false;
        
        [StringLength(2000)]
        public string? RecognitionData { get; set; } // JSON string from Donut service
    }
}


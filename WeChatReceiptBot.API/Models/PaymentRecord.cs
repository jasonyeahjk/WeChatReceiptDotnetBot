using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.Models
{
    public class PaymentRecord
    {
        [Key]
        public string PaymentId { get; set; } = Guid.NewGuid().ToString();
        
        public string? TransactionId { get; set; } // Optional: link to a specific transaction
        
        [Required]
        public string PayerUserId { get; set; } = string.Empty;
        
        [Required]
        public string ReceiverUserId { get; set; } = string.Empty;
        
        [Required]
        public decimal Amount { get; set; }
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
        
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // WeChat, Alipay, Bank Transfer, etc.
        
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Status { get; set; } = "Completed"; // Pending, Completed, Failed
        
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
        
        // Reference to external payment system
        [StringLength(100)]
        public string? ExternalPaymentId { get; set; }
        
        [StringLength(100)]
        public string? ExternalTransactionId { get; set; }
    }
}


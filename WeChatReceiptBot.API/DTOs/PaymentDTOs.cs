using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.DTOs
{
    public class CreatePaymentRecordRequest
    {
        public string? TransactionId { get; set; }
        
        [Required]
        public string PayerUserId { get; set; } = string.Empty;
        
        [Required]
        public string ReceiverUserId { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
        
        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;
        
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        // Base64 encoded image for Donut recognition
        public string? ImageFile { get; set; }
        
        [StringLength(100)]
        public string? ExternalPaymentId { get; set; }
        
        [StringLength(100)]
        public string? ExternalTransactionId { get; set; }
        
        // Whether to create a smart contract for this payment
        public bool CreateSmartContract { get; set; } = false;
    }
    
    public class PaymentRecordResponse
    {
        public string PaymentId { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
        public string PayerUserId { get; set; } = string.Empty;
        public string PayerUsername { get; set; } = string.Empty;
        public string PayerNickname { get; set; } = string.Empty;
        public string ReceiverUserId { get; set; } = string.Empty;
        public string ReceiverUsername { get; set; } = string.Empty;
        public string ReceiverNickname { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ContractAddress { get; set; }
        public string? TransactionHash { get; set; }
        public bool IsOnChain { get; set; }
        public bool HasImageRecognition { get; set; }
        public DonutRecognitionResponse? RecognitionData { get; set; }
        public string? ExternalPaymentId { get; set; }
        public string? ExternalTransactionId { get; set; }
    }
    
    public class PaymentSummaryResponse
    {
        public string PaymentId { get; set; } = string.Empty;
        public string PayerUsername { get; set; } = string.Empty;
        public string ReceiverUsername { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
    
    public class UpdatePaymentRecordRequest
    {
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? ExternalPaymentId { get; set; }
        
        [StringLength(100)]
        public string? ExternalTransactionId { get; set; }
    }
}


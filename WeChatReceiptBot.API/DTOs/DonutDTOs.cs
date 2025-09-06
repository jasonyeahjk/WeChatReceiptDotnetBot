using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.DTOs
{
    public class ReceiptRecognitionRequest
    {
        [Required]
        public string ImageBase64 { get; set; } = string.Empty;
        public Guid? GroupId { get; set; }
    }

    public class ReceiptRecognitionResponse
    {
        public Guid RecognitionId { get; set; }
        public float Confidence { get; set; }
        public string MerchantName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public List<ReceiptItem> Items { get; set; } = new();
        public string? SmartContractAddress { get; set; }
        public string? BlockchainTxHash { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    public class ReceiptItem
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class PaymentRecognitionRequest
    {
        [Required]
        public string ImageBase64 { get; set; } = string.Empty;
        public Guid? TransactionId { get; set; }
    }

    public class PaymentRecognitionResponse
    {
        public Guid RecognitionId { get; set; }
        public float Confidence { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string PayerName { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public string? BlockchainTxHash { get; set; }
        public DateTime ProcessedAt { get; set; }
    }

    public class DocumentRecognitionRequest
    {
        [Required]
        public string ImageBase64 { get; set; } = string.Empty;
        public string? DocumentType { get; set; }
    }

    public class DocumentRecognitionResponse
    {
        public Guid RecognitionId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public float Confidence { get; set; }
        public string ExtractedText { get; set; } = string.Empty;
        public Dictionary<string, object> StructuredData { get; set; } = new();
        public DateTime ProcessedAt { get; set; }
    }

    public class RecognitionHistory
    {
        public Guid Id { get; set; }
        public string RecognitionType { get; set; } = string.Empty;
        public float Confidence { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string MerchantName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class RecognitionDetail
    {
        public Guid Id { get; set; }
        public string RecognitionType { get; set; } = string.Empty;
        public float Confidence { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string OriginalImageUrl { get; set; } = string.Empty;
        public Dictionary<string, object> ExtractedData { get; set; } = new();
        public string? SmartContractAddress { get; set; }
        public string? BlockchainTxHash { get; set; }
    }
}



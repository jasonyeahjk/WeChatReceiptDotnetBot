using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.Models
{
    public class UserActivity
    {
        [Key]
        public string ActivityId { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string ActivityType { get; set; } = string.Empty; // Login, CreateBill, AddTransaction, ViewReport, etc.
        
        [StringLength(2000)]
        public string ActivityDetails { get; set; } = string.Empty; // JSON format for additional information
        
        [StringLength(45)]
        public string IpAddress { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string DeviceInfo { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string UserAgent { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Optional: link to specific entities
        public string? GroupId { get; set; }
        
        public string? BillId { get; set; }
        
        public string? TransactionId { get; set; }
        
        [StringLength(20)]
        public string Source { get; set; } = "Web"; // Web, Mobile, API, WeChat
        
        [StringLength(50)]
        public string SessionId { get; set; } = string.Empty;
        
        public bool IsSuccessful { get; set; } = true;
        
        [StringLength(500)]
        public string ErrorMessage { get; set; } = string.Empty;
    }
}


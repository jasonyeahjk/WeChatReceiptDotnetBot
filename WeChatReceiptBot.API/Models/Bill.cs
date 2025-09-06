using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.Models
{
    public class Bill
    {
        [Key]
        public string BillId { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string BillName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string GroupId { get; set; } = string.Empty;
        
        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
        
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Settled, Archived
        
        public decimal TotalAmount { get; set; } = 0;
        
        public decimal SettledAmount { get; set; } = 0;
        
        public bool IsAutoSettle { get; set; } = false;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }
}


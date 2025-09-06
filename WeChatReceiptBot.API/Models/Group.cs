using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.Models
{
    public class Group
    {
        [Key]
        public string GroupId { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(100)]
        public string GroupName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Completed, Archived
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
        
        public int MaxMembers { get; set; } = 50;
        
        public bool IsPublic { get; set; } = false;
        
        [StringLength(20)]
        public string InviteCode { get; set; } = string.Empty;
    }
}


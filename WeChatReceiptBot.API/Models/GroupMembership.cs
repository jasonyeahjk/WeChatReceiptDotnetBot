using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.Models
{
    public class GroupMembership
    {
        [Key]
        public string MembershipId { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public string GroupId { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Role { get; set; } = "Member"; // Creator, Admin, Member
        
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Left, Removed
        
        public bool CanManageBills { get; set; } = false;
        
        public bool CanInviteMembers { get; set; } = false;
        
        public bool CanViewAllTransactions { get; set; } = true;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        // Notification preferences
        public bool NotifyOnNewTransaction { get; set; } = true;
        
        public bool NotifyOnSettlement { get; set; } = true;
        
        public bool NotifyOnNewMember { get; set; } = true;
    }
}


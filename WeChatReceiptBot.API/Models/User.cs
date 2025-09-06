using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.Models
{
    public class User
    {
        [Key]
        public string UserId { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Nickname { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string AvatarUrl { get; set; } = string.Empty;
        
        [StringLength(42)]
        public string WalletAddress { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string WeChatOpenId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsActive { get; set; } = true;
    }
}


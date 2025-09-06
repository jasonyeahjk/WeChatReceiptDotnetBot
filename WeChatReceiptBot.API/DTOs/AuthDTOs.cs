using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.DTOs
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Nickname { get; set; } = string.Empty;
        
        [StringLength(42)]
        public string WalletAddress { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string WeChatOpenId { get; set; } = string.Empty;
    }
    
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
    
    public class AuthResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string WalletAddress { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserProfileResponse? User { get; set; }
    }
    
    public class UserProfileResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string WalletAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
    
    public class UpdateProfileRequest
    {
        [StringLength(50)]
        public string Nickname { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string AvatarUrl { get; set; } = string.Empty;
        
        [StringLength(42)]
        public string WalletAddress { get; set; } = string.Empty;
    }
}



    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshTokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }




    public class BindWalletRequest
    {
        [Required]
        public string WalletAddress { get; set; } = string.Empty;
    }



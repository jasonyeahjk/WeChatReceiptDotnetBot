using System.ComponentModel.DataAnnotations;

namespace WeChatReceiptBot.API.DTOs
{
    public class CreateGroupRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string GroupName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
        
        public int MaxMembers { get; set; } = 50;
        
        public bool IsPublic { get; set; } = false;
    }
    
    public class GroupResponse
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid CreatorId { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public int MaxMembers { get; set; }
        public int MemberCount { get; set; }
        public bool IsPublic { get; set; }
        public string InviteCode { get; set; } = string.Empty;
        public List<GroupMemberResponse> Members { get; set; } = new();
    }
    

    
    public class JoinGroupRequest
    {
        [Required]
        public Guid Id { get; set; }
        
        public string InviteCode { get; set; } = string.Empty;
    }
    
    public class UpdateGroupRequest
    {
        [StringLength(100)]
        public string GroupName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public int MaxMembers { get; set; }
        
        public bool IsPublic { get; set; }
        
        [StringLength(10)]
        public string Currency { get; set; } = "CNY";
    }
    
    public class UpdateMemberRoleRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty;
        
        public bool CanManageBills { get; set; }
        
        public bool CanInviteMembers { get; set; }
    }
}



    public class AddGroupMemberRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
    }

    public class GroupDetailResponse
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid CreatorId { get; set; }
        public int MemberCount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public List<GroupMemberResponse> Members { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public decimal SettledAmount { get; set; }
    }




    public class GroupMemberResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nickname { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool CanManageBills { get; set; }
        public bool CanInviteMembers { get; set; }
    }


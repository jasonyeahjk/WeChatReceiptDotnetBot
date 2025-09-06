namespace Web3Service.API.Domain.ValueObjects;

/// <summary>
/// 合约部署状态
/// </summary>
public enum DeploymentStatus
{
    /// <summary>
    /// 待部署
    /// </summary>
    Pending = 1,
    
    /// <summary>
    /// 部署中
    /// </summary>
    Deploying = 2,
    
    /// <summary>
    /// 已部署
    /// </summary>
    Deployed = 3,
    
    /// <summary>
    /// 部署失败
    /// </summary>
    Failed = 4,
    
    /// <summary>
    /// 已验证
    /// </summary>
    Verified = 5
}


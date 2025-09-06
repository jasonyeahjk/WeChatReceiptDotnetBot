using FluentResults;
using Web3Service.API.Application.DTOs.AccountDTOs;
using Web3Service.API.Application.DTOs.ContractDTOs;
using Web3Service.API.Application.DTOs.TransactionDTOs;
using Web3Service.API.Application.DTOs.Web3DTOs;

namespace Web3Service.API.Application.Interfaces;

/// <summary>
/// Web3应用服务接口
/// </summary>
public interface IWeb3ApplicationService
{
    // 账户管理
    /// <summary>
    /// 创建新账户
    /// </summary>
    Task<Result<AccountResponseDto>> CreateAccountAsync(CreateAccountRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 从私钥导入账户
    /// </summary>
    Task<Result<AccountResponseDto>> ImportAccountAsync(ImportAccountRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取账户详情
    /// </summary>
    Task<Result<AccountDetailResponseDto>> GetAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据地址获取账户
    /// </summary>
    Task<Result<AccountDetailResponseDto>> GetAccountByAddressAsync(string address, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取账户列表
    /// </summary>
    Task<Result<List<AccountResponseDto>>> GetAccountsAsync(AccountListRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取账户余额
    /// </summary>
    Task<Result<AccountBalanceResponseDto>> GetAccountBalanceAsync(string address, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 停用账户
    /// </summary>
    Task<Result> DeactivateAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    
    // 交易管理
    /// <summary>
    /// 发送交易
    /// </summary>
    Task<Result<TransactionResponseDto>> SendTransactionAsync(SendTransactionRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取交易详情
    /// </summary>
    Task<Result<TransactionResponseDto>> GetTransactionAsync(Guid transactionId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据哈希获取交易
    /// </summary>
    Task<Result<TransactionResponseDto>> GetTransactionByHashAsync(string transactionHash, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取交易列表
    /// </summary>
    Task<Result<List<TransactionResponseDto>>> GetTransactionsAsync(TransactionListRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 估算Gas费用
    /// </summary>
    Task<Result<EstimateGasResponseDto>> EstimateGasAsync(EstimateGasRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取交易统计
    /// </summary>
    Task<Result<TransactionStatisticsResponseDto>> GetTransactionStatisticsAsync(
        Guid? accountId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);
    
    // 合约管理
    /// <summary>
    /// 部署智能合约
    /// </summary>
    Task<Result<ContractDeploymentResponseDto>> DeployContractAsync(DeployContractRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取合约部署详情
    /// </summary>
    Task<Result<ContractDeploymentResponseDto>> GetContractDeploymentAsync(Guid deploymentId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 根据合约地址获取部署信息
    /// </summary>
    Task<Result<ContractDeploymentResponseDto>> GetContractDeploymentByAddressAsync(string contractAddress, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取合约部署列表
    /// </summary>
    Task<Result<List<ContractDeploymentResponseDto>>> GetContractDeploymentsAsync(ContractListRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 调用合约方法（只读）
    /// </summary>
    Task<Result<ContractMethodCallResponseDto>> CallContractMethodAsync(CallContractMethodRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 发送合约交易（写入）
    /// </summary>
    Task<Result<ContractMethodCallResponseDto>> SendContractTransactionAsync(SendContractTransactionRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取合约事件
    /// </summary>
    Task<Result<List<object>>> GetContractEventsAsync(ContractEventsRequestDto request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 验证合约
    /// </summary>
    Task<Result<bool>> VerifyContractAsync(VerifyContractRequestDto request, CancellationToken cancellationToken = default);
    
    // 网络信息
    /// <summary>
    /// 获取网络信息
    /// </summary>
    Task<Result<object>> GetNetworkInfoAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取当前Gas价格
    /// </summary>
    Task<Result<decimal>> GetGasPriceAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查网络连接状态
    /// </summary>
    Task<Result<bool>> CheckNetworkStatusAsync(CancellationToken cancellationToken = default);
}


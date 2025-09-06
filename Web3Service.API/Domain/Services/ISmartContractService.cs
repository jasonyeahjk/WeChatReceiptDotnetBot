using FluentResults;
using Web3Service.API.Domain.Entities;
using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Domain.Services;

/// <summary>
/// 智能合约服务接口
/// </summary>
public interface ISmartContractService
{
    /// <summary>
    /// 部署智能合约
    /// </summary>
    Task<Result<ContractDeployment>> DeployContractAsync(
        string contractName,
        string byteCode,
        string abi,
        ContractType contractType,
        string deployerAddress,
        string? deployerPrivateKey = null,
        object[]? constructorParameters = null,
        decimal? gasPrice = null,
        long? gasLimit = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 调用合约方法（只读）
    /// </summary>
    Task<Result<object>> CallContractMethodAsync(
        string contractAddress,
        string abi,
        string methodName,
        object[]? parameters = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 发送合约交易（写入）
    /// </summary>
    Task<Result<string>> SendContractTransactionAsync(
        string contractAddress,
        string abi,
        string methodName,
        string fromAddress,
        string? privateKey = null,
        object[]? parameters = null,
        decimal? gasPrice = null,
        long? gasLimit = null,
        decimal? value = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取合约事件日志
    /// </summary>
    Task<Result<List<object>>> GetContractEventsAsync(
        string contractAddress,
        string abi,
        string eventName,
        long? fromBlock = null,
        long? toBlock = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 估算合约方法调用的Gas费用
    /// </summary>
    Task<Result<long>> EstimateContractGasAsync(
        string contractAddress,
        string abi,
        string methodName,
        string fromAddress,
        object[]? parameters = null,
        decimal? value = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 验证合约代码
    /// </summary>
    Task<Result<bool>> VerifyContractAsync(
        string contractAddress,
        string sourceCode,
        string contractName,
        string compilerVersion,
        bool optimizationUsed,
        int runs,
        string? constructorArguments,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 获取合约字节码
    /// </summary>
    Task<Result<string>> GetContractByteCodeAsync(string contractAddress, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 检查合约是否存在
    /// </summary>
    Task<Result<bool>> ContractExistsAsync(string contractAddress, CancellationToken cancellationToken = default);
}


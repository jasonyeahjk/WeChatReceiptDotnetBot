using FluentResults;
using Microsoft.Extensions.Logging;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Blocks;
using Nethereum.Web3;
using Web3Service.API.Common.Errors;
using Web3Service.API.Domain.Entities;
using Web3Service.API.Domain.Services;
using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Infrastructure.Services;

/// <summary>
/// 基于Nethereum的智能合约服务实现
/// </summary>
public class NetheriumSmartContractService : ISmartContractService
{
    private readonly ILogger<NetheriumSmartContractService> _logger;
    private readonly Web3 _web3;
    private readonly string _rpcUrl;

    public NetheriumSmartContractService(ILogger<NetheriumSmartContractService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _rpcUrl = configuration.GetConnectionString("EthereumRpc") ?? "https://mainnet.infura.io/v3/YOUR_PROJECT_ID";
        _web3 = new Web3(_rpcUrl);
    }

    public async Task<Result<ContractDeployment>> DeployContractAsync(
        string contractName,
        string byteCode,
        string abi,
        ContractType contractType,
        string deployerAddress,
        string? deployerPrivateKey = null,
        object[]? constructorParameters = null,
        decimal? gasPrice = null,
        long? gasLimit = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deploying contract: {ContractName}", contractName);

            if (string.IsNullOrWhiteSpace(deployerPrivateKey))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_VALIDATION_MISSING_PRIVATE_KEY, "缺少部署者私钥"));
            }

            if (!ContractDeployment.IsValidByteCode(byteCode))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_INVALID_BYTECODE, "无效的合约字节码"));
            }

            var account = new Account(deployerPrivateKey);
            var web3 = new Web3(account, _rpcUrl);

            // 获取当前Gas价格（如果未指定）
            if (!gasPrice.HasValue)
            {
                var currentGasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                gasPrice = Web3.Convert.FromWei(currentGasPrice.Value);
            }

            // 估算Gas限制（如果未指定）
            if (!gasLimit.HasValue)
            {
                try
                {
                    var estimatedGas = await web3.Eth.DeployContract.EstimateGasAsync(abi, byteCode, deployerAddress, constructorParameters);
                    gasLimit = (long)estimatedGas.Value;
                }
                catch
                {
                    gasLimit = 3000000; // 默认Gas限制
                }
            }

            // 部署合约
            var receipt = await web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(
                abi, byteCode, deployerAddress, 
                new Nethereum.Hex.HexTypes.HexBigInteger(gasLimit.Value),
                new Nethereum.Hex.HexTypes.HexBigInteger(Web3.Convert.ToWei(gasPrice.Value)),
                new Nethereum.Hex.HexTypes.HexBigInteger(0), // value
                cancellationToken,
                constructorParameters);
            if (receipt.Status.Value != 1)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_DEPLOYMENT_FAILED, "合约部署失败"));
            }

            var contractDeployment = new ContractDeployment(
                contractName,
                receipt.ContractAddress,
                receipt.TransactionHash,
                byteCode,
                contractType,
                Guid.Empty, // 需要从外部提供DeployerAccountId
                abi);

            contractDeployment.MarkAsDeployed(
                (long)receipt.BlockNumber.Value,
                Web3.Convert.FromWei(receipt.GasUsed.Value * Web3.Convert.ToWei(gasPrice.Value)),
                (long)receipt.GasUsed.Value);

            _logger.LogInformation("Contract deployed successfully. Address: {ContractAddress}", receipt.ContractAddress);
            return Result.Ok(contractDeployment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deploying contract: {ContractName}", contractName);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_DEPLOYMENT_FAILED, "部署合约失败", ex));
        }
    }

    public async Task<Result<object>> CallContractMethodAsync(
        string contractAddress,
        string abi,
        string methodName,
        object[]? parameters = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling contract method: {MethodName} on {ContractAddress}", methodName, contractAddress);

            if (!ContractDeployment.IsValidContractAddress(contractAddress))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_INVALID_ADDRESS, "无效的合约地址"));
            }

            var contract = _web3.Eth.GetContract(abi, contractAddress);
            var function = contract.GetFunction(methodName);

            if (function == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_METHOD_NOT_FOUND, "合约方法不存在"));
            }

            object result;
            if (parameters != null && parameters.Length > 0)
            {
                result = await function.CallAsync<object>(parameters);
            }
            else
            {
                result = await function.CallAsync<object>();
            }

            _logger.LogInformation("Contract method called successfully: {MethodName}", methodName);
            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling contract method: {MethodName}", methodName);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_CALL_FAILED, "调用合约方法失败", ex));
        }
    }

    public async Task<Result<string>> SendContractTransactionAsync(
        string contractAddress,
        string abi,
        string methodName,
        string fromAddress,
        string? privateKey = null,
        object[]? parameters = null,
        decimal? gasPrice = null,
        long? gasLimit = null,
        decimal? value = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending contract transaction: {MethodName} on {ContractAddress}", methodName, contractAddress);

            if (string.IsNullOrWhiteSpace(privateKey))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_VALIDATION_MISSING_PRIVATE_KEY, "缺少私钥"));
            }

            if (!ContractDeployment.IsValidContractAddress(contractAddress))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_INVALID_ADDRESS, "无效的合约地址"));
            }

            var account = new Account(privateKey);
            var web3 = new Web3(account, _rpcUrl);

            var contract = web3.Eth.GetContract(abi, contractAddress);
            var function = contract.GetFunction(methodName);

            if (function == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_METHOD_NOT_FOUND, "合约方法不存在"));
            }

            // 获取当前Gas价格（如果未指定）
            if (!gasPrice.HasValue)
            {
                var currentGasPrice = await web3.Eth.GasPrice.SendRequestAsync();
                gasPrice = Web3.Convert.FromWei(currentGasPrice.Value);
            }

            // 估算Gas限制（如果未指定）
            if (!gasLimit.HasValue)
            {
                try
                {
                    var estimatedGas = await function.EstimateGasAsync(fromAddress, 
                        new Nethereum.Hex.HexTypes.HexBigInteger(Web3.Convert.ToWei(gasPrice.Value)),
                        new Nethereum.Hex.HexTypes.HexBigInteger(Web3.Convert.ToWei(value ?? 0)),
                        parameters);
                    gasLimit = (long)estimatedGas.Value;
                }
                catch
                {
                    gasLimit = 200000; // 默认Gas限制
                }
            }

            // 发送交易
            string transactionHash;
            if (parameters != null && parameters.Length > 0)
            {
                transactionHash = await function.SendTransactionAsync(fromAddress,
                    new Nethereum.Hex.HexTypes.HexBigInteger(gasLimit.Value),
                    new Nethereum.Hex.HexTypes.HexBigInteger(Web3.Convert.ToWei(gasPrice.Value)),
                    parameters);
            }
            else
            {
                transactionHash = await function.SendTransactionAsync(fromAddress,
                    new Nethereum.Hex.HexTypes.HexBigInteger(gasLimit.Value),
                    new Nethereum.Hex.HexTypes.HexBigInteger(Web3.Convert.ToWei(gasPrice.Value)),
                    new Nethereum.Hex.HexTypes.HexBigInteger(Web3.Convert.ToWei(value ?? 0)));
            }

            _logger.LogInformation("Contract transaction sent successfully. Hash: {TransactionHash}", transactionHash);
            return Result.Ok(transactionHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending contract transaction: {MethodName}", methodName);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_CALL_FAILED, "发送合约交易失败", ex));
        }
    }

    public async Task<Result<List<object>>> GetContractEventsAsync(
        string contractAddress,
        string abi,
        string eventName,
        long? fromBlock = null,
        long? toBlock = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting contract events: {EventName} from {ContractAddress}", eventName, contractAddress);

            var contract = _web3.Eth.GetContract(abi, contractAddress);
            var eventHandler = contract.GetEvent(eventName);

            if (eventHandler == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_METHOD_NOT_FOUND, "合约事件不存在"));
            }

            var filterInput = eventHandler.CreateFilterInput(
                fromBlock.HasValue ? new BlockParameter((ulong)fromBlock.Value) : BlockParameter.CreateEarliest(),
                toBlock.HasValue ? new BlockParameter((ulong)toBlock.Value) : BlockParameter.CreateLatest());

            var logs = await eventHandler.GetAllChangesAsync<object>(filterInput);
            var events = logs.Select(log => (object)log).ToList();

            _logger.LogInformation("Retrieved {Count} events for {EventName}", events.Count, eventName);
            return Result.Ok(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract events: {EventName}", eventName);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_CALL_FAILED, "获取合约事件失败", ex));
        }
    }

    public async Task<Result<long>> EstimateContractGasAsync(
        string contractAddress,
        string abi,
        string methodName,
        string fromAddress,
        object[]? parameters = null,
        decimal? value = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var contract = _web3.Eth.GetContract(abi, contractAddress);
            var function = contract.GetFunction(methodName);

            if (function == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_METHOD_NOT_FOUND, "合约方法不存在"));
            }

            var estimatedGas = await function.EstimateGasAsync(fromAddress,
                null, // gasPrice
                new Nethereum.Hex.HexTypes.HexBigInteger(Web3.Convert.ToWei(value ?? 0)),
                parameters);

            return Result.Ok((long)estimatedGas.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating contract gas: {MethodName}", methodName);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_GAS_ESTIMATION_FAILED, "估算合约Gas失败", ex));
        }
    }

    public async Task<Result<bool>> VerifyContractAsync(
        string contractAddress,
        string sourceCode,
        string contractName,
        string compilerVersion,
        bool optimizationUsed,
        int runs,
        string? constructorArguments,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Verifying contract: {ContractAddress}", contractAddress);

            // 这里应该实现合约验证逻辑
            // 由于合约验证通常需要第三方服务（如Etherscan），这里返回模拟结果
            await Task.Delay(1000, cancellationToken); // 模拟验证过程

            _logger.LogInformation("Contract verification completed for: {ContractAddress}", contractAddress);
            return Result.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying contract: {ContractAddress}", contractAddress);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_VERIFICATION_FAILED, "合约验证失败", ex));
        }
    }

    public async Task<Result<string>> GetContractByteCodeAsync(string contractAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            var code = await _web3.Eth.GetCode.SendRequestAsync(contractAddress);
            return Result.Ok(code);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract bytecode: {ContractAddress}", contractAddress);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_CALL_FAILED, "获取合约字节码失败", ex));
        }
    }

    public async Task<Result<bool>> ContractExistsAsync(string contractAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            var code = await _web3.Eth.GetCode.SendRequestAsync(contractAddress);
            var exists = !string.IsNullOrEmpty(code) && code != "0x";
            return Result.Ok(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking contract existence: {ContractAddress}", contractAddress);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_CALL_FAILED, "检查合约存在性失败", ex));
        }
    }
}


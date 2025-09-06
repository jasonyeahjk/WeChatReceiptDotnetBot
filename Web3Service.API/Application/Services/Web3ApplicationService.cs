using FluentResults;
using Microsoft.Extensions.Logging;
using Web3Service.API.Application.DTOs.AccountDTOs;
using Web3Service.API.Application.DTOs.ContractDTOs;
using Web3Service.API.Application.DTOs.TransactionDTOs;
using Web3Service.API.Application.DTOs.Web3DTOs;
using Web3Service.API.Application.Interfaces;
using Web3Service.API.Common.Errors;
using Web3Service.API.Domain.Entities;
using Web3Service.API.Domain.Repositories;
using Web3Service.API.Domain.Services;
using Web3Service.API.Domain.ValueObjects;

namespace Web3Service.API.Application.Services;

/// <summary>
/// Web3应用服务实现
/// </summary>
public class Web3ApplicationService : IWeb3ApplicationService
{
    private readonly IBlockchainAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IContractDeploymentRepository _contractRepository;
    private readonly IWeb3BlockchainService _blockchainService;
    private readonly ISmartContractService _contractService;
    private readonly ILogger<Web3ApplicationService> _logger;

    public Web3ApplicationService(
        IBlockchainAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IContractDeploymentRepository contractRepository,
        IWeb3BlockchainService blockchainService,
        ISmartContractService contractService,
        ILogger<Web3ApplicationService> logger)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _contractRepository = contractRepository;
        _blockchainService = blockchainService;
        _contractService = contractService;
        _logger = logger;
    }

    #region 账户管理

    public async Task<Result<AccountResponseDto>> CreateAccountAsync(CreateAccountRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new blockchain account with type: {AccountType}", request.AccountType);

            var result = await _blockchainService.CreateAccountAsync(cancellationToken);
            if (result.IsFailed)
            {
                _logger.LogError("Failed to create blockchain account: {Errors}", string.Join(", ", result.Errors));
                return result.ToResult<AccountResponseDto>();
            }

            var account = result.Value;
            var savedAccount = await _accountRepository.AddAsync(account, cancellationToken);

            _logger.LogInformation("Successfully created account with ID: {AccountId}, Address: {Address}", 
                savedAccount.Id, savedAccount.Address);

            return Result.Ok(MapToAccountResponseDto(savedAccount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_CREATION_FAILED, "创建账户失败"));
        }
    }

    public async Task<Result<AccountResponseDto>> ImportAccountAsync(ImportAccountRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Importing blockchain account from private key");

            if (string.IsNullOrWhiteSpace(request.PrivateKey))
            {
                return Result.Fail(new ValidationError("PrivateKey", "私钥不能为空"));
            }

            var result = await _blockchainService.ImportAccountFromPrivateKeyAsync(request.PrivateKey, cancellationToken);
            if (result.IsFailed)
            {
                _logger.LogError("Failed to import blockchain account: {Errors}", string.Join(", ", result.Errors));
                return result.ToResult<AccountResponseDto>();
            }

            var account = result.Value;
            
            // 检查账户是否已存在
            var existingAccount = await _accountRepository.GetByAddressAsync(account.Address, cancellationToken);
            if (existingAccount != null)
            {
                _logger.LogWarning("Account with address {Address} already exists", account.Address);
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_ALREADY_EXISTS, "账户已存在"));
            }

            var savedAccount = await _accountRepository.AddAsync(account, cancellationToken);

            _logger.LogInformation("Successfully imported account with ID: {AccountId}, Address: {Address}", 
                savedAccount.Id, savedAccount.Address);

            return Result.Ok(MapToAccountResponseDto(savedAccount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing account");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_IMPORT_FAILED, "导入账户失败"));
        }
    }

    public async Task<Result<AccountDetailResponseDto>> GetAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        try
        {
            var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
            if (account == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_NOT_FOUND, "账户不存在"));
            }

            // 获取账户余额
            var balanceResult = await _blockchainService.GetBalanceAsync(account.Address, cancellationToken);
            var balance = balanceResult.IsSuccess ? balanceResult.Value : 0;

            // 获取交易统计
            var transactionCount = await _transactionRepository.GetTransactionCountAsync(accountId, cancellationToken: cancellationToken);
            var contractCount = await _contractRepository.GetDeploymentCountAsync(accountId, cancellationToken: cancellationToken);

            return Result.Ok(MapToAccountDetailResponseDto(account, balance, transactionCount, contractCount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account {AccountId}", accountId);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_QUERY_FAILED, "查询账户失败"));
        }
    }

    public async Task<Result<AccountDetailResponseDto>> GetAccountByAddressAsync(string address, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!BlockchainAccount.IsValidAddress(address))
            {
                return Result.Fail(new ValidationError("Address", "无效的账户地址格式"));
            }

            var account = await _accountRepository.GetByAddressAsync(address, cancellationToken);
            if (account == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_NOT_FOUND, "账户不存在"));
            }

            // 获取账户余额
            var balanceResult = await _blockchainService.GetBalanceAsync(account.Address, cancellationToken);
            var balance = balanceResult.IsSuccess ? balanceResult.Value : 0;

            // 获取交易统计
            var transactionCount = await _transactionRepository.GetTransactionCountAsync(account.Id, cancellationToken: cancellationToken);
            var contractCount = await _contractRepository.GetDeploymentCountAsync(account.Id, cancellationToken: cancellationToken);

            return Result.Ok(MapToAccountDetailResponseDto(account, balance, transactionCount, contractCount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account by address {Address}", address);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_QUERY_FAILED, "查询账户失败"));
        }
    }

    public async Task<Result<List<AccountResponseDto>>> GetAccountsAsync(AccountListRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var accounts = await _accountRepository.GetAccountsAsync(
                request.AccountType,
                request.IsActive,
                request.Skip,
                request.Take,
                cancellationToken);

            var accountDtos = accounts.Select(MapToAccountResponseDto).ToList();
            return Result.Ok(accountDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting accounts list");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_QUERY_FAILED, "查询账户列表失败"));
        }
    }

    public async Task<Result<AccountBalanceResponseDto>> GetAccountBalanceAsync(string address, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!BlockchainAccount.IsValidAddress(address))
            {
                return Result.Fail(new ValidationError("Address", "无效的账户地址格式"));
            }

            var balanceResult = await _blockchainService.GetBalanceAsync(address, cancellationToken);
            if (balanceResult.IsFailed)
            {
                return balanceResult.ToResult<AccountBalanceResponseDto>();
            }

            var response = new AccountBalanceResponseDto
            {
                Address = address,
                Balance = balanceResult.Value,
                BalanceWei = (balanceResult.Value * 1_000_000_000_000_000_000m).ToString("F0"),
                QueryTime = DateTime.UtcNow
            };

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting balance for address {Address}", address);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_BALANCE_QUERY_FAILED, "查询余额失败"));
        }
    }

    public async Task<Result> DeactivateAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        try
        {
            var account = await _accountRepository.GetByIdAsync(accountId, cancellationToken);
            if (account == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_NOT_FOUND, "账户不存在"));
            }

            account.Deactivate();
            await _accountRepository.UpdateAsync(account, cancellationToken);

            _logger.LogInformation("Successfully deactivated account {AccountId}", accountId);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating account {AccountId}", accountId);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_UPDATE_FAILED, "停用账户失败"));
        }
    }

    #endregion

    #region 私有映射方法

    private static AccountResponseDto MapToAccountResponseDto(BlockchainAccount account)
    {
        return new AccountResponseDto
        {
            Id = account.Id,
            Address = account.Address,
            PublicKey = account.PublicKey,
            AccountType = account.AccountType,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };
    }

    private static AccountDetailResponseDto MapToAccountDetailResponseDto(
        BlockchainAccount account, 
        decimal balance, 
        int transactionCount, 
        int contractCount)
    {
        return new AccountDetailResponseDto
        {
            Id = account.Id,
            Address = account.Address,
            PublicKey = account.PublicKey,
            AccountType = account.AccountType,
            IsActive = account.IsActive,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt,
            Balance = balance,
            TransactionCount = transactionCount,
            DeployedContractCount = contractCount
        };
    }

    #endregion

    // 其他方法实现将在后续添加...
    #region 交易管理 - 待实现
    
    public async Task<Result<TransactionResponseDto>> SendTransactionAsync(SendTransactionRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending transaction from {FromAddress} to {ToAddress}, amount: {Amount}", 
                request.FromAddress, request.ToAddress, request.Amount);

            if (string.IsNullOrWhiteSpace(request.FromAddress) || !BlockchainAccount.IsValidAddress(request.FromAddress))
            {
                return Result.Fail(new ValidationError("FromAddress", "无效的发送方地址"));
            }
            if (string.IsNullOrWhiteSpace(request.ToAddress) || !BlockchainAccount.IsValidAddress(request.ToAddress))
            {
                return Result.Fail(new ValidationError("ToAddress", "无效的接收方地址"));
            }
            if (request.Amount <= 0)
            {
                return Result.Fail(new ValidationError("Amount", "交易金额必须大于0"));
            }
            if (string.IsNullOrWhiteSpace(request.PrivateKey))
            {
                return Result.Fail(new ValidationError("PrivateKey", "私钥不能为空"));
            }

            // 检查发送方账户是否存在并激活
            var fromAccount = await _accountRepository.GetByAddressAsync(request.FromAddress, cancellationToken);
            if (fromAccount == null || !fromAccount.IsActive)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_NOT_FOUND, "发送方账户不存在或未激活"));
            }

            var sendResult = await _blockchainService.SendTransactionAsync(
                request.FromAddress,
                request.ToAddress,
                request.Amount,
                request.PrivateKey,
                request.GasPrice,
                request.GasLimit,
                cancellationToken);

            if (sendResult.IsFailed)
            {
                _logger.LogError("Failed to send transaction: {Errors}", string.Join(", ", sendResult.Errors));
                return sendResult.ToResult<TransactionResponseDto>();
            }

            var transactionHash = sendResult.Value;

            // 创建并保存交易记录
            var transactionRecord = new TransactionRecord(
                transactionHash,
                request.FromAddress,
                request.ToAddress,
                request.Amount,
                request.GasPrice ?? 0, // 实际GasPrice需要从区块链服务获取
                request.GasLimit ?? 0, // 实际GasLimit需要从区块链服务获取
                fromAccount.Id // 关联发送方账户ID
            );
            // transactionRecord.SetStatus(Domain.ValueObjects.TransactionStatus.Pending); // Removed as per TransactionRecord.cs
            await _transactionRepository.AddAsync(transactionRecord, cancellationToken);

            _logger.LogInformation("Transaction sent and recorded with hash: {TransactionHash}", transactionHash);

            return Result.Ok(MapToTransactionResponseDto(transactionRecord));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending transaction");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_SEND_FAILED, "发送交易失败"));
        }
    }

    public async Task<Result<TransactionResponseDto>> GetTransactionAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting transaction with ID: {TransactionId}", transactionId);

            var transaction = await _transactionRepository.GetByIdAsync(transactionId, cancellationToken);
            if (transaction == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_NOT_FOUND, "交易不存在"));
            }

            return Result.Ok(MapToTransactionResponseDto(transaction));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction {TransactionId}", transactionId);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_QUERY_FAILED, "查询交易失败"));
        }
    }

    public async Task<Result<TransactionResponseDto>> GetTransactionByHashAsync(string transactionHash, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting transaction with hash: {TransactionHash}", transactionHash);

            if (string.IsNullOrWhiteSpace(transactionHash))
            {
                return Result.Fail(new ValidationError("TransactionHash", "交易哈希不能为空"));
            }

            var transaction = await _transactionRepository.GetByHashAsync(transactionHash, cancellationToken);
            if (transaction == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_NOT_FOUND, "交易不存在"));
            }

            return Result.Ok(MapToTransactionResponseDto(transaction));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction by hash {TransactionHash}", transactionHash);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_QUERY_FAILED, "查询交易失败"));
        }
    }

    public async Task<Result<List<TransactionResponseDto>>> GetTransactionsAsync(TransactionListRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting transactions list for account {AccountId}, skip: {Skip}, take: {Take}", 
                request.AccountId, request.Skip, request.Take);

            var transactions = await _transactionRepository.GetByAccountIdAsync(
                request.AccountId.Value,
                request.Status,
                request.FromDate,
                request.ToDate,
                request.Skip,
                request.Take,
                cancellationToken);

            var transactionDtos = transactions.Select(MapToTransactionResponseDto).ToList();
            return Result.Ok(transactionDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transactions list");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_QUERY_FAILED, "查询交易列表失败"));
        }
    }

    public async Task<Result<EstimateGasResponseDto>> EstimateGasAsync(EstimateGasRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Estimating gas for transaction from {FromAddress} to {ToAddress}, amount: {Amount}", 
                request.FromAddress, request.ToAddress, request.Amount);

            if (string.IsNullOrWhiteSpace(request.FromAddress) || !BlockchainAccount.IsValidAddress(request.FromAddress))
            {
                return Result.Fail(new ValidationError("FromAddress", "无效的发送方地址"));
            }
            if (string.IsNullOrWhiteSpace(request.ToAddress) || !BlockchainAccount.IsValidAddress(request.ToAddress))
            {
                return Result.Fail(new ValidationError("ToAddress", "无效的接收方地址"));
            }
            if (request.Amount <= 0)
            {
                return Result.Fail(new ValidationError("Amount", "交易金额必须大于0"));
            }

            var gasEstimateResult = await _blockchainService.EstimateGasAsync(
                request.FromAddress,
                request.ToAddress,
                request.Amount,
                request.Data,
                cancellationToken);

            if (gasEstimateResult.IsFailed)
            {
                _logger.LogError("Failed to estimate gas: {Errors}", string.Join(", ", gasEstimateResult.Errors));
                return gasEstimateResult.ToResult<EstimateGasResponseDto>();
            }

            return Result.Ok(new EstimateGasResponseDto
            {
                EstimatedGas = gasEstimateResult.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error estimating gas");                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_GAS_ESTIMATION_FAILED, "Gas估算失败"));      }
    }

    public async Task<Result<TransactionStatisticsResponseDto>> GetTransactionStatisticsAsync(Guid? accountId = null, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting transaction statistics for account {AccountId} from {FromDate} to {ToDate}", 
                accountId, fromDate, toDate);

            var totalTransactions = await _transactionRepository.GetTransactionCountAsync(accountId, null, fromDate, toDate, cancellationToken);
            var totalAmount = await _transactionRepository.GetTotalTransactionAmountAsync(accountId, fromDate, toDate, cancellationToken);

            return Result.Ok(new TransactionStatisticsResponseDto
            {
                AccountId = accountId,
                TotalTransactions = totalTransactions,
                TotalAmount = totalAmount,
                FromDate = fromDate,
                ToDate = toDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction statistics");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_TRANSACTION_QUERY_FAILED, "查询交易统计失败"));
        }
    }

    #endregion

    #region 合约管理 - 待实现

    public async Task<Result<ContractDeploymentResponseDto>> DeployContractAsync(DeployContractRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deploying contract {ContractName} from account {FromAddress}", 
                request.ContractName, request.FromAddress);

            if (string.IsNullOrWhiteSpace(request.FromAddress) || !BlockchainAccount.IsValidAddress(request.FromAddress))
            {
                return Result.Fail(new ValidationError("FromAddress", "无效的部署账户地址"));
            }
            if (string.IsNullOrWhiteSpace(request.ByteCode))
            {
                return Result.Fail(new ValidationError("Bytecode", "合约字节码不能为空"));
            }
            if (string.IsNullOrWhiteSpace(request.Abi))
            {
                return Result.Fail(new ValidationError("Abi", "合约ABI不能为空"));
            }
            if (string.IsNullOrWhiteSpace(request.PrivateKey))
            {
                return Result.Fail(new ValidationError("PrivateKey", "私钥不能为空"));
            }

            // 检查部署账户是否存在并激活
            var deployerAccount = await _accountRepository.GetByAddressAsync(request.FromAddress, cancellationToken);
            if (deployerAccount == null || !deployerAccount.IsActive)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_NOT_FOUND, "部署账户不存在或未激活"));
            }

            var deployResult = await _contractService.DeployContractAsync(
                request.ContractName,
                request.ByteCode,
                request.Abi,
                request.ContractType,
                request.FromAddress,
                request.PrivateKey,
                request.ConstructorArguments,
                request.GasPrice,
                request.GasLimit,
                cancellationToken);

            if (deployResult.IsFailed)
            {
                _logger.LogError("Failed to deploy contract: {Errors}", string.Join(", ", deployResult.Errors));
                return deployResult.ToResult<ContractDeploymentResponseDto>();
            }

            var contractAddress = deployResult.Value.ContractAddress;

            // 创建并保存合约部署记录
            var contractDeployment = deployResult.Value;
            await _contractRepository.AddAsync(contractDeployment, cancellationToken);

            _logger.LogInformation("Contract {ContractName} deployed at address: {ContractAddress}", 
                request.ContractName, contractAddress);

            return Result.Ok(MapToContractDeploymentResponseDto(contractDeployment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deploying contract");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_DEPLOYMENT_FAILED, "部署合约失败"));
        }
    }

    public async Task<Result<ContractDeploymentResponseDto>> GetContractDeploymentAsync(Guid deploymentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting contract deployment with ID: {DeploymentId}", deploymentId);

            var deployment = await _contractRepository.GetByIdAsync(deploymentId, cancellationToken);
            if (deployment == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_NOT_FOUND, "合约部署记录不存在"));
            }

            return Result.Ok(MapToContractDeploymentResponseDto(deployment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract deployment {DeploymentId}", deploymentId);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_QUERY_FAILED, "查询合约部署失败"));
        }
    }

    public async Task<Result<ContractDeploymentResponseDto>> GetContractDeploymentByAddressAsync(string contractAddress, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting contract deployment with address: {ContractAddress}", contractAddress);

            if (string.IsNullOrWhiteSpace(contractAddress) || !BlockchainAccount.IsValidAddress(contractAddress))
            {
                return Result.Fail(new ValidationError("ContractAddress", "无效的合约地址格式"));
            }

            var deployment = await _contractRepository.GetByContractAddressAsync(contractAddress, cancellationToken);
            if (deployment == null)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_NOT_FOUND, "合约部署记录不存在"));
            }

            return Result.Ok(MapToContractDeploymentResponseDto(deployment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract deployment by address {ContractAddress}", contractAddress);
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_QUERY_FAILED, "查询合约部署失败"));
        }
    }

    public async Task<Result<List<ContractDeploymentResponseDto>>> GetContractDeploymentsAsync(ContractListRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting contract deployments list for account {AccountId}, type: {ContractType}, skip: {Skip}, take: {Take}", 
                request.DeployerAccountId, request.ContractType, request.Skip, request.Take);

            List<ContractDeployment> deployments;
            if (request.DeployerAccountId.HasValue)
            {
                deployments = await _contractRepository.GetByDeployerAccountIdAsync(
                    request.DeployerAccountId.Value,
                    request.ContractType,
                    request.Status,
                    request.Skip,
                    request.Take,
                    cancellationToken);
            }
            else
            {
                deployments = await _contractRepository.GetDeploymentsAsync(
                    request.ContractType,
                    request.Status,
                    request.FromDate,
                    request.ToDate,
                    request.Skip,
                    request.Take,
                    cancellationToken);
            }

            var deploymentDtos = deployments.Select(d => MapToContractDeploymentResponseDto(d)).ToList();
            return Result.Ok(deploymentDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract deployments list");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_QUERY_FAILED, "查询合约部署列表失败"));
        }
    }

    public async Task<Result<ContractMethodCallResponseDto>> CallContractMethodAsync(CallContractMethodRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Calling contract method {MethodName} on contract {ContractAddress}", 
                request.MethodName, request.ContractAddress);

            if (string.IsNullOrWhiteSpace(request.ContractAddress) || !BlockchainAccount.IsValidAddress(request.ContractAddress))
            {
                return Result.Fail(new ValidationError("ContractAddress", "无效的合约地址格式"));
            }
            if (string.IsNullOrWhiteSpace(request.Abi))
            {
                return Result.Fail(new ValidationError("Abi", "合约ABI不能为空"));
            }
            if (string.IsNullOrWhiteSpace(request.MethodName))
            {
                return Result.Fail(new ValidationError("MethodName", "方法名不能为空"));
            }

            var callResult = await _contractService.CallContractMethodAsync(
                request.ContractAddress,
                request.Abi,
                request.MethodName,
                request.Parameters,
                cancellationToken);

            if (callResult.IsFailed)
            {
                _logger.LogError("Failed to call contract method: {Errors}", string.Join(", ", callResult.Errors));
                return callResult.ToResult<ContractMethodCallResponseDto>();
            }

            return Result.Ok(new ContractMethodCallResponseDto
            {
                ContractAddress = request.ContractAddress,
                MethodName = request.MethodName,
                Result = callResult.Value
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling contract method");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_CALL_FAILED, "调用合约方法失败"));
        }
    }

    public async Task<Result<ContractMethodCallResponseDto>> SendContractTransactionAsync(SendContractTransactionRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending contract transaction for method {MethodName} on contract {ContractAddress}", 
                request.MethodName, request.ContractAddress);

            if (string.IsNullOrWhiteSpace(request.ContractAddress) || !BlockchainAccount.IsValidAddress(request.ContractAddress))
            {
                return Result.Fail(new ValidationError("ContractAddress", "无效的合约地址格式"));
            }
            if (string.IsNullOrWhiteSpace(request.Abi))
            {
                return Result.Fail(new ValidationError("Abi", "合约ABI不能为空"));
            }
            if (string.IsNullOrWhiteSpace(request.MethodName))
            {
                return Result.Fail(new ValidationError("MethodName", "方法名不能为空"));
            }
            if (string.IsNullOrWhiteSpace(request.FromAddress) || !BlockchainAccount.IsValidAddress(request.FromAddress))
            {
                return Result.Fail(new ValidationError("FromAddress", "无效的发送方地址"));
            }
            if (string.IsNullOrWhiteSpace(request.PrivateKey))
            {
                return Result.Fail(new ValidationError("PrivateKey", "私钥不能为空"));
            }

            // 检查发送方账户是否存在并激活
            var fromAccount = await _accountRepository.GetByAddressAsync(request.FromAddress, cancellationToken);
            if (fromAccount == null || !fromAccount.IsActive)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.WEB3_ACCOUNT_NOT_FOUND, "发送方账户不存在或未激活"));
            }

            var sendResult = await _contractService.SendContractTransactionAsync(
                request.ContractAddress,
                request.Abi,
                request.MethodName,
                request.FromAddress,
                request.PrivateKey,
                request.Parameters,
                request.GasPrice,
                request.GasLimit,
                request.Value,
                cancellationToken);

            if (sendResult.IsFailed)
            {
                _logger.LogError("Failed to send contract transaction: {Errors}", string.Join(", ", sendResult.Errors));
                return sendResult.ToResult<ContractMethodCallResponseDto>();
            }

            // 记录交易
            var transactionRecord = new TransactionRecord(
                sendResult.Value, // Transaction Hash
                request.FromAddress,
                request.ContractAddress,
                0, // 合约交互通常金额为0
                request.GasPrice ?? 0,
                request.GasLimit ?? 0,
                fromAccount.Id
            );
            // transactionRecord.SetStatus(Domain.ValueObjects.TransactionStatus.Pending); // Removed as per TransactionRecord.cs
            await _transactionRepository.AddAsync(transactionRecord, cancellationToken);

            return Result.Ok(new ContractMethodCallResponseDto
            {
                ContractAddress = request.ContractAddress,
                MethodName = request.MethodName,
                Result = sendResult.Value // 返回交易哈希
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending contract transaction");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_TRANSACTION_FAILED, "发送合约交易失败"));
        }
    }

    public async Task<Result<List<object>>> GetContractEventsAsync(ContractEventsRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting contract events for contract {ContractAddress}, event {EventName}", 
                request.ContractAddress, request.EventName);

            if (string.IsNullOrWhiteSpace(request.ContractAddress) || !BlockchainAccount.IsValidAddress(request.ContractAddress))
            {
                return Result.Fail(new ValidationError("ContractAddress", "无效的合约地址格式"));
            }
            if (string.IsNullOrWhiteSpace(request.Abi))
            {
                return Result.Fail(new ValidationError("Abi", "合约ABI不能为空"));
            }
            if (string.IsNullOrWhiteSpace(request.EventName))
            {
                return Result.Fail(new ValidationError("EventName", "事件名不能为空"));
            }

            var events = await _contractService.GetContractEventsAsync(
                request.ContractAddress,
                request.Abi,
                request.EventName,
                request.FromBlock,
                request.ToBlock,
                cancellationToken);

            if (events.IsFailed)
            {
                _logger.LogError("Failed to get contract events: {Errors}", string.Join(", ", events.Errors));
                return events.ToResult<List<object>>();
            }

            return Result.Ok(events.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting contract events");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_EVENT_QUERY_FAILED, "查询合约事件失败"));
        }
    }

    public async Task<Result<bool>> VerifyContractAsync(VerifyContractRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Verifying contract at address: {ContractAddress}", request.ContractAddress);

            if (string.IsNullOrWhiteSpace(request.ContractAddress) || !BlockchainAccount.IsValidAddress(request.ContractAddress))
            {
                return Result.Fail(new ValidationError("ContractAddress", "无效的合约地址格式"));
            }
            if (string.IsNullOrWhiteSpace(request.SourceCode))
            {
                return Result.Fail(new ValidationError("SourceCode", "合约源代码不能为空"));
            }
            if (string.IsNullOrWhiteSpace(request.ContractName))
            {
                return Result.Fail(new ValidationError("ContractName", "合约名称不能为空"));
            }

            var verifyResult = await _contractService.VerifyContractAsync(
                request.ContractAddress,
                request.SourceCode,
                request.ContractName,
                request.CompilerVersion,
                request.OptimizationUsed,
                request.Runs, request.ConstructorArguments,
                cancellationToken);

            if (verifyResult.IsFailed)
            {
                _logger.LogError("Failed to verify contract: {Errors}", string.Join(", ", verifyResult.Errors));
                return verifyResult.ToResult<bool>();
            }

            return Result.Ok(verifyResult.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying contract");
            return Result.Fail(new ApplicationError(ErrorCodes.WEB3_CONTRACT_VERIFICATION_FAILED, "合约验证失败"));
        }
    }

    #endregion

    #region 网络信息 - 待实现

    public Task<Result<object>> GetNetworkInfoAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("网络信息功能待实现");
    }

    public Task<Result<decimal>> GetGasPriceAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("网络信息功能待实现");
    }

    public Task<Result<bool>> CheckNetworkStatusAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("网络信息功能待实现");
    }

    #endregion

    private static ContractDeploymentResponseDto MapToContractDeploymentResponseDto(ContractDeployment deployment)
    {
        return new ContractDeploymentResponseDto
        {
            Id = deployment.Id,
            ContractName = deployment.ContractName,
            ContractAddress = deployment.ContractAddress,
            DeploymentTransactionHash = deployment.DeploymentTransactionHash,
            ContractType = deployment.ContractType,
            Status = deployment.Status,
            BlockNumber = deployment.BlockNumber,
            DeploymentCost = deployment.DeploymentCost,
            GasUsed = deployment.GasUsed,
            DeployerAccountId = deployment.DeployerAccountId,
            ErrorMessage = deployment.ErrorMessage,
            CreatedAt = deployment.CreatedAt,
            UpdatedAt = deployment.UpdatedAt
        };
    }

    private static TransactionResponseDto MapToTransactionResponseDto(TransactionRecord transaction)
    {
        return new TransactionResponseDto
        {
            Id = transaction.Id,
            TransactionHash = transaction.TransactionHash,
            FromAddress = transaction.FromAddress,
            ToAddress = transaction.ToAddress,
            Amount = transaction.Amount,
            GasPrice = transaction.GasPrice,
            GasLimit = transaction.GasLimit,
            GasUsed = transaction.GasUsed,
            Status = transaction.Status,
            BlockNumber = transaction.BlockNumber,
            BlockHash = transaction.BlockHash,
            TransactionIndex = transaction.TransactionIndex,
            ContractAddress = transaction.ContractAddress,
            ErrorMessage = transaction.ErrorMessage,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }
}









using Microsoft.AspNetCore.Mvc;
using Web3Service.API.Application.DTOs.AccountDTOs;
using Web3Service.API.Application.DTOs.ContractDTOs;
using Web3Service.API.Application.DTOs.TransactionDTOs;
using Web3Service.API.Application.DTOs.Web3DTOs;
using Web3Service.API.Application.Interfaces;
using Web3Service.API.Common.Extensions;

namespace Web3Service.API.Controllers;

/// <summary>
/// Web3服务控制器
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class Web3Controller : ControllerBase
{
    private readonly IWeb3ApplicationService _web3Service;
    private readonly ILogger<Web3Controller> _logger;

    public Web3Controller(
        IWeb3ApplicationService web3Service,
        ILogger<Web3Controller> logger)
    {
        _web3Service = web3Service;
        _logger = logger;
    }

    #region 账户管理

    /// <summary>
    /// 创建新的区块链账户
    /// </summary>
    /// <param name="request">创建账户请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>账户信息</returns>
    [HttpPost("accounts")]
    public async Task<ActionResult<AccountResponseDto>> CreateAccount(
        [FromBody] CreateAccountRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new blockchain account");

        var result = await _web3Service.CreateAccountAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 从私钥导入账户
    /// </summary>
    /// <param name="request">导入账户请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>账户信息</returns>
    [HttpPost("accounts/import")]
    public async Task<ActionResult<AccountResponseDto>> ImportAccount(
        [FromBody] ImportAccountRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Importing blockchain account from private key");

        var result = await _web3Service.ImportAccountAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 获取账户详情
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>账户详情</returns>
    [HttpGet("accounts/{accountId:guid}")]
    public async Task<ActionResult<AccountDetailResponseDto>> GetAccount(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting account details for ID: {AccountId}", accountId);

        var result = await _web3Service.GetAccountAsync(accountId, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 根据地址获取账户详情
    /// </summary>
    /// <param name="address">账户地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>账户详情</returns>
    [HttpGet("accounts/address/{address}")]
    public async Task<ActionResult<AccountDetailResponseDto>> GetAccountByAddress(
        string address,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting account details for address: {Address}", address);

        var result = await _web3Service.GetAccountByAddressAsync(address, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 获取账户列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>账户列表</returns>
    [HttpGet("accounts")]
    public async Task<ActionResult<List<AccountResponseDto>>> GetAccounts(
        [FromQuery] AccountListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting accounts list");

        var result = await _web3Service.GetAccountsAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 获取账户余额
    /// </summary>
    /// <param name="address">账户地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>账户余额</returns>
    [HttpGet("accounts/{address}/balance")]
    public async Task<ActionResult<AccountBalanceResponseDto>> GetAccountBalance(
        string address,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting balance for address: {Address}", address);

        var result = await _web3Service.GetAccountBalanceAsync(address, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 停用账户
    /// </summary>
    /// <param name="accountId">账户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>操作结果</returns>
    [HttpPut("accounts/{accountId:guid}/deactivate")]
    public async Task<ActionResult> DeactivateAccount(
        Guid accountId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deactivating account: {AccountId}", accountId);

        var result = await _web3Service.DeactivateAccountAsync(accountId, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    #endregion

    #region 交易管理

    /// <summary>
    /// 发送交易
    /// </summary>
    /// <param name="request">发送交易请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>交易信息</returns>
    [HttpPost("transactions")]
    public async Task<ActionResult<TransactionResponseDto>> SendTransaction(
        [FromBody] SendTransactionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending transaction from {FromAddress} to {ToAddress}", 
            request.FromAddress, request.ToAddress);

        var result = await _web3Service.SendTransactionAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 获取交易详情
    /// </summary>
    /// <param name="transactionId">交易ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>交易详情</returns>
    [HttpGet("transactions/{transactionId:guid}")]
    public async Task<ActionResult<TransactionResponseDto>> GetTransaction(
        Guid transactionId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting transaction details for ID: {TransactionId}", transactionId);

        var result = await _web3Service.GetTransactionAsync(transactionId, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 根据哈希获取交易详情
    /// </summary>
    /// <param name="hash">交易哈希</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>交易详情</returns>
    [HttpGet("transactions/hash/{hash}")]
    public async Task<ActionResult<TransactionResponseDto>> GetTransactionByHash(
        string hash,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting transaction details for hash: {Hash}", hash);

        var result = await _web3Service.GetTransactionByHashAsync(hash, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 获取交易列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>交易列表</returns>
    [HttpGet("transactions")]
    public async Task<ActionResult<List<TransactionResponseDto>>> GetTransactions(
        [FromQuery] TransactionListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting transactions list");

        var result = await _web3Service.GetTransactionsAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 估算Gas费用
    /// </summary>
    /// <param name="request">估算请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>Gas估算结果</returns>
    [HttpPost("transactions/estimate-gas")]
    public async Task<ActionResult<EstimateGasResponseDto>> EstimateGas(
        [FromBody] EstimateGasRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Estimating gas for transaction from {FromAddress} to {ToAddress}", 
            request.FromAddress, request.ToAddress);

        var result = await _web3Service.EstimateGasAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 获取交易统计
    /// </summary>
    /// <param name="accountId">账户ID（可选）</param>
    /// <param name="fromDate">开始日期（可选）</param>
    /// <param name="toDate">结束日期（可选）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>交易统计</returns>
    [HttpGet("transactions/statistics")]
    public async Task<ActionResult<TransactionStatisticsResponseDto>> GetTransactionStatistics(
        [FromQuery] Guid? accountId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting transaction statistics");

        var result = await _web3Service.GetTransactionStatisticsAsync(accountId, fromDate, toDate, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    #endregion

    #region 智能合约管理

    /// <summary>
    /// 部署智能合约
    /// </summary>
    /// <param name="request">部署请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部署结果</returns>
    [HttpPost("contracts")]
    public async Task<ActionResult<ContractDeploymentResponseDto>> DeployContract(
        [FromBody] DeployContractRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deploying contract: {ContractName}", request.ContractName);

        var result = await _web3Service.DeployContractAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 获取合约部署详情
    /// </summary>
    /// <param name="deploymentId">部署ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部署详情</returns>
    [HttpGet("contracts/deployments/{deploymentId:guid}")]
    public async Task<ActionResult<ContractDeploymentResponseDto>> GetContractDeployment(
        Guid deploymentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting contract deployment details for ID: {DeploymentId}", deploymentId);

        var result = await _web3Service.GetContractDeploymentAsync(deploymentId, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 根据合约地址获取部署信息
    /// </summary>
    /// <param name="address">合约地址</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部署信息</returns>
    [HttpGet("contracts/{address}/deployment")]
    public async Task<ActionResult<ContractDeploymentResponseDto>> GetContractDeploymentByAddress(
        string address,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting contract deployment for address: {Address}", address);

        var result = await _web3Service.GetContractDeploymentByAddressAsync(address, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 获取合约部署列表
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>部署列表</returns>
    [HttpGet("contracts/deployments")]
    public async Task<ActionResult<List<ContractDeploymentResponseDto>>> GetContractDeployments(
        [FromQuery] ContractListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting contract deployments list");

        var result = await _web3Service.GetContractDeploymentsAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 调用合约方法（只读）
    /// </summary>
    /// <param name="request">调用请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>调用结果</returns>
    [HttpPost("contracts/call")]
    public async Task<ActionResult<ContractMethodCallResponseDto>> CallContractMethod(
        [FromBody] CallContractMethodRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calling contract method: {MethodName} on {ContractAddress}", 
            request.MethodName, request.ContractAddress);

        var result = await _web3Service.CallContractMethodAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 发送合约交易（写入）
    /// </summary>
    /// <param name="request">交易请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>交易结果</returns>
    [HttpPost("contracts/send")]
    public async Task<ActionResult<ContractMethodCallResponseDto>> SendContractTransaction(
        [FromBody] SendContractTransactionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending contract transaction: {MethodName} on {ContractAddress}", 
            request.MethodName, request.ContractAddress);

        var result = await _web3Service.SendContractTransactionAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 获取合约事件
    /// </summary>
    /// <param name="request">事件查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>事件列表</returns>
    [HttpPost("contracts/events")]
    public async Task<ActionResult<List<object>>> GetContractEvents(
        [FromBody] ContractEventsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting contract events: {EventName} from {ContractAddress}", 
            request.EventName, request.ContractAddress);

        var result = await _web3Service.GetContractEventsAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 验证合约
    /// </summary>
    /// <param name="request">验证请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>验证结果</returns>
    [HttpPost("contracts/verify")]
    public async Task<ActionResult<bool>> VerifyContract(
        [FromBody] VerifyContractRequestDto request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying contract: {ContractAddress}", request.ContractAddress);

        var result = await _web3Service.VerifyContractAsync(request, cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    #endregion

    #region 网络信息

    /// <summary>
    /// 获取网络信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>网络信息</returns>
    [HttpGet("network/info")]
    public async Task<ActionResult<object>> GetNetworkInfo(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting network information");

        var result = await _web3Service.GetNetworkInfoAsync(cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 获取当前Gas价格
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>Gas价格</returns>
    [HttpGet("network/gas-price")]
    public async Task<ActionResult<decimal>> GetGasPrice(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting current gas price");

        var result = await _web3Service.GetGasPriceAsync(cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    /// <summary>
    /// 检查网络连接状态
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>连接状态</returns>
    [HttpGet("network/status")]
    public async Task<ActionResult<bool>> CheckNetworkStatus(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking network connection status");

        var result = await _web3Service.CheckNetworkStatusAsync(cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    #endregion
}


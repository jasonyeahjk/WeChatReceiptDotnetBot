using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluentResults;
using Web3Service.API.Application.DTOs.AccountDTOs;
using Web3Service.API.Application.DTOs.ContractDTOs;
using Web3Service.API.Application.DTOs.TransactionDTOs;
using Web3Service.API.Application.DTOs.Web3DTOs;
using IWeb3ApplicationService = Web3Service.API.Application.Interfaces.IWeb3ApplicationService;
using WeChatReceiptBot.API.Common;
using WeChatReceiptBot.API.Extensions;
using System.Security.Claims;

namespace WeChatReceiptBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class Web3Controller : ControllerBase
    {
        private readonly ILogger<Web3Controller> _logger;
        private readonly IWeb3ApplicationService _web3Service;

        public Web3Controller(
            ILogger<Web3Controller> logger,
            IWeb3ApplicationService web3Service)
        {
            _logger = logger;
            _web3Service = web3Service;
        }

        [HttpPost("accounts")]
        public async Task<ActionResult<AccountResponseDto>> CreateAccount([FromBody] CreateAccountRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Creating Web3 account for user: {UserId}", userId);

            
            request.UserId = userId;
            var result = await _web3Service.CreateAccountAsync(request);
            return result.ToActionResult(HttpContext);
        }

        [HttpGet("accounts/{address}/balance")]
        public async Task<ActionResult<AccountBalanceResponseDto>> GetAccountBalance(string address)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Getting balance for address: {Address}, user: {UserId}", address, userId);

            var result = await _web3Service.GetAccountBalanceAsync(address);
            return result.ToActionResult(HttpContext);
        }

        [HttpPost("contracts/deploy")]
        public async Task<ActionResult<ContractDeploymentResponseDto>> DeployContract([FromBody] DeployContractRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Deploying contract for user: {UserId}", userId);

            request.DeployedByUserId = userId; 
            var result = await _web3Service.DeployContractAsync(request);
            return result.ToActionResult(HttpContext);
        }

        [HttpPost("contracts/{contractAddress}/call")]
        public async Task<ActionResult<ContractMethodCallResponseDto>> CallContract(string contractAddress, [FromBody] CallContractMethodRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Calling contract method for user: {UserId}, contract: {ContractAddress}", userId, contractAddress);

            request.ContractAddress = contractAddress; 
            var result = await _web3Service.CallContractMethodAsync(request);
            return result.ToActionResult(HttpContext);
        }

        [HttpGet("transactions/{txHash}")]
        public async Task<ActionResult<TransactionResponseDto>> GetTransaction(string txHash)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Getting transaction details: {TxHash} for user: {UserId}", txHash, userId);

            var result = await _web3Service.GetTransactionByHashAsync(txHash);
            return result.ToActionResult(HttpContext);
        }

        [HttpPost("gas/estimate")]
        public async Task<ActionResult<EstimateGasResponseDto>> EstimateGas([FromBody] EstimateGasRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Estimating gas for user: {UserId}", userId);

            
            var result = await _web3Service.EstimateGasAsync(request);
            return result.ToActionResult(HttpContext);
        }

        [HttpGet("network/status")]
        public async Task<ActionResult<NetworkStatusResponseDto>> GetNetworkStatus()
        {
            _logger.LogInformation("Getting network status");
            var result = await _web3Service.CheckNetworkStatusAsync();
            var res = new Result<NetworkStatusResponseDto>();
            if (result.IsSuccess)
            {
                res = Result.Ok(new NetworkStatusResponseDto
                {
                    IsSynchronizing = result.Value
                });
            }
            else
            {
                res = Result.Fail<NetworkStatusResponseDto>(result.Errors);
            }
            return res.ToActionResult(HttpContext);
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<Web3StatisticsDto>> GetWeb3Statistics()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Getting Web3 statistics for user: {UserId}", userId);

                var statistics = new Web3StatisticsDto
                {
                    TotalTransactions = 15,
                    SuccessfulTransactions = 14,
                    FailedTransactions = 1,
                    TotalGasUsed = 3150000,
                    ContractsDeployed = 3,
                    ContractCalls = 12,
                    AverageGasPrice = "21000000000"
                };

                return this.Success(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Web3 statistics");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        private static bool IsValidAddress(string address)
        {
            return !string.IsNullOrWhiteSpace(address) && 
                   address.StartsWith("0x") && 
                   address.Length == 42;
        }

        private static bool IsValidTransactionHash(string txHash)
        {
            return !string.IsNullOrWhiteSpace(txHash) && 
                   txHash.StartsWith("0x") && 
                   txHash.Length == 66;
        }

        private Result ValidateContractDeploymentRequest(DeployContractRequestDto request)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.ContractName))
            {
                errors.Add("ContractName", new[] { "合约名称不能为空" });
            }

            if (string.IsNullOrWhiteSpace(request.ByteCode))
            {
                errors.Add("ByteCode", new[] { "合约字节码不能为空" });
            }

            if (!IsValidAddress(request.FromAddress))
            {
                errors.Add("FromAddress", new[] { "无效的发送地址" });
            }

            if (errors.Any())
            {
                return Result.Fail(new ValidationError(errors));
            }

            return Result.Ok();
        }

        private Result ValidateContractCallRequest(CallContractMethodRequestDto request)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.MethodName))
            {
                errors.Add("MethodName", new[] { "方法名称不能为空" });
            }

            if (!IsValidAddress(request.FromAddress))
            {
                errors.Add("FromAddress", new[] { "无效的发送地址" });
            }

            if (errors.Any())
            {
                return Result.Fail(new ValidationError(errors));
            }

            return Result.Ok();
        }

        private Result ValidateGasEstimateRequest(EstimateGasRequestDto request)
        {
            var errors = new Dictionary<string, string[]>();

            if (!IsValidAddress(request.FromAddress))
            {
                errors.Add("From", new[] { "无效的发送地址" });
            }

            if (!IsValidAddress(request.ToAddress))
            {
                errors.Add("To", new[] { "无效的接收地址" });
            }

            if (errors.Any())
            {
                return Result.Fail(new ValidationError(errors));
            }

            return Result.Ok();
        }
    }
}



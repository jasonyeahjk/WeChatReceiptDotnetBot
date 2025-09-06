using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FluentResults;
using WeChatReceiptBot.API.DTOs;
using WeChatReceiptBot.API.Services;
using WeChatReceiptBot.API.Common;
using WeChatReceiptBot.API.Extensions;
using System.Security.Claims;

namespace WeChatReceiptBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> _logger;


        public TransactionController(
            ILogger<TransactionController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 创建交易
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> CreateTransaction([FromBody] CreateTransactionRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Creating transaction for user: {UserId}", userId);

                // 验证请求数据
                var validationResult = ValidateCreateTransactionRequest(request);
                if (!validationResult.IsSuccess)
                {
                    if (validationResult.Errors.First() is ValidationError validationError)
                    {
                        this.ValidationError(validationError.ValidationErrors);
                    }
                    this.ValidationError(request.BillId.ToString(), "验证请求数据不通过");
                }

                // TODO: 实现创建交易逻辑
                // 1. 验证账单是否存在且用户有权限
                // 2. 在NebulaGraph中创建交易节点
                // 3. 在Doris中记录交易数据
                // 4. 更新智能合约

                var transactionResponse = new TransactionResponse
                {
                    Id = Guid.NewGuid(),
                    BillId = request.BillId,
                    PayerId = Guid.Parse(userId!),
                    Amount = request.Amount,
                    Description = request.Description,
                    TransactionType = request.TransactionType,
                    Timestamp = DateTime.UtcNow,
                    Status = "Pending"
                };

                return this.Created($"/api/transactions/{transactionResponse.Id.ToString()}", transactionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 获取账单的交易列表
        /// </summary>
        [HttpGet("bill/{billId}")]
        public async Task<ActionResult<List<TransactionResponse>>> GetBillTransactions(Guid billId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Getting transactions for bill: {BillId}, user: {UserId}", billId, userId);

                // TODO: 验证用户是否有权限访问该账单
                // TODO: 从NebulaGraph查询账单的交易列表

                var transactions = new List<TransactionResponse>
                {
                    new TransactionResponse
                    {
                        Id = Guid.NewGuid(),
                        BillId = billId,
                        PayerId = Guid.Parse(userId!),
                        Amount = 680.00m,
                        Description = "聚餐费用",
                        TransactionType = "Expense",
                        Timestamp = DateTime.UtcNow.AddDays(-3),
                        Status = "Confirmed"
                    },
                    new TransactionResponse
                    {
                        Id = Guid.NewGuid(),
                        BillId = billId,
                        PayerId = Guid.NewGuid(),
                        Amount = 136.00m,
                        Description = "个人分摊",
                        TransactionType = "Payment",
                        Timestamp = DateTime.UtcNow.AddDays(-2),
                        Status = "Pending"
                    }
                };

                return this.Success(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill transactions");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 获取交易详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDetailResponse>> GetTransactionDetail(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Getting transaction detail for transaction: {TransactionId}, user: {UserId}", id, userId);

                // TODO: 从NebulaGraph查询交易详情
                // TODO: 验证用户是否有权限访问该交易

                var transactionDetail = new TransactionDetailResponse
                {
                    Id = id,
                    BillId = Guid.NewGuid(),
                    PayerId = Guid.Parse(userId!),
                    Amount = 680.00m,
                    Description = "聚餐费用",
                    TransactionType = "Expense",
                    Timestamp = DateTime.UtcNow.AddDays(-3),
                    Status = "Confirmed",
                    Beneficiaries = new List<TransactionBeneficiary>
                    {
                        new TransactionBeneficiary
                        {
                            UserId = Guid.Parse(userId ?? Guid.NewGuid().ToString()),
                            Username = "current_user",
                            Amount = 136.00m
                        },
                        new TransactionBeneficiary
                        {
                            UserId = Guid.NewGuid(),
                            Username = "friend1",
                            Amount = 136.00m
                        }
                    },
                    BlockchainTxHash = "0xabcdef1234567890",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                };

                return this.Success(transactionDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction detail");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 更新交易信息
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionResponse>> UpdateTransaction(Guid id, [FromBody] UpdateTransactionRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Updating transaction: {TransactionId} by user: {UserId}", id, userId);

                // 验证请求数据
                var validationResult = ValidateUpdateTransactionRequest(request);
                if (!validationResult.IsSuccess)
                {
                    if (validationResult.Errors.First() is ValidationError validationError)
                    {
                        this.ValidationError(validationError.ValidationErrors);
                    }
                    this.ValidationError(id.ToString(), "验证请求数据不通过");
                }

                // TODO: 验证用户是否有权限更新交易
                // TODO: 检查交易状态是否允许更新
                // TODO: 更新NebulaGraph中的交易信息

                // 模拟交易已确认，无法更新
                if (request.Status == "Confirmed")
                {
                    return this.Error(ErrorCodes.TRANSACTION_ALREADY_CONFIRMED, 409);
                }

                var updatedTransaction = new TransactionResponse
                {
                    Id = id,
                    BillId = Guid.NewGuid(),
                    PayerId = Guid.Parse(userId!),
                    Amount = request.Amount,
                    Description = request.Description,
                    TransactionType = request.TransactionType,
                    Timestamp = DateTime.UtcNow.AddDays(-3),
                    Status = request.Status
                };

                return this.Success(updatedTransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating transaction");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 确认交易
        /// </summary>
        [HttpPost("{id}/confirm")]
        public async Task<ActionResult> ConfirmTransaction(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Confirming transaction: {TransactionId} by user: {UserId}", id, userId);

                // TODO: 验证用户是否有权限确认交易
                // TODO: 检查交易状态
                // TODO: 更新智能合约状态
                // TODO: 在Doris中记录确认数据

                return this.NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming transaction");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 删除交易
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransaction(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Deleting transaction: {TransactionId} by user: {UserId}", id, userId);

                // TODO: 验证用户是否有权限删除交易
                // TODO: 检查交易是否可以删除（未确认的交易）
                // TODO: 从NebulaGraph删除交易节点

                return this.NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting transaction");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 获取用户的交易统计
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<TransactionStatistics>> GetTransactionStatistics()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Getting transaction statistics for user: {UserId}", userId);

                // TODO: 从Doris查询用户的交易统计数据
                var statistics = new TransactionStatistics
                {
                    TotalTransactions = 25,
                    PendingTransactions = 3,
                    ConfirmedTransactions = 22,
                    TotalAmount = 8950.75m,
                    TotalExpenses = 5680.50m,
                    TotalPayments = 3270.25m,
                    AverageTransactionAmount = 358.03m
                };

                return this.Success(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction statistics");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 验证创建交易请求
        /// </summary>
        private Result ValidateCreateTransactionRequest(CreateTransactionRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            if (request.BillId == Guid.Empty)
            {
                errors.Add("BillId", new[] { "账单ID不能为空" });
            }

            if (request.Amount <= 0)
            {
                errors.Add("Amount", new[] { "金额必须大于0" });
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                errors.Add("Description", new[] { "描述不能为空" });
            }

            if (string.IsNullOrWhiteSpace(request.TransactionType))
            {
                errors.Add("TransactionType", new[] { "交易类型不能为空" });
            }

            if (errors.Any())
            {
                return Result.Fail(new ValidationError(errors));
            }

            return Result.Ok();
        }

        /// <summary>
        /// 验证更新交易请求
        /// </summary>
        private Result ValidateUpdateTransactionRequest(UpdateTransactionRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            if (request.Amount <= 0)
            {
                errors.Add("Amount", new[] { "金额必须大于0" });
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                errors.Add("Description", new[] { "描述不能为空" });
            }

            if (string.IsNullOrWhiteSpace(request.TransactionType))
            {
                errors.Add("TransactionType", new[] { "交易类型不能为空" });
            }

            if (errors.Any())
            {
                return Result.Fail(new ValidationError(errors));
            }

            return Result.Ok();
        }
    }
}


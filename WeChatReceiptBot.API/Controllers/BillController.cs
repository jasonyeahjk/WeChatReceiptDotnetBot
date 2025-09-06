using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WeChatReceiptBot.API.DTOs;
using Web3Service.API.Application.Interfaces;
using System.Security.Claims;

namespace WeChatReceiptBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BillController : ControllerBase
    {
        private readonly ILogger<BillController> _logger;


        public BillController(
            ILogger<BillController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 创建账单
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<BillResponse>>> CreateBill([FromBody] CreateBillRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Creating bill for user: {UserId}", userId);

                // TODO: 实现创建账单逻辑
                // 1. 在NebulaGraph中创建账单节点
                // 2. 建立账单与拼团的关系
                // 3. 在Doris中记录账单数据
                // 4. 创建智能合约

                var billResponse = new BillResponse
                {
                    Id = Guid.NewGuid(),
                    BillName = request.BillName,
                    Description = request.Description,
                    GroupId = request.GroupId,
                    CreatorId = Guid.Parse(userId!),
                    TotalAmount = request.TotalAmount,
                    SettledAmount = 0,
                    Currency = request.Currency ?? "CNY",
                    CreatedAt = DateTime.UtcNow,
                    Status = "Active"
                };

                var response = new ApiResponse<BillResponse>
                {
                    Success = true,
                    Message = "账单创建成功",
                    Data = billResponse
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bill");
                return StatusCode(500, new ApiResponse<BillResponse>
                {
                    Success = false,
                    Message = "创建账单失败"
                });
            }
        }

        /// <summary>
        /// 获取拼团的账单列表
        /// </summary>
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<ApiResponse<List<BillResponse>>>> GetGroupBills(Guid groupId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Getting bills for group: {GroupId}, user: {UserId}", groupId, userId);

                // TODO: 从NebulaGraph查询拼团的账单列表
                var bills = new List<BillResponse>
                {
                    new BillResponse
                    {
                        Id = Guid.NewGuid(),
                        BillName = "聚餐费用",
                        Description = "海底捞聚餐",
                        GroupId = groupId,
                        CreatorId = Guid.Parse(userId!),
                        TotalAmount = 680.00m,
                        SettledAmount = 680.00m,
                        Currency = "CNY",
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        Status = "Settled"
                    },
                    new BillResponse
                    {
                        Id = Guid.NewGuid(),
                        BillName = "KTV费用",
                        Description = "唱歌娱乐费用",
                        GroupId = groupId,
                        CreatorId = Guid.NewGuid(),
                        TotalAmount = 320.00m,
                        SettledAmount = 120.00m,
                        Currency = "CNY",
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        Status = "Active"
                    }
                };

                var response = new ApiResponse<List<BillResponse>>
                {
                    Success = true,
                    Message = "获取账单列表成功",
                    Data = bills
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting group bills");
                return StatusCode(500, new ApiResponse<List<BillResponse>>
                {
                    Success = false,
                    Message = "获取账单列表失败"
                });
            }
        }

        /// <summary>
        /// 获取账单详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BillDetailResponse>>> GetBillDetail(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Getting bill detail for bill: {BillId}, user: {UserId}", id, userId);

                // TODO: 从NebulaGraph查询账单详情和交易记录
                var billDetail = new BillDetailResponse
                {
                    Id = id,
                    BillName = "聚餐费用",
                    Description = "海底捞聚餐",
                    GroupId = Guid.NewGuid(),
                    CreatorId = Guid.Parse(userId!),
                    TotalAmount = 680.00m,
                    SettledAmount = 680.00m,
                    Currency = "CNY",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    Status = "Settled",
                    Transactions = new List<TransactionResponse>
                    {
                        new TransactionResponse
                        {
                            Id = Guid.NewGuid(),
                            BillId = id,
                            PayerId = Guid.Parse(userId!),
                            Amount = 680.00m,
                            Description = "聚餐总费用",
                            TransactionType = "Expense",
                            Timestamp = DateTime.UtcNow.AddDays(-3),
                            Status = "Confirmed"
                        }
                    },
                    MemberShares = new List<BillMemberShareResponse>
                    {
                        new BillMemberShareResponse
                        {
                            UserId = Guid.Parse(userId!),
                            Username = "current_user",
                            ShareAmount = 136.00m,
                            PaidAmount = 680.00m,
                            Balance = 544.00m
                        }
                    }
                };

                var response = new ApiResponse<BillDetailResponse>
                {
                    Success = true,
                    Message = "获取账单详情成功",
                    Data = billDetail
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill detail");
                return StatusCode(500, new ApiResponse<BillDetailResponse>
                {
                    Success = false,
                    Message = "获取账单详情失败"
                });
            }
        }

        /// <summary>
        /// 更新账单信息
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<BillResponse>>> UpdateBill(Guid id, [FromBody] UpdateBillRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Updating bill: {BillId} by user: {UserId}", id, userId);

                // TODO: 验证用户是否有权限更新账单
                // TODO: 更新NebulaGraph中的账单信息

                var updatedBill = new BillResponse
                {
                    Id = id,
                    BillName = request.BillName,
                    Description = request.Description,
                    GroupId = Guid.NewGuid(),
                    CreatorId = Guid.Parse(userId!),
                    TotalAmount = request.TotalAmount,
                    SettledAmount = 0,
                    Currency = request.Currency ?? "CNY",
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    Status = "Active"
                };

                var response = new ApiResponse<BillResponse>
                {
                    Success = true,
                    Message = "账单更新成功",
                    Data = updatedBill
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bill");
                return StatusCode(500, new ApiResponse<BillResponse>
                {
                    Success = false,
                    Message = "更新账单失败"
                });
            }
        }

        /// <summary>
        /// 结算账单
        /// </summary>
        [HttpPost("{id}/settle")]
        public async Task<ActionResult<ApiResponse<object>>> SettleBill(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Settling bill: {BillId} by user: {UserId}", id, userId);

                // TODO: 实现账单结算逻辑
                // 1. 验证所有交易是否已确认
                // 2. 计算最终分摊金额
                // 3. 更新智能合约状态
                // 4. 在Doris中记录结算数据

                var response = new ApiResponse<object>
                {
                    Success = true,
                    Message = "账单结算成功",
                    Data = new { SettledAt = DateTime.UtcNow }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error settling bill");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "账单结算失败"
                });
            }
        }

        /// <summary>
        /// 删除账单
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteBill(Guid id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Deleting bill: {BillId} by user: {UserId}", id, userId);

                // TODO: 验证用户是否有权限删除账单
                // TODO: 检查账单是否可以删除（未结算的账单）
                // TODO: 从NebulaGraph删除账单节点和相关关系

                var response = new ApiResponse<object>
                {
                    Success = true,
                    Message = "账单删除成功",
                    Data = null
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bill");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "删除账单失败"
                });
            }
        }

        /// <summary>
        /// 获取用户的账单统计
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<ApiResponse<BillStatisticsResponse>>> GetBillStatistics()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Getting bill statistics for user: {UserId}", userId);

                // TODO: 从Doris查询用户的账单统计数据
                var statistics = new BillStatisticsResponse
                {
                    TotalBills = 15,
                    ActiveBills = 3,
                    SettledBills = 12,
                    TotalAmount = 5680.50m,
                    TotalPaid = 2340.25m,
                    TotalReceived = 1890.75m,
                    PendingAmount = 450.50m
                };

                var response = new ApiResponse<BillStatisticsResponse>
                {
                    Success = true,
                    Message = "获取账单统计成功",
                    Data = statistics
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bill statistics");
                return StatusCode(500, new ApiResponse<BillStatisticsResponse>
                {
                    Success = false,
                    Message = "获取账单统计失败"
                });
            }
        }
    }
}


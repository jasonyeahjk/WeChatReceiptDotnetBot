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
    public class DonutController : ControllerBase
    {
        private readonly ILogger<DonutController> _logger;

        public DonutController(
            ILogger<DonutController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 识别收据
        /// </summary>
        [HttpPost("recognize/receipt")]
        public async Task<ActionResult<ReceiptRecognitionResponse>> RecognizeReceipt([FromBody] ReceiptRecognitionRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Receipt recognition request from user: {UserId}", userId);

                // 验证请求数据
                var validationResult = ValidateRecognitionRequest(request.ImageBase64, request.GroupId);
                if (!validationResult.IsSuccess)
                {
                    if (validationResult.Errors.First() is ValidationError validationError)
                    {
                        this.ValidationError(validationError.ValidationErrors);
                    }
                    this.ValidationError(request.GroupId.ToString(), "验证请求数据不通过");
                }

                // TODO: 调用Donut服务识别收据
                // TODO: 解析识别结果
                // TODO: 生成智能合约
                // TODO: 返回识别结果

                // 模拟识别结果
                var recognitionResponse = new ReceiptRecognitionResponse
                {
                    RecognitionId = Guid.NewGuid(),
                    Confidence = 0.95f,
                    MerchantName = "海底捞火锅",
                    TotalAmount = 680.00m,
                    Currency = "CNY",
                    TransactionDate = DateTime.UtcNow.AddDays(-1),
                    Items = new List<ReceiptItem>
                    {
                        new ReceiptItem
                        {
                            Name = "毛肚",
                            Quantity = 2,
                            UnitPrice = 58.00m,
                            TotalPrice = 116.00m
                        },
                        new ReceiptItem
                        {
                            Name = "肥牛",
                            Quantity = 3,
                            UnitPrice = 68.00m,
                            TotalPrice = 204.00m
                        }
                    },
                    SmartContractAddress = "0xabcdef1234567890",
                    BlockchainTxHash = "0x1234567890abcdef",
                    ProcessedAt = DateTime.UtcNow
                };

                return this.Success(recognitionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during receipt recognition");
                return this.Error(ErrorCodes.DONUT_RECOGNITION_FAILED, 500);
            }
        }

        /// <summary>
        /// 识别支付记录
        /// </summary>
        [HttpPost("recognize/payment")]
        public async Task<ActionResult<PaymentRecognitionResponse>> RecognizePayment([FromBody] PaymentRecognitionRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Payment recognition request from user: {UserId}", userId);

                // 验证请求数据
                var validationResult = ValidatePaymentRecognitionRequest(request);
                if (!validationResult.IsSuccess)
                {
                    if (validationResult.Errors.First() is ValidationError validationError)
                    {
                        this.ValidationError(validationError.ValidationErrors);
                    }
                    this.ValidationError(request.TransactionId.ToString(), "验证请求数据不通过");
                }

                // TODO: 调用Donut服务识别支付记录
                // TODO: 解析识别结果
                // TODO: 验证支付记录与交易的匹配
                // TODO: 更新智能合约状态

                // 模拟识别结果
                var recognitionResponse = new PaymentRecognitionResponse
                {
                    RecognitionId = Guid.NewGuid(),
                    Confidence = 0.92f,
                    PaymentMethod = "微信支付",
                    Amount = 136.00m,
                    Currency = "CNY",
                    PaymentDate = DateTime.UtcNow.AddHours(-2),
                    TransactionId = "4200001234567890",
                    PayerName = "张三",
                    ReceiverName = "海底捞火锅",
                    IsVerified = true,
                    BlockchainTxHash = "0xfedcba0987654321",
                    ProcessedAt = DateTime.UtcNow
                };

                return this.Success(recognitionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during payment recognition");
                return this.Error(ErrorCodes.DONUT_RECOGNITION_FAILED, 500);
            }
        }

        /// <summary>
        /// 识别通用文档
        /// </summary>
        [HttpPost("recognize/document")]
        public async Task<ActionResult<DocumentRecognitionResponse>> RecognizeDocument([FromBody] DocumentRecognitionRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Document recognition request from user: {UserId}", userId);

                // 验证请求数据
                if (string.IsNullOrWhiteSpace(request.ImageBase64))
                {
                    return this.ValidationError("ImageBase64", "图像数据不能为空");
                }

                // TODO: 调用Donut服务识别文档
                // TODO: 解析识别结果

                // 模拟识别结果
                var recognitionResponse = new DocumentRecognitionResponse
                {
                    RecognitionId = Guid.NewGuid(),
                    DocumentType = request.DocumentType ?? "Unknown",
                    Confidence = 0.88f,
                    ExtractedText = "这是一份示例文档的识别结果...",
                    StructuredData = new Dictionary<string, object>
                    {
                        { "title", "示例文档" },
                        { "date", DateTime.UtcNow.ToString("yyyy-MM-dd") },
                        { "amount", "1000.00" }
                    },
                    ProcessedAt = DateTime.UtcNow
                };

                return this.Success(recognitionResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during document recognition");
                return this.Error(ErrorCodes.DONUT_RECOGNITION_FAILED, 500);
            }
        }

        /// <summary>
        /// 获取识别历史
        /// </summary>
        [HttpGet("history")]
        public async Task<ActionResult<List<RecognitionHistory>>> GetRecognitionHistory([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Getting recognition history for user: {UserId}", userId);

                // 验证分页参数
                if (page < 1 || pageSize < 1 || pageSize > 100)
                {
                    return this.ValidationError("Page", "分页参数无效");
                }

                // TODO: 从数据库查询用户的识别历史

                var history = new List<RecognitionHistory>
                {
                    new RecognitionHistory
                    {
                        Id = Guid.NewGuid(),
                        RecognitionType = "Receipt",
                        Confidence = 0.95f,
                        ProcessedAt = DateTime.UtcNow.AddDays(-1),
                        Status = "Success",
                        MerchantName = "海底捞火锅",
                        Amount = 680.00m
                    },
                    new RecognitionHistory
                    {
                        Id = Guid.NewGuid(),
                        RecognitionType = "Payment",
                        Confidence = 0.92f,
                        ProcessedAt = DateTime.UtcNow.AddDays(-2),
                        Status = "Success",
                        MerchantName = "微信支付",
                        Amount = 136.00m
                    }
                };

                return this.Success(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recognition history");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 获取识别详情
        /// </summary>
        [HttpGet("{recognitionId}")]
        public async Task<ActionResult<RecognitionDetail>> GetRecognitionDetail(Guid recognitionId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Getting recognition detail: {RecognitionId} for user: {UserId}", recognitionId, userId);

                // TODO: 从数据库查询识别详情
                // TODO: 验证用户是否有权限访问该识别记录

                var detail = new RecognitionDetail
                {
                    Id = recognitionId,
                    RecognitionType = "Receipt",
                    Confidence = 0.95f,
                    ProcessedAt = DateTime.UtcNow.AddDays(-1),
                    Status = "Success",
                    OriginalImageUrl = "/images/receipts/sample.jpg",
                    ExtractedData = new Dictionary<string, object>
                    {
                        { "merchantName", "海底捞火锅" },
                        { "totalAmount", 680.00m },
                        { "currency", "CNY" },
                        { "transactionDate", DateTime.UtcNow.AddDays(-1) }
                    },
                    SmartContractAddress = "0xabcdef1234567890",
                    BlockchainTxHash = "0x1234567890abcdef"
                };

                return this.Success(detail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recognition detail");
                return this.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500);
            }
        }

        /// <summary>
        /// 验证识别请求
        /// </summary>
        private Result ValidateRecognitionRequest(string imageBase64, Guid? groupId)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(imageBase64))
            {
                errors.Add("ImageBase64", new[] { "图像数据不能为空" });
            }
            else if (!IsValidBase64Image(imageBase64))
            {
                errors.Add("ImageBase64", new[] { "无效的图像格式" });
            }

            if (groupId.HasValue && groupId.Value == Guid.Empty)
            {
                errors.Add("GroupId", new[] { "拼团ID无效" });
            }

            if (errors.Any())
            {
                return Result.Fail(new ValidationError(errors));
            }

            return Result.Ok();
        }

        /// <summary>
        /// 验证支付识别请求
        /// </summary>
        private Result ValidatePaymentRecognitionRequest(PaymentRecognitionRequest request)
        {
            var errors = new Dictionary<string, string[]>();

            if (string.IsNullOrWhiteSpace(request.ImageBase64))
            {
                errors.Add("ImageBase64", new[] { "图像数据不能为空" });
            }
            else if (!IsValidBase64Image(request.ImageBase64))
            {
                errors.Add("ImageBase64", new[] { "无效的图像格式" });
            }

            if (request.TransactionId.HasValue && request.TransactionId.Value == Guid.Empty)
            {
                errors.Add("TransactionId", new[] { "交易ID无效" });
            }

            if (errors.Any())
            {
                return Result.Fail(new ValidationError(errors));
            }

            return Result.Ok();
        }

        /// <summary>
        /// 验证Base64图像格式
        /// </summary>
        private static bool IsValidBase64Image(string base64String)
        {
            try
            {
                // 检查是否包含数据URL前缀
                if (base64String.StartsWith("data:image/"))
                {
                    var commaIndex = base64String.IndexOf(',');
                    if (commaIndex > 0)
                    {
                        base64String = base64String.Substring(commaIndex + 1);
                    }
                }

                // 尝试解码Base64
                var bytes = Convert.FromBase64String(base64String);
                return bytes.Length > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}


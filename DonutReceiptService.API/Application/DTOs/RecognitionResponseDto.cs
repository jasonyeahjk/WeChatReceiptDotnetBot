using DonutReceiptService.API.Domain.ValueObjects;

namespace DonutReceiptService.API.Application.DTOs
{
    /// <summary>
    /// 识别响应 DTO
    /// </summary>
    public class RecognitionResponseDto
    {
        /// <summary>
        /// 识别任务ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 识别状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 提取的收据数据
        /// </summary>
        public ReceiptDataDto? ExtractedData { get; set; }

        /// <summary>
        /// 置信度分数
        /// </summary>
        public float? ConfidenceScore { get; set; }

        /// <summary>
        /// 处理时间（毫秒）
        /// </summary>
        public long? ProcessingTimeMs { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// 元数据
        /// </summary>
        public Dictionary<string, object>? Metadata { get; set; }
    }

    /// <summary>
    /// 收据数据 DTO
    /// </summary>
    public class ReceiptDataDto
    {
        /// <summary>
        /// 商家名称
        /// </summary>
        public string MerchantName { get; set; } = string.Empty;

        /// <summary>
        /// 商家地址
        /// </summary>
        public string MerchantAddress { get; set; } = string.Empty;

        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public string Currency { get; set; } = string.Empty;

        /// <summary>
        /// 收据号码
        /// </summary>
        public string? ReceiptNumber { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string? PaymentMethod { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        public decimal? TaxAmount { get; set; }

        /// <summary>
        /// 小费
        /// </summary>
        public decimal? TipAmount { get; set; }

        /// <summary>
        /// 收据项目列表
        /// </summary>
        public List<ReceiptItemDto> Items { get; set; } = new();

        /// <summary>
        /// 附加字段
        /// </summary>
        public Dictionary<string, object>? AdditionalFields { get; set; }
    }

    /// <summary>
    /// 收据项目 DTO
    /// </summary>
    public class ReceiptItemDto
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 类别
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// 识别历史查询响应 DTO
    /// </summary>
    public class RecognitionHistoryResponseDto
    {
        /// <summary>
        /// 识别结果列表
        /// </summary>
        public List<RecognitionResponseDto> Results { get; set; } = new();

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// 识别统计响应 DTO
    /// </summary>
    public class RecognitionStatisticsResponseDto
    {
        /// <summary>
        /// 总识别次数
        /// </summary>
        public int TotalRecognitions { get; set; }

        /// <summary>
        /// 成功识别次数
        /// </summary>
        public int SuccessfulRecognitions { get; set; }

        /// <summary>
        /// 失败识别次数
        /// </summary>
        public int FailedRecognitions { get; set; }

        /// <summary>
        /// 等待处理次数
        /// </summary>
        public int PendingRecognitions { get; set; }

        /// <summary>
        /// 成功率
        /// </summary>
        public double SuccessRate { get; set; }

        /// <summary>
        /// 平均处理时间（秒）
        /// </summary>
        public double AverageProcessingTimeSeconds { get; set; }

        /// <summary>
        /// 平均置信度分数
        /// </summary>
        public float AverageConfidenceScore { get; set; }

        /// <summary>
        /// 最后识别日期
        /// </summary>
        public DateTime? LastRecognitionDate { get; set; }
    }
}


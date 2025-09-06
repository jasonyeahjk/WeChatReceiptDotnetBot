using DonutReceiptService.API.Domain.ValueObjects;

namespace DonutReceiptService.API.Domain.Entities
{
    /// <summary>
    /// 收据识别结果实体
    /// </summary>
    public class ReceiptRecognitionResult
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public string OriginalImagePath { get; private set; }
        public RecognitionStatus Status { get; private set; }
        public ReceiptData? ExtractedData { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public TimeSpan? ProcessingTime { get; private set; }
        public float? ConfidenceScore { get; private set; }

        private ReceiptRecognitionResult() { } // For EF Core

        public ReceiptRecognitionResult(
            string userId,
            string originalImagePath)
        {
            Id = Guid.NewGuid();
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            OriginalImagePath = originalImagePath ?? throw new ArgumentNullException(nameof(originalImagePath));
            Status = RecognitionStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// 标记识别开始
        /// </summary>
        public void StartProcessing()
        {
            if (Status != RecognitionStatus.Pending)
            {
                throw new InvalidOperationException($"Cannot start processing when status is {Status}");
            }

            Status = RecognitionStatus.Processing;
        }

        /// <summary>
        /// 标记识别成功完成
        /// </summary>
        public void CompleteSuccessfully(ReceiptData extractedData, float confidenceScore)
        {
            if (Status != RecognitionStatus.Processing)
            {
                throw new InvalidOperationException($"Cannot complete when status is {Status}");
            }

            ExtractedData = extractedData ?? throw new ArgumentNullException(nameof(extractedData));
            ConfidenceScore = confidenceScore;
            Status = RecognitionStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            ProcessingTime = CompletedAt - CreatedAt;
        }

        /// <summary>
        /// 标记识别失败
        /// </summary>
        public void MarkAsFailed(string errorMessage)
        {
            if (Status != RecognitionStatus.Processing)
            {
                throw new InvalidOperationException($"Cannot mark as failed when status is {Status}");
            }

            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            Status = RecognitionStatus.Failed;
            CompletedAt = DateTime.UtcNow;
            ProcessingTime = CompletedAt - CreatedAt;
        }

        /// <summary>
        /// 重试识别
        /// </summary>
        public void Retry()
        {
            if (Status != RecognitionStatus.Failed)
            {
                throw new InvalidOperationException($"Cannot retry when status is {Status}");
            }

            Status = RecognitionStatus.Pending;
            ErrorMessage = null;
            CompletedAt = null;
            ProcessingTime = null;
            ConfidenceScore = null;
            ExtractedData = null;
        }
    }
}


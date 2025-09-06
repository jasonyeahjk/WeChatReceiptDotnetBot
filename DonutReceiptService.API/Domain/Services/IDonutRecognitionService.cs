using DonutReceiptService.API.Domain.ValueObjects;
using FluentResults;

namespace DonutReceiptService.API.Domain.Services
{
    /// <summary>
    /// Donut 识别服务领域接口
    /// </summary>
    public interface IDonutRecognitionService
    {
        /// <summary>
        /// 识别收据图像
        /// </summary>
        Task<Result<RecognitionResult>> RecognizeReceiptAsync(
            byte[] imageData, 
            string fileName, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 验证图像格式
        /// </summary>
        Result<bool> ValidateImageFormat(byte[] imageData, string fileName);

        /// <summary>
        /// 获取支持的图像格式
        /// </summary>
        IEnumerable<string> GetSupportedFormats();

        /// <summary>
        /// 预处理图像
        /// </summary>
        Task<Result<byte[]>> PreprocessImageAsync(
            byte[] imageData, 
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 识别结果
    /// </summary>
    public class RecognitionResult
    {
        public ReceiptData ExtractedData { get; }
        public float ConfidenceScore { get; }
        public TimeSpan ProcessingTime { get; }
        public Dictionary<string, object> Metadata { get; }

        public RecognitionResult(
            ReceiptData extractedData,
            float confidenceScore,
            TimeSpan processingTime,
            Dictionary<string, object>? metadata = null)
        {
            ExtractedData = extractedData ?? throw new ArgumentNullException(nameof(extractedData));
            ConfidenceScore = confidenceScore;
            ProcessingTime = processingTime;
            Metadata = metadata ?? new Dictionary<string, object>();
        }
    }
}


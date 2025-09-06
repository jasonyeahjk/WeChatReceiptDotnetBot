using DonutReceiptService.API.Domain.Entities;
using FluentResults;

namespace DonutReceiptService.API.Domain.Repositories
{
    /// <summary>
    /// 收据识别结果仓储接口
    /// </summary>
    public interface IReceiptRecognitionRepository
    {
        /// <summary>
        /// 根据ID获取识别结果
        /// </summary>
        Task<Result<ReceiptRecognitionResult?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 根据用户ID获取识别结果列表
        /// </summary>
        Task<Result<IEnumerable<ReceiptRecognitionResult>>> GetByUserIdAsync(
            string userId, 
            int pageNumber = 1, 
            int pageSize = 10, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 添加识别结果
        /// </summary>
        Task<Result<ReceiptRecognitionResult>> AddAsync(ReceiptRecognitionResult result, CancellationToken cancellationToken = default);

        /// <summary>
        /// 更新识别结果
        /// </summary>
        Task<Result> UpdateAsync(ReceiptRecognitionResult result, CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除识别结果
        /// </summary>
        Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取用户的识别统计信息
        /// </summary>
        Task<Result<RecognitionStatistics>> GetStatisticsAsync(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取等待处理的识别任务
        /// </summary>
        Task<Result<IEnumerable<ReceiptRecognitionResult>>> GetPendingTasksAsync(
            int limit = 10, 
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 识别统计信息
    /// </summary>
    public class RecognitionStatistics
    {
        public int TotalRecognitions { get; set; }
        public int SuccessfulRecognitions { get; set; }
        public int FailedRecognitions { get; set; }
        public int PendingRecognitions { get; set; }
        public double AverageProcessingTimeSeconds { get; set; }
        public float AverageConfidenceScore { get; set; }
        public DateTime? LastRecognitionDate { get; set; }
    }
}


using DonutReceiptService.API.Application.DTOs;
using FluentResults;

namespace DonutReceiptService.API.Application.Interfaces
{
    /// <summary>
    /// 收据识别应用服务接口
    /// </summary>
    public interface IReceiptRecognitionApplicationService
    {
        /// <summary>
        /// 提交收据识别任务
        /// </summary>
        Task<Result<RecognitionResponseDto>> SubmitRecognitionTaskAsync(
            RecognitionRequestDto request, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取识别结果
        /// </summary>
        Task<Result<RecognitionResponseDto>> GetRecognitionResultAsync(
            Guid taskId, 
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取用户识别历史
        /// </summary>
        Task<Result<RecognitionHistoryResponseDto>> GetRecognitionHistoryAsync(
            string userId,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取用户识别统计
        /// </summary>
        Task<Result<RecognitionStatisticsResponseDto>> GetRecognitionStatisticsAsync(
            string userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 重试失败的识别任务
        /// </summary>
        Task<Result<RecognitionResponseDto>> RetryRecognitionTaskAsync(
            Guid taskId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除识别结果
        /// </summary>
        Task<Result> DeleteRecognitionResultAsync(
            Guid taskId,
            string userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 批量处理等待中的识别任务
        /// </summary>
        Task<Result<int>> ProcessPendingTasksAsync(
            int batchSize = 10,
            CancellationToken cancellationToken = default);
    }
}


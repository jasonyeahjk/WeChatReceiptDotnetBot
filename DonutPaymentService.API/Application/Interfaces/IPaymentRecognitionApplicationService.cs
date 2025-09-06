using FluentResults;
using DonutPaymentService.API.Application.DTOs;

namespace DonutPaymentService.API.Application.Interfaces
{
    public interface IPaymentRecognitionApplicationService
    {
        Task<Result<RecognitionResponseDto>> SubmitRecognitionTaskAsync(RecognitionRequestDto request);
        Task<Result<RecognitionResponseDto>> GetRecognitionResultAsync(Guid taskId);
        Task<Result<RecognitionHistoryResponseDto>> GetRecognitionHistoryAsync(string userId, int pageNumber, int pageSize);
        Task<Result<RecognitionStatisticsResponseDto>> GetRecognitionStatisticsAsync(string userId);
        Task<Result<RecognitionResponseDto>> RetryRecognitionTaskAsync(Guid taskId);
        Task<Result> DeleteRecognitionResultAsync(Guid taskId, string userId);
        Task<Result<int>> ProcessPendingTasksAsync(int batchSize);
    }
}


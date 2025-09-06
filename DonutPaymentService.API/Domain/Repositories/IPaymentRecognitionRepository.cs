using DonutPaymentService.API.Domain.Entities;
using FluentResults;

namespace DonutPaymentService.API.Domain.Repositories
{
    public interface IPaymentRecognitionRepository
    {
        Task<Result> AddAsync(PaymentRecognitionResult result);
        Task<Result<PaymentRecognitionResult>> GetByIdAsync(Guid id);
        Task<Result<List<PaymentRecognitionResult>>> GetByUserIdAsync(string userId, int pageNumber, int pageSize);
        Task<Result> UpdateAsync(PaymentRecognitionResult result);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<List<PaymentRecognitionResult>>> GetPendingTasksAsync(int limit);
        Task<Result<int>> GetTotalCountByUserIdAsync(string userId);
        Task<Result<int>> GetCompletedCountByUserIdAsync(string userId);
        Task<Result<int>> GetFailedCountByUserIdAsync(string userId);
    }
}


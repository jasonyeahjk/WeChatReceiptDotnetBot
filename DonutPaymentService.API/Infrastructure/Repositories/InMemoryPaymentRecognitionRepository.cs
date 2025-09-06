using DonutPaymentService.API.Domain.Entities;
using DonutPaymentService.API.Domain.Repositories;
using DonutPaymentService.API.Domain.ValueObjects;
using FluentResults;
using System.Collections.Concurrent;

namespace DonutPaymentService.API.Infrastructure.Repositories
{
    public class InMemoryPaymentRecognitionRepository : IPaymentRecognitionRepository
    {
        private readonly ConcurrentDictionary<Guid, PaymentRecognitionResult> _results = new();

        public Task<Result> AddAsync(PaymentRecognitionResult result)
        {
            if (_results.TryAdd(result.Id, result))
            {
                return Task.FromResult(Result.Ok());
            }
            return Task.FromResult(Result.Fail("Failed to add payment recognition result."));
        }

        public Task<Result<PaymentRecognitionResult>> GetByIdAsync(Guid id)
        {
            if (_results.TryGetValue(id, out var result))
            {
                return Task.FromResult(Result.Ok(result));
            }
            return Task.FromResult(Result.Fail<PaymentRecognitionResult>(new Common.Errors.ApplicationError(Common.Errors.ErrorCodes.SYSTEM_NOT_FOUND, "Payment recognition result not found.", 404)));
        }

        public Task<Result<List<PaymentRecognitionResult>>> GetByUserIdAsync(string userId, int pageNumber, int pageSize)
        {
            var userResults = _results.Values
                                      .Where(r => r.UserId == userId)
                                      .OrderByDescending(r => r.CreatedAt)
                                      .Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToList();
            return Task.FromResult(Result.Ok(userResults));
        }

        public Task<Result> UpdateAsync(PaymentRecognitionResult result)
        {
            if (_results.ContainsKey(result.Id))
            {
                _results[result.Id] = result; // Overwrite
                return Task.FromResult(Result.Ok());
            }
            return Task.FromResult(Result.Fail("Payment recognition result not found for update."));
        }

        public Task<Result> DeleteAsync(Guid id)
        {
            if (_results.TryRemove(id, out _))
            {
                return Task.FromResult(Result.Ok());
            }
            return Task.FromResult(Result.Fail("Failed to delete payment recognition result."));
        }

        public Task<Result<List<PaymentRecognitionResult>>> GetPendingTasksAsync(int limit)
        {
            var pending = _results.Values
                                  .Where(r => r.Status == RecognitionStatus.Pending || r.Status == RecognitionStatus.Failed)
                                  .OrderBy(r => r.CreatedAt)
                                  .Take(limit)
                                  .ToList();
            return Task.FromResult(Result.Ok(pending));
        }

        public Task<Result<int>> GetTotalCountByUserIdAsync(string userId)
        {
            var count = _results.Values.Count(r => r.UserId == userId);
            return Task.FromResult(Result.Ok(count));
        }

        public Task<Result<int>> GetCompletedCountByUserIdAsync(string userId)
        {
            var count = _results.Values.Count(r => r.UserId == userId && r.Status == RecognitionStatus.Completed);
            return Task.FromResult(Result.Ok(count));
        }

        public Task<Result<int>> GetFailedCountByUserIdAsync(string userId)
        {
            var count = _results.Values.Count(r => r.UserId == userId && r.Status == RecognitionStatus.Failed);
            return Task.FromResult(Result.Ok(count));
        }
    }
}


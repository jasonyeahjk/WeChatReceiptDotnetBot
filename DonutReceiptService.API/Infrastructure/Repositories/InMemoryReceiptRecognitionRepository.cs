using DonutReceiptService.API.Domain.Entities;
using DonutReceiptService.API.Domain.Repositories;
using DonutReceiptService.API.Domain.ValueObjects;
using DonutReceiptService.API.Common.Errors;
using FluentResults;
using System.Collections.Concurrent;

namespace DonutReceiptService.API.Infrastructure.Repositories
{
    /// <summary>
    /// 内存中的收据识别结果仓储实现（用于开发和测试）
    /// </summary>
    public class InMemoryReceiptRecognitionRepository : IReceiptRecognitionRepository
    {
        private readonly ConcurrentDictionary<Guid, ReceiptRecognitionResult> _results = new();

        public Task<Result<ReceiptRecognitionResult?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _results.TryGetValue(id, out var result);
            return Task.FromResult(Result.Ok(result));
        }

        public Task<Result<IEnumerable<ReceiptRecognitionResult>>> GetByUserIdAsync(
            string userId, 
            int pageNumber = 1, 
            int pageSize = 10, 
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userResults = _results.Values
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Task.FromResult(Result.Ok<IEnumerable<ReceiptRecognitionResult>>(userResults));
        }

        public Task<Result<ReceiptRecognitionResult>> AddAsync(ReceiptRecognitionResult result, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_results.TryAdd(result.Id, result))
            {
                return Task.FromResult(Result.Ok(result));
            }
            return Task.FromResult(Result.Fail<ReceiptRecognitionResult>(new ApplicationError(ErrorCodes.DATABASE_INSERT_FAILED, "添加识别结果失败")));
        }

        public Task<Result> UpdateAsync(ReceiptRecognitionResult result, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_results.ContainsKey(result.Id))
            {
                _results[result.Id] = result; // 简单替换，实际应考虑并发更新
                return Task.FromResult(Result.Ok());
            }
            return Task.FromResult(Result.Fail(new ApplicationError(ErrorCodes.DATABASE_UPDATE_FAILED, "更新识别结果失败：未找到记录")));
        }

        public Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_results.TryRemove(id, out _))
            {
                return Task.FromResult(Result.Ok());
            }
            return Task.FromResult(Result.Fail(new ApplicationError(ErrorCodes.DATABASE_DELETE_FAILED, "删除识别结果失败：未找到记录")));
        }

        public Task<Result<RecognitionStatistics>> GetStatisticsAsync(string userId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var userResults = _results.Values.Where(r => r.UserId == userId).ToList();

            var total = userResults.Count;
            var successful = userResults.Count(r => r.Status == RecognitionStatus.Completed);
            var failed = userResults.Count(r => r.Status == RecognitionStatus.Failed);
            var pending = userResults.Count(r => r.Status == RecognitionStatus.Pending || r.Status == RecognitionStatus.Processing);

            var completedResults = userResults.Where(r => r.Status == RecognitionStatus.Completed && r.ProcessingTime.HasValue);
            var avgProcessingTime = completedResults.Any() 
                ? completedResults.Average(r => r.ProcessingTime!.Value.TotalSeconds) 
                : 0;
            var avgConfidenceScore = completedResults.Any() 
                ? completedResults.Average(r => r.ConfidenceScore!.Value) 
                : 0;
            var lastRecognitionDate = userResults.Any() 
                ? userResults.Max(r => r.CreatedAt) 
                : (DateTime?)null;

            var statistics = new RecognitionStatistics
            {
                TotalRecognitions = total,
                SuccessfulRecognitions = successful,
                FailedRecognitions = failed,
                PendingRecognitions = pending,
                AverageProcessingTimeSeconds = avgProcessingTime,
                AverageConfidenceScore = (float)avgConfidenceScore,
                LastRecognitionDate = lastRecognitionDate
            };

            return Task.FromResult(Result.Ok(statistics));
        }

        public Task<Result<IEnumerable<ReceiptRecognitionResult>>> GetPendingTasksAsync(int limit = 10, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var pendingTasks = _results.Values
                .Where(r => r.Status == RecognitionStatus.Pending)
                .OrderBy(r => r.CreatedAt)
                .Take(limit)
                .ToList();
            return Task.FromResult(Result.Ok<IEnumerable<ReceiptRecognitionResult>>(pendingTasks));
        }
    }
}


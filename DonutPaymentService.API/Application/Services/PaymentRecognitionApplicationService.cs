using FluentResults;
using DonutPaymentService.API.Application.DTOs;
using DonutPaymentService.API.Application.Interfaces;
using DonutPaymentService.API.Domain.Entities;
using DonutPaymentService.API.Domain.Repositories;
using DonutPaymentService.API.Domain.Services;
using DonutPaymentService.API.Common.Errors;
using Microsoft.Extensions.Logging;
using DonutPaymentService.API.Domain.ValueObjects;

namespace DonutPaymentService.API.Application.Services
{
    public class PaymentRecognitionApplicationService : IPaymentRecognitionApplicationService
    {
        private readonly IPaymentRecognitionRepository _repository;
        private readonly IPaymentRecognitionService _donutService;
        private readonly ILogger<PaymentRecognitionApplicationService> _logger;

        public PaymentRecognitionApplicationService(
            IPaymentRecognitionRepository repository,
            IPaymentRecognitionService donutService,
            ILogger<PaymentRecognitionApplicationService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _donutService = donutService ?? throw new ArgumentNullException(nameof(donutService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<RecognitionResponseDto>> SubmitRecognitionTaskAsync(RecognitionRequestDto request)
        {
            _logger.LogInformation("Submitting payment recognition task for user {UserId}", request.UserId);

            // Basic validation
            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                return Result.Fail(new ValidationError("UserId", "User ID cannot be empty."));
            }
            if (string.IsNullOrWhiteSpace(request.ImageBase64))
            {
                return Result.Fail(new ValidationError("ImageBase64", "Image data cannot be empty."));
            }

            var newRecognition = new PaymentRecognitionResult(request.UserId, request.ImageBase64);
            var addResult = await _repository.AddAsync(newRecognition);

            if (addResult.IsFailed)
            {
                _logger.LogError("Failed to add new recognition task to repository: {Errors}", addResult.Errors);
                return Result.Fail(new ApplicationError(ErrorCodes.DATABASE_INSERT_FAILED, "Failed to save recognition task."));
            }

            // Optionally, trigger immediate processing or rely on a background worker
            // For now, we'll just return the pending task and assume a background process will pick it up.

            return Result.Ok(MapToDto(newRecognition));
        }

        public async Task<Result<RecognitionResponseDto>> GetRecognitionResultAsync(Guid taskId)
        {
            _logger.LogInformation("Getting payment recognition result for task {TaskId}", taskId);

            var result = await _repository.GetByIdAsync(taskId);
            if (result.IsFailed)
            {
                if (result.Errors.Any(e => e is ApplicationError ae && ae.ErrorCode == ErrorCodes.SYSTEM_NOT_FOUND))
                {
                    return Result.Fail(new ApplicationError(ErrorCodes.DONUT_TASK_NOT_FOUND, "Payment recognition task not found.", 404));
                }
                _logger.LogError("Failed to retrieve recognition task {TaskId} from repository: {Errors}", taskId, result.Errors);
                return Result.Fail(new ApplicationError(ErrorCodes.DATABASE_QUERY_FAILED, "Failed to retrieve recognition task."));
            }

            return Result.Ok(MapToDto(result.Value));
        }

        public async Task<Result<RecognitionHistoryResponseDto>> GetRecognitionHistoryAsync(string userId, int pageNumber, int pageSize)
        {
            _logger.LogInformation("Getting payment recognition history for user {UserId}, page {PageNumber}, size {PageSize}", userId, pageNumber, pageSize);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Fail(new ValidationError("UserId", "User ID cannot be empty."));
            }
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var results = await _repository.GetByUserIdAsync(userId, pageNumber, pageSize);
            var totalCount = await _repository.GetTotalCountByUserIdAsync(userId);

            if (results.IsFailed || totalCount.IsFailed)
            {
                _logger.LogError("Failed to retrieve payment recognition history for user {UserId}: {Errors}", userId, results.Errors.Concat(totalCount.Errors));
                return Result.Fail(new ApplicationError(ErrorCodes.DATABASE_QUERY_FAILED, "Failed to retrieve payment recognition history."));
            }

            var dtos = results.Value.Select(MapToDto).ToList();

            return Result.Ok(new RecognitionHistoryResponseDto
            {
                Results = dtos,
                TotalCount = totalCount.Value,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        public async Task<Result<RecognitionStatisticsResponseDto>> GetRecognitionStatisticsAsync(string userId)
        {
            _logger.LogInformation("Getting payment recognition statistics for user {UserId}", userId);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Fail(new ValidationError("UserId", "User ID cannot be empty."));
            }

            var total = await _repository.GetTotalCountByUserIdAsync(userId);
            var completed = await _repository.GetCompletedCountByUserIdAsync(userId);
            var failed = await _repository.GetFailedCountByUserIdAsync(userId);

            if (total.IsFailed || completed.IsFailed || failed.IsFailed)
            {
                _logger.LogError("Failed to retrieve payment recognition statistics for user {UserId}: {Errors}", userId, total.Errors.Concat(completed.Errors).Concat(failed.Errors));
                return Result.Fail(new ApplicationError(ErrorCodes.DATABASE_QUERY_FAILED, "Failed to retrieve recognition statistics."));
            }

            return Result.Ok(new RecognitionStatisticsResponseDto
            {
                UserId = userId,
                TotalTasks = total.Value,
                CompletedTasks = completed.Value,
                FailedTasks = failed.Value
            });
        }

        public async Task<Result<RecognitionResponseDto>> RetryRecognitionTaskAsync(Guid taskId)
        {
            _logger.LogInformation("Retrying payment recognition task {TaskId}", taskId);

            var result = await _repository.GetByIdAsync(taskId);
            if (result.IsFailed)
            {
                if (result.Errors.Any(e => e is ApplicationError ae && ae.ErrorCode == ErrorCodes.SYSTEM_NOT_FOUND))
                {
                    return Result.Fail(new ApplicationError(ErrorCodes.DONUT_TASK_NOT_FOUND, "Payment recognition task not found.", 404));
                }
                _logger.LogError("Failed to retrieve recognition task {TaskId} for retry: {Errors}", taskId, result.Errors);
                return Result.Fail(new ApplicationError(ErrorCodes.DATABASE_QUERY_FAILED, "Failed to retrieve recognition task for retry."));
            }

            var taskToRetry = result.Value;
            if (taskToRetry.Status != DonutPaymentService.API.Domain.ValueObjects.RecognitionStatus.Failed)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.SYSTEM_BAD_REQUEST, "Only failed tasks can be retried.", 400));
            }

            taskToRetry.ClearError();
            taskToRetry.MarkAsProcessing(); // Mark as processing to be picked up by background worker

            var updateResult = await _repository.UpdateAsync(taskToRetry);
            if (updateResult.IsFailed)
            {
                _logger.LogError("Failed to update recognition task {TaskId} for retry: {Errors}", taskId, updateResult.Errors);
                return Result.Fail(new ApplicationError(ErrorCodes.DATABASE_UPDATE_FAILED, "Failed to update recognition task for retry."));
            }

            return Result.Ok(MapToDto(taskToRetry));
        }

        public async Task<Result> DeleteRecognitionResultAsync(Guid taskId, string userId)
        {
            _logger.LogInformation("Deleting payment recognition result {TaskId} for user {UserId}", taskId, userId);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Result.Fail(new ValidationError("UserId", "User ID cannot be empty."));
            }

            var existingTaskResult = await _repository.GetByIdAsync(taskId);
            if (existingTaskResult.IsFailed)
            {
                if (existingTaskResult.Errors.Any(e => e is ApplicationError ae && ae.ErrorCode == ErrorCodes.SYSTEM_NOT_FOUND))
                {
                    return Result.Fail(new ApplicationError(ErrorCodes.DONUT_TASK_NOT_FOUND, "Payment recognition task not found.", 404));
                }
                _logger.LogError("Failed to retrieve recognition task {TaskId} for deletion: {Errors}", taskId, existingTaskResult.Errors);
                return Result.Fail(new ApplicationError(ErrorCodes.DATABASE_QUERY_FAILED, "Failed to retrieve recognition task for deletion."));
            }

            if (existingTaskResult.Value.UserId != userId)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.SYSTEM_FORBIDDEN, "You do not have permission to delete this task.", 403));
            }

            var deleteResult = await _repository.DeleteAsync(taskId);
            if (deleteResult.IsFailed)
            {
                _logger.LogError("Failed to delete recognition task {TaskId}: {Errors}", taskId, deleteResult.Errors);
                return Result.Fail(new ApplicationError(ErrorCodes.DATABASE_DELETE_FAILED, "Failed to delete recognition task."));
            }

            return Result.Ok();
        }

        public async Task<Result<int>> ProcessPendingTasksAsync(int batchSize)
        {
            _logger.LogInformation("Processing {BatchSize} pending payment recognition tasks.", batchSize);

            var pendingTasksResult = await _repository.GetPendingTasksAsync(batchSize);
            if (pendingTasksResult.IsFailed)
            {
                _logger.LogError("Failed to get pending tasks: {Errors}", pendingTasksResult.Errors);
                return Result.Fail(new ApplicationError(ErrorCodes.DATABASE_QUERY_FAILED, "Failed to retrieve pending tasks."));
            }

            var pendingTasks = pendingTasksResult.Value;
            if (!pendingTasks.Any())
            {
                _logger.LogInformation("No pending payment recognition tasks to process.");
                return Result.Ok(0);
            }

            int processedCount = 0;
            foreach (var task in pendingTasks)
            {
                try
                {
                    task.MarkAsProcessing();
                    await _repository.UpdateAsync(task); // Update status to processing

                    var recognitionResult = await _donutService.RecognizePaymentAsync(task.ImageBase64);

                    if (recognitionResult.IsSuccess)
                    {
                        task.MarkAsCompleted(recognitionResult.Value);
                        _logger.LogInformation("Payment recognition task {TaskId} completed successfully.", task.Id);
                    }
                    else
                    {
                        task.MarkAsFailed(recognitionResult.Errors.FirstOrDefault()?.Message ?? "Unknown recognition error.");
                        _logger.LogWarning("Payment recognition task {TaskId} failed: {Errors}", task.Id, recognitionResult.Errors);
                    }

                    await _repository.UpdateAsync(task); // Update with final status and data
                    processedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred while processing task {TaskId}", task.Id);
                    task.MarkAsFailed($"Unexpected error: {ex.Message}");
                    await _repository.UpdateAsync(task); // Update with failed status
                }
            }

            _logger.LogInformation("Finished processing {ProcessedCount} payment recognition tasks.", processedCount);
            return Result.Ok(processedCount);
        }

        private static RecognitionResponseDto MapToDto(PaymentRecognitionResult entity)
        {
            return new RecognitionResponseDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                ImageBase64 = entity.ImageBase64,
                Status = entity.Status.ToString(),
                RecognizedData = entity.RecognizedData,
                CreatedAt = entity.CreatedAt,
                ProcessedAt = entity.ProcessedAt,
                ErrorMessage = entity.ErrorMessage,
                RetryCount = entity.RetryCount
            };
        }
    }
}


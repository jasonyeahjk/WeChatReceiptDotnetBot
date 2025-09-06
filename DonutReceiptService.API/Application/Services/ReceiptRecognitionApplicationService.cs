using DonutReceiptService.API.Application.DTOs;
using DonutReceiptService.API.Application.Interfaces;
using DonutReceiptService.API.Domain.Entities;
using DonutReceiptService.API.Domain.Repositories;
using DonutReceiptService.API.Domain.Services;
using DonutReceiptService.API.Domain.ValueObjects;
using DonutReceiptService.API.Common.Errors;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.Text;

namespace DonutReceiptService.API.Application.Services
{
    /// <summary>
    /// 收据识别应用服务实现
    /// </summary>
    public class ReceiptRecognitionApplicationService : IReceiptRecognitionApplicationService
    {
        private readonly IReceiptRecognitionRepository _repository;
        private readonly IDonutRecognitionService _recognitionService;
        private readonly ILogger<ReceiptRecognitionApplicationService> _logger;

        public ReceiptRecognitionApplicationService(
            IReceiptRecognitionRepository repository,
            IDonutRecognitionService recognitionService,
            ILogger<ReceiptRecognitionApplicationService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _recognitionService = recognitionService ?? throw new ArgumentNullException(nameof(recognitionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result<RecognitionResponseDto>> SubmitRecognitionTaskAsync(
            RecognitionRequestDto request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Submitting recognition task for user: {UserId}, file: {FileName}", 
                    request.UserId, request.FileName);

                // 验证和解码图像数据
                var imageDataResult = DecodeBase64Image(request.ImageData);
                if (imageDataResult.IsFailed)
                {
                    return Result.Fail(imageDataResult.Errors);
                }

                var imageData = imageDataResult.Value;

                // 验证图像格式
                var formatValidationResult = _recognitionService.ValidateImageFormat(imageData, request.FileName);
                if (formatValidationResult.IsFailed)
                {
                    return Result.Fail(formatValidationResult.Errors);
                }

                // 创建识别任务
                var recognitionResult = new ReceiptRecognitionResult(request.UserId, request.FileName);

                // 保存到仓储
                var saveResult = await _repository.AddAsync(recognitionResult, cancellationToken);
                if (saveResult.IsFailed)
                {
                    return Result.Fail(saveResult.Errors);
                }

                // 异步处理识别任务
                _ = Task.Run(async () => await ProcessRecognitionTaskAsync(
                    recognitionResult.Id, imageData, request.Options), cancellationToken);

                var responseDto = MapToResponseDto(recognitionResult);
                
                _logger.LogInformation("Recognition task submitted successfully: {TaskId}", recognitionResult.Id);
                return Result.Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting recognition task for user: {UserId}", request.UserId);
                return Result.Fail(new ApplicationError(
                    ErrorCodes.DONUT_RECOGNITION_FAILED, 
                    "提交识别任务失败", 
                    500));
            }
        }

        public async Task<Result<RecognitionResponseDto>> GetRecognitionResultAsync(
            Guid taskId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting recognition result for task: {TaskId}", taskId);

                var result = await _repository.GetByIdAsync(taskId, cancellationToken);
                if (result.IsFailed)
                {
                    return Result.Fail(result.Errors);
                }

                if (result.Value == null)
                {
                    return Result.Fail(new ApplicationError(
                        ErrorCodes.DONUT_TASK_NOT_FOUND, 
                        "识别任务不存在", 
                        404));
                }

                var responseDto = MapToResponseDto(result.Value);
                return Result.Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recognition result for task: {TaskId}", taskId);
                return Result.Fail(new ApplicationError(
                    ErrorCodes.SYSTEM_INTERNAL_ERROR, 
                    "获取识别结果失败", 
                    500));
            }
        }

        public async Task<Result<RecognitionHistoryResponseDto>> GetRecognitionHistoryAsync(
            string userId, 
            int pageNumber = 1, 
            int pageSize = 10, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting recognition history for user: {UserId}, page: {PageNumber}, size: {PageSize}", 
                    userId, pageNumber, pageSize);

                var result = await _repository.GetByUserIdAsync(userId, pageNumber, pageSize, cancellationToken);
                if (result.IsFailed)
                {
                    return Result.Fail(result.Errors);
                }

                var results = result.Value.Select(MapToResponseDto).ToList();
                var totalCount = results.Count; // 简化实现，实际应该从仓储获取总数

                var historyResponse = new RecognitionHistoryResponseDto
                {
                    Results = results,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return Result.Ok(historyResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recognition history for user: {UserId}", userId);
                return Result.Fail(new ApplicationError(
                    ErrorCodes.SYSTEM_INTERNAL_ERROR, 
                    "获取识别历史失败", 
                    500));
            }
        }

        public async Task<Result<RecognitionStatisticsResponseDto>> GetRecognitionStatisticsAsync(
            string userId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting recognition statistics for user: {UserId}", userId);

                var result = await _repository.GetStatisticsAsync(userId, cancellationToken);
                if (result.IsFailed)
                {
                    return Result.Fail(result.Errors);
                }

                var statistics = result.Value;
                var statisticsResponse = new RecognitionStatisticsResponseDto
                {
                    TotalRecognitions = statistics.TotalRecognitions,
                    SuccessfulRecognitions = statistics.SuccessfulRecognitions,
                    FailedRecognitions = statistics.FailedRecognitions,
                    PendingRecognitions = statistics.PendingRecognitions,
                    SuccessRate = statistics.TotalRecognitions > 0 
                        ? (double)statistics.SuccessfulRecognitions / statistics.TotalRecognitions * 100 
                        : 0,
                    AverageProcessingTimeSeconds = statistics.AverageProcessingTimeSeconds,
                    AverageConfidenceScore = statistics.AverageConfidenceScore,
                    LastRecognitionDate = statistics.LastRecognitionDate
                };

                return Result.Ok(statisticsResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recognition statistics for user: {UserId}", userId);
                return Result.Fail(new ApplicationError(
                    ErrorCodes.SYSTEM_INTERNAL_ERROR, 
                    "获取识别统计失败", 
                    500));
            }
        }

        public async Task<Result<RecognitionResponseDto>> RetryRecognitionTaskAsync(
            Guid taskId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Retrying recognition task: {TaskId}", taskId);

                var result = await _repository.GetByIdAsync(taskId, cancellationToken);
                if (result.IsFailed)
                {
                    return Result.Fail(result.Errors);
                }

                if (result.Value == null)
                {
                    return Result.Fail(new ApplicationError(
                        ErrorCodes.DONUT_TASK_NOT_FOUND, 
                        "识别任务不存在", 
                        404));
                }

                var recognitionResult = result.Value;
                recognitionResult.Retry();

                var updateResult = await _repository.UpdateAsync(recognitionResult, cancellationToken);
                if (updateResult.IsFailed)
                {
                    return Result.Fail(updateResult.Errors);
                }

                // 重新提交识别任务到处理队列
                var imageDataResult = DecodeBase64Image(recognitionResult.OriginalImagePath); // Assuming the path is the base64 string for simplicity
                if (imageDataResult.IsFailed)
                {
                    recognitionResult.MarkAsFailed("无法解码图像数据以重试任务");
                    await _repository.UpdateAsync(recognitionResult, cancellationToken);
                    return Result.Fail(imageDataResult.Errors);
                }

                _ = Task.Run(async () => await ProcessRecognitionTaskAsync(
                    recognitionResult.Id, imageDataResult.Value, null), cancellationToken);

                var responseDto = MapToResponseDto(recognitionResult);
                return Result.Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying recognition task: {TaskId}", taskId);
                return Result.Fail(new ApplicationError(
                    ErrorCodes.DONUT_RECOGNITION_FAILED, 
                    "重试识别任务失败", 
                    500));
            }
        }

        public async Task<Result> DeleteRecognitionResultAsync(
            Guid taskId, 
            string userId, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Deleting recognition result: {TaskId} for user: {UserId}", taskId, userId);

                // 验证任务所有权
                var result = await _repository.GetByIdAsync(taskId, cancellationToken);
                if (result.IsFailed)
                {
                    return Result.Fail(result.Errors);
                }

                if (result.Value == null)
                {
                    return Result.Fail(new ApplicationError(
                        ErrorCodes.DONUT_TASK_NOT_FOUND, 
                        "识别任务不存在", 
                        404));
                }

                if (result.Value.UserId != userId)
                {
                    return Result.Fail(new ApplicationError(
                        ErrorCodes.SYSTEM_FORBIDDEN, 
                        "无权限删除此识别任务", 
                        403));
                }

                var deleteResult = await _repository.DeleteAsync(taskId, cancellationToken);
                if (deleteResult.IsFailed)
                {
                    return Result.Fail(deleteResult.Errors);
                }

                _logger.LogInformation("Recognition result deleted successfully: {TaskId}", taskId);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recognition result: {TaskId}", taskId);
                return Result.Fail(new ApplicationError(
                    ErrorCodes.SYSTEM_INTERNAL_ERROR, 
                    "删除识别结果失败", 
                    500));
            }
        }

        public async Task<Result<int>> ProcessPendingTasksAsync(
            int batchSize = 10, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Processing pending recognition tasks, batch size: {BatchSize}", batchSize);

                var result = await _repository.GetPendingTasksAsync(batchSize, cancellationToken);
                if (result.IsFailed)
                {
                    return Result.Fail(result.Errors);
                }

                var pendingTasks = result.Value.ToList();
                var processedCount = 0;

                foreach (var task in pendingTasks)
                {
                    try
                    {
                        var imageDataResult = DecodeBase64Image(task.OriginalImagePath);
                        if (imageDataResult.IsFailed)
                        {
                            _logger.LogError("Failed to decode image data for task {TaskId}: {Errors}", task.Id, imageDataResult.Errors);
                            task.MarkAsFailed("无法解码图像数据");
                            await _repository.UpdateAsync(task);
                            continue;
                        }
                        await ProcessRecognitionTaskAsync(task.Id, imageDataResult.Value, null);
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing recognition task: {TaskId}", task.Id);
                    }
                }

                _logger.LogInformation("Processed {ProcessedCount} out of {TotalCount} pending tasks", 
                    processedCount, pendingTasks.Count);

                return Result.Ok(processedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending recognition tasks");
                return Result.Fail(new ApplicationError(
                    ErrorCodes.SYSTEM_INTERNAL_ERROR, 
                    "处理等待任务失败", 
                    500));
            }
        }

        private async Task ProcessRecognitionTaskAsync(
            Guid taskId, 
            byte[] imageData, 
            RecognitionOptionsDto? options)
        {
            try
            {
                _logger.LogInformation("Processing recognition task: {TaskId}", taskId);

                // 获取任务
                var taskResult = await _repository.GetByIdAsync(taskId);
                if (taskResult.IsFailed || taskResult.Value == null)
                {
                    _logger.LogError("Failed to get recognition task: {TaskId}", taskId);
                    return;
                }

                var task = taskResult.Value;
                task.StartProcessing();

                // 更新状态
                await _repository.UpdateAsync(task);

                // 执行识别
                var recognitionResult = await _recognitionService.RecognizeReceiptAsync(
                    imageData, task.OriginalImagePath);

                if (recognitionResult.IsSuccess)
                {
                    task.CompleteSuccessfully(
                        recognitionResult.Value.ExtractedData, 
                        recognitionResult.Value.ConfidenceScore);
                }
                else
                {
                    var errorMessage = string.Join("; ", recognitionResult.Errors.Select(e => e.Message));
                    task.MarkAsFailed(errorMessage);
                }

                // 更新最终状态
                await _repository.UpdateAsync(task);

                _logger.LogInformation("Recognition task processed: {TaskId}, Status: {Status}", 
                    taskId, task.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing recognition task: {TaskId}", taskId);

                try
                {
                    var taskResult = await _repository.GetByIdAsync(taskId);
                    if (taskResult.IsSuccess && taskResult.Value != null)
                    {
                        taskResult.Value.MarkAsFailed($"处理异常: {ex.Message}");
                        await _repository.UpdateAsync(taskResult.Value);
                    }
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "Error updating failed recognition task: {TaskId}", taskId);
                }
            }
        }

        private Result<byte[]> DecodeBase64Image(string base64Data)
        {
            try
            {
                // 移除 data URL 前缀（如果存在）
                var base64String = base64Data;
                if (base64Data.Contains(','))
                {
                    base64String = base64Data.Split(',')[1];
                }

                var imageData = Convert.FromBase64String(base64String);
                return Result.Ok(imageData);
            }
            catch (Exception ex)
            {
                return Result.Fail(new ApplicationError(
                    ErrorCodes.DONUT_INVALID_IMAGE, 
                    $"无效的Base64图像数据: {ex.Message}", 
                    400));
            }
        }

        private RecognitionResponseDto MapToResponseDto(ReceiptRecognitionResult entity)
        {
            return new RecognitionResponseDto
            {
                TaskId = entity.Id,
                Status = entity.Status.ToString(),
                ExtractedData = entity.ExtractedData != null ? MapToReceiptDataDto(entity.ExtractedData) : null,
                ConfidenceScore = entity.ConfidenceScore,
                ProcessingTimeMs = entity.ProcessingTime?.TotalMilliseconds != null 
                    ? (long)entity.ProcessingTime.Value.TotalMilliseconds 
                    : null,
                ErrorMessage = entity.ErrorMessage,
                CreatedAt = entity.CreatedAt,
                CompletedAt = entity.CompletedAt
            };
        }

        private ReceiptDataDto MapToReceiptDataDto(ReceiptData receiptData)
        {
            return new ReceiptDataDto
            {
                MerchantName = receiptData.MerchantName,
                MerchantAddress = receiptData.MerchantAddress,
                TransactionDate = receiptData.TransactionDate,
                TotalAmount = receiptData.TotalAmount,
                Currency = receiptData.Currency,
                ReceiptNumber = receiptData.ReceiptNumber,
                PaymentMethod = receiptData.PaymentMethod,
                TaxAmount = receiptData.TaxAmount,
                TipAmount = receiptData.TipAmount,
                Items = receiptData.Items.Select(MapToReceiptItemDto).ToList(),
                AdditionalFields = receiptData.AdditionalFields
            };
        }

        private ReceiptItemDto MapToReceiptItemDto(ReceiptItem item)
        {
            return new ReceiptItemDto
            {
                Name = item.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                Category = item.Category,
                Description = item.Description
            };
        }
    }
}


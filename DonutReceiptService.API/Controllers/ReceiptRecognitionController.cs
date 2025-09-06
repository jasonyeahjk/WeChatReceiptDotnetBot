using Microsoft.AspNetCore.Mvc;
using FluentResults;
using DonutReceiptService.API.Application.DTOs;
using DonutReceiptService.API.Application.Interfaces;
using DonutReceiptService.API.Common.Errors;
using DonutReceiptService.API.Common.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace DonutReceiptService.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    // [Authorize] // 根据需要添加认证
    public class ReceiptRecognitionController : ControllerBase
    {
        private readonly IReceiptRecognitionApplicationService _applicationService;
        private readonly ILogger<ReceiptRecognitionController> _logger;

        public ReceiptRecognitionController(
            IReceiptRecognitionApplicationService applicationService,
            ILogger<ReceiptRecognitionController> logger)
        {
            _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 提交收据识别任务
        /// </summary>
        /// <param name="request">识别请求数据</param>
        /// <returns>识别任务响应</returns>
        [HttpPost("submit")]
        [ProducesResponseType(typeof(RecognitionResponseDto), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecognitionResponseDto>> SubmitRecognitionTask(
            [FromBody] RecognitionRequestDto request)
        {
            _logger.LogInformation("Received recognition request for user: {UserId}", request.UserId);
            var result = await _applicationService.SubmitRecognitionTaskAsync(request);
            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// 获取识别任务结果
        /// </summary>
        /// <param name="taskId">识别任务ID</param>
        /// <returns>识别任务响应</returns>
        [HttpGet("{taskId}")]
        [ProducesResponseType(typeof(RecognitionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecognitionResponseDto>> GetRecognitionResult(
            Guid taskId)
        {
            _logger.LogInformation("Getting recognition result for task: {TaskId}", taskId);
            var result = await _applicationService.GetRecognitionResultAsync(taskId);
            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// 获取用户识别历史
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="pageNumber">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>识别历史响应</returns>
        [HttpGet("history/{userId}")]
        [ProducesResponseType(typeof(RecognitionHistoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecognitionHistoryResponseDto>> GetRecognitionHistory(
            string userId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting recognition history for user: {UserId}, page: {PageNumber}, size: {PageSize}", 
                userId, pageNumber, pageSize);
            var result = await _applicationService.GetRecognitionHistoryAsync(userId, pageNumber, pageSize);
            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// 获取用户识别统计信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>识别统计响应</returns>
        [HttpGet("statistics/{userId}")]
        [ProducesResponseType(typeof(RecognitionStatisticsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecognitionStatisticsResponseDto>> GetRecognitionStatistics(
            string userId)
        {
            _logger.LogInformation("Getting recognition statistics for user: {UserId}", userId);
            var result = await _applicationService.GetRecognitionStatisticsAsync(userId);
            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// 重试失败的识别任务
        /// </summary>
        /// <param name="taskId">识别任务ID</param>
        /// <returns>识别任务响应</returns>
        [HttpPost("retry/{taskId}")]
        [ProducesResponseType(typeof(RecognitionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RecognitionResponseDto>> RetryRecognitionTask(
            Guid taskId)
        {
            _logger.LogInformation("Retrying recognition task: {TaskId}", taskId);
            var result = await _applicationService.RetryRecognitionTaskAsync(taskId);
            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// 删除识别结果
        /// </summary>
        /// <param name="taskId">识别任务ID</param>
        /// <param name="userId">用户ID (用于权限验证)</param>
        /// <returns>无内容响应</returns>
        [HttpDelete("{taskId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteRecognitionResult(
            Guid taskId,
            [FromQuery] string userId) // 实际应用中，userId应从认证信息中获取
        {
            _logger.LogInformation("Deleting recognition result: {TaskId} for user: {UserId}", taskId, userId);
            var result = await _applicationService.DeleteRecognitionResultAsync(taskId, userId);
            return result.ToActionResult(HttpContext);
        }

        /// <summary>
        /// 批量处理等待中的识别任务 (内部调用或后台任务)
        /// </summary>
        /// <param name="batchSize">批量大小</param>
        /// <returns>处理的任务数量</returns>
        [HttpPost("process-pending")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> ProcessPendingTasks(
            [FromQuery] int batchSize = 10)
        {
            _logger.LogInformation("Processing pending tasks with batch size: {BatchSize}", batchSize);
            var result = await _applicationService.ProcessPendingTasksAsync(batchSize);
            return result.ToActionResult(HttpContext);
        }
    }
}


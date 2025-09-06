using FluentResults;
using Microsoft.AspNetCore.Mvc;
using WeChatReceiptBot.API.Common;
using Microsoft.AspNetCore.Http;

namespace WeChatReceiptBot.API.Extensions
{
    /// <summary>
    /// FluentResults 扩展方法
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// 将 Result 转换为 ActionResult
        /// </summary>
        public static ActionResult ToActionResult(this Result result, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new OkResult();
            }

            return CreateProblemResult(result.Errors, httpContext);
        }

        /// <summary>
        /// 将 Result<T> 转换为 ActionResult<T>
        /// </summary>
        public static ActionResult<T> ToActionResult<T>(this Result<T> result, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }

            return CreateProblemResult(result.Errors, httpContext);
        }

        /// <summary>
        /// 将 Result 转换为 ActionResult，成功时返回 201 Created
        /// </summary>
        public static ActionResult ToCreatedResult(this Result result, string location, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new CreatedResult(location, null);
            }

            return CreateProblemResult(result.Errors, httpContext);
        }

        /// <summary>
        /// 将 Result<T> 转换为 ActionResult<T>，成功时返回 201 Created
        /// </summary>
        public static ActionResult<T> ToCreatedResult<T>(this Result<T> result, string location, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new CreatedResult(location, result.Value);
            }

            return CreateProblemResult(result.Errors, httpContext);
        }

        /// <summary>
        /// 将 Result 转换为 ActionResult，成功时返回 204 No Content
        /// </summary>
        public static ActionResult ToNoContentResult(this Result result, HttpContext httpContext)
        {
            if (result.IsSuccess)
            {
                return new NoContentResult();
            }

            return CreateProblemResult(result.Errors, httpContext);
        }

        /// <summary>
        /// 创建 Problem Details 响应
        /// </summary>
        private static ActionResult CreateProblemResult(IReadOnlyList<IError> errors, HttpContext httpContext)
        {
            var firstError = errors.FirstOrDefault();
            
            if (firstError is ApplicationError appError)
            {
                var problemDetails = new ProblemDetails
                {
                    Title = GetTitleForStatusCode(appError.StatusCode),
                    Status = appError.StatusCode,
                    Detail = appError.Message,
                    Type = $"https://httpstatuses.com/{appError.StatusCode}",
                    Instance = httpContext.Request.Path
                };

                // 添加错误代码
                problemDetails.Extensions["errorCode"] = appError.ErrorCode;

                // 添加自定义扩展信息
                if (appError.Extensions != null)
                {
                    foreach (var extension in appError.Extensions)
                    {
                        problemDetails.Extensions[extension.Key] = extension.Value;
                    }
                }

                // 处理验证错误
                if (appError is ValidationError validationError)
                {
                    problemDetails.Extensions["errors"] = validationError.ValidationErrors;
                }

                return new ObjectResult(problemDetails)
                {
                    StatusCode = appError.StatusCode
                };
            }

            // 处理一般错误
            var generalProblemDetails = new ProblemDetails
            {
                Title = "An error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Detail = firstError?.Message ?? "An unexpected error occurred",
                Type = "https://httpstatuses.com/500",
                Instance = httpContext.Request.Path
            };

            generalProblemDetails.Extensions["errorCode"] = ErrorCodes.SYSTEM_INTERNAL_ERROR;

            return new ObjectResult(generalProblemDetails)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        /// <summary>
        /// 根据状态码获取标题
        /// </summary>
        private static string GetTitleForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                403 => "Forbidden",
                404 => "Not Found",
                409 => "Conflict",
                422 => "Unprocessable Entity",
                500 => "Internal Server Error",
                503 => "Service Unavailable",
                _ => "An error occurred"
            };
        }
    }

    /// <summary>
    /// ControllerBase 扩展方法
    /// </summary>
    public static class ControllerBaseExtensions
    {
        /// <summary>
        /// 创建成功响应
        /// </summary>
        public static ActionResult<T> Success<T>(this ControllerBase controller, T data)
        {
            return controller.Ok(data);
        }

        /// <summary>
        /// 创建创建成功响应
        /// </summary>
        public static ActionResult<T> Created<T>(this ControllerBase controller, string location, T data)
        {
            return controller.Created(location, data);
        }

        /// <summary>
        /// 创建无内容响应
        /// </summary>
        public static ActionResult NoContent(this ControllerBase controller)
        {
            return controller.NoContent();
        }

        /// <summary>
        /// 创建错误响应
        /// </summary>
        public static ActionResult Error(this ControllerBase controller, string errorCode, int statusCode = 400, string? customMessage = null)
        {
            var error = new ApplicationError(errorCode, customMessage ?? ErrorMessages.GetMessage(errorCode), statusCode);
            var result = Result.Fail(error);
            return result.ToActionResult(controller.HttpContext);
        }

        /// <summary>
        /// 创建验证错误响应
        /// </summary>
        public static ActionResult ValidationError(this ControllerBase controller, Dictionary<string, string[]> validationErrors)
        {
            var error = new ValidationError(validationErrors);
            var result = Result.Fail(error);
            return result.ToActionResult(controller.HttpContext);
        }

        /// <summary>
        /// 创建验证错误响应
        /// </summary>
        public static ActionResult ValidationError(this ControllerBase controller, string field, string errorMessage)
        {
            var error = new ValidationError(field, errorMessage);
            var result = Result.Fail(error);
            return result.ToActionResult(controller.HttpContext);
        }
    }
}


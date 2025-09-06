using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Web3Service.API.Common.Errors;

namespace Web3Service.API.Common.Extensions;

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
    /// 将 Result&lt;T&gt; 转换为 ActionResult&lt;T&gt;
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
    /// 创建 Problem Details 响应
    /// </summary>
    private static ObjectResult CreateProblemResult(IReadOnlyList<IError> errors, HttpContext httpContext)
    {
        var firstError = errors.FirstOrDefault();
        
        if (firstError is ValidationError validationError)
        {
            return CreateValidationProblemResult(errors.OfType<ValidationError>().ToList(), httpContext);
        }
        
        if (firstError is ApplicationError applicationError)
        {
            return CreateApplicationErrorResult(applicationError, httpContext);
        }

        // 默认错误处理
        var problemDetails = new ProblemDetails
        {
            Title = "Error",
            Detail = firstError?.Message ?? "An error occurred",
            Status = 500,
            Type = "https://httpstatuses.com/500",
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["errorCode"] = "UNKNOWN_ERROR";
        problemDetails.Extensions["errors"] = errors.Select(e => e.Message).ToArray();

        return new ObjectResult(problemDetails)
        {
            StatusCode = 500
        };
    }

    /// <summary>
    /// 创建验证错误响应
    /// </summary>
    private static ObjectResult CreateValidationProblemResult(List<ValidationError> validationErrors, HttpContext httpContext)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Title = "Validation Error",
            Detail = "One or more validation errors occurred",
            Status = 422,
            Type = "https://httpstatuses.com/422",
            Instance = httpContext.Request.Path
        };

        // 按字段分组验证错误
        var errorGroups = validationErrors.GroupBy(e => e.FieldName);
        foreach (var group in errorGroups)
        {
            var fieldErrors = group.Select(e => e.Message).ToArray();
            problemDetails.Errors[group.Key] = fieldErrors;
        }

        // 添加错误编码
        var firstError = validationErrors.FirstOrDefault();
        problemDetails.Extensions["errorCode"] = firstError?.ErrorCode ?? ErrorCodes.SYSTEM_VALIDATION_FAILED;

        return new ObjectResult(problemDetails)
        {
            StatusCode = 422
        };
    }

    /// <summary>
    /// 创建应用错误响应
    /// </summary>
    private static ObjectResult CreateApplicationErrorResult(ApplicationError applicationError, HttpContext httpContext)
    {
        var problemDetails = new ProblemDetails
        {
            Title = GetTitleForStatusCode(applicationError.StatusCode),
            Detail = applicationError.Message,
            Status = applicationError.StatusCode,
            Type = $"https://httpstatuses.com/{applicationError.StatusCode}",
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["errorCode"] = applicationError.ErrorCode;

        return new ObjectResult(problemDetails)
        {
            StatusCode = applicationError.StatusCode
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
            _ => "Error"
        };
    }
}


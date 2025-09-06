using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Web3Service.API.Common.Errors;

namespace Web3Service.API.Common.Extensions;

/// <summary>
/// ControllerBase 扩展方法
/// </summary>
public static class ControllerBaseExtensions
{
    /// <summary>
    /// 返回成功结果
    /// </summary>
    public static ActionResult Success(this ControllerBase controller)
    {
        return Result.Ok().ToActionResult(controller.HttpContext);
    }

    /// <summary>
    /// 返回成功结果（带数据）
    /// </summary>
    public static ActionResult<T> Success<T>(this ControllerBase controller, T data)
    {
        return Result.Ok(data).ToActionResult(controller.HttpContext);
    }

    /// <summary>
    /// 返回错误结果
    /// </summary>
    public static ActionResult Error(this ControllerBase controller, string errorCode, int statusCode = 400, string? customMessage = null)
    {
        var message = customMessage ?? ErrorMessages.GetMessage(errorCode);
        var error = new ApplicationError(errorCode, message, statusCode);
        return Result.Fail(error).ToActionResult(controller.HttpContext);
    }

    /// <summary>
    /// 返回错误结果（泛型）
    /// </summary>
    public static ActionResult<T> Error<T>(this ControllerBase controller, string errorCode, int statusCode = 400, string? customMessage = null)
    {
        var message = customMessage ?? ErrorMessages.GetMessage(errorCode);
        var error = new ApplicationError(errorCode, message, statusCode);
        return Result.Fail<T>(error).ToActionResult(controller.HttpContext);
    }

    /// <summary>
    /// 返回验证错误结果
    /// </summary>
    public static ActionResult ValidationError(this ControllerBase controller, string fieldName, string message)
    {
        var error = new ValidationError(fieldName, message);
        return Result.Fail(error).ToActionResult(controller.HttpContext);
    }

    /// <summary>
    /// 返回验证错误结果（泛型）
    /// </summary>
    public static ActionResult<T> ValidationError<T>(this ControllerBase controller, string fieldName, string message)
    {
        var error = new ValidationError(fieldName, message);
        return Result.Fail<T>(error).ToActionResult(controller.HttpContext);
    }

    /// <summary>
    /// 返回多个验证错误结果
    /// </summary>
    public static ActionResult ValidationErrors(this ControllerBase controller, Dictionary<string, string> fieldErrors)
    {
        var errors = fieldErrors.Select(kvp => new ValidationError(kvp.Key, kvp.Value)).Cast<IError>().ToList();
        return Result.Fail(errors).ToActionResult(controller.HttpContext);
    }

    /// <summary>
    /// 返回多个验证错误结果（泛型）
    /// </summary>
    public static ActionResult<T> ValidationErrors<T>(this ControllerBase controller, Dictionary<string, string> fieldErrors)
    {
        var errors = fieldErrors.Select(kvp => new ValidationError(kvp.Key, kvp.Value)).Cast<IError>().ToList();
        return Result.Fail<T>(errors).ToActionResult(controller.HttpContext);
    }

    /// <summary>
    /// 返回未找到错误
    /// </summary>
    public static ActionResult NotFound(this ControllerBase controller, string errorCode, string? customMessage = null)
    {
        return controller.Error(errorCode, 404, customMessage);
    }

    /// <summary>
    /// 返回未找到错误（泛型）
    /// </summary>
    public static ActionResult<T> NotFound<T>(this ControllerBase controller, string errorCode, string? customMessage = null)
    {
        return controller.Error<T>(errorCode, 404, customMessage);
    }

    /// <summary>
    /// 返回冲突错误
    /// </summary>
    public static ActionResult Conflict(this ControllerBase controller, string errorCode, string? customMessage = null)
    {
        return controller.Error(errorCode, 409, customMessage);
    }

    /// <summary>
    /// 返回冲突错误（泛型）
    /// </summary>
    public static ActionResult<T> Conflict<T>(this ControllerBase controller, string errorCode, string? customMessage = null)
    {
        return controller.Error<T>(errorCode, 409, customMessage);
    }

    /// <summary>
    /// 返回未授权错误
    /// </summary>
    public static ActionResult Unauthorized(this ControllerBase controller, string? customMessage = null)
    {
        return controller.Error(ErrorCodes.SYSTEM_UNAUTHORIZED, 401, customMessage);
    }

    /// <summary>
    /// 返回禁止访问错误
    /// </summary>
    public static ActionResult Forbidden(this ControllerBase controller, string? customMessage = null)
    {
        return controller.Error(ErrorCodes.SYSTEM_FORBIDDEN, 403, customMessage);
    }

    /// <summary>
    /// 返回内部服务器错误
    /// </summary>
    public static ActionResult InternalServerError(this ControllerBase controller, string? customMessage = null)
    {
        return controller.Error(ErrorCodes.SYSTEM_INTERNAL_ERROR, 500, customMessage);
    }
}


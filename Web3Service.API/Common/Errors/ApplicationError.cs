using FluentResults;

namespace Web3Service.API.Common.Errors
{

/// <summary>
/// 应用程序错误类
/// </summary>
public class ApplicationError : Error
{
    /// <summary>
    /// 错误编码
    /// </summary>
    public string ErrorCode { get; }
    
    /// <summary>
    /// HTTP状态码
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="errorCode">错误编码</param>
    /// <param name="message">错误消息</param>
    /// <param name="statusCode">HTTP状态码</param>
    public ApplicationError(string errorCode, string message, int statusCode = 400) 
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        
        // 添加元数据
        WithMetadata("ErrorCode", errorCode);
        WithMetadata("StatusCode", statusCode);
    }

    /// <summary>
    /// 构造函数（带内部异常）
    /// </summary>
    /// <param name="errorCode">错误编码</param>
    /// <param name="message">错误消息</param>
    /// <param name="causedBy">内部异常</param>
    /// <param name="statusCode">HTTP状态码</param>
    public ApplicationError(string errorCode, string message, Exception causedBy, int statusCode = 400) 
        : base(message, new ExceptionalError(causedBy))
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        
        // 添加元数据
        WithMetadata("ErrorCode", errorCode);
        WithMetadata("StatusCode", statusCode);
    }

    /// <summary>
    /// 创建系统内部错误
    /// </summary>
    public static ApplicationError InternalError(string message = "系统内部错误")
    {
        return new ApplicationError(ErrorCodes.SYSTEM_INTERNAL_ERROR, message, 500);
    }

    /// <summary>
    /// 创建网络错误
    /// </summary>
    public static ApplicationError NetworkError(string message = "网络连接错误")
    {
        return new ApplicationError(ErrorCodes.SYSTEM_NETWORK_ERROR, message, 503);
    }

    /// <summary>
    /// 创建未授权错误
    /// </summary>
    public static ApplicationError Unauthorized(string message = "未授权访问")
    {
        return new ApplicationError(ErrorCodes.SYSTEM_UNAUTHORIZED, message, 401);
    }

    /// <summary>
    /// 创建禁止访问错误
    /// </summary>
    public static ApplicationError Forbidden(string message = "禁止访问")
    {
        return new ApplicationError(ErrorCodes.SYSTEM_FORBIDDEN, message, 403);
    }

    /// <summary>
    /// 创建资源未找到错误
    /// </summary>
    public static ApplicationError NotFound(string errorCode, string message)
    {
        return new ApplicationError(errorCode, message, 404);
    }

    /// <summary>
    /// 创建冲突错误
    /// </summary>
    public static ApplicationError Conflict(string errorCode, string message)
    {
        return new ApplicationError(errorCode, message, 409);
    }
}

}


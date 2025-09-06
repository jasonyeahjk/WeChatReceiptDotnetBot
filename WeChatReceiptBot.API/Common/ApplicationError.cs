using FluentResults;

namespace WeChatReceiptBot.API.Common
{
    /// <summary>
    /// 应用程序自定义错误类
    /// </summary>
    public class ApplicationError : Error
    {
        public string ErrorCode { get; }
        public int StatusCode { get; }
        public Dictionary<string, object>? Extensions { get; }

        public ApplicationError(string errorCode, string message, int statusCode = 400, Dictionary<string, object>? extensions = null)
            : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Extensions = extensions;
        }

        public ApplicationError(string errorCode, int statusCode = 400, Dictionary<string, object>? extensions = null)
            : base(ErrorMessages.GetMessage(errorCode))
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
            Extensions = extensions;
        }

        /// <summary>
        /// 创建认证错误
        /// </summary>
        public static ApplicationError Unauthorized(string errorCode, string? customMessage = null)
        {
            return new ApplicationError(errorCode, customMessage ?? ErrorMessages.GetMessage(errorCode), 401);
        }

        /// <summary>
        /// 创建权限不足错误
        /// </summary>
        public static ApplicationError Forbidden(string errorCode, string? customMessage = null)
        {
            return new ApplicationError(errorCode, customMessage ?? ErrorMessages.GetMessage(errorCode), 403);
        }

        /// <summary>
        /// 创建资源未找到错误
        /// </summary>
        public static ApplicationError NotFound(string errorCode, string? customMessage = null)
        {
            return new ApplicationError(errorCode, customMessage ?? ErrorMessages.GetMessage(errorCode), 404);
        }

        /// <summary>
        /// 创建冲突错误
        /// </summary>
        public static ApplicationError Conflict(string errorCode, string? customMessage = null)
        {
            return new ApplicationError(errorCode, customMessage ?? ErrorMessages.GetMessage(errorCode), 409);
        }

        /// <summary>
        /// 创建验证错误
        /// </summary>
        public static ApplicationError ValidationError(string errorCode, string? customMessage = null, Dictionary<string, object>? extensions = null)
        {
            return new ApplicationError(errorCode, customMessage ?? ErrorMessages.GetMessage(errorCode), 422, extensions);
        }

        /// <summary>
        /// 创建服务器内部错误
        /// </summary>
        public static ApplicationError InternalError(string errorCode, string? customMessage = null)
        {
            return new ApplicationError(errorCode, customMessage ?? ErrorMessages.GetMessage(errorCode), 500);
        }

        /// <summary>
        /// 创建服务不可用错误
        /// </summary>
        public static ApplicationError ServiceUnavailable(string errorCode, string? customMessage = null)
        {
            return new ApplicationError(errorCode, customMessage ?? ErrorMessages.GetMessage(errorCode), 503);
        }
    }

    /// <summary>
    /// 验证错误类
    /// </summary>
    public class ValidationError : ApplicationError
    {
        public Dictionary<string, string[]> ValidationErrors { get; }

        public ValidationError(Dictionary<string, string[]> validationErrors)
            : base(ErrorCodes.SYSTEM_VALIDATION_FAILED, "数据验证失败", 422)
        {
            ValidationErrors = validationErrors;
        }

        public ValidationError(string field, string error)
            : this(new Dictionary<string, string[]> { { field, new[] { error } } })
        {
        }
    }
}


using FluentResults;

namespace Web3Service.API.Common.Errors;

/// <summary>
/// 验证错误类
/// </summary>
public class ValidationError : Error
{
    /// <summary>
    /// 字段名称
    /// </summary>
    public string FieldName { get; }
    
    /// <summary>
    /// 错误编码
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="fieldName">字段名称</param>
    /// <param name="message">错误消息</param>
    /// <param name="errorCode">错误编码</param>
    public ValidationError(string fieldName, string message, string? errorCode = null) 
        : base(message)
    {
        FieldName = fieldName;
        ErrorCode = errorCode ?? ErrorCodes.SYSTEM_VALIDATION_FAILED;
        
        // 添加元数据
        WithMetadata("FieldName", fieldName);
        WithMetadata("ErrorCode", ErrorCode);
        WithMetadata("StatusCode", 422); // Unprocessable Entity
    }

    /// <summary>
    /// 创建必填字段验证错误
    /// </summary>
    public static ValidationError Required(string fieldName, string? customMessage = null)
    {
        var message = customMessage ?? $"{fieldName}不能为空";
        return new ValidationError(fieldName, message);
    }

    /// <summary>
    /// 创建格式验证错误
    /// </summary>
    public static ValidationError InvalidFormat(string fieldName, string? customMessage = null)
    {
        var message = customMessage ?? $"{fieldName}格式不正确";
        return new ValidationError(fieldName, message);
    }

    /// <summary>
    /// 创建范围验证错误
    /// </summary>
    public static ValidationError OutOfRange(string fieldName, string? customMessage = null)
    {
        var message = customMessage ?? $"{fieldName}超出有效范围";
        return new ValidationError(fieldName, message);
    }

    /// <summary>
    /// 创建长度验证错误
    /// </summary>
    public static ValidationError InvalidLength(string fieldName, int? minLength = null, int? maxLength = null)
    {
        string message;
        if (minLength.HasValue && maxLength.HasValue)
        {
            message = $"{fieldName}长度必须在{minLength}-{maxLength}之间";
        }
        else if (minLength.HasValue)
        {
            message = $"{fieldName}长度不能少于{minLength}个字符";
        }
        else if (maxLength.HasValue)
        {
            message = $"{fieldName}长度不能超过{maxLength}个字符";
        }
        else
        {
            message = $"{fieldName}长度不正确";
        }
        
        return new ValidationError(fieldName, message);
    }

    /// <summary>
    /// 创建自定义验证错误
    /// </summary>
    public static ValidationError Custom(string fieldName, string message, string? errorCode = null)
    {
        return new ValidationError(fieldName, message, errorCode);
    }
}


using System.Collections.ObjectModel;
using Web3Service.API.Common.Errors;

namespace Web3Service.API.Common;

/// <summary>
/// 错误消息映射
/// </summary>
public static class ErrorMessages
{
    private static readonly Dictionary<string, string> _messages = new()
    {
        // 系统级错误
        [ErrorCodes.SYSTEM_INTERNAL_ERROR] = "系统内部错误，请稍后重试",
        [ErrorCodes.SYSTEM_NETWORK_ERROR] = "网络连接错误，请检查网络连接",
        [ErrorCodes.SYSTEM_VALIDATION_FAILED] = "数据验证失败",
        [ErrorCodes.SYSTEM_UNAUTHORIZED] = "未授权访问，请先登录",
        [ErrorCodes.SYSTEM_FORBIDDEN] = "禁止访问，权限不足",

        // Web3账户相关错误
        [ErrorCodes.WEB3_ACCOUNT_NOT_FOUND] = "账户不存在",
        [ErrorCodes.WEB3_ACCOUNT_ALREADY_EXISTS] = "账户已存在",
        [ErrorCodes.WEB3_ACCOUNT_CREATION_FAILED] = "创建账户失败",
        [ErrorCodes.WEB3_ACCOUNT_IMPORT_FAILED] = "导入账户失败",
        [ErrorCodes.WEB3_ACCOUNT_UPDATE_FAILED] = "更新账户失败",
        [ErrorCodes.WEB3_ACCOUNT_QUERY_FAILED] = "查询账户失败",
        [ErrorCodes.WEB3_INVALID_ADDRESS] = "无效的区块链地址",
        [ErrorCodes.WEB3_INVALID_PRIVATE_KEY] = "无效的私钥格式",

        // 区块链交易相关错误
        [ErrorCodes.WEB3_TRANSACTION_NOT_FOUND] = "交易不存在",
        [ErrorCodes.WEB3_TRANSACTION_SEND_FAILED] = "发送交易失败",
        [ErrorCodes.WEB3_TRANSACTION_QUERY_FAILED] = "查询交易失败",
        [ErrorCodes.WEB3_TRANSACTION_INVALID_HASH] = "无效的交易哈希",
        [ErrorCodes.WEB3_INSUFFICIENT_BALANCE] = "账户余额不足",
        [ErrorCodes.WEB3_GAS_ESTIMATION_FAILED] = "Gas费用估算失败",
        [ErrorCodes.WEB3_TRANSACTION_FAILED] = "交易执行失败",

        // 智能合约相关错误
        [ErrorCodes.WEB3_CONTRACT_NOT_FOUND] = "智能合约不存在",
        [ErrorCodes.WEB3_CONTRACT_DEPLOYMENT_FAILED] = "智能合约部署失败",
        [ErrorCodes.WEB3_CONTRACT_CALL_FAILED] = "智能合约调用失败",
        [ErrorCodes.WEB3_CONTRACT_INVALID_ABI] = "无效的合约ABI",
        [ErrorCodes.WEB3_CONTRACT_INVALID_BYTECODE] = "无效的合约字节码",
        [ErrorCodes.WEB3_CONTRACT_VERIFICATION_FAILED] = "合约验证失败",
        [ErrorCodes.WEB3_CONTRACT_METHOD_NOT_FOUND] = "合约方法不存在",

        // 网络连接相关错误
        [ErrorCodes.WEB3_NETWORK_CONNECTION_FAILED] = "区块链网络连接失败",
        [ErrorCodes.WEB3_NETWORK_TIMEOUT] = "网络请求超时",
        [ErrorCodes.WEB3_NETWORK_INVALID_RESPONSE] = "网络响应格式错误",
        [ErrorCodes.WEB3_BALANCE_QUERY_FAILED] = "查询余额失败",
        [ErrorCodes.WEB3_BLOCK_QUERY_FAILED] = "查询区块信息失败",
        [ErrorCodes.WEB3_GAS_PRICE_QUERY_FAILED] = "查询Gas价格失败",

        // 数据库相关错误
        [ErrorCodes.WEB3_DATABASE_CONNECTION_FAILED] = "数据库连接失败",
        [ErrorCodes.WEB3_DATABASE_QUERY_FAILED] = "数据库查询失败",
        [ErrorCodes.WEB3_DATABASE_UPDATE_FAILED] = "数据库更新失败",
        [ErrorCodes.WEB3_DATABASE_DELETE_FAILED] = "数据库删除失败",
        [ErrorCodes.WEB3_DATABASE_CONSTRAINT_VIOLATION] = "数据库约束冲突",

        // 验证相关错误
        [ErrorCodes.WEB3_VALIDATION_INVALID_AMOUNT] = "无效的转账金额",
        [ErrorCodes.WEB3_VALIDATION_INVALID_GAS_PRICE] = "无效的Gas价格",
        [ErrorCodes.WEB3_VALIDATION_INVALID_GAS_LIMIT] = "无效的Gas限制",
        [ErrorCodes.WEB3_VALIDATION_MISSING_PRIVATE_KEY] = "缺少私钥信息",
        [ErrorCodes.WEB3_VALIDATION_INVALID_PARAMETERS] = "无效的参数"
    };

    /// <summary>
    /// 错误消息只读字典
    /// </summary>
    public static ReadOnlyDictionary<string, string> Messages => new(_messages);

    /// <summary>
    /// 获取错误消息
    /// </summary>
    /// <param name="errorCode">错误编码</param>
    /// <returns>错误消息</returns>
    public static string GetMessage(string errorCode)
    {
        return _messages.TryGetValue(errorCode, out var message) ? message : "未知错误";
    }

    /// <summary>
    /// 检查错误编码是否存在
    /// </summary>
    /// <param name="errorCode">错误编码</param>
    /// <returns>是否存在</returns>
    public static bool ContainsErrorCode(string errorCode)
    {
        return _messages.ContainsKey(errorCode);
    }
}


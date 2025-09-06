namespace Web3Service.API.Common.Errors;

/// <summary>
/// Web3服务错误编码常量
/// </summary>
public static class ErrorCodes
{
    // 系统级错误 (SYS_1xxxx)
    public const string SYSTEM_INTERNAL_ERROR = "SYS_10001";
    public const string SYSTEM_NETWORK_ERROR = "SYS_10002";
    public const string SYSTEM_VALIDATION_FAILED = "SYS_10003";
    public const string SYSTEM_UNAUTHORIZED = "SYS_10004";
    public const string SYSTEM_FORBIDDEN = "SYS_10005";

    // Web3账户相关错误 (WEB3_2xxxx)
    public const string WEB3_ACCOUNT_NOT_FOUND = "WEB3_20001";
    public const string WEB3_ACCOUNT_ALREADY_EXISTS = "WEB3_20002";
    public const string WEB3_ACCOUNT_CREATION_FAILED = "WEB3_20003";
    public const string WEB3_ACCOUNT_IMPORT_FAILED = "WEB3_20004";
    public const string WEB3_ACCOUNT_UPDATE_FAILED = "WEB3_20005";
    public const string WEB3_ACCOUNT_QUERY_FAILED = "WEB3_20006";
    public const string WEB3_INVALID_ADDRESS = "WEB3_20007";
    public const string WEB3_INVALID_PRIVATE_KEY = "WEB3_20008";

    // 区块链交易相关错误 (WEB3_3xxxx)
    public const string WEB3_TRANSACTION_NOT_FOUND = "WEB3_30001";
    public const string WEB3_TRANSACTION_SEND_FAILED = "WEB3_30002";
    public const string WEB3_TRANSACTION_QUERY_FAILED = "WEB3_30003";
    public const string WEB3_TRANSACTION_INVALID_HASH = "WEB3_30004";
    public const string WEB3_INSUFFICIENT_BALANCE = "WEB3_30005";
    public const string WEB3_GAS_ESTIMATION_FAILED = "WEB3_30006";
    public const string WEB3_TRANSACTION_FAILED = "WEB3_30007";

    // 智能合约相关错误 (WEB3_4xxxx)
    public const string WEB3_CONTRACT_NOT_FOUND = "WEB3_40001";
    public const string WEB3_CONTRACT_DEPLOYMENT_FAILED = "WEB3_40002";
    public const string WEB3_CONTRACT_CALL_FAILED = "WEB3_40003";
    public const string WEB3_CONTRACT_INVALID_ABI = "WEB3_40004";
    public const string WEB3_CONTRACT_INVALID_BYTECODE = "WEB3_40005";
    public const string WEB3_CONTRACT_VERIFICATION_FAILED = "WEB3_40006";
    public const string WEB3_CONTRACT_METHOD_NOT_FOUND = "WEB3_40007";
    public const string WEB3_CONTRACT_QUERY_FAILED = "WEB3_40008";
    public const string WEB3_CONTRACT_TRANSACTION_FAILED = "WEB3_40009";
    public const string WEB3_CONTRACT_EVENT_QUERY_FAILED = "WEB3_40010";

    // 网络连接相关错误 (WEB3_5xxxx)
    public const string WEB3_NETWORK_CONNECTION_FAILED = "WEB3_50001";
    public const string WEB3_NETWORK_TIMEOUT = "WEB3_50002";
    public const string WEB3_NETWORK_INVALID_RESPONSE = "WEB3_50003";
    public const string WEB3_BALANCE_QUERY_FAILED = "WEB3_50004";
    public const string WEB3_BLOCK_QUERY_FAILED = "WEB3_50005";
    public const string WEB3_GAS_PRICE_QUERY_FAILED = "WEB3_50006";

    // 数据库相关错误 (WEB3_6xxxx)
    public const string WEB3_DATABASE_CONNECTION_FAILED = "WEB3_60001";
    public const string WEB3_DATABASE_QUERY_FAILED = "WEB3_60002";
    public const string WEB3_DATABASE_UPDATE_FAILED = "WEB3_60003";
    public const string WEB3_DATABASE_DELETE_FAILED = "WEB3_60004";
    public const string WEB3_DATABASE_CONSTRAINT_VIOLATION = "WEB3_60005";

    // 验证相关错误 (WEB3_7xxxx)
    public const string WEB3_VALIDATION_INVALID_AMOUNT = "WEB3_70001";
    public const string WEB3_VALIDATION_INVALID_GAS_PRICE = "WEB3_70002";
    public const string WEB3_VALIDATION_INVALID_GAS_LIMIT = "WEB3_70003";
    public const string WEB3_VALIDATION_MISSING_PRIVATE_KEY = "WEB3_70004";
    public const string WEB3_VALIDATION_INVALID_PARAMETERS = "WEB3_70005";
}



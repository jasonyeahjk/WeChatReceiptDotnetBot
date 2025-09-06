namespace WeChatReceiptBot.API.Common
{
    /// <summary>
    /// 统一错误编码定义
    /// </summary>
    public static class ErrorCodes
    {
        // 认证相关错误 (1000-1999)
        public const string AUTH_INVALID_CREDENTIALS = "AUTH_1001";
        public const string AUTH_USER_NOT_FOUND = "AUTH_1002";
        public const string AUTH_USER_ALREADY_EXISTS = "AUTH_1003";
        public const string AUTH_INVALID_TOKEN = "AUTH_1004";
        public const string AUTH_TOKEN_EXPIRED = "AUTH_1005";
        public const string AUTH_INSUFFICIENT_PERMISSIONS = "AUTH_1006";
        public const string AUTH_WALLET_ALREADY_BOUND = "AUTH_1007";
        public const string AUTH_INVALID_WALLET_ADDRESS = "AUTH_1008";

        // 用户相关错误 (2000-2999)
        public const string USER_NOT_FOUND = "USER_2001";
        public const string USER_PROFILE_UPDATE_FAILED = "USER_2002";
        public const string USER_INVALID_DATA = "USER_2003";

        // 拼团相关错误 (3000-3999)
        public const string GROUP_NOT_FOUND = "GROUP_3001";
        public const string GROUP_ACCESS_DENIED = "GROUP_3002";
        public const string GROUP_MEMBER_NOT_FOUND = "GROUP_3003";
        public const string GROUP_MEMBER_ALREADY_EXISTS = "GROUP_3004";
        public const string GROUP_CANNOT_LEAVE_AS_CREATOR = "GROUP_3005";
        public const string GROUP_INVALID_DATA = "GROUP_3006";

        // 账单相关错误 (4000-4999)
        public const string BILL_NOT_FOUND = "BILL_4001";
        public const string BILL_ACCESS_DENIED = "BILL_4002";
        public const string BILL_ALREADY_SETTLED = "BILL_4003";
        public const string BILL_CANNOT_SETTLE = "BILL_4004";
        public const string BILL_INVALID_AMOUNT = "BILL_4005";
        public const string BILL_INVALID_DATA = "BILL_4006";

        // 交易相关错误 (5000-5999)
        public const string TRANSACTION_NOT_FOUND = "TRANSACTION_5001";
        public const string TRANSACTION_ACCESS_DENIED = "TRANSACTION_5002";
        public const string TRANSACTION_ALREADY_CONFIRMED = "TRANSACTION_5003";
        public const string TRANSACTION_INVALID_AMOUNT = "TRANSACTION_5004";
        public const string TRANSACTION_INVALID_DATA = "TRANSACTION_5005";

        // 支付相关错误 (6000-6999)
        public const string PAYMENT_NOT_FOUND = "PAYMENT_6001";
        public const string PAYMENT_VERIFICATION_FAILED = "PAYMENT_6002";
        public const string PAYMENT_ALREADY_VERIFIED = "PAYMENT_6003";
        public const string PAYMENT_INVALID_DATA = "PAYMENT_6004";

        // 图像识别相关错误 (7000-7999)
        public const string DONUT_SERVICE_UNAVAILABLE = "DONUT_7001";
        public const string DONUT_RECOGNITION_FAILED = "DONUT_7002";
        public const string DONUT_INVALID_IMAGE = "DONUT_7003";
        public const string DONUT_UNSUPPORTED_FORMAT = "DONUT_7004";

        // Web3相关错误 (8000-8999)
        public const string WEB3_SERVICE_UNAVAILABLE = "WEB3_8001";
        public const string WEB3_CONTRACT_DEPLOYMENT_FAILED = "WEB3_8002";
        public const string WEB3_TRANSACTION_FAILED = "WEB3_8003";
        public const string WEB3_INVALID_ADDRESS = "WEB3_8004";
        public const string WEB3_INSUFFICIENT_BALANCE = "WEB3_8005";

        // 数据库相关错误 (9000-9999)
        public const string DATABASE_CONNECTION_FAILED = "DB_9001";
        public const string DATABASE_QUERY_FAILED = "DB_9002";
        public const string DATABASE_CONSTRAINT_VIOLATION = "DB_9003";
        public const string DATABASE_TIMEOUT = "DB_9004";

        // 系统相关错误 (10000-10999)
        public const string SYSTEM_INTERNAL_ERROR = "SYS_10001";
        public const string SYSTEM_SERVICE_UNAVAILABLE = "SYS_10002";
        public const string SYSTEM_VALIDATION_FAILED = "SYS_10003";
        public const string SYSTEM_CONFIGURATION_ERROR = "SYS_10004";
    }

    /// <summary>
    /// 错误消息映射
    /// </summary>
    public static class ErrorMessages
    {
        public static readonly Dictionary<string, string> Messages = new()
        {
            // 认证相关错误消息
            { ErrorCodes.AUTH_INVALID_CREDENTIALS, "用户名或密码错误" },
            { ErrorCodes.AUTH_USER_NOT_FOUND, "用户不存在" },
            { ErrorCodes.AUTH_USER_ALREADY_EXISTS, "用户已存在" },
            { ErrorCodes.AUTH_INVALID_TOKEN, "无效的访问令牌" },
            { ErrorCodes.AUTH_TOKEN_EXPIRED, "访问令牌已过期" },
            { ErrorCodes.AUTH_INSUFFICIENT_PERMISSIONS, "权限不足" },
            { ErrorCodes.AUTH_WALLET_ALREADY_BOUND, "钱包地址已绑定" },
            { ErrorCodes.AUTH_INVALID_WALLET_ADDRESS, "无效的钱包地址" },

            // 用户相关错误消息
            { ErrorCodes.USER_NOT_FOUND, "用户不存在" },
            { ErrorCodes.USER_PROFILE_UPDATE_FAILED, "用户资料更新失败" },
            { ErrorCodes.USER_INVALID_DATA, "用户数据无效" },

            // 拼团相关错误消息
            { ErrorCodes.GROUP_NOT_FOUND, "拼团不存在" },
            { ErrorCodes.GROUP_ACCESS_DENIED, "无权访问该拼团" },
            { ErrorCodes.GROUP_MEMBER_NOT_FOUND, "成员不存在" },
            { ErrorCodes.GROUP_MEMBER_ALREADY_EXISTS, "成员已存在" },
            { ErrorCodes.GROUP_CANNOT_LEAVE_AS_CREATOR, "创建者不能离开拼团" },
            { ErrorCodes.GROUP_INVALID_DATA, "拼团数据无效" },

            // 账单相关错误消息
            { ErrorCodes.BILL_NOT_FOUND, "账单不存在" },
            { ErrorCodes.BILL_ACCESS_DENIED, "无权访问该账单" },
            { ErrorCodes.BILL_ALREADY_SETTLED, "账单已结算" },
            { ErrorCodes.BILL_CANNOT_SETTLE, "账单无法结算" },
            { ErrorCodes.BILL_INVALID_AMOUNT, "无效的金额" },
            { ErrorCodes.BILL_INVALID_DATA, "账单数据无效" },

            // 交易相关错误消息
            { ErrorCodes.TRANSACTION_NOT_FOUND, "交易不存在" },
            { ErrorCodes.TRANSACTION_ACCESS_DENIED, "无权访问该交易" },
            { ErrorCodes.TRANSACTION_ALREADY_CONFIRMED, "交易已确认" },
            { ErrorCodes.TRANSACTION_INVALID_AMOUNT, "无效的交易金额" },
            { ErrorCodes.TRANSACTION_INVALID_DATA, "交易数据无效" },

            // 支付相关错误消息
            { ErrorCodes.PAYMENT_NOT_FOUND, "支付记录不存在" },
            { ErrorCodes.PAYMENT_VERIFICATION_FAILED, "支付验证失败" },
            { ErrorCodes.PAYMENT_ALREADY_VERIFIED, "支付已验证" },
            { ErrorCodes.PAYMENT_INVALID_DATA, "支付数据无效" },

            // 图像识别相关错误消息
            { ErrorCodes.DONUT_SERVICE_UNAVAILABLE, "图像识别服务不可用" },
            { ErrorCodes.DONUT_RECOGNITION_FAILED, "图像识别失败" },
            { ErrorCodes.DONUT_INVALID_IMAGE, "无效的图像" },
            { ErrorCodes.DONUT_UNSUPPORTED_FORMAT, "不支持的图像格式" },

            // Web3相关错误消息
            { ErrorCodes.WEB3_SERVICE_UNAVAILABLE, "Web3服务不可用" },
            { ErrorCodes.WEB3_CONTRACT_DEPLOYMENT_FAILED, "智能合约部署失败" },
            { ErrorCodes.WEB3_TRANSACTION_FAILED, "区块链交易失败" },
            { ErrorCodes.WEB3_INVALID_ADDRESS, "无效的区块链地址" },
            { ErrorCodes.WEB3_INSUFFICIENT_BALANCE, "余额不足" },

            // 数据库相关错误消息
            { ErrorCodes.DATABASE_CONNECTION_FAILED, "数据库连接失败" },
            { ErrorCodes.DATABASE_QUERY_FAILED, "数据库查询失败" },
            { ErrorCodes.DATABASE_CONSTRAINT_VIOLATION, "数据约束违反" },
            { ErrorCodes.DATABASE_TIMEOUT, "数据库操作超时" },

            // 系统相关错误消息
            { ErrorCodes.SYSTEM_INTERNAL_ERROR, "系统内部错误" },
            { ErrorCodes.SYSTEM_SERVICE_UNAVAILABLE, "服务不可用" },
            { ErrorCodes.SYSTEM_VALIDATION_FAILED, "数据验证失败" },
            { ErrorCodes.SYSTEM_CONFIGURATION_ERROR, "系统配置错误" }
        };

        public static string GetMessage(string errorCode)
        {
            return Messages.TryGetValue(errorCode, out var message) ? message : "未知错误";
        }
    }
}


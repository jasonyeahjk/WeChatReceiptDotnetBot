namespace DonutPaymentService.API.Common.Errors
{
    public static class ErrorCodes
    {
        // General System Errors
        public const string SYSTEM_INTERNAL_ERROR = "SYSTEM_1000";
        public const string SYSTEM_VALIDATION_FAILED = "SYSTEM_1001";
        public const string SYSTEM_NOT_FOUND = "SYSTEM_1002";
        public const string SYSTEM_UNAUTHORIZED = "SYSTEM_1003";
        public const string SYSTEM_FORBIDDEN = "SYSTEM_1004";
        public const string SYSTEM_BAD_REQUEST = "SYSTEM_1005";
        public const string SYSTEM_SERVICE_UNAVAILABLE = "SYSTEM_1006";

        // Donut Service Errors
        public const string DONUT_SERVICE_UNAVAILABLE = "DONUT_6000";
        public const string DONUT_RECOGNITION_FAILED = "DONUT_6001";
        public const string DONUT_INVALID_IMAGE = "DONUT_6002";
        public const string DONUT_TASK_NOT_FOUND = "DONUT_6003";

        // Database Errors
        public const string DATABASE_CONNECTION_FAILED = "DB_8000";
        public const string DATABASE_QUERY_FAILED = "DB_8001";
        public const string DATABASE_INSERT_FAILED = "DB_8002";
        public const string DATABASE_UPDATE_FAILED = "DB_8003";
        public const string DATABASE_DELETE_FAILED = "DB_8004";
    }
}


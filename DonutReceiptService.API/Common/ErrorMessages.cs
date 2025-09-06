using System.Collections.ObjectModel;

namespace DonutReceiptService.API.Common.Errors
{
    public static class ErrorMessages
    {
        private static readonly Dictionary<string, string> _messages = new()
        {
            // General System Errors
            { ErrorCodes.SYSTEM_INTERNAL_ERROR, "系统内部错误" },
            { ErrorCodes.SYSTEM_VALIDATION_FAILED, "请求参数验证失败" },
            { ErrorCodes.SYSTEM_NOT_FOUND, "请求的资源不存在" },
            { ErrorCodes.SYSTEM_UNAUTHORIZED, "未授权的访问" },
            { ErrorCodes.SYSTEM_FORBIDDEN, "无权限访问" },
            { ErrorCodes.SYSTEM_BAD_REQUEST, "无效的请求" },
            { ErrorCodes.SYSTEM_SERVICE_UNAVAILABLE, "服务暂时不可用" },

            // Donut Service Errors
            { ErrorCodes.DONUT_SERVICE_UNAVAILABLE, "Donut识别服务不可用" },
            { ErrorCodes.DONUT_RECOGNITION_FAILED, "图像识别失败" },
            { ErrorCodes.DONUT_INVALID_IMAGE, "无效的图像文件" },
            { ErrorCodes.DONUT_TASK_NOT_FOUND, "识别任务不存在" },

            // Database Errors
            { ErrorCodes.DATABASE_CONNECTION_FAILED, "数据库连接失败" },
            { ErrorCodes.DATABASE_QUERY_FAILED, "数据库查询失败" },
            { ErrorCodes.DATABASE_INSERT_FAILED, "数据插入失败" },
            { ErrorCodes.DATABASE_UPDATE_FAILED, "数据更新失败" },
            { ErrorCodes.DATABASE_DELETE_FAILED, "数据删除失败" }
        };

        public static ReadOnlyDictionary<string, string> Messages => new(_messages);

        public static string GetMessage(string errorCode)
        {
            if (Messages.TryGetValue(errorCode, out var message))
            {
                return message;
            }
            return "未知错误";
        }
    }
}



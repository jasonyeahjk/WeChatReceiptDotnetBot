namespace DonutReceiptService.API.Domain.ValueObjects
{
    /// <summary>
    /// 识别状态枚举
    /// </summary>
    public enum RecognitionStatus
    {
        /// <summary>
        /// 等待处理
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 处理中
        /// </summary>
        Processing = 1,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 2,

        /// <summary>
        /// 失败
        /// </summary>
        Failed = 3
    }
}


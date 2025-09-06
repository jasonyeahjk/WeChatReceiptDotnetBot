using System.ComponentModel.DataAnnotations;

namespace DonutReceiptService.API.Application.DTOs
{
    /// <summary>
    /// 识别请求 DTO
    /// </summary>
    public class RecognitionRequestDto
    {
        /// <summary>
        /// Base64 编码的图像数据
        /// </summary>
        [Required(ErrorMessage = "图像数据不能为空")]
        public string ImageData { get; set; } = string.Empty;

        /// <summary>
        /// 文件名
        /// </summary>
        [Required(ErrorMessage = "文件名不能为空")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// 用户ID
        /// </summary>
        [Required(ErrorMessage = "用户ID不能为空")]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 识别选项
        /// </summary>
        public RecognitionOptionsDto? Options { get; set; }
    }

    /// <summary>
    /// 识别选项 DTO
    /// </summary>
    public class RecognitionOptionsDto
    {
        /// <summary>
        /// 是否启用图像预处理
        /// </summary>
        public bool EnablePreprocessing { get; set; } = true;

        /// <summary>
        /// 最小置信度阈值
        /// </summary>
        [Range(0.0, 1.0, ErrorMessage = "置信度阈值必须在0到1之间")]
        public float MinConfidenceThreshold { get; set; } = 0.5f;

        /// <summary>
        /// 识别语言
        /// </summary>
        public string Language { get; set; } = "zh-CN";

        /// <summary>
        /// 是否返回详细元数据
        /// </summary>
        public bool IncludeMetadata { get; set; } = false;
    }
}


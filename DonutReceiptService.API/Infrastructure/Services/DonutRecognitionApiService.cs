using DonutReceiptService.API.Domain.Services;
using DonutReceiptService.API.Domain.ValueObjects;
using DonutReceiptService.API.Common.Errors;
using FluentResults;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace DonutReceiptService.API.Infrastructure.Services
{
    /// <summary>
    /// Donut 识别服务 API 实现
    /// </summary>
    public class DonutRecognitionApiService : IDonutRecognitionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DonutRecognitionApiService> _logger;
        private readonly string _donutApiBaseUrl;

        public DonutRecognitionApiService(
            HttpClient httpClient,
            ILogger<DonutRecognitionApiService> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _donutApiBaseUrl = configuration["DonutApi:BaseUrl"] ?? 
                               throw new ArgumentNullException("DonutApi:BaseUrl configuration is missing");

            // 确保 HttpClient 的 BaseAddress 已设置
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(_donutApiBaseUrl);
            }
        }

        public async Task<Result<RecognitionResult>> RecognizeReceiptAsync(
            byte[] imageData, 
            string fileName, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Calling Donut API for receipt recognition. File: {FileName}", fileName);

                // 图像预处理 (如果需要)
                byte[] processedImageData = imageData;
                // TODO: 根据实际需求调用预处理逻辑

                var base64Image = Convert.ToBase64String(processedImageData);

                var requestBody = new
                {
                    imageData = base64Image,
                    fileName = fileName
                };

                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(
                    "api/recognize/receipt", 
                    jsonContent, 
                    cancellationToken);

                response.EnsureSuccessStatusCode(); // 抛出 HttpRequestException 如果状态码不是成功

                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
                var apiResponse = JsonConvert.DeserializeObject<DonutApiResponse>(responseString);

                if (apiResponse == null || !apiResponse.Success)
                {
                    _logger.LogError("Donut API returned an error: {Message}", apiResponse?.Message);
                    return Result.Fail(new ApplicationError(
                        ErrorCodes.DONUT_RECOGNITION_FAILED, 
                        apiResponse?.Message ?? "Donut API识别失败"));
                }

                // 模拟解析 Donut API 返回的数据到 ReceiptData
                var extractedData = MapDonutResponseToReceiptData(apiResponse.Data);
                var confidenceScore = apiResponse.ConfidenceScore ?? 0.9f; // 模拟置信度
                var processingTime = TimeSpan.FromMilliseconds(apiResponse.ProcessingTimeMs ?? 100); // 模拟处理时间

                _logger.LogInformation("Donut API recognition successful for file: {FileName}", fileName);
                return Result.Ok(new RecognitionResult(extractedData, confidenceScore, processingTime));
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request error to Donut API: {Message}", httpEx.Message);
                return Result.Fail(new ApplicationError(
                    ErrorCodes.DONUT_SERVICE_UNAVAILABLE, 
                    $"Donut识别服务连接失败: {httpEx.Message}", 
                    503));
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization error from Donut API: {Message}", jsonEx.Message);
                return Result.Fail(new ApplicationError(
                    ErrorCodes.DONUT_RECOGNITION_FAILED, 
                    $"Donut API响应解析失败: {jsonEx.Message}", 
                    500));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during Donut API recognition: {Message}", ex.Message);
                return Result.Fail(new ApplicationError(
                    ErrorCodes.DONUT_RECOGNITION_FAILED, 
                    $"Donut API识别过程中发生未知错误: {ex.Message}", 
                    500));
            }
        }

        public Result<bool> ValidateImageFormat(byte[] imageData, string fileName)
        {
            // 这是一个简单的模拟验证，实际应使用图像处理库来验证
            if (imageData == null || imageData.Length == 0)
            {
                return Result.Fail(new ApplicationError(ErrorCodes.DONUT_INVALID_IMAGE, "图像数据为空"));
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };

            if (!supportedExtensions.Contains(extension))
            {
                return Result.Fail(new ApplicationError(ErrorCodes.DONUT_INVALID_IMAGE, $"不支持的图像格式: {extension}"));
            }

            return Result.Ok(true);
        }

        public IEnumerable<string> GetSupportedFormats()
        {
            return new[] { "jpg", "jpeg", "png", "pdf" };
        }

        public Task<Result<byte[]>> PreprocessImageAsync(byte[] imageData, CancellationToken cancellationToken = default)
        {
            // TODO: 实现图像预处理逻辑，例如调整大小、灰度化、增强对比度等
            _logger.LogInformation("Image preprocessing not fully implemented. Returning original image data.");
            return Task.FromResult(Result.Ok(imageData));
        }

        private ReceiptData MapDonutResponseToReceiptData(dynamic donutData)
        {
            // 这是一个模拟的映射逻辑，需要根据实际 Donut API 返回的 JSON 结构进行调整
            // 假设 Donut API 返回的 JSON 结构类似：
            // {
            //   "merchant_name": "",
            //   "total_amount": 123.45,
            //   "items": [
            //     {"name": "", "quantity": 1, "unit_price": 10.0, "total_price": 10.0}
            //   ]
            // }

            string merchantName = donutData.merchant_name?.ToString() ?? "未知商家";
            string merchantAddress = donutData.merchant_address?.ToString() ?? "未知地址";
            DateTime transactionDate = DateTime.TryParse(donutData.transaction_date?.ToString(), out DateTime date) ? date : DateTime.Today;
            decimal totalAmount = decimal.TryParse(donutData.total_amount?.ToString(), out decimal amount) ? amount : 0m;
            string currency = donutData.currency?.ToString() ?? "CNY";

            var items = new List<ReceiptItem>();
            if (donutData.items != null)
            {
                foreach (var item in donutData.items)
                {
                    items.Add(new ReceiptItem(
                        item.name?.ToString() ?? "",
                        (int)(item.quantity ?? 1),
                        (decimal)(item.unit_price ?? 0m),
                        (decimal)(item.total_price ?? 0m),
                        item.category?.ToString(),
                        item.description?.ToString()
                    ));
                }
            }

            return new ReceiptData(
                merchantName,
                merchantAddress,
                transactionDate,
                totalAmount,
                currency,
                items,
                receiptNumber: donutData.receipt_number?.ToString(),
                paymentMethod: donutData.payment_method?.ToString(),
                taxAmount: decimal.TryParse(donutData.tax_amount?.ToString(), out decimal tax) ? tax : (decimal?)null,
                tipAmount: decimal.TryParse(donutData.tip_amount?.ToString(), out decimal tip) ? tip : (decimal?)null,
                additionalFields: JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(donutData))
            );
        }

        private class DonutApiResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public dynamic? Data { get; set; }
            public float? ConfidenceScore { get; set; }
            public long? ProcessingTimeMs { get; set; }
        }
    }
}



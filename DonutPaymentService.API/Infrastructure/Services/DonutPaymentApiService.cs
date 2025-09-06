using FluentResults;
using DonutPaymentService.API.Domain.Services;
using DonutPaymentService.API.Domain.ValueObjects;
using DonutPaymentService.API.Common.Errors;
using System.Text.Json;
using System.Text;

namespace DonutPaymentService.API.Infrastructure.Services
{
    public class DonutPaymentApiService : IPaymentRecognitionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DonutPaymentApiService> _logger;

        public DonutPaymentApiService(HttpClient httpClient, ILogger<DonutPaymentApiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configure HttpClient base address for the external Donut service
            // This should ideally come from configuration (e.g., appsettings.json)
            _httpClient.BaseAddress = new Uri("http://localhost:8001/"); // Assuming Donut payment service runs on port 8001
        }

        public async Task<Result<PaymentData>> RecognizePaymentAsync(string imageBase64)
        {
            _logger.LogInformation("Calling external Donut payment recognition service.");

            try
            {
                var requestBody = new { image = imageBase64 };
                var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody),
                                                    Encoding.UTF8,
                                                    "application/json");

                var response = await _httpClient.PostAsync("api/recognize/payment", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var donutResponse = JsonSerializer.Deserialize<DonutApiResponse>(responseContent);

                    if (donutResponse?.Success == true && donutResponse.Data.ValueKind != JsonValueKind.Undefined)
                    {
                        var merchantName = donutResponse.Data.TryGetProperty("merchant_name", out var mn) ? mn.GetString() : null;
                        var paymentMethod = donutResponse.Data.TryGetProperty("payment_method", out var pm) ? pm.GetString() : null;
                        var amount = donutResponse.Data.TryGetProperty("amount", out var amt) ? ParseDecimal(amt.ToString()) : 0;
                        var paymentDate = donutResponse.Data.TryGetProperty("date", out var pd) ? ParseDateTime(pd.ToString()) : null;
                        var transactionId = donutResponse.Data.TryGetProperty("transaction_id", out var tid) ? tid.GetString() : null;
                        var currency = donutResponse.Data.TryGetProperty("currency", out var curr) ? curr.GetString() : null;
                        var notes = donutResponse.Data.TryGetProperty("notes", out var nts) ? nts.GetString() : null;

                        var items = new List<PaymentItem>();
                        if (donutResponse.Data.TryGetProperty("items", out var itemsElement) && itemsElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var itemElement in itemsElement.EnumerateArray())
                            {
                                var itemName = itemElement.TryGetProperty("name", out var iname) ? iname.GetString() : null;
                                var itemAmount = itemElement.TryGetProperty("amount", out var iamt) ? ParseDecimal(iamt.ToString()) : 0;
                                var itemQuantity = itemElement.TryGetProperty("quantity", out var iqty) ? iqty.GetInt32() : 0;
                                items.Add(new PaymentItem(itemName, itemAmount, itemQuantity));
                            }
                        }

                        var paymentData = new PaymentData(
                            merchantName: merchantName,
                            paymentMethod: paymentMethod,
                            amount: amount,
                            paymentDate: paymentDate,
                            transactionId: transactionId,
                            currency: currency,
                            notes: notes,
                            items: items
                        );
                        _logger.LogInformation("Donut payment recognition successful.");
                        return Result.Ok(paymentData);
                    }
                    else
                    {
                        var errorMessage = donutResponse?.Message ?? "Unknown error from Donut service.";
                        _logger.LogWarning("Donut payment recognition failed: {ErrorMessage}", errorMessage);
                        return Result.Fail(new ApplicationError(ErrorCodes.DONUT_RECOGNITION_FAILED, errorMessage));
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Donut payment service returned error status code {StatusCode}: {Content}", response.StatusCode, errorContent);
                    return Result.Fail(new ApplicationError(ErrorCodes.DONUT_SERVICE_UNAVAILABLE, $"Donut service error: {response.StatusCode} - {errorContent}", (int)response.StatusCode));
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to Donut payment service failed.");
                return Result.Fail(new ApplicationError(ErrorCodes.DONUT_SERVICE_UNAVAILABLE, "Failed to connect to Donut payment recognition service.", 503));
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response from Donut payment service.");
                return Result.Fail(new ApplicationError(ErrorCodes.SYSTEM_INTERNAL_ERROR, "Invalid response from Donut payment recognition service."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while calling Donut payment service.");
                return Result.Fail(new ApplicationError(ErrorCodes.SYSTEM_INTERNAL_ERROR, "An unexpected error occurred during payment recognition."));
            }
        }

        private static decimal ParseDecimal(string? value)
        {
            if (decimal.TryParse(value, out var result))
            {
                return result;
            }
            return 0;
        }

        private static DateTime? ParseDateTime(string? value)
        {
            if (DateTime.TryParse(value, out var result))
            {
                return result;
            }
            return null;
        }

        // Helper class to deserialize the generic JSON response from the Python Donut service
        private class DonutApiResponse
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public JsonElement Data { get; set; }
        }
    }
}



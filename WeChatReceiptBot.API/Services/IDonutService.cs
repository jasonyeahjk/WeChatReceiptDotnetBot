using WeChatReceiptBot.API.DTOs;

namespace WeChatReceiptBot.API.Services
{
    public interface IDonutService
    {
        // Receipt recognition
        Task<DonutRecognitionResponse> RecognizeReceiptAsync(string base64Image);
        Task<DonutRecognitionResponse> RecognizeReceiptAsync(byte[] imageBytes);
        Task<DonutRecognitionResponse> RecognizeReceiptFromUrlAsync(string imageUrl);
        
        // Payment record recognition
        Task<DonutRecognitionResponse> RecognizePaymentRecordAsync(string base64Image);
        Task<DonutRecognitionResponse> RecognizePaymentRecordAsync(byte[] imageBytes);
        Task<DonutRecognitionResponse> RecognizePaymentRecordFromUrlAsync(string imageUrl);
        
        // Generic document recognition
        Task<DonutRecognitionResponse> RecognizeDocumentAsync(string base64Image, string documentType = "receipt");
        Task<DonutRecognitionResponse> RecognizeDocumentAsync(byte[] imageBytes, string documentType = "receipt");
        
        // Service health check
        Task<bool> IsServiceHealthyAsync();
        Task<string> GetServiceVersionAsync();
        
        // Batch processing
        Task<List<DonutRecognitionResponse>> RecognizeReceiptsBatchAsync(List<string> base64Images);
        Task<List<DonutRecognitionResponse>> RecognizePaymentRecordsBatchAsync(List<string> base64Images);
        
        // Configuration
        Task<bool> UpdateRecognitionConfigAsync(string configJson);
        Task<string> GetRecognitionConfigAsync();
    }
}


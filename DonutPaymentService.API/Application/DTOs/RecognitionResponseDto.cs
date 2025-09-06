using DonutPaymentService.API.Domain.ValueObjects;

namespace DonutPaymentService.API.Application.DTOs
{
    public class RecognitionResponseDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string ImageBase64 { get; set; }
        public string Status { get; set; }
        public PaymentData? RecognizedData { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public int RetryCount { get; set; }
    }

    public class RecognitionHistoryResponseDto
    {
        public List<RecognitionResponseDto> Results { get; set; } = new List<RecognitionResponseDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class RecognitionStatisticsResponseDto
    {
        public string UserId { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int FailedTasks { get; set; }
        public double SuccessRate => TotalTasks == 0 ? 0 : (double)CompletedTasks / TotalTasks;
    }
}


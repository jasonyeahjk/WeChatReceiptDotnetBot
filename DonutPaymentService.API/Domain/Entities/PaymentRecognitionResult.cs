using DonutPaymentService.API.Domain.ValueObjects;

namespace DonutPaymentService.API.Domain.Entities
{
    public class PaymentRecognitionResult
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public string ImageBase64 { get; private set; }
        public RecognitionStatus Status { get; private set; }
        public PaymentData? RecognizedData { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }
        public string? ErrorMessage { get; private set; }
        public int RetryCount { get; private set; }

        private PaymentRecognitionResult() { }

        public PaymentRecognitionResult(string userId, string imageBase64)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            ImageBase64 = imageBase64;
            Status = RecognitionStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            RetryCount = 0;
        }

        public void MarkAsProcessing()
        {
            if (Status != RecognitionStatus.Pending && Status != RecognitionStatus.Failed)
            {
                throw new InvalidOperationException("Cannot mark as processing from current status.");
            }
            Status = RecognitionStatus.Processing;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkAsCompleted(PaymentData recognizedData)
        {
            if (Status != RecognitionStatus.Processing)
            {
                throw new InvalidOperationException("Cannot mark as completed from current status.");
            }
            Status = RecognitionStatus.Completed;
            RecognizedData = recognizedData;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed(string errorMessage)
        {
            Status = RecognitionStatus.Failed;
            ErrorMessage = errorMessage;
            RetryCount++;
            ProcessedAt = DateTime.UtcNow;
        }

        public void ClearError()
        {
            ErrorMessage = null;
        }
    }
}


using FluentResults;
using DonutPaymentService.API.Domain.ValueObjects;

namespace DonutPaymentService.API.Domain.Services
{
    public interface IPaymentRecognitionService
    {
        Task<Result<PaymentData>> RecognizePaymentAsync(string imageBase64);
    }
}


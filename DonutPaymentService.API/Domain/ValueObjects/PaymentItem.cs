namespace DonutPaymentService.API.Domain.ValueObjects
{
    public record PaymentItem(string? Name, decimal Amount, int Quantity);
}



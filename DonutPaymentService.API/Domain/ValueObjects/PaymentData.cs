using System.Collections.Generic;

namespace DonutPaymentService.API.Domain.ValueObjects
{
    public class PaymentData
    {
        public string? MerchantName { get; private set; }
        public string? PaymentMethod { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime? PaymentDate { get; private set; }
        public string? TransactionId { get; private set; }
        public string? Currency { get; private set; }
        public string? Notes { get; private set; }
        public IReadOnlyList<PaymentItem> Items { get; private set; }

        public PaymentData(string? merchantName, string? paymentMethod, decimal amount, DateTime? paymentDate, string? transactionId, string? currency, string? notes, IReadOnlyList<PaymentItem>? items = null)
        {
            MerchantName = merchantName;
            PaymentMethod = paymentMethod;
            Amount = amount;
            PaymentDate = paymentDate;
            TransactionId = transactionId;
            Currency = currency;
            Notes = notes;
            Items = items ?? new List<PaymentItem>();
        }

        // For deserialization
        private PaymentData() { }

        public override bool Equals(object? obj)
        {
            return obj is PaymentData data &&
                   MerchantName == data.MerchantName &&
                   PaymentMethod == data.PaymentMethod &&
                   Amount == data.Amount &&
                   PaymentDate == data.PaymentDate &&
                   TransactionId == data.TransactionId &&
                   Currency == data.Currency &&
                   Notes == data.Notes;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MerchantName, PaymentMethod, Amount, PaymentDate, TransactionId, Currency, Notes);
        }
    }
}


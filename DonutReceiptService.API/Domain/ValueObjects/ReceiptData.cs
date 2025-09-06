namespace DonutReceiptService.API.Domain.ValueObjects
{
    /// <summary>
    /// 收据数据值对象
    /// </summary>
    public class ReceiptData
    {
        public string MerchantName { get; }
        public string MerchantAddress { get; }
        public DateTime TransactionDate { get; }
        public decimal TotalAmount { get; }
        public string Currency { get; }
        public List<ReceiptItem> Items { get; }
        public string? ReceiptNumber { get; }
        public string? PaymentMethod { get; }
        public decimal? TaxAmount { get; }
        public decimal? TipAmount { get; }
        public Dictionary<string, object> AdditionalFields { get; }

        public ReceiptData(
            string merchantName,
            string merchantAddress,
            DateTime transactionDate,
            decimal totalAmount,
            string currency,
            List<ReceiptItem> items,
            string? receiptNumber = null,
            string? paymentMethod = null,
            decimal? taxAmount = null,
            decimal? tipAmount = null,
            Dictionary<string, object>? additionalFields = null)
        {
            MerchantName = merchantName ?? throw new ArgumentNullException(nameof(merchantName));
            MerchantAddress = merchantAddress ?? throw new ArgumentNullException(nameof(merchantAddress));
            TransactionDate = transactionDate;
            TotalAmount = totalAmount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
            Items = items ?? throw new ArgumentNullException(nameof(items));
            ReceiptNumber = receiptNumber;
            PaymentMethod = paymentMethod;
            TaxAmount = taxAmount;
            TipAmount = tipAmount;
            AdditionalFields = additionalFields ?? new Dictionary<string, object>();

            ValidateData();
        }

        private void ValidateData()
        {
            if (string.IsNullOrWhiteSpace(MerchantName))
                throw new ArgumentException("Merchant name cannot be empty", nameof(MerchantName));

            if (TotalAmount < 0)
                throw new ArgumentException("Total amount cannot be negative", nameof(TotalAmount));

            if (string.IsNullOrWhiteSpace(Currency))
                throw new ArgumentException("Currency cannot be empty", nameof(Currency));

            if (TaxAmount.HasValue && TaxAmount < 0)
                throw new ArgumentException("Tax amount cannot be negative", nameof(TaxAmount));

            if (TipAmount.HasValue && TipAmount < 0)
                throw new ArgumentException("Tip amount cannot be negative", nameof(TipAmount));
        }
    }

    /// <summary>
    /// 收据项目值对象
    /// </summary>
    public class ReceiptItem
    {
        public string Name { get; }
        public int Quantity { get; }
        public decimal UnitPrice { get; }
        public decimal TotalPrice { get; }
        public string? Category { get; }
        public string? Description { get; }

        public ReceiptItem(
            string name,
            int quantity,
            decimal unitPrice,
            decimal totalPrice,
            string? category = null,
            string? description = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = totalPrice;
            Category = category;
            Description = description;

            ValidateItem();
        }

        private void ValidateItem()
        {
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentException("Item name cannot be empty", nameof(Name));

            if (Quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(Quantity));

            if (UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(UnitPrice));

            if (TotalPrice < 0)
                throw new ArgumentException("Total price cannot be negative", nameof(TotalPrice));
        }
    }
}


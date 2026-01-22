using Domain.Enums;
using Domain.SeedWork;

namespace Domain.ValueObjects
{
    public class PaymentContext : ValueObject
    {
        public PaymentChannel Channel { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string? ExternalTransactionId { get; private set; }
        public bool IsProductSettlementRequired { get; private set; } // Supplier'a ödeme?
        public bool IsLogisticsSettlementRequired { get; private set; } // Transporter'a ödeme?

        private PaymentContext() { }

        public PaymentContext(PaymentChannel channel, PaymentStatus status)
        {
            Channel = channel;
            Status = status;
            // Kural: Para Multillo üzerinden geçtiyse (OnPlatform), Supplier'a biz öderiz.
            // Yoksa (CSV/External), parayı dışarıda almışlardır, biz karışmayız.
            IsProductSettlementRequired = (channel == PaymentChannel.OnPlatform);
            IsLogisticsSettlementRequired = true; // Lojistik ücreti her zaman vardır.
        }
        protected override IEnumerable<object> GetEqualityComponents() { yield return Channel; }
    }
}
using Domain.Enums;
using Domain.SeedWork;

namespace Domain.Entities.Order
{
    public class ReturnItem : Entity
    {
        public Guid? PackageId { get; private set; }
        public string Description { get; private set; } // "3 tane boş kasa"
        public int Quantity { get; private set; }
        public ReturnReason Reason { get; private set; } // Deposit, Damaged

        private ReturnItem() { }
    }
}
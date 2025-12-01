namespace Domain.Entities.Customer
{
    // KURUMSAL MÜŞTERİ (Zincir Market/Restoran)
    public class CorporateCustomer : Customer
    {
        public string CvrNumber { get; private set; }

        public CorporateCustomer(string name) : base(name) { }
    }
}
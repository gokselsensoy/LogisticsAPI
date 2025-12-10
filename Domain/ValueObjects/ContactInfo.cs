using Domain.SeedWork;

namespace Domain.ValueObjects
{
    public class ContactInfo : ValueObject
    {
        public string Name { get; private set; } // "Ahmet Yılmaz"
        public string Phone { get; private set; }
        public string Email { get; private set; }
        private ContactInfo() { }

        public ContactInfo(string name, string phone, string email)
        {
            Name = name; Phone = phone; Email = email;
        }
        protected override IEnumerable<object> GetEqualityComponents() { yield return Phone; yield return Email; }
    }
}
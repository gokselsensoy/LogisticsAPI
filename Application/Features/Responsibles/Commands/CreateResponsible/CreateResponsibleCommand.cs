using Application.Abstractions.Messaging;
using Domain.Enums;

namespace Application.Features.Responsibles.Commands.CreateResponsible
{
    public class CreateResponsibleCommand : ICommand<Guid>
    {
        // Identity ve AppUser oluşturmak için gerekli
        public string Email { get; set; }
        public string Password { get; set; }

        // CorporateResponsible tablosu için gerekli detaylar
        public string FullName { get; set; }
        public string Phone { get; set; }

        // Yetkilendirme
        public List<CorporateRole> Roles { get; set; } = new();
    }
}

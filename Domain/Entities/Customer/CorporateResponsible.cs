using Domain.Entities.Departments;
using Domain.Enums;
using Domain.Exceptions;
using Domain.SeedWork;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Domain.Entities.Customer
{
    public class CorporateResponsible : FullAuditedEntity, IAggregateRoot
    {
        public Guid CorporateCustomerId { get; private set; }
        public Guid AppUserId { get; private set; }
        public string FullName { get; private set; }
        public string Phone { get; private set; }
        private readonly List<CorporateRole> _roles = new();
        public IReadOnlyCollection<CorporateRole> Roles => _roles.AsReadOnly();

        // Şube Haritası
        private readonly List<CorporateAddressResponsibleMap> _assignedAddresses = new();
        public IReadOnlyCollection<CorporateAddressResponsibleMap> AssignedAddresses => _assignedAddresses.AsReadOnly();

        private CorporateResponsible() { }

        public CorporateResponsible(Guid corporateCustomerId, Guid appUserId, string fullName, string phone, List<CorporateRole>? roles = null)
        {
            Id = Guid.NewGuid();
            CorporateCustomerId = corporateCustomerId;
            AppUserId = appUserId;
            FullName = fullName;
            Phone = phone;

            if (roles != null && roles.Any())
            {
                _roles.AddRange(roles);
            }
        }

        public void UpdateDetails(string fullName, string phone, List<CorporateRole> roles)
        {
            FullName = fullName;
            Phone = phone;

            // Rolleri güncelle
            _roles.Clear();
            _roles.AddRange(roles);
        }

        public void AssignAddress(Guid addressId)
        {
            if (!_assignedAddresses.Any(x => x.AddressId == addressId))
            {
                _assignedAddresses.Add(new CorporateAddressResponsibleMap(Id, addressId));
            }
        }

        public void UnassignAddress(Guid addressId)
        {
            var map = _assignedAddresses.FirstOrDefault(x => x.AddressId == addressId);
            if (map != null)
            {
                _assignedAddresses.Remove(map);
            }
        }

        public void AddRole(CorporateRole role)
        {
            if (!_roles.Contains(role))
            {
                _roles.Add(role);
            }
        }

        public void UpdateRoles(List<CorporateRole> newRoles)
        {
            // Eğer liste null ise temizle
            if (newRoles == null)
            {
                _roles.Clear();
                return;
            }

            var currentRoles = _roles.OrderBy(r => r).ToList();
            var incomingRoles = newRoles.OrderBy(r => r).ToList();

            if (!currentRoles.SequenceEqual(incomingRoles))
            {
                _roles.Clear();
                _roles.AddRange(newRoles);

                // Roller değişti eventi (Güvenlik logları için önemli olabilir)
                // AddDomainEvent(new WorkerRolesUpdatedEvent(Id));
            }
        }

        public void RemoveRole(CorporateRole role)
        {
            if (_roles.Contains(role))
            {
                _roles.Remove(role);
            }
        }

        public void UnlinkUser()
        {
            AppUserId = Guid.Empty;
            // Not: Eğer AppUserId üzerinde veritabanında Foreign Key varsa, 
            // Guid.Empty hataya sebep olabilir. Eğer FK varsa AppUserId'yi nullable (Guid?) yapmalısın.
            // FK yoksa Guid.Empty çalışır.
        }

        public bool HasRole(CorporateRole role) => _roles.Contains(role);

        public bool IsAdmin() => _roles.Contains(CorporateRole.Admin);
    }
}
using Domain.SeedWork;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Domain.Entities
{
    // =========================================================================
    // 1. SHARED KERNEL (ORTAK YAPILAR)
    // =========================================================================

    // Value Object: Adres ve Koordinat
    public class Location : ValueObject
    {
        public Point Point { get; private set; } // Harita konumu
        public string FormattedAddress { get; private set; } // Açık adres
        public string City { get; private set; }
        public string District { get; private set; }

        private Location() { } // EF Core için boş ctor

        public Location(Point point, string formattedAddress, string city, string district)
        {
            Point = point;
            FormattedAddress = formattedAddress;
            City = city;
            District = district;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Point;
            yield return FormattedAddress;
        }
    }

    // Value Object: Boyutlar
    public class Dimensions : ValueObject
    {
        public double Width { get; private set; }
        public double Height { get; private set; }
        public double Depth { get; private set; }
        public double VolumeM3 => Width * Height * Depth;

        public Dimensions(double width, double height, double depth)
        {
            Width = width; Height = height; Depth = depth;
        }

        protected override IEnumerable<object> GetEqualityComponents() { yield return VolumeM3; }
    }

    // Value Object: Para
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        public Money(decimal amount, string currency)
        {
            Amount = amount; Currency = currency;
        }

        protected override IEnumerable<object> GetEqualityComponents() { yield return Amount; yield return Currency; }
    }

    public enum WorkerRole { Owner, Admin, Manager, WarehouseStaff, Driver, Dispatcher }

    public enum AddressType { Branch, Personal }
    public enum PackageType { Pallet, Parcel, Piece }

    public enum CustomerType { CorporateBranch, Individual }

    public enum ShipmentStatus { Pending, Planned, Loaded, InTransit, Delivered, Failed }

    public enum TaskType { Delivery, ReturnPickup, DepositCheck }

    public class ReturnRequest : Entity, IAggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public Guid? OriginalOrderId { get; private set; }
        public Guid TargetTerminalId { get; private set; } // Nereye dönecek?

        public ReturnStatus Status { get; private set; } // Requested, Approved, PickedUp, Refunded

        private readonly List<ReturnItem> _items = new();
        public IReadOnlyCollection<ReturnItem> Items => _items.AsReadOnly();
    }

    public class ReturnItem : Entity
    {
        public Guid PackageId { get; private set; }
        public int Quantity { get; private set; }
        public ReturnReason Reason { get; private set; } // Damaged, Expired, EmptyDeposit
    }

    public enum ReturnReason { Damaged, Expired, WrongItem, EmptyPackageDeposit }
    public enum ReturnStatus { Requested, Approved, AssignedToRoute, PickedUp, Completed, Rejected }

    // Ara Tablo: Hangi Çalışan -> Hangi Şubeye Bakıyor?
    public class WorkerBranchAssignment : Entity
    {
        public Guid CorporateWorkerId { get; private set; }
        public Guid BranchId { get; private set; }

        public WorkerBranchAssignment(Guid corporateWorkerId, Guid branchId)
        {
            CorporateWorkerId = corporateWorkerId;
            BranchId = branchId;
        }
    }

    public enum CorporateRole
    {
        Owner,          // Patron (Her yeri görür)
        Admin,          // Sistem Yöneticisi
        RegionManager,  // Bölge Müdürü (Atanan şubeleri görür)
        BranchManager,  // Şube Müdürü (Genelde tek şube atanır)
        Staff           // Garson/Operatör
    }
}
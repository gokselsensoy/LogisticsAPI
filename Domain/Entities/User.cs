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

    // =========================================================================
    // 2. IDENTITY & ACTORS (KİMLİK VE AKTÖRLER)
    // =========================================================================

    // [AppUser]: Sisteme giren GERÇEK KİŞİ (Ahmet, Mehmet)
    // Bu tabloda şirket bilgisi YOKTUR. Sadece kişi bilgisi vardır.
    public class AppUser : Entity, IAggregateRoot
    {
        public Guid IdentityId { get; private set; } // Auth servisindeki ID
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public string PhoneNumber { get; private set; }

        public AppUser(Guid identityId, string email, string fullName)
        {
            Id = Guid.NewGuid();
            IdentityId = identityId;
            Email = email;
            FullName = fullName;
        }
    }

    // [Company]: Tüzel Kişilik (Base Class)
    // Transporter, Supplier ve Kurumsal Müşteri buradan türer.
    public abstract class Company : Entity, IAggregateRoot
    {
        public string Name { get; private set; } // Firma Adı
        public string? TaxNumber { get; private set; } // CVR No
        public string BillingAddress { get; private set; }

        // Bu şirkette çalışanlar (Worker tablosu ile bağlı)
        private readonly List<Worker> _workers = new();
        public IReadOnlyCollection<Worker> Workers => _workers.AsReadOnly();

        protected Company(string name, string? taxNumber)
        {
            Id = Guid.NewGuid();
            Name = name;
            TaxNumber = taxNumber;
        }
    }

    // [Worker]: Hangi AppUser, Hangi Company'de çalışıyor?
    public class Worker : Entity
    {
        public Guid CompanyId { get; private set; } // Hangi Şirket?
        public Guid AppUserId { get; private set; } // Hangi Kişi?

        // Bu kişi bu şirkette ne iş yapıyor?
        // Bir kişi hem Şoför hem Müdür olabilir, o yüzden Flag veya List tutulabilir.
        // Basitlik için burada Enum tutuyoruz.
        public WorkerRole Role { get; private set; }

        public Worker(Guid companyId, Guid appUserId, WorkerRole role)
        {
            CompanyId = companyId;
            AppUserId = appUserId;
            Role = role;
        }
    }

    public enum WorkerRole { Owner, Admin, Manager, WarehouseStaff, Driver, Dispatcher }

    // -------------------------------------------------------------------------
    // ŞİRKET TİPLERİ
    // -------------------------------------------------------------------------

    public class TransporterCompany : Company
    {
        public TransporterCompany(string name, string taxNumber) : base(name, taxNumber) { }
        // Filo, Araçlar vb. buraya eklenecek
    }

    public class SupplierCompany : Company
    {
        public SupplierCompany(string name, string taxNumber) : base(name, taxNumber) { }
        // Terminaller vb. buraya eklenecek
    }

    // KURUMSAL MÜŞTERİ (Zincir Market/Restoran)
    public class CorporateCustomer : Company
    {
        // Bir şirketin N tane şubesi olabilir
        private readonly List<CustomerBranch> _branches = new();
        public IReadOnlyCollection<CustomerBranch> Branches => _branches.AsReadOnly();

        public CorporateCustomer(string name, string taxNumber) : base(name, taxNumber) { }

        public void AddBranch(string name, Location location)
        {
            _branches.Add(new CustomerBranch(Id, name, location));
        }
    }

    // ŞUBE (Fiziksel Mekan - Sipariş buradan verilir)
    public class CustomerBranch : Entity
    {
        public Guid CorporateCustomerId { get; private set; }
        public string Name { get; private set; } // "Kadıköy Şubesi"
        public Location Location { get; private set; } // Koordinat

        public CustomerBranch(Guid corporateCustomerId, string name, Location location)
        {
            CorporateCustomerId = corporateCustomerId;
            Name = name;
            Location = location;
        }
    }

    // BİREYSEL MÜŞTERİ (Ahmet Bey - Şirket Değil!)
    // Bu yüzden Company'den türemez. Direkt AppUser'a bağlıdır.
    public class IndividualCustomer : Entity, IAggregateRoot
    {
        public Guid AppUserId { get; private set; } // Direkt Kişi Bağlantısı

        // Bireysel müşterinin şubesi olmaz, adres defteri olur.
        private readonly List<PersonalAddress> _addresses = new();
        public IReadOnlyCollection<PersonalAddress> Addresses => _addresses.AsReadOnly();

        public IndividualCustomer(Guid appUserId)
        {
            Id = Guid.NewGuid();
            AppUserId = appUserId;
        }

        public void AddAddress(string title, Location location)
        {
            _addresses.Add(new PersonalAddress(Id, title, location));
        }
    }

    public class PersonalAddress : Entity
    {
        public Guid IndividualCustomerId { get; private set; }
        public string Title { get; private set; } // "Ev", "İş"
        public Location Location { get; private set; }

        public PersonalAddress(Guid customerId, string title, Location location)
        {
            IndividualCustomerId = customerId;
            Title = title;
            Location = location;
        }
    }

    // =========================================================================
    // 3. PRODUCT & INVENTORY (ÜRÜN VE STOK)
    // =========================================================================

    public class Product : Entity, IAggregateRoot
    {
        public Guid SupplierId { get; private set; }
        public string Name { get; private set; }

        private readonly List<Package> _packages = new();
        public IReadOnlyCollection<Package> Packages => _packages.AsReadOnly();

        public Product(Guid supplierId, string name)
        {
            Id = Guid.NewGuid();
            SupplierId = supplierId;
            Name = name;
        }
    }

    public class Package : Entity
    {
        public Guid ProductId { get; private set; }
        public string Name { get; private set; } // "6-Pack"
        public PackageType Type { get; private set; } // Pallet, Parcel, Piece

        // Hiyerarşi: 1 Palet = 50 Koli ise, AtomicQuantity = 50 (veya içindeki en küçük birim sayısı)
        public int AtomicQuantity { get; private set; }

        public bool IsReturnable { get; private set; }
        public Money DepositPrice { get; private set; }

        public Package(Guid productId, string name, PackageType type, int atomicQuantity)
        {
            ProductId = productId;
            Name = name;
            Type = type;
            AtomicQuantity = atomicQuantity;
        }
    }
    public enum PackageType { Pallet, Parcel, Piece }

    // TERMİNAL (Depo / Çıkış Noktası)
    // Hem Supplier'ın hem Transporter'ın terminali olabilir.
    public class Terminal : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public Location Location { get; private set; }

        public Guid? SupplierId { get; private set; }
        public Guid? TransporterId { get; private set; }

        public Terminal(string name, Location location, Guid? supplierId, Guid? transporterId)
        {
            Id = Guid.NewGuid();
            Name = name;
            Location = location;
            SupplierId = supplierId;
            TransporterId = transporterId;
        }
    }

    // 1. HIZLI ENVANTER (Sipariş anında stok kontrolü için)
    public class TerminalInventory : Entity
    {
        public Guid TerminalId { get; private set; }
        public Guid PackageId { get; private set; }
        public int TotalQuantity { get; private set; } // Toplam fiziksel stok
        public int ReservedQuantity { get; private set; } // Satılmış ama çıkmamış

        // Transporter deposunda başkasının malı durabilir (OwnerId)
        public Guid OwnerId { get; private set; } // Malın sahibi kim? (SupplierId)
    }

    // 2. DETAYLI LOKASYON (Depocu ürünü bulsun diye)
    public class InventoryLocation : Entity
    {
        public Guid TerminalInventoryId { get; private set; }
        public string Aisle { get; private set; } // Koridor A
        public string Shelf { get; private set; } // Raf 3
        public int Quantity { get; private set; }
    }

    // =========================================================================
    // 4. ORDER & SHIPMENT (SİPARİŞ VE SEVKİYAT)
    // =========================================================================

    public class Order : Entity, IAggregateRoot
    {
        // Siparişi veren "Customer" (Corporate veya Individual ID'si değil, soyut ID)
        // Burada işi basitleştirmek için: 
        // Eğer Kurumsalsa -> CustomerType=Corporate, CustomerId=BranchId
        // Eğer Bireyselse -> CustomerType=Individual, CustomerId=IndividualCustomerId
        public Guid CustomerId { get; private set; }
        public CustomerType CustomerType { get; private set; }

        public Guid SupplierId { get; private set; }

        // Malın teslim edileceği nokta (Snapshot)
        public Location DeliveryLocation { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public Order(Guid customerId, CustomerType type, Guid supplierId, Location deliveryLocation)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            CustomerType = type;
            SupplierId = supplierId;
            DeliveryLocation = deliveryLocation;
        }
    }

    public enum CustomerType { CorporateBranch, Individual }

    public class OrderItem : Entity
    {
        public Guid OrderId { get; private set; }
        public Guid PackageId { get; private set; }
        public int Quantity { get; private set; }
    }

    // =========================================================================
    // 5. LOGISTICS (FİLO VE ROTA)
    // =========================================================================

    public class Vehicle : Entity
    {
        public Guid TransporterId { get; private set; }
        public string PlateNumber { get; private set; }
        // ... Kapasite vb ...
    }

    // Araç - Sürücü Ataması (Vardiya)
    public class VehicleAssignment : Entity
    {
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; } // WorkerId (Rolü Driver olan)
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
    }

    public class Route : Entity, IAggregateRoot
    {
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; }
        public DateTime RouteDate { get; private set; }

        private readonly List<RouteTask> _tasks = new();
        public IReadOnlyCollection<RouteTask> Tasks => _tasks.AsReadOnly();
    }

    public class RouteTask : Entity
    {
        public Guid RouteId { get; private set; }
        public int Sequence { get; private set; }
        public TaskType Type { get; private set; }
        public Location TargetLocation { get; private set; }

        // Görev neyle ilgili?
        public Guid? OrderId { get; private set; } // Teslimat ise
        public Guid? ReturnRequestId { get; private set; } // İade ise
    }

    public enum TaskType { Delivery, ReturnPickup, DepositCheck }

    public class CorporateCustomer : Company // Base Company tablosuna bağlı
    {
        // Şubeler (Restoranlar)
        private readonly List<CustomerBranch> _branches = new();
        public IReadOnlyCollection<CustomerBranch> Branches => _branches.AsReadOnly();

        // Bu şirketi yöneten çalışanlar (Müdürler, Satın almacılar)
        private readonly List<CorporateWorker> _workers = new();
        public IReadOnlyCollection<CorporateWorker> Workers => _workers.AsReadOnly();

        public CorporateCustomer(string name, string taxNumber) : base(name, taxNumber) { }

        public void AddBranch(string name, Location location)
        {
            _branches.Add(new CustomerBranch(Id, name, location));
        }

        // Yeni çalışan ekleme (Örn: Bölge Müdürü)
        public void AddWorker(Guid appUserId, CorporateRole role)
        {
            _workers.Add(new CorporateWorker(Id, appUserId, role));
        }
    }

    // =========================================================================
    // ŞUBE (Fiziksel Mekan)
    // =========================================================================
    public class CustomerBranch : Entity
    {
        public Guid CorporateCustomerId { get; private set; }
        public string Name { get; private set; } // "Kadıköy Şubesi"
        public Location Location { get; private set; }

        public CustomerBranch(Guid corporateCustomerId, string name, Location location)
        {
            CorporateCustomerId = corporateCustomerId;
            Name = name;
            Location = location;
        }
    }

    // =========================================================================
    // KURUMSAL ÇALIŞAN (Yetkili Kişi)
    // =========================================================================
    public class CorporateWorker : Entity
    {
        public Guid CorporateCustomerId { get; private set; }
        public Guid AppUserId { get; private set; } // Login olan kişi
        public CorporateRole Role { get; private set; } // Admin, Bölge Müdürü vb.

        // *** İSTEDİĞİN ÖZELLİK BURADA ***
        // Bu çalışanın sorumlu olduğu şubeler listesi.
        // Eğer bu liste boşsa ve Rolü "Admin" ise -> Tüm şubeleri görür.
        // Eğer listede şube varsa -> Sadece o şubelerin siparişini görür/yönetir.
        private readonly List<WorkerBranchAssignment> _assignedBranches = new();
        public IReadOnlyCollection<WorkerBranchAssignment> AssignedBranches => _assignedBranches.AsReadOnly();

        public CorporateWorker(Guid corporateCustomerId, Guid appUserId, CorporateRole role)
        {
            CorporateCustomerId = corporateCustomerId;
            AppUserId = appUserId;
            Role = role;
        }

        // Çalışana şube atama metodu
        public void AssignBranch(Guid branchId)
        {
            if (!_assignedBranches.Any(x => x.BranchId == branchId))
            {
                _assignedBranches.Add(new WorkerBranchAssignment(Id, branchId));
            }
        }
    }

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
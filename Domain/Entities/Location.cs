using Domain;
using Domain.Entities;
using Domain.SeedWork;
using NetTopologySuite.Geometries;
using System.Drawing;

namespace Domain
{
    // =========================================================================
    // 1. SHARED KERNEL (ORTAK DEĞER NESNELERİ)
    // =========================================================================

    // Coğrafi Konum (PostGIS)
    public class Location : ValueObject
    {
        public Point Point { get; private set; } // SRID 4326 (GPS Koordinatı)
        public string FormattedAddress { get; private set; } // "Atatürk Mah. No:5..."
        public string City { get; private set; }
        public string District { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents() { yield return Point; }
    }

    // Fiziksel Boyutlar (Lojistik Planlama için Kritik)
    public class Dimensions : ValueObject
    {
        public double Width { get; private set; }
        public double Height { get; private set; }
        public double Depth { get; private set; }
        public double VolumeM3 => Width * Height * Depth; // Hacim hesabı için

        protected override IEnumerable<object> GetEqualityComponents() { yield return VolumeM3; }
    }

    // Para Birimi (Ödemeler ve Hesaplamalar için)
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } // "TRY", "USD"

        protected override IEnumerable<object> GetEqualityComponents() { yield return Amount; yield return Currency; }
    }

    // Çalışma Saatleri (Driver/Worker Müsaitliği için)
    public class WorkSchedule : ValueObject
    {
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public List<DayOfWeek> WorkingDays { get; private set; } // [Monday, Tuesday...]

        protected override IEnumerable<object> GetEqualityComponents() { yield return StartTime; }
    }

    // =========================================================================
    // 2. AUTH INTEGRATION (KİMLİK ENTEGRASYONU)
    // =========================================================================

    // IdentityAPI'den gelen kullanıcının yansıması
    public class User : Entity
    {
        public Guid IdentityId { get; private set; } // Auth servisindeki ID
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public string PhoneNumber { get; private set; }
    }

    // =========================================================================
    // 3. PRODUCT CATALOG (ÜRÜN VE PAKET YÖNETİMİ)
    // =========================================================================

    // "Coca-Cola" (Ana Ürün)
    public class Product : Entity, IAggregateRoot
    {
        public Guid SupplierId { get; private set; } // Hangi tedarikçinin?
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Brand { get; private set; }

        // Ürüne bağlı paketler (1'li, 6'lı, 24'lü)
        private readonly List<Package> _packages = new();
        public IReadOnlyCollection<Package> Packages => _packages.AsReadOnly();
    }

    // "6'lı Şrink Kola" (Satılabilir Birim)
    public class Package : Entity
    {
        public Guid ProductId { get; private set; }
        public string Name { get; private set; } // Örn: "6-Pack"
        public string Sku { get; private set; } // Stok Kodu
        public string Barcode { get; private set; }
        public PackageType Type { get; private set; }
        public int AtomicQuantity { get; private set; }

        public int QuantityInPackage { get; private set; } // İçinde kaç adet Product var?

        public Dimensions Dimensions { get; private set; } // Hacim hesabı için
        public double WeightKg { get; private set; } // Ağırlık hesabı için

        // İade ve Depozito Ayarları
        public bool IsOrderable { get; private set; }
        public bool IsReturnable { get; private set; } // Geri iade edilebilir mi?
        public Money DepositPrice { get; private set; } // Depozito ücreti var mı?
    }

    public enum PackageType
    {
        Pallet,
        Parcel,
        Palleremme,
        Piece
    }

    // =========================================================================
    // 4. PARTIES (TARAFLAR: TEDARİKÇİ, MÜŞTERİ, TAŞIYICI)
    // =========================================================================

    public class Department : Entity
    {
        // Bu departman ya bir Supplier'a ya da Transporter'a aittir.
        public Guid? SupplierId { get; private set; }
        public Guid? TransporterId { get; private set; }

        public string Name { get; private set; } // "Depo Yönetimi", "Filo Yönetimi"

        // Bir departmana bağlı çalışanlar
        private readonly List<Worker> _workers = new();
        public IReadOnlyCollection<Worker> Workers => _workers.AsReadOnly();
    }

    public class Worker : Entity
    {
        public Guid DepartmentId { get; private set; }
        public Guid IdentityUserId { get; private set; } // Auth bağlantısı burada!

        public string FullName { get; private set; }
        public WorkerRole Role { get; private set; } // Manager, Staff, Operator
    }

    public enum WorkerRole
    {
        Manager,
        Operator,
        WarehouseStaff
    }

    // TEDARİKÇİ
    public class Supplier : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public string TaxNumber { get; private set; }

        // Ayarlar
        public int MinDeliveryHours { get; private set; } // Min teslim süresi
        public Guid? AutoAssignTransporterId { get; private set; } // Otomatik taşıyıcı

        private readonly List<Department> _departments = new();
        public IReadOnlyCollection<Department> Departments => _departments.AsReadOnly();

        private readonly List<SupplierCustomerRelation> _customerRelations = new();
        public IReadOnlyCollection<SupplierCustomerRelation> CustomerRelations => _customerRelations.AsReadOnly();

        private readonly List<SupplierTransporterRelation> _transporterRelations = new();
        public IReadOnlyCollection<SupplierTransporterRelation> TransporterRelations => _transporterRelations.AsReadOnly();

        // Terminaller (Depolar)
        private readonly List<Terminal> _terminals = new();
        public IReadOnlyCollection<Terminal> Terminals => _terminals.AsReadOnly();
    }

    // TEDARİKÇİ TERMİNALİ (Çıkış Noktası)
    public class Terminal : Entity
    {
        public Guid SupplierId { get; private set; }
        public string Name { get; private set; }
        public Location Location { get; private set; } // PostGIS: En yakın terminal hesabı için

        // Stok (Envanter)
        private readonly List<InventoryItem> _inventory = new();
        public IReadOnlyCollection<InventoryItem> Inventory => _inventory.AsReadOnly();
    }

    // STOK KALEMİ
    public class InventoryItem : Entity
    {
        public Guid TerminalId { get; private set; }
        public Guid PackageId { get; private set; }
        public int QuantityOnHand { get; private set; } // Eldeki fiziksel stok
        public int QuantityReserved { get; private set; } // Sipariş gelmiş, ayrılmış stok
    }

    // MÜŞTERİ (Restoran, Market)
    public class Customer : Entity, IAggregateRoot
    {
        public Guid IdentityUserId { get; private set; } // Login olan kullanıcı
        public string CompanyName { get; private set; }
        public CustomerType Type { get; private set; } // Restaurant, Market, Individual

        // Bakiye (Depozito iadeleri vb. için)
        public Money CurrentBalance { get; private set; }

        // Adresler
        private readonly List<CustomerAddress> _addresses = new();
        public IReadOnlyCollection<CustomerAddress> Addresses => _addresses.AsReadOnly();
    }

    public enum CustomerType { Individual, Restaurant, Market, ChainStore }

    public class CustomerAddress : Entity
    {
        public Guid CustomerId { get; private set; }
        public string Title { get; private set; } // "Merkez Şube", "Depo 1"
        public Location Location { get; private set; }
    }

    // TAŞIYICI (Lojistik Firması)
    public class Transporter : Entity, IAggregateRoot
    {
        public string Name { get; private set; }

        private readonly List<Vehicle> _vehicles = new();
        private readonly List<Driver> _drivers = new();
        private readonly List<Department> _departments = new();
        public IReadOnlyCollection<Vehicle> Vehicles => _vehicles.AsReadOnly();
        public IReadOnlyCollection<Driver> Drivers => _drivers.AsReadOnly();
        public IReadOnlyCollection<Department> Departments => _departments.AsReadOnly();
    }

    public class SupplierCustomerRelation : Entity
    {
        public Guid SupplierId { get; private set; }
        public Guid CustomerId { get; private set; }
    }

    public class SupplierTransporterRelation : Entity
    {
        public Guid SupplierId { get; private set; }
        public Guid TransporterId { get; private set; }
        // Örn: Sözleşme bitiş tarihi, öncelik sırası vb.
    }

    public class Vehicle : Entity
    {
        public string PlateNumber { get; private set; }
        public VehicleType Type { get; private set; } // Truck, Van, Motorcycle

        // Kapasite (Planlama için)
        public double MaxWeightKg { get; private set; }
        public double MaxVolumeM3 { get; private set; }

        // Canlı Takip (GPS Entegrasyonu)
        public Location LastKnownLocation { get; private set; }
        public DateTime LastLocationUpdate { get; private set; }
        public VehicleStatus Status { get; private set; }
    }

    public enum VehicleType { Motorcycle, Van, Truck }
    public enum VehicleStatus { Active, Maintenance, InTransit, Offline }

    public class Driver : Entity
    {
        public Guid IdentityUserId { get; private set; } // Login olan şoför
        public string LicenseNumber { get; private set; }
        public WorkSchedule WorkSchedule { get; private set; } // Vardiya
        public bool IsAvailable { get; private set; }

        private readonly List<VehicleAssignment> _assignments = new();
        public IReadOnlyCollection<VehicleAssignment> Assignments => _assignments.AsReadOnly();
    }

    public class VehicleAssignment : Entity
    {
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        // İsteğe bağlı: Periyodik mi? (Her Pazartesi 09:00-17:00)
        public bool IsRecurring { get; private set; }
        public DayOfWeek? RecurDay { get; private set; }
    }
}

    // =========================================================================
    // 5. ORDERING (SİPARİŞ SÜRECİ)
    // =========================================================================

    public class Order : Entity, IAggregateRoot
    {
        public Guid CustomerId { get; private set; }
        public Guid SupplierId { get; private set; }

        // PostGIS ile hesaplanıp seçilen çıkış noktası
        public Guid SourceTerminalId { get; private set; }
        public Guid ShippingAddressId { get; private set; } // Referans (Analytics için)
        public Location ShippingAddressSnapshot { get; private set; } // Değer (Değişmezlik için)

        public OrderStatus Status { get; private set; }
        public DateTime OrderDate { get; private set; }
        public DateTime? RequestedDeliveryDate { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public Money TotalAmount { get; private set; }
    }

    public class OrderItem : Entity
    {
        public Guid PackageId { get; private set; } // DİKKAT: Product değil Package
        public int Quantity { get; private set; }
        public Money UnitPrice { get; private set; }
    }

    public enum OrderStatus { Draft, Pending, Confirmed, Shipped, Delivered, Cancelled }

    // =========================================================================
    // 6. LOGISTICS & FULFILLMENT (PLANLAMA VE DAĞITIM)
    // =========================================================================

    // Bir siparişin taşınabilir hali. (Bir sipariş parçalanabilir veya birleşebilir)
    public class Shipment : Entity, IAggregateRoot
    {
        public Guid OrderId { get; private set; }
        public Guid SourceTerminalId { get; private set; }
        public Location Destination { get; private set; }

        // Atama
        public Guid? AssignedTransporterId { get; private set; }

        public ShipmentStatus Status { get; private set; }

        // Bu shipment içinde hangi paketlerden kaç tane var?
        private readonly List<ShipmentItem> _items = new();
    }

    public class ShipmentItem : Entity { public Guid PackageId; public int Quantity; }
    public enum ShipmentStatus { Pending, Planned, Loaded, InTransit, Delivered, Failed }

    // Transporter'ın yaptığı günlük plan (Task Creation Ekranı)
    public class DeliveryPlan : Entity, IAggregateRoot
    {
        public Guid TransporterId { get; private set; }
        public DateTime PlanDate { get; private set; }

        // Bir plan birden fazla rotadan oluşur (Her araç için bir rota)
        private readonly List<Route> _routes = new();
        public IReadOnlyCollection<Route> Routes => _routes.AsReadOnly();
    }

    // Bir aracın/şoförün izleyeceği yol
    public class Route : Entity
    {
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; }

        // Rota üzerindeki görevler (Sıralı)
        private readonly List<RouteTask> _tasks = new();
        public IReadOnlyCollection<RouteTask> Tasks => _tasks.AsReadOnly();

        public RouteStatus Status { get; private set; }

        // Rota özeti
        public double TotalDistanceKm { get; private set; }
        public TimeSpan EstimatedDuration { get; private set; }
    }

    public enum RouteStatus { Draft, Approved, Started, Completed }

    // Rota üzerindeki her bir durak (Görev)
    public class RouteTask : Entity
    {
        public int SequenceNumber { get; private set; } // 1, 2, 3... (Sıralama algoritması belirler)
        public TaskType Type { get; private set; }
        public Location TargetLocation { get; private set; }

        // Hangi Shipment veya İade ile ilgili?
        public Guid? RelatedShipmentId { get; private set; }
        public Guid? RelatedReturnRequestId { get; private set; }

        public TaskStatus Status { get; private set; }
        public DateTime? CompletionTime { get; private set; }
    }

    public enum TaskType { Delivery, ReturnPickup, DepositPickup }
    public enum TaskStatus { Pending, Arrived, Completed, Failed, Skipped }

    // =========================================================================
    // 7. RETURNS & DEPOSITS (İADE VE DEPOZİTO)
    // =========================================================================

    // Müşterinin oluşturduğu iade talebi
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
}
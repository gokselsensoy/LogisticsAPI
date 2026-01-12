using Domain.Entities;
using Domain.Entities.Companies;
using Domain.Entities.Customers;
using Domain.Entities.Departments;
<<<<<<< HEAD
using Domain.Entities.Inventories;
using Domain.Entities.Orders;
=======
using Domain.Entities.Inventory;
using Domain.Entities.Order;
using Domain.Entities.Subscriptions;
>>>>>>> 4952e842f75477b0a804ce86a86615304aac01d9
using Domain.Entities.Task;
using Domain.Entities.WorkSchedule;
using Domain.SeedWork;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using System.Reflection;

namespace Infrastructure.Persistence.Context
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IPublisher _publisher;
        private IDbContextTransaction? _currentTransaction;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher publisher) : base(options)
        {
            _publisher = publisher;
        }

        #region UnitOfWork
        public bool HasActiveTransaction => _currentTransaction != null;

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                // Zaten bir transaction varsa (iç içe çağrılırsa) bir şey yapma
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction == null)
                    throw new InvalidOperationException("Aktif bir transaction bulunamadı.");

                await _currentTransaction.CommitAsync(cancellationToken);
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction == null)
                    throw new InvalidOperationException("Aktif bir transaction bulunamadı.");

                await _currentTransaction.RollbackAsync(cancellationToken);
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await PublishDomainEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }
        #endregion

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Transporter> Transporters { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Freelancer> Freelancers { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<CorporateCustomer> CorporateCustomers { get; set; }
        public DbSet<IndividualCustomer> IndividualCustomers { get; set; }
        public DbSet<CorporateResponsible> CorporateResponsibles { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<CorporateAddressResponsibleMap> CorporateAddressResponsibleMap { get; set; }

        public DbSet<Department> Departments { get; set; }
        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Worker> Workers { get; set; }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ReturnRequest> ReturnRequests { get; set; }
        public DbSet<ReturnItem> ReturnItems { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentItem> ShipmentItems { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }

        public DbSet<WeeklyShiftPattern> WeeklyShiftPatterns { get; set; }
        public DbSet<ShiftPatternItem> ShiftPatternItems { get; set; }
        public DbSet<DailyWorkSchedule> DailyWorkSchedules { get; set; }
        public DbSet<ScheduleAllocation> ScheduleAllocations { get; set; }

        public DbSet<DeliveryPlan> DeliveryPlans { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteTask> RouteTasks{ get; set; }

        public DbSet<AppUserSubscription> AppUserSubscriptions { get; set; }
        public DbSet<PlanFeatureCache> PlanFeatureCache { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);

            // *** GLOBAL QUERY FILTER (SOFT DELETE) ***
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDeletableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    if (entityType.BaseType == null)
                    {
                        var parameter = Expression.Parameter(entityType.ClrType, "e");
                        var propertyMethodInfo = typeof(EF).GetMethod("Property")?.MakeGenericMethod(typeof(bool));
                        var isDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IsDeleted"));

                        // IsDeleted == true görülmek istendiği zaman sorguda IgnoreQueryFilters eklenecek.
                        // e => e.IsDeleted == false
                        BinaryExpression compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(false));
                        var lambda = Expression.Lambda(compareExpression, parameter);

                        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                    }
                }
            }
        }

        private async Task PublishDomainEventsAsync(CancellationToken cancellationToken)
        {
            // Takip edilen (tracked) Entity'leri bul
            var domainEntities = ChangeTracker
                .Entries<Entity>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents.Any())
                .ToList();

            // Tüm event'leri topla
            var domainEvents = domainEntities
                .SelectMany(entity => entity.DomainEvents)
                .ToList();

            // Event'leri temizle ki tekrar fırlatılmasın
            domainEntities.ForEach(entity => entity.ClearDomainEvents());

            // MediatR aracılığıyla event'leri "yayınla" (publish)
            // Bu event'leri dinleyen Handler'lar (örn: OrderCreatedEmailHandler) çalışır
            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
        }
    }
}

using Application.Abstractions.EntityRepositories;
using Application.Abstractions.Repositories;
using Domain.Repositories;
using Domain.SeedWork;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.QueryRepositories;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), o => o.UseNetTopologySuite()));

            services.AddScoped<IUnitOfWork>(sp =>
                sp.GetRequiredService<ApplicationDbContext>());

            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ITransporterRepository, TransporterRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IFreelancerRepository, FreelancerRepository>();

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ICorporateCustomerRepository, CorporateCustomerRepository>();
            services.AddScoped<ICorporateResponsibleRepository, CorporateResponsibleRepository>();
            services.AddScoped<IIndividualCustomerRepository, IndividualCustomerRepository>();

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IWorkerRepository, WorkerRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();

            services.AddScoped<IWeeklyShiftPatternRepository, WeeklyShiftPatternRepository>();
            services.AddScoped<IDailyWorkScheduleRepository, DailyWorkScheduleRepository>();

            services.AddScoped<ITerminalRepository, TerminalRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IShipmentRepository, ShipmentRepository>();

            services.AddScoped<IBasketRepository, BasketRepository>();

            services.AddScoped<IOrderQueryRepository, OrderQueryRepository>();
            services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
            services.AddScoped<IProductQueryRepository, ProductQueryRepository>();
            services.AddScoped<ISupplierQueryRepository, SupplierQueryRepository>();
            services.AddScoped<IBasketQueryRepository, BasketQueryRepository>();

            services.AddScoped<IAppUserSubscription, AppUserSubscriptionRepository>();
            services.AddScoped<IPlanFeatureCacheRepository,PlanFeatureCacheRepository>();


            // 5. Diğer servisler (Email vb.)
            // services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}

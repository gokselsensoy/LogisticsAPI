using Application.Abstractions.Services;
using Integration.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Integration.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIntegrationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // services.AddTransient<IEmailService, SendGridEmailService>();
            services.AddTransient<IPhotoUploader, GoogleDrivePhotoUploader>();
            
            services.AddHttpClient<GoogleDrivePhotoUploader>(client =>
            {
                // client.BaseAddress = new Uri(configuration["GoogleApiSettings:BaseUrl"]);
            });

            services.AddHttpClient<ISubOrbitService, SubOrbitService>();
            services.AddTransient<S2STokenHandler>();

            services.AddHttpClient<IIdentityService, HttpIdentityService>(client =>
            {
                client.BaseAddress = new Uri(configuration["Auth:Authority"]);
            })
            .AddHttpMessageHandler<S2STokenHandler>();

            return services;
        }
    }
}

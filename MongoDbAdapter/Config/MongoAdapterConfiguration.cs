using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.Shared;

namespace MongoDbAdapter.Config
{
    public static class MongoAdapterConfiguration
    {
        public static void ConfigureMongoDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<NotificationStoreDatabaseSettings>(_ =>
                configuration.GetSection(nameof(NotificationStoreDatabaseSettings)));

            services.AddScoped<IStore<Notification>, NotificationStore>();
        }
    }
}

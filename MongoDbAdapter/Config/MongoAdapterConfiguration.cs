using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDbAdapter.DataAccess;
using MongoDbAdapter.DataAccess.Base;
using NotificationService.Shared;

namespace MongoDbAdapter.Config;

public static class MongoAdapterConfiguration
{
    public static void ConfigureMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(opts =>
        {
            var dbSettings = configuration
                .GetRequiredSection(nameof(DatabaseSettings))
                .GetChildren()
                .ToArray();

            opts.CollectionName = dbSettings[0].Value;
            opts.ConnectionString = dbSettings[1].Value;
            opts.DatabaseName = dbSettings[2].Value;
        });

        services.AddScoped<IStore<Notification>, NotificationStore>();

        BsonClassMap.RegisterClassMap<Notification>(cm =>
        {
            cm.AutoMap();
            cm.SetIgnoreExtraElements(true);
        });
    }
}

using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbAdapter.Config;
using MongoDbAdapter.DataAccess.Base;
using NotificationService.Shared;

namespace MongoDbAdapter.DataAccess;

public class NotificationStore : Store<Notification>
{
    public NotificationStore(IOptions<DatabaseSettings> dbOptions)
        : base(dbOptions)
    {
        var expireAtIndex = new CreateIndexModel<Notification>(
            keys: Builders<Notification>.IndexKeys.Ascending(n => n.ExpiresAt),
            options: new CreateIndexOptions
            {
                ExpireAfter = TimeSpan.FromSeconds(0),
                Name = "ExpireAtIndex"
            });

        Collection.Indexes.CreateOneAsync(expireAtIndex);
    }


    public override async Task<IEnumerable<Notification>> GetAllAsync() =>
        await Collection.Find(x => x.ClientId == null).ToListAsync();
}
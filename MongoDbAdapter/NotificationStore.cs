using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.Shared;
using System.Linq.Expressions;
using MongoDbAdapter.Config;

namespace MongoDbAdapter
{
    public class NotificationStore : IStore<Notification>
    {
        readonly IMongoCollection<Notification> _collection;

        public NotificationStore(IOptions<NotificationStoreDatabaseSettings> notificationDatabaseSettings)
        {
            var settings = MongoClientSettings.FromConnectionString(notificationDatabaseSettings.Value.ConnectionString);
            var mongoClient = new MongoClient(settings);

            var mongoDatabase = mongoClient.GetDatabase(notificationDatabaseSettings.Value.DatabaseName);

            _collection = mongoDatabase.GetCollection<Notification>(notificationDatabaseSettings.Value.CollectionName);
        }


        public async Task<List<Notification>> GetAsync() =>
            await _collection.Find(x => x.ClientId == null).ToListAsync();

        public async Task<Notification?> GetAsync(Expression<Func<Notification, bool>> filter) =>
            (await _collection.FindAsync(filter)).First();

        public async Task CreateAsync(Notification newEntity) =>
            await _collection.InsertOneAsync(newEntity);

        public Task UpdateAsync(Guid id, Notification updatedEntity) =>
            throw new NotImplementedException();

        public Task RemoveAsync(Guid id) =>
            throw new NotImplementedException();
    }
}

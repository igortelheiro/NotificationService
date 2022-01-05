using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDbAdapter.Config;
using NotificationService.Shared;

namespace MongoDbAdapter.DataAccess.Base
{
    public abstract class Store<TEntity> : IStore<TEntity> where TEntity : IEntity
    {
        protected readonly IMongoCollection<TEntity> Collection;

        protected Store(IOptions<DatabaseSettings> dbOptions)
        {
            var dbSettings = dbOptions.Value;

            var mongoSettings = MongoClientSettings.FromConnectionString(dbSettings.ConnectionString);
            var mongoClient = new MongoClient(mongoSettings);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.DatabaseName);

            Collection = mongoDatabase.GetCollection<TEntity>(dbSettings.CollectionName);
        }


        public abstract Task<IEnumerable<TEntity>> GetAllAsync();

        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter) =>
            await Collection.Find(filter).ToListAsync();

        public virtual async Task CreateAsync(TEntity newEntity) =>
            await Collection.InsertOneAsync(newEntity);

        public virtual async Task RemoveAsync(Guid id) =>
            await Collection.DeleteOneAsync(e => e.Id == id);

        public virtual async Task UpdateAsync(Guid id, TEntity updatedEntity) =>
            await Collection.ReplaceOneAsync(e => e.Id == id, updatedEntity);
    }
}

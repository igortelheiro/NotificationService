using System.Linq.Expressions;

namespace MongoDbAdapter
{
    public interface IStore<TEntity> where TEntity : class
    {
        public Task<List<TEntity>> GetAsync();

        public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter);

        public Task CreateAsync(TEntity newEntity);

        public Task UpdateAsync(Guid id, TEntity updatedEntity);

        public Task RemoveAsync(Guid id);
    }
}

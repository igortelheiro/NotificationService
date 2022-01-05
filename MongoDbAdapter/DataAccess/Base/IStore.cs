using System.Linq.Expressions;
using NotificationService.Shared;

namespace MongoDbAdapter.DataAccess.Base
{
    public interface IStore<TEntity> where TEntity : IEntity
    {
        public Task<IEnumerable<TEntity>> GetAllAsync();

        public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter);

        public Task CreateAsync(TEntity newEntity);

        public Task UpdateAsync(Guid id, TEntity updatedEntity);

        public Task RemoveAsync(Guid id);
    }
}

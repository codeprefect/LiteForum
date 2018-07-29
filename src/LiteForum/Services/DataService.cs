using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LiteForum.Data.Interfaces;
using LiteForum.Entities.Interfaces;
using LiteForum.Services.Interfaces;

namespace LiteForum.Services
{
    public abstract class DataService<TContext, TEntity> : IDataService<TContext, TEntity>
        where TEntity : class, IEntity
    {
        private readonly IRepository<TContext> _repository;

        public DataService(IRepository<TContext> repository) => _repository = repository;

        #region just all the getters

        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null, int? skip = null, int? take = null)
                => await _repository.GetAsync<TEntity>(filter, orderBy, includeProperties, skip, take);

        public async Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>,
            IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = null,
            int? skip = null, int? take = null)
                => await _repository.GetAllAsync<TEntity>(orderBy, includeProperties, skip, take);

        public async Task<TEntity> GetByIdAsync(object id)
            => await _repository.GetByIdAsync<TEntity>(id);

        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter)
            => await _repository.GetCountAsync<TEntity>(filter);

        public async Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> filter)
            => await _repository.GetExistsAsync<TEntity>(filter);

        public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>>
            filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null)
                => await _repository.GetFirstAsync<TEntity>(filter, orderBy, includeProperties);

        public async Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null)
                => await _repository.GetOneAsync<TEntity>(filter, includeProperties);

        #endregion end of getters

        #region create, update, delete and save
        public void Create(TEntity entity, string createdBy = null)
            => _repository.Create<TEntity>(entity, createdBy);

        public void Delete(object id, string deletedBy = null) => _repository.Delete<TEntity>(id, deletedBy);

        public void Delete(TEntity entity, string deletedBy = null) => _repository.Delete<TEntity>(entity, deletedBy);

        public void Update(TEntity entity, string modifiedBy = null)
            => _repository.Update<TEntity>(entity, modifiedBy);

        public Task SaveAsync() => _repository.SaveAsync();

        #endregion
    }
}

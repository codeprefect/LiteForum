using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CP.Repositories.Interfaces;
using CP.Entities.Interfaces;

namespace LiteForum.Services {
    public class DataWithUserService<TContext, TEntity> : DataService<TContext, TEntity> where TEntity : class, IEntity {
        public DataWithUserService (IRepository<TContext> repository) : base (repository) { }

        #region just all the getters

        public override async Task<IEnumerable<TEntity>> GetAsync (Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null, int? skip = null, int? take = null) {
            var properties = string.IsNullOrEmpty (includeProperties) ? "User" : $"User,{includeProperties}";
            return await base.GetAsync (filter, orderBy, properties, skip, take);
        }

        public new async Task<IEnumerable<TEntity>> GetAllAsync (Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = null,
            int? skip = null, int? take = null) {
            var properties = string.IsNullOrEmpty (includeProperties) ? "User" : $"User, {includeProperties}";
            return await base.GetAllAsync (orderBy, properties, skip, take);
        }

        public new async Task<TEntity> GetFirstAsync (Expression<Func<TEntity, bool>>
            filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = null) {
            var properties = string.IsNullOrEmpty (includeProperties) ? "User" : $"User, {includeProperties}";
            return await base.GetFirstAsync (filter, orderBy, properties);
        }

        public new async Task<TEntity> GetOneAsync (Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = null) {
            var properties = string.IsNullOrEmpty (includeProperties) ? "User" : $"User, {includeProperties}";
            return await base.GetOneAsync (filter, properties);
        }

        #endregion end of getters
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LiteForum.Data.Interfaces;
using LiteForum.Entities.Interfaces;
using Microsoft.Extensions.Logging;

namespace LiteForum.Data.Repositories
{
    public class Repository<TContext> : ReadOnlyRepository<TContext>, IRepository<TContext> where TContext : DbContext
    {
        public Repository(TContext context, ILogger<Repository<TContext>> logger) : base(context, logger)
        { }

        public virtual void Create<TEntity>(TEntity entity, string createdBy = null) where TEntity : class, IEntity
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.CreatedBy = createdBy;
            _context.Set<TEntity>().Add(entity);
        }

        public virtual void Update<TEntity>(TEntity entity, string modifiedBy = null) where TEntity : class, IEntity
        {
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ModifiedBy = modifiedBy;
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete<TEntity>(object id, string deletedBy = null) where TEntity : class, IEntity
        {
            TEntity entity = _context.Set<TEntity>().Find(id);
            Delete(entity, deletedBy);
        }

        public virtual void Delete<TEntity>(TEntity entity, string deletedBy = null) where TEntity : class, IEntity
        {
            var dbSet = _context.Set<TEntity>();
            entity.Deleted = DateTime.UtcNow;
            entity.ModifiedBy = deletedBy;
            if (_context.Entry(entity).State == EntityState.Modified)
            {
                dbSet.Attach(entity);
            }
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
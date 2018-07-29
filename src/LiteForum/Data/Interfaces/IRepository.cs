using System.Threading.Tasks;
using LiteForum.Entities.Interfaces;

namespace LiteForum.Data.Interfaces
{
    public interface IRepository<TContext> : IReadOnlyRepository
    {
        TEntity Create<TEntity>(TEntity entity, string createdBy = null)
            where TEntity : class, IEntity;

        TEntity Update<TEntity>(TEntity entity, string modifiedBy = null)
            where TEntity : class, IEntity;

        void Delete<TEntity>(object id, string deletedBy = null)
            where TEntity : class, IEntity;

        void Delete<TEntity>(TEntity entity, string deletedBy = null)
            where TEntity : class, IEntity;

        Task SaveAsync();
    }
}

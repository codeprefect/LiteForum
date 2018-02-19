using System.Threading.Tasks;
using LiteForum.Entities.Interfaces;

namespace LiteForum.Data.Interfaces
{
    public interface IRepository<TContext> : IReadOnlyRepository
    {
        void Create<TEntity>(TEntity entity, string createdBy = null)
            where TEntity : class, IEntity;

        void Update<TEntity>(TEntity entity, string modifiedBy = null)
            where TEntity : class, IEntity;

        void Delete<TEntity>(object id)
            where TEntity : class, IEntity;

        void Delete<TEntity>(TEntity entity)
            where TEntity : class, IEntity;

        Task SaveAsync();
    }
}
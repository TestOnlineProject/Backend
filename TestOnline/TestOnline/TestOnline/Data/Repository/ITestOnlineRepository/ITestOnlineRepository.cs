using System.Linq.Expressions;
using TestOnline.Models.Entities;

namespace TestOnline.Data.Repository.ITestOnlineRepository
{
    public interface ITestOnlineRepository<Tentity> where Tentity : class
    {
        IQueryable<Tentity> GetByCondition(Expression<Func<Tentity, bool>> expression);

        IQueryable<Tentity> GetAll();

        IQueryable<Tentity> GetById(Expression<Func<Tentity, bool>> expression);

        void Create(Tentity entity);
        void CreateRange(List<Tentity> entity);

        void Delete(Tentity entity);
        void DeleteRange(List<Tentity> entity);

        void Update(Tentity entity);
        void UpdateRange(List<Tentity> entity);
    }
}

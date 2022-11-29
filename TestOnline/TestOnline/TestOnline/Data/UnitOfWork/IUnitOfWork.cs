using TestOnline.Data.Repository.ITestOnlineRepository;

namespace TestOnline.Data.UnitOfWork
{
    public interface IUnitOfWork
    {
        public ITestOnlineRepository<TEntity> Repository<TEntity>() where TEntity : class;
        bool Complete();
    }
}

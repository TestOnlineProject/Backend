using System.Collections;
using TestOnline.Data.Repository;
using TestOnline.Data.Repository.ITestOnlineRepository;

namespace TestOnline.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        private Hashtable _repositories;

        public UnitOfWork(DataContext context)
        {
            _context = context;
        }
        public bool Complete()
        {
            var numberOfAffectedRows = _context.SaveChanges();
            return numberOfAffectedRows > 0;
        }

        public ITestOnlineRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null) _repositories = new Hashtable();

            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(TestOnlineRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (ITestOnlineRepository<TEntity>)_repositories[type];
        }
    }
}

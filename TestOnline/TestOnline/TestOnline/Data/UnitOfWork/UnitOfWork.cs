using System.Collections;
using TestOnline.Data.Repository;
using TestOnline.Data.Repository.ITestOnlineRepository;

namespace TestOnline.Data.UnitOfWork
{
    //The unit of work class serves one purpose: to make sure that when you use multiple repositories,
    //they share a single database context.
    //That way, when a unit of work is complete you can call the SaveChanges method on that instance
    //of the context and be assured that all related changes will be coordinated.
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

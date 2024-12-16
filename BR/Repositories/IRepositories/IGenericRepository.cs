using System.Linq.Expressions;

namespace BR.Repositories.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression);
        Task<T> GetById(long id);
        Task<T?> GetOne(Expression<Func<T, bool>> expression);
        void Update(T t);
        void Delete(T t);
        void Add(T t);
        Task<T> AddAndReturn(T entity);
        void SaveAsync();
    }
}

using System.Linq.Expressions;

namespace BR.Repositories.IRepositories
{
    public interface IQueryRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(int skip, int take);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression, int skip, int take);
        Task<T?> GetOne(Expression<Func<T, bool>> expression);
        Task<T?> GetById(long Id);
        Task<T?> GetById(int Id);
    }
}

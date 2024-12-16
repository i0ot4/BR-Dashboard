using BR.Models;
using System.Linq.Expressions;

namespace BR.Repositories.IRepositories
{
    public interface IPreviousWorkRepository : IGenericRepository<PreviousWork>
    {
        Task<IEnumerable<PreviousWork>> GetAllWithOwner();
        Task<IEnumerable<PreviousWork>> GetAllWithOwner(Expression<Func<PreviousWork, bool>> expression);
    }
}

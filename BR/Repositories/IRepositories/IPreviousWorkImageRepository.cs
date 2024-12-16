using BR.Models;
using System.Linq.Expressions;

namespace BR.Repositories.IRepositories
{
    public interface IPreviousWorkImageRepository : IGenericRepository<PreviousWorkImage>
    {
        Task<IEnumerable<PreviousWorkImage>> GetAllWithOwner();
        Task<IEnumerable<PreviousWorkImage>> GetAllWithOwner(Expression<Func<PreviousWorkImage, bool>> expression);
    }
}

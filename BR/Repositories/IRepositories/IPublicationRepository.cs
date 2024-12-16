using BR.helper;
using BR.Models;
using System.Linq.Expressions;

namespace BR.Repositories.IRepositories
{
    public interface IPublicationRepository: IGenericRepository<Publication>
    {
        Task<IEnumerable<Publication>> GetAllWithOwner();
        Task<IEnumerable<Publication>> GetAllWithOwner(Expression<Func<Publication, bool>> expression);
    }
}

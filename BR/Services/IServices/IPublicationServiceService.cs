using BR.helper;
using BR.Models;
using System.Linq.Expressions;

namespace BR.Services.IServices
{
    public interface IPublicationService : IGenericQueryService<Publication>, IGenericWriteService<Publication>
    {
        Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<Publication>>> GetAllWithOwner(Expression<Func<Publication, bool>> expression);
        //Task<IResult<City>> AddOrEdit(City entity, CancellationToken cancellationToken = default);
    }
}

using BR.helper;
using BR.Models;
using System.Linq.Expressions;

namespace BR.Services.IServices
{
    public interface IPreviousWorkService : IGenericQueryService<PreviousWork>, IGenericWriteService<PreviousWork>
    {
        Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<PreviousWork>>> GetAllWithOwner(Expression<Func<PreviousWork, bool>> expression);
        //Task<IResult<City>> AddOrEdit(City entity, CancellationToken cancellationToken = default);
    }
    
}

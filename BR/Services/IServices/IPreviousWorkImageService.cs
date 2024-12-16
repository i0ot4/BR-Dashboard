using BR.helper;
using BR.Models;
using System.Linq.Expressions;

namespace BR.Services.IServices
{
    public interface IPreviousWorkImageService : IGenericQueryService<PreviousWorkImage>, IGenericWriteService<PreviousWorkImage>
    {
        Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<PreviousWorkImage>>> GetAllWithOwner(Expression<Func<PreviousWorkImage, bool>> expression);
        //Task<IResult<City>> AddOrEdit(City entity, CancellationToken cancellationToken = default);
    }
}

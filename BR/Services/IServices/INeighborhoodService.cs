using BR.helper;
using BR.Models;

namespace BR.Services.IServices
{
    public interface INeighborhoodService : IGenericQueryService<Neighborhood>, IGenericWriteService<Neighborhood>
    {
        Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default);
        //Task<IResult<City>> AddOrEdit(City entity, CancellationToken cancellationToken = default);
    }
}

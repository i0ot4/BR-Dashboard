using BR.helper;
using BR.Models;

namespace BR.Services.IServices
{
    public interface ICityService : IGenericQueryService<City>, IGenericWriteService<City>
    {
        Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default);
        //Task<IResult<City>> AddOrEdit(City entity, CancellationToken cancellationToken = default);
    }
}

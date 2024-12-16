using BR.helper;
using BR.Models;

namespace BR.Services.IServices
{
    public interface IContractorWorkerService : IGenericQueryService<ContractorWorkers>, IGenericWriteService<ContractorWorkers>
    {
        Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default);

    }
}

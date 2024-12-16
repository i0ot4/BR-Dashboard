using BR.helper;
using BR.Models.VMs;

namespace BR.Repositories.IRepositories
{
    public interface IWorkerRepository
    {
        Task<IResult<UserViewVM>> UpdateWorker(CreatWorkerVM contractorVM, string id, CancellationToken cancellationToken = default);
    }
}

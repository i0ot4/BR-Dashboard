using BR.helper;
using BR.Models.VMs;

namespace BR.Repositories.IRepositories
{
    public interface IFactoryRepository
    {
        Task<IResult<UserViewVM>> UpdateFactory(CreatFactoryVM factoryVM, string id, CancellationToken cancellationToken = default);
    }
}

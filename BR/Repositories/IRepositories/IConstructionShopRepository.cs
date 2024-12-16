using BR.helper;
using BR.Models.VMs;

namespace BR.Repositories.IRepositories
{
    public interface IConstructionShopRepository
    {
        Task<IResult<UserViewVM>> UpdateConstructionShopVM(CreateConstructionShopVM contractorVM, string id, CancellationToken cancellationToken = default);
    }
}

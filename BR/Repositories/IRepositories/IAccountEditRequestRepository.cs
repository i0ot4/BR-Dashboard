using BR.Models;

namespace BR.Repositories.IRepositories
{
    public interface IAccountEditRequestRepository : IGenericRepository<AccountEditRequest>
    {
        Task<bool> AllowEdit(string accountId, long requestId);
    }
}

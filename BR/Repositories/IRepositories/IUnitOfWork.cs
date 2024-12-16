using Microsoft.EntityFrameworkCore;

namespace BR.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        DbContext GetContext();
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}

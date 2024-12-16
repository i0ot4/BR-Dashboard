using BR.Data;
using BR.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace BR.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public DbContext GetContext()
        {
            return _context;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.CommitAsync(cancellationToken);
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction?.Dispose();

            _context.Dispose();
        }
    }
}

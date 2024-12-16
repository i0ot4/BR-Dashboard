using BR.Data;
using BR.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BR.Repositories
{
    public class QueryRepository<T> : IQueryRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public QueryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual IQueryable<T> Entities => _context.Set<T>().AsNoTracking();

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression, int skip, int take)
        {
            return await _context.Set<T>().Where(expression).Skip(skip).Take(take).AsNoTracking().ToListAsync();
        }

       
        public virtual async Task<IEnumerable<T>> GetAll(int skip, int take)
        {
            return await _context.Set<T>().Skip(skip).Take(take).ToListAsync();
        }
        
        public async Task<T?> GetOne(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(expression);
        }
        
        public async Task<T?> GetById(long Id)
        {
            return await _context.Set<T>().FindAsync(Id);
        }
        public async Task<T?> GetById(int Id)
        {
            return await _context.Set<T>().FindAsync(Id);
        }
    }
}

using BR.Data;
using BR.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BR.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(T model)
        {
            _context.Set<T>().Add(model);
        }

        public async Task<T> AddAndReturn(T entity)
        {
            var result = await _context.Set<T>().AddAsync(entity);

            return result.Entity;
        }
        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().Where(expression).AsNoTracking().ToListAsync();
        }
        public void Delete(T model)
        {
            _context.Set<T>().Remove(model);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(long id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
        public async Task<T?> GetOne(Expression<Func<T, bool>> expression)
        {
            return await _context.Set<T>().AsNoTracking().SingleOrDefaultAsync(expression);
        }
        public void SaveAsync()
        {
            _context.SaveChanges();
        }

        public void Update(T model)
        {
            _context.Set<T>().Update(model);
        }
    }
}

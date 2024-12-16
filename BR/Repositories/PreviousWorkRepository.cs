
using BR.Data;
using BR.Models;
using BR.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BR.Repositories
{
    public class PreviousWorkRepository : GenericRepository<PreviousWork>, IPreviousWorkRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IUnitOfWork unitOfWork;

        public PreviousWorkRepository(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PreviousWork>> GetAllWithOwner()
        {
            return await context.PreviousWorks.Include(p => p.UserId).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<PreviousWork>> GetAllWithOwner(Expression<Func<PreviousWork, bool>> expression)
        {
            return await context.PreviousWorks.Where(expression).Include(p => p.SysUser).AsNoTracking().ToListAsync();
        }
    }
    


}

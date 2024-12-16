
using BR.Data;
using BR.Models;
using BR.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BR.Repositories
{
    public class PreviousWorkImageRepository : GenericRepository<PreviousWorkImage>, IPreviousWorkImageRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IUnitOfWork unitOfWork;

        public PreviousWorkImageRepository(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PreviousWorkImage>> GetAllWithOwner()
        {
            return await context.PreviousWorkImages.Include(p => p.PreviousWorkId).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<PreviousWorkImage>> GetAllWithOwner(Expression<Func<PreviousWorkImage, bool>> expression)
        {
            return await context.PreviousWorkImages.Where(expression).Include(p => p.PreviousWorkId).AsNoTracking().ToListAsync();
        }
    }


}

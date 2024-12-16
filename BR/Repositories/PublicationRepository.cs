
using BR.Data;
using BR.helper;
using BR.Models;
using BR.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BR.Repositories
{
    public class PublicationRepository : GenericRepository<Publication>, IPublicationRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IUnitOfWork unitOfWork;

        public PublicationRepository(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Publication>> GetAllWithOwner()
        {
            return await context.Publications.Include(p=>p.UserId).AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Publication>> GetAllWithOwner(Expression<Func<Publication, bool>> expression)
        {
            return await context.Publications.Where(expression).Include(p=>p.SysUser).AsNoTracking().ToListAsync();
        }
    }


}

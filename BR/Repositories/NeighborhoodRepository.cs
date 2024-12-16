
using BR.Data;
using BR.Models;
using BR.Repositories.IRepositories;

namespace BR.Repositories
{
    public class NeighborhoodRepository : GenericRepository<Neighborhood>, INeighborhoodRepository
    {
        private readonly ApplicationDbContext context;

        public NeighborhoodRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}


using BR.Data;
using BR.Models;
using BR.Repositories.IRepositories;

namespace BR.Repositories
{
    public class DirectorateRepository : GenericRepository<Directorate>, IDirectorateRepository
    {
        private readonly ApplicationDbContext context;

        public DirectorateRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}

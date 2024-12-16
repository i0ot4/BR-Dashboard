
using BR.Data;
using BR.Models;
using BR.Repositories.IRepositories;

namespace BR.Repositories
{
    public class CityRepository : GenericRepository<City>, ICityRepository
    {
        private readonly ApplicationDbContext context;

        public CityRepository(ApplicationDbContext context): base(context)
        {
            this.context = context;
        }
    }
}

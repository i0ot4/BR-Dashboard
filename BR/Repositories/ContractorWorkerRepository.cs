using BR.Data;
using BR.Models;
using BR.Repositories.IRepositories;

namespace BR.Repositories
{
    public class ContractorWorkerRepository : GenericRepository<ContractorWorkers>, IContractorWorkerRepository
    {
        private readonly ApplicationDbContext context;
        public ContractorWorkerRepository(ApplicationDbContext context) : base(context)
        {
            this.context = context;
        }
    }
}

using BR.helper;
using BR.Models;
using BR.Repositories;
using BR.Repositories.IRepositories;
using BR.Services.IServices;

namespace BR.Services
{
    public class ContractorWorkerService : GenericQueryService<ContractorWorkers>, IContractorWorkerService
    {
        private readonly IContractorWorkerRepository contractorWorkerRepository;
        private readonly IUnitOfWork unitOfWork;

        public ContractorWorkerService(IQueryRepository<ContractorWorkers> queryRepository, IContractorWorkerRepository contractorWorkerRepository, IUnitOfWork unitOfWork) : base(queryRepository)
        {
            this.contractorWorkerRepository = contractorWorkerRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default)
        {
            var item = await contractorWorkerRepository.GetById(Id);
            if (item == null) return Result<bool>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsActive = status;
            contractorWorkerRepository.Update(item);
            try
            {
                var res = await unitOfWork.CompleteAsync(cancellationToken);
                if (res > 0)
                {
                    return await Result<bool>.SuccessAsync(true, " record state updated");
                }

                return Result<bool>.Fail($"--- Error, no data changed ---");
            }
            catch (Exception exp)
            {
                return await Result<bool>.FailAsync($"EXP in Change Active at ( {this.GetType().Name} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult<ContractorWorkers>> Add(ContractorWorkers entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<ContractorWorkers>.FailAsync($"Error in Add of: {GetType()}, the passed entity is NULL !!!.");
            try
            {


                var newEntity = await contractorWorkerRepository.AddAndReturn(entity);

                await unitOfWork.CompleteAsync(cancellationToken);



                return await Result<ContractorWorkers>.SuccessAsync(newEntity, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<ContractorWorkers>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }
        }

        public Task<helper.IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<helper.IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IResult<ContractorWorkers>> Update(ContractorWorkers entity, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}

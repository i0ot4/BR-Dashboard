using BR.helper;
using BR.Models;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using IResult = BR.helper.IResult;
namespace BR.Services
{
    public class NeighborhoodService : GenericQueryService<Neighborhood>, INeighborhoodService
    {
        private readonly INeighborhoodRepository neighborhoodRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NeighborhoodService(IQueryRepository<Neighborhood> queryRepository,INeighborhoodRepository NeighborhoodRepository, IUnitOfWork unitOfWork): base(queryRepository)
        {
            this.neighborhoodRepository = NeighborhoodRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<Neighborhood>> Add(Neighborhood entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<Neighborhood>.FailAsync($"Error in Add of: {GetType()}, the passed entity is NULL !!!.");
            try
            {

                
                var newEntity = await neighborhoodRepository.AddAndReturn(entity);

                await _unitOfWork.CompleteAsync(cancellationToken);

                //var entityMap = _mapper.Map<PatientDto>(newEntity);


                return await Result<Neighborhood>.SuccessAsync(newEntity, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<Neighborhood>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult<Neighborhood>> Update(Neighborhood entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<Neighborhood>.FailAsync($"Error in {GetType()} : the passed entity IS NULL.");

            var item = await neighborhoodRepository.GetById(entity.Id);

            if (item == null) return await Result<Neighborhood>.FailAsync($"--- there is no Data with this id: {entity.Id}---");

            //_mapper.Map(entity, item);

            neighborhoodRepository.Update(item);

            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<Neighborhood>.SuccessAsync(item, "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<Neighborhood>.FailAsync($"EXP in Update at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await neighborhoodRepository.GetById(Id);
            if (item == null) return Result<Neighborhood>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.UtcNow;
            //item.ModifiedBy = $"{session.UserId}";
            neighborhoodRepository.Update(item);
            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<Neighborhood>.SuccessAsync(item, " record removed");
            }
            catch (Exception exp)
            {
                return await Result<Neighborhood>.FailAsync($"EXP in Remove at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default)
        {
            var item = await neighborhoodRepository.GetById(Id);
            if (item == null) return Result<bool>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsActive = status;
            neighborhoodRepository.Update(item);
            try
            {
                var res = await _unitOfWork.CompleteAsync(cancellationToken);
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

        public async Task<IResult> Remove(int Id, CancellationToken cancellationToken = default)
        {
            var item = await neighborhoodRepository.GetById(Id);
            if (item == null) return Result<Neighborhood>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            //item.ModifiedBy = $"{session.UserId}";
            neighborhoodRepository.Update(item);
            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<Neighborhood>.SuccessAsync(item, " record removed");
            }
            catch (Exception exp)
            {
                return await Result<Neighborhood>.FailAsync($"EXP in Remove at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }


    }
}

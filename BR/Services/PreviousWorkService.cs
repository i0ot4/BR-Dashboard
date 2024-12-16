using BR.helper;
using BR.Models;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using System.Linq.Expressions;
using IResult = BR.helper.IResult;
namespace BR.Services
{
    public class PreviousWorkService : GenericQueryService<PreviousWork>, IPreviousWorkService
    {
        private readonly IPreviousWorkRepository _previousWorkRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PreviousWorkService(IQueryRepository<PreviousWork> queryRepository, IPreviousWorkRepository previousWorkRepository, IUnitOfWork unitOfWork): base(queryRepository)
        {
            _previousWorkRepository = previousWorkRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<PreviousWork>> Add(PreviousWork entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<PreviousWork>.FailAsync($"Error in Add of: {GetType()}, the passed entity is NULL !!!.");
            try
            {

                
                var newEntity = await _previousWorkRepository.AddAndReturn(entity);

                await _unitOfWork.CompleteAsync(cancellationToken);

                //var entityMap = _mapper.Map<PatientDto>(newEntity);


                return await Result<PreviousWork>.SuccessAsync(newEntity, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<PreviousWork>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult<PreviousWork>> Update(PreviousWork entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<PreviousWork>.FailAsync($"Error in {GetType()} : the passed entity IS NULL.");

            var item = await _previousWorkRepository.GetById(entity.Id);

            if (item == null) return await Result<PreviousWork>.FailAsync($"--- there is no Data with this id: {entity.Id}---");

            //_mapper.Map(entity, item);

            _previousWorkRepository.Update(item);

            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<PreviousWork>.SuccessAsync(item, "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<PreviousWork>.FailAsync($"EXP in Update at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<IEnumerable<PreviousWork>>> GetAllWithOwner(Expression<Func<PreviousWork, bool>> expression)
        {
            try
            {
                var items = await _previousWorkRepository.GetAllWithOwner(expression);
                if(items == null) return await Result<IEnumerable<PreviousWork>>.FailAsync("No Data Found");

                return await Result<IEnumerable<PreviousWork>>.SuccessAsync(items, "records retrieved");
            }
            catch (Exception exc)
            {
                return await Result<IEnumerable<PreviousWork>>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }
        }
        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await _previousWorkRepository.GetById(Id);
            if (item == null) return Result<PreviousWork>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.UtcNow;
            //item.ModifiedBy = $"{session.UserId}";
            _previousWorkRepository.Update(item);
            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<PreviousWork>.SuccessAsync(item, " record removed");
            }
            catch (Exception exp)
            {
                return await Result<PreviousWork>.FailAsync($"EXP in Remove at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default)
        {
            var item = await _previousWorkRepository.GetById(Id);
            if (item == null) return Result<bool>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsActive = status;
            _previousWorkRepository.Update(item);
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
            var item = await _previousWorkRepository.GetById(Id);
            if (item == null) return Result<City>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            //item.ModifiedBy = $"{session.UserId}";
            _previousWorkRepository.Update(item);
            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<PreviousWork>.SuccessAsync(item, " record removed");
            }
            catch (Exception exp)
            {
                return await Result<PreviousWork>.FailAsync($"EXP in Remove at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }


    }
}

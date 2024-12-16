using BR.helper;
using BR.Models;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using System.Linq.Expressions;
using IResult = BR.helper.IResult;
namespace BR.Services
{
    public class PreviousWorkImageService : GenericQueryService<PreviousWorkImage>, IPreviousWorkImageService
    {
        private readonly IPreviousWorkImageRepository _previousWorkImageRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PreviousWorkImageService(IQueryRepository<PreviousWorkImage> queryRepository, IPreviousWorkImageRepository previousWorkImageRepository, IUnitOfWork unitOfWork): base(queryRepository)
        {
            _previousWorkImageRepository = previousWorkImageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<PreviousWorkImage>> Add(PreviousWorkImage entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<PreviousWorkImage>.FailAsync($"Error in Add of: {GetType()}, the passed entity is NULL !!!.");
            try
            {

                
                var newEntity = await _previousWorkImageRepository.AddAndReturn(entity);

                await _unitOfWork.CompleteAsync(cancellationToken);

                //var entityMap = _mapper.Map<PatientDto>(newEntity);


                return await Result<PreviousWorkImage>.SuccessAsync(newEntity, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<PreviousWorkImage>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult<PreviousWorkImage>> Update(PreviousWorkImage entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<PreviousWorkImage>.FailAsync($"Error in {GetType()} : the passed entity IS NULL.");

            var item = await _previousWorkImageRepository.GetById(entity.Id);

            if (item == null) return await Result<PreviousWorkImage>.FailAsync($"--- there is no Data with this id: {entity.Id}---");

            //_mapper.Map(entity, item);

            _previousWorkImageRepository.Update(item);

            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<PreviousWorkImage>.SuccessAsync(item, "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<PreviousWorkImage>.FailAsync($"EXP in Update at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<IEnumerable<PreviousWorkImage>>> GetAllWithOwner(Expression<Func<PreviousWorkImage, bool>> expression)
        {
            try
            {
                var items = await _previousWorkImageRepository.GetAllWithOwner(expression);
                if(items == null) return await Result<IEnumerable<PreviousWorkImage>>.FailAsync("No Data Found");

                return await Result<IEnumerable<PreviousWorkImage>>.SuccessAsync(items, "records retrieved");
            }
            catch (Exception exc)
            {
                return await Result<IEnumerable<PreviousWorkImage>>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }
        }
        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await _previousWorkImageRepository.GetById(Id);
            if (item == null) return Result<PreviousWorkImage>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.UtcNow;
            //item.ModifiedBy = $"{session.UserId}";
            _previousWorkImageRepository.Update(item);
            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<PreviousWorkImage>.SuccessAsync(item, " record removed");
            }
            catch (Exception exp)
            {
                return await Result<PreviousWorkImage>.FailAsync($"EXP in Remove at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default)
        {
            var item = await _previousWorkImageRepository.GetById(Id);
            if (item == null) return Result<bool>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsActive = status;
            _previousWorkImageRepository.Update(item);
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
            var item = await _previousWorkImageRepository.GetById(Id);
            if (item == null) return Result<PreviousWorkImage>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            //item.ModifiedBy = $"{session.UserId}";
            _previousWorkImageRepository.Update(item);
            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<PreviousWorkImage>.SuccessAsync(item, " record removed");
            }
            catch (Exception exp)
            {
                return await Result<PreviousWorkImage>.FailAsync($"EXP in Remove at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }


    }
}

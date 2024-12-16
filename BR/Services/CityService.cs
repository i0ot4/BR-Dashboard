using BR.helper;
using BR.Models;
using BR.Repositories;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using IResult = BR.helper.IResult;
namespace BR.Services
{
    public class CityService : GenericQueryService<City>, ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CityService(IQueryRepository<City> queryRepository,ICityRepository cityRepository, IUnitOfWork unitOfWork): base(queryRepository)
        {
            _cityRepository = cityRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<City>> Add(City entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<City>.FailAsync($"Error in Add of: {GetType()}, the passed entity is NULL !!!.");
            try
            {

                
                var newEntity = await _cityRepository.AddAndReturn(entity);

                await _unitOfWork.CompleteAsync(cancellationToken);

                //var entityMap = _mapper.Map<PatientDto>(newEntity);


                return await Result<City>.SuccessAsync(newEntity, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<City>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult<City>> Update(City entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<City>.FailAsync($"Error in {GetType()} : the passed entity IS NULL.");

            var item = await _cityRepository.GetById(entity.Id);

            if (item == null) return await Result<City>.FailAsync($"--- there is no Data with this id: {entity.Id}---");

            //_mapper.Map(entity, item);

            _cityRepository.Update(item);

            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<City>.SuccessAsync(item, "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<City>.FailAsync($"EXP in Update at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await _cityRepository.GetById(Id);
            if (item == null) return Result<City>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.UtcNow;
            //item.ModifiedBy = $"{session.UserId}";
            _cityRepository.Update(item);
            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<City>.SuccessAsync(item, " record removed");
            }
            catch (Exception exp)
            {
                return await Result<City>.FailAsync($"EXP in Remove at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default)
        {
            var item = await _cityRepository.GetById(Id);
            if (item == null) return Result<bool>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsActive = status;
            _cityRepository.Update(item);
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
            var item = await _cityRepository.GetById(Id);
            if (item == null) return Result<City>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            //item.ModifiedBy = $"{session.UserId}";
            _cityRepository.Update(item);
            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<City>.SuccessAsync(item, " record removed");
            }
            catch (Exception exp)
            {
                return await Result<City>.FailAsync($"EXP in Remove at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }


    }
}

using BR.helper;
using BR.Models;
using BR.Repositories;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using System.Linq.Expressions;
using IResult = BR.helper.IResult;
namespace BR.Services
{
    public class PublicationService : GenericQueryService<Publication>, IPublicationService
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PublicationService(IQueryRepository<Publication> queryRepository, IPublicationRepository publicationRepository, IUnitOfWork unitOfWork): base(queryRepository)
        {
            _publicationRepository = publicationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<Publication>> Add(Publication entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<Publication>.FailAsync($"Error in Add of: {GetType()}, the passed entity is NULL !!!.");
            try
            {

                
                var newEntity = await _publicationRepository.AddAndReturn(entity);

                await _unitOfWork.CompleteAsync(cancellationToken);

                //var entityMap = _mapper.Map<PatientDto>(newEntity);


                return await Result<Publication>.SuccessAsync(newEntity, "item added successfully");
            }
            catch (Exception exc)
            {

                return await Result<Publication>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }

        }

        public async Task<IResult<Publication>> Update(Publication entity, CancellationToken cancellationToken = default)
        {
            if (entity == null) return await Result<Publication>.FailAsync($"Error in {GetType()} : the passed entity IS NULL.");

            var item = await _publicationRepository.GetById(entity.Id);

            if (item == null) return await Result<Publication>.FailAsync($"--- there is no Data with this id: {entity.Id}---");

            //_mapper.Map(entity, item);

            _publicationRepository.Update(item);

            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<Publication>.SuccessAsync(item, "Item updated successfully");
            }
            catch (Exception exp)
            {
                Console.WriteLine($"EXP in Update at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
                return await Result<Publication>.FailAsync($"EXP in Update at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<IEnumerable<Publication>>> GetAllWithOwner(Expression<Func<Publication, bool>> expression)
        {
            try
            {
                var items = await _publicationRepository.GetAllWithOwner(expression);
                if(items == null) return await Result<IEnumerable<Publication>>.FailAsync("No Data Found");

                return await Result<IEnumerable<Publication>>.SuccessAsync(items, "records retrieved");
            }
            catch (Exception exc)
            {
                return await Result<IEnumerable<Publication>>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }
        }
        public async Task<IResult> Remove(long Id, CancellationToken cancellationToken = default)
        {
            var item = await _publicationRepository.GetById(Id);
            if (item == null) return Result<City>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.UtcNow;
            //item.ModifiedBy = $"{session.UserId}";
            _publicationRepository.Update(item);
            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<Publication>.SuccessAsync(item, " record removed");
            }
            catch (Exception exp)
            {
                return await Result<Publication>.FailAsync($"EXP in Remove at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<bool>> ChangeActive(long Id, bool status, CancellationToken cancellationToken = default)
        {
            var item = await _publicationRepository.GetById(Id);
            if (item == null) return Result<bool>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsActive = status;
            _publicationRepository.Update(item);
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
            var item = await _publicationRepository.GetById(Id);
            if (item == null) return Result<City>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsDeleted = true;
            item.ModifiedOn = DateTime.Now;
            //item.ModifiedBy = $"{session.UserId}";
            _publicationRepository.Update(item);
            try
            {
                await _unitOfWork.CompleteAsync(cancellationToken);

                return await Result<Publication>.SuccessAsync(item, " record removed");
            }
            catch (Exception exp)
            {
                return await Result<Publication>.FailAsync($"EXP in Remove at ( {GetType()} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }


    }
}

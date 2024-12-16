using BR.helper;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using System.Linq.Expressions;

namespace BR.Services
{
    public class GenericQueryService<TDto> : IGenericQueryService<TDto> where TDto : class
    {
        private IQueryRepository<TDto> queryRepository;

        public GenericQueryService(IQueryRepository<TDto> queryRepository)
        {
            this.queryRepository = queryRepository;
        }

        public async Task<IResult<IEnumerable<TDto>>> GetAll(CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await queryRepository.GetAll();


                if (items == null) return await Result<IEnumerable<TDto>>.FailAsync("No Data Found");
                var itemMap = items;

                return await Result<IEnumerable<TDto>>.SuccessAsync(items, "records retrieved");

            }
            catch (Exception exc)
            {
                return await Result<IEnumerable<TDto>>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }
        }
        public async Task<IResult<IEnumerable<TDto>>> GetAll(Expression<Func<TDto, bool>> expression, CancellationToken cancellationToken = default)
        {
            try
            {
                //var mapExpr = _mapper.Map<Expression<Func<TModel, bool>>>(expression);
                var items = await queryRepository.GetAll(expression);
                if (items == null) return await Result<IEnumerable<TDto>>.FailAsync("No Data Found");
                //var itemMap = _mapper.Map<IEnumerable<TDto>>(items);

                return await Result<IEnumerable<TDto>>.SuccessAsync(items, "records retrieved");

            }
            catch (Exception exc)
            {
                return await Result<IEnumerable<TDto>>.FailAsync($"EXP in {GetType()}, Meesage: {exc.Message}");
            }
        }
        //public async Task<IResult<TDto>> GetForUpdate<TDto>(long Id, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        var item = await queryRepository.GetById(Id);
        //        if (item == null) return Result<TDto>.Fail($"--- there is no Data with this id: {Id}---");
        //        var newEntity = _mapper.Map<TEditDto>(item);
        //        return await Result<TDto>.SuccessAsync(item, "");

        //    }
        //    catch (Exception ex)
        //    {
        //        return Result<TDto>.Fail($"Exp in get data by Id: {ex.Message} --- {(ex.InnerException != null ? "InnerExp: " + ex.InnerException.Message : "no inner")} .");
        //    }
        //}
        public async Task<IResult<TDto>> GetForUpdate(long Id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await queryRepository.GetById(Id);
                if (item == null) return Result<TDto>.Fail($"--- there is no Data with this id: {Id}---");
                return await Result<TDto>.SuccessAsync(item, "");

            }catch (Exception ex)
            {
                return Result<TDto>.Fail($"Exp in get data by Id: {ex.Message} --- {(ex.InnerException != null ? "InnerExp: " + ex.InnerException.Message : "no inner")} .");
            }
        }
        public async Task<IResult<TDto>> GetById(long Id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await queryRepository.GetById(Id);
                if (item == null) return Result<TDto>.Fail($"--- there is no Data with this id: {Id}---");
                //var newEntity = _mapper.Map<TDto>(item);
                return await Result<TDto>.SuccessAsync(item, "");

            }
            catch (Exception ex)
            {
                return Result<TDto>.Fail($"Exp in get data by Id: {ex.Message} --- {(ex.InnerException != null ? "InnerExp: " + ex.InnerException.Message : "no inner")} .");
            }
        }

        public async Task<IResult<TDto>> GetById(int Id, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = await queryRepository.GetById(Id);
                if (item == null) return Result<TDto>.Fail($"--- there is no Data with this id: {Id}---");
                //var newEntity = _mapper.Map<TDto>(item);
                return await Result<TDto>.SuccessAsync(item, "");

            }
            catch (Exception ex)
            {
                return Result<TDto>.Fail($"Exp in get data by Id: {ex.Message} --- {(ex.InnerException != null ? "InnerExp: " + ex.InnerException.Message : "no inner")} .");
            }
        }
    }
}

using BR.helper;
using System.Linq.Expressions;

namespace BR.Services.IServices
{
    public interface IGenericQueryService<TDto> where TDto : class
    {
        Task<IResult<IEnumerable<TDto>>> GetAll(CancellationToken cancellationToken = default);
        Task<IResult<IEnumerable<TDto>>> GetAll(Expression<Func<TDto, bool>> expression, CancellationToken cancellationToken = default);
        //Task<IResult<IEnumerable<TDto>>> GetAll(Expression<Func<TDto, bool>> expression, int skip, int take, CancellationToken cancellationToken = default);
        Task<IResult<TDto>> GetForUpdate(long Id, CancellationToken cancellationToken = default);
        Task<IResult<TDto>> GetById(int Id, CancellationToken cancellationToken = default);
        Task<IResult<TDto>> GetById(long Id, CancellationToken cancellationToken = default);
        //Task<IResult<TDto>> GetOne(Expression<Func<TDto, bool>> expression, CancellationToken cancellationToken = default);
    }
}

using BR.helper;
using IResult = BR.helper.IResult;
namespace BR.Services.IServices
{
    public interface IGenericWriteService<TAddDto> where TAddDto : class
    {
        Task<IResult<TAddDto>> Add(TAddDto entity, CancellationToken cancellationToken = default);
        Task<IResult<TAddDto>> Update(TAddDto entity, CancellationToken cancellationToken = default);

        Task<IResult> Remove(long Id, CancellationToken cancellationToken = default);
        Task<IResult> Remove(int Id, CancellationToken cancellationToken = default);
    }
}

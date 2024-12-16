using BR.helper;
using BR.Models;
using BR.Models.VMs;
using BR.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace BR.Repositories
{
    public class FactoryRepository : IFactoryRepository
    {
        private readonly ISysUserRepository sysUserRepository;
        private readonly IUnitOfWork unitOfWork;
        public FactoryRepository(ISysUserRepository sysUserRepository, IUnitOfWork unitOfWork)
        {
            this.sysUserRepository = sysUserRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<IResult<UserViewVM>> UpdateFactory(CreatFactoryVM factoryVM, string id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (factoryVM == null)
                {
                    return await Result<UserViewVM>.FailAsync($"Error in update user at: {GetType()}, the passed entity is NULL !!!.");
                }
                else
                {
                    var item = await sysUserRepository.GetById(id);
                    if (item == null) return Result<UserViewVM>.Fail($"--- there is no Data with this id: {id}---");

                    if (item.UserType != UserTypes.Factory) return Result<UserViewVM>.Fail($"--- user not match: {id}---");

                    item.Image = factoryVM.Image;
                    item.Description = factoryVM.Description;
                    item.CityId = factoryVM.CityId;
                    item.DirectorateId = factoryVM.DirectorateId;
                    item.NeighborhoodId = factoryVM.NeighborhoodId;
                    item.CommericalRegistrationImage = factoryVM.Image;
                    sysUserRepository.Update(item);

                    var res = await unitOfWork.CompleteAsync(cancellationToken);
                    if (res > 0)
                    {
                        var userView = new UserViewVM
                        {
                            User = item
                        };
                        return await Result<UserViewVM>.SuccessAsync(userView, " user data updated");
                    }

                    return Result<UserViewVM>.Fail($"--- Error, no data changed ---");

                }

            }

            catch (Exception exp)
            {
                return await Result<UserViewVM>.FailAsync($"EXP in Update User at ( {this.GetType().Name} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }

    }
}

using BR.Data;
using BR.helper;
using BR.Models;
using BR.Models.VMs;
using BR.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;

namespace BR.Repositories
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<SysUser> userManager;
        private readonly ISysUserRepository sysUserRepository;
        private readonly IUnitOfWork unitOfWork;
        public WorkerRepository(
            ApplicationDbContext context,
            UserManager<SysUser> userManager,
            ISysUserRepository sysUserRepository,
            IUnitOfWork unitOfWork)
        {
            this.context = context;
            this.userManager = userManager;
            this.sysUserRepository = sysUserRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<IResult<UserViewVM>> UpdateWorker(CreatWorkerVM contractorVM, string id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (contractorVM == null)
                {
                    return await Result<UserViewVM>.FailAsync($"Error in update user at: {GetType()}, the passed entity is NULL !!!.");
                }
                else
                {
                    var item = await sysUserRepository.GetById(id);
                    if (item == null) return Result<UserViewVM>.Fail($"--- there is no Data with this id: {id}---");

                    if(item.UserType != UserTypes.Worker) return Result<UserViewVM>.Fail($"--- user not match: {id}---");

                    item.Image = contractorVM.Image;
                    item.Description = contractorVM.Description;
                    item.Occupation = contractorVM.Occupation;
                    item.CityId = contractorVM.CityId;
                    item.DirectorateId = contractorVM.DirectorateId;
                    item.NeighborhoodId = contractorVM.NeighborhoodId;
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

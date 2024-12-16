using BR.helper;
using BR.Models.VMs;
using Microsoft.AspNetCore.Identity;

namespace BR.Services.IServices
{
    public interface IAuthService
    {
        Task<IResult<AuthVM>> Login(LoginVM login, bool fromWeb = false);
        Task<IResult<UserViewVM>> CreateUser(CreateUserVM user);
        Task<IResult<UserViewVM>> UpdateContractorUser(CreateContractorVM contractorVM, string id, CancellationToken cancellationToken = default);
        Task<IResult<CreateRoleVM>> CreateRole(CreateRoleVM role);
        Task<IResult<bool>> ChangeActive(string id, bool isActive, CancellationToken cancellationToken = default);
        Task<IResult<bool>> ConfirmUser(string id, bool confirm = true, CancellationToken cancellationToken = default);
        Task<IResult<bool>> Logout();
        Task<IEnumerable<IdentityRole>> GetRoleList();
        Task<List<UserRoleVM>> EditUsersInRole(string roleId);
        Task<RoleEditVM?> EditRole(string id);
        Task<bool> EditUsersInRole(List<UserRoleVM> model, string roleId);
    }
}

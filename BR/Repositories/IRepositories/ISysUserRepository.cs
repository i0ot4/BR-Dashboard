using BR.helper;
using BR.Models;
using BR.Models.VMs;

namespace BR.Repositories.IRepositories
{
    public interface ISysUserRepository : IGenericRepository<SysUser>
    {
        Task<SysUser> GetByPhone(string phone);
        Task<SysUser> GetById(string id);
    }
}

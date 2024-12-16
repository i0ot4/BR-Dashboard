using BR.Data;
using BR.helper;
using BR.Models;
using BR.Models.VMs;
using BR.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace BR.Repositories
{
    public class SysUserRepository : GenericRepository<SysUser>, ISysUserRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<SysUser> userManager;
        public SysUserRepository(ApplicationDbContext context, UserManager<SysUser> userManager) : base(context)
        {
            this.context = context;
            this.userManager = userManager;
        }
        public async Task<SysUser> GetById(string id)
        {
            try
            {
                var user = userManager.Users.Where(s => s.Id == id).SingleOrDefault();
                if (user != null)
                {
                    return await Task.FromResult<SysUser>(user);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<SysUser> GetByPhone(string phone)
        {
            try
            {
                var user = userManager.Users.Where(s => s.PhoneNumber == phone).SingleOrDefault();
                if (user != null)
                {
                    return await Task.FromResult<SysUser>(user);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }
        
    }
}

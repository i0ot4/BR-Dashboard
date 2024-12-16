
using BR.Data;
using BR.Models;
using BR.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace BR.Repositories
{
    public class AccountEditRequestRepository : GenericRepository<AccountEditRequest>, IAccountEditRequestRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ISysUserRepository sysUserRepository;

        public AccountEditRequestRepository(ApplicationDbContext context,ISysUserRepository sysUserRepository): base(context)
        {
            this.context = context;
            this.sysUserRepository = sysUserRepository;
        }

        public async  Task<bool> AllowEdit(string accountId, long requestId)
        {
            try
            {
                var resutl = context.AccountEditRequests.FirstOrDefault(i => i.Id == requestId);
                if (resutl == null)
                {
                    return false;
                }
                resutl.IsDeleted = true;
                resutl.IsActive = true;
                context.Update(resutl);
                var user = await sysUserRepository.GetById(accountId);
                
                if (user != null)
                {
                    user.CanEdit = true;
                    user.EditCount = user.EditCount + 1;
                    sysUserRepository.Update(user);
                    sysUserRepository.SaveAsync();
                    return await Task.FromResult<bool>(true);
                }
                return false;

            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }
        }
    }
}

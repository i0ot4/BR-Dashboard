using BR.helper;
using BR.Models;
using BR.Models.VMs;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BR.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<SysUser> userManager;
        private readonly SignInManager<SysUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ISysUserRepository sysUserRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IConfiguration config;

        public AuthService(
            UserManager<SysUser> userManager,
            SignInManager<SysUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ISysUserRepository sysUserRepository,
            IUnitOfWork unitOfWork,
            IConfiguration config)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.sysUserRepository = sysUserRepository;
            this.unitOfWork = unitOfWork;
            this.config = config;
            this.roleManager = roleManager;
        }


        public async Task<IResult<AuthVM>> Login(LoginVM login, bool fromWeb = false)
        {
            try
            {
                if (login == null) return await Result<AuthVM>.FailAsync($"Error in login of: {GetType()}, the passed entity is NULL !!!.");

                if (string.IsNullOrEmpty(login.PhoneNumber))
                {
                    return await Result<AuthVM>.FailAsync($"اسم المستخدم مطلوب");
                }

                var user = await sysUserRepository.GetByPhone(login.PhoneNumber);
                if (user == null)
                {
                    return await Result<AuthVM>.FailAsync($"الحساب غير موجود");
                }

                if (user.IsDeleted)
                {
                    return await Result<AuthVM>.FailAsync($"الحساب غير موجود");
                }

                if (!user.IsConfirmed)
                {
                    return await Result<AuthVM>.FailAsync($"الحساب غير مؤكد");
                }

                if (!user.IsActive)
                {
                    return await Result<AuthVM>.FailAsync($"الحساب غير فعال");
                }

                var result = await signInManager.PasswordSignInAsync(login.PhoneNumber, login.Password, login.RememberMe, false);
                if (result != null && result.Succeeded)
                {
                    var token = CreateToken(user);
                    if (string.IsNullOrEmpty(token) && !fromWeb)
                    {
                        return await Result<AuthVM>.FailAsync($"error while creating token");
                    }

                    var authRes = new AuthVM { User = user, Token = token };
                    return await Result<AuthVM>.SuccessAsync(authRes, "تم تسجيل الدخول بنجاح");
                }
                else
                {
                    return await Result<AuthVM>.FailAsync($"خطأ في اسم المستخدم او كلمة السر");
                }

            }
            catch (Exception exp)
            {
                return await Result<AuthVM>.FailAsync($"EXP in {GetType()}, Meesage: {exp.Message}");
            }
        }
        public async Task<IResult<bool>> ChangeActive(string id, bool isActive = false, CancellationToken cancellationToken = default)
        {
            var item = await sysUserRepository.GetById(id);
            if (item == null) return Result<bool>.Fail($"--- there is no Data with this id: {id}---");
            item.IsActive = isActive;
            sysUserRepository.Update(item);
            try
            {
                var res = await unitOfWork.CompleteAsync(cancellationToken);
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
        public async Task<RoleEditVM?> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return null;
            }
            var model = new RoleEditVM
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach (var user in userManager.Users)
            {

                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }   
            }
            return model;
        }
        public async Task<IResult<bool>> ConfirmUser(string Id, bool confirm = true, CancellationToken cancellationToken = default)
        {
            var item = await sysUserRepository.GetById(Id);
            if (item == null) return Result<bool>.Fail($"--- there is no Data with this id: {Id}---");
            item.IsConfirmed = true;
            sysUserRepository.Update(item);
            try
            {
                var res = await unitOfWork.CompleteAsync(cancellationToken);
                if (res > 0)
                {
                    return await Result<bool>.SuccessAsync(true, " user confirmed successfully");
                }

                return Result<bool>.Fail($"--- Error, no data changed ---");
            }
            catch (Exception exp)
            {
                return await Result<bool>.FailAsync($"EXP in Confirm User at ( {this.GetType().Name} ) , Message: {exp.Message} --- {(exp.InnerException != null ? "InnerExp: " + exp.InnerException.Message : "no inner")}");
            }
        }
        public async Task<IResult<CreateRoleVM>> CreateRole(CreateRoleVM role)
        {
            try
            {
                IdentityRole roleIdentity = new IdentityRole
                {
                    Name = role.RoleName,
                };
                IdentityResult result = await roleManager.CreateAsync(roleIdentity);
                if (result.Succeeded)
                {
                    return await Result<CreateRoleVM>.SuccessAsync(role, "user added successfully");
                }
                return await Result<CreateRoleVM>.FailAsync($"خطأ اثناء اضافة الصلاحية");


            }
            catch (Exception exp)
            {
                return await Result<CreateRoleVM>.FailAsync($"EXP in {GetType()}, Meesage: {exp.Message}");
            }
        }
        public async Task<List<UserRoleVM>> EditUsersInRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            var model = new List<UserRoleVM>();

            if (role == null)
            {
                return model;

            }
            foreach (var user in userManager.Users)
            {
                var UserRoleDto = new UserRoleVM
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    UserRoleDto.IsSelected = true;
                }
                else
                {
                    UserRoleDto.IsSelected = false;

                }
                model.Add(UserRoleDto);
            }
            return model;
        }
        public async Task<bool> EditUsersInRole(List<UserRoleVM> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return false;
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return true;
                }
            }
            return false;
        }
        public async Task<IResult<UserViewVM>> CreateUser(CreateUserVM user)
        {
            try
            {
                if (user == null)
                {
                    return await Result<UserViewVM>.FailAsync($"Error in create user at: {GetType()}, the passed entity is NULL !!!.");
                }

                if (!await IsUniquePhone(user.PhoneNumber))
                {
                    return await Result<UserViewVM>.FailAsync($"خطأ رقم الهاتف هذا مستخدم من قبل");
                }

                if (!string.IsNullOrEmpty(user.Email))
                {
                    if (!await IsUniqueEmail(user.Email))
                    {
                        return await Result<UserViewVM>.FailAsync($"خطأ هذا الايميل مستخدم من قبل");
                    }
                }

                // =============== begin transaction to add the user in one proccess ==================
                // var hasher = new PasswordHasher();
                //var hashedPassword = hasher.HashPassword(user.Password);
                var newUser = new SysUser
                {
                    PhoneNumber = user.PhoneNumber,
                    Email = user.PhoneNumber,
                    FullName = user.FullName,
                    Image = user.Image,
                    UserName = user.PhoneNumber,
                    UserType = user.UserType,
                    CreatedOn = DateTime.UtcNow,
                    IsConfirmed = false,
                    IsDeleted = false,
                    IsActive = true
                };

                var addRes = await userManager.CreateAsync(newUser, user.Password);
                if (addRes != null && addRes.Succeeded)
                {
                    var addedUser = await sysUserRepository.GetByPhone(newUser.PhoneNumber);
                    if (addedUser != null)
                    {
                        var userView = new UserViewVM
                        {
                            User = addedUser
                        };
                        return await Result<UserViewVM>.SuccessAsync(userView, "user added successfully");
                    }
                }
                return await Result<UserViewVM>.FailAsync($"خطأ اثناء اضافة المستخدم");
            }
            catch (Exception exp)
            {
                return await Result<UserViewVM>.FailAsync($"EXP in {GetType()}, Meesage: {exp.Message}");
            }
        }
        public async Task<IResult<UserViewVM>> UpdateContractorUser(CreateContractorVM contractorVM, string id, CancellationToken cancellationToken)
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
                    item.Image = contractorVM.Image;
                    item.CommercialRegistrationNo = contractorVM.CommercialRegistrationNo;
                    item.CommericalRegistrationImage = contractorVM.CommericalRegistrationImage;
                    item.Description = contractorVM.Description;
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
        public async Task<IResult<bool>> Logout()
        {
            try
            {
                await signInManager.SignOutAsync();
                return await Result<bool>.SuccessAsync(true, "");
            }
            catch (Exception ex)
            {
                return await Result<bool>.FailAsync($"Exption in Logout: {ex.Message}");
            }
        }
        private async Task<bool> IsUniquePhone(string phone)
        {
            try
            {
                var res = await sysUserRepository.GetByPhone(phone);
                if (res == null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private async Task<bool> IsUniqueEmail(string email)
        {
            try
            {
                var res = await sysUserRepository.GetOne(d => d.Email == email);
                if (res == null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private string CreateToken(SysUser user, string? deviceId = "")
        {
            try
            {
                if (user == null) return string.Empty;
                var tokenHandler = new JwtSecurityTokenHandler();
                var appSetting = config.GetSection("AppSettings");
                var secret = appSetting.GetValue<string>("Secret");
                var key = Encoding.UTF8.GetBytes(secret);


                List<Claim> claims = new List<Claim>
                {
                    new Claim("FullName", user.FullName??""),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("Phone", user.PhoneNumber??""),
                    new Claim("Email", user.Email??""),
                    new Claim("UserType", user.UserType??""),
                    //new Claim("exp", $"{DateTime.Now.AddSeconds(40)}"),

                };
                SigningCredentials signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

                var tokenOptions = new JwtSecurityToken
                (
                claims: claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: signingCredentials,
                issuer: "NashwanNejadCleanMobile",
                audience: "NashwanNejadCleanSever"
                );

                var dt = DateTime.Now.ToBinary();
                var dt1 = DateTime.Now.ToFileTime();
                var dt2 = DateTime.Now.ToOADate();
                Console.WriteLine(tokenOptions);
                var token = tokenHandler.WriteToken(tokenOptions);

                return token;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        async Task<IEnumerable<IdentityRole>> IAuthService.GetRoleList()
        {
            IEnumerable<IdentityRole> resutl = await roleManager.Roles.ToListAsync();
            return resutl;
        }
    }
}

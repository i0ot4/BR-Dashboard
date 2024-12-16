using BR.helper;
using BR.Models;
using BR.Models.VMs;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.RegularExpressions;

namespace BR.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<SysUser> userManager;
        private readonly IAuthService authService;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ISessionHelper sessionHelper;
        private readonly IWebHostEnvironment env;
        private readonly ICityRepository cityRepository;
        private readonly IDirectorateRepository directorateRepository;
        private readonly INeighborhoodRepository neighborhoodRepository;
        private readonly ISysUserRepository sysUserRepository;
        public AccountController(
            IAuthService authService,
            ISessionHelper sessionHelper,
            IWebHostEnvironment env,
            ICityRepository cityRepository,
            IDirectorateRepository directorateRepository,
            INeighborhoodRepository neighborhoodRepository,
            ISysUserRepository sysUserRepository
            )
        {
            this.authService = authService;
            this.sessionHelper = sessionHelper;
            this.env = env;
            this.cityRepository = cityRepository;
            this.directorateRepository = directorateRepository;
            this.neighborhoodRepository = neighborhoodRepository;
            this.sysUserRepository = sysUserRepository;
        }

        [AllowAnonymous]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login, string? ReturnUrl)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(login);
                }
                login.RememberMe = false;
                var res = await authService.Login(login, true);
                if (res.Succeeded && res.Data != null)
                {
                    var user = res.Data.User;
                    if (user != null)
                    {
                        if (user.UserType == "Admin" || user.UserType == "SuperAdmin")
                        {
                            var id = user.Id;
                            //sessionHelper.AddData<string>("UserId", $"{id}");
                            //sessionHelper.AddData<SysUser>("user", user);
                            return RedirectToAction("index", "home");
                            return new RedirectToPageResult("/Home/Index");
                            
                            //if (string.IsNullOrEmpty(ReturnUrl))
                            //{
                            //    return RedirectToAction("index", "home");
                            //}
                            //else
                            //{
                            //    return Redirect(ReturnUrl);
                            //}

                        }

                        ModelState.AddModelError(string.Empty, "هذا الحساب لايمكنه تسجيل الدخول");
                        return View(login);
                    }
                    ModelState.AddModelError(string.Empty, "هذا الحساب يواجه مشكلة حاليا");
                    return View(login);
                }
                ModelState.AddModelError(string.Empty, res.Status.message);
                return View(login);
            }
            catch (Exception ex)
            {
                return View(login);
            }
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            return View();
        }

        public async Task<ActionResult> Logout()
        {
            var res = await authService.Logout();
            if (res.Succeeded)
            {
                var logout = sessionHelper.ClearSession();
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["error"] = $"{res.Status.message}";
                return RedirectToAction("Index", "Home");
            }

        }
        [Authorize]
        public async Task<ActionResult> ConfirmUser(string id, bool Confirm = true)
        {
            try
            {
                Console.WriteLine("=========== confirm user account =============");
                var upRes = await authService.ConfirmUser(id, true);
                if (upRes.Succeeded)
                {
                    TempData["msg"] = "تم تأكيد الحساب";
                    return RedirectToAction(nameof(Index));
                }
                TempData["error"] = upRes.Status.message;
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [Authorize]
        public async Task<ActionResult> ChangeIsActive(string id, bool isActive = false)
        {
            try
            {
                Console.WriteLine("=========== start change status =============");
                var upRes = await authService.ChangeActive(id, !isActive);
                if (upRes.Succeeded)
                {
                    TempData["msg"] = "تم تحديث البيانات";
                    return RedirectToAction(nameof(Index));
                }
                TempData["error"] = upRes.Status.message;
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [Authorize]
        public async Task<IActionResult> EditUsersInRole(string id)
        {
            ViewBag.roleId = id;

            var result = await authService.EditUsersInRole(id);
            if (result.Count > 0)
            {
                return View(result);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleVM> model, string id)
        {
            var resutl = await authService.EditUsersInRole(model, id);
            if (resutl)
            {
                return RedirectToAction("EditRole", new { Id = id });
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var result = await authService.EditRole(id);
            if (result == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            return View(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ListRoles()
        {
            var roles = await authService.GetRoleList();

            return View(roles.AsEnumerable());
        }
        [Authorize]
        [HttpGet]

        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await authService.CreateRole(model);
                if (result.Succeeded)
                {
                    return RedirectToAction("index", "home");

                }
                // We just need to specify a unique role name to create a new role




                ModelState.AddModelError("", result.Status.message);
                return View(model);


            }

            return View(model);
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult CreateUser()
        {
            SelectList usersTypes = new SelectList(UserTypes.getUserTypes());
            ViewData["userTypes"] = usersTypes;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserVM user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await Result<UserViewVM>.FailAsync($"يجب ادخال البيانات بشكل صحيح");
                    ModelState.AddModelError(string.Empty, "يجب ادخال البيانات بشكل صحيح");
                    SelectList usersTypes = new SelectList(UserTypes.getUserTypes());
                    ViewData["userTypes"] = usersTypes;
                    return View(user);
                }

                if (!IsOnlyNumbers(user.PhoneNumber))
                {
                    await Result<UserViewVM>.FailAsync($" الهاتف يجب ان يكون ارقام فقط");
                    ModelState.AddModelError(string.Empty, "يجب ادخال رقم هاتف صحيح");
                    SelectList usersTypes = new SelectList(UserTypes.getUserTypes());
                    ViewData["userTypes"] = usersTypes;
                    return View(user);
                }

                if (!IsComplexPassword(user.Password))
                {
                    await Result<UserViewVM>.FailAsync($"كلمة المرور يجب ان تحتوي على مزيج من حروف وارقام ");
                    ModelState.AddModelError(string.Empty, "يجب ادخال كلمة سر قوية");
                    SelectList usersTypes = new SelectList(UserTypes.getUserTypes());
                    ViewData["userTypes"] = usersTypes;
                    return View(user);
                }

                var res = await authService.CreateUser(user);
                if(res.Succeeded && res.Data != null)
                {
                    return RedirectToAction("index");
                }
                else
                {
                    
                    await Result<UserViewVM>.FailAsync($"خطأ!!");
                    ModelState.AddModelError(string.Empty, $"{res.Status.message}");
                    SelectList usersTypes = new SelectList(UserTypes.getUserTypes());
                    ViewData["userTypes"] = usersTypes;
                    return View(user);
                }
                
                

            }
            catch (Exception ex)
            {
                await Result<UserViewVM>.FailAsync($"======= Exp in create user: {ex.Message}");
                ModelState.AddModelError(string.Empty, $"======= Exp in create user: {ex.Message}");
                SelectList usersTypes = new SelectList(UserTypes.getUserTypes());
                ViewData["userTypes"] = usersTypes;
                return View(user);

            }
        }

        [Authorize]
        public async Task<IActionResult> ChangePassword(string userId)
        {
            return View();
            //var getItems = await sysUserRepository.GetOne(u => u.Id == userId && u.IsDeleted == false);
            //if (getItems.Succeeded && getItems.Data != null)
            //{
            //    var pass = new ChangePasswordDto
            //    {
            //        FullName = getItems.Data.FullName,
            //        PhoneNumber = getItems.Data.PhoneNumber,
            //        UserId = userId
            //    };
            //    return View(pass);
            //}
            //else
            //{
            //    TempData["error"] = $"{getItems.Status.message}";
            //    return RedirectToAction(nameof(Index));
            //}
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateContractor(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["error"] = "Oooops!";
                return View("NotFound");
            }
            
            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null)
                {
                    TempData["error"] = "Oooops!";

                    return View("NotFound");
                }
                ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                ViewData["neighborhoods"] = new SelectList(await neighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");

                ViewData["id"] = id;
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateContractor(CreateContractorVM user, IFormFile image, IFormFile commericalRegistrationImage, string id)
            {
            
                if(string.IsNullOrEmpty(id))
                {
                    await Result<CreateContractorVM>.FailAsync($"الحساب غير موجود");
                    ModelState.AddModelError(string.Empty, "الحساب غير موجود");
                    return View("NotFound");
                }
            
                if(commericalRegistrationImage == null)
                {
                    ModelState.AddModelError(string.Empty, "صورة السجل التجاري مطلوبة");
                    ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                    ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                    ViewData["neighborhoods"] = new SelectList(await neighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                    ViewData["id"] = id;
                    return View(user);
                }
                else
                {
                    if(user == null)
                    {
                        ModelState.AddModelError(string.Empty, "يرجى إدخال البيانات");
                        ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                        ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                        ViewData["neighborhoods"] = new SelectList(await neighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                        ViewData["id"] = id;
                        return View(user);
                    }
                    else
                    {
                        try
                        {

                            var userResult = sysUserRepository.GetById(id);
                            if(userResult == null)
                            {
                                await Result<CreateContractorVM>.FailAsync($"يرجى التحقق من المستخدم");
                                ModelState.AddModelError(string.Empty, "يجب ادخال البيانات بشكل صحيح");
                                ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                                ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                                ViewData["neighborhoods"] = new SelectList(await neighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                                ViewData["id"] = id;
                                return View(user);
                            }
                            if (!ModelState.IsValid)
                            {
                                await Result<CreateContractorVM>.FailAsync($"يجب ادخال البيانات بشكل صحيح");
                                ModelState.AddModelError(string.Empty, "يجب ادخال البيانات بشكل صحيح");
                                ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                                ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                                ViewData["neighborhoods"] = new SelectList(await neighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                                ViewData["id"] = id;
                                return View(user);
                            }

                            var userImage = "";
                            if (image != null)
                            {
                                userImage = await SaveFile(image, "usersImages");
                            }
                            var commericalImage = await SaveFile(commericalRegistrationImage, "usersImages");
                            if(string.IsNullOrEmpty(commericalImage)) {
                                await Result<CreateContractorVM>.FailAsync($"لم نستطيع حفظ صورة السجل التجاري ");
                                ModelState.AddModelError(string.Empty, "لم نستطيع حفظ صورة السجل التجاري");
                                ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                                ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                                ViewData["neighborhoods"] = new SelectList(await neighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                                ViewData["id"] = id;
                                return View(user);
                            }
                            user.Image = userImage;
                            user.CommericalRegistrationImage = commericalImage;
                            var result = await authService.UpdateContractorUser(user, id);
                            return View("Index");
                        }
                        catch (Exception ex)
                        {
                            await Result<UserViewVM>.FailAsync($"======= Exp in create user: {ex.Message}");
                            ModelState.AddModelError(string.Empty, $"======= Exp in create user: {ex.Message}");
                            ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                            ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                            ViewData["neighborhoods"] = new SelectList(await neighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                            ViewData["id"] = id;
                            return View(user);

                        }



                    }
                }
            }
        private bool IsOnlyNumbers(string input)
        {
            return Regex.IsMatch(input, @"^[0-9]+$");
        }
        
        private bool IsComplexPassword(string password)
        {
            bool containsNumbers = Regex.IsMatch(password, @"\d");
            bool containsChars = Regex.IsMatch(password, @"[a-zA-Z]");

            return containsNumbers && containsChars;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await sysUserRepository.GetAll(x=>x.IsConfirmed == true);
                if(result.Any() && result.Count() > 0)
                {
                    return View(result);
                }
                TempData["msg"] = "لا يوجد بيانات";
                return View();

            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View("index", "home");
            }
        }

        public async Task<string> SaveFile(IFormFile file, string savePath = "AllFiles", int fileType = 0)
        {
            try
            {
                string fname = "";
                string unqName = "";
                string ext = "";
                if (file != null && file.Length > 0)
                {
                    //string tempFileName = Path.GetTempFileName();
                    ext = Path.GetExtension(file.FileName);
                    string attFolder = Path.Combine(env.WebRootPath, savePath);
                    if (!Directory.Exists(attFolder))
                    {
                        Directory.CreateDirectory(attFolder);
                    }
                    unqName = string.Format("{0}{1}", DateTime.Now.ToString("ddmmyyhhmmssfff"), ext); //Guid.NewGuid().ToString().Substring(0, 10) + ext);

                    fname = Path.Combine(attFolder, unqName);
                    using (FileStream str = new FileStream(fname, FileMode.Create))
                    {
                        file.CopyTo(str);
                    }

                    return Path.Combine(savePath, unqName);
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        
    }
}

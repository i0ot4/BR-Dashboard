using BR.helper;
using BR.Models.VMs;
using BR.Repositories;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BR.Controllers
{
    [Authorize]
    public class ConstructionShopController : Controller
    {
        private readonly IWebHostEnvironment env;
        private readonly ISessionHelper sessionHelper;
        private readonly ISysUserRepository sysUserRepository;
        private readonly ICityRepository cityRepository;
        private readonly IDirectorateRepository directorateRepository;
        private readonly INeighborhoodRepository nighborhoodRepository;
        private readonly IConstructionShopRepository constructionShopRepository;

        public ConstructionShopController(
            IWebHostEnvironment env,
            ISessionHelper sessionHelper,
            ISysUserRepository sysUserRepository,
            ICityRepository cityRepository,
            IDirectorateRepository directorateRepository,
            INeighborhoodRepository neighborhoodRepository,
            IAuthService authService,
            IConstructionShopRepository constructionShopRepository
            )
        {
            this.env = env;
            this.sessionHelper = sessionHelper;
            this.sysUserRepository = sysUserRepository;
            this.cityRepository = cityRepository;
            this.directorateRepository = directorateRepository;
            this.nighborhoodRepository = neighborhoodRepository;
            this.constructionShopRepository = constructionShopRepository;
        }


        public async Task<IActionResult> Index()
        {
            var result = await sysUserRepository.GetAll(p => p.IsDeleted == false && p.UserType == UserTypes.ConstructionShop);
            if (result.Any() && result.Count() > 0)
            {
                return View(result);
            }
            return View();
        }

        public async Task<IActionResult> Create(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["error"] = "Oooops!";
                return View("NotFound");
            }

            try
            {
                var user = await sysUserRepository.GetById(id);
                if (user == null)
                {
                    TempData["error"] = "Oooops!";

                    return View("NotFound");
                }
                ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");

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
        public async Task<IActionResult> Create(CreateConstructionShopVM user, IFormFile? image, string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                await Result<CreateConstructionShopVM>.FailAsync($"الحساب غير موجود");
                ModelState.AddModelError(string.Empty, "الحساب غير موجود");
                return View("NotFound");
            }

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "يرجى إدخال البيانات");
                    ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                    ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                    ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                    ViewData["id"] = id;
                    return View(user);
                }
                else
                {
                    try
                    {

                        var userResult = sysUserRepository.GetById(id);
                        if (userResult == null)
                        {
                            await Result<CreateConstructionShopVM>.FailAsync($"يرجى التحقق من المستخدم");
                            ModelState.AddModelError(string.Empty, "يجب ادخال البيانات بشكل صحيح");
                            ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                            ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                            ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                            ViewData["id"] = id;
                            return View(user);
                        }
                        if (!ModelState.IsValid)
                        {
                            await Result<CreateConstructionShopVM>.FailAsync($"يجب ادخال البيانات بشكل صحيح");
                            ModelState.AddModelError(string.Empty, "يجب ادخال البيانات بشكل صحيح");
                            ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                            ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                            ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                            ViewData["id"] = id;
                            return View(user);
                        }

                        var userImage = "";
                        if (image != null)
                        {
                            userImage = await SaveFile(image, "usersImages");
                        }
                        
                        user.Image = userImage;
                        
                        var result = await constructionShopRepository.UpdateConstructionShopVM(user, id);
                    if(result.Succeeded && result.Data != null)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        await Result<CreateConstructionShopVM>.FailAsync($"المستخدم غير صحيح");
                        ModelState.AddModelError(string.Empty, "المستخدم غير صحيح");
                        ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                        ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                        ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                        ViewData["id"] = id;
                        return View(user);
                    }
                        
                    }
                    catch (Exception ex)
                    {
                        await Result<UserViewVM>.FailAsync($"======= Exp in create user: {ex.Message}");
                        ModelState.AddModelError(string.Empty, $"======= Exp in create user: {ex.Message}");
                        ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                        ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                        ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                        ViewData["id"] = id;
                        return View(user);

                    }

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

using BR.helper;
using BR.Models;
using BR.Models.VMs;
using BR.Repositories;
using BR.Repositories.IRepositories;
using BR.Services;
using BR.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BR.Controllers
{
    [Authorize]
    public class ContractorController : Controller
    {
        private readonly IWebHostEnvironment env;
        private readonly ISessionHelper sessionHelper;
        private readonly ISysUserRepository sysUserRepository;
        private readonly ICityRepository cityRepository;
        private readonly IDirectorateRepository directorateRepository;
        private readonly INeighborhoodRepository nighborhoodRepository;
        private readonly IAuthService authService;
        private readonly IContractorWorkerService contractorWorkerService;


        public ContractorController(
            IWebHostEnvironment env,
            ISessionHelper sessionHelper,
            ISysUserRepository sysUserRepository,
            ICityRepository cityRepository,
            IDirectorateRepository directorateRepository,
            INeighborhoodRepository neighborhoodRepository,
            IAuthService authService,
            IContractorWorkerService contractorWorkerService
            )
        {
            this.env = env;
            this.sessionHelper = sessionHelper;
            this.sysUserRepository = sysUserRepository;
            this.cityRepository = cityRepository;
            this.directorateRepository = directorateRepository;
            this.nighborhoodRepository = neighborhoodRepository;
            this.authService = authService;
            this.contractorWorkerService = contractorWorkerService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await sysUserRepository.GetAll(p => p.IsDeleted == false && p.UserType == UserTypes.Contractor);
            if (result.Any() && result.Count() > 0)
            {
                return View(result);
            }
            return View();
        }
        public async Task<IActionResult> ContractorWorkers(string id)
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
                ViewData["id"] = id;
                var result = await contractorWorkerService.GetAll(p => p.IsDeleted == false && p.SysUserId == id);
                if (result == null)
                {
                    return View();
                }
                if (result.Data.Any() && result.Data.Count() > 0)
                {
                    return View(result.Data);
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
                   }
        public async Task<IActionResult> CreateContractor(string id)
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

        public async Task<ActionResult> ChangeIsActiveWorker(long id, bool isActive = false)
        {
            try
            {
                Console.WriteLine("=========== start change status =============");
                var upRes = await contractorWorkerService.ChangeActive(id, !isActive);
                if (upRes.Succeeded)
                {
                    //TempData["msg"] = upRes.Status.message;
                    return RedirectToAction(nameof(Index));
                }
                //TempData["error"] = upRes.Status.message;
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateContractor(CreateContractorVM user, IFormFile? image, IFormFile commericalRegistrationImage, string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                await Result<CreateContractorVM>.FailAsync($"الحساب غير موجود");
                ModelState.AddModelError(string.Empty, "الحساب غير موجود");
                return View("NotFound");
            }

            if (commericalRegistrationImage == null)
            {
                ModelState.AddModelError(string.Empty, "صورة السجل التجاري مطلوبة");
                ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                ViewData["id"] = id;
                return View(user);
            }
            else
            {
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
                            await Result<CreateContractorVM>.FailAsync($"يرجى التحقق من المستخدم");
                            ModelState.AddModelError(string.Empty, "يجب ادخال البيانات بشكل صحيح");
                            ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                            ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                            ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                            ViewData["id"] = id;
                            return View(user);
                        }
                        if (!ModelState.IsValid)
                        {
                            await Result<CreateContractorVM>.FailAsync($"يجب ادخال البيانات بشكل صحيح");
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
                        var commericalImage = await SaveFile(commericalRegistrationImage, "usersImages");
                        if (string.IsNullOrEmpty(commericalImage))
                        {
                            await Result<CreateContractorVM>.FailAsync($"لم نستطيع حفظ صورة السجل التجاري ");
                            ModelState.AddModelError(string.Empty, "لم نستطيع حفظ صورة السجل التجاري");
                            ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                            ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                            ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                            ViewData["id"] = id;
                            return View(user);
                        }
                        user.Image = userImage;
                        user.CommericalRegistrationImage = commericalImage;
                        var result = await authService.UpdateContractorUser(user, id);
                        return RedirectToAction("Index");
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
        }

        public async Task<IActionResult> CreateContractorWorker(string id)
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
        public async Task<IActionResult> CreateContractorWorker(ContractorWorkers user, IFormFile? image , string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                await Result<ContractorWorkers>.FailAsync($"الحساب غير موجود");
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
                            await Result<ContractorWorkers>.FailAsync($"يرجى التحقق من المستخدم");
                            ModelState.AddModelError(string.Empty, "يجب ادخال البيانات بشكل صحيح");
                            ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                            ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                            ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                            ViewData["id"] = id;
                            return View(user);
                        }
                    user.SysUserId = id;
                        //if (!ModelState.IsValid)
                        //{
                        //    await Result<ContractorWorkers>.FailAsync($"يجب ادخال البيانات بشكل صحيح");
                        //    ModelState.AddModelError(string.Empty, "يجب ادخال البيانات بشكل صحيح");
                        //    ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted == false), "Id", "CityName");
                        //    ViewData["directorates"] = new SelectList(await directorateRepository.GetAll(p => p.IsDeleted == false), "Id", "DirectorateName");
                        //    ViewData["neighborhoods"] = new SelectList(await nighborhoodRepository.GetAll(p => p.IsDeleted == false), "Id", "NeighborhoodName");
                        //    ViewData["id"] = id;
                        //    return View(user);
                        //}

                        var userImage = "";
                        if (image != null)
                        {
                            userImage = await SaveFile(image, "usersImages");
                        }                        
                        user.Image = userImage;

                        var result = await contractorWorkerService.Add(user);
                        return RedirectToAction("Index");
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

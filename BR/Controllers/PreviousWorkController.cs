using BR.Models;
using BR.Models.VMs;
using BR.Repositories;
using BR.Repositories.IRepositories;
using BR.Services;
using BR.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BR.Controllers
{
    [Authorize]
    public class PreviousWorkController : Controller
    {
        private readonly IPreviousWorkService previousWorkService;
        private readonly IPreviousWorkImageService previousWorkImageService;
        private readonly IUnitOfWork unitOfWork;
        private readonly ISysUserRepository sysUserRepository;
        private readonly IWebHostEnvironment env;

        public PreviousWorkController(
            IPreviousWorkService previousWorkService,
            IPreviousWorkImageService previousWorkImageService,
            IUnitOfWork unitOfWork,
            ISysUserRepository sysUserRepository,
            IWebHostEnvironment env)
        {
            this.previousWorkService = previousWorkService;
            this.previousWorkImageService = previousWorkImageService;
            this.unitOfWork = unitOfWork;
            this.sysUserRepository = sysUserRepository;
            this.env = env;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                
                var getItems = await previousWorkService.GetAllWithOwner(p=>p.IsDeleted == false);

                if (getItems.Succeeded && getItems.Data != null)
                {
                    return View(getItems.Data);
                }
                TempData["error"] = $"{getItems.Status.message}";
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
        }

        public async Task<IActionResult> Add()
        {
            try
            {
                
                ViewData["users"] = new SelectList(await sysUserRepository.GetAll(p => p.IsDeleted == false && p.IsActive == true), "Id", "FullName");
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(PreviousWork obj)
        {
            try
            {
                if (obj != null)
                {
                    
                    if (obj.Title == null || obj.Title.Length == 0)
                    {
                        ViewData["users"] = new SelectList(await sysUserRepository.GetAll(p => p.IsDeleted == false && p.IsActive == true), "Id", "FullName");
                        return View(obj);
                    }
                    obj.IsDeleted = false;
                    var addItem = await previousWorkService.Add(obj);
                    if (addItem.Succeeded)
                    {
                        TempData["msg"] = "تمت الاضافة بنجاح";
                        return RedirectToAction(nameof(Index));
                    }
                    ViewData["users"] = new SelectList(await sysUserRepository.GetAll(p => p.IsDeleted == false && p.IsActive == true), "Id", "FullName");
                    TempData["error"] = $"{addItem.Status.message}";
                    return View(obj);
                }
                else
                {
                    TempData["error"] = "image can not be null";
                    return View(obj);
                }


            }
            catch (Exception exp)
            {
                TempData["error"] = $"{exp.Message}";
                return View(obj);
            }
        }


        public async Task<IActionResult> AddImage(long id)
        {
            if (id == 0)
            {
                TempData["error"] = "Oooops!";
                return View("NotFound");
            }

            try
            {
                
                var result = await previousWorkService.GetById(id);
                if (result == null)
                {
                    TempData["error"] = "Oooops!";

                    return View("NotFound");
                }
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
        public async Task<IActionResult> AddImage(PreviousWorkImage obj, IFormFile image, long id)
        {
            if(id == 0)
            {
                TempData["error"] = "Oooops!";
                return View();
            }
            if(image == null || obj == null)
            {
                TempData["error"] = "يرجى إختيار صورة";
                return View();
            }
            var imageresult = await SaveFile(image, "previousWorkImage");
            if (string.IsNullOrEmpty(imageresult))
            {
                TempData["error"] = "حدث خطأ وقت حفظ الصورة";
                return View();
            }
            var i = new PreviousWorkImage
            {
                ImagePath = imageresult,
                ImageTitle = obj.ImageTitle,
                IsDeleted = false,
                PreviousWorkId = id,

            };
            var addItem = await previousWorkImageService.Add(i);
            if (addItem.Succeeded)
            {
                TempData["msg"] = "تمت الاضافة بنجاح";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "error";
            return View(obj);

        }

        public async Task<IActionResult> GetWorkImages(long id)
        {
            try
            {
                var result = await previousWorkImageService.GetAll(p => p.PreviousWorkId == id);
                if (result.Succeeded)
                {
                    return View(result.Data);
                }
                else
                {
                    TempData["error"] = "خطا في الحصول على البيانات";
                    return View();
                }
            }catch (Exception ex)
            {
                TempData["error"] = "خطأ";
                return View();
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
        public async Task<ActionResult> ChangeIsActiveImage(long id, bool isActive = false)
        {
            try
            {
                Console.WriteLine("=========== start change status =============");
                var upRes = await previousWorkImageService.ChangeActive(id, !isActive);
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
        public async Task<ActionResult> ChangeIsActive(long id, bool isActive = false)
        {
            try
            {
                Console.WriteLine("=========== start change status =============");
                var upRes = await previousWorkService.ChangeActive(id, !isActive);
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
    }
}

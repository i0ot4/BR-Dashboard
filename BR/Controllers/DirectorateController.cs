using BR.Models;
using BR.Repositories.IRepositories;
using BR.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BR.Controllers
{
    [Authorize]
    public class DirectorateController : Controller
    {
        private readonly IDirectorateService directorateService;
        private readonly ICityRepository cityRepository;
        private readonly IUnitOfWork unitOfWork;
        public DirectorateController(IDirectorateService directorateService,ICityRepository cityRepository, IUnitOfWork unitOfWork)
        {
            this.directorateService = directorateService;
            this.cityRepository = cityRepository;
            this.unitOfWork = unitOfWork;
        }       

        public async Task<IActionResult> Index()
        {
            try
            {
                var getItems = await directorateService.GetAll(p => p.IsDeleted == false);
                if(getItems.Succeeded && getItems.Data != null)
                {
                    return View(getItems.Data);
                }
                TempData["error"] = $"{getItems.Status.message}";
                return View();
            }catch(Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
            
        }

        

        public async Task<IActionResult> Add()
        {
            try
            {

                ViewData["cities"] = new SelectList(await cityRepository.GetAll(p=>p.IsDeleted == false), "Id","CityName");                
                return View();
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(Directorate obj)
        {
            try
            {
                if (obj != null)
                {
                    
                    obj.IsDeleted = false;
                    if(obj.DirectorateName == null ||  obj.DirectorateName.Length == 0)
                    {
                        ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted != false), "Id", "CityName");
                        return View(obj);
                    }
                    
                    var addItem = await directorateService.Add(obj);
                    if (addItem.Succeeded)
                    {
                        TempData["msg"] = "تمت الاضافة بنجاح";
                        return RedirectToAction(nameof(Index));
                    }
                    TempData["error"] = $"{addItem.Status.message}";
                    ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted != false), "Id", "CityName");
                    return View(obj);
                }
                else
                {
                    ViewData["cities"] = new SelectList(await cityRepository.GetAll(p => p.IsDeleted != false), "Id", "CityName");
                    TempData["error"] = "not be null";
                    return View(obj);
                }


            }
            catch (Exception exp)
            {
                TempData["error"] = $"{exp.Message}";
                return View(obj);
            }
        }

        public async Task<ActionResult> ChangeIsActive(long id, bool isActive = false)
        {
            try
            {
                Console.WriteLine("=========== start change status =============");
                var upRes = await directorateService.ChangeActive(id, !isActive);
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(long id)
        {

            try
            {
                if (id == 0)
                {
                    TempData["error"] = $"يرجى اختيار عنصر صحيحة لتعديلها";
                    return RedirectToAction(nameof(Index));
                }
                var getItem = await directorateService.GetForUpdate(id);
                if (getItem.Succeeded && getItem.Data != null)
                {
                    return View(getItem.Data);
                }

                TempData["error"] = $"يرجى اختيار عنصر صحيحة لتعديلها";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"{ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Directorate obj)
        {
            try
            {

                if (obj == null)
                {
                    return View(obj);
                }
                if(obj.DirectorateName == null || obj.DirectorateName.Length == 0)
                {
                    return View(obj);
                }
                var editItem = await directorateService.Update(obj);
                if (editItem.Succeeded)
                {
                    TempData["msg"] = "تمت العملية بنجاح";
                    return RedirectToAction(nameof(Index));
                }

                TempData["error"] = $"{editItem.Status.message}";
                return View(obj);

            }
            catch (Exception exp)
            {
                TempData["error"] = $"{exp.Message}";
                return View(obj);
            }
        }
    }
}
